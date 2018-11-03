namespace TramTimes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using QuickType;

    class Program
    {
        static string MainPath;

        static void Main(string[] args)
        {
            foreach(var arg in args)
            {
                Console.WriteLine(arg);
            }

            MainPath = args[0];
            int intervalSeconds = int.Parse(args[1]);
            DownloadAndSaveOnce(null, null);
            var timer = new System.Timers.Timer(intervalSeconds * 1000) { AutoReset = true,};
            timer.Elapsed += DownloadAndSaveOnce;
            timer.Start();
            Console.CancelKeyPress += ExitApp;
            while (true)
            {
                Console.ReadKey();
            }
        }

        private static void ExitApp(object sender, ConsoleCancelEventArgs e) => Environment.Exit(0);

        private static async void DownloadAndSaveOnce(object sender, System.Timers.ElapsedEventArgs e)
        {
            var ticks = DateTime.Now.Ticks;
            var directory = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(MainPath, $"time_{ticks}"));
            var (stops, stopsJson) = await GetStopsAsync();
            if(stops == null)
            {
                await LogError($"Couldn't download stops, cancelling download for tick {ticks}");
                return;
            }
            var stopPassages = (await GetStopPassages(stops.Stops)).Where(s => s.stops != null).ToList();
            var (vehicles, vehiclesJson) = await GetVehicles();
            if (vehicles == null)
            {
                await LogError($"Couldn't download vehicles, cancelling download for tick {ticks}");
                return;
            }
            var tripPassages = (await GetTripPassages(vehicles.Vehicles.Where(v => !string.IsNullOrWhiteSpace(v.TripId) && !string.IsNullOrWhiteSpace(v.Id)).ToList()));
            var routeIds = stopPassages.Select(s => s.stops).SelectMany(s => s.Routes).Select(s => s.Id).Distinct();
            var routes = (await GetRouteStops(routeIds)).Where(r => r.routeInfos != null);

            var stopPassagesDirectory = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(directory.FullName, "stopPassages"));
            var tripPassagesDirectory = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(directory.FullName, "tripPassages"));
            var routeStopsDirectory = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(directory.FullName, "routeStops"));

            await System.IO.File.WriteAllTextAsync(System.IO.Path.Combine(directory.FullName, $"stops.json"), stopsJson);
            await System.IO.File.WriteAllTextAsync(System.IO.Path.Combine(directory.FullName, $"vehicles.json"), vehiclesJson);

            foreach(var (stopId, _, json) in stopPassages)
            {
                await System.IO.File.WriteAllTextAsync(System.IO.Path.Combine(stopPassagesDirectory.FullName, $"stopPassages_{stopId}.json"), json);
            }

            foreach(var tripPassage in tripPassages)
            {
                await System.IO.File.WriteAllTextAsync(System.IO.Path.Combine(tripPassagesDirectory.FullName, $"tripPassages_{tripPassage.Key.tripId}_{tripPassage.Key.vehicleId}.json"), tripPassage.Value.json);
            }

            foreach(var (routeId, _, json) in routes)
            {
                await System.IO.File.WriteAllTextAsync(System.IO.Path.Combine(routeStopsDirectory.FullName, $"routeStops_{routeId}.json"), json);
            }

            ////example
            //var trainStation = stops.Stops.FirstOrDefault(s => s.ShortName == 2690);
            //var trainStationStopPassages = stopPassages.Select(s => s.stops)
            //    .Where(s => s.StopShortName == 2690)
            //    .SelectMany(t => t.Actual)
            //    .Select(a => (a.TripId, a.VehicleId));

            //var trainStationTripPassages = tripPassages.Where(t => trainStationStopPassages.Contains(t.Key));
        }

        private static async Task<Dictionary<(string tripId, string vehicleId), (TripInfoResponse deserialized, string json)>> GetTripPassages(List<Vehicle> vehicles)
        {
            var tripInfoResponses = await Task.WhenAll(from vehicle in vehicles select GetTripPassages(vehicle.TripId, vehicle.Id));

            return tripInfoResponses.Where(r => r.tripInfoResponse != null).ToDictionary(k => (k.tripId, k.vehicleId), k => (k.tripInfoResponse, k.json));
        }

        private static async Task<IEnumerable<(string stopId, StopPassagesResponse stops, string json)>> GetStopPassages(IEnumerable<StopElement> stops) 
            => await Task.WhenAll(from stop in stops select GetStopPassages(stop.ShortName.ToString()));

        static async Task<(VehicleResponse vehicles, string json)> GetVehicles() 
            => await GetAsync<VehicleResponse>("http://www.ttss.krakow.pl/internetservice/geoserviceDispatcher/services/vehicleinfo/vehicles");

        static async Task<(StopsResponse stops, string json)> GetStopsAsync() 
            => await GetAsync<StopsResponse>("http://www.ttss.krakow.pl/internetservice/geoserviceDispatcher/services/stopinfo/stops?left=-648000000&bottom=-324000000&right=648000000&top=324000000");

        static async Task<(string stopId, StopPassagesResponse stopPassages, string json)> GetStopPassages(string stopId)
        {
            var (deserialized, rawJson) = await GetAsync<StopPassagesResponse>($"http://www.ttss.krakow.pl/internetservice/services/passageInfo/stopPassages/stop?stop={stopId}");
            return (stopId, deserialized, rawJson);
        }

        static async Task<(string tripId, string vehicleId, TripInfoResponse tripInfoResponse, string json)> GetTripPassages(string tripId, string vehicleId)
        {
            var (deserialized, rawJson) = await GetAsync<TripInfoResponse>($"http://www.ttss.krakow.pl/internetservice/services/tripInfo/tripPassages?tripId={tripId}&vehicleId={vehicleId}");
            return (tripId, vehicleId, deserialized, rawJson);
        }

        private static async Task<IEnumerable<(string routeId, RouteInfoResponse routeInfos, string json)>> GetRouteStops(IEnumerable<string> routeIds)
            => await Task.WhenAll(from routeId in routeIds select GetRouteInfoResponse(routeId));

        static async Task<(string routeId, RouteInfoResponse routeInfoResponse, string json)> GetRouteInfoResponse(string routeId)
        {
            var (deserialized, rawJson) = await GetAsync<RouteInfoResponse>($"http://www.ttss.krakow.pl/internetservice/services/routeInfo/routeStops?routeId={routeId}");
            return (routeId, deserialized, rawJson);
        }
        
        static async Task<(T deserialized, string rawJson)> GetAsync<T>(string path)
        {
            int retries = 0;
            while(true)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(path);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return (JsonConvert.DeserializeObject<T>(json, QuickType.Converter.Settings), json);
                    }
                    else
                    {
                        throw new ApplicationException($"{path} failed, status code: {response.StatusCode}");
                    }
                }
                catch (Exception e)
                {
                    retries++;
                    var message = $"{path}{Environment.NewLine}{e.Message}";
                    Console.WriteLine(message);
                    if(retries > 20)
                    {
                        await LogError($"Request failed 20 times: {Environment.NewLine} {message}");
                        return (default(T), null);
                    }
                }
            }
        }

        private static async Task LogError(string message)
        {
            await System.IO.File.AppendAllTextAsync(System.IO.Path.Combine(MainPath, $"error.log"), $"{message} {Environment.NewLine}{Environment.NewLine}");
        }
    }
}

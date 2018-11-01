// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do one of these:
//
//    using QuickType;
//
//    var routeInfoResponse = RouteInfoResponse.FromJson(jsonString);
//    var stopPassagesResponse = StopPassagesResponse.FromJson(jsonString);
//    var stopsResponse = StopsResponse.FromJson(jsonString);
//    var tripPassagesResponse = TripPassagesResponse.FromJson(jsonString);
//    var tripInfoResponse = TripInfoResponse.FromJson(jsonString);
//    var vehicleResponse = VehicleResponse.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class RouteInfoResponse
    {
        [JsonProperty("route")]
        public Route Route { get; set; }

        [JsonProperty("stops")]
        public RouteInfoResponseStop[] Stops { get; set; }
    }

    public partial class StopPassagesResponse
    {
        [JsonProperty("actual")]
        public StopPassagesResponseActual[] Actual { get; set; }

        [JsonProperty("directions")]
        public object[] Directions { get; set; }

        [JsonProperty("firstPassageTime")]
        public long FirstPassageTime { get; set; }

        [JsonProperty("generalAlerts")]
        public object[] GeneralAlerts { get; set; }

        [JsonProperty("lastPassageTime")]
        public long LastPassageTime { get; set; }

        [JsonProperty("old")]
        public StopPassagesResponseActual[] Old { get; set; }

        [JsonProperty("routes")]
        public Route[] Routes { get; set; }

        [JsonProperty("stopName")]
        public string StopName { get; set; }

        [JsonProperty("stopShortName")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long StopShortName { get; set; }
    }

    public partial class RouteInfoResponseStop
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("number")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Number { get; set; }
    }

    public partial class StopPassagesResponseActual
    {
        [JsonProperty("actualRelativeTime")]
        public long ActualRelativeTime { get; set; }

        [JsonProperty("actualTime", NullValueHandling = NullValueHandling.Ignore)]
        public string ActualTime { get; set; }

        [JsonProperty("direction")]
        public string Direction { get; set; }

        [JsonProperty("mixedTime")]
        public string MixedTime { get; set; }

        [JsonProperty("passageid")]
        public string Passageid { get; set; }

        [JsonProperty("patternText")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long PatternText { get; set; }

        [JsonProperty("plannedTime")]
        public string PlannedTime { get; set; }

        [JsonProperty("routeId")]
        public string RouteId { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("tripId")]
        public string TripId { get; set; }

        [JsonProperty("vehicleId", NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleId { get; set; }
    }

    public partial class Route
    {
        [JsonProperty("alerts")]
        public object[] Alerts { get; set; }

        [JsonProperty("authority")]
        public string Authority { get; set; }

        [JsonProperty("directions")]
        public string[] Directions { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Name { get; set; }

        [JsonProperty("routeType")]
        public RouteType RouteType { get; set; }

        [JsonProperty("shortName")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ShortName { get; set; }
    }

    public partial class StopsResponse
    {
        [JsonProperty("stops")]
        public StopElement[] Stops { get; set; }
    }

    public partial class StopElement
    {
        [JsonProperty("category")]
        public RouteType Category { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("latitude")]
        public long Latitude { get; set; }

        [JsonProperty("longitude")]
        public long Longitude { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shortName")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ShortName { get; set; }
    }

    public partial class TripInfoResponse
    {
        [JsonProperty("actual")]
        public TripPassagesResponseActual[] Actual { get; set; }

        [JsonProperty("directionText")]
        public string DirectionText { get; set; }

        [JsonProperty("old")]
        public TripPassagesResponseActual[] Old { get; set; }

        [JsonProperty("routeName")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long RouteName { get; set; }
    }

    public partial class TripPassagesResponseActual
    {
        [JsonProperty("actualTime")]
        public string ActualTime { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("stop")]
        public ActualStop Stop { get; set; }

        [JsonProperty("stop_seq_num")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long StopSeqNum { get; set; }
    }

    public partial class ActualStop
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shortName")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ShortName { get; set; }
    }

    public partial class VehicleResponse
    {
        [JsonProperty("lastUpdate")]
        public long LastUpdate { get; set; }

        [JsonProperty("vehicles")]
        public Vehicle[] Vehicles { get; set; }
    }

    public partial class Vehicle
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("isDeleted", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsDeleted { get; set; }

        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public RouteType? Category { get; set; }

        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public Color? Color { get; set; }

        [JsonProperty("tripId", NullValueHandling = NullValueHandling.Ignore)]
        public string TripId { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public Path[] Path { get; set; }

        [JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
        public long? Longitude { get; set; }

        [JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
        public long? Latitude { get; set; }

        [JsonProperty("heading", NullValueHandling = NullValueHandling.Ignore)]
        public long? Heading { get; set; }
    }

    public partial class Path
    {
        [JsonProperty("length")]
        public double Length { get; set; }

        [JsonProperty("y1")]
        public long Y1 { get; set; }

        [JsonProperty("y2")]
        public long Y2 { get; set; }

        [JsonProperty("x2")]
        public long X2 { get; set; }

        [JsonProperty("angle")]
        public long Angle { get; set; }

        [JsonProperty("x1")]
        public long X1 { get; set; }
    }

    public enum Status { Departed, Planned, Predicted, Stopping };

    public enum RouteType { Other, Tram };

    public enum Color { The0X000000 };

    public partial class StopPassagesResponse
    {
        public static StopPassagesResponse FromJson(string json) => JsonConvert.DeserializeObject<StopPassagesResponse>(json, QuickType.Converter.Settings);
    }

    public partial class StopsResponse
    {
        public static StopsResponse FromJson(string json) => JsonConvert.DeserializeObject<StopsResponse>(json, QuickType.Converter.Settings);
    }

    public partial class TripPassagesResponse
    {
        public static TripInfoResponse FromJson(string json) => JsonConvert.DeserializeObject<TripInfoResponse>(json, QuickType.Converter.Settings);
    }

    public partial class TripInfoResponse
    {
        public static TripInfoResponse FromJson(string json) => JsonConvert.DeserializeObject<TripInfoResponse>(json, QuickType.Converter.Settings);
    }

    public partial class VehicleResponse
    {
        public static VehicleResponse FromJson(string json) => JsonConvert.DeserializeObject<VehicleResponse>(json, QuickType.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this StopPassagesResponse self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
        public static string ToJson(this StopsResponse self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
        public static string ToJson(this TripInfoResponse self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
        public static string ToJson(this VehicleResponse self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                StatusConverter.Singleton,
                RouteTypeConverter.Singleton,
                ColorConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class StatusConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Status) || t == typeof(Status?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "DEPARTED":
                    return Status.Departed;
                case "PLANNED":
                    return Status.Planned;
                case "PREDICTED":
                    return Status.Predicted;
                case "STOPPING":
                    return Status.Stopping;
            }
            throw new Exception("Cannot unmarshal type Status");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Status)untypedValue;
            switch (value)
            {
                case Status.Departed:
                    serializer.Serialize(writer, "DEPARTED");
                    return;
                case Status.Planned:
                    serializer.Serialize(writer, "PLANNED");
                    return;
                case Status.Predicted:
                    serializer.Serialize(writer, "PREDICTED");
                    return;
                case Status.Stopping:
                    serializer.Serialize(writer, "STOPPING");
                    return;
            }
            throw new Exception("Cannot marshal type Status");
        }

        public static readonly StatusConverter Singleton = new StatusConverter();
    }

    internal class RouteTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(RouteType) || t == typeof(RouteType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "other":
                    return RouteType.Other;
                case "tram":
                    return RouteType.Tram;
            }
            throw new Exception("Cannot unmarshal type RouteType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (RouteType)untypedValue;
            switch (value)
            {
                case RouteType.Other:
                    serializer.Serialize(writer, "other");
                    return;
                case RouteType.Tram:
                    serializer.Serialize(writer, "tram");
                    return;
            }
            throw new Exception("Cannot marshal type RouteType");
        }

        public static readonly RouteTypeConverter Singleton = new RouteTypeConverter();
    }

    internal class ColorConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Color) || t == typeof(Color?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "0x000000")
            {
                return Color.The0X000000;
            }
            throw new Exception("Cannot unmarshal type Color");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Color)untypedValue;
            if (value == Color.The0X000000)
            {
                serializer.Serialize(writer, "0x000000");
                return;
            }
            throw new Exception("Cannot marshal type Color");
        }

        public static readonly ColorConverter Singleton = new ColorConverter();
    }
}

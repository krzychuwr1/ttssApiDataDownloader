import asyncio
import json
import logging
import os
from collections import defaultdict
from concurrent.futures import ThreadPoolExecutor
from timeit import default_timer
from datetime import datetime

import requests

def ticks(dt):
    return (dt - datetime(1, 1, 1)).total_seconds() * 10000000

main_dir = '/media/toshiba/ed/data/time_%s' % int(ticks(datetime.utcnow()))
# main_dir = '/home/karol/ed/data/time_%s' % int(ticks(datetime.utcnow()))

def get_all_route_ids():
    filenames = os.listdir(os.path.join(main_dir, 'stopPassages'))
    routes = defaultdict(lambda: 0)
    for filename in filenames:
        with open(os.path.join(main_dir, 'stopPassages', filename)) as f:
            data = json.load(f)
            for r in data['routes']:
                routes[r['id']] += 1
    return routes.keys()


def fetch(session, url, filename):
    with session.get(url) as response:
        data = response.text
        if response.status_code != 200:
            logging.error("FAILURE::{0}".format(url))
        else:
            with open(filename, 'w') as f:
                f.write(data)
        return data


async def get_data_asynchronous(url_filenames):
    with ThreadPoolExecutor(max_workers=8) as executor:
        with requests.Session() as session:
            loop = asyncio.get_event_loop()
            tasks = [
                loop.run_in_executor(
                    executor,
                    fetch,
                    *(session, url, filename)  # Allows us to pass in multiple arguments to `fetch`
                )
                for (url, filename) in url_filenames
            ]
            for response in await asyncio.gather(*tasks):
                pass

def main():
    for _ in range(0, 10):
        try:
            logging.basicConfig(
                format='[%(levelname)s][%(asctime)s] %(message)s',
                filename='main.log',
                level=logging.INFO)

            os.mkdir(main_dir)
            os.mkdir(os.path.join(main_dir, 'stopPassages'))
            os.mkdir(os.path.join(main_dir, 'tripPassages'))
            os.mkdir(os.path.join(main_dir, 'routeStops'))
            START_TIME = default_timer()

            stops = json.loads(requests.get('http://www.ttss.krakow.pl/internetservice/geoserviceDispatcher/services/stopinfo/stops?left=-648000000&bottom=-324000000&right=648000000&top=324000000').content)
            with open(os.path.join(main_dir, 'stops.json'), 'w') as f:
                f.write(json.dumps(stops))
            stops = stops['stops']
            print("After stops time: %s" % (default_timer() - START_TIME))

            vehicles = json.loads(requests.get('http://www.ttss.krakow.pl/internetservice/geoserviceDispatcher/services/vehicleinfo/vehicles').content)
            with open(os.path.join(main_dir, 'vehicles.json'), 'w') as f:
                f.write(json.dumps(vehicles))
            vehicles = vehicles['vehicles']
            vehicles = [v for v in vehicles if 'isDeleted' not in v or v['isDeleted'] == False]
            print("After vehicles time: %s" % (default_timer() - START_TIME))

            stopPassagesUrlFilenames = [
                ('http://www.ttss.krakow.pl/internetservice/services/passageInfo/stopPassages/stop?stop=%s' % s['shortName'],
                 os.path.join(main_dir, 'stopPassages', 'stopPassages_%s.json' % s['shortName'])) for s in stops]
            loop = asyncio.get_event_loop()
            future = asyncio.ensure_future(get_data_asynchronous(stopPassagesUrlFilenames))
            loop.run_until_complete(future)
            print("After stopPassages time: %s" % (default_timer() - START_TIME))

            tripPassagesUrlFilenames = [
                ('http://www.ttss.krakow.pl/internetservice/services/tripInfo/tripPassages?tripId=%s&vehicleId=%s' % (v['tripId'], v['id']),
                 os.path.join(main_dir, 'tripPassages', 'tripPassages_%s_%s.json' % (v['tripId'], v['id']))) for v in vehicles]
            loop = asyncio.get_event_loop()
            future = asyncio.ensure_future(get_data_asynchronous(tripPassagesUrlFilenames))
            loop.run_until_complete(future)
            print("After tripPassages time: %s" % (default_timer() - START_TIME))

            routeStopsUrlFilenames = [
                ('http://www.ttss.krakow.pl/internetservice/services/routeInfo/routeStops?routeId=%s' % id,
                 os.path.join(main_dir, 'routeStops', 'routeStops_%s.json' % id)) for id in get_all_route_ids()]
            loop = asyncio.get_event_loop()
            future = asyncio.ensure_future(get_data_asynchronous(routeStopsUrlFilenames))
            loop.run_until_complete(future)
            print("After routeStops time: %s" % (default_timer() - START_TIME))
            logging.info('Elapsed time = %s' % (default_timer() - START_TIME))
        except Exception as e:
            logging.error(e)
        else:
            break


main()


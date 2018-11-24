import os
import json
import pandas as pd
from time import time
from datetime import datetime, timedelta

start_time = time()

root_dir = '/home/karol/ed/data'

time_dir_names = list(sorted(os.listdir(root_dir)))

print('Times to process = ', len(time_dir_names))

prev_stops_count = 5

idx = 0
nr = 0

time_dir_name_chunks = [time_dir_names[x:x+10000] for x in range(0, len(time_dir_names), 10000)]
for time_dir_name_chunk in time_dir_name_chunks:
    tram_delays = []
    for time_dir_name in time_dir_name_chunk:
        dir_dt = datetime(1, 1, 1) + timedelta(microseconds=int(time_dir_name.split('_')[-1])/10)
        idx = idx + 1
        if idx % int(len(time_dir_names)/100) == 1:
            print(100 * idx / len(time_dir_names), '%')
            print('Elapsed time = ', time() - start_time)
        try:
            stopPassages_path = os.path.join(root_dir, time_dir_name, 'stopPassages')
            stopPassages_file_names = os.listdir(stopPassages_path)
            for file_name in stopPassages_file_names:
                try:
                    file_path = os.path.join(stopPassages_path, file_name)
                    with open(file_path, encoding="utf8") as f:
                        stopPassage_dict = json.load(f)
                    stopName = stopPassage_dict['stopName']
                    stoppings = [s for s in stopPassage_dict['actual'] if s['status'] == 'STOPPING']
                    departeds = [s for s in stopPassage_dict['old'] if s['status'] == 'DEPARTED']
                    for stopping in stoppings + departeds:
                        try:
                            # WE NEED STOPS IN RIGHT ORDER
                            # with open(os.path.join(root_dir, time_dir_name, 'routeStops', 'routeStops_%s.json' % stopping['routeId']), encoding="utf8") as f:
                            #     route_configs = json.load(f)
                            # sign = 1 if route_configs['route']['directions'][0] == stopping['direction'] else -1
                            # route_stops = []
                            # idx = [i for i in range(0, len(route_configs['stops'])) if route_configs['stops'][i]['name'] == stopPassage_dict['stopName']][0]
                            # for i in range(idx, idx + prev_stops_count * sign, sign):
                            #     route_stops.append(route_configs['stops'][i % (len(route_configs['stops']) - 1)])
                            actual_dt = dir_dt + timedelta(hours=1, seconds=stopping['actualRelativeTime'])
                            planned_dt = datetime.strptime(stopping['plannedTime'], '%H:%M')
                            planned_dt = actual_dt.replace(hour=planned_dt.hour, minute=planned_dt.minute, second=planned_dt.second)
                            if planned_dt > actual_dt + timedelta(hours=15):
                                planned_dt = planned_dt + timedelta(days=-1)
                            elif planned_dt < actual_dt + timedelta(hours=-15):
                                planned_dt = planned_dt + timedelta(days=1)
                            delay = int((actual_dt - planned_dt).total_seconds() / 60)
                            tram_delays.append({'vehicleId': stopping['vehicleId'],
                                                'direction': stopping['direction'],
                                                'patternText': stopping['patternText'],
                                                'tripId': stopping['tripId'],
                                                'stopName': stopName,
                                                'delay': delay,
                                                'time': actual_dt,
                                                'day_of_week': dir_dt.weekday()
                                                }
                                               # .update({('prevStop%s' % i): v for (i, v) in enumerate(route_stops)})
                                               )
                        except Exception as e:
                            print(e)
                except Exception as e:
                    print(e)
        except Exception as e:
            print(e)

    tram_delays_df = pd.DataFrame(tram_delays, columns=['vehicleId', 'direction', 'patternText', 'tripId', 'stopName', 'time', 'day_of_week', 'delay', ])

    tram_delays_df.to_csv(os.path.join(os.path.dirname(root_dir), 'tram_delays_%s.csv' % nr))
    nr += 1

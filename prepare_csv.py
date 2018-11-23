import os
import json
import pandas as pd
from datetime import datetime
from time import time
from datetime import datetime, timedelta

start_time = time()

root_dir = '/media/toshiba/ed/data'

tram_delays = []

time_dir_names = os.listdir(root_dir)

print('Times to process = ', len(time_dir_names))

idx = 0
for time_dir_name in time_dir_names:
    dir_dt = datetime(1, 1, 1) + timedelta(microseconds=int(time_dir_name.split('_')[-1])/10)
    idx = idx + 1
    if idx % (len(time_dir_names)/1000) == 1:
        print(100 * idx / len(time_dir_names), '%')
    try:
        stopPassages_path = os.path.join(root_dir, time_dir_name, 'stopPassages')
        stopPassages_file_names = os.listdir(stopPassages_path)
        for file_name in stopPassages_file_names:
            try:
                file_path = os.path.join(stopPassages_path, file_name)
                with open(file_path, encoding="utf8") as f:
                    file_content = f.read()
                    stopPassage_dict = json.loads(file_content)
                    stopName = stopPassage_dict['stopName']
                    stoppings = [s for s in stopPassage_dict['actual'] if s['status'] == 'STOPPING']
                    for stopping in stoppings:
                        try:
                            actual_dt = datetime.strptime(stopping['actualTime'], '%H:%M')
                            planned_dt = datetime.strptime(stopping['plannedTime'], '%H:%M')
                            tram_delays.append({'vehicleId': stopping['vehicleId'],
                                                'direction': stopping['direction'],
                                                'patternText': stopping['patternText'],
                                                'tripId': stopping['tripId'],
                                                'stopName': stopName,
                                                'delay': int((actual_dt - planned_dt).seconds / 60),
                                                'time': actual_dt.time(),
                                                'day_of_week': dir_dt.weekday()})
                        except Exception as e:
                            print(e)
            except Exception as e:
                print(e)
    except Exception as e:
        print(e)

tram_delays_df = pd.DataFrame(tram_delays, columns=['vehicleId', 'direction', 'patternText', 'tripId', 'stopName', 'time', 'day_of_week', 'delay', ])
       
tram_delays_df.to_csv(os.path.join(os.path.dir_name(root_dir), 'tram_delays.csv'))

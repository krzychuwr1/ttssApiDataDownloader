import os
import json
import matplotlib.pyplot as plt
import pandas as pd
from datetime import datetime
from time import time

start_time = time()

root_dir = '/home/karol/ed/data'

tram_delays = []

time_dir_names = os.listdir(root_dir)

print('Times to process = ', len(time_dir_names))

for time_dir_name in time_dir_names:
    try:
        stopPassages_path = os.path.join(root_dir, time_dir_name, 'stopPassages')
        stopPassages_file_names = os.listdir(stopPassages_path)
        for file_name in stopPassages_file_names:
            try:
                file_path = os.path.join(stopPassages_path, file_name)
                with open(file_path) as f:
                    file_content = f.read()
                    stopPassage_dict = json.loads(file_content)
                    stoppings = [s for s in stopPassage_dict['actual'] if s['status'] == 'STOPPING']
                    for stopping in stoppings:
                        try:
                            actual_dt = datetime.strptime(stopping['actualTime'], '%H:%M')
                            planned_dt = datetime.strptime(stopping['plannedTime'], '%H:%M')
                            tram_delays.append({'vehicleId': stopping['vehicleId'],
                                                'delay': int((actual_dt - planned_dt).seconds / 60),
                                                'time': actual_dt.time()})
                        except Exception as e:
                            print(e)
            except Exception as e:
                print(e)
    except Exception as e:
        print(e)

tram_delays = pd.DataFrame(tram_delays, columns=['vehicleId', 'time', 'delay'])

tram_delays.to_csv('tram_delays.csv')
tram_delays = pd.DataFrame.from_csv('tram_delays.csv')

print(tram_delays.head(10).to_string())

selected_trams = tram_delays.query('vehicleId in ["6352185295672181187", "6352185295672181367", "6352185295672181042", "6352185295672181169"]')

print('Processing time = ', time() - start_time)

fig, ax = plt.subplots(4)

idx = 0
for key, grp in selected_trams.groupby(['vehicleId']):
    grp.plot(ax=ax[idx], style='.-', x='time', y='delay', color='b', label=key)
    idx += 1

print('Total time = ', time() - start_time)
plt.show()

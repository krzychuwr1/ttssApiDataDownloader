#!/bin/bash

# Remember to add your key to jabba (ssh connection without password is needed)
ssh -tt -L 8080:www.ttss.krakow.pl:80 kgongola@jabba.icsr.agh.edu.pl &
sleep 3
cd /home/karol/ed/ttssApiDataDownloader/TramTimes
dotnet run -- "/media/toshiba/ed/data/" 60

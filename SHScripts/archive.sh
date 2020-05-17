#!/bin/bash
7z a -t7z $1.7z -xr!node_modules -x!Pristine -x!obj -x!Temp -x!Build -x!Library
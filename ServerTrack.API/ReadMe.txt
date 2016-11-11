Data and testing entry points

Load test data, stricly for testing
<webserver>/servertrack/loaddata
http://localhost:50547/servertrack/loaddata

View test data, strictly for testing
<webserver>/servertrack/all/<servername>
http://localhost:50547/servertrack/loaddata



API entry points for actual use

Record Data GET, simplified way to add data for testing
<webserver>/servertrack/servername/cpuLoad/ramLoad
http://localhost:50547/servertrack/recordload/server-three/56/345

Record Data POST
<webserver>/servertrack/recordload
POST elements
servername
cpuLoad
ramLoad
http://localhost:50547/servertrack/recordload


See Server History
<webserver>/servertrack/serverhistory/<servername>/segment <hour | h | minute | m>
Group by the hour for the last 24 hours
http://localhost:50547/servertrack/serverhistory/server-one/h
http://localhost:50547/servertrack/serverhistory/server-one/hour

Group by the minute for the last 60 minutes
http://localhost:50547/servertrack/serverhistory/server-one/m
http://localhost:50547/servertrack/serverhistory/server-one/minute




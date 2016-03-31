create buffer states(state string)

insert into states
select 'TX'

insert into states
select 'AR'

insert into states
select 'OR'

create buffer station(url string, st string, city string, state string)

insert into station
select
	pick 'td:nth-child(1) a' take attribute 'href', --link to details
	pick 'td:nth-child(1) a', --station
	pick 'td:nth-child(2)', --city
	pick 'td:nth-child(4)' --state
from download page (select
	'https://www.faa.gov/air_traffic/weather/asos/?state=' + state
	from states)
where nodes = 'table.asos tbody tr'

create buffer stationReadings(url string, stamp string, time string, wind string, visibility string, weather string, temp string, humidity string)

insert into stationReadings
select
	url,
	pick 'td:nth-child(1)', --date
	pick 'td:nth-child(2)', --time
	pick 'td:nth-child(3)', --wind
	pick 'td:nth-child(4)', --visibility
	pick 'td:nth-child(5)', --weather
	pick 'td:nth-child(7)', --temp
	pick 'td:nth-child(11)' --humidity
from download page (select url from station) with (thread(10))
where nodes = 'table[cellspacing="3"] tr'

select city, state, stamp, time, wind, visibility, weather, humidity, temp
from station s
join stationReadings r on r.url = s.url

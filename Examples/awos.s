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
	pick 'td:nth-child(1) a' take attribute 'href' match 'raw' replace 'decoded' match 'hours=36' replace 'hours=0', --link to details
	pick 'td:nth-child(1) a', --station
	pick 'td:nth-child(2)', --city
	pick 'td:nth-child(4)' --state
from download page (select
	'https://www.faa.gov/air_traffic/weather/asos/?state=' + state
	from states)
where nodes = 'table.asos tbody tr'

select
	d.url,
	s.city,
	s.state,
	pick 'tr:nth-of-type(3) td:nth-of-type(2)', --temp
	pick 'tr:nth-of-type(4) td:nth-of-type(2)', --dewpoint
	pick 'tr:nth-of-type(5) td:nth-of-type(2)', --pressure
	pick 'tr:nth-of-type(6) td:nth-of-type(2)', --winds
	pick 'tr:nth-of-type(7) td:nth-of-type(2)', --Visbility
	pick 'tr:nth-of-type(8) td:nth-of-type(2)', --ceiling,
	pick 'tr:nth-of-type(8) td:nth-of-type(2)' --clouds,
from download page (select url from station) d with (thread(10))
join station s on s.url = d.url
where nodes = '#awc_main_content table:not([align="left"])'



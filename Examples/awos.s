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
	s.city,
	s.state,
	pick 'td:nth-child(2)'
from download page (select url from station) d with (thread(10))
join station s on s.url = d.url
where nodes = 'table[cellpadding="3"] tr:nth-last-of-type(n+2)'



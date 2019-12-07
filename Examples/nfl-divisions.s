var page = download page 'https://www.espn.com/nfl/standings' with (js)

create buffer a(id identity, wins int, loses int)

insert into a
select
	pick 'td:nth-child(1)' as wins,
	pick 'td:nth-child(2)' as loses
from page
where nodes = 'div.Table__Scroller tr:not(.subgroup-headers)'

create buffer b(id identity, team string)

insert into b
select 
	pick 'td span:last-child a'
from page
where nodes = 'table.Table.Table--fixed tr:not(.subgroup-headers)'


select b.team, a.wins, a.loses
from b
join a on a.id = b.id




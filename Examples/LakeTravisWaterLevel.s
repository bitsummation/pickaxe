create buffer levels(month string, year string, level float)

insert into levels
select
	pick 'td:nth-child(1) p.bold' take text match '(\d+)-(\w+)' replace '$2',
	pick 'td:nth-child(1) p.bold' take text match '(\d+)-(\w+)' replace '$1',
	pick 'td:nth-child(2) p' take text match '\d{3}\.\d{2}'
from download page (
	select
		'http://www.golaketravis.com/waterlevel/' + pick '' take attribute 'href'
	from download page 'http://www.golaketravis.com/waterlevel/'
	where nodes = 'table[width="600"] td[style="background-color: #62ABCC;"] p.white a'
	)
where nodes = 'table[width="600"] tr'

select *
from levels

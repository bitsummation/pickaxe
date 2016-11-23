create buffer specialty(id identity, url string, spec string)

insert into specialty
select
	'http://doctor.webmd.com' + pick 'a' take attribute 'href',
	pick 'a'
from download page 'http://doctor.webmd.com/find-a-doctor/specialties'
where nodes ='section.seo-lists div:nth-child(3) li'


--states
select
	'http://doctor.webmd.com' + pick 'a' take attribute 'href',
	d.url,
	spec
from download page (select url from specialty) d with (thread(4))
join specialty s on s.url = d.url and d.nodes = 'div.states li'



--states
select
	
	'http://doctor.webmd.com' + pick 'a' take attribute 'href'
from download page 'http://doctor.webmd.com/find-a-doctor/specialty/abdominal-radiology'
where nodes = 'div.states li'


--cities
select
	'http://doctor.webmd.com' + pick 'a' take attribute 'href'
from download page 'http://doctor.webmd.com/find-a-doctor/specialty/abdominal-radiology/oregon'
where nodes = '.top-cities li'

select *
from specialty





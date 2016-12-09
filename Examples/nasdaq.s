create buffer pages(endPage int)

insert into pages
select
	pick '.pager li:last-child a' take attribute 'href' match '\d+'
from download page 'http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NASDAQ'

create buffer pageUrls(url string)
each(var p in pages){
	insert into pageUrls
	select
		'http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NASDAQ&page=' + value
	from expand(1 to p.endPage) 
}

create buffer companies(company string, ticker string, url string)

--get all nasdaq companies
insert into companies
select
	pick 'td:nth-child(1) a:first-child',
	pick 'td:nth-child(2) h3 a' match '\w+',
	'https://www.google.com/finance?q=' + pick 'td:nth-child(2) h3 a' match '\w+'
from download page (select url from pageUrls) with (thread(10))
where nodes = '#CompanylistResults tr'

--get all nasdaq prices from google
create buffer prices(company string, price string)
insert into prices
select
	c.ticker,
	pick '.pr'
from download page (select url from companies where company != null and ticker != null) d with (thread(10))
join companies c on c.url = d.url

select *
from prices
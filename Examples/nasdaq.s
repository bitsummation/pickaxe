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

create buffer companies(company string, ticker string)

--get all nasdaq companies
insert into companies
select
	pick 'td:nth-child(1) a:first-child',
	pick 'td:nth-child(2) h3 a' match '\w+'
from download page (select url from pageUrls) with (thread(10))
where nodes = '#CompanylistResults tr'

--get all nasdaq prices from yahoo
create buffer prices(company string, price string)
insert into prices
select
	pick '.title h2',
	pick '.time_rtq_ticker'
from download page (select 'http://finance.yahoo.com/q?s=' + ticker from companies) with (thread(10))

select *
from prices
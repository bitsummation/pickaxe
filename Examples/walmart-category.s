//subcategories

create buffer subcats(url string)

insert into subcats
select
	pick 'a' take attribute 'href'
from download page 'https://www.walmart.com/cp/oral-care/1007221' with (js)
where nodes = 'div[data-module-id=e7b5e491-0866-4e49-8f62-32caee337d4f] div[id|=TempoCategoryTile--tile]'


update subcats
set url = 'https://walmart.com' + url
where url not like '%walmart.com%'


select
	pick 'div.search-result-product-title a span' as description,
	pick 'div.product-price-with-fulfillment span.price-main-block span.visuallyhidden' as price
from download page (

	select
	'https://walmart.com' + pick 'a' take attribute 'href'
	from download page (select url from subcats) with (js|thread(3))
	where nodes = 'div.paginator ul.paginator-list li:not(.paginator-list-gap)'
	
) with (js|thread(5))
where nodes = 'div.search-product-result li.Grid-col div.search-result-gridview-item'


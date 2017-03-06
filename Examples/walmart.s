create buffer superCategories(url string)

insert into superCategories
select 'http://www.walmart.com/cp/Household-Essentials/1115193'

insert into superCategories
select 'http://www.walmart.com/cp/food/976759'

insert into superCategories
select 'http://www.walmart.com/cp/Beauty/1085666'

insert into superCategories
select 'http://www.walmart.com/cp/Health/976760'

insert into superCategories
select 'http://www.walmart.com/cp/Baby-Products/5427'

create buffer categories(url string)

insert into categories
select
    pick 'a' take attribute 'href'
from download page (select url from superCategories) with (thread(5))
where nodes = '.expander-content li.SideBarMenuModuleItem ul.block-list li'

update categories
set url = 'http://walmart.com' + url
where url not like '%walmart.com%'

create buffer division(url string)

insert into division
select url
from categories    
where url like '%/browse/%'

insert into division
select
    pick '' take attribute 'href'
from download page (select url from categories where url like '%/cp/%') with (thread(5))
where nodes = 'a.TempoCategoryTile-tile-overlay'    

update division
set url = 'http://walmart.com' + url
where url not like '%walmart.com%'


create buffer fetchPage (first int, last int, url string)

insert into fetchPage
select
    pick 'li:nth-child(1)',
    pick 'li:last-child',
    url
from download page (select url from division) with (js|thread(10))
where nodes = '.paginator-list'

create buffer urls(url string)

each(var p in fetchPage)
{
    insert into urls
    select
        p.url + '?page=' + value
    from expand (p.first to p.last)
}

create buffer product(url string, detailUrl string, description string, price string, upc string)

insert overwrite product
select
    url,
    'http://www.walmart.com' + pick '.prod-ProductCard--Image a' take attribute 'href',
    pick '.prod-ProductTitle div',
    pick '.Price-characteristic' + '.' + pick '.Price-mantissa',
    ''
from download page (select url from urls) with (js|thread(10))
where nodes = '.search-result-gridview-item'


update p
set p.upc = u.upc
from (select upc, url
	from download page (select detailUrl from product) with (js|thread(10)) => {
	"
		var primaryProductId = __WML_REDUX_INITIAL_STATE__.product.primaryProduct;
	
		var primaryProduct = __WML_REDUX_INITIAL_STATE__.product.products[primaryProductId];
	
		return [{upc:primaryProduct.upc, url:url}];
	"
	}) u
join product p on p.detailUrl = u.url


select *
from product
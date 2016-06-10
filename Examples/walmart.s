
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

create buffer categories(relativeUrl string)

insert into categories
select
    pick 'a.lhn-menu-toggle' take attribute 'href'
from download page (select url from superCategories) with (thread(5))
where nodes = 'div.zone[data-zone="zone3"] li'

create buffer division(url string)

insert into division
select 'http://walmart.com' + relativeUrl
from categories    
where relativeUrl like '%/browse/%'

insert into division
select
    pick '' take attribute 'href'
from download page (select 'http://walmart.com' + relativeUrl from categories where relativeUrl like '%/cp/%') with (thread(10))
where nodes = '.tile-section-0,.tile-section-1'    

update division
set url = 'http://walmart.com' + url
where url not like '%walmart.com%'

create buffer fetchPage (first int, last int, url string)

insert into fetchPage
select
    pick 'li:nth-child(1)',
    pick 'li:last-child',
    url
from download page (select url from division) with (thread(10))
where nodes = '.paginator-list'

create buffer urls(url string)

each(var p in fetchPage)
{
    insert into urls
    select
        p.url + '?page=' + value
    from expand (p.first to p.last)
}

select
    url,
    pick '.tile-heading',
    pick '.price-display' match '[\d.]+'
from download page (select url from urls) with (thread(20))
where nodes = '.tile-grid-unit-wrapper'
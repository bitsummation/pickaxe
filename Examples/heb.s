create buffer superCategories(url string)
insert into superCategories
select 'https://www.heb.com/category/shop/health-beauty/490021'

create buffer categories(url string)

insert into categories
select
	'https://heb.com' + pick 'a' take attribute 'href'
from download page (select url from superCategories) with (js('div.cat-list-deparment', 5))
where nodes = 'li div.cat-list-deparment'

create buffer division(url string)

insert into division
select
	'https://heb.com' + pick 'a' take attribute 'href'
from download page (select url from categories)
where nodes = 'div.cat-list-deparment'

create buffer fetchPage (first int, last int, url string)

insert into fetchPage 
select
	pick 'li:first-child a',
	pick 'li:last-child a',
	url
from download page (select url from division) with (thread(5))
where nodes = '.paging-container'	

create buffer urls(url string)

each(var p in fetchPage)
{
    insert into urls
    select
        p.url + '?No=' + value
    from expand (p.first to p.last){($-1)*60}
}

create buffer product(description string, price string)

insert into product
select
	pick '.responsivegriditem__title',
	pick 'div.cat-price span.cat-price-number'
from download page (select url from urls) with (thread(5)|js('div.cat-price', 5))
where nodes = 'li.responsivegriditem'

select *
from product






create buffer superCategories(url string)

insert into superCategories
select 'https://www.heb.com/category/shop/food-and-drinks/grocery/2967'

insert into superCategories
select 'https://www.heb.com/category/shop/health-and-beauty/2869'

insert into superCategories
select 'https://www.heb.com/category/shop/baby/2874'

create buffer categories(url string)

insert into categories
select
	'https://heb.com' + pick 'a' take attribute 'href'
from download page (select url from superCategories)
where nodes = '.left-nav li'

create buffer division(url string)

insert into division
select
	'https://heb.com' + pick 'a' take attribute 'href'
from download page (select url from categories)
where nodes = '.left-nav li'

create buffer fetchPage (first int, last int, url string)

insert into fetchPage 
select
	pick 'a#selected',
	case pick 'a#last' when null then 2 else pick 'a#last' end,
	url
from download page (select url from division) with (thread(5))
where nodes = '.paging-container'	

create buffer urls(url string)

each(var p in fetchPage)
{
    insert into urls
    select
        p.url + '?No=' + value
    from expand (p.first to p.last){($-1)*35}
}

create buffer product(url string, detailUrl string, description string, price string)

insert into product
select
	url,
	'https://heb.com' + pick '.cat-list-deparment a' take attribute 'href',
	pick '.cat-list-deparment a',
	pick '.cat-price span'
from download page (select url from urls) with (thread(5))
where nodes = '.gridproductview'

select *
from product
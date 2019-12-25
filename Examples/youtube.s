create buffer t(value int)

//used for loop count
insert into t
select *
from expand (1 to 3)

create buffer videos(url string, link string, title string, views string, duration string)

create buffer urls(url string)

insert into urls
select 'https://www.youtube.com/watch?v=7NJqUN9TClM'

each(var i in t)
{
	insert into videos
	select
		url,
		'http://youtube.com' + pick '.content-wrapper a' take attribute 'href',
		pick '.content-wrapper a span.title',
		pick '.content-wrapper .view-count' match '\d+',
		pick '.thumb-wrapper .video-time'
	from download page (select url from urls) with (thread(10))
	where nodes = 'div.watch-sidebar-body .video-list-item'		
	
	insert overwrite urls
	select link
	from videos v
	join urls u on v.url = u.url
}

select *
from videos
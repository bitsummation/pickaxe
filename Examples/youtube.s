create buffer t(value int)

insert into t
select *
from expand (1 to 2)

create buffer videos(url string, link string, title string, views string, duration string, processed int)
insert into videos
select
	url,
	'http://youtube.com' + pick '.content-wrapper a' take attribute 'href',
	pick '.content-wrapper a span.title',
	pick '.content-wrapper .view-count',
	pick '.thumb-wrapper .video-time',
	0
from download page 'https://www.youtube.com/watch?v=7NJqUN9TClM'
where nodes = 'div.watch-sidebar-body .video-list-item'

create buffer urls(url string)

each(var i in t)
{
	insert overwrite urls
	select url
	from videos
	where processed = 0
	
	insert into videos
	select
		url,
		'http://youtube.com' + pick '.content-wrapper a' take attribute 'href',
		pick '.content-wrapper a span.title',
		pick '.content-wrapper .view-count',
		pick '.thumb-wrapper .video-time',
		0
	from download page (select link from videos where processed = 0) with (thread(10))
	where nodes = 'div.watch-sidebar-body .video-list-item'
	
	update v
	set processed = 1
	from videos v
	join urls on v.url = urls.url
}

select *
from videos
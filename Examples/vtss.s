create buffer postpages (startPage int, endPage int)

insert into postpages
select
	pick 'li.current a' take text,
	pick 'li:nth-child(7) a' take text
from download page 'http://vtss.brockreeve.com/?t=All'
where nodes = 'ol.page-nav'

create buffer pageurls (url string)

each(row in postpages){
	
	insert into pageurls
	select
		'http://vtss.brockreeve.com/Home/Index/' + value + '?t=All'
	from expand (row.startPage to row.endPage)
}


create buffer detailurls (url string)

each(row in pageurls){
	
	insert into detailurls
	select
	'http://vtss.brockreeve.com' + pick 'h3 a' take attribute 'href'
	from download page row.url
	where nodes = 'div.topic'

}

create buffer topicdetails (id identity, title string, post string, user string)
create buffer topicreplies (id int, url string, post string, user string)

each(row in detailurls) {
	
	downloadPage = download page row.url

	insert into topicdetails
	select
		pick 'h3' take text, --title
		pick 'p:nth-child(3)' take text, --post
		pick 'p.author a' take text --user
	from downloadPage
	where nodes = 'div.topic'
	
	insert into topicreplies
	select
	@@identity,
	row.url,
	pick 'p:nth-child(2)' take text,
	pick 'p.author a' take text
	from downloadPage
	where nodes = 'div.reply'

}

select *
from topicreplies

/*
select
		pick 'h3' take text, --title
		pick 'p:nth-child(3)' take text, --post
		pick 'p.author a' take text --user
	from download page (select url from detailurls)
	where nodes = 'div.topic'*/
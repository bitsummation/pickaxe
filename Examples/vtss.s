create buffer postpages (startPage int, endPage int)

insert into postpages
select
	pick 'li.current a',
	pick 'li:nth-child(7) a'
from download page 'http://vtss.brockreeve.com/?t=All'
where nodes = 'ol.page-nav'

create buffer detailurls (url string)

each(var row in postpages){
	
	insert into detailurls
	select
	'http://vtss.brockreeve.com' + pick 'h3 a' take attribute 'href'
	from download page (select
							'http://vtss.brockreeve.com/Home/Index/' + value + '?t=All'
						from expand (row.startPage to row.endPage)
	)
	where nodes = 'div.topic'

}

create buffer topicdetails (id identity, title string, post string, user string)
create buffer topicreplies (id int, url string, post string, user string)

each(var row in detailurls) {
	
	var downloadPage = download page row.url

	insert into topicdetails
	select
		pick 'h3', --title
		pick 'p:nth-child(3)', --post
		pick 'p.author a' --user
	from downloadPage
	where nodes = 'div.topic'
	
	insert into topicreplies
	select
	@@identity,
	row.url,
	pick 'p:nth-child(2)',
	pick 'p.author a'
	from downloadPage
	where nodes = 'div.reply'

}

select title, d.user, r.post, r.user
from topicdetails d
join topicreplies r on r.id = d.id

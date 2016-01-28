# Pickaxe
---
An easy to use SQL based web scraper language. If you know SQL and a little about CSS selectors and want to capture data from the web, this is the tool for you.
## Downloads
---
Found [here](https://github.com/bitsummation/pickaxe/releases). It requires .NET framework 4.0. The Pickaxe-Console.zip is the command line version without the editor.
## Quickstart
---
Pickaxe uses SQL statements combined with CSS selectors to pick out text from a web page. Download the tool from above and unzip the contents and double click on **Pickaxe.Studio.exe**. Below are some example snippets. A full runnable example that scrapes a forum I host is found [here](https://raw.githubusercontent.com/bitsummation/pickaxe/master/Examples/vtss.s).
#### Example 1
Capture the commit information from this page.
```sql
select
	case pick '.icon span.octicon-file-text' take text
		when null then 'Folder'
		else 'File'
	end, --folder/file
	pick '.content a' take text, --name
	pick '.message a' take text, --comment
	pick '.age span' take text --date
from download page 'https://github.com/bitsummation/pickaxe'
where nodes = 'table.files tr.js-navigation-item'
```
#### Example 2
What's your ip address?
```sql
select
	pick '#section_left div:nth-child(2) a' take text
from download page 'http://whatismyipaddress.com/'
```
#### In Memory Buffer
Store results in memory.
```sql
create buffer results(type string, folder string, message string, changeDate string)

insert overwrite results
select
    case pick '.icon span.octicon-file-text' take text
        when null then 'Folder'
        else 'File'
    end, --folder/file
    pick '.content a' take text, --name
    pick '.message a' take text, --comment
    pick '.age span' take text --date
from download page 'https://github.com/bitsummation/pickaxe'
where nodes = 'table.files tr.js-navigation-item'

select *
from results
```
#### File Buffer
Store results into a file.
``` sql
create file results(type string, folder string, message string, changeDate string)
with (
    fieldterminator = '|',
    rowterminator = '\r\n'
)
location 'C:\windows\temp\results.txt'

insert overwrite results
select
    case pick '.icon span.octicon-file-text' take text
        when null then 'Folder'
        else 'File'
    end, --folder/file
    pick '.content a' take text, --name
    pick '.message a' take text, --comment
    pick '.age span' take text --date
from download page 'https://github.com/bitsummation/pickaxe'
where nodes = 'table.files tr.js-navigation-item'

```
#### Variables
``` sql
startPage = 1
endPage = 10
```
#### Expand
Used mostly to generate urls.
``` sql
startPage = 1
endPage = 10

select
    'http://example.com/p=' + value + '?t=All'
from expand (startPage to endPage){($*2) + 10}
```
#### Loops
Allows looping through tables.
``` sql
create buffer temp(baseUrl string, startPage int, endPage int)

insert into temp
	select 'http://test.com', 0, 10

insert into temp
	select 'http://temp.com', 2, 10

create buffer urls(url string)

each(t in temp){
	
	insert into urls
    	select t.baseUrl + value + '?t=All'
    from expand (t.startPage to t.endPage){($*2) + 10}

}

select *
from urls
```
#### Proxies
Must be first statement in program. If the expression in the test block returns any rows, the proxy is considered good and all http requests will be routed through it. If more than one passes they are used in Round-robin fashion.
``` sql
proxies ('104.156.252.188:8080', '75.64.204.199:8888', '107.191.49.249:8080')
with test {	
	select
		pick '#tagline' take text
	from download page 'http://vtss.brockreeve.com/'
}
```

More in-depth tutorials are listed in the tutorials section. Features include.
* You can insert into memory tables or tables that wrap a file.
* Download images

## Tutorials
---
* [Pickaxe Tutorial #1](http://brockreeve.com/post/2015/07/23/SQL-based-web-scraper-language-Tutorial-1.aspx)
* [Pickaxe Tutorial #2](http://brockreeve.com/post/2015/07/31/SQL-based-web-scraper-language-Tutorial-2.aspx)
* [Pickaxe Tutorial #3](http://brockreeve.com/post/2015/08/06/Pickaxe-August-2015-release-notes.aspx)

## Video
---
30 minute in depth video of language features. [Pickaxe Video Tutorial](https://www.youtube.com/watch?v=-F-FftxaXOs)



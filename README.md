# Pickaxe
---
An easy to use SQL based web scraper language. If you know SQL and a little about CSS selectors and want to capture data from the web, this is the tool for you.
## Downloads
---
Found [here](https://github.com/bitsummation/pickaxe/releases). It requires .NET framework 4.0. The **Pickaxe-Console.zip** is the command line version. The command line can run on non-windows platforms using mono.
## Quickstart
---
Pickaxe uses SQL statements combined with CSS selectors to pick out text from a web page. Download **Pickaxe.zip** from above and unzip the contents and double click on **Pickaxe.Studio.exe** to run the GUI editor. Below are some example snippets. A full runnable example that scrapes a forum I host is found [here](https://raw.githubusercontent.com/bitsummation/pickaxe/master/Examples/vtss.s).
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
What's your WAN ip address?
```sql
select
	pick '#section_left div:nth-child(2) a' take text
from download page 'http://whatismyipaddress.com/'
```
#### Match
The match expression uses regular expressions to match text. In this case, it is just taking the numbers of the ip address.
```sql
select
    pick '#section_left div:nth-child(2) a' take text match '\d'
from download page 'http://whatismyipaddress.com'
```
#### Match/Replace
A match expression can be followed by a replace. In this case, we replace the dots with dashes.
```sql
select
    pick '#section_left div:nth-child(2) a' take text match '\.' replace '---'
from download page 'http://whatismyipaddress.com'
```
#### In Memory Buffer
Store results in memory. The insert overwrite statement overwrites existing data in the buffer--if any--while insert into just appends to existing data.
```sql
create buffer results(type string, folder string, message string, changeDate string)

insert overwrite results
select
    case pick '.icon .octicon-file-text' take text
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

insert into results
select
    case pick '.icon .octicon-file-text' take text
        when null then 'Folder'
        else 'File'
    end, --folder/file
    pick '.content a' take text, --name
    pick '.message a' take text, --comment
    pick '.age span' take text --date
from download page 'https://github.com/bitsummation/pickaxe'
where nodes = 'table.files tr.js-navigation-item'
```
### Update
Update table.
``` sql
create buffer videos(video string, link string, title string, processed int)

insert into videos
select
	url,
	'https://www.youtube.com' + pick '.content-wrapper a' take attribute 'href',
	pick '.content-wrapper a' take attribute 'title',
	0
from download page 'https://www.youtube.com/watch?v=7NJqUN9TClM'
where nodes = '#watch-related li.video-list-item'

update videos
set processed = 1
where link = 'https://www.youtube.com/watch?v=JYZMT8otKdI'

select *
from videos
```
### Download Images
``` sql
insert file into 'C:\Windows\temp'
select
    filename,
    image
from download image 'http://brockreeve.com/image.axd?picture=2015%2f6%2fheadShot.jpg'
```
#### Variables
Use var to declare variable.
``` sql
var startPage = 1
var endPage = 10
```
#### Expand
Used mostly to generate urls.
``` sql
var startPage = 1
var endPage = 10

select
    'http://example.com/p=' + value + '?t=All'
from expand (startPage to endPage){($*2) + 10}
```
### Join
Inner join tables.
``` sql
create buffer a (id int, name string)
create buffer b (id int, name string)

insert into a
select 1, 'first'

insert into a
select 2, 'first'

insert into b
select 1, 'second'

select a.name, b.name
from a
join b on a.id = b.id
```
### Case
Case statement in select.
``` sql
select
    case when value < 20
    	then 'small' else 'large'
    end
from expand (1 to 10){($*2) + 10}
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

each(var t in temp){
	
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
## Command Line
---
The **Pickaxe-Console.zip** contains the command line version. Run pickaxe.exe without any arguments to run in interactive mode. Type in statements and run them by ending statement with a semicolon. Pass the file path to run the program. Command line arguments can be passed to the script.

### Run Program
```
pickaxe c:\test.s arg1 arg2
```
Get command line values in program. The ?? is used to assign a default value if args are not passed on command line.
``` sql
@1 = @1 ?? 'first'
@2 = @2 ?? 'second'
```
### Interactive Mode
To run in interactive mode, run pickaxe.exe without any arguments. Now type in statements. When you want the statement to run, end with a semicolon. The statement will then be executed. See screen shot below:
![](https://cloud.githubusercontent.com/assets/13210937/13126421/f66b240a-d58f-11e5-875c-40344f44b3fe.png)

## Tutorials
---
* [Pickaxe Tutorial #1](http://brockreeve.com/post/2015/07/23/SQL-based-web-scraper-language-Tutorial-1.aspx)
* [Pickaxe Tutorial #2](http://brockreeve.com/post/2015/07/31/SQL-based-web-scraper-language-Tutorial-2.aspx)
* [Pickaxe Tutorial #3](http://brockreeve.com/post/2015/08/06/Pickaxe-August-2015-release-notes.aspx)

## Video
---
30 minute in depth video of language features. [Pickaxe Video Tutorial](https://www.youtube.com/watch?v=-F-FftxaXOs)



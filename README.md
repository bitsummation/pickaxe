# Pickaxe
---
Pickaxe uses SQL statements combined with CSS selectors to pick out text from a web page. If you know SQL and a little about CSS selectors, this is the tool for you.
## Downloads
---
Pickaxe runs on linux, MacOS, and windows. Quickest way to get started is to run command below if docker is installed. To install locally, see installation instructions at bottom of page.
```console
docker run -it bitsummation/pickaxe /bin/bash
```
---
Pickaxe is command line tool. Once installed or docker images is running you type pickaxe. If no arguments are given, it runs in interactive mode where code can be typed into the prompt and then when a semicolon is typed the code is run. A file location can be passed or a url to source on the web. Run below to see if things are working.
```console
bash-4.2# pickaxe https://raw.githubusercontent.com/bitsummation/pickaxe/CoreWithCodeDom/Examples/georgetown-airport.s
```
you should see a table like:
```console
+---+-----+-------+------+-----------+----------+
|   | day | time  | wind | visibilty | weather  |
+---+-----+-------+------+-----------+----------+
| 1 | 21  | 11:56 | N 5  | 10.00     | Fog/Mist |
| 2 | 21  | 10:56 | NE 3 | 10.00     | Fog/Mist |
| 3 | 21  | 09:56 | Calm | 6.00      | Fog/Mist |
| 4 | 21  | 08:56 | E 3  | 0.25      | Fog/Mist |
| 5 | 21  | 07:56 | E 5  | 0.25      | Fog/Mist |
| 6 | 21  | 06:53 | Calm | 0.75      | Fog/Mist |
| 7 | 21  | 05:56 | Calm | 1.00      | Fog/Mist |
+---+-----+-------+------+-----------+----------+
```
## How To Write Queries
---
Pickaxe uses SQL like statements to select text from web pages. Instead of the SQL statements running against a database they run against live web pages.
### Download Page
Download page returns a table with columns url, nodes, date, size. The statement below downloads aviation weather information for airports in Texas.
```sql
select *
from download page 'https://www.faa.gov/air_traffic/weather/asos/?state=TX'
```
### Where
Select the nodes we are interested in. To accomplish, set the nodes equal to a css expression. The css selector below gets all tr nodes that are under the table with class asos.
```sql
select *
from download page 'https://www.faa.gov/air_traffic/weather/asos/?state=TX'
where nodes = 'table.asos tbody tr'
```
### Pick
The pick expression picks out nodes under each node specified in the where clause. Pick takes a css selector. In this case, we are getting data in the td elements under each tr element. After the pick css selector, a part of the element can be specified.
* take attribute 'attribute' - takes the attribute value of the node. 
* take text - takes the text of the node (default value and doesn't have to be specified)
* take html - takes the html of the node
```sql
select
    pick 'td:nth-child(1) a' take attribute 'href' as details,
    pick 'td:nth-child(1) a' as station,
    pick 'td:nth-child(2)' as city,
    pick 'td:nth-child(4)' as state 
from download page 'https://www.faa.gov/air_traffic/weather/asos/?state=TX' 
where nodes = 'table.asos tbody tr'
```
### Nested download selects
We create a memory table to store state strings then we insert states into it. The nested download select statement allows the download page statement to download multiple pages at once. 
```sql
create buffer states(state string)

insert into states
select 'TX'

insert into states
select 'OR'

select
	pick 'td:nth-child(1) a' take attribute 'href', --link to details
	pick 'td:nth-child(1) a', --station
	pick 'td:nth-child(2)', --city
	pick 'td:nth-child(4)' --state
from download page (select
	'https://www.faa.gov/air_traffic/weather/asos/?state=' + state
	from states)
where nodes = 'table.asos tbody tr'
```
### Download Threads (make it faster)
A download page statement can use with (thread(2)) hint. The download page statement will then use the number of threads specified to download the pages resulting in much better performance. 
```sql
create buffer states(state string)

insert into states
select 'TX'

insert into states
select 'OR'

select
	pick 'td:nth-child(1) a' take attribute 'href', --link to details
	pick 'td:nth-child(1) a', --station
	pick 'td:nth-child(2)', --city
	pick 'td:nth-child(4)' --state
from download page (select
	'https://www.faa.gov/air_traffic/weather/asos/?state=' + state
	from states) with (thread(2))
where nodes = 'table.asos tbody tr'
```
### Proxies
Must be first statement in program. If the expression in the test block returns any rows, the proxy is considered good and all http requests will be routed through it. If more than one passes they are used in Round-robin fashion.
``` sql
proxies ('104.156.252.188:8080', '75.64.204.199:8888', '107.191.49.249:8080')
with test {	
	select
		pick '#tagline' take text
	from download page 'http://vtss.brockreeve.com/'
}
```
### Identity Column
Specify type as identity and it will auto increment.
```sql
create buffer temp(id identity, name string)

insert into temp
select 'test'

insert into temp
select 'test2'

select *
from temp
```
### Storage Buffers
There are three different ways to store results. In memory, files, and sql databases. An example of each is listed below.
#### In Memory Buffer
Store results in memory. The insert overwrite statement overwrites existing data in the buffer--if any--while insert into just appends to existing data.
```sql
create buffer results(type string, folder string, message string, changeDate string)

insert overwrite results
select
    case pick '.icon .octicon-file-text'
        when null then 'Folder'
        else 'File'
    end, --folder/file
    pick '.content a', --name
    pick '.message a', --comment
    pick '.age span' --date
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
    case pick '.icon .octicon-file-text'
        when null then 'Folder'
        else 'File'
    end, --folder/file
    pick '.content a', --name
    pick '.message a', --comment
    pick '.age span' --date
from download page 'https://github.com/bitsummation/pickaxe'
where nodes = 'table.files tr.js-navigation-item'
```
#### SQL Buffer
Store results in Microsoft SQL Server. The mssql buffer definition must match the sql table structure.
``` sql
create mssql results(type string, folder string, message string, changeDate string)
with (
   connectionstring = 'Server=localhost;Database=scrape;Trusted_Connection=True;',
   dbtable = 'Results'
)

insert into results
select
    case pick '.icon .octicon-file-text'
        when null then 'Folder'
        else 'File'
    end, --folder/file
    pick '.content a', --name
    pick '.message a', --comment
    pick '.age span' --date
from download page 'https://github.com/bitsummation/pickaxe'
where nodes = 'table.files tr.js-navigation-item'
```
## Javscript Rendered Pages
---
If a page renders the HTML client side with javascript a simple js hint is all that is needed. Only use if needed as performance is slower.
```sql
select
	pick '.main-link a' take attribute 'href',
	pick '.main-link a',
	pick '.posts span', --replies
	pick '.views span' --views
from download page 'https://try.discourse.org/' with (js)
where nodes = 'tr.topic-list-item'
```
For more control, specify the HTML element to wait for -- will wait until javscript renders the element -- and the time to wait before timing out (in seconds).
```sql
select
	pick '.main-link a' take attribute 'href',
	pick '.main-link a',
	pick '.posts span', --replies
	pick '.views span' --views
from download page 'https://try.discourse.org/' with (js('tr.topic-list-item', 5))
where nodes = 'tr.topic-list-item'
```
#### Run Javascript
Run Javascript on the downloaded pages. Must return a javascript object or array of objects. The url variable is given by the framework and stores the url of the downloaded page.
```sql
select upc, u
from download page 'https://www.walmart.com/ip/Cheerios-Family-Size-Gluten-Free-Cereal-21-oz/33886599' with (js) => (
	"
		var primaryProductId = __WML_REDUX_INITIAL_STATE__.product.primaryProduct;
		var primaryProduct = __WML_REDUX_INITIAL_STATE__.product.products[primaryProductId];
		return { upc:primaryProduct.upc, u:url };
	"
)
```
## Sub Queries
```sql
select p.title, u.upc
from (
	select
	pick '.prod-ProductTitle div' as title,
	url
	from download page 'https://www.walmart.com/ip/Cheerios-Family-Size-Gluten-Free-Cereal-21-oz/33886599'
) p
join (
	select upc, url
	from download page 'https://www.walmart.com/ip/Cheerios-Family-Size-Gluten-Free-Cereal-21-oz/33886599' with (js) => 
    (
      "
        var primaryProductId = __WML_REDUX_INITIAL_STATE__.product.primaryProduct;
        var primaryProduct = __WML_REDUX_INITIAL_STATE__.product.products[primaryProductId];
        return { upc:primaryProduct.upc, url:url };
      "
	)
) u on u.url = p.url
```
## More Examples
---
#### Example 1
Capture the commit information from this page.
```sql
select
    case pick '.icon .octicon-file-text'
        when null then 'Folder'
        else 'File' 
    end as type, --folder/file
    pick '.content a' as name, --name
    pick '.message a' as comment, --comment
    pick '.age span' as date --date
from download page 'https://github.com/bitsummation/pickaxe'
where nodes = 'table.files tr.js-navigation-item'
```
#### Example 2
What's your WAN ip address?
```sql
select
	pick '#section_left div:nth-child(2) a'
from download page 'http://whatismyipaddress.com/'
```
#### Match
The match expression uses regular expressions to match text. In this case, it is just taking the numbers of the ip address.
```sql
select
    pick '#section_left div:nth-child(2) a' match '\d'
from download page 'http://whatismyipaddress.com'
```
#### Match/Replace
A match expression can be followed by a replace. In this case, we replace the dots with dashes.
```sql
select
    pick '#section_left div:nth-child(2) a' match '\.' replace '---'
from download page 'http://whatismyipaddress.com'
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
## Installation Instructions
---
If .NET Core Runtime 3.1 is installed on machine then download and extract:
##### Linux
https://github.com/bitsummation/pickaxe/releases/latest/download/pickaxe-linux-x64.tar.gz
##### MacOS
https://github.com/bitsummation/pickaxe/releases/latest/download/pickaxe-osx-x64.tar.gz
##### Windows
https://github.com/bitsummation/pickaxe/releases/latest/download/pickaxe-win-x64.zip

If .NET Core Runtime 3.1 is not installed on machine then downlod and extract:
##### Linux
https://github.com/bitsummation/pickaxe/releases/latest/download/self-contained-pickaxe-linux-x64.tar.gz
##### MacOS
https://github.com/bitsummation/pickaxe/releases/latest/download/self-contained-pickaxe-osx-x64.tar.gz
##### Windows
https://github.com/bitsummation/pickaxe/releases/latest/download/self-contained-pickaxe-win-x64.zip

Download and extract chrome driver for the right version of your installed chrome:
https://chromedriver.chromium.org/
Put the location in the path so it can be run from any directory.



# Pickaxe
---
An easy to use SQL based web scraper language. If you know SQL and a litle about CSS selectors and want to capture data from the web, this is the tool for you.
## Downloads
---
Found [here](http://brockreeve.com/page/Pickaxe-A-SQL-web-scraper.aspx).
## Quickstart
---
Pickaxe uses SQL statements combined with CSS selectors to pick out text from a web page. Download the tool from above and unzip the contents and double click on **Pickaxe.Studio.exe**. Below are some simple examples.
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
More in-depth tutorials are listed in the tutorials section. Features include.
* You can insert into memory tables or tables that wrap a file.
* Loop through tables
* Download images
* Expand statement to generate urls that page

## Tutorials
---
* [Pickaxe Tutorial #1](http://brockreeve.com/post/2015/07/23/SQL-based-web-scraper-language-Tutorial-1.aspx)
* [Pickaxe Tutorial #2](http://brockreeve.com/post/2015/07/31/SQL-based-web-scraper-language-Tutorial-2.aspx)

## Video
---
30 minute in depth video of language features. [Pickaxe Video Tutorial](https://www.youtube.com/watch?v=-F-FftxaXOs)



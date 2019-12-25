select 
	pick 'td:nth-child(1)' as ticker,
	pick 'td[data-field=name]' as name,
	pick 'td[data-field=last]' as quote,
	pick 'td[data-field=change]' as change,
	case pick 'td.arrow div.arrow_up'
		when null then 'down'
		else 'up'
	end as direction
from download page 'https://www.cnbc.com/dow-30/' with (js)
where nodes = 'table.quoteTable tbody tr'
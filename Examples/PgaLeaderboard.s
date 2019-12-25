select
	pick 'td.position' as position,
	pick 'td.player-name' as name,
	pick 'td.total' as score,
	pick 'td.thru' as hole
from download page 'http://www.pgatour.com/leaderboard.html' with (js)
where nodes = 'table.leaderboard.leaderboard-table tbody  tr.line-row'
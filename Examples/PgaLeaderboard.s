select
	pick 'td:nth-child(2)' as position,
	pick 'td:nth-child(5) .hidden-small' as player,
	pick 'td:nth-child(6)' as score,
	pick 'td:nth-child(7)' as hole,
	pick 'td:nth-child(8)' as round
from download page 'http://www.pgatour.com/leaderboard.html' with (js)
where nodes = '.leaderboard-item tr.row-main'
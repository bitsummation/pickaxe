select
	pick 'td:nth-child(2)' as position,
	pick 'td:nth-child(5) .hidden-small' as player,
	pick 'td:nth-child(6)' match '\s+' replace '' as score,
	pick 'td:nth-child(7)' match '\s+' replace '' as hole,
	pick 'td:nth-child(8)' match '\s+' replace '' as round
from download page 'http://www.pgatour.com/leaderboard.html' with (js)
where nodes = '.leaderboard-item tr.row-main'
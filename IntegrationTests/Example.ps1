$lines = $args[0]
$lines = $lines | Where-Object { $_ -match "^[^[}]" }   #remove log lines
$count = $lines | Measure-Object -Line
9 -eq $count.Lines

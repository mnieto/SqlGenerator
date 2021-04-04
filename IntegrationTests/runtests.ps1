$lines = dotnet run --project ..\sqlg --no-build -- -s Example.xlsx -c exampleTemplate.json
$lines = $lines | Where-Object { $_ -match "^[^[}]" }   #remove log lines
$count = $lines | Measure-Object -Line
if (9 -ne $count.Lines) {
    exit 1
}




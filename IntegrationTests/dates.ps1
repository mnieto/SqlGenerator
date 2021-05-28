#test columns with FieldType = DateTime
#also test text column with maxLength property

$lines = $args[0]
$lines = $lines | Where-Object { $_ -match "^[^[}]" }   #remove log lines


$count = $lines | Measure-Object -Line
$numLines = 10 -eq $count.Lines

$firstLine = $lines | Select-Object -First 1
$isDate = $firstLine -match "2019-12-02"
$isTruncated = $firstLine -match "'P.O. Box 930, 8060 Very very large name with more '"

$numLines -and $isDate -and $isTruncated
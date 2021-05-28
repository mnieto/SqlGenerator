function Get-TemplateName([string]$fileName) {
    $dir = $name = [System.IO.Path]::GetDirectoryName($fileName)
    $name = [System.IO.Path]::GetFileNameWithoutExtension($fileName)
    Join-Path -Path $dir -ChildPath "${name}Template.json"
}
function Get-ScriptName([string]$fileName) {
    $dir = $name = [System.IO.Path]::GetDirectoryName($fileName)
    $name = [System.IO.Path]::GetFileNameWithoutExtension($fileName)
    Join-Path -Path $dir -ChildPath "${name}.ps1"
}

function Get-RelativePaths([string] $source, [string]$template) {
    $source = Resolve-Path $source -Relative
    $template = Resolve-Path $template -Relative
    " {0} with {1}" -f $source, $template
}

#Collect all the defined tests
#Each test is defined by:
# - A source Excel file
# - A template with same name that the excel but ending with template.json
# - A pwsh script that must return $true or $false and have an input parameter with the output lines from sqlg
function Get-Templates() {
    Import-Module ImportExcel
    $files = Get-ChildItem -Recurse -File -Filter *.xlsx
    $templates = @()
    foreach ($file in $files) {
        $templates += [PSCustomObject]@{
            Source = $file.FullName
            Template = Get-TemplateName($file.FullName)
            Script = Get-ScriptName($file.FullName)
        }
    }
    $templates
}


#Execute tests
$failCount = 0
$totalTests = 0
foreach ($template in Get-Templates) {
    $exception = $null
    $totalTests++
    try {
        
        #excecutes the sqlg commandline with the source and template files and saves the output lines
        $lines = dotnet run --project ..\sqlg --no-build -- -s $template.Source -c $template.Template

        #the *>&1 redirects the existing output in the test script and the output so we can return it
        #the last returned object will be the real return value, that must be
        #$true if test pass or $false if it has failed
        $result = Invoke-Command {& $template.Script *>&1  ($lines) }
        Write-Host $result[0..($result.Count - 2)]
        if ($result[-1]) {
            Write-Host "Passed" -ForegroundColor Green -NoNewline
        } else {
            Write-Host "Failed" -ForegroundColor Yellow -NoNewline
            $failCount++
        }
    } catch {
        Write-Host "Failed" -ForegroundColor Yellow -NoNewline
        $exception = $_.Exception
    }
    Get-RelativePaths -Source $template.Source -Template $template.Template | Write-Host
    if ($null -ne $exception) {
        Write-Host "Error:"
        Write-Host $exception
    }
}

#Summary
Write-Host
Write-Host "$($totalTests - $failCount)/$totalTests test passed"
if (0 -ne $failCount) {
    exit 1
}

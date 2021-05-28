runtests.ps1 will iterate recursivelly all test sets and execute them. Each test set is composed by 3 files:
* ExampleNameFile.xlsx: A Excel file with the source data
* ExampleNameFile*Template.json*: A template with the same name that the Excel file but ending with Template.json This template will define the behaviour of the sqlg tool
* ExampleNameFile.*ps1*: A pwsh script that will test the specific test case. 


The powershell script must receive `string[]` argument containing the output of the ~sqlg~ command line tool. Also it must return `$true` or `$false` depending on if the test pass or fails.

# SqlGenerator
Map input from Excel|text files|SQL engine and generate sql insert|update|delete commands or update directly SQL database

Intended as command line tool and Windows application

# Status
[![.NET](https://github.com/mnieto/SqlGenerator/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/mnieto/SqlGenerator/actions/workflows/dotnet.yml)

# Features
* Infer from Excel data types and table name by convention. No need to specify target information
* Use of extensive command-line arguments to define behaviour. Allow to generate sql in batch commands
* Use of .sqlg files that store options and mappings and can be used as input argument
* Intended to be extensible to different database engines (SQL dialects). By default SQL Server

# Related projects
https://github.com/jaklithn/SqlGenerator

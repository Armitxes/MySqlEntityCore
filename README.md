# MySQL Entity Core

Intuitive framework to quickly convert your C# models into a MySQL database table.

Unlike alternative frameworks such as the [MS Entity Framework](https://docs.microsoft.com/de-de/ef/), we rely on [attributes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/) to quickly describe you model and model fields to the database. 

Combining [attributes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/) with custom attribute properties and given [data types](https://docs.microsoft.com/de-de/dotnet/csharp/language-reference/builtin-types/built-in-types) provides us the great flexibility we require without having to write much code.

## Restrictions
* MySQL Entity Core is not build nor meant for BigData or Corporate projects!
* As for now, the project is maintained by myself as a proof of concept. Use at your own risk!

## Requirements
* MySQL Server 8.0 or greater.
* This project uses the MySql.Data package for database transactions.

## Development
* [Developement Documentation](/docs/README.md)
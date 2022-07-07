![GitHub](https://img.shields.io/github/license/Armitxes/MySqlEntityCore) [![NuGet Package](https://github.com/Armitxes/MySqlEntityCore/actions/workflows/nuget.yml/badge.svg)](https://github.com/Armitxes/MySqlEntityCore/actions/workflows/nuget.yml) [![NuGet](https://img.shields.io/nuget/v/MySqlEntityCore?color=darkgreen)](https://www.nuget.org/packages/MySqlEntityCore) [![MySqlEntityCore on fuget.org](https://www.fuget.org/packages/MySqlEntityCore/badge.svg)](https://www.fuget.org/packages/MySqlEntityCore)

# MySQL Entity Core
Intuitive framework to quickly convert your C# models into MySQL database tables.

Unlike alternative frameworks such as the [MS Entity Framework](https://docs.microsoft.com/de-de/ef/), we rely on [attributes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/) to quickly describe your model and model fields to the database. 

Combining [attributes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/) with custom attribute properties and native [data types](https://docs.microsoft.com/de-de/dotnet/csharp/language-reference/builtin-types/built-in-types) provides the required flexibility for any database concept.

## Restrictions
* MySQL Entity Core is not build nor tested for BigData or mayor corporate projects!
* As for now, the project is maintained by myself as a proof of concept. Use at your own risk!

## Requirements
* MySQL Server 8.0 or greater.
* .NET (Core) 6 or above
* This project uses the MySql.Data package for database transactions.

## Development
* [Developement Documentation](/docs/README.md)

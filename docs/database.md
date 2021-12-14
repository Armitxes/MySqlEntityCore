[<- Back](README.md)

# Switch Database

Within the Connection class you can access the `ChangeDatabase` method to switch the Database for the current connection instance.

Example: `Default.Stream.ChangeDatabase("MyDatabase");`

The method will attempt to create the Database if it's not found.

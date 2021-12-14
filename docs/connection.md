[<- Back](README.md)

# Database connection
To establish a connection, we require the `MySqlEntityCore.Connection` class.

Example with default values:
```
new Connection(
    host: "127.0.0.1",
    string: "3306",
    user: "root",
    password: "",
    default: true
);
```
Of course we strongly disencourage the use of any superusers on production environments.
Ensure to set the `default` parameter to false if you wish to establish side-connections to another databases.

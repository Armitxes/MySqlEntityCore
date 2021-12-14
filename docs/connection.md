[<- Back](README.md)

# Database connection
To establish a connection, we require the `MySqlEntityCore.Connection` class.

Example with default values:
```
new Connection(
    host: "127.0.0.1",
    string: "3306",
    user: "",
    password: "",
    database: "mysqlentitycore"
);
```
Of course we strongly disencourage the use of any superusers on production environments.

It's recommended to call this constructor as early as possible to provide the application default connection details for further queries. Alternatively you can call the static `MySqlEntityCore.Connection.SetDefaultPoolingConnection` method.
This command will not open or pull any connection from the pool until queries are executed.

[<- Back](../README.md)

# Data Query
This page describes different supported approaches to get data from the database.

We will refer to following model for demonstration purposes.
```csharp
[Model]
public class User : MySqlEntityCore.Template.DefaultModel
{
    [Field(Size = 45, Unique = true, Required = true)]
    public string Username { get; set; }

    [Field(Size = 64)]
    public string Password { get; set; }

    [Field(Unique = true, Required = true)]
    public Contact Contact { get; set; }
}
```

## Getter

### Get single record by id
Inheriting from `MySqlEntityCore.Template.DefaultModel` provides a constructor to get a record by id.

```csharp
new User(1);  // Gets the user with unsigned (uint) id 1 from database.
```

### Get records by ids
As constructor are limited to its current single instance, `MySqlEntityCore.Template.DefaultModel` provices a static `Get` method to obtain a list of records.
```csharp
uint[] ids = new uint[] {1, 2, 3};
List<User> users = DefaultModel.Get<User>(ids);  // returns users with id 1, 2 and 3
// List<User> users = User.Get<User>(ids);       // alternative, returns users with id 1, 2 and 3
```

### Get records by custom conditions
`MySqlEntityCore.Template.Core` provides a static `Get` method for custom order by and where conditions.
```csharp
List<User> users = DefaultModel.Get<User>(
    where: "`contact`=1 AND `username`='Test'",
    orderby: "`contact` DESC",
    offset: 0,  // Default = 0
    limit: null,  // Default = null (None)
);
```

### Get related record(s)
Models are connected by relations such as one2one, one2many, many2one or many2many.
In our example you see an identifying one2one (1:1) relation to `contact`.

For performance reasons, related records only contain the related id while all other fields are set to null.
To fill all other fields with with the id related data, you must call the `fetch` method.

```csharp
User user = new User(1);
Console.WriteLine(user.Contact.Name); // won't work
user.Contact.Fetch();
Console.WriteLine(user.Contact.Name); // works.
```


## Custom Queries
SQL statements can be used directly if none of the framework provided features fulfill the needs. <span style="color:red">Note that custom query results are not cached.</span>

For queries with returning rows (records) you may use `new Connection().Query(sql);`
* Returns a list of key-value pairs. LINQ can be used on the result.
* If you expect a single row/record result, you may use `new Connection().Query(sql).First[OrDefault]();`

For queries without expected response you may use `new Connection().NonQuery(sql);`
* Returns void

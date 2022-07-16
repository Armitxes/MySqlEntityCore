[<- Back](../README.md)

# Data Manipulation
This page describes supported approaches to insert, update or delete data from the database.
All methods have effect at the executing class instance values, cached values aswell as database row(s).

We will refer to following model for demonstration purposes.
```csharp
[Model]
public class User : MySqlEntityCore.Template.DefaultModel
{
    [Field(Size = 45, Unique = true, Required = true)]
    public string Username { get; set; }

    [Field(Size = 64)]
    public string Password { get; set; }
}
```

## Create
To create, or insert, data into to database you must call the provided `Create` method.
```csharp
User user = new User();
user.Username = "User1";
user.Password = "12345"; // Please hash and salt PWs ;) 
user.Create();
```

Note that the create method will try to set generated values such as primary keys of the record.
Calling the create method on a new instance of `DefaultModel` will thereby always set its id field.
Calling the create method on an already created record will have no effect.

### Write
To write, or update, existing records you must call the provided `Write` method.
Access the `Origin` property of a record to retrieve original field values prior its modification.
The `Origin` property will be updated once any unsubmitted changes are written into the database. 

```csharp
User user = new User(1);
user.Password = "54321"; // Please hash and salt PWs ;) 
user.Write();
```

### Delete
Simply call the provided `Delete` method to delete the record.
```csharp
new User(1).Delete();
```

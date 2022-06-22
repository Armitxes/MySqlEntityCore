[<- Back](../README.md)

# One2one
In a one2one relation one record has exactly one related record in another model and vice versa.
A one2one relation is always identifiable as
* in theory the fields of model B could be included in model A.
* and id of a record model A could be used to uniquely identify a record in model B and vice versa.

```csharp
[Model]
class User : MySqlEntityCore.Template.DefaultModel
{
    // We could use Template.Core and use the Contact.Id as primary key.
    // But normally it's better to keep things separated for greater flexibility.
    [Field(Unique=true, Required=true)]
    public Contact RelContact { get; private set; }

    // ...

    [Field(Required=true)]
    public string Username { get; private set; }
}


[Model]
class Contact : MySqlEntityCore.Template.DefaultModel
{
    [Field(Unique=true, Required=true)]
    public User RelUser { get; private set; }

    // ...

    [Field(Required=true)]
    public string FirstName { get; private set; }

    [Field(Required=true)]
    public string LastName { get; private set; }
}
```

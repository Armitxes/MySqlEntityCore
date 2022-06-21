[<- Back](../README.md)

# Many2many
In a many2one relation many records are related to many records in another model and vice versa.
For example one group has many users, but one user may also have many groups.

```csharp
[Model]
class User : MySqlEntityCore.Template.DefaultModel
{
    [Field(Required=true)]
    public string Username { get; private set; }
}

class Group : MySqlEntityCore.Template.DefaultModel
{
    [Field(Required=true)]
    public string Name { get; private set; }
}

class GroupUser : MySqlEntityCore.Template.DefaultModel
{
    [Field(Required=true)]
    public User RelUser { get; private set; }

    [Field(Required=true)]
    public Group RelGroup { get; private set; }

    [Field]
    public string Note { get; private set; };
}
```

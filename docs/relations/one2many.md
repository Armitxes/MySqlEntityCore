[<- Back](../README.md)

# One2many
In a one2many relation one record has zero or more related record in another model.
By creating a one2many relationship your autmatically create a many2one relationship and vice versa.


For example, one company can own many other companies.
```csharp
[Model]
class Company : MySqlEntityCore.Template.DefaultModel
{
    [Field]
    public Company ParentCompany { get; private set; } = null;

    // ...

    [Field(Required=true)]
    public string Name { get; private set; }
}
```

```csharp
Company corporate = new Company(); 
corporate.Name = "Inquisitor Inc.";
corporate.Create();

Company subsidiaryCompany = new Company(); 
subsidiaryCompany.ParentCompany = corporate.Id;
subsidiaryCompany.Name = "SubInquisitor Ltd.";
subsidiaryCompany.Create();

Company subsidiaryCompany2 = new Company(); 
subsidiaryCompany2.ParentCompany = corporate.Id;
subsidiaryCompany2.Name = "SubInquisitor GbR.";
subsidiaryCompany2.Create();

// SubInquisitor Ltd. and SubInquisitor GbR. are now owned by Inquisitor Inc.
```

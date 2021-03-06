[<- Back](../README.md)

# One2many
In a one2many relation one record has zero or more related records in another model.
Every one2many relationship has a many2one relationship as counterpart.


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
subsidiaryCompany.ParentCompany = corporate;
subsidiaryCompany.Name = "SubInquisitor Ltd.";
subsidiaryCompany.Create();

Company subsidiaryCompany2 = new Company(); 
subsidiaryCompany2.ParentCompany = corporate;
subsidiaryCompany2.Name = "SubInquisitor GbR.";
subsidiaryCompany2.Create();

// SubInquisitor Ltd. and SubInquisitor GbR. are now owned by Inquisitor Inc.
```

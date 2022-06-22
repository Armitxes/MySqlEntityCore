[<- Back](../README.md)

# Transaction Control
This page explains how to perform [transaction control](https://dev.mysql.com/doc/refman/8.0/en/commit.html).
Note that any operations performed within a transaction are NOT cached.

## Initialization
To initialize a new transaction block the `MySqlEntityCore.Transaction` constructor is required.

```csharp
MySqlEntityCore.Transaction transaction = new MySqlEntityCore.Transaction();
```

## Attaching
To have effect, transactions must be attached or provided to models and queries.
This can be achived either by passing the Transaction instance as parameter or by attaching it to the model.

```csharp
MySqlEntityCore.Transaction transaction = new MySqlEntityCore.Transaction();

Contact contact = new Contact();
User user = new User();

// Attach transaction to the user model instance
contact.AttachedTransaction = transaction;
user.AttachedTransaction = transaction; 

// This block has no effect outside the transaction until commited.
contact.email = "john@example.com";
contact.Firstname = "Jon";
contact.Create();
contact.Firstname = "John";
contact.Write();

// This block has no effect outside the transaction until commited.
user.Username = "JUser";
user.Contact = contact;
user.Create();

// Everything is rolled back if something goes wrong while commiting the chain to the DB
transaction.CommitOrRollback();  // Raises no excepion
// transaction.Rollback()  // Revert all changes, can raise exception.
// transaction.Commit()  // Commit all changes, can raise exception.
```

The transaction can be passed to the `Get<T>` methods as parameter.
```csharp
User user = User.Get<User>(1, new MySqlEntityCore.Transaction());
user.AttachedTransaction.Commit();
```

## Custom queries
`MySqlEntityCore.Transaction` implements its own `Query` and `NonQuery` method.

```csharp
MySqlEntityCore.Transaction transaction = new MySqlEntityCore.Transaction();
List<Dictionary<string, object>> result = transacion.Query("SELECT a,b,c FROM ...");
transacion.NonQuery("UPDATE <table> SET a=x, b=y, c=z WHERE ...");
transaction.CommitOrRollback();
```

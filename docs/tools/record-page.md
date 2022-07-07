[<- Back](../README.md)

# Record Page
Record pages are useful when dealing with large amount of records to prevent long query request times.
`MySqlEntityCore.Tools.RecordPage<T> where T : Core`

Example usage with default values:
```csharp
RecordPage<User>(
    pageNumber: 1,
    recordLimit: 30,
    where: null,
    orderBy: null
);
```

## Class instance properties
- `uint Number`
  - Current page number.
- `uint Limit`
  - Maximum record count of current page.
- `uint Offset`
  - Total offset from first record.
- `List<T> Records`
  - Record list of the current page.

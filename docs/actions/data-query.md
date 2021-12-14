[<- Back](../README.md)

# Data Query

## Custom Queries
For queries with returning rows (records) you may use `new Connection().Query(sql);`
* Returns a list of key-value pairs. LINQ can be used on the result.
* If you expect a single row/record result, you may use `new Connection().Query(sql).First[OrDefault]();`

For queries without expected response you may use `new Connection().NonQuery(sql);`
* Returns void

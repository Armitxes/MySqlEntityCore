[<- Back](../README.md)

# Caching
`MySqlEntityCore.Cache` is responsible for caching retrieved database records to minimize database requests.
`MySqlEnitityCore` handles caching by itself, it's thereby recommended not to intervene.

## Get
Cache entries can be retrieved by calling the `Get(string key)` method.
```csharp
MySqlEntityCore.Cache.Get(string key);
```

## Set
Cache entries can be added or modified by calling the `Set()` method.
```csharp
MySqlEntityCore.Cache.Set(string Set(string key, object value, int keepSeconds = 0))
```

## Remove
Cache entries are removed by the `Remove(string key)` method.
```csharp
MySqlEntityCore.Cache.Remove(string key);
```

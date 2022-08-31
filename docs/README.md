# QueryableValues for EF6

[![MIT License](https://badgen.net/badge/license/MIT/blue)](https://github.com/yv989c/BlazarTech.QueryableValues.EF6/blob/main/LICENSE.md)
[![GitHub Stars](https://badgen.net/github/stars/yv989c/BlazarTech.QueryableValues.EF6?icon=github)][Repository]
[![Nuget Downloads](https://badgen.net/nuget/dt/BlazarTech.QueryableValues.EF6.SqlServer?icon=nuget)][NuGet Package]

This library allows you to efficiently compose an [IEnumerable&lt;T&gt;] in your [Entity Framework 6] (non-core) queries when using the [SQL Server Provider]. This is accomplished by using the `AsQueryableValues` extension method available on the [DbContext] class. Everything is evaluated on the server with a single round trip, in a way that preserves the query's [execution plan], even when the values behind the [IEnumerable&lt;T&gt;] are changed on subsequent executions.

The supported types for `T` are: [Byte], [Int16], [Int32], [Int64], [Guid], and [String].

For a detailed explanation of the problem solved by QueryableValues, please continue reading [here][readme-background].

> 💡 This is a streamlined version of the original [QueryableValues][QueryableValuesEFCoreRepository] for Entity Framework Core, which I have adapted to provide some of its features on [Entity Framework 6].

> 💡 Are you on Entity Framework Core? Use [the original version][QueryableValuesEFCoreRepository] of QueryableValues instead.

## When Should You Use It?
The `AsQueryableValues` extension method is intended for queries that are dependent upon a *non-constant* sequence of external values. In such cases, the underlying SQL query will be efficient on subsequent executions.

## Your Support is Appreciated!
If you feel that this solution has provided you some value, please consider [buying me a ☕][BuyMeACoffee].

[![Buy me a coffee][BuyMeACoffeeButton]][BuyMeACoffee]

Your ⭐ on [this repository][Repository] also helps! Thanks! 🖖🙂

# Getting Started

## Installation
QueryableValues for EF6 is distributed as a [NuGet Package]. You can install it using the command below in your NuGet Package Manager Console window in Visual Studio:

`Install-Package BlazarTech.QueryableValues.EF6.SqlServer`

## How Do You Use It?
The `AsQueryableValues` extension method is provided by the `BlazarTech.QueryableValues` namespace; therefore, you must add the following `using` directive to your source code file for it to appear as a method of your [DbContext] instance:
```c#
using BlazarTech.QueryableValues;
```

> 💡 If you access your [DbContext] via an interface, you can also make the `AsQueryableValues` extension methods available on it by inheriting from the `IQueryableValuesEnabledDbContext` interface.

Below are a few examples composing a query using the values provided by an [IEnumerable&lt;T&gt;].

### Examples
Using the [Contains][ContainsQueryable] LINQ method:


```c#
// Sample values.
IEnumerable<int> values = Enumerable.Range(1, 10);

// This intermediary variable is needed to avoid a NotSupportedException
// with the message: "LINQ to Entities does not recognize the method...".
// Seems to be caused by the use of the Contains method.
var qvQuery = dbContext.AsQueryableValues(values);

// Example #1 (LINQ method syntax)
var myQuery1 = dbContext.TestData
    .Where(e => qvQuery.Contains(e.Id))
    .Select(i => new
    {
        i.Id,
        i.GuidValue
    })
    .ToList();
                
// Example #2 (LINQ query syntax)
var myQuery2 = (
    from e in dbContext.TestData
    where qvQuery.Contains(e.Id)
    select new
    {
        e.Id,
        e.GuidValue
    })
    .ToList();
```

Using the [Join] LINQ method:
```c#
// Sample values.
IEnumerable<int> values = Enumerable.Range(1, 10);

// Here we can make direct use of the AsQueryableValues extension without issues.

// Example #1 (LINQ method syntax)
var myQuery1 = dbContext.TestData
    .Join(
        dbContext.AsQueryableValues(values),
        e => e.Id,
        v => v,
        (e, v) => new
        {
            e.Id,
            e.GuidValue
        }
    )
    .ToList();

// Example #2 (LINQ query syntax)
var myQuery2 = (
    from e in dbContext.TestData
    join v in dbContext.AsQueryableValues(values) on e.Id equals v
    select new
    {
        e.Id,
        e.GuidValue
    })
    .ToList();
```

You can combine multiple set of values in a query:
```c#
// Sample values.
IEnumerable<int> values1 = Enumerable.Range(1, 5);
IEnumerable<int> values2 = Enumerable.Range(5, 5);

var qvQuery1 = dbContext.AsQueryableValues(values1);
var qvQuery2 = dbContext.AsQueryableValues(values2);

var myQuery = (
    from e in dbContext.TestData
    where
        qvQuery1.Contains(e.Id) ||
        qvQuery2.Contains(e.Id)
    select new
    {
        e.Id,
        e.GuidValue
    })
    .ToList();
```

You can mix and match these strategies in your query and make them as complex as you like.

# That's it!
If you find this work useful please don't forget to ⭐ [this repository][Repository].

Thanks! 🖖🙂


[IEnumerable&lt;T&gt;]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable
[Entity Framework 6]: https://docs.microsoft.com/en-us/ef/ef6/
[SQL Server Provider]: https://docs.microsoft.com/en-us/ef/ef6/fundamentals/providers/#which-providers-are-available-for-ef6
[DbContext]: https://docs.microsoft.com/en-us/dotnet/api/system.data.entity.dbcontext
[execution plan]: https://docs.microsoft.com/en-us/sql/relational-databases/query-processing-architecture-guide?
[QueryableValuesEFCoreRepository]: https://github.com/yv989c/BlazarTech.QueryableValues
[readme-background]: https://github.com/yv989c/BlazarTech.QueryableValues#background-
[ContainsQueryable]: https://docs.microsoft.com/en-us/dotnet/api/system.linq.queryable.contains
[Join]: https://docs.microsoft.com/en-us/dotnet/api/system.linq.queryable.join

[Boolean]: https://docs.microsoft.com/en-us/dotnet/api/system.boolean
[Byte]: https://docs.microsoft.com/en-us/dotnet/api/system.byte
[Int16]: https://docs.microsoft.com/en-us/dotnet/api/system.int16
[Int32]: https://docs.microsoft.com/en-us/dotnet/api/system.int32
[Int64]: https://docs.microsoft.com/en-us/dotnet/api/system.int64
[Guid]: https://docs.microsoft.com/en-us/dotnet/api/system.guid
[String]: https://docs.microsoft.com/en-us/dotnet/api/system.string

[Repository]: https://github.com/yv989c/BlazarTech.QueryableValues.EF6
[NuGet Package]: https://www.nuget.org/packages/BlazarTech.QueryableValues.EF6.SqlServer/
[BuyMeACoffee]: https://www.buymeacoffee.com/yv989c
[BuyMeACoffeeButton]: https://raw.githubusercontent.com/yv989c/BlazarTech.QueryableValues.EF6/develop/docs/images/bmc-48.svg

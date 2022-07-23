[<- Back](../README.md)

# Stored routines
Stored routines (also known as stored procedures or functions) are methods stored within and executed within your MySQL DB.

While stored procedures and functions can be great for many scenarios i.e. to keep SQL code 
outside your program code for clean separation it also has considerable limitations.

To bust the myth of the performance advantages of stored routines over plain queries:
- MySQL used to cache routine calls and its result. Means the won time came from a rather inefficient caching feature. This caching feature is deprecated and removed since MySQL 8.0
- Using stored routines for logical operations and calculations is slow (not just in MySQL). Given that we're already using a reliable and performant programming language desinged to deal with logical operations we recommend its usage instead.

Stored routines may still be accessed using [`Query`](./data-query.md#custom-queries) or [`NonQuery`](./data-query.md#custom-queries).

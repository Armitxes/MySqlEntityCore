[<- Back](../README.md)

# Stored routines
Stored routines (also known as stored procedures or functions) are methods stored within MySQL.

While stored procedures and functions can be great for many scenarios i.e. to keep SQL code 
outside your program code for clean separation it also has considerable limitations.

To bust the myth of the performance advantages of stored routines over plain queries:
- MySQL used to cache routine calls and it's result. Means the won time came from a rather inefficient caching feature. This caching feature is also long deprecated and removed since MySQL 8.0
- Using stored routines for logical operations and calculations is slow (not just in MySQL)! Given that you're already using full modern programming language that is great at dealing with logical operations we recommend you using it instead.

Of course you can still use `Query` and `NonQuery` to access stored routines. 

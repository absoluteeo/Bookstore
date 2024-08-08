# Awesome Books Inventory application 

This is a simple RESTful API for managing a list of books. 
THe API uses a JSON file as a persistence layer. 

## Features
The API provides endpoints for managing book records:

GET /books - Retrieve a list of all books
GET /books{isbn} - Retrieve a book by isbn
POST /books - Add a new book
PUT /books/{id} - Update an existing book
DELETE /books/{id} - Delete a book

There is a built in swagger UI for calling endpoints easier

https://localhost:7036/swagger

## Prerequisites: .NET 8.0 SDK

- restore dependencies: `dotnet restore`
- build: `dotnet build`
- run: `dotnet run`

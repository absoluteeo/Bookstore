## The code
- One project for simplicity
- Originally console app, which was then ported to a asp net core web app
- The API is a simple RESTful API for managing a list of books.
- The API uses a JSON file as a persistence layer for simplicity as required in the brief.
- The API provides endpoints for managing book records:
  - GET /books - Retrieve a list of all books
  - GET /books{isbn} - Retrieve a book by isbn
  - POST /books - Add a new book
  - PUT /books/{id} - Update an existing book
  - DELETE /books/{id} - Delete a book
	- There is a built in swagger UI for calling endpoints easier
	- https://localhost:7036/swagger
- Built loosely following a vertical slice architecture
- Feature folder (books) which contains handlers for each endpoint
- Handler files contain the commands results etc
- Implements CQRS using Mediator Pattern for loosely coupled endpoints and handlers
- Use dependency injection for better testability
- Use of FluentValidation for input validation of commands


## Given more time I would
- Add a GUI for the API
- Add more tests
- Add a good locking mechanism for update and delete operations
- Add a proper persistence layer
- Add logging 
- Add more error handling
- Add middlewares 
- Add authentication and authorization, perhaps identity server
- Add pagination

## From the bookstore owners point of view they may require: 
- A UI
- Auth and user management
- Ability for employees to add / remove books
- Business alerts for low stock
- Business alerts for new books
- Analytics on sales 
- Integration with a website 
- Integration with other business systems 

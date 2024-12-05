# Task Management

## Overview

This project is a **Task Management** application built using **.NET 8.0**, with a focus on clean architecture and efficient task handling. The system is designed to help users manage tasks with ease while maintaining a scalable backend architecture.

---

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- Docker (optional for containerization)
- Visual Studio or any code editor of your choice (e.g., VSCode)

### Clone the Repository

```bash
git clone https://github.com/olehkavetskyi/TaskManagement.git
cd TaskManagement
```

### Build and Run Locally

To build and run the application locally, follow these steps:

1. Restore the project dependencies:

```bash
dotnet restore
```

2. Build the project:

```bash
dotnet build
```

3. Run the project:

```bash
dotnet run
```

### Running with Docker

If you'd like to run the application inside a Docker container, ensure that you have Docker installed and then execute:

1. Build the Docker image:

```bash
docker-compose build --no-cache
```

2. Run the application:

```bash
docker-compose up
```

## API Usage Examples

### User

POST <code>/api/user/register</code>

Request:

```json
{
  "username": "string",
  "email": "user@example.com",
  "password": "suPer12#"
}
```

Response:

```json
{
  "token": "jwt_token"
}
```

POST <code>/api/user/login</code>

Request:

```json
{
  "username": "user1",
  "password": "suPer12#"
}
```

Response:

```json
{
  "token": "jwt_token"
}
```

### Task

POST <code>/api/task</code>

Request:

```json
{
  "title": "string",
  "description": "string",
  "dueDate": "2024-12-05T12:36:29.667Z",
  "status": 0,
  "priority": 0
}
```

Response:

```json
{
  "id": "914b1c91-d49f-4a7e-b884-6bc24f0ef99a",
  "description": "string",
  "dueDate": "2024-12-05T12:36:29.667Z",
  "status": 0,
  "priority": 0
}
```

GET <code>/api/task</code>

Response:

```json
{
    "items": [
      {
        "id": "914b1c91-d49f-4a7e-b884-6bc24f0ef99a",
        "description": "string",
        "dueDate": "2024-12-05T12:36:29.667Z",
        "status": 0,
        "priority": 0
      }
    ],
    "totalCount": 1,
    "pageNumber": 1,
    "pageSize": 10
}
```

GET <code>/api/task/:id</code>

Response:

```json
{
  "id": "914b1c91-d49f-4a7e-b884-6bc24f0ef99a",
  "description": "string",
  "dueDate": "2024-12-05T12:36:29.667Z",
  "status": 0,
  "priority": 0
}
```

PUT <code>/api/task:id</code>

Request:

```json
{
  "title": "string",
  "description": "string",
  "dueDate": "2024-12-05T12:49:33.218Z",
  "status": 0,
  "taskPriority": 0
}
```

Response:

```json
{
  "id": "914b1c91-d49f-4a7e-b884-6bc24f0ef99a",
  "description": "string",
  "dueDate": "2024-12-05T12:49:33.218Z",
  "status": 0,
  "priority": 0
}
```

DELETE <code>/api/task/:id</code>

Response:

<code>204 No Content</code>

## Explanation of Design Choices

### 1. Clean Architecture
The project is structured with a focus on clean architecture principles. The main components include:

**API Layer:** Responsible for handling HTTP requests, input validation, and responses.

**Application Layer:** Contains the core business logic, including service interfaces and implementations.

**Domain Layer:** Holds the core entities and domain logic, ensuring that business rules are kept independent from the infrastructure and UI layers.

**Infrastructure Layer:** Contains the actual implementation of data access, email services, external APIs, and logging.

This separation of concerns helps ensure that the project is easy to maintain and scale as business logic changes.

### 2. Docker for Containerization
Docker is used to containerize the application to simplify deployment and ensure consistency between environments. 
By using Docker, the setup process is simplified, and the application can be easily deployed to any environment that supports Docker containers.

*Note: Unit tests are not included in the Docker containerization setup, as they are intended to be run in the development environment. 
This approach ensures a clean separation of runtime and testing concerns.*



### 3. JWT Authentication
The application uses JWT for user authentication. This provides a secure, scalable method of managing user sessions without relying on server-side session storage. 
JWT tokens are passed with each request to the API, allowing stateless authentication.

### 4. Logging with Serilog
For logging, <code>Serilog</code> is used due to its flexibility and easy configuration. It allows logging to various destinations like console, file, and external monitoring services.

### 5. Unit Testing
Unit tests are designed to ensure the reliability of the core business logic. Each service in the <code>Application</code> Layer has been tested as well as <code>TaskController</code> to validate its correctness.

The XUnit framework is used for its flexibility and modern approach.

Mocking dependencies is achieved using libraries like <code>Moq</code> to isolate the components under test.

Unit Tests start automatically with help of <code>GitHub Actions</code>.

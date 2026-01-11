# Order Processing System (.NET)

## Overview
This project implements a backend Order Processing System for an e-commerce platform using modern .NET and clean architecture principles.  
The system supports order creation, status tracking, cancellation, and automated background processing.

The solution is designed to be easy to run locally, well-structured, and easy to extend.

---

## Tech Stack
- .NET 10
- Blazor Web App (Interactive Server) – used as application host
- Entity Framework Core
- SQL Server LocalDB
- ASP.NET Core Minimal APIs
- NUnit (unit testing)
- Swagger (API testing & documentation)

---

## Architecture
The solution follows a layered architecture:

- **OrderProcessing.Domain**
  - Core entities and enums
- **OrderProcessing.Application**
  - Business logic, services, DTOs
- **OrderProcessing.Infrastructure**
  - EF Core DbContext and persistence
- **OrderProcessing.Host**
  - Application host, APIs, background jobs, Swagger
- **OrderProcessing.Tests**
  - NUnit tests for business rules

This separation ensures clean boundaries, testability, and maintainability.

---

## Features Implemented

### 1. Create Order
- Customers can create an order with multiple items.
- Orders are created with initial status **PENDING**.

### 2. Retrieve Order
- Fetch order details by Order ID.

### 3. List Orders
- Retrieve all orders.
- Optional query parameter to filter by status:

### 4. Update Order Status
- Valid status flow:
- PENDING → PROCESSING
- PROCESSING → SHIPPED
- SHIPPED → DELIVERED
- Invalid transitions are rejected.

### 5. Cancel Order
- Orders can be cancelled **only if status is PENDING**.
- Cancellation is blocked once processing has started.

### 6. Background Job
- A background service runs every 5 minutes.
- Automatically updates all **PENDING** orders to **PROCESSING**.

---

## API Endpoints

| Method | Endpoint | Description |
|------|--------|------------|
POST | `/api/orders` | Create a new order |
GET | `/api/orders/{id}` | Get order by ID |
GET | `/api/orders` | List all orders (optional status filter) |
PUT | `/api/orders/{id}/status` | Update order status |
DELETE | `/api/orders/{id}` | Cancel an order |

---

## API Documentation
Swagger is enabled in Development mode.

After running the application, access:

Swagger UI allows testing all APIs and provides enum dropdowns for order status values.

---

## Configuration-Based Business Rules

Order constraints are externalized using configuration:

json
"OrderRules": {
  "MaxItemQuantity": 100,
  "MaxItemsPerOrder": 20
}

---

## Database
- Uses **SQL Server LocalDB**
- Database is auto-created on application startup
- EF Core migrations are applied automatically

No manual database setup is required.

---

## Error Handling

- Centralized exception handling using middleware
- Consistent error responses using Problem Details format
- Prevents internal stack traces from leaking to clients


---

## Testing
- Unit tests are written using **NUnit**
- EF Core In-Memory provider is used for fast and isolated tests
- Tests focus on business rules:
  - Order creation
  - Status transitions
  - Cancellation rules
  - Optional status filtering

---

## How to Run
1. Clone the repository
2. Open the solution in Visual Studio 2026
3. Run the `OrderProcessing.Host` project
4. Access Swagger to test APIs

---

## Notes
- The Blazor project is used purely as a hosting model; all backend logic is exposed via APIs.
- DTOs are used to avoid circular reference issues when serializing EF Core entities.


## Security & Authentication

Authentication and authorization are intentionally not implemented as part of this assignment.
The focus is on core order processing, business rules, and backend architecture.

In a production environment, the system would be secured using JWT-based authentication
with role-based authorization to restrict order updates and cancellations.

---

## AI Assistance Disclosure
AI tools were used to validate architectural decisions, identify edge cases, and improve clarity.
All business logic and implementation decisions were reviewed and implemented manually.

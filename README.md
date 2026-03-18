、# 📚 Enterprise-Grade Book Management System

A robust, full-stack C# web application built with .NET 8, featuring role-based access control (RBAC), JWT authentication, and transaction-safe business logic. 

## ✨ Key Features

### 🔐 Security & Authentication
- **JWT-Based Auth:** Secure login and registration system using JSON Web Tokens.
- **Role-Based Access Control (RBAC):** Strict separation of privileges between `Admin` and standard `User` roles.

### 👥 User Roles & Capabilities
- **Admin Privileges:** - Full CRUD operations on the book inventory (Add, Edit, Delete).
  - Access to a Global Borrowing Log (built with complex 3-table SQL INNER JOINs) to monitor all users, track due dates, and identify overdue books.
- **Reader (User) Privileges:**
  - Browse available books.
  - Borrow and return books with real-time UI state updates.
  - Access a personal "My Books" dashboard with dynamic countdowns for due dates.

### 🛡️ Resilient Business Logic
- **Database Transactions:** The borrow/return operations are wrapped in MySQL transactions to ensure data integrity and prevent "overselling" or concurrency anomalies.
- **Defensive Programming:** Backend validation strictly prevents users from returning books they haven't borrowed or borrowing books already checked out.

## 🚀 Tech Stack
- **Backend:** .NET 8 Web API
- **Frontend:** ASP.NET Core MVC (Razor Views)
- **Database:** MySQL
- **Infrastructure:** Docker & Docker Compose

## 📂 Project Structure
- `LabClient/` : ASP.NET MVC frontend application.
- `LabServiceAPI/` : RESTful Web API backend.
- `data-samples/` : Initialization scripts (`Books.sql`) for the database.
- `docker-compose.yml` : Multi-container orchestration setup.

## 🛠️ How to Run Locally

**Step 1: Prerequisites**
Ensure Docker and Docker Compose are installed on your machine. Ensure ports `8080` (API), `8082` (Client), and `3306` (MySQL) are available.

**Step 2: Build and Run**
Run the following commands in your terminal:
```bash
docker compose down -v
docker compose up --build -d

Step 3: Access the Application

Frontend UI: Open http://localhost:8082 in your browser.

Default Test Accounts:

Admin Account -> Username: admin | Password: 123

Reader Account -> Username: reader | Password: 123

Or easily register a new user directly from the UI!

🔌 API Endpoints Overview
Authentication (/api/auth)

POST /register : Create a new user account.

POST /login : Authenticate and receive a JWT token.

Book Inventory (/api/books)

GET / : Retrieve all books (Requires Auth).

GET /{id} : Retrieve a specific book (Requires Auth).

POST / : Add a new book (Admin Only).

PUT /{id} : Update book details (Admin Only).

DELETE /{id} : Remove a book from the system (Admin Only).

Borrowing System (/api/borrow)

POST /{bookId}/borrow : Borrow a book (Transaction-safe).

POST /{bookId}/return : Return a borrowed book.

GET /mybooks : Get the current user's active borrowed books.

GET /all : Get the global borrowing log for all users (Admin Only).

Created by Xiaona Wan for educational and portfolio purposes.
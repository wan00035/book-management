# 📚 Book Management System

A full-stack C# web application for managing a book collection, featuring:

- ✅ **LabServiceAPI** – .NET 6 Web API with RESTful endpoints
- ✅ **LabClient** – ASP.NET MVC frontend using Razor views
- ✅ **MySQL** backend with sample data auto-loaded from SQL
- ✅ **Docker Compose** for seamless local development and testing

---

## 🔧 Project Structure
Book Management/
├── LabClient/             # ASP.NET MVC frontend
├── LabServiceAPI/         # .NET 6 Web API backend
├── data-samples/          # Books.sql, XML, XSD sample data
├── docker-compose.yml     # Docker setup for API + DB
└── README.md

---

## 🚀 How to Run with Docker

> This will spin up the Web API and MySQL database locally.

### ✅ Step 1: Requirements

- Docker & Docker Compose installed
- Ensure ports `8081` (API) and `3307` (MySQL) are free

### ✅ Step 2: Run the app

```bash
docker compose down -v          # Clean up previous containers and data
docker compose up --build       # Build and run API + MySQL
```
✅ Step 3: Test the API
Once running, visit:
http://localhost:8081/books
You should see a JSON array of book entries.

 📂 Sample Data
📄 Books.sql: Initializes the Books database and creates the books table with 5 sample entries.
📄 Books.xml: Sample book entries in XML format.
📄 Books.xsd: XML schema file for validation.
SQL is automatically executed on first startup via Docker volume bind.

## 🔧 API Overview

| Endpoint        | Method | Description           |
|-----------------|--------|-----------------------|
| /books          | GET    | Get all books         |
| /books/{id}     | GET    | Get a book by ID      |
| /books          | POST   | Add a new book        |
| /books/{id}     | PUT    | Update an existing book |
| /books/{id}     | DELETE | Delete a book by ID   |


🛠 Built With
.NET 6
ASP.NET MVC
MySQL
Docker
Visual Studio / Rider


📜 Author
Created by xiaona wan for educational and portfolio purposes.

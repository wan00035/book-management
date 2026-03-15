# 📚 Book Management System

A full-stack C# web application for managing a book collection, featuring:

- ✅ **LabServiceAPI** – .NET 8 Web API with RESTful endpoints 
- ✅ **LabClient** – ASP.NET MVC frontend using Razor views  
- ✅ **MySQL** backend with sample data auto-loaded from SQL  
- ✅ **Docker Compose** for seamless local development and testing  

---

# 🔧 Project Structure
Book Management/
├── LabClient/             # ASP.NET MVC frontend
├── LabServiceAPI/         # .NET 8 Web API backend
├── data-samples/          # Books.sql, XML, XSD sample data
├── docker-compose.yml     # Docker setup for API + DB + frontend
└── README.md

---

## 🚀 How to Run with Docker

> This will spin up the frontend, Web API, and MySQL database locally.

### ✅ Step 1: Requirements

- Docker & Docker Compose installed  
- Ensure ports `8081` (API), `8082` (Client), and `3307` (MySQL) are free  

---

### ✅ Step 2: Run the app

```bash
docker compose down -v          # Optional: clean previous containers/volumes
docker compose up --build       # Build and run frontend + API + DB
```
✅ Step 3: Access the app
🌐 Frontend: http://localhost:8082
🔗 API (JSON): http://localhost:8081/books

📂 Sample Data
📄 Books.sql: Initializes the Books database with a books table and 5 sample entries
📄 Books.xml: Optional XML version of sample entries
📄 Books.xsd: XML schema for validation

SQL file is auto-executed on container startup via Docker bind mount.

## 🔧 API Overview

| Endpoint        | Method | Description           |
|-----------------|--------|-----------------------|
| /books          | GET    | Get all books         |
| /books/{id}     | GET    | Get a book by ID      |
| /books          | POST   | Add a new book        |
| /books/{id}     | PUT    | Update an existing book |
| /books/{id}     | DELETE | Delete a book by ID   |


🛠 Built With
.NET 8
ASP.NET MVC
MySQL
Docker
Visual Studio / Rider


📜 Author
Created by xiaona wan for educational and portfolio purposes.

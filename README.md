# Book Management System

This is a full-stack C# web application that manages a collection of books. It includes:

- ✅ **LabServiceAPI**: A .NET 6 Web API built with RESTful endpoints
- ✅ **LabClient**: An ASP.NET MVC frontend with dynamic Razor views
- ✅ **MySQL backend** with sample SQL and XML data
- ✅ **Docker support** for easy deployment
- ✅ **IIS Express** and local testing instructions

## 🔧 Project Structure
Book Management/
├── LabClient/           # ASP.NET MVC frontend
├── LabServiceAPI/       # .NET 6 Web API backend (RESTful)
├── data-samples/        # Sample XML, SQL, XSD files
└── README.md

## 🐳 How to Run with Docker

```bash
# Docker commands here
cd LabServiceAPI
docker build -t labserviceapi .
docker run -d -p 8080:80 labserviceapi
```

 📂 Sample Data
	•	Books.sql initializes MySQL database
	•	Books.xml provides sample entries
	•	Books.xsd validates XML schema

📜 Author

Created by xiaona wan for educational and portfolio purposes.

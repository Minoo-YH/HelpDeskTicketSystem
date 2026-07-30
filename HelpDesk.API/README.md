# 🎫 HelpDesk API

<p align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=.net)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-5C2D91?style=for-the-badge&logo=dotnet)
![EF Core](https://img.shields.io/badge/Entity_Framework-Core-6B3FA0?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver)
![JWT](https://img.shields.io/badge/JWT-Authentication-black?style=for-the-badge&logo=jsonwebtokens)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=for-the-badge&logo=swagger)

</p>

<p align="center">
A modern HelpDesk Ticket System built with ASP.NET Core 8 Web API.<br>
Secure authentication, clean architecture and scalable backend design.
</p>

---

# ✨ Features

- ✅ User Registration
- ✅ Secure Password Hashing (BCrypt)
- ✅ User Login
- ✅ JWT Authentication
- ✅ Protected API Endpoints
- ✅ Swagger Authorization
- ✅ Entity Framework Core
- ✅ SQL Server Database
- ✅ Health Check API
- ✅ Docker Ready

---

# 🛠 Tech Stack

| Backend | Database | Security | Tools |
|----------|-----------|----------|-------|
| ASP.NET Core 8 | SQL Server | JWT | Swagger |
| EF Core 8 | Docker | BCrypt | Git |

---

# 📷 Project Preview

## 🏠 Swagger Home

<p align="center">
<img src="./pic/1.png" width="900">
</p>

---

## ❤️ Health Check

<p align="center">
<img src="./pic/health.png" width="900">
</p>

---

## 👤 Register User

<p align="center">
<img src="./pic/apiendpointadd.png" width="900">
</p>

---

## 🔑 Login

<p align="center">
<img src="./pic/login.png" width="900">
</p>

---

## 🔒 JWT Protected Endpoint

<p align="center">
<img src="./pic/userprofile.png" width="900">
</p>

---

## 🚀 Full API

<p align="center">
<img src="./pic/full.png" width="900">
</p>

---

# 🔐 Authentication

After logging in, Swagger returns a JWT Token.

Add the token like this:

```text
Bearer YOUR_TOKEN
```

Then access protected endpoints.

---

# 📌 Available Endpoints

| Method | Endpoint | Description |
|----------|----------------------|----------------------|
| GET | /api/Health | API Health Check |
| POST | /api/Users | Register User |
| POST | /api/Users/login | Login |
| GET | /api/Users/profile | Protected Endpoint |

---

# 🚀 Getting Started

```bash
git clone https://github.com/Minoo-YH/HelpDeskTicketSystem.git

cd HelpDesk.API

dotnet restore

dotnet ef database update

dotnet run
```

---

# 📅 Roadmap

- ✅ Authentication
- ✅ JWT Authorization
- ✅ Swagger
- 🚧 Ticket Management
- ⏳ User Roles
- ⏳ Ticket Comments
- ⏳ Dashboard
- ⏳ React Frontend

---

# 👩‍💻 Author

### **Minoo YH**

GitHub

https://github.com/Minoo-YH

---

## ⭐ If you like this project

Give it a ⭐ on GitHub.
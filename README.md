# HelpDesk Ticket System 🎫

An AI-powered HelpDesk Ticket Management System built with **ASP.NET Core 8**, **SQL Server**, **Docker**, **JWT Authentication**, and **Google Gemini AI**.

The application allows users to create and manage support tickets. Gemini AI analyzes ticket descriptions and suggests a category, priority level, and response.

## 🛠️ Technologies

- ASP.NET Core 8 Web API
- Entity Framework Core
- Microsoft SQL Server 2022
- Docker and Docker Desktop
- JWT Authentication
- Google Gemini AI
- Swagger / OpenAPI
- C#

## ✨ Features

- User registration and login
- JWT-based authentication
- Create and retrieve support tickets
- Admin ticket management
- AI-powered ticket analysis
- Automatic ticket categorization
- AI-generated priority suggestions
- Suggested support responses
- SQL Server database integration
- Docker-based deployment
- Swagger API documentation

## 📁 Project Structure

```text
HelpDeskTicketSystem/
├── HelpDesk.API/
│   ├── Controllers/
│   ├── DTOs/
│   ├── Models/
│   ├── Services/
│   │   └── AI/
│   │       └── GeminiService.cs
│   ├── Migrations/
│   ├── Dockerfile
│   ├── appsettings.json
│   └── HelpDesk.API.csproj
├── pic/
└── README.md
```

## 📋 Prerequisites

Install the following tools:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio Code](https://code.visualstudio.com/)
- Git

The API can run entirely inside Docker, so running the compiled application directly on Windows is not required.

## 🔑 How to Get a Gemini API Key

1. Open [Google AI Studio](https://aistudio.google.com/apikey).
2. Sign in with your Google account.
3. Select **Create API Key**.
4. Copy the generated key.
5. Add the key to your local environment configuration.

**Important:** Never publish your Gemini API Key, JWT signing key, or SQL Server password on GitHub.

## 🔐 Environment Configuration

Inside the `HelpDesk.API` folder, create a file named `.env.docker`:

```dotenv
ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=HelpDeskDb;User Id=sa;Password=YOUR_SQL_PASSWORD;TrustServerCertificate=True;
Jwt__Key=YOUR_SECURE_JWT_KEY
Gemini__ApiKey=YOUR_GEMINI_API_KEY
```

Replace the placeholder values with your own credentials.

Generate a secure JWT key using Python:

```bash
python -c "import secrets; print(secrets.token_urlsafe(48))"
```

Ensure `.env.docker` is included in `.gitignore`:

```gitignore
.env.docker
```

Keep secrets out of `appsettings.json` and other files committed to Git.

## 🐳 Running the Project with Docker

Open a terminal in the `HelpDesk.API` directory.

### 1. Start Docker Desktop

Make sure Docker Desktop is running in Linux container mode.

### 2. Set Up SQL Server

If you already have a SQL Server container named `sqlserver`, you can reuse it.

Otherwise, create a new SQL Server container using your own strong password:

```bash
docker run -d \
  --name sqlserver \
  -e ACCEPT_EULA=Y \
  -e MSSQL_SA_PASSWORD=YOUR_SQL_PASSWORD \
  -p 1433:1433 \
  -v helpdesk-sql-data:/var/opt/mssql \
  mcr.microsoft.com/mssql/server:2022-latest
```

Use the same SQL Server password in `.env.docker`.

### 3. Create the Docker Network

```bash
docker network create helpdesk-network
```

Connect SQL Server to the network:

```bash
docker network connect helpdesk-network sqlserver
```

If the network or connection already exists, reuse it rather than recreating it.

### 4. Build the API Docker Image

```bash
docker build -t helpdesk-api .
```

### 5. Run the API Container

```bash
docker run -d \
  --name helpdesk-api \
  --network helpdesk-network \
  -p 5291:8080 \
  --env-file .env.docker \
  -e ASPNETCORE_ENVIRONMENT=Development \
  helpdesk-api
```

### 6. Check the Running Containers

```bash
docker ps
```

Expected container names:

- `sqlserver`
- `helpdesk-api`

The SQL Server database must also have the project's Entity Framework Core migrations applied before the API can use its tables.

## 🌐 How to Open Swagger

After starting the API container, open your browser and visit:

**http://localhost:5291/swagger/index.html**

Swagger allows you to test API endpoints directly from your browser.

![Swagger API Documentation](pic/apiendpointadd.png)

## 🔑 JWT Authentication in Swagger

Some endpoints require a valid JWT token.

1. Open Swagger.
2. Find **POST `/api/Users/login`**.
3. Select **Try it out**.
4. Enter your registered email and password.
5. Select **Execute**.
6. Copy the returned JWT token.
7. Select **Authorize** at the top of Swagger.
8. Enter the token in the required format. Depending on the Swagger configuration, this may be the token alone or `Bearer YOUR_TOKEN`.
9. Select **Authorize**, then **Close**.

You can now access protected endpoints.

### User Login

![User Login](pic/login.png)

## 📡 API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/Users` | Register a new user |
| POST | `/api/Users/login` | Login and receive JWT |
| POST | `/api/Tickets` | Create a support ticket |
| GET | `/api/Tickets` | Retrieve the authenticated user's tickets |
| POST | `/api/ai/analyze` | Analyze text using Gemini AI |

Additional admin, update, and close-ticket operations are implemented in the ticket controller.

## 🤖 Gemini AI Ticket Analysis

When a user creates a support ticket, the application sends the ticket information to Gemini AI for analysis.

The AI generates three fields:

| Field | Description |
|---|---|
| `aiCategory` | Suggested ticket category |
| `aiPriority` | Suggested priority

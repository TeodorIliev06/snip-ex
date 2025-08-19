# ASP.NET PROJECT SNIPEX

## Summary

Welcome to **SnipEx**, the ultimate social platform for developers!

Think of it as the perfect fusion of Stack Overflow's knowledge sharing and Instagram's social experience, but specifically crafted for code snippets. Whether you're a seasoned developer looking to showcase your coding prowess, a beginner seeking feedback on your first functions, or someone hunting for that perfect solution to a tricky problem, SnipEx has got you covered. 🚀

This dynamic web application empowers developers to share, discover, and collaborate on code snippets across multiple programming languages. With real-time interactions, comprehensive review systems, and a vibrant community-driven approach, SnipEx transforms the way developers learn, share, and grow together. From quick one-liners to complex algorithms, every piece of code finds its home here! 💻✨

## Built With

The Following Technologies:

- **ASP.NET Core 8.0** ⚙️
- **Entity Framework Core 8.0** 🗃️
- **Microsoft SQL Server** 💾
- **ASP.NET Identity System** 🔑
- **JWT Authentication** 🎫
- **MVC Areas** 🏙️
- **Razor Views, Sections, Partial Views** 👁️
- **Repository Pattern** 📦
- **Mediator Pattern** 🔄
- **Dependency Injection** 💉
- **SignalR for Real-time Communication** 📡
- **Client and Server-side Validation** ✅
- **NUnit Tests** 🧪
- **Responsive Design** 📱
- **jQuery** 📜
- **Bootstrap and Custom CSS** 🎨

## Architecture

SnipEx follows a **modular monolith** architecture, providing excellent separation of concerns while maintaining deployment simplicity:

```
SnipEx/
├── SnipEx.Common/           # Shared utilities and common functionality
├── SnipEx.Data/             # Data access layer and repositories
├── SnipEx.Data.Models/      # Entity models and database schema
├── SnipEx.Realtime/         # SignalR hubs and real-time services
├── SnipEx.Service.Data/     # Business logic and data services
├── SnipEx.Service.Mapping/  # Object mapping configurations
├── SnipEx.Service.Mediator/ # CQRS implementation with MediatR
├── SnipEx.Service.Tests/    # Unit and integration tests
├── SnipEx.Web/              # MVC web application
├── SnipEx.Web.Infrastructure/ # Web-specific infrastructure
├── SnipEx.Web.ViewModels/   # View models for web presentation
└── SnipEx.WebApi/           # RESTful API endpoints
```

## Database

![Image](https://github.com/user-attachments/assets/0ceccbfd-ff77-4ace-9fa7-9d4eb04d1e2e)

## Test Coverage

![Image](https://github.com/user-attachments/assets/6a7a86cc-2ad5-43cb-953c-5a6c2e044cca)

## Features

### 🔍 **Code Snippet Management**
- Create, edit, and delete code snippets
- Support for multiple programming languages
- Syntax highlighting and code formatting
- Categorization and tagging system

### 👥 **Social Interactions**
- Like, comment, and share snippets
- Follow other developers
- Real-time notifications via SignalR
- Community-driven reviews and feedback

### 🔍 **Discovery & Search**
- Advanced search and filtering
- Browse by language, tags, or popularity
- Trending snippets and featured content
- Personalized recommendations

### 📊 **User Profiles**
- Showcase your coding portfolio
- Track your contributions and achievements
- View statistics and activity history
- Customizable developer profiles

### 💬 **Real-time Communication**
- Instant messaging between users
- Live comment updates
- Push notifications for interactions
- Real-time collaboration features

### 🔐 **Authentication Enhancements**
- **External Login Providers:**
  - Sign in with **Google** or **GitHub** using OAuth 2.0.
  - Don't have an account? One will be created for you automatically.
- **Forgot Password:**
  - Secure password reset flow via email.
  - Users can request a password reset link and set a new password securely.

## Roles

### *User*
- Create, edit, and delete personal code snippets
- Browse and search the community library
- Like, comment, and share snippets
- Follow other developers and receive updates
- Participate in discussions and code reviews

### *Admin*
- All functionalities of Users
- Manage user accounts and permissions

## Admin Login:
- **Email:** admin@example.com
- **Password:** Admin123!

## HOW TO RUN:

### Prerequisites
- .NET 8.0 SDK
- Microsoft SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code
- Check out the [`CONFIGURATION.md`](./docs/CONFIGURATION.md) for advanced features

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <https://github.com/TeodorIliev06/snip-ex>
   cd snip-ex
   ```

2. **Update Connection String**
   - Open `appsettings.json` in both `SnipEx.Web` and `SnipEx.WebApi` projects
   - Update the connection string to point to your SQL Server instance

3. **Apply Database Migrations**
   ```bash
   dotnet ef database update --project SnipEx.Data --startup-project SnipEx.Web
   ```

4. **Run the Applications**
   - Start both `SnipEx.Web` and `SnipEx.WebApi` projects simultaneously
   - The web application will be available at `https://localhost:7024`
   - The API will be available at `https://localhost:7000`

### *To explore the project's full capabilities, ensure both the web app and the API are running simultaneously for optimal real-time features and seamless user experience. Happy coding!* 🚀

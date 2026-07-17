

---

# 🚀 WasmAI.AutoGenerator

**WasmAI.AutoGenerator** is a powerful code-generation library for .NET 8 that automates the scaffolding of backend components like models, services, controllers, and more—based on a flexible folder configuration.

---

## ✨ Features

- ⚙️ **Automatic Generation of Backend Layers**:
  - DTOs (Data Transfer Objects)
  - DSOs (Domain Service Objects)
  - Repositories
  - Services
  - Controllers
  - Validators
  - Schedulers

- 🗂️ **Dynamic Folder and File Generation**:
  Uses `folderStructure.json` to generate nested folders and files.

- 🏗️ **Architecture Pattern Support**:
  Built-in templates: `Country`, `Plug`, `Share`, `Builder`, `Service`, and `Scope`.

- 🛠️ **Customizable & Modular**:
  Easily adapt to any backend architecture style.

- 🔁 **Service Lifetime Support**:
  Scoped, singleton, and transient services supported.

- 🔔 **Built-in Notification Provider**:
  Supports Email, SMS, Push, and In-App notifications.

---

## 🧱 Architecture Overview

```
┌─────────────┐       ┌────────────────────┐       ┌────────────────┐
│   Models    │──────▶│ Marker Interfaces  │──────▶│ AutoGenerator  │
└─────────────┘       └────────────────────┘       └──────┬──────────┘
                                                        ▼
    ┌────────────┬─────────────┬──────────────┬─────────────┬──────────────┐
    │   DTOs     │ Repositories│  Services    │ Controllers │  Validators  │
    └────────────┴─────────────┴──────────────┴─────────────┴──────────────┘
```

---

## 📦 Installation

```bash
dotnet add package WasmAI.AutoGenerator.API --version 1.0.0-c
```

👉 After installation, add the necessary namespace:

```csharp
using AutoGenerator.ApiFolder;
```

---

## 📁 Folder Structure Configuration (`folderStructure.json`)

Here's an example:

```json
{
  "Controllers": [ "Api", "Auth", "Admin" ],
  "Repositories": [ "Base", "Builder", "Share" ],
  "Services": [ "Email", "Logging" ],
  "DyModels": [
    {
      "VM": [],
      "Dto": {
        "Build": [ "Request", "Response", "ResponseFilter" ],
        "Share": [ "Request", "Response", "ResponseFilter" ]
      },
      "Dso": [ "Request", "Response", "ResponseFilter" ]
    }
  ],
  "Config": [ "Mappers", "Scopes", "Singletons", "Transients" ],
  "Models": [],
  "Builders": [ "Db" ],
  "Helper": [],
  "Data": [],
  "Enums": [],
  "Validators": [ "Conditions" ],
  "Schedulers": []
}
```

---

## 🏗️ How to Generate Your Backend Architecture

### ✅ Option 1: Programmatically

```csharp
using AutoGenerator.ApiFolder;
using System;

class Program
{
    static void Main(string[] args)
    {
        string projectPath = "path_to_your_project";
        ApiFolderGenerator.Build(projectPath);
        Console.WriteLine("✅ All folders have been created successfully!");
    }
}
```

### ⚡ Option 2: Command Line (Recommended)

```bash
dotnet run generate
```

This command reads the `folderStructure.json` and creates all required folders and files instantly.

---

## Steps to Implement `DataContext` and `CategoryModel` with `ITModel` and `ITAutoDbContext`

## 1. Create Your `DataContext`
First, you need to make sure that `DataContext` inherits from `AutoIdentityDataContext` and implements `ITAutoDbContext`. This allows you to manage identity operations and database access in a simple and automated way.

### Code:
```csharp
public class DataContext : AutoIdentityDataContext<ApplicationUser, IdentityRole, string>, ITAutoDbContext
{
    // Add properties like DbSet for your models
    public DbSet<CategoryModel> Categories { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    // You can add any custom functions for DbContext here
}
```

### Explanation:
- `DataContext` inherits from `AutoIdentityDataContext<ApplicationUser, IdentityRole, string>` because you need to work with identity management using `ApplicationUser` and `IdentityRole`.
- `ITAutoDbContext` ensures that the `DataContext` can handle automatic operations.
- `DbSet<CategoryModel>` is an example of adding a model to `DataContext` so it can be interacted with in the database.

---

## 2. Create `CategoryModel` and Implement `ITModel`
Now, you need to implement the `ITModel` interface in your models like `CategoryModel` to take advantage of automatic operations.

### Code:
```csharp
public class CategoryModel : ITModel
{
    [Key]
    public string? Id { get; set; } = $"catm_{Guid.NewGuid():N}";  // Automatically generates a unique value

    [Required]
    [ToTranslation]  // Mark to ensure the field is translated automatically
    public string? Name { get; set; }

    [ToTranslation]  // Mark to ensure the field is translated automatically
    public string? Description { get; set; }
}
```

### Explanation:
- The `CategoryModel` class implements `ITModel`, which means it includes an `Id` property that gets automatically generated using `Guid.NewGuid()`.
- The `Name` and `Description` properties have the `[ToTranslation]` attribute to indicate that they should be translated automatically.

---

## 🔧 API Builder Configuration

Here’s how to configure it in your project:

```csharp
builder.Services
    . AddAutoBuilderApiCore<DataContext,ApplicationUser>(new()
    {
        Arags = args,
        NameRootApi = "V1",
        IsMapper = true,
        Assembly = Assembly.GetExecutingAssembly(),
        DbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection"),
        ProjectPath= "folderStructure.json"
    })
    .AddAutoValidator()
    .AddAutoConfigScheduler()
    .AddAutoNotifier(new()
    {
        MailConfiguration = new MailConfig()
        {
            SmtpUsername = "user@gmail.com",
            SmtpPassword = "your_smtp_password",  // 🔒 Secure this!
            SmtpHost = "smtp.gmail.com",
            SmtpPort = 587,
            FromEmail = "user@gmail.com",
            NameApp = "app"
        },
       // sms
      //  push web any platforms
    });
```

---


## 🚀 Command Line Generation Options

```bash
dotnet run generate
```
```bash
# Basic Generation
dotnet run generate                   # Standard generation
dotnet run generate /m               # With Model Generator
dotnet run generate /m/ai            # AI-Assisted generation

# 🏢 BPR ENTERPRISE PATTERNS
dotnet run generate /bpr c:dso       # Domain Service Objects
dotnet run generate /bpr c:share     # Shared DTO Pattern  
dotnet run generate /bpr c:build     # Builder Pattern
dotnet run generate /bpr c:all       # Complete Enterprise Stack

# 🔄 COMBINED WORKFLOWS
dotnet run generate /bpr c:dso /m    # DSO + Model Generator
dotnet run generate /bpr c:all /m/ai # Complete AI-Assisted

# 🔐 AUTHENTICATION COMMANDS
dotnet run generate /bpr e:auth      # Add authentication files (skip if exists)
dotnet run generate /bpr e:nauth     # Force create new auth files

# 💳 PAYMENT INTEGRATION COMMANDS
dotnet run generate /bpr e:payment   # Add payment provider infrastructure
dotnet run generate /bpr e:payment stripe   # Add Stripe payment integration
dotnet run generate /bpr e:payment paypal   # Add PayPal payment integration
dotnet run generate /bpr e:payment all     # Add all payment providers

# 🎯 COMBINED AUTH & PAYMENT
dotnet run generate /bpr e:auth e:payment  # Auth + Payment infrastructure
dotnet run generate /bpr c:all e:auth e:payment /m/ai  # Complete stack with AI
```
## 🏁 Getting Started Example
## Updated Program.cs Implementation
```csharp
using System.Diagnostics;
using System.Reflection;
using AutoGenerator;
using AutoGenerator.Config;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WasmAI.AutoGenerator.Auth;
using WasmAI.PaymentProvider.Data;
using WasmAI.PaymentProvider.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Enhanced Main Service Registration
builder.Services
    .AddWasmAutoGeneratorApi<PaymentDbContext, ApplicationUser>(new()
    {
        Arags = args,
        NameRootApi = "v1",
        Assembly = Assembly.GetExecutingAssembly(),
        DbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection"),
    });

var app = builder.Build();

// ✅ Enhanced Middleware Registration
app.UseWasmAutoGeneratorApi();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
```

## ✅ Conclusion

**WasmAI.AutoGenerator** supercharges your .NET development by reducing boilerplate and enforcing clean, modular architecture. Whether you're building an admin panel, a complex API, or a service-oriented backend—this tool lets you build your project architecture in seconds with:

--- 


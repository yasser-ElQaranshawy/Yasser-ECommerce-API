# 🛒 E-Commerce Web API (ASP.NET Core)

A robust and scalable E-Commerce backend API built with **ASP.NET Core** and **Entity Framework Core**, designed with clean architecture principles to handle core shopping and payment workflows seamlessly.

## 🚀 Core Features & Modules

- **Product Management:** Efficient handling of product catalogs, details, and inventory data.
- **Shopping Cart (`CartService`):** Supports adding items, updating quantities, viewing cart contents, and managing user-specific carts.
- **Checkout & Payment Processing (`PaymentService`):** 
  - Validates user carts and calculates precise order totals.
  - Generates secure, unique transaction IDs (e.g., `TXN_XXXXXXXX`).
  - Records payment status (`Completed`), payment methods, and timestamps.
- **Automatic Cart Clearing:** Automatically empties the user's shopping cart immediately upon successful payment completion to ensure data consistency.

## 🛠️ Tech Stack & Architecture

- **Framework:** .NET (ASP.NET Core Web API)
- **Data Access:** Entity Framework Core (EF Core)
- **Database:** SQL Server
- **Architecture Pattern:** Clean separation of concerns (Controllers, Services, Models, Data Context, and DTOs)
- **Testing:** Validated end-to-end via `.http` files and REST client tools.

---

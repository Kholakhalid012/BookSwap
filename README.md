# 📚 BookSwap – Buyer–Seller Marketplace

BookSwap is a role-based marketplace web application where users can list, browse, and purchase books securely.  
The system is designed with **Buyer, Seller, and Admin roles**, implementing **policy-based authorization** and **real-time updates using SignalR**.

This project was built as a **portfolio and learning project** following best practices in ASP.NET Core MVC.

---

## 🚀 Features

### 🔐 Authentication & Authorization
- Role-based authentication (Admin, Seller, Buyer)
- Policy-based authorization
- Secure login and registration

### 📖 Book Management
- Sellers can add, edit, and delete books
- Image upload for books
- Book availability status handling

### 🛒 Buyer Functionality
- Browse available books
- Buy/request books
- Real-time notifications using SignalR

### 🛠 Admin Panel
- Manage users and roles
- Manage book listings
- Restricted access using Admin-only policies

### ⚡ Real-Time Features
- SignalR integration for live updates and notifications

---

## 🧰 Tech Stack

- **Backend:** ASP.NET Core MVC
- **Language:** C#
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Real-Time:** SignalR
- **Frontend:** Razor Views, Bootstrap
- **Authentication:** ASP.NET Core Identity

---

## 👥 User Roles

| Role   | Responsibilities |
|-------|------------------|
| Admin | Manage users, roles, and books |
| Seller | Add and manage book listings |
| Buyer | Browse and buy books |

---

## 📂 Project Structure


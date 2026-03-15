# OnlineBookShop

## Database Setup (Entity Framework Core)

This project uses **Entity Framework Core (Code First)** to manage the database schema.

Follow the steps below to create and update the database.

### 1. Open Package Manager Console

In **Visual Studio**:

Tools → NuGet Package Manager → Package Manager Console

---

### 2. Create Initial Migration

Run the following command:

```
Add-Migration InitialCreate
```

This command creates migration files that represent the database schema based on the project models.

Example generated files:

```
Migrations/
 ├── 202603150001_InitialCreate.cs
 └── ApplicationDbContextModelSnapshot.cs
```

---

### 3. Create the Database

Run:

```
Update-Database
```

This will:

* Create the database
* Create tables
* Apply migrations

---

### 4. After Changing Models

Whenever you modify models (Book, Order, CartItem, etc.), create a new migration:

```
Add-Migration UpdateSchema
```

Then apply the changes:

```
Update-Database
```

---


### Reset the Database (Optional)

If you want to recreate the database:

```
Drop-Database
Update-Database
```

---

### 7. Database Tables

Running migrations will create the following tables:

* Users
* Books
* CartItems
* Orders
* OrderItems

---

### Typical Workflow

```
Create Models
      ↓
Add-Migration InitialCreate
      ↓
Update-Database
      ↓
Database Created
```

---


## 🔐 Login Access

### 👤 User Login

Regular users can browse books, add items to the cart, and place orders.

**Features available to users:**

* 📚 Browse book catalog
* 🔍 Search books by title or author
* 🛒 Add books to cart
* 💳 Checkout and place orders
* 📦 View order history

---

### 🛠️ Admin Login

Administrators can manage the bookstore inventory and orders.

**Admin capabilities include:**

* ➕ Add new books
* ✏️ Edit book details
* ❌ Delete books
* 📊 View and manage customer orders
* 📦 Update order status

---

### 🎨 Example Roles

| Role      | Access                          |
| --------- | ------------------------------- |
| 👤 User   | Browse, Cart, Orders            |
| 🛠️ Admin | Manage Books, Orders, Dashboard |

---

### 🔑 Default Access (Example)

```id="4z3tpy"
User Login
Email: user@example.com
Password: user123
```

```id="2x6q7n"
Admin Login
Email: admin@bookshop.local
Password: Admin123
```

---

### 🧰 Technology Used

* ASP.NET Core MVC
* Entity Framework Core
* SQL Server
* Bootstrap 5
* Razor Views

---

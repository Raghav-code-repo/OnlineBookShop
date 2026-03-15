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

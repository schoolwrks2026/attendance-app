# Gym & Turf Management Platform

Welcome to the **Gym & Turf Management Platform**! This is a production-ready, mobile-first SaaS Web Application built with **ASP.NET Core** and **React 19**. It features multi-branch location tracking, facility room/pitch management, complimentary gym memberships, and court/turf calendar bookings with real-time overlap validation.

---

## 🏗️ Architecture & Stack

### Backend
- **ASP.NET Core (C#)**
- **Dapper ORM** (Fast, lightweight queries)
- **PostgreSQL** (Database)
- **FluentMigrator** (Version-controlled DB migrations)
- **FluentValidation** (Decoupled input request validation)
- **JWT Authentication** (Secure stateless tokens)

### Frontend
- **React 19 + TypeScript + Vite**
- **Tailwind CSS** (Responsive utility-first styling)
- **Lucide Icons**

---

## 🚀 How to Build & Run Locally

Follow these step-by-step instructions to get the platform up and running on your local machine:

### 1. Prerequisite Infrastructure
Ensure you have **PostgreSQL** and **Redis** running. You can easily start them using Docker:
```bash
docker-compose up -d
```
This spins up PostgreSQL on port `5432` (with database `gymturf`) and Redis on port `6379`.

---

### 2. Running the Backend API
1. Navigate to the backend directory:
   ```bash
   cd backend
   ```
2. Build the .NET solution:
   ```bash
   dotnet build
   ```
3. Run the database migrations (automatically executed during API start) and launch the web API host:
   ```bash
   dotnet run --project GymTurf.API/GymTurf.API.csproj
   ```
   The backend API will start up on `http://localhost:5264`. You can access the Swagger UI directly at `http://localhost:5264/swagger` to explore the API endpoints!

---

### 3. Running the Frontend SPA
1. Open a new terminal and navigate to the frontend directory:
   ```bash
   cd frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Launch the Vite development server:
   ```bash
   npm run dev
   ```
   The React SPA will start up on `http://localhost:5173`. Open your web browser to this address to interact with the platform!

---

## 🧪 How to Run Tests

### Run Backend Unit Tests
1. Navigate to the backend folder:
   ```bash
   cd backend
   ```
2. Run the full xUnit test suite (covering authentication validation, branch rules, membership purchase, and timeslot overlap checks):
   ```bash
   dotnet test
   ```

---

## 👤 Sample Roles & Logins
You can register new users directly from the login page! The platform supports the following Roles:
- **System Owner** (Administrative privileges to manage branches)
- **Branch Manager** (Administrative privileges to manage facilities)
- **Receptionist** (Front-desk operations)
- **Member** (Complimentary membership upgrades and court/pitch bookings)
- **Guest** (Free pitch/court bookings)

# 💻 Windows 11 Setup, Build, and Test Guide

This guide is tailored specifically to help you set up, build, run, and test the entire **Gym & Turf Management Platform** on a **Windows 11 PC** using **PowerShell** or **Command Prompt (CMD)**.

---

## 📋 Prerequisites for Windows 11
Ensure you have the following software installed on your machine:
1. **Docker Desktop for Windows** (with WSL 2 integration enabled) -> [Download Link](https://www.docker.com/products/docker-desktop/)
2. **.NET 8.0 SDK** (Windows Installer x64) -> [Download Link](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
3. **Node.js LTS** (Windows Installer .msi) -> [Download Link](https://nodejs.org/)

---

## 🛠️ Step 1: Start the Database & Caching Services (Docker)
We use Docker Compose to run PostgreSQL (port `5432`) and Redis (port `6379`) instantly.

1. Open **PowerShell** as Administrator.
2. Navigate to your project root folder:
   ```powershell
   cd \path\to\your\project-root
   ```
3. Run the container cluster in the background:
   ```powershell
   docker-compose up -d
   ```
4. Verify the containers are running:
   ```powershell
   docker ps
   ```
   *You should see `gymturf-postgres` and `gymturf-redis` up and healthy.*

---

## ⚙️ Step 2: Build and Launch the ASP.NET Core Backend
Our backend automatically handles migrations on startup.

1. In **PowerShell**, navigate to the `backend` folder:
   ```powershell
   cd backend
   ```
2. Restore package dependencies:
   ```powershell
   dotnet restore
   ```
3. Build the backend solutions:
   ```powershell
   dotnet build
   ```
4. Start the Web API server:
   ```powershell
   dotnet run --project GymTurf.API\GymTurf.API.csproj
   ```
5. **Verify the Backend is Running:**
   - Open your web browser and go to: **`http://localhost:5264/swagger`**
   - You should see the fully documented **Swagger API Playground** listing the Auth, Branch, Member, and Booking controller endpoints!

---

## 🎨 Step 3: Build and Run the React 19 Frontend
Now, let's start the Vite web client.

1. Open a **new PowerShell window** (leave the backend API terminal running).
2. Navigate to the `frontend` folder:
   ```powershell
   cd \path\to\your\project-root\frontend
   ```
3. Install the web dependencies:
   ```powershell
   npm install
   ```
4. Start the Vite hot-reloading development server:
   ```powershell
   npm run dev
   ```
5. **Verify the Frontend:**
   - Open your browser and go to: **`http://localhost:5173`**
   - Click **Register** on the navigation tab, choose a role like **System Owner** or **Member**, fill out the fields, and hit **Sign Up** to log in to your beautiful operational dashboard!

---

## 🧪 Step 4: How to Run the Automated Tests

To execute the full xUnit test suite (testing overlapping booking blocks, membership purchase dates, active subscription checks, and auth registers):

1. Open a terminal and navigate to the `backend` directory:
   ```powershell
   cd \path\to\your\project-root\backend
   ```
2. Execute the test runner:
   ```powershell
   dotnet test
   ```
   *You will see the test suite compile and run, reporting 100% successful passes!*

---

## 👥 Troubleshooting Common Windows 11 Port Errors
- **Error: "Port 5432 is already in use"** -> This means a local instance of PostgreSQL is already running on your Windows PC. Stop your local Windows service using:
  ```powershell
  Stop-Service -Name postgresql*
  ```
  Then rerun `docker-compose up -d`.

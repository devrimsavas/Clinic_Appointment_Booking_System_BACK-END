# Clinic Appointment Booking System BACK-END 

This is the **Back-end API** for the Clinic Appointment Booking system developed using **ASP.NET Core 8** and **Entity Framework Core**, with a **MySQL** database.

It allows patients to book appointments with doctors without authentication, following REST principles.

---

## Technologies Used

- ASP.NET Core 8
- Entity Framework Core
- MySQL
- Swagger (API Documentation)
- LINQ
- CORS

---

##  Setup & Configuration

### 1. Clone the Repository
```bash
git clone https://github.com/yourusername/clinic-booking-backend.git
cd clinic-booking-backend
```

### 2. Add `appsettings.json`
Create a new file named `appsettings.json` in the root folder:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=medicalclinics;user=root;password=yourpassword"
  }
}
```

>  This file is excluded from version control using `.gitignore`. Use `appsettings.json.sample.txt` for reference.

### 3. Run the Application
```bash
dotnet build
dotnet ef database update
dotnet run
```

The Swagger UI will be available at: https://localhost:{PORT}/swagger

---

##  Endpoints Overview

###  Appointments
| Method | Endpoint               | Description                     |
|--------|------------------------|---------------------------------|
| GET    | `/api/appointments`    | Get all appointments            |
| GET    | `/api/appointments/{id}` | Get an appointment by ID      |
| POST   | `/api/appointments`    | Create a new appointment        |
| PUT    | `/api/appointments/{id}` | Update an appointment         |
| DELETE | `/api/appointments/{id}` | Delete an appointment         |

### Patients
| Method | Endpoint           | Description                  |
|--------|--------------------|------------------------------|
| GET    | `/api/patients`    | Get all patients             |
| GET    | `/api/patients/{id}` | Get patient by ID         |
| POST   | `/api/patients`    | Create a patient             |
| PUT    | `/api/patients/{id}` | Update a patient           |
| DELETE | `/api/patients/{id}` | Delete a patient           |

###  Doctors
| Method | Endpoint         | Description              |
|--------|------------------|--------------------------|
| GET    | `/api/doctors`   | Get all doctors          |
| GET    | `/api/doctors/{id}` | Get doctor by ID     |
| POST   | `/api/doctors`   | Create a doctor          |
| PUT    | `/api/doctors/{id}` | Update a doctor       |
| DELETE | `/api/doctors/{id}` | Delete a doctor       |

###  Clinics
| Method | Endpoint         | Description              |
|--------|------------------|--------------------------|
| GET    | `/api/clinics`   | Get all clinics          |
| GET    | `/api/clinics/{id}` | Get clinic by ID     |
| POST   | `/api/clinics`   | Create a clinic          |
| PUT    | `/api/clinics/{id}` | Update a clinic       |
| DELETE | `/api/clinics/{id}` | Delete a clinic       |

###  Specialities
| Method | Endpoint             | Description                  |
|--------|----------------------|------------------------------|
| GET    | `/api/specialities`  | Get all specialities         |
| GET    | `/api/specialities/{id}` | Get by ID               |
| POST   | `/api/specialities`  | Create a new speciality      |
| PUT    | `/api/specialities/{id}` | Update a speciality     |
| DELETE | `/api/specialities/{id}` | Delete a speciality     |

###  Doctor Search
| Method | Endpoint               | Description                                  |
|--------|------------------------|----------------------------------------------|
| POST   | `/api/search/doctors`  | Search by first name or last name (no auth) |

Sample POST Body:
```json
{
  "firstName": "John",
  "lastName": "Doe"
}
```

---

##  Swagger
Visit the Swagger UI:
```
https://localhost:{PORT}/swagger
```
This shows request/response schemas, example requests, and testing tools.

---

##  CORS Configuration
All origins, headers, and methods are allowed:
```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
```
---

##  Front-end
To be implemented using **React.js**. API is designed to be consumed directly with fetch/axios.

---

##  Notes
- Only non-sensitive PII (Personal Identifiable Information) is stored.
- DTOs are used to shape responses and avoid entity leakage.
- XML comments included for Swagger documentation.

---

## 💼 Author
Devrim Savas Yilmaz 2025

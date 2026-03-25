# Estructura del Proyecto: Task_API

Este documento describe la arquitectura y organización de carpetas del proyecto **Task_API**, basado en **.NET 9.0** y siguiendo los principios de **Arquitectura Hexagonal / Limpia**.

## 🏗️ Arquitectura General

El proyecto está dividido en cuatro capas principales que gestionan la separación de responsabilidades:

1.  **Domain (Dominio)**: El núcleo del negocio. Contiene las reglas, entidades e interfaces fundamentales.
2.  **Application (Aplicación)**: Casos de uso y lógica de aplicación. Coordina el flujo de datos entre el dominio y las capas externas.
3.  **Infrastructure (Infraestructura)**: Implementaciones técnicas, servicios externos y persistencia.
4.  **Api (Presentación)**: El punto de entrada del sistema. Controladores REST, middlewares y configuración de la aplicación.

---

## 📁 Detalle de Carpetas

### 🟢 1. Api (Capa de Presentación)
Ubicación: `/Api`
- **Controllers/**: Endpoints de la API organizados por recurso:
  - `Seguridad/`: Controladores para Roles, Usuarios y **Webhooks de Clerk**.
  - `Catalogos/`: Controladores para categorías de tareas.
- **Middlewares/**: 
  - `ExceptionMiddleware`: Manejo global de errores.
  - `SyncUserMiddleware`: Sincronización JIT (Just-in-Time) de usuarios de Clerk con **Caché de Memoria**.
- **Program.cs**: Configuración del pipeline HTTP, Inyección de Dependencias (DI), y configuración de OpenAPI (Scalar). También registra los Jobs recurrentes de Hangfire.
- **appsettings.json**: Configuración del entorno, cadenas de conexión, JWT de Clerk, Hangfire y Email.

### 🟡 2. Application (Capa de Aplicación)
Ubicación: `/Application`
- **Features/**: Contiene la lógica de negocio dividida por módulos:
  - `Tasks/`: CRUD y gestión de tareas.
  - `Seguridad/`: 
    - `Interfaces/`: `IUserSyncService`, `ICurrentUserContext`.
    - `Operations/`: Implementación de la lógica de sincronización de usuarios.
  - `Mappings/`: Perfiles de AutoMapper u otros mapeadores.
- **DTOs/**: Objetos de transferencia de datos para entrada/salida de la API.
- **Results/**: Implementación del patrón Result para manejar éxitos y errores de forma consistente.
- **Exceptions/**: Excepciones personalizadas del negocio.
- **Extensions/**: Métodos de extensión para registrar servicios de esta capa (`AddServicesLayer`).

### 🔴 3. Domain (Capa de Dominio)
Ubicación: `/Domain`
- **Entities/**: Clases que representan las tablas de la base de datos:
  - `seguridad/`: `User` (con `ClerkId`), `Role`.
  - `Tasks/`: `TaskItem` (con `IsNotified`), `TaskCategory`.
- **Interfaces/**: Contratos de repositorios y servicios esenciales.
- **Enums/**: Definiciones de estados y tipos constantes.
- **SqlDbContext.cs**: El contexto de Entity Framework Core.
- **Migrations/**: Historial de cambios en el esquema de la base de datos.

### 🔵 4. Infrastructure (Capa de Infraestructura)
Ubicación: `/Infrastructure`
- **BackGroundJobs/**: Contiene la lógica de Hangfire y las implementaciones de los Jobs (e.g., `NotifyExpiringTasksJob`).
- **Services/**: Implementaciones de interfaces de aplicación:
  - `Email/`: `EmailSender` y `EmailNotificationService` (MailKit).
  - `JwtService.cs`: Generación y validación de tokens.
  - `Reports/`: Generación de reportes Excel.
- **Seeders/**: Lógica para poblar la base de datos con datos iniciales (`DbSeeder.cs`).
- **Extensions/**: Registro de servicios de infraestructura y configuración del DbContext (`AddInfrastructure`).

---

## 🛠️ Tecnologías Principales

- **Framework**: .NET 9.0
- **Auth Externa**: **Clerk** (Sincronización vía Middleware JIT y Webhooks).
- **ORM**: Entity Framework Core 9.0 (SQL Server)
- **Background Jobs**: Hangfire (con persistencia en SQL Server).
- **Email**: MailKit / MimeKit (SMTP).
- **Caché**: In-Memory Cache para optimización de Auth.
- **Logging**: Serilog.
- **Documentación**: OpenAPI (Swagger) + Scalar.

## 🚀 Cómo trabajar en este proyecto

1.  **Entidades**: Deben crearse en `Domain/Entities`.
2.  **Casos de Uso**: Deben implementarse en `Application/Features`.
3.  **Tareas Programadas**: Añadir nuevos Jobs en `Infrastructure/BackGroundJobs/Jobs` y registrarlos en `Program.cs`.
4.  **Inyección de Dependencias**: Asegúrate de actualizar los métodos de extensión al añadir nuevos servicios.

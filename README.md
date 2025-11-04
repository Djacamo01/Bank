# Lafise Banking System API

Sistema bancario REST API desarrollado con ASP.NET Core 8 que implementa funcionalidades de gestión de clientes, cuentas y transacciones.

## 📋 Descripción

Este proyecto es un sistema bancario completo que incluye:

- **Gestión de Clientes**: Registro y autenticación de usuarios
- **Gestión de Cuentas**: Creación y consulta de cuentas bancarias (Savings, Checking, MoneyMarket)
- **Transacciones**: Depósitos, retiros y transferencias entre cuentas
- **Autenticación JWT**: Sistema seguro de autenticación con tokens

## 🏗️ Estructura del Proyecto

```
Lafise/
├── Lafise.API/          # Proyecto principal de la API REST
├── Lafise.Tests/        # Proyecto de pruebas unitarias
└── README.md           # Este archivo
```

### Proyectos

- **Lafise.API**: API REST desarrollada con ASP.NET Core 8
  - Controllers: Auth, Client, Account, Transaction
  - Servicios: Lógica de negocio organizada por módulos
  - Base de datos: SQLite con Entity Framework Core
  - Autenticación: JWT Bearer Tokens

- **Lafise.Tests**: Pruebas unitarias con xUnit
  - Tests de servicios (AccountService, TransactionService)
  - Base de datos en memoria para tests aislados
  - Mocks con Moq y aserciones con FluentAssertions

## 🔧 Requisitos Previos

- **.NET 8 SDK** o superior
  - Verifica tu versión: `dotnet --version`
  - Descarga: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
- **Git** (para clonar el repositorio)
- **Visual Studio 2022** o **Visual Studio Code** (opcional)

## 🚀 Inicio Rápido

### 1. Clonar el Repositorio

```bash
git clone https://github.com/tu-usuario/Lafise.git
cd Lafise
```

### 2. Restaurar Dependencias

```bash
dotnet restore
```

### 3. Configurar Base de Datos

La base de datos SQLite se crea automáticamente. Si necesitas aplicar migraciones:

```bash
cd Lafise.API
dotnet ef database update
```

**Nota:** Si no tienes la herramienta EF Core CLI instalada:

```bash
dotnet tool install --global dotnet-ef
```

### 4. Ejecutar la API

```bash
# Desde la raíz del proyecto
dotnet run --project Lafise.API/Lafise.API.csproj

# O desde el directorio de la API
cd Lafise.API
dotnet run
```

La API estará disponible en:

- **HTTP**: `http://localhost:5135`
- **HTTPS**: `https://localhost:7233`
- **Documentación Interactiva**: `http://localhost:5135/docs.html` (Recomendado)
- **Swagger UI**: `https://localhost:7233/swagger` (Alternativa)

### 5. Ejecutar las Pruebas

```bash
# Ejecutar todos los tests
dotnet test

# O específicamente el proyecto de tests
dotnet test Lafise.Tests/Lafise.Tests.csproj

# Con información detallada
dotnet test --verbosity normal
```

## 📚 Flujo de Trabajo

### 1. Crear un Cliente

```bash
POST /api/clients
Content-Type: application/json

{
  "name": "Juan",
  "lastName": "Perez",
  "taxId": "001-231019-0007E",
  "email": "juan.perez@email.com",
  "password": "StrongPassword123!",
  "dateOfBirth": "1991-10-23",
  "gender": "M",
  "income": 52000.80,
  "accountType": "Savings"
}
```

### 2. Autenticarse

```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "juan.perez@email.com",
  "password": "StrongPassword123!"
}
```

**⚠️ Importante:** El token JWT expira en **1 hora**. Guarda el `refreshToken` para renovarlo.

### 3. Usar Endpoints Protegidos

Todos los endpoints (excepto crear cliente y login) requieren el token JWT:

```bash
Authorization: Bearer {tu-token-jwt}
```

## 🔐 Endpoints Principales

### Públicos (No requieren autenticación)

- `POST /api/clients` - Crear nuevo cliente
- `POST /api/auth/login` - Autenticarse
- `POST /api/auth/refresh` - Renovar token

### Protegidos (Requieren token JWT)

**Cuentas:**
- `POST /api/accounts/create?accountType=Savings` - Crear cuenta
- `GET /api/accounts/{accountNumber}/balance` - Consultar saldo
- `GET /api/accounts/{accountNumber}/movements` - Ver movimientos

**Transacciones:**
- `POST /api/transactions/deposit` - Realizar depósito
- `POST /api/transactions/withdraw` - Realizar retiro
- `POST /api/transactions/transfer` - Transferir fondos

**Clientes:**
- `GET /api/clients/summary` - Resumen de cuentas y transacciones

## 🧪 Pruebas

### Ejecutar Tests

```bash
# Todos los tests
dotnet test

# Tests específicos
dotnet test --filter "FullyQualifiedName~AccountServiceTests"

# Modo watch (auto-ejecución)
dotnet watch test
```

### Cobertura de Tests

Los tests cubren:
- ✅ Creación y gestión de cuentas
- ✅ Depósitos, retiros y transferencias
- ✅ Validaciones de negocio
- ✅ Casos exitosos y de error

Para más detalles sobre los tests, consulta: [Lafise.Tests/README.md](Lafise.Tests/README.md)

## 📖 Documentación

- **API Completa**: [Lafise.API/README.md](Lafise.API/README.md) - Guía detallada de la API
- **Tests**: [Lafise.Tests/README.md](Lafise.Tests/README.md) - Guía de pruebas unitarias
- **Datos de Prueba**: [Lafise.API/TEST_DATA.md](Lafise.API/TEST_DATA.md) - Ejemplos de datos

## 🛠️ Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **ASP.NET Core Web API** - Framework para REST API
- **Entity Framework Core** - ORM para acceso a datos
- **SQLite** - Base de datos
- **JWT Bearer** - Autenticación
- **xUnit** - Framework de testing
- **Moq** - Mocking para tests
- **FluentAssertions** - Aserciones legibles
- **Swagger/OpenAPI** - Documentación de API
- **AutoMapper** - Mapeo de objetos

## 📦 Compilar el Proyecto

```bash
# Compilar la solución completa
dotnet build

# Compilar solo la API
dotnet build Lafise.API/Lafise.API.csproj

# Compilar solo los tests
dotnet build Lafise.Tests/Lafise.Tests.csproj
```

## 🔄 Comandos Git Útiles

```bash
# Clonar el repositorio
git clone https://github.com/tu-usuario/Lafise.git

# Ver estado de cambios
git status

# Agregar cambios
git add .

# Crear commit
git commit -m "Descripción del cambio"

# Subir cambios
git push origin main

# Actualizar desde el repositorio
git pull origin main
```

## 📝 Notas Importantes

- **Token JWT**: Expira en 1 hora. Usa `/api/auth/refresh` para renovarlo.
- **Base de Datos**: Se crea automáticamente en `Lafise.API/lafise.db`
- **Tipos de Cuenta**: Savings, Checking, MoneyMarket
- **Documentación**: Usa `docs.html` para mejor experiencia de navegación

## 🤝 Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto es parte de una prueba técnica. El código y las implementaciones son de autoría individual exclusivamente con fines evaluativos y demostrativos.

---

**Desarrollado con ❤️ usando .NET 8**

**Última actualización**: Noviembre 2024

# Lafise Banking System API - Guía de Ejecución y Uso

API REST desarrollada con ASP.NET Core 8 que implementa funcionalidades bancarias básicas, incluyendo gestión de clientes, cuentas y transacciones.

## 📋 Tabla de Contenidos

1. [Requisitos Previos](#requisitos-previos)
2. [Instalación y Configuración](#instalación-y-configuración)
3. [Ejecución del Proyecto](#ejecución-del-proyecto)
4. [Flujo de Trabajo Principal](#flujo-de-trabajo-principal)
5. [Módulos del Sistema](#módulos-del-sistema)
6. [Endpoints Disponibles](#endpoints-disponibles)
7. [Autenticación y Autorización](#autenticación-y-autorización)
8. [Ejemplos de Uso](#ejemplos-de-uso)
9. [Documentación Adicional](#documentación-adicional)

---

## 🔧 Requisitos Previos

Antes de ejecutar el proyecto, asegúrate de tener instalado:

- **.NET 8 SDK** o superior
  - Puedes verificar la versión instalada ejecutando: `dotnet --version`
  - Descargar desde: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
- **Visual Studio 2022** (opcional pero recomendado) o **Visual Studio Code**
- **Git** (para clonar el repositorio)

---

## 📦 Instalación y Configuración

### Paso 1: Clonar el Repositorio

Si el proyecto aún no está en tu máquina local:

```bash
git clone https://github.com/Djacamo01/Bank.git
cd Bank
```

### Paso 2: Verificar la Base de Datos

El proyecto utiliza SQLite como base de datos. La base de datos se crea automáticamente al ejecutar las migraciones.

La base de datos se encuentra en: `Lafise.API/lafise.db`

### Paso 3: Aplicar Migraciones (si es necesario)

Si es la primera vez que ejecutas el proyecto o si hay nuevas migraciones:

```bash
cd Lafise.API
dotnet ef database update
```

**Nota:** Si no tienes la herramienta de Entity Framework CLI instalada globalmente:

```bash
dotnet tool install --global dotnet-ef
```

### Paso 4: Verificar Configuración

Revisa el archivo `appsettings.json` para asegurarte de que la configuración sea correcta:

- `db-cnstr-bank`: Cadena de conexión a la base de datos SQLite
- `jwt-token-secret-key`: Clave secreta para generar tokens JWT
- `jwt-issuer` y `jwt-audience`: Configuración del emisor y audiencia para JWT
- `AccountSettings`: Tipos de cuenta válidos (Savings, Checking, MoneyMarket)

---

## 🚀 Ejecución del Proyecto

### Opción 1: Desde la Línea de Comandos

1. Navega al directorio del proyecto API:

```bash
cd Lafise.API
```

2. Restaura las dependencias (si es necesario):

```bash
dotnet restore
```

3. Ejecuta el proyecto:

```bash
dotnet run
```

O para ejecutar en modo HTTPS:

```bash
dotnet run --launch-profile https
```

### Opción 2: Desde Visual Studio

1. Abre el archivo `Lafise.sln` en Visual Studio
2. Selecciona el proyecto `Lafise.API` como proyecto de inicio
3. Presiona `F5` o haz clic en "Ejecutar"

### Puertos de Ejecución

Una vez ejecutado, la API estará disponible en:

- **HTTP**: `http://localhost:5135`
- **HTTPS**: `https://localhost:7233`
- **Documentación Interactiva (Recomendada)**: `http://localhost:5135/docs.html` o `https://localhost:7233/docs.html`
- **Swagger UI (Alternativa)**: `https://localhost:7233/swagger` o `http://localhost:5135/swagger`

### Verificar que la API está Funcionando

**🌟 Recomendado:** Abre tu navegador y visita la documentación interactiva mejorada:
- **URL**: `http://localhost:5135/docs.html` o `https://localhost:7233/docs.html`

Esta documentación ofrece una mejor experiencia de usuario con navegación mejorada, mejor formato y diseño más moderno.

**Alternativa:** También puedes usar Swagger UI tradicional en: `https://localhost:7233/swagger`

---

## 🔄 Flujo de Trabajo Principal

**⚠️ IMPORTANTE:** Para usar la API correctamente, debes seguir este orden específico:

### 1️⃣ Crear un Nuevo Cliente

**Primero**, debes crear un cliente en el sistema. Este es el único endpoint que NO requiere autenticación.

**Endpoint:** `POST /api/clients`

El cliente se crea con una cuenta inicial del tipo especificado.

### 2️⃣ Autenticarse (Login)

**Segundo**, una vez creado el cliente, debes autenticarte para obtener un token JWT.

**Endpoint:** `POST /api/auth/login`

- Proporciona el `email` y `password` del cliente creado
- Recibirás un token JWT que **expira en 1 hora (60 minutos)**
- También recibirás un `refreshToken` para renovar el token cuando expire

### 3️⃣ Usar los Endpoints Protegidos

**Tercero**, con el token obtenido, puedes acceder a todos los demás endpoints:

- **Cuentas**: Crear cuentas, consultar saldos, ver movimientos
- **Transacciones**: Depositar, retirar, transferir
- **Clientes**: Ver resumen de cuentas y transacciones

**⚠️ Recordatorio:** Todos estos endpoints requieren el token JWT en el header `Authorization: Bearer {token}`

### 4️⃣ Renovar el Token (cuando expire)

Si el token expira (después de 1 hora), puedes usar el `refreshToken` para obtener uno nuevo:

**Endpoint:** `POST /api/auth/refresh`

---

## 📚 Módulos del Sistema

### 🔐 Módulo de Autenticación (`/services/Auth`)

**Funcionalidad:**
- Gestiona la autenticación de usuarios mediante JWT (JSON Web Tokens)
- Valida credenciales (email y contraseña)
- Genera tokens de acceso con expiración de 1 hora
- Proporciona funcionalidad de refresh token para renovar tokens expirados
- Encripta y verifica contraseñas usando hashing seguro

**Componentes principales:**
- `AuthService`: Lógica de negocio de autenticación
- `JwtTokenGenerator`: Generación de tokens JWT
- `Cryptor`: Utilidad para encriptación de contraseñas

---

### 👥 Módulo de Clientes (`/services/Clients`)

**Funcionalidad:**
- Creación de nuevos clientes en el sistema
- Validación de datos del cliente (email único, taxId único, etc.)
- Creación automática de cuenta inicial al registrar un cliente
- Consulta de resumen completo de cuentas y transacciones del cliente autenticado
- Cálculo de estadísticas de transacciones por cuenta

**Componentes principales:**
- `ClientService`: Servicio principal de gestión de clientes
- `ClientRepository`: Acceso a datos de clientes
- `ClientFactory`: Factory para crear instancias de clientes
- `ClientCreationValidator`: Validación de datos al crear clientes

---

### 💳 Módulo de Cuentas (`/services/Accounts`)

**Funcionalidad:**
- Creación de nuevas cuentas bancarias (Savings, Checking, MoneyMarket)
- Consulta de saldo de cuentas específicas
- Obtención de movimientos (transacciones) de una cuenta con paginación
- Generación automática de números de cuenta únicos
- Validación de propiedad de cuentas (solo el dueño puede acceder a sus cuentas)

**Tipos de cuenta disponibles:**
- **Savings**: Cuenta de ahorros
- **Checking**: Cuenta corriente
- **MoneyMarket**: Cuenta de mercado monetario

**Componentes principales:**
- `AccountService`: Servicio principal de gestión de cuentas
- `AccountRepository`: Acceso a datos de cuentas
- `AccountFactory`: Factory para crear instancias de cuentas
- `AccountNumberGenerator`: Generador de números de cuenta únicos

---

### 💰 Módulo de Transacciones (`/services/Transactions`)

**Funcionalidad:**
- **Depósitos**: Agregar fondos a una cuenta propia
- **Retiros**: Extraer fondos de una cuenta propia (con validación de saldo suficiente)
- **Transferencias**: Transferir fondos entre cuentas (desde cuenta propia a cualquier cuenta)
- Consulta de historial de transacciones con paginación
- Validación de saldo suficiente antes de retiros y transferencias
- Validación de propiedad de cuenta (solo puedes operar en tus propias cuentas)

**Tipos de transacciones:**
- `Deposit`: Depósito a cuenta
- `Withdrawal`: Retiro de cuenta
- `TransferOut`: Transferencia saliente (cuenta origen)
- `TransferIn`: Transferencia entrante (cuenta destino)

**Componentes principales:**
- `TransactionService`: Servicio principal de gestión de transacciones
- `TransactionRepository`: Acceso a datos de transacciones
- `TransactionFactory`: Factory para crear instancias de transacciones
- `TransactionValidator`: Validación de reglas de negocio para transacciones

---

## 🌐 Endpoints Disponibles

### Endpoints Públicos (No requieren autenticación)

#### 1. Crear Cliente
```
POST /api/clients
Content-Type: application/json

Body:
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

**Respuesta exitosa (200 OK):**
```json
{
  "id": "51b9ecc1-f701-4f67-8554-73ff0db565fc",
  "name": "Juan",
  "lastName": "Perez",
  "email": "juan.perez@email.com",
  "dateOfBirth": "1991-10-23",
  "gender": "M",
  "income": 52000.80,
  "accountNumber": "1000000",
  "dateCreated": "2024-11-04T10:30:00Z",
  "dateModified": null
}
```

---

### Endpoints de Autenticación

#### 2. Login (Autenticarse)
```
POST /api/auth/login
Content-Type: application/json

Body:
{
  "email": "juan.perez@email.com",
  "password": "StrongPassword123!"
}
```

**Respuesta exitosa (200 OK):**
```json
{
  "userId": "51b9ecc1-f701-4f67-8554-73ff0db565fc",
  "userName": "Juan Perez",
  "userEmail": "juan.perez@email.com",
  "authToken": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123...",
  "authTokenExpiration": "2024-11-04T11:30:00Z"
}
```

**⚠️ IMPORTANTE:**
- El `authToken` expira en **1 hora (60 minutos)**
- Guarda el `refreshToken` para renovar el token cuando expire
- Usa el `authToken` en el header `Authorization: Bearer {token}` para todos los endpoints protegidos

#### 3. Renovar Token
```
POST /api/auth/refresh
Content-Type: application/json

Body:
{
  "refreshToken": "abc123..."
}
```

**Respuesta exitosa (200 OK):**
```json
{
  "userId": "51b9ecc1-f701-4f67-8554-73ff0db565fc",
  "userName": "Juan Perez",
  "userEmail": "juan.perez@email.com",
  "authToken": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "xyz789...",
  "authTokenExpiration": "2024-11-04T12:30:00Z"
}
```

---

### Endpoints Protegidos (Requieren token JWT)

**⚠️ Todos los siguientes endpoints requieren el header:**
```
Authorization: Bearer {tu-token-jwt}
```

#### 4. Crear Nueva Cuenta
```
POST /api/accounts/create?accountType=Savings
Authorization: Bearer {token}
```

**Parámetros de consulta:**
- `accountType`: Tipo de cuenta (`Savings`, `Checking`, `MoneyMarket`)

**Respuesta exitosa (200 OK):**
```json
{
  "accountNumber": "1000001",
  "accountType": "Savings",
  "balance": 0.00,
  "dateCreated": "2024-11-04T10:35:00Z"
}
```

#### 5. Consultar Saldo de Cuenta
```
GET /api/accounts/{accountNumber}/balance
Authorization: Bearer {token}
```

**Ejemplo:**
```
GET /api/accounts/1000000/balance
Authorization: Bearer {token}
```

**Respuesta exitosa (200 OK):**
```json
{
  "accountNumber": "1000000",
  "accountType": "Savings",
  "balance": 1500.50,
  "lastUpdated": "2024-11-04T10:40:00Z"
}
```

#### 6. Ver Movimientos de Cuenta
```
GET /api/accounts/{accountNumber}/movements?page=1&pageSize=10
Authorization: Bearer {token}
```

**Parámetros de consulta:**
- `page`: Número de página (default: 1)
- `pageSize`: Tamaño de página (default: 10, máximo: 100)

**Ejemplo:**
```
GET /api/accounts/1000000/movements?page=1&pageSize=10
Authorization: Bearer {token}
```

**Respuesta exitosa (200 OK):**
```json
{
  "data": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "accountNumber": "1000000",
      "transactionType": "Deposit",
      "amount": 500.00,
      "description": "Deposit transaction",
      "dateCreated": "2024-11-04T10:40:00Z"
    }
  ],
  "summary": {
    "totalDeposits": 500.00,
    "totalWithdrawals": 0.00,
    "totalTransfersOut": 0.00,
    "totalTransfersIn": 0.00
  },
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 1,
    "totalPages": 1
  }
}
```

#### 7. Realizar Depósito
```
POST /api/transactions/deposit
Authorization: Bearer {token}
Content-Type: application/json

Body:
{
  "accountNumber": "1000000",
  "amount": 500.00,
  "description": "Deposito inicial"
}
```

**Respuesta exitosa (200 OK):**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "accountNumber": "1000000",
  "transactionType": "Deposit",
  "amount": 500.00,
  "description": "Deposito inicial",
  "dateCreated": "2024-11-04T10:45:00Z"
}
```

#### 8. Realizar Retiro
```
POST /api/transactions/withdraw
Authorization: Bearer {token}
Content-Type: application/json

Body:
{
  "accountNumber": "1000000",
  "amount": 100.00,
  "description": "Retiro de efectivo"
}
```

**⚠️ Validaciones:**
- La cuenta debe pertenecer al usuario autenticado
- La cuenta debe tener saldo suficiente

#### 9. Realizar Transferencia
```
POST /api/transactions/transfer
Authorization: Bearer {token}
Content-Type: application/json

Body:
{
  "fromAccountNumber": "1000000",
  "toAccountNumber": "1000001",
  "amount": 200.00,
  "description": "Transferencia a otra cuenta"
}
```

**⚠️ Validaciones:**
- La cuenta origen (`fromAccountNumber`) debe pertenecer al usuario autenticado
- La cuenta origen debe tener saldo suficiente
- La cuenta destino puede pertenecer a cualquier usuario

**Respuesta exitosa (200 OK):**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174001",
  "accountNumber": "1000000",
  "transactionType": "TransferOut",
  "amount": 200.00,
  "description": "Transferencia a otra cuenta",
  "dateCreated": "2024-11-04T10:50:00Z"
}
```

#### 10. Ver Movimientos (Alternativa)
```
GET /api/transactions/movements/{accountNumber}?page=1&pageSize=10
Authorization: Bearer {token}
```

#### 11. Resumen de Cliente
```
GET /api/clients/summary
Authorization: Bearer {token}
```

**Respuesta exitosa (200 OK):**
```json
{
  "client": {
    "id": "51b9ecc1-f701-4f67-8554-73ff0db565fc",
    "name": "Juan",
    "lastName": "Perez",
    "email": "juan.perez@email.com"
  },
  "accounts": [
    {
      "accountNumber": "1000000",
      "accountType": "Savings",
      "balance": 1300.50,
      "transactionSummary": {
        "totalDeposits": 500.00,
        "totalWithdrawals": 100.00,
        "totalTransfersOut": 200.00,
        "totalTransfersIn": 0.00
      }
    }
  ],
  "totalBalance": 1300.50,
  "totalAccounts": 1
}
```

---

## 🔒 Autenticación y Autorización

### Cómo Funciona la Autenticación

1. **Creación de Cliente**: Al crear un cliente, se genera automáticamente:
   - Un hash seguro de la contraseña (usando sal y hash)
   - Una cuenta inicial del tipo especificado
   - Un número de cuenta único

2. **Login**: Al autenticarse:
   - Se valida el email y contraseña
   - Se genera un token JWT con expiración de **1 hora**
   - Se genera un refresh token
   - El token incluye información del usuario (ID, email, nombre, número de cuenta)

3. **Uso del Token**: En cada petición a endpoints protegidos:
   - Incluye el header: `Authorization: Bearer {token}`
   - El servidor valida el token
   - Extrae la información del usuario del token
   - Verifica que el usuario tenga permisos para la operación

4. **Renovación del Token**: Cuando el token expira:
   - Usa el endpoint `/api/auth/refresh` con el `refreshToken`
   - Obtendrás un nuevo token y refresh token

### Ejemplo de Uso del Token en Peticiones

**Con cURL:**
```bash
curl -X GET "https://localhost:7233/api/accounts/1000000/balance" \
  -H "Authorization: Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9..."
```

**Con Postman:**
1. Ve a la pestaña "Authorization"
2. Selecciona "Bearer Token"
3. Pega tu token en el campo "Token"

**Con la Documentación Interactiva (docs.html):**
1. Haz clic en el botón "Authorize" (🔒) en la parte superior
2. Ingresa tu token en el formato: `Bearer {token}` o simplemente `{token}`
3. Haz clic en "Authorize"

**Con Swagger UI (alternativa):**
1. Haz clic en el botón "Authorize" (🔒) en la parte superior
2. Ingresa tu token en el formato: `Bearer {token}` o simplemente `{token}`
3. Haz clic en "Authorize"

---

## 📝 Ejemplos de Uso Completo

### Ejemplo 1: Flujo Completo de un Nuevo Usuario

```bash
# 1. Crear cliente
curl -X POST "https://localhost:7233/api/clients" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Juan",
    "lastName": "Perez",
    "taxId": "001-231019-0007E",
    "email": "juan.perez@email.com",
    "password": "StrongPassword123!",
    "dateOfBirth": "1991-10-23",
    "gender": "M",
    "income": 52000.80,
    "accountType": "Savings"
  }'

# Respuesta: Guarda el accountNumber (ej: "1000000")

# 2. Autenticarse
curl -X POST "https://localhost:7233/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "juan.perez@email.com",
    "password": "StrongPassword123!"
  }'

# Respuesta: Guarda el authToken y refreshToken

# 3. Consultar saldo de la cuenta inicial
curl -X GET "https://localhost:7233/api/accounts/1000000/balance" \
  -H "Authorization: Bearer {TU_TOKEN_AQUI}"

# 4. Realizar un depósito
curl -X POST "https://localhost:7233/api/transactions/deposit" \
  -H "Authorization: Bearer {TU_TOKEN_AQUI}" \
  -H "Content-Type: application/json" \
  -d '{
    "accountNumber": "1000000",
    "amount": 1000.00,
    "description": "Deposito inicial"
  }'

# 5. Crear una segunda cuenta
curl -X POST "https://localhost:7233/api/accounts/create?accountType=Checking" \
  -H "Authorization: Bearer {TU_TOKEN_AQUI}"

# 6. Consultar resumen completo
curl -X GET "https://localhost:7233/api/clients/summary" \
  -H "Authorization: Bearer {TU_TOKEN_AQUI}"
```

### Ejemplo 2: Renovar Token Expirado

```bash
# Si el token expiró (después de 1 hora), renueva con el refresh token
curl -X POST "https://localhost:7233/api/auth/refresh" \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "TU_REFRESH_TOKEN_AQUI"
  }'

# Respuesta: Nuevo authToken y refreshToken
```

---

## 📖 Documentación Adicional

### Documentación Interactiva

**🌟 Recomendado:** La documentación interactiva mejorada de la API está disponible en:

- **URL**: `http://localhost:5135/docs.html` o `https://localhost:7233/docs.html`
- **Funcionalidades**:
  - Interfaz moderna y mejor diseñada
  - Navegación mejorada con sidebar
  - Ver todos los endpoints disponibles
  - Probar endpoints directamente desde el navegador
  - Ver esquemas de request/response de forma más clara
  - Autenticarse con tu token JWT
  - Soporte para modo oscuro/claro

**Alternativa:** También puedes usar Swagger UI tradicional:

- **URL**: `https://localhost:7233/swagger` o `http://localhost:5135/swagger`

### Archivos de Referencia

- **TEST_DATA.md**: Contiene ejemplos de datos de prueba para crear clientes
- **appsettings.json**: Configuración de la aplicación (base de datos, JWT, tipos de cuenta)
- **Properties/launchSettings.json**: Configuración de perfiles de ejecución

### Base de Datos

- **Motor**: SQLite
- **Ubicación**: `Lafise.API/lafise.db`
- **Migraciones**: Se encuentran en `Lafise.API/Migrations/`

### Pruebas Unitarias

Para ejecutar las pruebas unitarias:

```bash
dotnet test
```

O específicamente el proyecto de pruebas:

```bash
dotnet test Lafise.Tests/Lafise.Tests.csproj
```

---

## ⚠️ Consideraciones Importantes

### Validaciones de Seguridad

1. **Contraseñas**: Deben cumplir con requisitos mínimos:
   - Mínimo 8 caracteres
   - Al menos una mayúscula
   - Al menos una minúscula
   - Al menos un número
   - Al menos un carácter especial

2. **Email y TaxId**: Deben ser únicos en el sistema

3. **Propiedad de Cuentas**: Solo puedes operar en tus propias cuentas (excepto transferencias de destino)

4. **Saldo Suficiente**: Se valida antes de retiros y transferencias

### Expiración de Tokens

- **Token JWT**: Expira en **1 hora (60 minutos)** desde su creación
- **Refresh Token**: También expira en 1 hora
- **Recomendación**: Guarda el refresh token y renueva el token antes de que expire

### Códigos de Estado HTTP

- **200 OK**: Operación exitosa
- **400 Bad Request**: Error en los datos enviados
- **401 Unauthorized**: Token inválido o expirado
- **403 Forbidden**: No tienes permisos para la operación
- **404 Not Found**: Recurso no encontrado
- **500 Internal Server Error**: Error del servidor

---

## 🛠️ Solución de Problemas

### La API no inicia

1. Verifica que .NET 8 SDK esté instalado: `dotnet --version`
2. Verifica que los puertos 5135 y 7233 no estén en uso
3. Revisa los logs en la consola para errores específicos

### Error de Base de Datos

1. Asegúrate de que el archivo `lafise.db` tenga permisos de escritura
2. Ejecuta las migraciones: `dotnet ef database update`

### Error 401 Unauthorized

1. Verifica que el token esté incluido en el header `Authorization`
2. Verifica que el token no haya expirado (máximo 1 hora)
3. Usa el endpoint `/api/auth/refresh` para obtener un nuevo token

### Error 403 Forbidden

- Estás intentando operar en una cuenta que no te pertenece
- Solo puedes depositar/retirar en tus propias cuentas

### Error 404 Not Found

- El recurso solicitado no existe (cuenta, cliente, etc.)
- Verifica que los números de cuenta sean correctos

---

## 📞 Soporte

Para más información o problemas, consulta:

- **Documentación Interactiva (Recomendada)**: `http://localhost:5135/docs.html` o `https://localhost:7233/docs.html`
- **Swagger UI (Alternativa)**: `https://localhost:7233/swagger` o `http://localhost:5135/swagger`
- Archivo TEST_DATA.md para ejemplos de datos
- Logs de la aplicación en la consola

---

## 📄 Licencia

Este proyecto es parte de una prueba técnica. El código y las implementaciones son de autoría individual exclusivamente con fines evaluativos y demostrativos.

---

**Última actualización**: Noviembre 2024


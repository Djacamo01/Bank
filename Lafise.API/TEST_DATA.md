# Datos de Prueba para la API Lafise

## Crear Cliente (POST /api/clients)

### Cliente 1 - Juan Pérez (Savings)
```json
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

### Cliente 2 - María González (Checking)
```json
{
  "name": "Maria",
  "lastName": "Gonzalez",
  "taxId": "002-150875-0001K",
  "email": "maria.gonzalez@email.com",
  "password": "SecurePass456@",
  "dateOfBirth": "1988-05-15",
  "gender": "F",
  "income": 75000.50,
  "accountType": "Checking"
}
```

### Cliente 3 - Carlos Rodríguez (MoneyMarket)
```json
{
  "name": "Carlos",
  "lastName": "Rodriguez",
  "taxId": "003-280392-0002M",
  "email": "carlos.rodriguez@email.com",
  "password": "MySecure123!",
  "dateOfBirth": "1995-03-20",
  "gender": "M",
  "income": 45000.00,
  "accountType": "MoneyMarket"
}
```

### Cliente 4 - Ana Martínez (Savings)
```json
{
  "name": "Ana",
  "lastName": "Martinez",
  "taxId": "004-120490-0003P",
  "email": "ana.martinez@email.com",
  "password": "AnaPass2024!",
  "dateOfBirth": "1990-12-08",
  "gender": "F",
  "income": 68000.75,
  "accountType": "Savings"
}
```

### Cliente 5 - Luis Fernández (Checking)
```json
{
  "name": "Luis",
  "lastName": "Fernandez",
  "taxId": "005-051187-0004R",
  "email": "luis.fernandez@email.com",
  "password": "LuisSecure789!",
  "dateOfBirth": "1987-11-05",
  "gender": "M",
  "income": 92000.00,
  "accountType": "Checking"
}
```

### Cliente 6 - Sofía López (Savings)
```json
{
  "name": "Sofia",
  "lastName": "Lopez",
  "taxId": "006-200195-0005T",
  "email": "sofia.lopez@email.com",
  "password": "SofiaPass2024!",
  "dateOfBirth": "1995-02-14",
  "gender": "F",
  "income": 55000.25,
  "accountType": "Savings"
}
```

## Tipos de Cuenta Válidos
- `Savings` - Cuenta de ahorros
- `Checking` - Cuenta corriente
- `MoneyMarket` - Cuenta de mercado monetario

## Notas Importantes
- El `taxId` debe ser único (no puede repetirse)
- El `email` debe ser único
- El `password` debe cumplir con los requisitos de validación (mínimo 8 caracteres, mayúsculas, minúsculas, números y caracteres especiales)
- El `dateOfBirth` debe estar en formato ISO 8601 (YYYY-MM-DD)
- El `gender` puede ser "M" (Masculino) o "F" (Femenino)
- El `income` es un valor decimal

## Ejemplo de Respuesta Exitosa
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


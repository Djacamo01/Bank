# Guía de Tests Unitarios - Lafise API

Guía para ejecutar y trabajar con los tests unitarios del proyecto.

## 🔧 Requisitos

- .NET 8 SDK o superior
- Visual Studio 2022 o Visual Studio Code

## 📁 Estructura

```
Lafise.Tests/
├── Services/
│   ├── AccountServiceTests.cs      # Tests de cuentas
│   └── TransactionServiceTests.cs  # Tests de transacciones
└── README.md
```

## 🚀 Ejecutar Tests

### Visual Studio

1. Abre `Lafise.sln`
2. **Test** > **Run All Tests** (o `Ctrl+R, A`)
3. Ver resultados en **Test Explorer**

### Línea de Comandos

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests específicos
dotnet test --filter "FullyQualifiedName~AccountServiceTests"

# Modo watch (auto-ejecución)
dotnet watch test
```

### Visual Studio Code

1. Instala **.NET Extension Pack** y **xUnit Test Explorer**
2. `Ctrl+Shift+P` > `.NET: Run Tests`

## 📐 Estructura de Tests

Los tests siguen el patrón **AAA (Arrange-Act-Assert)**:

```csharp
[Fact]
public async Task CreateAccount_WithValidData_CreatesAccount()
{
    // Arrange: Configurar datos
    var client = new Client { /* ... */ };
    _context.Clients.Add(client);
    await _context.SaveChangesAsync();

    // Act: Ejecutar método
    var result = await _accountService.CreateAccount("Savings");

    // Assert: Verificar resultado
    result.Should().NotBeNull();
    result.AccountNumber.Should().NotBeNullOrEmpty();
}
```

## 🛠️ Tecnologías

| Tecnología | Propósito |
|------------|-----------|
| xUnit | Framework de testing |
| Moq | Creación de mocks |
| FluentAssertions | Aserciones legibles |
| EF Core InMemory | Base de datos en memoria |

## 📊 Tests Disponibles

### AccountServiceTests

- **Creación**: `CreateAccount_WithValidData_CreatesAccount`, `CreateAccount_WithInvalidAccountType_ThrowsException`
- **Saldos**: `GetAccountBalance_WithValidAccountNumber_ReturnsBalance`
- **Movimientos**: `GetAccountMovements_WithValidAccount_ReturnsTransactionHistory`

### TransactionServiceTests

- **Depósitos**: `Deposit_WithValidRequest_CreatesTransactionAndUpdatesBalance`
- **Retiros**: `Withdraw_WithValidRequest_CreatesTransactionAndUpdatesBalance`, `Withdraw_WithInsufficientFunds_ThrowsException`

## 🎯 Mejores Prácticas

1. **Nombres descriptivos**: `MethodName_Scenario_ExpectedBehavior`
2. **Patrón AAA**: Arrange → Act → Assert
3. **Tests aislados**: Cada test usa su propia BD en memoria
4. **FluentAssertions**: `result.Should().NotBeNull()` en lugar de `Assert.NotNull(result)`
5. **Limpieza**: Implementar `IDisposable` para liberar recursos

## 💡 Ejemplo Completo

```csharp
[Fact]
public async Task CreateAccount_WithValidData_CreatesAccount()
{
    // Arrange
    var client = new Client
    {
        Id = "client-1",
        Name = "John",
        Email = "john@example.com",
        // ... otros campos
    };
    _context.Clients.Add(client);
    await _context.SaveChangesAsync();
    _mockAuthInfo.Setup(x => x.UserId()).Returns("client-1");

    // Act
    var result = await _accountService.CreateAccount("Savings");

    // Assert
    result.Should().NotBeNull();
    result.AccountType.Should().Be("Savings");
    result.Balance.Should().Be(0m);
}
```

## 🔍 Solución de Problemas

### Tests no se ejecutan

```bash
dotnet build Lafise.Tests
dotnet restore
```

### Tests fallan con errores de BD

Asegúrate de que cada test usa una BD única:
```csharp
_databaseName = Guid.NewGuid().ToString();
```

### Mocks no funcionan

Verifica que configuraste el mock antes de usarlo:
```csharp
_mockAuthInfo.Setup(x => x.UserId()).Returns("client-1");
```

## 📈 Cobertura

Los tests cubren:
- ✅ AccountService (creación, saldos, movimientos)
- ✅ TransactionService (depósitos, retiros, validaciones)
- ✅ Casos exitosos y de error

## ✅ Checklist para Nuevos Tests

- [ ] Patrón AAA (Arrange-Act-Assert)
- [ ] Nombre descriptivo: `MethodName_Scenario_ExpectedBehavior`
- [ ] Probar casos exitosos y de error
- [ ] Usar FluentAssertions
- [ ] Test aislado e independiente
- [ ] Implementar `IDisposable` si es necesario

---

**Última actualización**: Noviembre 2024

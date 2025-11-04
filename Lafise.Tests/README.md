# Tests Unitarios - Lafise API

Este proyecto contiene los tests unitarios para la API de Lafise.

## Estructura del Proyecto

```
Lafise.Tests/
├── Controllers/
│   ├── AuthControllerTests.cs      # Tests para AuthController
│   ├── ClientControllerTests.cs    # Tests para ClientController
│   └── AccountControllerTests.cs   # Tests para AccountController
├── Services/
│   ├── AuthServiceTests.cs         # Tests para AuthService
│   ├── ClientServiceTests.cs       # Tests para ClientService
│   └── AccountServiceTests.cs      # Tests para AccountService
└── README.md
```

## Tecnologías Utilizadas

- **xUnit**: Framework de testing para .NET
- **Moq**: Biblioteca para crear mocks de dependencias
- **FluentAssertions**: Biblioteca para hacer aserciones más legibles
- **Entity Framework Core InMemory**: Para simular la base de datos en tests

## Cómo Ejecutar los Tests

### Desde Visual Studio
1. Abre la solución en Visual Studio
2. Ve al menú **Test** > **Run All Tests**
3. O usa el atajo `Ctrl+R, A`

### Desde la Terminal (CLI)

```bash
# Navegar al directorio del proyecto de tests
cd Lafise.Tests

# Ejecutar todos los tests
dotnet test

# Ejecutar tests con información detallada
dotnet test --verbosity normal

# Ejecutar tests con cobertura de código
dotnet test /p:CollectCoverage=true

# Ejecutar tests de una clase específica
dotnet test --filter "FullyQualifiedName~AuthControllerTests"
```

## Tipos de Tests

### Tests de Controladores (Controllers)
Los tests de controladores verifican:
- Respuestas HTTP correctas (200 OK, 400 Bad Request, 500 Internal Server Error)
- Manejo de excepciones personalizadas (LafiseException)
- Validación de datos de entrada
- Llamadas correctas a los servicios

**Ejemplo:**
```csharp
[Fact]
public async Task Login_WithValidCredentials_ReturnsOkResult()
{
    // Arrange: Configurar mocks y datos de prueba
    // Act: Ejecutar el método bajo prueba
    // Assert: Verificar los resultados
}
```

### Tests de Servicios (Services)
Los tests de servicios verifican:
- Lógica de negocio
- Validaciones de datos
- Interacciones con la base de datos (usando EF Core InMemory)
- Generación de datos (números de cuenta, etc.)

**Características:**
- Usan `BankDataContext` con base de datos en memoria
- Cada test tiene su propia instancia de base de datos aislada
- Verifican tanto casos exitosos como casos de error

## Patrones Utilizados

### 1. Arrange-Act-Assert (AAA)
Todos los tests siguen el patrón AAA:
- **Arrange**: Configurar datos de prueba y mocks
- **Act**: Ejecutar el método bajo prueba
- **Assert**: Verificar los resultados esperados

### 2. Mocking
Se utilizan mocks para:
- **Servicios**: `Mock<IAuthService>`, `Mock<IClientService>`, etc.
- **Utilidades**: `Mock<ICryptor>`, `Mock<IJwtTokenGenerator>`
- **Configuración**: `Mock<IConfiguration>`

### 3. In-Memory Database
Para tests que requieren base de datos:
- Se usa `UseInMemoryDatabase()` de EF Core
- Cada test crea una base de datos nueva y aislada
- Se limpia automáticamente al finalizar el test

## Ejemplos de Tests

### Test de Controlador con Mock
```csharp
[Fact]
public async Task Login_WithValidCredentials_ReturnsOkResult()
{
    // Arrange
    var loginDto = new LoginDto { Email = "test@example.com", Password = "Test1234!" };
    var expectedResult = new LoginResultDto { /* ... */ };
    
    _mockAuthService
        .Setup(x => x.Login(loginDto.Email, loginDto.Password))
        .ReturnsAsync(expectedResult);

    // Act
    var result = await _controller.Login(loginDto);

    // Assert
    result.Should().NotBeNull();
    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.Value.Should().BeEquivalentTo(expectedResult);
}
```

### Test de Servicio con Base de Datos
```csharp
[Fact]
public async Task CreateAccount_WithValidData_CreatesAccount()
{
    // Arrange
    var client = new Client { /* ... */ };
    _context.Clients.Add(client);
    await _context.SaveChangesAsync();

    // Act
    var result = await _accountService.CreateAccount("client-1", "Savings");

    // Assert
    result.Should().NotBeNull();
    result.AccountNumber.Should().NotBeNullOrEmpty();
    
    var accountInDb = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == result.Id);
    accountInDb.Should().NotBeNull();
}
```

## Cobertura de Tests

Los tests cubren:
- ✅ Todos los controladores (Auth, Client, Account)
- ✅ Todos los servicios principales (Auth, Client, Account)
- ✅ Casos exitosos (happy paths)
- ✅ Casos de error (excepciones, validaciones)
- ✅ Validaciones de entrada
- ✅ Manejo de excepciones personalizadas

## Mejores Prácticas Aplicadas

1. **Aislamiento**: Cada test es independiente y no depende de otros
2. **Nombres descriptivos**: Los nombres de los tests explican qué se está probando
3. **Arrange-Act-Assert**: Estructura clara y consistente
4. **FluentAssertions**: Aserciones más legibles y expresivas
5. **Mocks apropiados**: Solo se mockean dependencias externas
6. **Base de datos en memoria**: Tests rápidos sin dependencias externas

## Comandos Útiles

```bash
# Ver cobertura de código
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Ejecutar tests en modo watch (re-ejecuta al cambiar archivos)
dotnet watch test

# Ejecutar tests con filtro
dotnet test --filter "Category=Integration"
```

## Notas

- Los tests usan `FluentAssertions` para hacer las aserciones más legibles
- Se usa `Moq` para crear mocks de dependencias
- `EF Core InMemory` se usa para simular la base de datos sin necesidad de SQLite real
- Cada test clase implementa `IDisposable` para limpiar recursos (base de datos en memoria)


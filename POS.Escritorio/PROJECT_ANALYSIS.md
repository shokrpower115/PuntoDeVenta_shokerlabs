# Análisis y documentación del proyecto POS.Escritorio3

Fecha: julio 23, 2026  
Generado para: uso por otra IA o desarrollador — contiene todo lo necesario para entender, ejecutar y modificar el proyecto.

---

## Resumen rápido
- Nombre: POS.Escritorio3 (Punto de Venta)
- Lenguaje: C# 12.0
- Plataforma: .NET 8
- UI: WPF (MVVM)
- Persistencia: SQLite (Entity Framework Core)
- Propósito: POS local con gestión de ventas, inventario, cortes de caja y administración de usuarios.

---

## Objetivo del archivo
Este documento está pensado para que, si lo pasas a otra IA, pueda:
- Comprender la arquitectura y responsabilidades.
- Localizar archivos clave para cambios.
- Ejecutar y extender funcionalidades (migraciones, cambio de fuente de datos, etc.).
- Proponer y aplicar modificaciones con mínimo contexto adicional.

---

## Estructura del repositorio (resumen)
- src/POS.Core — Modelos, contratos (interfaces), seguridad
- src/POS.Data — Acceso a datos, implementaciones SQLite, seeding y migraciones
  - src/POS.Data/Sqlite — Implementaciones concretas (Producto, Venta, CorteCaja, Auth, etc.)
- src/POS.UI — Interfaz WPF, ViewModels, Views, recursos (Theme.xaml), App.xaml
- src/POS.Data/Migrations — Migraciones EF Core

---

## Cómo compilar y ejecutar (local)
Requisitos:
- .NET 8 SDK instalado
- Visual Studio 2022 (recomendado) o CLI dotnet

Pasos (CLI):
1. Restaurar paquetes:
   - __dotnet restore__
2. Compilar:
   - __dotnet build__
3. Aplicar migraciones y crear/actualizar la DB:
   - __dotnet ef database update__ (desde el proyecto `src/POS.Data` o usando la factory)
4. Ejecutar:
   - __dotnet run --project src/POS.UI__

Pasos (Visual Studio):
- Abrir la solución en Visual Studio 2022.
- Establecer proyecto de inicio: `POS.UI`.
- __Build > Build Solution__.
- __Debug > Start Debugging__ o ejecutar sin depurar.

---

## Puntos de entrada / Inicio de la aplicación
- src/POS.UI\App.xaml.cs
  - Aquí se instancian manualmente las implementaciones de servicios (SQLite).
  - Cambio único para usar API remota: sustituir las instancias Sqlite por clases `*ServiceApi`.

---

## DbContext y migraciones
- Archivo principal: src/POS.Data/PosDbContext.cs
  - Configura entidades, relaciones y datos seed en `OnModelCreating`.
  - Usa `UseSqlite` con ruta proporcionada por `RutaBaseDatos.Obtener()`.

- Migraciones:
  - Directorio: `src/POS.Data/Migrations`
  - Comandos útiles:
    - Crear migración: __dotnet ef migrations add NombreMigracion --project src/POS.Data --startup-project src/POS.UI__
    - Actualizar base de datos: __dotnet ef database update --project src/POS.Data --startup-project src/POS.UI__

---

## Modelos principales (ubicación + responsabilidades)
- src/POS.Core\Models\CorteDeCaja.cs
  - Representa turno, movimientos, cálculos: EfectivoEsperado, Diferencia, TotalEntradas/Retiros.
- src/POS.Core\Models\MovimientoCaja.cs
  - Movimientos de caja por categoría.
- src/POS.Core\Models\CategoriaMovimientoCaja.cs
  - Catálogo (Entrada/Retiro).
- src/POS.Core\Models\Venta.cs y VentaDetalle.cs
  - Ventas y líneas.
- src/POS.Core\Models\Producto*, Usuario, Sucursal, MetodoPago, DatosNegocio, TicketVenta

---

## Servicios (contratos y implementaciones)
Contratos (interfaces) en `src/POS.Core\Services`:
- ICorteCajaService
- IVentaService
- IProductoService
- IAuthService
- IMetodoPagoService
- IImpresoraTicketService

Implementaciones SQLite en `src/POS.Data.Sqlite`:
- CorteCajaServiceSqlite.cs
- VentaServiceSqlite.cs
- ProductoServiceSqlite.cs
- AuthServiceSqlite.cs
- MetodoPagoServiceSqlite.cs
- ImpresoraTicketPdfService.cs

Notas:
- Cada implementación crea un PosDbContext local usando `using var db = new PosDbContext(RutaBaseDatos.Obtener());`.
- Cambiar a API requiere crear nuevas implementaciones que usen HttpClient y reemplazarlas en `App.xaml.cs`.

---

## ViewModels y Views clave
- src/POS.UI\ViewModels\MainViewModel.cs — navegación entre pantallas.
- src/POS.UI\ViewModels\VentaViewModel.cs — flujo de venta, impresión, stock.
- src/POS.UI\ViewModels\CorteCajaViewModel.cs — abrir/registrar movimientos/cerrar turno.
- src/POS.UI\ViewModels\InventarioViewModel.cs — stock por sucursal.
- Views: en `src/POS.UI\Views\*` y recursos como `src/POS.UI\Theme.xaml`.

Tema y estilos:
- src/POS.UI\Theme.xaml — paleta y estilos de botones (BotonPrimario, BotonMenu, BotonAccento).

---

## Flujo de negocio: Corte de caja (detalle)
1. Abrir turno:
   - CorteCajaViewModel.AbrirTurnoAsync → ICorteCajaService.AbrirTurnoAsync
2. Registrar ventas durante el turno:
   - VentaViewModel registra ventas via IVentaService.RegistrarVentaAsync
   - VentaServiceSqlite guarda venta y detalles; EF rellena ids y se decrementa stock.
3. Registrar movimientos (entradas/retiros):
   - CorteCajaViewModel usa ICorteCajaService.RegistrarMovimientoAsync
4. Cerrar turno:
   - CorteCajaViewModel.CerrarTurnoAsync llama ICorteCajaService.CerrarTurnoAsync con totales.
5. Cálculos:
   - EfectivoEsperado = EfectivoInicial + TotalVentasEfectivo + TotalEntradas - TotalRetiros
   - Diferencia = EfectivoFinal - EfectivoEsperado

---

## Archivos clave (lista rápida)
- App: src/POS.UI\App.xaml, src/POS.UI\App.xaml.cs
- DbContext: src/POS.Data\PosDbContext.cs
- Ruta DB: src/POS.Data\RutaBaseDatos.cs
- Servicios SQLite: src/POS.Data.Sqlite\*.cs
- Modelos: src/POS.Core\Models\*.cs
- Interfaces: src/POS.Core\Services\*.cs
- ViewModels: src/POS.UI\ViewModels\*.cs
- Views: src/POS.UI\Views\*.xaml
- Theme: src/POS.UI\Theme.xaml

---

## Tareas comunes y cómo solicitarlas a otra IA (plantillas)
- Añadir campo/propiedad a modelo + migración:
  - "Modificar modelo X (ruta/file). Agregar propiedad Y (tipo Z) y crear migración. Actualizar PosDbContext.OnModelCreating si hace falta. Generar comandos __dotnet ef migrations add__ y __dotnet ef database update__."
- Reemplazar almacenamiento por API:
  - "Crear nuevas implementaciones *ServiceApi que usen HttpClient para los métodos de I*. Reemplazar inyección en `src/POS.UI\App.xaml.cs` y documentar endpoints y payloads necesarios."
- Añadir validaciones en UI:
  - "Actualizar ViewModel X (ruta) para validar campo Y. Mostrar mensajes en la propiedad Mensaje y deshabilitar comando Z si no cumple."
- Mejorar UI/tema:
  - "Actualizar src/POS.UI\Theme.xaml: añadir nuevo estilo para ToggleButton y variables de color para tema oscuro."

Incluye siempre:
- Archivo(s) a modificar con ruta exacta.
- Descripción clara del cambio.
- Comportamiento esperado y pruebas manuales rápidas.

---

## Comandos útiles (resumen)
- Restaurar: __dotnet restore__
- Compilar: __dotnet build__
- Ejecutar: __dotnet run --project src/POS.UI__
- Migraciones:
  - Crear: __dotnet ef migrations add Nombre --project src/POS.Data --startup-project src/POS.UI__
  - Aplicar: __dotnet ef database update --project src/POS.Data --startup-project src/POS.UI__

Visual Studio (GUI):
- __Build > Build Solution__
- __Debug > Start Debugging__
- Abrir __Package Manager Console__ para ejecutar comandos EF si se prefiere.

---

## Consideraciones y recomendaciones
- Actualmente la inyección de dependencias es manual en `App.xaml.cs`. Considerar registrar servicios en un contenedor IoC (Microsoft.Extensions.DependencyInjection) para facilitar pruebas y cambios.
- SQLite local no sincroniza entre sucursales: plan de migración para multi-sucursal requiere backend centralizado.
- Añadir logging estructurado (Serilog) facilitará diagnóstico en producción.
- Añadir tests unitarios para ViewModels y servicios (mock PosDbContext o usar interfaces) aumenta confianza al cambiar lógica.

---

## Lista breve de problemas o mejoras detectadas (prioridad sugerida)
1. Externalizar inyección de servicios hacia un contenedor DI. (Alto)
2. Añadir pruebas unitarias para ViewModels críticos (Venta/Corte). (Medio)
3. Soporte offline/sincronización o migración a API central. (Alto)
4. Mejorar manejo de concurrencia en PosDbContext si se comparte entre hilos (actualmente instancias por llamada). (Medio)
5. Documentar endpoints esperados si se crea backend. (Bajo)

---

## Contacto para la IA receptora
Si necesitas cambios automáticos, solicita:
- Lista de archivos a modificar con rutas.
- Tests o ejemplos de entrada/salida.
- Restricciones (p. ej. mantener compatibilidad con SQLite).

---

Fin del documento — disponible para descarga y edición.  
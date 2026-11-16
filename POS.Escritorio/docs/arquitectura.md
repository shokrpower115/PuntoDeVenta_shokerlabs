# Arquitectura

Aplicación de escritorio **WPF sobre .NET 8**, con **SQLite** local mediante **Entity Framework Core 8**. Patrón **MVVM** sin frameworks externos.

## Capas

```
POS.UI  ──────►  POS.Core  ◄──────  POS.Data
(WPF)            (modelos +         (SQLite / EF Core,
 Vistas,          interfaces de      impresión de tickets)
 ViewModels,      servicio, reglas)
 tema)
```

| Proyecto | Contiene | No debe contener |
|---|---|---|
| `src/POS.Core` | Modelos (`Venta`, `CorteDeCaja`…), interfaces de servicio (`IVentaService`…), `ReglaNegocioException`, `HashPassword`. | Referencias a WPF, EF Core o SQLite. |
| `src/POS.Data` | Implementaciones `*ServiceSqlite`, `PosDbContext`, migraciones, `InicializadorBaseDatos`, impresión de tickets (QuestPDF). | Referencias a WPF. |
| `src/POS.UI` | Ventanas, vistas (`Views/`), `ViewModels/`, convertidores, `Theme.xaml`, `Apariencia/Marca.cs`. | Consultas a la base de datos (siempre vía interfaces de `POS.Core`). |

Las interfaces de `POS.Core` permiten cambiar SQLite por una API central (multisucursal) creando `*ServiceApi` en otro proyecto y cambiando las líneas marcadas en `App.xaml.cs`, sin tocar vistas ni ViewModels.

## Flujo de arranque (`App.OnStartup`)

```
ConfigurarLog ─► InicializadorBaseDatos (crea/migra la BD, garantiza un admin)
   │
   ├─ ¿Hay datos del negocio? ── no ─► AsistenteConfiguracionWindow ─► (cancelar = salir)
   │
   ├─ Marca.Aplicar(datos)  → color, nombre y logo en toda la interfaz
   │
   └─ ciclo de sesiones:
        LoginWindow
          ─► ¿Debe cambiar contraseña? ─► CambiarPasswordWindow
          ─► ¿Turno abierto de otra persona? (cajero) ─► aviso y regresa al login
          ─► MainWindow
          ─► ¿Sin turno? ─► FondoInicialWindow
          ─► espera a que se cierre la sesión (corte o "Cerrar sesión") ─► login otra vez
```

## Patrones y convenciones

### Servicios
- Un servicio por área (`IVentaService`, `ICorteCajaService`, …). Cada método abre y cierra su propio `PosDbContext` (`using var db = new PosDbContext(RutaBaseDatos.Obtener())`).
- **Lo que debe ser atómico se hace en un solo `SaveChangesAsync`** (por ejemplo, la venta y el descuento de existencias, RN-VEN-03).
- **Las reglas de negocio viven en los servicios**, no solo en la interfaz. La interfaz valida para dar buena experiencia; el servicio valida para garantizar los datos.
- Si se viola una regla, el servicio lanza `ReglaNegocioException("mensaje para el usuario")`. El manejador global de `App` la muestra como aviso (no como error). Los ViewModels pueden atraparla para mostrar el mensaje en línea.
- La lógica nunca depende de textos que el usuario puede cambiar (nombres de métodos de pago o categorías); usa tipos o indicadores (`MetodoPago.Tipo`, `CategoriaMovimientoCaja.RequiereDescripcion`).

### ViewModels
- Heredan de `ViewModelBase` (`SetProperty`, `OnPropertyChanged`). Los comandos son `RelayCommand` / `RelayCommand<T>`.
- La navegación se hace cambiando `PantallaActual`; `App.xaml` tiene un `DataTemplate` por tipo de ViewModel que decide qué vista dibujar.
- Mensajes para la interfaz: `Mensaje` o `MensajeError` para avisos y `MensajeExito` para confirmaciones (string vacío = oculto).

### Interfaz
- Todo estilo sale de `Theme.xaml` (ver [guia-de-estilo-ui.md](guia-de-estilo-ui.md)). Lo que depende de la marca del negocio se usa con `DynamicResource`.
- `Marca.Aplicar(DatosNegocio)` es el único lugar que traduce la configuración del negocio a recursos visuales.

### Errores y bitácora
- `App` registra `DispatcherUnhandledException`, `TaskScheduler.UnobservedTaskException` y `AppDomain.UnhandledException`: ningún error cierra la aplicación a media venta.
- Bitácora con Serilog en `%LocalAppData%\PuntoDeVenta\logs\pos-AAAAMMDD.log` (60 días). Se registran inicios y cierres de sesión, cortes, avisos de reglas y errores.

### Código
- Nombres de clases, métodos y variables en **español**, como el resto del código.
- Archivos en **UTF-8**.
- Comentarios para explicar el **por qué**, no el qué. Cuando un comentario aplica una regla, cita su ID (`RN-…`).

## Tareas comunes

### Agregar una pantalla dentro de la ventana principal
1. Crea `ViewModels/MiPantallaViewModel.cs` (hereda de `ViewModelBase`).
2. Crea `Views/MiPantallaView.xaml` (UserControl) usando solo estilos del tema.
3. Registra en `App.xaml`: `<DataTemplate DataType="{x:Type vm:MiPantallaViewModel}"><views:MiPantallaView/></DataTemplate>`.
4. Navega asignando `PantallaActual = new MiPantallaViewModel(...)`.

### Agregar un servicio
1. Interfaz en `POS.Core/Services`.
2. Implementación `…ServiceSqlite` en `POS.Data/Sqlite`.
3. Instánciala en `App.xaml.cs` (bloque "ÚNICO LUGAR QUE CAMBIARÁ…").

### Cambiar el modelo de datos (migración)
```bash
# POS_DB_PATH evita que la herramienta toque la base real del equipo.
POS_DB_PATH=/tmp/diseno.db dotnet ef migrations add NombreDelCambio --project src/POS.Data --startup-project src/POS.Data
```
- **Revisa la migración generada antes de confirmarla**: valores por defecto de columnas nuevas (un `bool` nuevo llega en `false` a los registros existentes), datos que haya que rellenar con `migrationBuilder.Sql(...)`, y semillas (`HasData`) que choquen con registros reales.
- `HasData` debe ser **determinista**: nada de `Guid.NewGuid()`, `DateTime.Now` ni `HashPassword.Generar()` adentro.
- No hace falta correr `database update`: la aplicación aplica las migraciones al arrancar.

### Probar contra una base de datos desechable
Todos los servicios respetan la variable de entorno `POS_DB_PATH`. Para probar sin tocar la base real, copia `pos.db` a otro lugar y arranca con `POS_DB_PATH` apuntando a la copia.

## Pendientes técnicos
- Pruebas automatizadas (servicios de venta y corte primero).
- Contenedor de inyección de dependencias en lugar de `new` en `App.xaml.cs`.
- Las ventanas modales se abren desde los ViewModels (`new CobroWindow(...)`); conviene un servicio de diálogos para poder probarlos.

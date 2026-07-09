# POS Escritorio — Esqueleto WPF

## Cómo abrirlo
1. Copia esta carpeta a tu máquina Windows.
2. Requiere **.NET 8 SDK** y **Visual Studio 2022** (con la carga de trabajo ".NET desktop development").
3. Abre `POS.Escritorio.sln` con doble clic o desde Visual Studio (Archivo → Abrir → Proyecto o Solución).
4. Marca `POS.UI` como proyecto de inicio (clic derecho → "Establecer como proyecto de inicio").
5. F5 para correr. Ya debería abrir la ventana con los 3 módulos y datos de prueba (3 sucursales simuladas).

## Estructura

- **POS.Core** → Modelos (`Producto`, `Venta`, `CorteDeCaja`, `Sucursal`) e **interfaces**
  (`IProductoService`, `IVentaService`, `ICorteCajaService`). Este proyecto no sabe si los datos
  vienen de memoria, SQLite o una API — solo define el contrato.

- **POS.Data** → HOY contiene los "Mock" (datos en memoria, simulan 3 sucursales con stock distinto).
  Aquí es donde, más adelante, vas a agregar `ProductoServiceApi.cs`, `VentaServiceApi.cs`, etc.,
  que implementen las mismas interfaces pero usando `HttpClient` contra tu backend real.

- **POS.UI** → El proyecto WPF. Contiene:
  - `Views/` → las pantallas (XAML): Venta, Inventario, Corte de Caja
  - `ViewModels/` → la lógica de cada pantalla (qué hace el botón "Cobrar", etc.)
  - `App.xaml.cs` → **el único archivo que vas a tocar para cambiar de mock a API real**
    (ahí se decide qué implementación de las interfaces se usa)

## El día que conectes la API real

Solo necesitas:
1. Crear `ProductoServiceApi : IProductoService` (y las otras 2) en `POS.Data`, usando `HttpClient`.
2. Cambiar 3 líneas en `App.xaml.cs` para usar esas clases en vez de los Mock.
3. Nada en `Views/` ni en `ViewModels/` se toca. Por diseño.

## Qué falta (a propósito, para no atrasar el prototipo)

- Login / selección de sucursal (ahorita `SucursalActualId` está fijo en 1 en `MainViewModel`)
- Persistencia real (ahorita todo se pierde al cerrar la app, porque los Mock son listas en memoria)
- Impresión de tickets
- Validaciones (cantidades editables en el carrito, cancelar productos, etc.)

Estos se agregan sin romper nada de lo que ya está, porque la separación por capas ya está lista.

# Reglas de negocio

Cada regla tiene un identificador estable (`RN-ÁREA-NN`). Los identificadores **no se reutilizan**: si una regla se elimina, se marca como *Retirada* y se deja su número.

Estado de cada regla:
- ✅ **Vigente**: implementada en el código.
- 🟡 **Pendiente**: acordada, pero todavía no implementada (ver [hoja-de-ruta.md](hoja-de-ruta.md)).

---

## 1. Usuarios y acceso (`RN-USR`)

| ID | Regla | Estado | Dónde se aplica |
|---|---|---|---|
| RN-USR-01 | Existen dos roles: **Cajero** y **Administrador**. `SuperAdministrador` está reservado para la versión multisucursal con servidor. | ✅ | `Usuario.Rol` |
| RN-USR-02 | Solo un Administrador ve y usa el módulo **Administrador** (productos, precios, existencias, usuarios, configuración del negocio). | ✅ | `MainViewModel.EsAdministrador` |
| RN-USR-03 | El nombre de usuario es único **sin distinguir mayúsculas** (`Maria` y `maria` son el mismo). | ✅ | Índice único con `NOCASE`, `AuthServiceSqlite.CrearUsuarioAsync` |
| RN-USR-04 | Las contraseñas nunca se guardan en texto plano: se guarda un hash PBKDF2-SHA256 con sal aleatoria y 100 000 iteraciones. | ✅ | `HashPassword` |
| RN-USR-05 | Si al arrancar no existe ningún administrador, el sistema crea `admin` / `admin` marcado para cambiar la contraseña. | ✅ | `InicializadorBaseDatos` |
| RN-USR-06 | Un usuario marcado con *Debe cambiar contraseña* no entra al sistema hasta elegir una nueva de **mínimo 6 caracteres**, distinta a la actual. | ✅ | `CambiarPasswordWindow` |
| RN-USR-07 | Cada usuario pertenece a una sucursal y solo opera la caja de esa sucursal. | ✅ | `Usuario.SucursalId` |
| RN-USR-08 | Un administrador puede desactivar usuarios (sin borrarlos, porque tienen ventas y cortes a su nombre). | 🟡 | — |

## 2. Configuración del negocio (`RN-NEG`)

| ID | Regla | Estado | Dónde se aplica |
|---|---|---|---|
| RN-NEG-01 | Los datos del negocio (nombre, dirección, teléfono, RFC, sitio web, mensaje del ticket, color y logo) se guardan en la base de datos, **nunca en el código**. Hay un solo registro. | ✅ | `DatosNegocio`, `ConfiguracionNegocioServiceSqlite` |
| RN-NEG-02 | En el primer arranque (sin datos del negocio) se muestra el asistente de configuración **antes** del login. El nombre comercial es obligatorio. Si se cierra el asistente, la aplicación se cierra. | ✅ | `App.OnStartup`, `AsistenteConfiguracionWindow` |
| RN-NEG-03 | El color principal se captura como `#RRGGBB`. El logo es PNG o JPG de **máximo 2 MB** y se guarda dentro de la base de datos, así un respaldo de `pos.db` incluye la configuración completa. | ✅ | `ConfiguracionNegocioViewModel` |
| RN-NEG-04 | Sin logo, la interfaz muestra las iniciales del negocio sobre el color principal. | ✅ | `Marca.Aplicar` |
| RN-NEG-05 | Los cambios de configuración se ven de inmediato en toda la aplicación, sin reiniciar. | ✅ | Recursos dinámicos (`DynamicResource`) |

## 3. Turnos de caja (`RN-TUR`)

Un **turno** es el periodo entre que se abre la caja con un fondo inicial y se hace el corte.

| ID | Regla | Estado | Dónde se aplica |
|---|---|---|---|
| RN-TUR-01 | Hay **una caja por sucursal**: como máximo un turno abierto por sucursal a la vez ([ADR 0001](decisiones/0001-una-caja-por-sucursal.md)). | ✅ | `CorteCajaServiceSqlite.AbrirTurnoAsync` |
| RN-TUR-02 | Para vender debe haber un turno abierto. Si el cajero inicia sesión y no hay turno, se le pide el **fondo inicial** (≥ 0) para abrirlo. | ✅ | `App.OnStartup`, `FondoInicialWindow` |
| RN-TUR-03 | El turno pertenece a quien lo abrió. Si hay un turno abierto por **otra persona**, un cajero no puede entrar hasta que se haga el corte. Un administrador sí puede entrar (por ejemplo, para hacer ese corte). | ✅ | `App.OnStartup` |
| RN-TUR-04 | Un administrador puede entrar sin abrir turno para usar solo el módulo Administrador; si intenta vender, el sistema lo rechaza. | ✅ | `App.OnStartup`, `VentaServiceSqlite` |
| RN-TUR-05 | Cerrar sesión con el turno abierto muestra una advertencia; el turno **sigue abierto** (no se hace corte automático). | ✅ | `MainViewModel.CerrarSesionAsync` |

## 4. Ventas (`RN-VEN`)

| ID | Regla | Estado | Dónde se aplica |
|---|---|---|---|
| RN-VEN-01 | Toda venta queda ligada al turno abierto de su sucursal. | ✅ | `VentaServiceSqlite.RegistrarVentaAsync` |
| RN-VEN-02 | No se puede vender más de lo que hay en existencia en la sucursal. Se valida al agregar al ticket **y** otra vez al registrar. | ✅ | `VentaViewModel`, `VentaServiceSqlite` |
| RN-VEN-03 | La venta y el descuento de existencias son **una sola operación**: o se guardan las dos, o ninguna. | ✅ | `VentaServiceSqlite` (un solo `SaveChanges`) |
| RN-VEN-04 | La línea de venta guarda una copia del nombre y del precio del producto en ese momento: cambiar un precio después no altera ventas pasadas. | ✅ | `VentaDetalle` |
| RN-VEN-05 | En **efectivo**, el monto recibido debe ser ≥ al total y el cambio es `recibido − total`. En los demás métodos, el monto recibido es igual al total. | ✅ | `CobroViewModel` |
| RN-VEN-06 | El folio del ticket es el número de la venta. | ✅ | `TicketVenta.Folio` |
| RN-VEN-07 | Si falla la impresión del ticket, **la venta ya quedó registrada**: se avisa y el ticket en pantalla se limpia para no cobrar dos veces. | ✅ | `VentaViewModel.CobrarAsync` |
| RN-VEN-08 | Si el efectivo estimado en caja (fondo + ventas en efectivo + entradas − retiros) pasa de **$4,000**, se sugiere hacer un retiro. | ✅ | `VentaViewModel` (umbral fijo; ver RN-VEN-10) |
| RN-VEN-09 | Cancelaciones y devoluciones: solo con autorización de un administrador, regresan existencias y quedan registradas. | 🟡 | — |
| RN-VEN-10 | El umbral de RN-VEN-08 es configurable por negocio. | 🟡 | — |
| RN-VEN-11 | Impuestos (IVA/IEPS) desglosados por producto. | 🟡 | — |

## 5. Métodos de pago (`RN-PAG`)

| ID | Regla | Estado | Dónde se aplica |
|---|---|---|---|
| RN-PAG-01 | Cada método de pago tiene un **tipo**: Efectivo, Tarjeta, Transferencia u Otro. Toda la lógica usa el tipo, **nunca el nombre** (renombrar "Tarjeta de crédito" no rompe el corte). | ✅ | `MetodoPago.Tipo` |
| RN-PAG-02 | Solo el tipo **Efectivo** entra en el efectivo esperado del corte. Las ventas con **Tarjeta** se reportan como total con tarjeta. Las demás aparecen en el desglose por método. | ✅ | `CorteCajaServiceSqlite.CerrarTurnoAsync` |

## 6. Movimientos de caja (`RN-MOV`)

Dinero que entra o sale del cajón sin ser una venta (cambio adicional, retiro por seguridad, depósito…).

| ID | Regla | Estado | Dónde se aplica |
|---|---|---|---|
| RN-MOV-01 | Cada movimiento es **Entrada** o **Retiro**, con una categoría. Las categorías marcadas con *Requiere descripción* (las "Otro") exigen escribir el motivo. | ✅ | `MovimientoCajaViewModel` |
| RN-MOV-02 | El monto debe ser mayor a 0, debe haber turno abierto y el usuario en sesión confirma con su contraseña. | ✅ | `MovimientoCajaViewModel` |
| RN-MOV-03 | Un retiro no puede ser mayor al efectivo estimado en caja. | 🟡 | — |

## 7. Corte de caja (`RN-COR`)

| ID | Regla | Estado | Dónde se aplica |
|---|---|---|---|
| RN-COR-01 | **Efectivo esperado** = fondo inicial + ventas en efectivo del turno + entradas − retiros. | ✅ | `CorteDeCaja.EfectivoEsperado` |
| RN-COR-02 | **Diferencia** = efectivo contado − efectivo esperado. Negativa es faltante; positiva es sobrante. | ✅ | `CorteDeCaja.Diferencia` |
| RN-COR-03 | **Conteo ciego**: el cajero captura lo que contó **sin ver** el efectivo esperado; la diferencia aparece en el ticket del corte ([ADR 0004](decisiones/0004-conteo-ciego.md)). | ✅ | `CorteCajaView` |
| RN-COR-04 | El corte lo autoriza, con su contraseña, **quien abrió el turno o un administrador**. | ✅ | `CorteCajaViewModel.RealizarCorteAsync` |
| RN-COR-05 | Los totales los calcula el sistema a partir de las ventas del turno (no los captura nadie). Un turno cerrado no se puede volver a cerrar. | ✅ | `CorteCajaServiceSqlite.CerrarTurnoAsync` |
| RN-COR-06 | Al terminar el corte se imprime el ticket del corte (ventas por método, movimientos, esperado, contado y diferencia) y se cierra la sesión. | ✅ | `CorteCajaViewModel`, `ImpresoraTicketPdfService` |

## 8. Productos e inventario (`RN-PRO`)

| ID | Regla | Estado | Dónde se aplica |
|---|---|---|---|
| RN-PRO-01 | Un producto requiere nombre y precio mayor a 0. El código de barras es opcional. | ✅ | `ProductosAdminViewModel` |
| RN-PRO-02 | La existencia se lleva **por sucursal**: un registro por producto y sucursal. | ✅ | `ProductoSucursal` (llave compuesta) |
| RN-PRO-03 | Los productos **no se borran, se desactivan**: dejan de aparecer en venta y catálogo, pero sus ventas pasadas se conservan. | ✅ | `ProductoServiceSqlite.EliminarAsync` |
| RN-PRO-04 | Desde la venta se puede consultar la existencia del producto en las demás sucursales. | ✅ | `ConsultarEnOtrasSucursalesAsync` |
| RN-PRO-05 | El código de barras es único entre productos activos. | 🟡 | — |
| RN-PRO-06 | Las sucursales se leen de la base de datos (hoy la pantalla de productos tiene Centro/Norte/Sur fijas). | 🟡 | — |

## 9. Datos e integridad (`RN-DAT`)

| ID | Regla | Estado | Dónde se aplica |
|---|---|---|---|
| RN-DAT-01 | El historial (ventas, cortes, movimientos) **nunca se borra en cascada**: no se puede borrar un usuario, sucursal o método de pago que tenga historial ([ADR 0006](decisiones/0006-sin-borrado-en-cascada.md)). | ✅ | `PosDbContext.OnModelCreating` |
| RN-DAT-02 | La base de datos vive en `%LocalAppData%\PuntoDeVenta\pos.db` y se crea o actualiza sola al arrancar ([ADR 0002](decisiones/0002-base-de-datos-local.md)). | ✅ | `RutaBaseDatos`, `InicializadorBaseDatos` |
| RN-DAT-03 | Toda falla inesperada queda registrada en la bitácora (`%LocalAppData%\PuntoDeVenta\logs`), y la aplicación no se cierra. | ✅ | `App.ConfigurarLog` |
| RN-DAT-04 | Respaldo automático diario de la base de datos. | 🟡 | — |

---

## Cómo agregar o cambiar una regla

1. Agrégala en la sección que corresponde con el **siguiente número libre** y estado 🟡.
2. Implementa. En el código, el comentario que la aplica cita el ID, por ejemplo `// RN-VEN-02: ...`.
3. Cambia el estado a ✅ y llena "Dónde se aplica".
4. Si la regla viola una regla de negocio, el servicio lanza `ReglaNegocioException` con un mensaje **dirigido al usuario** (la interfaz lo muestra tal cual).

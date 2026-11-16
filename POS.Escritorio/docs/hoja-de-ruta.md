# Hoja de ruta

## Hecho

### Fase 1 — Estabilización
- Control de acceso por rol; administrador inicial con cambio de contraseña obligatorio.
- Base de datos que se crea y migra sola, ubicada en `%LocalAppData%`.
- Sin borrado en cascada; productos desactivables.
- Venta y descuento de existencias atómicos; ventas ligadas a su turno.
- Un turno por sucursal; corte autorizado por el dueño del turno o un administrador; totales calculados por el sistema.
- Métodos de pago por tipo; manejo global de errores y bitácora.

### Rediseño visual y producto instalable
- Sistema de diseño (`Theme.xaml`) y rediseño de todas las pantallas.
- Marca configurable por negocio (nombre, logo, color, datos del ticket) con asistente de primer arranque.
- Operación con teclado y lector: Enter agrega, F2 buscar, F12 cobrar; botones de efectivo rápido.
- Publicación autocontenida (el cliente no instala .NET).
- Documentación en `docs/`.

## Siguiente: mínimo vendible

Prioridad de arriba hacia abajo.

| # | Tema | Reglas / notas |
|---|---|---|
| 1 | Quitar usuarios de demostración de las semillas de instalaciones nuevas. | Hoy se crean `centro`/`norte`/`sur` con `1234`. |
| 2 | Sucursales administrables (alta/edición) y pantallas sin sucursales fijas. | RN-PRO-06 |
| 3 | Impresión térmica directa (ESC/POS) y apertura de cajón; elegir impresora en la configuración. | Hoy: PDF. |
| 4 | Cancelaciones y devoluciones con autorización. | RN-VEN-09 |
| 5 | Retiro no mayor al efectivo en caja; umbral de alerta configurable. | RN-MOV-03, RN-VEN-10 |
| 6 | Desactivar usuarios; recuperación de acceso del administrador. | RN-USR-08 |
| 7 | Código de barras único. | RN-PRO-05 |
| 8 | Impuestos (IVA/IEPS) por producto y desglose en ticket. | RN-VEN-11 |
| 9 | Reportes: ventas por día, producto y cajero; exportar a Excel. | |
| 10 | Respaldo automático diario. | RN-DAT-04 |
| 11 | Instalador (Inno Setup o MSIX) con acceso directo y desinstalador; firma de código. | Ver [instalacion.md](instalacion.md). |
| 12 | Pruebas automatizadas de venta y corte. | Ver [arquitectura.md](arquitectura.md). |

## Después: diferenciación

- Facturación electrónica CFDI 4.0 mediante un PAC.
- Servidor central (API) con modo sin conexión y sincronización entre sucursales.
- Panel web o app para el dueño (ventas en tiempo real).
- Licenciamiento por suscripción.
- Revisar la licencia de QuestPDF para uso comercial.

# 0006. Sin borrado en cascada; desactivar en lugar de borrar

- **Estado:** Aceptada
- **Fecha:** 2026-11-15

## Contexto
Todas las relaciones de la base tenían borrado en cascada (el valor por defecto de EF Core). Borrar un usuario borraba sus ventas y sus cortes; borrar un método de pago borraba las ventas hechas con él. "Eliminar producto" lo borraba aunque tuviera ventas.

## Decisión
- Ninguna relación borra en cascada, salvo las de un documento con sus partes: venta → líneas de venta, corte → movimientos de caja, producto → existencias por sucursal (RN-DAT-01).
- Lo que tiene historial se **desactiva** (`Activo = false`) en lugar de borrarse. Hoy aplica a productos; usuarios y métodos de pago están en la hoja de ruta.

## Consecuencias
- El historial de ventas y cortes nunca se pierde por una acción administrativa.
- Intentar borrar un registro con historial falla en la base de datos: la interfaz debe ofrecer "desactivar", nunca "borrar".
- Las consultas de catálogo deben filtrar `Activo`.

## Alternativas descartadas
- **Borrado lógico global (filtro de consulta en EF Core):** útil más adelante, pero oculta registros en reportes históricos si no se maneja con cuidado.

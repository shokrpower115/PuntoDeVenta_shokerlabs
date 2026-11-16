# 0002. SQLite local en %LocalAppData%, migrada al arrancar

- **Estado:** Aceptada
- **Fecha:** 2026-11-15

## Contexto
El producto se instala en negocios pequeños, a menudo con un solo equipo y sin servidor ni personal técnico. La base estaba en `Documentos\PuntoDeVenta`, que en Windows 10/11 suele estar sincronizada con OneDrive (la sincronización puede corromper un SQLite abierto), y una instalación nueva no creaba las tablas: solo funcionaba en el equipo de desarrollo, donde se había corrido `dotnet ef database update`.

## Decisión
- Base de datos **SQLite** en `%LocalAppData%\PuntoDeVenta\pos.db`.
- La aplicación ejecuta `Database.Migrate()` al arrancar (`InicializadorBaseDatos`): crea la base si no existe y aplica migraciones pendientes.
- La variable de entorno `POS_DB_PATH` permite otra ubicación (pruebas, instalaciones especiales).
- Si existe una base en la ubicación anterior, se copia una vez y el original queda como respaldo.

## Consecuencias
- Instalar = copiar el ejecutable. Actualizar = reemplazar el ejecutable.
- Cada usuario de Windows tiene su propia base; si varios usuarios de Windows comparten el equipo, hay que usar `POS_DB_PATH`.
- No hay sincronización entre sucursales: cada equipo tiene sus datos. La consulta "en otras sucursales" solo es real cuando las sucursales comparten base. La versión multisucursal requiere un servidor central (hoja de ruta).

## Alternativas descartadas
- **SQL Server / PostgreSQL local:** exige instalar y mantener un servidor en cada negocio.
- **Migraciones manuales:** inviable sin personal técnico en el cliente.

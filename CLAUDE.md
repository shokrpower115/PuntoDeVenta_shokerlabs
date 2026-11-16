# Punto de Venta — instrucciones para asistentes de IA

Proyecto WPF (.NET 8) + SQLite (EF Core 8). Solución en `POS.Escritorio/POS.Escritorio.sln`.

**Antes de cambiar comportamiento, lee la documentación en `POS.Escritorio/docs/`:**
- `reglas-de-negocio.md`: reglas numeradas `RN-…`. Una regla nueva se agrega ahí primero; el código que la aplica cita su ID en un comentario.
- `arquitectura.md`: capas, patrones y cómo agregar pantallas, servicios y migraciones.
- `guia-de-estilo-ui.md`: las vistas solo usan estilos de `src/POS.UI/Theme.xaml`; lo que depende de la marca va con `DynamicResource`.
- `decisiones/`: ADRs. No deshagas una decisión documentada sin escribir un ADR que la reemplace.

Reglas rápidas:
- Código, comentarios y documentación en español; archivos en UTF-8.
- La lógica de negocio va en los servicios (`POS.Data/Sqlite`) y lanza `ReglaNegocioException` con mensajes para el usuario.
- Nunca escribas datos de un cliente (nombre, RFC, logo, colores) en el código: viven en `DatosNegocio`.
- Migraciones: `POS_DB_PATH=<archivo temporal> dotnet ef migrations add Nombre --project src/POS.Data --startup-project src/POS.Data`, y revisa la migración generada (valores por defecto, semillas que choquen con datos reales).
- Actualiza `docs/` en el mismo commit que el código que describe.

# 0003. La marca del negocio se configura en la base de datos

- **Estado:** Aceptada
- **Fecha:** 2026-11-15

## Contexto
El nombre, RFC y sitio web del primer cliente estaban escritos en `App.xaml.cs`, su logo iba incrustado en el ejecutable y los colores del tema estaban tomados de ese logo. Para venderlo a otro negocio había que modificar el código y recompilar.

## Decisión
- Los datos del negocio (nombre, dirección, teléfono, RFC, sitio web, mensaje del ticket, color principal y logo) se guardan en la tabla `DatosNegocio` (un registro).
- En el primer arranque, un asistente los pide **antes** del login. Después se editan en *Administrador → Negocio*.
- El logo se guarda como bytes dentro de la base: un respaldo de `pos.db` lleva toda la configuración.
- La interfaz usa **un solo color de marca** sobre una base neutra. `Marca.Aplicar()` deriva del color los tonos oscuro, suave y el color de texto con buen contraste, y los publica como recursos dinámicos junto con el nombre, las iniciales y el logo.

## Consecuencias
- El mismo ejecutable sirve para cualquier cliente.
- Los cambios de marca se ven de inmediato, sin reiniciar.
- Las pantallas deben usar `DynamicResource` para todo lo que dependa de la marca (ver la guía de estilo). Un `StaticResource` sobre esos recursos no se actualizaría.
- Colores de estado (verde de cobrar, rojo de error) **no** cambian con la marca, para que signifiquen siempre lo mismo.

## Alternativas descartadas
- **Archivo de configuración junto al .exe:** se pierde al actualizar o reinstalar, y no viaja con el respaldo de la base.
- **Temas completos por cliente:** demasiadas combinaciones que probar; un color sobre una base neutra se ve bien con cualquier marca.

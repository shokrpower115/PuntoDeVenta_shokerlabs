# Instalación en un cliente

## Requisitos del equipo

- Windows 10 o 11 de 64 bits.
- **No necesita .NET instalado**: la publicación es autocontenida ([ADR 0005](decisiones/0005-publicacion-autocontenida.md)).
- Impresión: hoy el ticket se genera como PDF y se abre con el visor predeterminado; desde ahí se imprime. La impresión térmica directa está en la [hoja de ruta](hoja-de-ruta.md).

## 1. Generar el paquete (en el equipo de desarrollo)

```bash
cd POS.Escritorio
dotnet publish src/POS.UI -p:PublishProfile=Cliente
```

Resultado en `POS.Escritorio/publicacion/PuntoDeVenta/` (ignorado por git):

| Archivo | Qué es |
|---|---|
| `PuntoDeVenta.exe` | El programa completo (incluye .NET y SQLite). |
| `LatoFont/` | Fuente que usan los tickets PDF. **Debe ir junto al .exe.** |

Antes de publicar una versión nueva, sube `<Version>` en `src/POS.UI/POS.UI.csproj`.

> **Aviso de Windows (SmartScreen):** mientras el ejecutable no esté firmado con un certificado de firma de código, Windows mostrará "Windows protegió su PC" la primera vez. Se continúa con *Más información → Ejecutar de todas formas*. Firmar el ejecutable está en la hoja de ruta.

## 2. Instalar en el equipo del cliente

1. Copia la carpeta `PuntoDeVenta` a `C:\Program Files\PuntoDeVenta\` (o a cualquier carpeta).
2. Crea un acceso directo a `PuntoDeVenta.exe` en el escritorio.
3. Ábrelo. Como es la primera vez:
   1. Aparece el **asistente de configuración**: nombre del negocio, dirección, RFC, mensaje del ticket, logo y color. Solo el nombre es obligatorio; todo se puede cambiar después en *Administrador → Negocio*.
   2. Inicia sesión con **`admin` / `admin`**. El sistema obliga a cambiar la contraseña.
   3. En *Administrador → Usuarios* da de alta a los cajeros; en *Administrador → Productos*, el catálogo.
4. Cierra sesión y entra como cajero: se pedirá el fondo inicial para abrir el primer turno.

> Los usuarios de demostración `centro`, `norte` y `sur` (contraseña `1234`) todavía se crean en instalaciones nuevas. **Bórralos o cámbiales la contraseña antes de entregar**, hasta que se quiten de las semillas (ver hoja de ruta).

## Dónde quedan los datos

Todo está en `%LocalAppData%\PuntoDeVenta\` del usuario de Windows que abre el programa:

| Ruta | Contenido |
|---|---|
| `pos.db` | Base de datos: ventas, productos, usuarios, cortes **y la configuración del negocio (incluido el logo)**. |
| `logs\pos-AAAAMMDD.log` | Bitácora diaria (se guardan 60 días). Es lo primero que hay que pedir cuando el cliente reporta un problema. |

Los tickets PDF se guardan en `Documentos\TicketsPOS\`.

Para usar otra ubicación de la base de datos (por ejemplo, una carpeta compartida por varios usuarios de Windows del mismo equipo), define la variable de entorno `POS_DB_PATH` con la ruta completa del archivo.

## Respaldo y restauración

- **Respaldar:** cierra el programa y copia `%LocalAppData%\PuntoDeVenta\pos.db` a una USB o a la nube. Con ese único archivo se recupera todo.
- **Restaurar:** cierra el programa y reemplaza `pos.db` con el respaldo.
- Hacer el respaldo con el programa abierto puede copiar un archivo a medio escribir.

## Actualizar a una versión nueva

1. Cierra el programa y respalda `pos.db`.
2. Reemplaza `PuntoDeVenta.exe` (y `LatoFont/`) por los de la versión nueva.
3. Ábrelo: si la versión trae cambios de base de datos, se aplican solos al arrancar. Los datos se conservan.

## Instalaciones anteriores

Las versiones anteriores guardaban la base en `Documentos\PuntoDeVenta\pos.db`. La primera vez que corre esta versión, la copia a `%LocalAppData%` y deja el original como respaldo.

## Problemas frecuentes

| Síntoma | Causa probable | Solución |
|---|---|---|
| "No se pudo abrir la base de datos" al arrancar | Sin permisos en la carpeta o `pos.db` dañado. | Revisar la bitácora; restaurar el último respaldo. |
| "La caja tiene un turno abierto por …" | El cajero anterior no hizo corte. | Que esa persona (o un administrador) entre y haga el corte. |
| No se abre el ticket | No hay un visor de PDF predeterminado. | Instalar un visor de PDF o abrir el archivo desde `Documentos\TicketsPOS`. |
| Olvidaron la contraseña del administrador | — | Hoy requiere soporte técnico (editar la base). Pendiente: recuperación de acceso. |

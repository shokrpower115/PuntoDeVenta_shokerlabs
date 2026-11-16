# 0005. Publicación autocontenida de un solo ejecutable

- **Estado:** Aceptada
- **Fecha:** 2026-11-15

## Contexto
Los equipos de los clientes no tienen el runtime de .NET 8 de escritorio y no se puede contar con que alguien lo instale. Instalar dependencias en sitio es la causa más común de "en mi máquina sí funciona".

## Decisión
Perfil de publicación `Cliente` (`src/POS.UI/Properties/PublishProfiles/Cliente.pubxml`): `win-x64`, autocontenido, un solo archivo, con las bibliotecas nativas (SQLite) empaquetadas y compresión.

```bash
dotnet publish src/POS.UI -p:PublishProfile=Cliente
```

## Consecuencias
- El cliente solo necesita Windows 10/11 de 64 bits.
- El ejecutable pesa alrededor de 95 MB porque incluye .NET.
- La carpeta `LatoFont/` (fuentes de los tickets) debe acompañar al ejecutable.
- Sin firma de código, Windows SmartScreen muestra una advertencia la primera vez (pendiente: certificado de firma).
- Falta un instalador con acceso directo y desinstalador (Inno Setup o MSIX); está en la hoja de ruta.

## Alternativas descartadas
- **Publicación dependiente del runtime:** ejecutable pequeño, pero obliga a instalar .NET en cada cliente.
- **ClickOnce:** requiere un servidor de publicación y tiene limitaciones con apps autocontenidas de un solo archivo.

# Registro de decisiones (ADR)

Un ADR documenta una decisión que alguien podría cuestionar más adelante: el contexto, lo que se decidió, por qué y qué se descartó. **Antes de "corregir" algo que parece raro, revisa si hay un ADR que lo explique.**

| # | Decisión | Estado |
|---|---|---|
| [0001](0001-una-caja-por-sucursal.md) | Una caja (un turno abierto) por sucursal | Aceptada |
| [0002](0002-base-de-datos-local.md) | SQLite local en `%LocalAppData%`, migrada al arrancar | Aceptada |
| [0003](0003-marca-configurable.md) | La marca del negocio se configura en la base de datos, no en el código | Aceptada |
| [0004](0004-conteo-ciego.md) | Conteo ciego en el corte de caja | Aceptada |
| [0005](0005-publicacion-autocontenida.md) | Publicación autocontenida de un solo ejecutable | Aceptada |
| [0006](0006-sin-borrado-en-cascada.md) | Sin borrado en cascada; desactivar en lugar de borrar | Aceptada |

## Plantilla

Copia esto a `NNNN-titulo-corto.md` con el siguiente número:

```markdown
# NNNN. Título

- **Estado:** Propuesta | Aceptada | Reemplazada por NNNN
- **Fecha:** AAAA-MM-DD

## Contexto
Qué problema hay y qué restricciones existen.

## Decisión
Qué se hace.

## Consecuencias
Qué se gana, qué se pierde y qué habría que hacer si cambia el contexto.

## Alternativas descartadas
- Opción — por qué no.
```

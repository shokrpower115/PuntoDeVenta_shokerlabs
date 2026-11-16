# 0004. Conteo ciego en el corte de caja

- **Estado:** Aceptada
- **Fecha:** 2026-11-15

## Contexto
Al hacer el corte, el cajero captura cuánto efectivo contó. Si la pantalla le muestra antes cuánto *debería* haber, es fácil "ajustar" el conteo para que cuadre y ocultar faltantes o sobrantes.

## Decisión
La pantalla de corte muestra los datos del turno (quién lo abrió, desde cuándo y el fondo inicial), pero **no** el efectivo esperado. El cajero captura lo que contó; el esperado y la diferencia aparecen en el ticket del corte, después de confirmar (RN-COR-03).

## Consecuencias
- Las diferencias reflejan el conteo real y el dueño puede detectarlas.
- Si el cajero se equivoca al contar, la diferencia queda registrada. Corregir un corte cerrado requiere una función de administrador (no existe hoy).

## Alternativas descartadas
- **Mostrar el esperado antes de contar:** más cómodo, pero anula el control.
- **Configurable por negocio:** posible a futuro si un cliente lo pide; por defecto se protege al dueño.

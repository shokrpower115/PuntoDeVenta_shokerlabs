# 0001. Una caja por sucursal

- **Estado:** Aceptada
- **Fecha:** 2026-11-15

## Contexto
Antes, el turno abierto se buscaba solo por sucursal: si el cajero A dejaba su turno abierto, el cajero B que iniciaba sesión heredaba ese turno, vendía en él y podía cerrarlo con su propia contraseña. Las ventas de B quedaban en el corte de A. No existía la noción de "caja" como entidad.

## Decisión
Cada sucursal tiene **una sola caja** y, por lo tanto, **un solo turno abierto a la vez** (RN-TUR-01).
- El turno pertenece a quien lo abrió.
- Otro cajero no puede entrar mientras haya un turno abierto de alguien más (RN-TUR-03).
- El corte lo autoriza el dueño del turno o un administrador (RN-COR-04), así el negocio no se queda bloqueado si el cajero se fue sin cortar.

## Consecuencias
- Cada corte cuadra con el dinero de un solo cajón y un solo responsable.
- Un negocio con **varias cajas en la misma sucursal** no está soportado. Para soportarlo habría que agregar la entidad `Caja` (con su propio turno) y elegir la caja al iniciar sesión.

## Alternativas descartadas
- **Turno por usuario sin entidad caja:** dos cajeros con turnos abiertos al mismo tiempo compartirían un cajón físico y ningún corte cuadraría.
- **Permitir heredar el turno:** era el comportamiento anterior; mezcla responsables.

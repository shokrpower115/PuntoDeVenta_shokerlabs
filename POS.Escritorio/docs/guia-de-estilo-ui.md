# Guía de estilo de la interfaz

El sistema de diseño vive en `src/POS.UI/Theme.xaml`. **Las pantallas no definen colores, tamaños de letra ni plantillas propias**: usan los estilos de esta guía. Si falta algo, se agrega al tema y se documenta aquí.

## Principios

1. **La caja es rápida.** Lo más frecuente se hace con teclado y lector de código de barras, sin mouse.
2. **Una acción principal por pantalla.** Es el único botón con color de marca (o verde, si cobra dinero).
3. **El dinero siempre se ve grande.** Totales y cambio usan `TextoMonto`.
4. **Neutro + una marca.** La interfaz es gris/blanca; el color del negocio solo marca lo importante (acción principal, selección, foco). Así cualquier color de marca se ve bien.
5. **Los mensajes explican qué pasó y qué hacer**, en el lugar donde ocurrió, sin ventanas emergentes cuando no hace falta.

## Color

| Recurso | Uso | Cambia por negocio |
|---|---|---|
| `BrushPrimario` | Acción principal, foco, selección, indicador del menú. | Sí (`DynamicResource`) |
| `BrushPrimarioSuave` | Fondo de elementos seleccionados (filas, mosaicos). | Sí |
| `BrushPrimarioTexto` | Texto sobre `BrushPrimario` (blanco u oscuro según contraste). | Sí |
| `BrushFondo` | Fondo de ventanas. | No |
| `BrushSuperficie` / `BrushSuperficieAlt` | Tarjetas / encabezados de tabla, cajas internas. | No |
| `BrushBorde` / `BrushBordeFuerte` | Divisiones / bordes de campos y botones secundarios. | No |
| `BrushTexto` / `BrushTextoSecundario` / `BrushTextoTenue` | Texto principal / ayudas / placeholders. | No |
| `BrushMenu` | Menú lateral (oscuro). | No |
| `BrushExito` (+`Suave`) | Cobrar, cambio, confirmaciones. | No |
| `BrushPeligro` (+`Suave`) | Errores, desactivar, eliminar. | No |
| `BrushAdvertencia` (+`Suave`) | Avisos (sin stock, faltan datos). | No |

Regla: **todo lo que use el color de marca va con `DynamicResource`**, todo lo demás con `StaticResource`.

Los colores sugeridos en la configuración del negocio (`Marca.ColoresSugeridos`) tienen buen contraste con texto blanco. Si el cliente escribe un color claro, `Marca` cambia el texto de los botones a oscuro automáticamente.

## Tipografía

Fuente: **Segoe UI** (incluida en Windows). Tamaño base 14.

| Estilo | Tamaño | Uso |
|---|---|---|
| `TextoTitulo` | 24 seminegrita | Título de la pantalla o ventana (uno por pantalla). |
| `TextoSubtitulo` | 17 seminegrita | Título de una tarjeta o sección. |
| `TextoEtiqueta` | 13 seminegrita, gris | Etiqueta encima de un campo. |
| `TextoSecundario` | 13 gris | Explicaciones y datos de apoyo. |
| `TextoMonto` | 34 negrita, números tabulares | Total de la venta, total a cobrar. |
| `Icono` | 16, fuente de íconos | Íconos (ver tabla abajo). |

## Componentes

| Estilo | Cuándo usarlo |
|---|---|
| `VentanaBase` | **Toda** ventana nueva: `Style="{StaticResource VentanaBase}"`. |
| `Tarjeta` (Border) | Agrupar contenido sobre el fondo. `Padding="0"` cuando adentro hay una tabla. |
| `BotonPrimario` | La acción principal de la pantalla. |
| `BotonExito` | Acciones que cobran o confirman dinero (Cobrar, Confirmar cobro). |
| `BotonSecundario` | Acciones secundarias y "Cancelar". Es el estilo por defecto de cualquier `Button`. |
| `BotonPeligro` | Acciones destructivas que necesitan peso visual. |
| `BotonFantasma` | Acciones de poco peso: "← Volver", "Quitar", cerrar sesión. |
| `BotonIcono` | Botón cuadrado de 34 px con un glifo (+, −, eliminar). |
| `ItemMenu` (RadioButton) | Opciones del menú lateral. `Tag` = glifo del ícono. |
| `OpcionSegmento` (RadioButton) | Elegir entre 2–4 opciones excluyentes (Retiro / Entrada). |
| `ListaMosaicos` (ListBox) | Elegir entre opciones visuales (métodos de pago). |
| `CampoMonto` (TextBox) | Captura de dinero: grande y alineado a la derecha. |
| `TextBox` (por defecto) | El placeholder se pone en `Tag`: `<TextBox Tag="Buscar producto..."/>`. |
| `AvisoAdvertencia` / `AvisoError` / `AvisoExito` (Border) | Mensajes en línea. Se ocultan solos con `Visibility="{Binding Mensaje, Converter={StaticResource TextoNoVacioAVisibilidad}}"`. |
| `InsigniaNegocio` (Border) | Iniciales del negocio cuando no hay logo. |
| `DataGrid` (por defecto) | Tablas: solo lectura, sin líneas verticales, encabezados en MAYÚSCULAS. |

### Logo y nombre del negocio
Usa siempre los recursos dinámicos: `{DynamicResource NombreNegocio}`, `{DynamicResource LogoNegocio}`, `{DynamicResource InicialesNegocio}`, `{DynamicResource VisibilidadLogo}` y `{DynamicResource VisibilidadIniciales}`. Ejemplo en `MainWindow.xaml` (menú lateral).

## Íconos

Fuente **Segoe MDL2 Assets** (incluida en Windows 10/11), con el estilo `Icono`. Glifos en uso:

| Glifo | Significado | | Glifo | Significado |
|---|---|---|---|---|
| `&#xE7BF;` | Carrito / venta | | `&#xE713;` | Configuración / administrador |
| `&#xE721;` | Buscar | | `&#xE7B8;` | Inventario / productos |
| `&#xE710;` | Agregar | | `&#xE738;` | Quitar uno |
| `&#xE74D;` | Eliminar | | `&#xE73E;` | Confirmado |
| `&#xE7BA;` | Advertencia | | `&#xE946;` | Información |
| `&#xE8EF;` | Caja / corte | | `&#xE8AB;` | Movimiento / transferencia |
| `&#xE8C7;` | Tarjeta | | `&#xE774;` | Otras sucursales |
| `&#xE716;` | Usuarios | | `&#xE77B;` | Usuario en sesión |
| `&#xE72E;` | Autorización / contraseña | | `&#xE7E8;` | Cerrar sesión |
| `&#xE72B;` | Volver | | `&#xE72C;` | Actualizar |
| `&#xE790;` | Apariencia / negocio | | `&#xE70D;` | Desplegar |

El símbolo `$` (efectivo) se dibuja con la fuente de texto, no con la de íconos.

## Espaciado

- Margen de ventanas y diálogos: **28**. Área de contenido de la ventana principal: **28, 24**.
- Interior de tarjetas: **20**. Separación entre tarjetas: **20**.
- Entre un campo y la siguiente etiqueta: **12–16**. Entre botones: **8–10**.
- Alto mínimo de botones y campos: **40** (cómodo para pantallas táctiles). Acción principal de un diálogo: **44–50**.

## Teclado (caja)

| Tecla | Dónde | Acción |
|---|---|---|
| Enter | Buscador de venta | Agrega el producto (código de barras exacto, único resultado o el seleccionado). Pensado para el lector. |
| Enter / doble clic | Tabla de productos | Agrega el producto seleccionado. |
| F2 | Venta | Regresa al buscador. |
| F12 | Venta | Cobrar. |
| Enter / Esc | Ventana de cobro | Confirmar / cancelar. |

Al agregar un atajo nuevo, muéstralo en el botón (como la etiqueta `F12` de "Cobrar") y regístralo en esta tabla.

## Textos

- Tú, no usted: "Escribe tu usuario", "Selecciona un producto".
- Botones con verbo: "Crear producto", "Registrar movimiento", "Realizar corte y cerrar turno".
- Errores con causa y salida: "Stock insuficiente de 'Cuaderno': se piden 3 y hay 2. No se registró la venta."
- Montos siempre con formato de moneda (`StringFormat=C`).

## Lista de revisión para una pantalla nueva

- [ ] La ventana usa `VentanaBase`; no hay colores ni tamaños de letra escritos a mano.
- [ ] Hay un solo `BotonPrimario` (o `BotonExito`).
- [ ] Todo lo que use el color de marca está con `DynamicResource`.
- [ ] Los campos tienen etiqueta (`TextoEtiqueta`) y, si ayuda, placeholder (`Tag`).
- [ ] Los mensajes usan los `Aviso…` y se ocultan cuando están vacíos.
- [ ] Se puede operar con teclado (foco inicial, Enter, Esc).
- [ ] Se ve bien a 1024×640 (el tamaño mínimo de la ventana principal).

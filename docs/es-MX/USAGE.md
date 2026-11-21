# Uso

> Traducción asistida por máquina. Si difiere de la [versión en inglés](../USAGE.md), rige la versión en inglés. Las correcciones son bienvenidas: [abre un issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

El tracker funciona en segundo plano, con un ícono dorado "XP" en el área de notificación (*notification area*), abajo a la derecha de la barra de tareas. Tiene tres partes:

- el **overlay** en el juego
- la ventana del **panel**
- el archivo del **historial de partidas**

Los textos entre comillas, como "Tiempo de EXP:", son los textos que muestra el tracker.

<a id="overlay"></a>
## Overlay

El overlay es un pequeño recuadro abajo a la derecha, sobre el botón de opciones de Hearthstone (el engrane en la esquina inferior derecha). En el panel puedes moverlo abajo a la izquierda, sobre tu retrato ("You" con tu rango). Las etiquetas están a la izquierda. Los valores están a la derecha, en barras azules.

El overlay solo se muestra mientras Hearthstone es la ventana activa. Nunca toma el foco, y todos los clics lo atraviesan y llegan al juego.

El overlay solo se muestra en **Clasificado, Casual, Gresca de la taberna y Arena**. Los demás modos todavía no se miden, así que ahí el tracker no muestra nada. Una Gresca de la taberna contra la computadora (JcE, por ejemplo contra el Tabernero) no da EXP, así que ahí el overlay muestra "+0".

<a id="during-a-game"></a>
### Durante una partida

Las filas aparecen con el primer turno. Mientras eliges tu mano inicial, el overlay está oculto.

| Fila | Significado |
|---|---|
| "Tiempo de EXP:" | El tiempo que cuenta para la EXP hasta ahora, en minutos y segundos |
| "Si ganas:" | La EXP que obtienes si ganas ahora |
| "Si pierdes:" | La EXP que obtienes si pierdes o te rindes ahora |

El tiempo de EXP puede llevar una nota:

| Nota | Significado |
|---|---|
| "(en pausa)" | Dejaste que tu turno se acabara sin hacer ninguna acción. Ese turno y el siguiente turno del oponente no cuentan, así que el tiempo de EXP retrocedió y espera. Vuelve a correr en tu siguiente turno. |
| "(máx.)" | El tiempo de EXP llegó a 30 minutos. Más tiempo ya no da más EXP. |

Durante un evento de Hearthstone, activa "Evento en curso (mostrar EXP de evento)" en el panel. Entonces el overlay muestra una fila más, en vivo y en el resumen después de la partida:

| Fila | Significado |
|---|---|
| "EXP de evento:" | La EXP de evento hasta ahora, con un cristal rojo: una por minuto de tiempo de EXP (consulta [FORMULA.md](../FORMULA.md#event-xp)). No depende del resultado. |

Desactívalo cuando termine el evento.

Con "Pase de la taberna (bonificación de EXP)" activado en el panel, "Si ganas:", "Si pierdes:" y "Obtenida:" incluyen la bonificación del pase según tu nivel en la Senda de recompensas. La EXP de evento no recibe bonificación.

Las dos últimas filas muestran el daño, en vivo y en el resumen después de la partida. Desactívalas con "Mostrar el daño en el overlay" en el panel.

| Fila | Significado |
|---|---|
| "Daño total:" | Todo el daño que tu lado hizo al héroe y a los esbirros del oponente hasta ahora: tus ataques, los ataques de tus esbirros, hechizos, armas, Poder de héroe y otros efectos. Cada golpe cuenta con la cantidad que muestra el juego, también cuando supera la salud del objetivo o pega en la armadura. El daño a tu propio héroe o a tus esbirros no cuenta, y tampoco el daño sin origen, como la fatiga. Un golpe contra un Escudo divino cuenta 0. |
| "Daño al héroe:" | La parte de "Daño total:" que golpeó al héroe del oponente, incluida la parte que absorbió la armadura |

El daño se lee cuando el servidor confirma una acción, así que puede subir un momento antes de la animación en el tablero.

Mientras el tiempo de EXP esté por debajo de 60 segundos, los dos valores de EXP muestran "+0". Las partidas de menos de un minuto no dan EXP. Consulta [FORMULA.md](../FORMULA.md) para ver las reglas.

Si inicias el tracker a mitad de una partida, de todos modos lee la partida desde el principio.

<a id="after-a-game"></a>
### Después de una partida

Después de la partida, el overlay muestra un resumen hasta la mano inicial de tu siguiente partida:

| Fila | Significado |
|---|---|
| "Última partida:" | Victoria o Derrota |
| "Tiempo de EXP:" | El tiempo que contó para la EXP |
| "Tiempo total:" | Toda la partida, incluida la mano inicial |
| "Obtenida:" | La EXP que dio la partida |

<a id="update-hint"></a>
### Aviso de actualización

Cuando existe una versión más nueva del tracker, el overlay muestra una fila más mientras no hay ninguna partida en curso:

| Fila | Significado |
|---|---|
| "Actualización disponible:" | El número de la nueva versión, por ejemplo 0.2.0 |

El panel muestra un enlace a la página de descarga. Consulta [INSTALL.md](INSTALL.md#update) para saber cómo actualizar.

<a id="dashboard"></a>
## Panel

La ventana del panel se llama "HS XP Tracker". Se abre cuando arranca el tracker. Para volver a abrirla:

- haz clic en el ícono dorado "XP" del área de notificación (*notification area*),
- haz clic derecho en el ícono y elige "Panel de EXP", o
- vuelve a iniciar `HearthstoneXpTracker.exe`.

Si cierras el panel, el tracker y el overlay siguen funcionando. Para cerrar el tracker, haz clic derecho en el ícono y elige "Salir".

<a id="toolbar"></a>
### Barra de herramientas

| Control | Significado |
|---|---|
| "Sesión" | Partidas de la sesión actual de Hearthstone, desde que abres el juego hasta que lo cierras. Después de cerrar Hearthstone, la última sesión sigue visible hasta el siguiente inicio. Es el periodo que se muestra al abrir el panel. |
| "Hoy" | Partidas desde la medianoche |
| "Esta semana" | Partidas desde el lunes a las 00:00 |
| "Esta temporada" | Partidas desde el día 1 del mes. Las temporadas de Clasificado en Hearthstone duran un mes calendario. |
| "Últimos 30 días" | Partidas de los últimos 30 días, incluido hoy |
| "Todo" | Todas las partidas del historial |
| "Actualizar" | Vuelve a leer el historial |
| "Abrir carpeta de datos" | Abre la carpeta del archivo del historial en el Explorador de Windows |
| "Idioma (Language)" | Idioma del overlay, del panel y del menú del ícono. El inglés es el idioma predeterminado. El cambio se aplica de inmediato; el panel se vuelve a abrir en el nuevo idioma. En los idiomas que no son inglés, la etiqueta conserva la palabra en inglés, como aquí: "Idioma (Language)". |
| "Posición del overlay" | Dónde está el overlay: "Derecha, sobre el botón de opciones" (predeterminado) o "Izquierda, sobre tu retrato", así el lado derecho queda libre, por ejemplo para la lista de mazo de Hearthstone Deck Tracker. El overlay se mueve al instante. |
| "Evento en curso (mostrar EXP de evento)" | Actívalo durante un evento de Hearthstone: el overlay muestra la EXP de evento y las partidas nuevas la guardan. El tracker no puede saber cuándo hay un evento, así que esta opción está desactivada de forma predeterminada; mientras lo esté, un breve aviso debajo te lo recuerda. |
| "Pase de la taberna (bonificación de EXP)" | Actívalo si tienes el Pase de la taberna de la expansión actual. La bonificación se calcula, no se mide: con el pase, la EXP mostrada puede variar en 1 EXP por partida (consulta [FORMULA.md](../FORMULA.md#tavern-pass)). |
| "Nivel" con "−", un número y "+" | Tu nivel en la Senda de recompensas. Haz clic en "−" o "+", o escribe el número. La línea de abajo muestra la bonificación y el siguiente paso, por ejemplo "10 % de bonificación de EXP, 2 niveles para 15 %". La bonificación es del 10 %, desde el nivel 20 del 15 % y desde el nivel 70 del 20 % (consulta [FORMULA.md](../FORMULA.md#tavern-pass)). Actualiza el nivel cuando pases el nivel 20 o el 70. |
| "Mostrar el daño en el overlay" | Muestra u oculta las filas "Daño total:" y "Daño al héroe:". Activado de forma predeterminada. |
| "Buscar actualizaciones al iniciar el tracker" | Activa o desactiva la búsqueda de actualizaciones. Consulta [Privacidad](#privacy). |
| "Iniciar con Windows" | Inicia el tracker al iniciar sesión en Windows. Entonces solo aparece en el área de notificación; haz clic en el ícono para abrir el panel. Desactivado de forma predeterminada. También puedes desactivarlo en el Administrador de tareas, en *Aplicaciones de arranque* (*Startup apps*). |
| "Acceso directo en el menú Inicio" | Agrega "Hearthstone XP Tracker" al menú Inicio o lo quita. En el primer inicio, una barra en la parte superior del panel lo pregunta una vez. |

Una partida terminada aparece sola en el panel abierto. No necesitas hacer clic en "Actualizar".

En "Sesión", una línea debajo de la barra de herramientas indica la sesión, por ejemplo "Sesión desde mar. 2026-11-10, 19:56 (en curso)" o "(terminada)". Una sesión puede seguir después de la medianoche.

Debajo, una línea de estado indica si Hearthstone escribe el registro de partida que lee el tracker: "Registro de partida (Power.log): activado". Si el tracker tuvo que activar el registro con Hearthstone abierto, la línea te pide en rojo que reinicies Hearthstone una vez. Si no pudo activarlo, lo indica en rojo y menciona el archivo (consulta [INSTALL.md](INSTALL.md#troubleshooting)).

<a id="tabs"></a>
### Pestañas

**Resumen**

| Recuadro | Significado |
|---|---|
| "EXP" | EXP obtenida en el periodo |
| "Tiempo de EXP" | Tiempo que contó para la EXP |
| "Tiempo de juego con mano inicial" | Tiempo en partidas, incluida la mano inicial |
| "Tiempo en línea" | Tiempo en que Hearthstone estuvo abierto. Ver abajo. |
| "Partidas" | Número de partidas, con victorias y derrotas |
| "EXP por hora" | EXP por hora de tiempo de EXP |
| "EXP por hora en línea" | EXP por hora de tiempo en línea, o sea, incluidos los menús y la cola |
| "Porcentaje de victorias" | Proporción de partidas ganadas |
| "EXP de evento" | EXP de evento del periodo; solo se muestra si se jugaron partidas durante un evento |

El tiempo en línea se cuenta por sesión de Hearthstone: desde que se abre Hearthstone hasta el final de la última partida. Mientras Hearthstone está abierto, cuenta hasta ahora. Solo cuentan las sesiones con al menos una partida en el periodo.

Las partidas en modos sin tasas medidas, por ejemplo Campos de batalla, se guardan sin EXP. Aparecen en "Partidas" y cuentan para el tiempo de juego y el tiempo en línea, pero no para los demás recuadros ni los gráficos.

**EXP perdida**

| Recuadro | Significado |
|---|---|
| "Turnos AFK" | EXP que perdiste por turnos AFK: turnos sin ninguna acción que se acabaron sin "LISTO", y los turnos del oponente que les siguen |
| "Partidas de menos de 1 minuto" | EXP que daría la fórmula por las partidas de menos de 60 segundos, que el juego no paga |
| "Tiempo fuera de partidas" | Tiempo en línea fuera de partidas (menús, cola, pausas), calculado con tu EXP por hora del periodo |

**Partidas**

Todas las partidas del periodo, de la más reciente a la más antigua: fecha, inicio, modo, resultado, tiempo de EXP, tiempo total, EXP, EXP de evento, turnos AFK y clase del oponente. Haz clic en el encabezado de una columna para ordenar.

**Gráficos**

Dos curvas sobre el mismo eje de tiempo. El eje siempre cubre todo el periodo, así que las horas o los días sin partidas siguen visibles:

| Periodo | Eje de tiempo |
|---|---|
| "Sesión" | cada hora desde el inicio de la sesión hasta su final, o hasta ahora mientras sigue en curso |
| "Hoy" | las 24 horas del día |
| "Esta semana" | de lunes a domingo |
| "Esta temporada" | cada día del mes |
| "Últimos 30 días" | los últimos 30 días, incluido hoy |
| "Todo" | cada día desde la primera partida del historial |

| Curva | Significado |
|---|---|
| "EXP obtenida" (dorada) | EXP de las partidas que empezaron en cada hora o en cada día. Los pasos sin partidas quedan en 0. |
| "EXP por hora" (azul) | Velocidad en cada hora o en cada día: EXP dividida entre el tiempo de EXP. Solo los pasos con partidas tienen un punto. La línea discontinua es el promedio de todo el periodo. |

Una partida cuenta en la hora o en el día en que empezó.

<a id="game-history-file"></a>
## Archivo del historial de partidas

Cada partida terminada se agrega a un archivo CSV:

```
%APPDATA%\HearthstoneXpTracker\hs_games.csv
```

El archivo usa comas entre los campos y un punto como separador decimal. Las horas son en hora local.

| Columna | Contenido |
|---|---|
| `Session` | Nombre de la carpeta de logs de Hearthstone de la sesión, por ejemplo `Hearthstone_2026_11_08_09_10_51` |
| `Start` | Inicio del primer turno, `yyyy-MM-ddTHH:mm:ss.fff` |
| `End` | Final de la partida, mismo formato |
| `Mode` | Modo de juego, por ejemplo `Ranked` |
| `Format` | Formato, por ejemplo Standard o Wild; vacío si el modo no tiene formato |
| `Result` | Por ejemplo `Win` o `Loss` |
| `XpSeconds` | Tiempo que contó para la EXP |
| `PlaySeconds` | Tiempo desde el primer turno hasta el final, incluidos los turnos AFK |
| `WithMulliganSeconds` | Tiempo incluida la mano inicial |
| `AfkTurns` | Número de turnos AFK propios |
| `Xp` | EXP calculada por el tracker; vacía en los modos sin tasas medidas |
| `Deck` | Nombre del mazo, solo en las partidas registradas por el antiguo plugin de HDT; vacío en las partidas más recientes |
| `PlayerClass` | Tu clase |
| `OpponentClass` | La clase del oponente |
| `EventXp` | EXP de evento calculada por el tracker; vacía en las partidas fuera de eventos. Los archivos de versiones anteriores sin esta columna se siguen leyendo, y la columna se agrega con la siguiente partida. |
| `XpBonus` | Bonificación de EXP del Pase de la taberna, en porcentaje, que aplicaba cuando se jugó la partida, por ejemplo `15`; `0` sin el pase. Los archivos anteriores sin esta columna se leen como `0`. |

La columna `Xp` la calcula el tracker a partir del tiempo de EXP. El juego en sí no registra la EXP. Cuando se abre el panel, el tracker vuelve a calcular la EXP de todas las partidas guardadas con las tasas actuales, así que una tasa corregida también arregla las partidas anteriores. Cada partida conserva su propio `XpBonus`, así que si después cambias el pase o el nivel, las partidas anteriores no cambian.

Una partida leída dos veces del mismo log se guarda solo una vez. Al instalar una nueva versión, el archivo se conserva. Cada vez que arranca, el tracker también agrega las partidas del historial del antiguo plugin de HDT que todavía no tiene (consulta [INSTALL.md](INSTALL.md#coming-from-the-hdt-plugin)).

<a id="privacy"></a>
## Privacidad

- El historial de partidas y la configuración se quedan en tu computadora. El tracker no sube nada.
- La única solicitud de red es la búsqueda de actualizaciones. Cuando el tracker arranca, le pregunta una vez a GitHub si existe una versión más nueva. La solicitud solo envía el nombre y la versión del programa, por ejemplo `HearthstoneXpTracker/0.2.0`. Como en cualquier solicitud web, GitHub ve tu dirección IP.
- Para desactivar la búsqueda de actualizaciones, desmarca "Buscar actualizaciones al iniciar el tracker" en el panel. La opción se guarda en `%APPDATA%\HearthstoneXpTracker\settings.ini`.
- El tracker solo lee el log de Hearthstone. No lee la memoria del juego ni modifica archivos del juego. Solo escribe en su propia carpeta de datos y, una sola vez, la sección `[Power]` de `log.config` (consulta [INSTALL.md](INSTALL.md#requirements)).

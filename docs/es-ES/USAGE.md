# Uso

> Traducción asistida por máquina. Si difiere de la [versión en inglés](../USAGE.md), prevalece la versión en inglés. Las correcciones son bienvenidas: [abre una incidencia](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

El tracker funciona en segundo plano, con un icono dorado "XP" en el área de notificación (*notification area*), abajo a la derecha de la barra de tareas. Tiene tres partes:

- el **overlay** en el juego
- la ventana del **panel**
- el archivo del **historial de partidas**

Los textos entre comillas, como "Tiempo de PE:", son los textos que muestra el tracker.

<a id="overlay"></a>
## Overlay

El overlay es un pequeño recuadro abajo a la derecha, sobre el botón de opciones de Hearthstone (el engranaje de la esquina inferior derecha). En el panel puedes moverlo abajo a la izquierda, sobre tu retrato ("You" con tu rango). Las etiquetas están a la izquierda. Los valores están a la derecha, en barras azules.

El overlay solo se muestra mientras Hearthstone es la ventana activa. Nunca toma el foco, y todos los clics lo atraviesan y llegan al juego.

El overlay solo se muestra en **Con rango, Aficionado, Pelea de taberna y Arena**. Los demás modos aún no se miden, así que en ellos el tracker no muestra nada. Una Pelea de taberna contra el ordenador (JcE, por ejemplo contra el tabernero) no da PE, así que en ella el overlay muestra "+0".

<a id="during-a-game"></a>
### Durante una partida

Las filas aparecen con el primer turno. Mientras eliges tu mano inicial, el overlay está oculto.

| Fila | Significado |
|---|---|
| "Tiempo de PE:" | El tiempo que cuenta para los PE hasta ahora, en minutos y segundos |
| "Si ganas:" | Los PE que consigues si ganas ahora |
| "Si pierdes:" | Los PE que consigues si pierdes o te rindes ahora |

El tiempo de PE puede llevar una nota:

| Nota | Significado |
|---|---|
| "(en pausa)" | Has dejado que tu turno se agote sin hacer ninguna acción. Ese turno y el siguiente turno del rival no cuentan, así que el tiempo de PE ha retrocedido y espera. Vuelve a correr en tu siguiente turno. |
| "(máx.)" | El tiempo de PE ha llegado a 30 minutos. Más tiempo ya no da más PE. |

Durante un evento de Hearthstone, activa "Evento activo (mostrar PE de evento)" en el panel. Entonces el overlay muestra una fila más, en directo y en el resumen después de la partida:

| Fila | Significado |
|---|---|
| "PE de evento:" | Los PE de evento hasta ahora, con un cristal rojo: uno por minuto de tiempo de PE (consulta [FORMULA.md](../FORMULA.md#event-xp)). No dependen del resultado. |

Desactívalo cuando termine el evento.

Con "Pase de taberna (bonificación de PE)" activado en el panel, "Si ganas:", "Si pierdes:" y "Obtenidos:" incluyen la bonificación del pase según tu nivel en el Mapa de recompensas. Los PE de evento no reciben bonificación.

Las dos últimas filas muestran el daño, en directo y en el resumen después de la partida. Desactívalas con "Mostrar el daño en el overlay" en el panel.

| Fila | Significado |
|---|---|
| "Daño total:" | Todo el daño que tu bando ha hecho al héroe y a los esbirros del rival hasta ahora: tus ataques, los ataques de tus esbirros, hechizos, armas, poder de héroe y otros efectos. Cada golpe cuenta con la cantidad que muestra el juego, también cuando supera la salud del objetivo o da en la armadura. El daño a tu propio héroe o a tus esbirros no cuenta, ni tampoco el daño sin origen, como la fatiga. Un golpe contra un Escudo divino cuenta 0. |
| "Daño al héroe:" | La parte de "Daño total:" que ha alcanzado al héroe rival, incluida la parte que ha absorbido la armadura |

El daño se lee cuando el servidor confirma una acción, así que puede subir un momento antes de la animación en el tablero.

Mientras el tiempo de PE esté por debajo de 60 segundos, los dos valores de PE muestran "+0". Las partidas de menos de un minuto no dan PE. Consulta [FORMULA.md](../FORMULA.md) para ver las reglas.

Si inicias el tracker en mitad de una partida, lee igualmente la partida desde el principio.

<a id="after-a-game"></a>
### Después de una partida

Después de la partida, el overlay muestra un resumen hasta la mano inicial de tu siguiente partida:

| Fila | Significado |
|---|---|
| "Última partida:" | Victoria o Derrota |
| "Tiempo de PE:" | El tiempo que ha contado para los PE |
| "Tiempo total:" | Toda la partida, incluida la mano inicial |
| "Obtenidos:" | Los PE que ha dado la partida |

<a id="update-hint"></a>
### Aviso de actualización

Cuando existe una versión más reciente del tracker, el overlay muestra una fila más mientras no hay ninguna partida en curso:

| Fila | Significado |
|---|---|
| "Actualización disponible:" | El número de la nueva versión, por ejemplo 0.2.0 |

El panel muestra un enlace a la página de descarga. Consulta [INSTALL.md](INSTALL.md#update) para saber cómo actualizar.

<a id="dashboard"></a>
## Panel

La ventana del panel se llama "HS XP Tracker". Se abre al iniciar el tracker. Para volver a abrirla:

- haz clic en el icono dorado "XP" del área de notificación (*notification area*),
- haz clic con el botón derecho en el icono y elige "Panel de PE", o
- vuelve a iniciar `HearthstoneXpTracker.exe`.

Si cierras el panel, el tracker y el overlay siguen funcionando. Para cerrar el tracker, haz clic con el botón derecho en el icono y elige "Salir".

<a id="toolbar"></a>
### Barra de herramientas

| Control | Significado |
|---|---|
| "Sesión" | Partidas de la sesión actual de Hearthstone, desde que abres el juego hasta que lo cierras. Después de cerrar Hearthstone, la última sesión sigue visible hasta el siguiente inicio. Es el periodo que se muestra al abrir el panel. |
| "Hoy" | Partidas desde medianoche |
| "Esta semana" | Partidas desde el lunes a las 00:00 |
| "Esta temporada" | Partidas desde el día 1 del mes. Las temporadas de Con rango en Hearthstone duran un mes natural. |
| "Últimos 30 días" | Partidas de los últimos 30 días, incluido hoy |
| "Todo" | Todas las partidas del historial |
| "Actualizar" | Vuelve a leer el historial |
| "Abrir carpeta de datos" | Abre la carpeta del archivo del historial en el Explorador de Windows |
| "Idioma (Language)" | Idioma del overlay, del panel y del menú del icono. El inglés es el idioma predeterminado. El cambio se aplica al momento; el panel se vuelve a abrir en el nuevo idioma. En los idiomas distintos del inglés, la etiqueta mantiene la palabra inglesa, como aquí: "Idioma (Language)". |
| "Posición del overlay" | Dónde está el overlay: "Derecha, sobre el botón de opciones" (predeterminado) o "Izquierda, sobre tu retrato", así el lado derecho queda libre, por ejemplo para la lista del mazo de Hearthstone Deck Tracker. El overlay se mueve al momento. |
| "Evento activo (mostrar PE de evento)" | Actívalo durante un evento de Hearthstone: el overlay muestra los PE de evento y las partidas nuevas los guardan. El tracker no puede saber cuándo hay un evento, así que esta opción está desactivada de forma predeterminada; mientras lo esté, un breve aviso debajo te lo recuerda. |
| "Pase de taberna (bonificación de PE)" | Actívalo si tienes el Pase de taberna de la expansión actual. La bonificación se calcula, no se mide: con el pase, los PE mostrados pueden desviarse 1 PE por partida (consulta [FORMULA.md](../FORMULA.md#tavern-pass)). |
| "Nivel" con "−", un número y "+" | Tu nivel en el Mapa de recompensas. Pulsa "−" o "+", o escribe el número. La línea de debajo muestra la bonificación y el siguiente escalón, por ejemplo "10 % de bonificación de PE, 2 niveles para el 15 %". La bonificación es del 10 %, a partir del nivel 20 del 15 % y a partir del nivel 70 del 20 % (consulta [FORMULA.md](../FORMULA.md#tavern-pass)). Actualiza el nivel cuando superes el nivel 20 o el 70. |
| "Mostrar el daño en el overlay" | Muestra u oculta las filas "Daño total:" y "Daño al héroe:". Activado por defecto. |
| "Buscar actualizaciones al iniciar el tracker" | Activa o desactiva la búsqueda de actualizaciones. Consulta [Privacidad](#privacy). |

Una partida terminada aparece sola en el panel abierto. No hace falta pulsar "Actualizar".

En "Sesión", una línea debajo de la barra de herramientas indica la sesión, por ejemplo "Sesión desde ma. 2026-11-10, 19:56 (en curso)" o "(terminada)". Una sesión puede pasar de la medianoche.

Debajo, una línea de estado indica si Hearthstone escribe el registro de partida que lee el tracker: "Registro de partida (Power.log): activado". Si el tracker tuvo que activar el registro con Hearthstone abierto, la línea te pide en rojo que reinicies Hearthstone una vez. Si no pudo activarlo, lo indica en rojo y menciona el archivo (consulta [INSTALL.md](INSTALL.md#troubleshooting)).

<a id="tabs"></a>
### Pestañas

**Resumen**

| Recuadro | Significado |
|---|---|
| "PE" | PE obtenidos en el periodo |
| "Tiempo de PE" | Tiempo que ha contado para los PE |
| "Tiempo de juego con mano inicial" | Tiempo en partidas, incluida la mano inicial |
| "Tiempo conectado" | Tiempo que Hearthstone ha estado abierto. Ver abajo. |
| "Partidas" | Número de partidas, con victorias y derrotas |
| "PE por hora" | PE por hora de tiempo de PE |
| "PE por hora conectado" | PE por hora de tiempo conectado, es decir, incluidos los menús y la cola |
| "Porcentaje de victorias" | Proporción de partidas ganadas |
| "PE de evento" | PE de evento del periodo; solo se muestra si se han jugado partidas durante un evento |

El tiempo conectado se cuenta por sesión de Hearthstone: desde que se abre Hearthstone hasta el final de la última partida. Mientras Hearthstone está abierto, cuenta hasta este momento. Solo cuentan las sesiones con al menos una partida en el periodo.

Las partidas en modos sin tasas medidas, por ejemplo Campos de batalla, se guardan sin PE. Aparecen en "Partidas" y cuentan para el tiempo de juego y el tiempo conectado, pero no para los demás recuadros ni los gráficos.

**PE perdidos**

| Recuadro | Significado |
|---|---|
| "Turnos AFK" | PE que has perdido por turnos AFK: turnos sin ninguna acción que se han agotado sin pulsar "LISTO", y los turnos del rival que los siguen |
| "Partidas de menos de 1 minuto" | PE que daría la fórmula por las partidas de menos de 60 segundos, que el juego no paga |
| "Tiempo fuera de partidas" | Tiempo conectado fuera de partidas (menús, cola, pausas), valorado con tus PE por hora del periodo |

**Partidas**

Todas las partidas del periodo, de la más reciente a la más antigua: fecha, inicio, modo, resultado, tiempo de PE, tiempo total, PE, PE de evento, turnos AFK y clase del rival. Haz clic en el encabezado de una columna para ordenar.

**Gráficos**

Dos curvas sobre el mismo eje de tiempo. El eje siempre abarca todo el periodo, así que las horas o los días sin partidas siguen visibles:

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
| "PE obtenidos" (dorada) | PE de las partidas que empezaron en cada hora o en cada día. Los tramos sin partidas quedan en 0. |
| "PE por hora" (azul) | Ritmo en cada hora o en cada día: PE divididos entre el tiempo de PE. Solo los tramos con partidas tienen un punto. La línea discontinua es la media de todo el periodo. |

Una partida cuenta en la hora o en el día en que empezó.

<a id="game-history-file"></a>
## Archivo del historial de partidas

Cada partida terminada se añade a un archivo CSV:

```
%APPDATA%\HearthstoneXpTracker\hs_games.csv
```

El archivo usa comas entre los campos y un punto como separador decimal. Las horas están en hora local.

| Columna | Contenido |
|---|---|
| `Session` | Nombre de la carpeta de logs de Hearthstone de la sesión, por ejemplo `Hearthstone_2026_11_08_09_10_51` |
| `Start` | Inicio del primer turno, `yyyy-MM-ddTHH:mm:ss.fff` |
| `End` | Final de la partida, mismo formato |
| `Mode` | Modo de juego, por ejemplo `Ranked` |
| `Format` | Formato, por ejemplo Standard o Wild; vacío si el modo no tiene formato |
| `Result` | Por ejemplo `Win` o `Loss` |
| `XpSeconds` | Tiempo que ha contado para los PE |
| `PlaySeconds` | Tiempo desde el primer turno hasta el final, incluidos los turnos AFK |
| `WithMulliganSeconds` | Tiempo incluida la mano inicial |
| `AfkTurns` | Número de turnos AFK propios |
| `Xp` | PE calculados por el tracker; vacío en los modos sin tasas medidas |
| `Deck` | Nombre del mazo, solo en las partidas registradas por el antiguo plugin de HDT; vacío en las partidas más recientes |
| `PlayerClass` | Tu clase |
| `OpponentClass` | La clase del rival |
| `EventXp` | PE de evento calculados por el tracker; vacío en las partidas fuera de eventos. Los archivos de versiones anteriores sin esta columna se siguen leyendo, y la columna se añade con la siguiente partida. |
| `XpBonus` | Bonificación de PE del Pase de taberna, en porcentaje, que se aplicaba cuando se jugó la partida, por ejemplo `15`; `0` sin el pase. Los archivos anteriores sin esta columna se leen como `0`. |

La columna `Xp` la calcula el tracker a partir del tiempo de PE. El propio juego no registra los PE. Al abrir el panel, el tracker vuelve a calcular los PE de todas las partidas guardadas con las tasas actuales, así que una tasa corregida también corrige las partidas anteriores. Cada partida conserva su propio `XpBonus`, así que si más adelante cambias el pase o el nivel, las partidas anteriores no cambian.

Una partida leída dos veces del mismo log se guarda solo una vez. Al instalar una nueva versión, el archivo se conserva. Cada vez que se inicia, el tracker también añade las partidas del historial del antiguo plugin de HDT que aún no tiene (consulta [INSTALL.md](INSTALL.md#coming-from-the-hdt-plugin)).

<a id="privacy"></a>
## Privacidad

- El historial de partidas y los ajustes se quedan en tu ordenador. El tracker no sube nada.
- La única petición de red es la búsqueda de actualizaciones. Cuando se inicia el tracker, pregunta una vez a GitHub si existe una versión más reciente. La petición solo envía el nombre y la versión del programa, por ejemplo `HearthstoneXpTracker/0.2.0`. Como en cualquier petición web, GitHub ve tu dirección IP.
- Para desactivar la búsqueda de actualizaciones, desmarca "Buscar actualizaciones al iniciar el tracker" en el panel. El ajuste se guarda en `%APPDATA%\HearthstoneXpTracker\settings.ini`.
- El tracker solo lee el log de Hearthstone. No lee la memoria del juego ni modifica archivos del juego. Solo escribe en su propia carpeta de datos y, una sola vez, la sección `[Power]` de `log.config` (consulta [INSTALL.md](INSTALL.md#requirements)).

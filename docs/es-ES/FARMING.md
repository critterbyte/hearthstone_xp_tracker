# Cómo conseguir más PE

> Traducción asistida por máquina. Si difiere de la [versión en inglés](../FARMING.md), prevalece la versión en inglés. Las correcciones son bienvenidas: [abre una incidencia](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

Consejos limpios para conseguir más PE del Mapa de recompensas por hora. Se derivan de la [fórmula de PE](../FORMULA.md). Las afirmaciones se marcan igual que allí:

- **Medido:** lo ha descubierto este proyecto.
- **Según [n]:** de las fuentes que hay al final de esta página.

El panel del plugin muestra adónde van tus PE. La pestaña "PE perdidos" es un buen punto de partida (consulta [USAGE.md](USAGE.md#tabs)).

<a id="play-fair"></a>
## Juega limpio

Esta página trata de jugar de forma normal y eficiente. Los bots, el win trading, compartir cuenta y otras formas de saltarse las normas del juego incumplen las condiciones de Blizzard y pueden hacer que te suspendan la cuenta. Aquí no se tratan.

<a id="choose-the-mode"></a>
## Elige el modo

| Modo | Victoria | Derrota |
|---|---:|---:|
| Con rango (Estándar y Salvaje) | 470 PE/h | 336 PE/h |
| Aficionado, Pelea de taberna y Arena | 336 PE/h | 240 PE/h |

- **Medido:** Con rango da 1,40 veces lo que dan Aficionado, Pelea de taberna y Arena. Una victoria en Aficionado da lo mismo que una derrota en Con rango.
- **Medido:** Estándar y Salvaje dan lo mismo. Juega el formato que más te guste o en el que más ganes.
- **Medido:** una victoria da 1,40 veces lo que una derrota. Tu porcentaje de victorias importa, así que juega un mazo con el que ganes.
- **Según [1]:** Campos de batalla da unos 300 PE por hora. Este proyecto no lo ha medido.

<a id="do-not-go-afk"></a>
## No te quedes AFK

- **Medido:** si dejas que tu turno se agote sin hacer ninguna acción, ese turno **y** el siguiente turno del rival no dan PE.
- Si no tienes nada que jugar, pulsa **"LISTO"**. Un turno sin ninguna acción sí cuenta si lo terminas tú.
- Si juegas al menos una carta, atacas o usas tu poder de héroe, el turno cuenta aunque se agote el tiempo.
- **Medido:** cuando el **rival** se queda AFK, no pierdes nada. Sus turnos inactivos cuentan por completo para ti.

<a id="mind-the-60-second-minimum"></a>
## Cuidado con el mínimo de 60 segundos

- **Medido:** las partidas con menos de 60 segundos de tiempo de PE **no dan PE**, ni siquiera unos pocos. Esto incluye las partidas en las que el rival se rinde pronto.
- Si te rindes justo después del inicio, la partida no da nada.

<a id="conceding"></a>
## Rendirse

- **Medido:** rendirte no cuesta nada extra. Recibes la tasa de derrota por el tiempo jugado.
- **Medido:** una partida perdida sigue dando la tasa de derrota mientras dura, así que seguir jugando no es tiempo perdido.
- **Medido:** cada partida nueva cuesta tiempo sin PE: alrededor de 1:07 desde el final de una partida hasta el primer turno de la siguiente (media de 10 mediciones), más unos 35 segundos de mano inicial. Rendirse compensa cuando es probable que la siguiente partida sea mejor, por ejemplo cuando prefieres jugar una partida que puedes ganar.

<a id="game-length"></a>
## Duración de la partida

- **Medido:** los PE por segundo son constantes. Jugar más despacio no aumenta los PE por hora de tiempo de juego; solo hace que cada partida valga más.
- Con partidas más largas, pasas una parte menor de tu tiempo en la cola y en la mano inicial. Aun así, por favor, no alargues la partida a propósito. Es de mala educación con tu rival.
- **Según [2]:** el tiempo de PE tiene un límite de unos 30 minutos por partida. El tiempo de más no da nada.

<a id="quests-give-most-of-the-xp"></a>
## Las misiones dan la mayor parte de los PE

- **Según [3]:** "Outside of the very hardcore player, majority of XP comes from quests." (Salvo para los jugadores más hardcore, la mayor parte de los PE procede de las misiones.)
- **Según [4]:** las misiones diarias dan 900, 1000 o 1500 PE. Las misiones semanales dan 1750 PE, y la misión semanal garantizada de Con rango ("Win 5 games of Hearthstone's Ranked Mode", es decir, gana 5 partidas en el modo Con rango) da 2500 PE.
- **Según [5]:** puedes cambiar una misión diaria y una misión semanal cada día. El progreso de una misión cambiada se pierde.
- Una misión diaria vale aproximadamente de dos a cuatro horas de partidas en Con rango. Elige mazos y modos que completen tus misiones mientras juegas.

<a id="tavern-pass"></a>
## Pase de taberna

- **Según [6]:** el Pase de taberna da al momento una bonificación de PE del 10 %, del 15 % en el nivel 20 del mapa y del 20 % en el nivel 70, durante el resto de la expansión.
- **Según [7]:** la bonificación se aplica a los PE de cualquier origen.
- Sin medir: si la bonificación multiplica exactamente las tasas de las partidas. Todas las mediciones se han hecho sin ella.

<a id="track-size"></a>
## Tamaño del mapa

- **Según [6]:** los niveles 1 a 100 del Mapa de recompensas necesitan 155 200 PE en total. Los niveles de bonificación llegan hasta el nivel 400 y dan 50 de oro cada uno.
- **Según [7]:** el mapa se reinicia con cada nueva expansión.

<a id="sources"></a>
## Fuentes

1. Chadd Nervig (desarrollador de Hearthstone), tuit, 11 de noviembre de 2020: "Battlegrounds is around ~300xp/hr." <https://twitter.com/Celestalon/status/1326626763766550528>
2. August Dean Ayala (director del juego Hearthstone), tuit, 11 de noviembre de 2020: "There is an XP cap on each game around 30 min." <https://twitter.com/IksarHS/status/1326615786878767105>
3. August Dean Ayala, tuit, 11 de noviembre de 2020. <https://twitter.com/IksarHS/status/1326613836531597314>
4. Hearthstone Wiki, *Quest*. <https://hearthstone.wiki.gg/wiki/Quest>
5. Hearthstone Wiki, *Daily quest*. <https://hearthstone.wiki.gg/wiki/Daily_quest>
6. Blizzard, *Reign of the Black Empire Rewards Track Refresh*, 2026. <https://news.blizzard.com/en-us/article/24309582/>
7. Hearthstone Wiki, *Rewards Track*. <https://hearthstone.wiki.gg/wiki/Rewards_Track>

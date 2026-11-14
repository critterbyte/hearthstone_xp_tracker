# Cómo conseguir más EXP

> Traducción asistida por máquina. Si difiere de la [versión en inglés](../FARMING.md), rige la versión en inglés. Las correcciones son bienvenidas: [abre un issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

Consejos limpios para conseguir más EXP de la Senda de recompensas por hora. Se derivan de la [fórmula de EXP](../FORMULA.md). Las afirmaciones se marcan igual que allá:

- **Medido:** lo descubrió este proyecto.
- **Según [n]:** de las fuentes al final de esta página.

El panel del plugin muestra a dónde se va tu EXP. La pestaña "EXP perdida" es un buen punto de partida (consulta [USAGE.md](USAGE.md#tabs)).

<a id="play-fair"></a>
## Juega limpio

Esta página trata de jugar de forma normal y eficiente. Los bots, el win trading, compartir la cuenta y otras formas de saltarse las reglas del juego violan los términos de Blizzard y pueden hacer que te baneen la cuenta. Aquí no se tratan.

<a id="choose-the-mode"></a>
## Elige el modo

| Modo | Victoria | Derrota |
|---|---:|---:|
| Clasificado (Estándar y Salvaje) | 470 EXP/h | 336 EXP/h |
| Casual y Gresca de la taberna | 336 EXP/h | 240 EXP/h |

- **Medido:** Clasificado paga 1.40 veces lo que pagan Casual y Gresca de la taberna. Una victoria en Casual paga lo mismo que una derrota en Clasificado.
- **Medido:** Estándar y Salvaje pagan lo mismo. Juega el formato que más disfrutes o en el que ganes más.
- **Medido:** una victoria paga 1.40 veces lo que una derrota. Tu porcentaje de victorias importa, así que juega un mazo con el que ganes.
- **Según [1]:** Campos de batalla da unos 300 de EXP por hora. Este proyecto no lo ha medido.

<a id="do-not-go-afk"></a>
## No te quedes AFK

- **Medido:** si dejas que tu turno se acabe sin hacer ninguna acción, ese turno **y** el siguiente turno del oponente no dan EXP.
- Si no tienes nada que jugar, haz clic en **"LISTO"**. Un turno sin ninguna acción sí cuenta si tú lo terminas.
- Si juegas al menos una carta, atacas o usas tu Poder de héroe, el turno cuenta aunque se acabe el tiempo.
- **Medido:** cuando el **oponente** se queda AFK, tú no pierdes nada. Sus turnos inactivos cuentan completos para ti.

<a id="mind-the-60-second-minimum"></a>
## Ojo con el mínimo de 60 segundos

- **Medido:** las partidas con menos de 60 segundos de tiempo de EXP **no dan EXP**, ni siquiera un poco. Esto incluye las partidas en las que el oponente se rinde pronto.
- Si te rindes justo después del inicio, la partida no da nada.

<a id="conceding"></a>
## Rendirse

- **Medido:** rendirte no cuesta nada extra. Recibes la tasa de derrota por el tiempo jugado.
- **Medido:** una partida perdida sigue pagando la tasa de derrota mientras dura, así que seguir jugando no es tiempo perdido.
- **Medido:** cada partida nueva cuesta tiempo sin EXP: alrededor de 1:07 desde el final de una partida hasta el primer turno de la siguiente (promedio de 10 mediciones), más unos 35 segundos de mano inicial. Rendirse vale la pena cuando es probable que la siguiente partida sea mejor, por ejemplo cuando prefieres jugar una partida que puedes ganar.

<a id="game-length"></a>
## Duración de la partida

- **Medido:** la EXP por segundo es constante. Jugar más lento no sube la EXP por hora de tiempo de juego; solo hace que cada partida valga más.
- Con partidas más largas, pasas una parte menor de tu tiempo en la cola y en la mano inicial. Aun así, por favor no alargues la partida a propósito. Es de mala educación con tu oponente.
- **Según [2]:** el tiempo de EXP tiene un tope de unos 30 minutos por partida. El tiempo de más no da nada.

<a id="quests-give-most-of-the-xp"></a>
## Las misiones dan la mayor parte de la EXP

- **Según [3]:** "Outside of the very hardcore player, majority of XP comes from quests." (Salvo para los jugadores más hardcore, la mayor parte de la EXP viene de las misiones.)
- **Según [4]:** las misiones diarias dan 900, 1,000 o 1,500 de EXP. Las misiones semanales dan 1,750 de EXP, y la misión semanal garantizada de Clasificado ("Win 5 games of Hearthstone's Ranked Mode", o sea, gana 5 partidas en el modo Clasificado) da 2,500 de EXP.
- **Según [5]:** puedes cambiar una misión diaria y una misión semanal cada día. El progreso de una misión cambiada se pierde.
- Una misión diaria vale entre dos y cuatro horas de partidas en Clasificado, aproximadamente. Elige mazos y modos que completen tus misiones mientras juegas.

<a id="tavern-pass"></a>
## Pase de la taberna

- **Según [6]:** el Pase de la taberna da de inmediato una bonificación de EXP del 10 %, del 15 % en el nivel 20 de la senda y del 20 % en el nivel 70, durante el resto de la expansión.
- **Según [7]:** la bonificación se aplica a la EXP de cualquier origen.
- Sin medir: si la bonificación multiplica exactamente las tasas de las partidas. Todas las mediciones se hicieron sin ella.

<a id="track-size"></a>
## Tamaño de la senda

- **Según [6]:** los niveles 1 a 100 de la Senda de recompensas necesitan 155,200 de EXP en total. Los niveles de bonificación llegan hasta el nivel 400 y dan 50 de oro cada uno.
- **Según [7]:** la senda se reinicia con cada nueva expansión.

<a id="sources"></a>
## Fuentes

1. Chadd Nervig (desarrollador de Hearthstone), tuit, 11 de noviembre de 2020: "Battlegrounds is around ~300xp/hr." <https://twitter.com/Celestalon/status/1326626763766550528>
2. August Dean Ayala (director del juego Hearthstone), tuit, 11 de noviembre de 2020: "There is an XP cap on each game around 30 min." <https://twitter.com/IksarHS/status/1326615786878767105>
3. August Dean Ayala, tuit, 11 de noviembre de 2020. <https://twitter.com/IksarHS/status/1326613836531597314>
4. Hearthstone Wiki, *Quest*. <https://hearthstone.wiki.gg/wiki/Quest>
5. Hearthstone Wiki, *Daily quest*. <https://hearthstone.wiki.gg/wiki/Daily_quest>
6. Blizzard, *Reign of the Black Empire Rewards Track Refresh*, 2026. <https://news.blizzard.com/en-us/article/24309582/>
7. Hearthstone Wiki, *Rewards Track*. <https://hearthstone.wiki.gg/wiki/Rewards_Track>

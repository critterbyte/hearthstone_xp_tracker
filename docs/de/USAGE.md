# Bedienung

> Maschinell unterstützte Übersetzung. Weicht sie von der [englischen Fassung](../USAGE.md) ab, gilt die englische Fassung. Korrekturen sind willkommen: [eröffne ein Issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

Das Plugin besteht aus drei Teilen:

- dem **Overlay** im Spiel
- dem **Dashboard**-Fenster
- der Datei mit dem **Partieverlauf**

Die Texte in Anführungszeichen, etwa „EP-Zeit:“, sind die Texte, die das Plugin anzeigt.

<a id="overlay"></a>
## Overlay

Das Overlay ist ein kleiner Kasten unten links über deinem Spielerbild („You“ mit deinem Rang). Im Dashboard kannst du es stattdessen über den Einstellungs-Button von Hearthstone (das Zahnrad unten rechts) setzen. Links stehen die Beschriftungen, rechts die Werte in blauen Balken.

Das Overlay erscheint nur in **Gewertet, Ungewertet, Kartenchaos und Arena**. Andere Modi werden noch nicht gemessen, dort zeigt das Plugin nichts an. Ein Kartenchaos gegen den Computer (PvE, zum Beispiel gegen den Gastwirt) gibt keine EP, darum zeigt das Overlay dort „+0“.

<a id="during-a-game"></a>
### Während einer Partie

Die Zeilen erscheinen mit dem ersten Zug. Während des Mulligans ist das Overlay ausgeblendet.

| Zeile | Bedeutung |
|---|---|
| „EP-Zeit:“ | Die bisher für EP zählende Zeit, in Minuten und Sekunden |
| „Bei Sieg:“ | Die EP, die du bekommst, wenn du jetzt gewinnst |
| „Bei Niederlage:“ | Die EP, die du bekommst, wenn du jetzt verlierst oder aufgibst |

Die EP-Zeit kann einen Zusatz haben:

| Zusatz | Bedeutung |
|---|---|
| „(pausiert)“ | Du hast deinen Zug ohne jede Aktion ablaufen lassen. Dieser Zug und der nächste Zug des Gegners zählen nicht, darum ist die EP-Zeit zurückgesprungen und wartet. In deinem nächsten Zug läuft sie wieder. |
| „(max.)“ | Die EP-Zeit hat 30 Minuten erreicht. Mehr Zeit bringt keine weiteren EP. |

Schalte während eines Hearthstone-Events im Dashboard „Event läuft (Event-EP anzeigen)“ ein. Das Overlay zeigt dann eine weitere Zeile, live und in der Zusammenfassung nach der Partie:

| Zeile | Bedeutung |
|---|---|
| „Event-EP:“ | Die bisherigen Event-EP, mit einem roten Kristall: eine pro Minute EP-Zeit (siehe [FORMULA.md](../FORMULA.md#event-xp)). Sie hängen nicht vom Ergebnis ab. |

Schalte es aus, wenn das Event endet.

Ist im Dashboard „Gasthauspass (EP-Bonus)“ eingeschaltet, enthalten „Bei Sieg:“, „Bei Niederlage:“ und „Erhalten:“ den Bonus des Passes für deine Stufe im Belohnungsweg. Event-EP bekommen keinen Bonus.

Die letzten beiden Zeilen zeigen den Schaden, live und in der Zusammenfassung nach der Partie. Mit „Schaden im Overlay anzeigen“ im Dashboard schaltest du sie aus.

| Zeile | Bedeutung |
|---|---|
| „Gesamtschaden:“ | Der gesamte Schaden, den deine Seite bisher dem Helden und den Dienern des Gegners zugefügt hat: deine Angriffe, die Angriffe deiner Diener, Zauber, Waffen, Heldenfähigkeit und andere Effekte. Jeder Treffer zählt mit dem Wert, den das Spiel anzeigt, auch wenn er höher ist als das Leben des Ziels oder auf Rüstung trifft. Schaden an deinem eigenen Helden oder deinen Dienern zählt nicht, ebenso wenig Schaden ohne Quelle, etwa Erschöpfung. Ein Treffer auf ein Gottesschild zählt 0. |
| „Heldenschaden:“ | Der Teil von „Gesamtschaden:“, der den gegnerischen Helden getroffen hat, einschließlich des Teils, den die Rüstung abgefangen hat |

Der Schaden wird gelesen, sobald der Server eine Aktion bestätigt. Er kann also kurz vor der Animation auf dem Spielfeld steigen.

Solange die EP-Zeit unter 60 Sekunden liegt, zeigen beide EP-Werte „+0“. Partien unter einer Minute geben keine EP. Die Regeln stehen in [FORMULA.md](../FORMULA.md).

Startest du HDT mitten in einer Partie, liest das Plugin die Partie trotzdem von Anfang an.

<a id="after-a-game"></a>
### Nach einer Partie

Nach der Partie zeigt das Overlay bis zum Mulligan deiner nächsten Partie eine Zusammenfassung:

| Zeile | Bedeutung |
|---|---|
| „Letzte Partie:“ | Sieg oder Niederlage |
| „EP-Zeit:“ | Die Zeit, die für EP gezählt hat |
| „Gesamtzeit:“ | Die ganze Partie einschließlich Mulligan |
| „Erhalten:“ | Die EP, die die Partie gebracht hat |

<a id="update-hint"></a>
### Update-Hinweis

Gibt es eine neuere Version des Plugins, zeigt das Overlay eine weitere Zeile, solange keine Partie läuft:

| Zeile | Bedeutung |
|---|---|
| „Update verfügbar:“ | Die Nummer der neuen Version, zum Beispiel 0.2.0 |

Das Dashboard zeigt einen Link zur Downloadseite. Wie du aktualisierst, steht in [INSTALL.md](INSTALL.md#update).

<a id="dashboard"></a>
## Dashboard

Das Dashboard-Fenster heißt „HS XP Tracker“. Du öffnest es in HDT auf einem von zwei Wegen:

- Menü *Plugins* → „EP-Dashboard“
- *Settings* (das Zahnrad rechts in der oberen Leiste) *→ Tracker → Plugins* → der Button „EP-Dashboard“ des Plugins

<a id="toolbar"></a>
### Werkzeugleiste

| Element | Bedeutung |
|---|---|
| „Sitzung“ | Partien der aktuellen Hearthstone-Sitzung, vom Start des Spiels bis zum Schließen. Nach dem Schließen von Hearthstone bleibt die letzte Sitzung bis zum nächsten Start sichtbar. Diesen Zeitraum zeigt das Dashboard beim Öffnen. |
| „Heute“ | Partien seit Mitternacht |
| „Diese Woche“ | Partien seit Montag, 00:00 Uhr |
| „Diese Saison“ | Partien seit dem 1. des Monats. Gewertete Saisons dauern in Hearthstone einen Kalendermonat. |
| „Letzte 30 Tage“ | Partien der letzten 30 Tage, einschließlich heute |
| „Gesamt“ | Alle Partien im Verlauf |
| „Aktualisieren“ | Den Verlauf neu einlesen |
| „Datenordner öffnen“ | Den Ordner mit der Verlaufsdatei im Windows Explorer öffnen |
| „Sprache (Language)“ | Sprache des Overlays, des Dashboards und des Menüeintrags des Plugins. Standard ist Englisch. Die Änderung gilt sofort; das Dashboard öffnet sich in der neuen Sprache neu. In anderen Sprachen behält die Beschriftung das englische Wort, zum Beispiel „Sprache (Language)“. |
| „Overlay-Position“ | Wo das Overlay sitzt: „Links über deinem Spielerbild“ (Standard, so bleibt die HDT-Deckliste rechts frei) oder „Rechts über dem Einstellungs-Knopf“. Das Overlay wechselt sofort. |
| „Event läuft (Event-EP anzeigen)“ | Während eines Hearthstone-Events einschalten: Das Overlay zeigt die Event-EP, und neue Partien speichern sie. |
| „Gasthauspass (EP-Bonus)“ | Einschalten, wenn du den Gasthauspass der aktuellen Erweiterung besitzt. Der Bonus wird berechnet, nicht gemessen: Mit dem Pass können die angezeigten EP pro Partie um 1 EP abweichen (siehe [FORMULA.md](../FORMULA.md#tavern-pass)). |
| „Stufe“ mit „−“, einer Zahl und „+“ | Deine Stufe im Belohnungsweg. Klicke auf „−“ oder „+“ oder tippe die Zahl ein. Die Zeile darunter zeigt den Bonus und die nächste Schwelle, zum Beispiel „10 % EP-Bonus, noch 2 Stufen bis 15 %“. Der Bonus beträgt 10 %, ab Stufe 20 15 % und ab Stufe 70 20 % (siehe [FORMULA.md](../FORMULA.md#tavern-pass)). Passe die Stufe an, wenn du Stufe 20 oder 70 erreichst. |
| „Schaden im Overlay anzeigen“ | Die Zeilen „Gesamtschaden:“ und „Heldenschaden:“ ein- oder ausblenden. Standardmäßig an. |
| „Beim Start von HDT nach Updates suchen“ | Die Update-Prüfung ein- oder ausschalten. Siehe [Datenschutz](#privacy). |

Eine beendete Partie erscheint in einem offenen Dashboard von selbst. Du musst nicht auf „Aktualisieren“ klicken.

Bei „Sitzung“ nennt eine Zeile unter der Werkzeugleiste die Sitzung, zum Beispiel „Sitzung seit Di 2026-11-10, 19:56 (läuft)“ oder „(beendet)“. Eine Sitzung kann über Mitternacht hinaus laufen.

<a id="tabs"></a>
### Tabs

**Übersicht**

| Kachel | Bedeutung |
|---|---|
| „EP“ | Im Zeitraum erhaltene EP |
| „EP-Zeit“ | Zeit, die für EP gezählt hat |
| „Spielzeit inkl. Mulligan“ | Zeit in Partien, einschließlich Mulligan |
| „Online-Zeit“ | Zeit, in der Hearthstone lief. Siehe unten. |
| „Partien“ | Anzahl der Partien, mit Siegen und Niederlagen |
| „EP pro Stunde“ | EP pro Stunde EP-Zeit |
| „EP pro Online-Stunde“ | EP pro Stunde Online-Zeit, also inklusive Menüs und Warteschlange |
| „Siegquote“ | Anteil der gewonnenen Partien |
| „Event-EP“ | Event-EP des Zeitraums; nur sichtbar, wenn Partien während eines Events gespielt wurden |

Die Online-Zeit wird pro Hearthstone-Sitzung gezählt: vom Start von Hearthstone bis zum Ende der letzten Partie. Solange Hearthstone läuft, zählt sie bis jetzt. Es zählen nur Sitzungen mit mindestens einer Partie im Zeitraum.

**Decks**

Eine Zeile pro Deck: Partien, Siegquote, EP, EP pro Stunde und die durchschnittliche Partiedauer. Das Deck mit den meisten EP pro Stunde steht oben.

Die Decknamen stammen aus der Statistik von HDT. Eine Partie, die HDT nicht erfasst hat, zeigt „Kein Deck erfasst“.

**Verlorene EP**

| Kachel | Bedeutung |
|---|---|
| „AFK-Züge“ | EP, die dir durch AFK-Züge entgangen sind: Züge ohne jede Aktion, die ohne „Fertig“ abgelaufen sind, und die Gegnerzüge danach |
| „Partien unter 1 Minute“ | EP, die die Formel für Partien unter 60 Sekunden ergeben würde, die das Spiel aber nicht auszahlt |
| „Zeit außerhalb von Partien“ | Online-Zeit außerhalb von Partien (Menüs, Warteschlange, Pausen), bewertet mit deinen EP pro Stunde im Zeitraum |

**Partien**

Alle Partien des Zeitraums, die neueste zuerst: Datum, Start, Modus, Ergebnis, EP-Zeit, Gesamtzeit, EP, Event-EP, AFK-Züge, Deck und Klasse des Gegners. Klicke auf einen Spaltenkopf, um zu sortieren.

**Diagramme**

Zwei Kurven auf derselben Zeitachse. Die Achse deckt immer den ganzen Zeitraum ab, damit auch Stunden oder Tage ohne Partien sichtbar bleiben:

| Zeitraum | Zeitachse |
|---|---|
| „Sitzung“ | jede Stunde vom Start der Sitzung bis zu ihrem Ende, oder bis jetzt, solange sie läuft |
| „Heute“ | die 24 Stunden des Tages |
| „Diese Woche“ | Montag bis Sonntag |
| „Diese Saison“ | jeder Tag des Monats |
| „Letzte 30 Tage“ | die letzten 30 Tage, einschließlich heute |
| „Gesamt“ | jeder Tag seit der ersten Partie im Verlauf |

| Kurve | Bedeutung |
|---|---|
| „Erhaltene EP“ (gold) | EP der Partien, die in der jeweiligen Stunde oder am jeweiligen Tag begonnen haben. Schritte ohne Partien stehen bei 0. |
| „EP pro Stunde“ (blau) | Tempo in jeder Stunde oder an jedem Tag: EP geteilt durch EP-Zeit. Nur Schritte mit Partien haben einen Punkt. Die gestrichelte Linie ist der Durchschnitt des ganzen Zeitraums. |

Eine Partie zählt in der Stunde oder an dem Tag, in der oder an dem sie begonnen hat.

<a id="game-history-file"></a>
## Datei mit dem Partieverlauf

Jede beendete Partie wird an eine CSV-Datei angehängt:

```
%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv
```

Die Datei trennt die Felder mit Kommas und nutzt einen Punkt als Dezimalzeichen. Zeiten sind Ortszeit.

| Spalte | Inhalt |
|---|---|
| `Session` | Name des Log-Ordners von Hearthstone für die Sitzung, zum Beispiel `Hearthstone_2026_11_08_09_10_51` |
| `Start` | Beginn des ersten Zugs, `yyyy-MM-ddTHH:mm:ss.fff` |
| `End` | Ende der Partie, gleiches Format |
| `Mode` | Spielmodus, zum Beispiel `Ranked` |
| `Format` | Format, zum Beispiel Standard oder Wild; leer, wenn der Modus keines hat |
| `Result` | Zum Beispiel `Win` oder `Loss` |
| `XpSeconds` | Zeit, die für EP gezählt hat |
| `PlaySeconds` | Zeit vom ersten Zug bis zum Ende, AFK-Züge eingeschlossen |
| `WithMulliganSeconds` | Zeit einschließlich Mulligan |
| `AfkTurns` | Anzahl der eigenen AFK-Züge |
| `Xp` | Vom Plugin berechnete EP; leer bei Modi ohne gemessene Raten |
| `Deck` | Deckname aus HDT |
| `PlayerClass` | Deine Klasse |
| `OpponentClass` | Die Klasse des Gegners |
| `EventXp` | Vom Plugin berechnete Event-EP; leer bei Partien außerhalb von Events. Dateien älterer Versionen ohne diese Spalte werden weiterhin gelesen, und die Spalte kommt mit der nächsten Partie hinzu. |
| `XpBonus` | EP-Bonus des Gasthauspasses in Prozent, der beim Spielen der Partie galt, zum Beispiel `15`; `0` ohne Pass. Ältere Dateien ohne diese Spalte werden als `0` gelesen. |

Die Spalte `Xp` berechnet das Plugin aus der EP-Zeit. Das Spiel selbst protokolliert keine EP. Beim Öffnen des Dashboards berechnet das Plugin die EP aller gespeicherten Partien mit den aktuellen Raten neu, eine korrigierte Rate berichtigt also auch ältere Partien. Jede Partie behält ihren eigenen `XpBonus`, spätere Änderungen am Pass oder an der Stufe ändern ältere Partien also nicht.

Eine Partie, die zweimal aus demselben Log gelesen wird, wird nur einmal gespeichert. Bei der Installation einer neuen Plugin-Version bleibt die Datei erhalten.

<a id="privacy"></a>
## Datenschutz

- Der Partieverlauf und die Einstellungen bleiben auf deinem Computer. Das Plugin lädt nichts hoch.
- Die einzige Netzwerkanfrage ist die Update-Prüfung. Beim Start von HDT fragt das Plugin einmal bei GitHub nach, ob es eine neuere Version gibt. Die Anfrage sendet nur den Namen und die Version des Plugins, zum Beispiel `HearthstoneXpTracker/0.1.0`. Wie bei jeder Webanfrage sieht GitHub deine IP-Adresse.
- Um die Update-Prüfung auszuschalten, entferne im Dashboard den Haken bei „Beim Start von HDT nach Updates suchen“. Die Einstellung wird in `%APPDATA%\HearthstoneDeckTracker\XpTracker\settings.ini` gespeichert.

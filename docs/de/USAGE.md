# Bedienung

> Maschinell unterstützte Übersetzung. Weicht sie von der [englischen Fassung](../USAGE.md) ab, gilt die englische Fassung. Korrekturen sind willkommen: [eröffne ein Issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

Der Tracker läuft im Hintergrund, mit einem goldenen „XP“-Symbol im Infobereich (*notification area*) unten rechts in der Taskleiste. Er besteht aus drei Teilen:

- dem **Overlay** im Spiel
- dem **Dashboard**-Fenster
- der Datei mit dem **Partieverlauf**

Die Texte in Anführungszeichen, etwa „EP-Zeit:“, sind die Texte, die der Tracker anzeigt.

<a id="overlay"></a>
## Overlay

Das Overlay ist ein kleiner Kasten unten rechts über dem Einstellungs-Button von Hearthstone (dem Zahnrad unten rechts). Im Dashboard kannst du es stattdessen unten links über dein Spielerbild („You“ mit deinem Rang) setzen. Links stehen die Beschriftungen, rechts die Werte in blauen Balken.

Das Overlay erscheint nur, solange Hearthstone das aktive Fenster ist. Es übernimmt nie den Fokus, und jeder Klick geht durch es hindurch an das Spiel.

Das Overlay erscheint nur in **Gewertet, Ungewertet, Kartenchaos und Arena**. Andere Modi werden noch nicht gemessen, dort zeigt der Tracker nichts an. Ein Kartenchaos gegen den Computer (PvE, zum Beispiel gegen den Gastwirt) gibt keine EP, darum zeigt das Overlay dort „+0“.

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

Startest du den Tracker mitten in einer Partie, liest er die Partie trotzdem von Anfang an.

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

Gibt es eine neuere Version des Trackers, zeigt das Overlay eine weitere Zeile, solange keine Partie läuft:

| Zeile | Bedeutung |
|---|---|
| „Update verfügbar:“ | Die Nummer der neuen Version, zum Beispiel 0.2.0 |

Das Dashboard zeigt einen Link zur Downloadseite. Wie du aktualisierst, steht in [INSTALL.md](INSTALL.md#update).

<a id="dashboard"></a>
## Dashboard

Das Dashboard-Fenster heißt „HS XP Tracker“. Es öffnet sich, wenn der Tracker startet. So öffnest du es erneut:

- klicke auf das goldene „XP“-Symbol im Infobereich, oder
- klicke mit der rechten Maustaste auf das Symbol und wähle „EP-Dashboard“, oder
- starte `HearthstoneXpTracker.exe` erneut.

Wenn du das Dashboard schließt, laufen der Tracker und das Overlay weiter. Um den Tracker zu beenden, klicke mit der rechten Maustaste auf das Symbol und wähle „Beenden“.

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
| „Sprache (Language)“ | Sprache des Overlays, des Dashboards und des Menüs am Symbol. Standard ist Englisch. Die Änderung gilt sofort; das Dashboard öffnet sich in der neuen Sprache neu. In anderen Sprachen behält die Beschriftung das englische Wort, zum Beispiel „Sprache (Language)“. |
| „Overlay-Position“ | Wo das Overlay sitzt: „Rechts über dem Einstellungs-Knopf“ (Standard) oder „Links über deinem Spielerbild“, so bleibt die rechte Seite frei, zum Beispiel für die Deckliste von Hearthstone Deck Tracker. Das Overlay wechselt sofort. |
| „Event läuft (Event-EP anzeigen)“ | Während eines Hearthstone-Events einschalten: Das Overlay zeigt die Event-EP, und neue Partien speichern sie. Der Tracker kann nicht erkennen, wann ein Event läuft, darum ist dies standardmäßig aus; solange es aus ist, erinnert dich ein kurzer Hinweis darunter daran. |
| „Gasthauspass (EP-Bonus)“ | Einschalten, wenn du den Gasthauspass der aktuellen Erweiterung besitzt. Der Bonus wird berechnet, nicht gemessen: Mit dem Pass können die angezeigten EP pro Partie um 1 EP abweichen (siehe [FORMULA.md](../FORMULA.md#tavern-pass)). |
| „Stufe“ mit „−“, einer Zahl und „+“ | Deine Stufe im Belohnungsweg. Klicke auf „−“ oder „+“ oder tippe die Zahl ein. Die Zeile darunter zeigt den Bonus und die nächste Schwelle, zum Beispiel „10 % EP-Bonus, noch 2 Stufen bis 15 %“. Der Bonus beträgt 10 %, ab Stufe 20 15 % und ab Stufe 70 20 % (siehe [FORMULA.md](../FORMULA.md#tavern-pass)). Passe die Stufe an, wenn du Stufe 20 oder 70 erreichst. |
| „Schaden im Overlay anzeigen“ | Die Zeilen „Gesamtschaden:“ und „Heldenschaden:“ ein- oder ausblenden. Standardmäßig an. |
| „Beim Start des Trackers nach Updates suchen“ | Die Update-Prüfung ein- oder ausschalten. Siehe [Datenschutz](#privacy). |
| „Mit Windows starten“ | Den Tracker beim Anmelden an Windows starten. Er startet dann nur im Infobereich; ein Klick auf das Symbol öffnet das Dashboard. Standardmäßig aus. Ausschalten lässt es sich auch im Task-Manager unter *Autostart-Apps* (*Startup apps*). |
| „Verknüpfung im Startmenü“ | „Hearthstone XP Tracker“ zum Startmenü hinzufügen oder daraus entfernen. Beim ersten Start fragt eine Leiste oben im Dashboard einmal danach. |

Eine beendete Partie erscheint in einem offenen Dashboard von selbst. Du musst nicht auf „Aktualisieren“ klicken.

Bei „Sitzung“ nennt eine Zeile unter der Werkzeugleiste die Sitzung, zum Beispiel „Sitzung seit Di 2026-11-10, 19:56 (läuft)“ oder „(beendet)“. Eine Sitzung kann über Mitternacht hinaus laufen.

Darunter zeigt eine Statuszeile, ob Hearthstone das Spiel-Log schreibt, das der Tracker liest: „Spiel-Log (Power.log): an“. Musste der Tracker das Log einschalten, während Hearthstone lief, bittet die Zeile in Rot, Hearthstone einmal neu zu starten. Konnte er das Log nicht einschalten, sagt sie das in Rot und nennt die Datei (siehe [INSTALL.md](INSTALL.md#troubleshooting)).

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

Partien in Modi ohne gemessene Raten, zum Beispiel Schlachtfeld, werden ohne EP gespeichert. Sie stehen unter „Partien“ und zählen bei Spielzeit und Online-Zeit mit, aber nicht in den übrigen Kacheln oder den Diagrammen.

**Verlorene EP**

| Kachel | Bedeutung |
|---|---|
| „AFK-Züge“ | EP, die dir durch AFK-Züge entgangen sind: Züge ohne jede Aktion, die ohne „Fertig“ abgelaufen sind, und die Gegnerzüge danach |
| „Partien unter 1 Minute“ | EP, die die Formel für Partien unter 60 Sekunden ergeben würde, die das Spiel aber nicht auszahlt |
| „Zeit außerhalb von Partien“ | Online-Zeit außerhalb von Partien (Menüs, Warteschlange, Pausen), bewertet mit deinen EP pro Stunde im Zeitraum |

**Partien**

Alle Partien des Zeitraums, die neueste zuerst: Datum, Start, Modus, Ergebnis, EP-Zeit, Gesamtzeit, EP, Event-EP, AFK-Züge und Klasse des Gegners. Klicke auf einen Spaltenkopf, um zu sortieren.

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
%APPDATA%\HearthstoneXpTracker\hs_games.csv
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
| `Xp` | Vom Tracker berechnete EP; leer bei Modi ohne gemessene Raten |
| `Deck` | Deckname, nur bei Partien, die das frühere HDT-Plugin erfasst hat; leer bei neueren Partien |
| `PlayerClass` | Deine Klasse |
| `OpponentClass` | Die Klasse des Gegners |
| `EventXp` | Vom Tracker berechnete Event-EP; leer bei Partien außerhalb von Events. Dateien älterer Versionen ohne diese Spalte werden weiterhin gelesen, und die Spalte kommt mit der nächsten Partie hinzu. |
| `XpBonus` | EP-Bonus des Gasthauspasses in Prozent, der beim Spielen der Partie galt, zum Beispiel `15`; `0` ohne Pass. Ältere Dateien ohne diese Spalte werden als `0` gelesen. |

Die Spalte `Xp` berechnet der Tracker aus der EP-Zeit. Das Spiel selbst protokolliert keine EP. Beim Öffnen des Dashboards berechnet der Tracker die EP aller gespeicherten Partien mit den aktuellen Raten neu, eine korrigierte Rate berichtigt also auch ältere Partien. Jede Partie behält ihren eigenen `XpBonus`, spätere Änderungen am Pass oder an der Stufe ändern ältere Partien also nicht.

Eine Partie, die zweimal aus demselben Log gelesen wird, wird nur einmal gespeichert. Bei der Installation einer neuen Version bleibt die Datei erhalten. Bei jedem Start übernimmt der Tracker außerdem die Partien aus dem Verlauf des früheren HDT-Plugins, die er noch nicht hat (siehe [INSTALL.md](INSTALL.md#coming-from-the-hdt-plugin)).

<a id="privacy"></a>
## Datenschutz

- Der Partieverlauf und die Einstellungen bleiben auf deinem Computer. Der Tracker lädt nichts hoch.
- Die einzige Netzwerkanfrage ist die Update-Prüfung. Beim Start des Trackers fragt er einmal bei GitHub nach, ob es eine neuere Version gibt. Die Anfrage sendet nur den Namen und die Version des Programms, zum Beispiel `HearthstoneXpTracker/0.2.0`. Wie bei jeder Webanfrage sieht GitHub deine IP-Adresse.
- Um die Update-Prüfung auszuschalten, entferne im Dashboard den Haken bei „Beim Start des Trackers nach Updates suchen“. Die Einstellung wird in `%APPDATA%\HearthstoneXpTracker\settings.ini` gespeichert.
- Der Tracker liest nur das Log von Hearthstone. Er liest nicht den Speicher des Spiels und ändert keine Spieldateien. Er schreibt nur in seinen eigenen Datenordner und einmalig den Abschnitt `[Power]` in `log.config` (siehe [INSTALL.md](INSTALL.md#requirements)).

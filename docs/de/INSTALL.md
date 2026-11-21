# Installation

> Maschinell unterstützte Übersetzung. Weicht sie von der [englischen Fassung](../INSTALL.md) ab, gilt die englische Fassung. Korrekturen sind willkommen: [eröffne ein Issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Voraussetzungen

- Windows 10 oder 11. Der Tracker braucht .NET Framework 4.7.2, das in beiden enthalten ist.
- Hearthstone

Hearthstone Deck Tracker (HDT) wird nicht gebraucht. Der Tracker läuft eigenständig oder neben HDT.

Der Tracker liest das `Power.log` von Hearthstone. Hearthstone schreibt dieses Log nur, wenn es in seiner eigenen Einstellungsdatei `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config` eingeschaltet ist. Der Tracker schaltet es beim ersten Start ein: Er fügt den Abschnitt `[Power]` hinzu und ändert sonst nichts an der Datei. Vor dieser Änderung legt er eine Kopie der alten Datei als `log.config.bak` an. Ist HDT installiert, ist das Log bereits eingeschaltet.

<a id="install"></a>
## Installieren

1. Lade die ZIP-Datei der neuesten Version von der [Releases-Seite](https://github.com/critterbyte/hearthstone_xp_tracker/releases) herunter, zum Beispiel `HearthstoneXpTracker-0.2.0.zip`.
2. Klicke mit der rechten Maustaste auf die ZIP-Datei, wähle *Eigenschaften* (*Properties*) und setze den Haken bei *Zulassen* (*Unblock*), falls er angezeigt wird.
3. Entpacke die ZIP-Datei in einen Ordner deiner Wahl, zum Beispiel *Dokumente* (*Documents*). Dabei entsteht der Ordner `HearthstoneXpTracker` mit diesen Dateien:
   - `HearthstoneXpTracker.exe`: der Tracker
   - `HearthstoneXpTracker.exe.config`, `XpTracker.Core.dll`, `LiveCharts.dll`, `LiveCharts.Wpf.dll` und `Newtonsoft.Json.dll`: werden vom Tracker benötigt
   - `INSTALL.txt`, `LICENSE.txt` und `THIRD-PARTY-NOTICES.txt`

   Wähle einen Ordner, in den du schreiben darfst. In *Programme* (*Program Files*) braucht jedes Update Administratorrechte.
4. Starte `HearthstoneXpTracker.exe`. Zeigt Windows „Der Computer wurde durch Windows geschützt“ (*Windows protected your PC*), wähle *Weitere Informationen* (*More info*) und dann *Trotzdem ausführen* (*Run anyway*). Der Tracker ist noch nicht signiert, darum kennt Windows ihn nicht.
5. Das Dashboard öffnet sich, und im Infobereich (*notification area*) unten rechts in der Taskleiste erscheint ein goldenes „XP“-Symbol. Es kann hinter dem kleinen Pfeil (^) versteckt sein.

Lief Hearthstone beim ersten Start schon, starte es einmal neu. Hearthstone liest `log.config` nur beim Start.

Das Overlay erscheint mit dem ersten Zug deiner nächsten Partie. Was es zeigt, steht in [USAGE.md](USAGE.md).

Alle Dateien müssen zusammen in einem Ordner bleiben. Beim ersten Start fragt eine Leiste oben im Dashboard einmal, ob der Tracker ins Startmenü soll. Damit der Tracker beim Anmelden an Windows automatisch startet, setze im Dashboard den Haken bei „Mit Windows starten“. Wenn du den Ordner später verschiebst, starte den Tracker einmal am neuen Ort; dann passt er die Verknüpfung im Startmenü und den Autostart-Eintrag an.

Zu jeder Version ist die SHA-256-Prüfsumme der ZIP-Datei angegeben. Um deinen Download zu prüfen, führe `Get-FileHash HearthstoneXpTracker-0.2.0.zip` in PowerShell aus und vergleiche das Ergebnis.

<a id="coming-from-the-hdt-plugin"></a>
## Umstieg vom HDT-Plugin

Bis Version 0.1.2 war der Tracker ein Plugin für HDT. HDT beendet die Unterstützung für Plugins am 30. November 2026, darum ist der Tracker jetzt ein eigenes Programm mit demselben Overlay und Dashboard.

- **Deine Daten kommen mit.** Bei jedem Start übernimmt der Tracker die Partien aus dem Verlauf des Plugins (`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`), die er noch nicht hat. Außerdem übernimmt er einmalig die Einstellungen des Plugins, wenn er noch keine eigenen hat. Die Dateien des Plugins werden nur gelesen, nie verändert.
- **Deaktiviere das Plugin.** Öffne in HDT *Settings* (das Zahnrad rechts in der oberen Leiste) *→ Tracker → Plugins* und deaktiviere **Hearthstone XP Tracker**. Sonst zeigen zwei Overlays dieselben Zahlen. Du kannst auch den Ordner `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` löschen.
- **Vorerst keine Decknamen.** Die Decknamen kamen von HDT, darum fallen der Tab *Decks* und die Deck-Spalte vorerst weg. Ältere Partien behalten ihre Decknamen in der Verlaufsdatei.

<a id="update"></a>
## Update

Wenn es eine neuere Version gibt, sagt dir der Tracker Bescheid (siehe [USAGE.md](USAGE.md#update-hint)). So aktualisierst du:

1. Lade die ZIP-Datei der neuen Version herunter und gib sie frei (siehe oben).
2. Beende den Tracker: Klicke mit der rechten Maustaste auf das goldene „XP“-Symbol und wähle „Beenden“.
3. Entpacke die ZIP-Datei erneut in denselben Ordner und ersetze die alten Dateien.
4. Starte `HearthstoneXpTracker.exe`.

Dein Partieverlauf und deine Einstellungen bleiben erhalten. Sie liegen in einem anderen Ordner (siehe unten).

Um zu einer älteren Version zurückzukehren, gehe mit deren ZIP-Datei genauso vor.

<a id="uninstall"></a>
## Deinstallieren

1. Entferne im Dashboard die Haken bei „Mit Windows starten“ und „Verknüpfung im Startmenü“.
2. Beende den Tracker: Klicke mit der rechten Maustaste auf das goldene „XP“-Symbol und wähle „Beenden“.
3. Lösche den Ordner `HearthstoneXpTracker`, den du entpackt hast.

Der Tracker speichert deine Daten in `%APPDATA%\HearthstoneXpTracker`:

| Datei | Inhalt |
|---|---|
| `hs_games.csv` | Partieverlauf |
| `settings.ini` | Einstellungen |
| `xptracker.log` | Unerwartete Fehler, falls welche auftreten |

Lösche auch diesen Ordner, wenn du die Daten nicht behalten willst.

Der Abschnitt `[Power]` bleibt in `log.config`, weil auch andere Tools wie HDT ihn nutzen. Er sorgt nur dafür, dass Hearthstone sein Log schreibt. Um die Datei so wiederherzustellen, wie sie vorher war, ersetze `log.config` durch `log.config.bak`.

<a id="troubleshooting"></a>
## Fehlerbehebung

**Windows zeigt „Der Computer wurde durch Windows geschützt“ (*Windows protected your PC*).**

- Wähle *Weitere Informationen* (*More info*) und dann *Trotzdem ausführen* (*Run anyway*). Windows zeigt diese Meldung bei Programmen, die es noch nicht kennt. Der Tracker ist Open Source; du kannst den Code auf GitHub lesen und die SHA-256-Prüfsumme der ZIP-Datei prüfen.

**Beim Start des Trackers passiert nichts.**

- Vielleicht läuft er schon. Ein erneuter Start öffnet das Dashboard des laufenden Trackers. Suche das goldene „XP“-Symbol im Infobereich, auch hinter dem kleinen Pfeil (^).
- Prüfe, ob alle Dateien der ZIP-Datei im selben Ordner liegen und ob die ZIP-Datei freigegeben wurde (siehe [Installieren](#install)).

**Das Overlay erscheint nicht.**

- Das Overlay erscheint nur in Gewertet, Ungewertet, Kartenchaos und Arena. Andere Modi werden noch nicht gemessen.
- Während des Mulligans ist das Overlay ausgeblendet. Es erscheint mit dem ersten Zug.
- Das Overlay erscheint nur, solange Hearthstone das aktive Fenster ist. Klicke einmal ins Spiel.
- Sieh dir im Dashboard die Statuszeile unter den Zeitraum-Buttons an. Dort sollte „Spiel-Log (Power.log): an“ stehen. Bittet sie dich, Hearthstone neu zu starten, dann tu das.

**Das Dashboard meldet, dass das Spiel-Log nicht eingeschaltet werden konnte.**

- Der Tracker konnte `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config` nicht schreiben. Schließe Hearthstone, öffne die Datei im *Editor* (*Notepad*) (lege sie an, falls sie fehlt), füge am Ende diese Zeilen hinzu und speichere sie:

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**Etwas anderes funktioniert nicht.**

Bitte [eröffne ein Issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) und gib die Version des Trackers an (das Dashboard zeigt sie oben rechts), deine Windows-Version und was du erwartet hast. Wenn `%APPDATA%\HearthstoneXpTracker\xptracker.log` existiert, helfen auch seine letzten Zeilen.

<a id="build-from-source"></a>
## Aus dem Quellcode bauen

Siehe [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

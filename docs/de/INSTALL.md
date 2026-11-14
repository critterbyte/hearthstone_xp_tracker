# Installation

> Maschinell unterstützte Übersetzung. Weicht sie von der [englischen Fassung](../INSTALL.md) ab, gilt die englische Fassung. Korrekturen sind willkommen: [eröffne ein Issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Voraussetzungen

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT). Getestet mit HDT 1.60.1.
- Hearthstone

Das Plugin liest das `Power.log` von Hearthstone. HDT schaltet dieses Log selbst ein, du musst also keine Einstellungen in Hearthstone ändern.

<a id="install"></a>
## Installieren

1. Lade die ZIP-Datei der neuesten Version von der [Releases-Seite](https://github.com/critterbyte/hearthstone_xp_tracker/releases) herunter, zum Beispiel `HearthstoneXpTracker-0.1.0.zip`.
2. Schließe HDT.
3. Klicke mit der rechten Maustaste auf die ZIP-Datei, wähle *Eigenschaften* (*Properties*) und setze den Haken bei *Zulassen* (*Unblock*), falls er angezeigt wird. Sonst blockiert Windows das Plugin möglicherweise.
4. Öffne den Plugin-Ordner von HDT. Du findest ihn auf zwei Wegen:
   - In HDT: *Settings* (das Zahnrad rechts in der oberen Leiste) *→ Tracker → Plugins → Plugins Folder*
   - Im Windows Explorer: gib `%APPDATA%\HearthstoneDeckTracker\Plugins` in die Adressleiste ein
5. Entpacke die ZIP-Datei in den Plugin-Ordner. Dabei entsteht der Ordner `HearthstoneXpTracker` mit diesen Dateien:
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` und `LICENSE.txt`
6. Starte HDT.
7. Öffne *Settings → Tracker → Plugins* und aktiviere **Hearthstone XP Tracker**.

Das Overlay erscheint mit dem ersten Zug deiner nächsten Partie. Was es zeigt, steht in [USAGE.md](USAGE.md).

Beide DLL-Dateien müssen im selben Ordner liegen. Ohne `XpTracker.Core.dll` startet das Plugin nicht.

Zu jeder Version ist die SHA-256-Prüfsumme der ZIP-Datei angegeben. Um deinen Download zu prüfen, führe `Get-FileHash HearthstoneXpTracker-0.1.0.zip` in PowerShell aus und vergleiche das Ergebnis.

<a id="update"></a>
## Update

Wenn es eine neuere Version gibt, sagt dir das Plugin Bescheid (siehe [USAGE.md](USAGE.md#update-hint)). So aktualisierst du:

1. Lade die ZIP-Datei der neuen Version herunter und gib sie frei (siehe oben).
2. Schließe HDT.
3. Entpacke die ZIP-Datei erneut in den Plugin-Ordner und ersetze die alten Dateien.
4. Starte HDT.

Dein Partieverlauf und deine Einstellungen bleiben erhalten. Sie liegen in einem anderen Ordner (siehe unten).

<a id="uninstall"></a>
## Deinstallieren

1. Öffne in HDT *Settings → Tracker → Plugins* und deaktiviere das Plugin.
2. Schließe HDT.
3. Lösche den Ordner `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.

Das Plugin speichert deine Daten in `%APPDATA%\HearthstoneDeckTracker\XpTracker`:

| Datei | Inhalt |
|---|---|
| `hs_games.csv` | Partieverlauf |
| `settings.ini` | Einstellungen des Plugins |

Lösche auch diesen Ordner, wenn du die Daten nicht behalten willst.

<a id="troubleshooting"></a>
## Fehlerbehebung

**Das Plugin steht nicht in der Liste, oder HDT zeigt beim Laden einen Fehler.**

- Prüfe, ob beide DLL-Dateien direkt in `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` liegen und nicht eine Ebene tiefer (zum Beispiel `...\HearthstoneXpTracker\HearthstoneXpTracker`).
- Windows kann Dateien aus dem Internet blockieren. Klicke mit der rechten Maustaste auf jede DLL, wähle *Eigenschaften* (*Properties*), setze den Haken bei *Zulassen* (*Unblock*), falls vorhanden, und klicke auf *OK*. Starte HDT danach neu.

**Das Overlay erscheint nicht.**

- Das Overlay erscheint nur in Gewertet, Ungewertet, Kartenchaos und Arena. Andere Modi werden noch nicht gemessen.
- Während des Mulligans ist das Overlay ausgeblendet. Es erscheint mit dem ersten Zug.
- Prüfe, ob das Overlay von HDT selbst funktioniert. Das Plugin zeichnet auf das HDT-Overlay.

**Etwas anderes funktioniert nicht.**

Bitte [eröffne ein Issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) und gib deine HDT-Version an und was du erwartet hast.

<a id="build-from-source"></a>
## Aus dem Quellcode bauen

Siehe [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

# Installazione

> Traduzione assistita da macchina. Se differisce dalla [versione inglese](../INSTALL.md), vale la versione inglese. Le correzioni sono benvenute: [apri una segnalazione](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Requisiti

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT). Testato con HDT 1.60.1.
- Hearthstone

Il plugin legge il file `Power.log` di Hearthstone. HDT attiva questo log da solo, quindi non devi cambiare nessuna impostazione di Hearthstone.

<a id="install"></a>
## Installare

1. Scarica il file ZIP della versione più recente dalla [pagina Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), per esempio `HearthstoneXpTracker-0.1.0.zip`.
2. Chiudi HDT.
3. Fai clic destro sul file ZIP, scegli *Proprietà* (*Properties*) e seleziona *Annulla blocco* (*Unblock*), se compare. Altrimenti Windows potrebbe bloccare il plugin.
4. Apri la cartella dei plugin di HDT. Puoi trovarla in due modi:
   - In HDT: *Settings* (l'icona a ingranaggio a destra nella barra in alto) *→ Tracker → Plugins → Plugins Folder*
   - In Esplora file di Windows: scrivi `%APPDATA%\HearthstoneDeckTracker\Plugins` nella barra degli indirizzi
5. Estrai lo ZIP nella cartella dei plugin. Si crea la cartella `HearthstoneXpTracker` con questi file:
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` e `LICENSE.txt`
6. Avvia HDT.
7. Apri *Settings → Tracker → Plugins* e attiva **Hearthstone XP Tracker**.

L'overlay compare con il primo turno della tua prossima partita. Vedi [USAGE.md](USAGE.md) per sapere cosa mostra.

Entrambi i file DLL devono stare nella stessa cartella. Senza `XpTracker.Core.dll` il plugin non si avvia.

Ogni versione indica il checksum SHA-256 dello ZIP. Per controllare il tuo download, esegui `Get-FileHash HearthstoneXpTracker-0.1.0.zip` in PowerShell e confronta il risultato.

<a id="update"></a>
## Aggiornare

Quando esiste una versione più recente, il plugin te lo dice (vedi [USAGE.md](USAGE.md#update-hint)). Per aggiornare:

1. Scarica il file ZIP della nuova versione e sbloccalo (vedi sopra).
2. Chiudi HDT.
3. Estrai di nuovo lo ZIP nella cartella dei plugin e sostituisci i vecchi file.
4. Avvia HDT.

La cronologia delle partite e le impostazioni restano. Sono salvate in un'altra cartella (vedi sotto).

<a id="uninstall"></a>
## Disinstallare

1. In HDT apri *Settings → Tracker → Plugins* e disattiva il plugin.
2. Chiudi HDT.
3. Elimina la cartella `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.

Il plugin salva i tuoi dati in `%APPDATA%\HearthstoneDeckTracker\XpTracker`:

| File | Contenuto |
|---|---|
| `hs_games.csv` | Cronologia delle partite |
| `settings.ini` | Impostazioni del plugin |

Elimina anche questa cartella se non vuoi tenere i dati.

<a id="troubleshooting"></a>
## Risoluzione dei problemi

**Il plugin non è nell'elenco, oppure HDT mostra un errore quando lo carica.**

- Controlla che entrambi i file DLL siano in `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` e non in una cartella più in basso (per esempio `...\HearthstoneXpTracker\HearthstoneXpTracker`).
- Windows può bloccare i file scaricati da internet. Fai clic destro su ogni DLL, scegli *Proprietà* (*Properties*) e, se c'è la casella *Annulla blocco* (*Unblock*), selezionala e fai clic su *OK*. Poi riavvia HDT.

**L'overlay non compare.**

- L'overlay compare solo in Classificata, Amichevole, Rissa e Arena. Le altre modalità non sono ancora misurate.
- Durante la scelta della mano iniziale l'overlay è nascosto. Compare con il primo turno.
- Controlla che l'overlay di HDT funzioni. Il plugin disegna sull'overlay di HDT.

**C'è un altro problema.**

[Apri una segnalazione](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) e indica la tua versione di HDT e cosa ti aspettavi di vedere.

<a id="build-from-source"></a>
## Compilare dal codice sorgente

Vedi [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

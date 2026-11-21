# Installazione

> Traduzione assistita da macchina. Se differisce dalla [versione inglese](../INSTALL.md), vale la versione inglese. Le correzioni sono benvenute: [apri una segnalazione](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Requisiti

- Windows 10 o 11. Il tracker richiede .NET Framework 4.7.2, incluso in entrambi.
- Hearthstone

Hearthstone Deck Tracker (HDT) non serve. Il tracker funziona da solo oppure insieme a HDT.

Il tracker legge il file `Power.log` di Hearthstone. Hearthstone scrive questo log solo se è attivato nel suo file di impostazioni, `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. Il tracker lo attiva al primo avvio: aggiunge la sezione `[Power]` e non cambia nient'altro nel file. Prima di questa modifica salva una copia del vecchio file come `log.config.bak`. Se HDT è installato, il log è già attivo.

<a id="install"></a>
## Installare

1. Scarica il file ZIP della versione più recente dalla [pagina Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), per esempio `HearthstoneXpTracker-0.2.0.zip`.
2. Fai clic destro sul file ZIP, scegli *Proprietà* (*Properties*) e seleziona *Annulla blocco* (*Unblock*), se compare.
3. Estrai lo ZIP in una cartella a tua scelta, per esempio *Documenti*. Si crea la cartella `HearthstoneXpTracker` con questi file:
   - `HearthstoneXpTracker.exe`: il tracker
   - `HearthstoneXpTracker.exe.config`, `XpTracker.Core.dll`, `LiveCharts.dll`, `LiveCharts.Wpf.dll` e `Newtonsoft.Json.dll`: servono al tracker
   - `INSTALL.txt`, `LICENSE.txt` e `THIRD-PARTY-NOTICES.txt`

   Scegli una cartella in cui puoi scrivere. In *Program Files* ogni aggiornamento richiede i diritti di amministratore.
4. Avvia `HearthstoneXpTracker.exe`. Se Windows mostra "PC protetto da Windows" (*Windows protected your PC*), scegli *Ulteriori informazioni* (*More info*) e poi *Esegui comunque* (*Run anyway*). Il tracker non è ancora firmato, quindi Windows non lo conosce.
5. Si apre il pannello e nell'area di notifica (*notification area*), in basso a destra nella barra delle applicazioni, compare un'icona dorata "XP". Può essere nascosta dietro la piccola freccia (^).

Se Hearthstone era già in esecuzione al primo avvio, riavvialo una volta. Hearthstone legge `log.config` solo quando si avvia.

L'overlay compare con il primo turno della tua prossima partita. Vedi [USAGE.md](USAGE.md) per sapere cosa mostra.

Tutti i file devono restare insieme in una cartella. Al primo avvio, una barra in alto nel pannello chiede una sola volta se aggiungere il tracker al menu Start. Per avviare il tracker automaticamente quando accedi a Windows, spunta "Avvia con Windows" nel pannello. Se in seguito sposti la cartella, avvia il tracker una volta dalla nuova posizione: aggiornerà il collegamento nel menu Start e la voce di avvio automatico.

Ogni versione indica il checksum SHA-256 dello ZIP. Per controllare il tuo download, esegui `Get-FileHash HearthstoneXpTracker-0.2.0.zip` in PowerShell e confronta il risultato.

<a id="coming-from-the-hdt-plugin"></a>
## Se usavi il plugin per HDT

Fino alla versione 0.1.2 il tracker era un plugin per HDT. HDT smette di supportare i plugin il 30 novembre 2026, quindi ora il tracker è un programma a sé, con lo stesso overlay e lo stesso pannello.

- **I tuoi dati vengono con te.** A ogni avvio il tracker aggiunge le partite della cronologia del plugin (`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`) che non ha ancora. Riprende anche una volta le impostazioni del plugin, se non ne ha ancora di proprie. I file del plugin vengono solo letti, mai modificati.
- **Disattiva il plugin.** In HDT apri *Settings* (l'icona a ingranaggio a destra nella barra in alto) *→ Tracker → Plugins* e disattiva **Hearthstone XP Tracker**. Altrimenti due overlay mostrano gli stessi numeri. Puoi anche eliminare la cartella `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.
- **Per ora niente nomi dei mazzi.** I nomi dei mazzi li forniva HDT, quindi per ora la scheda *Mazzi* e la colonna del mazzo non ci sono più. Le partite più vecchie mantengono il nome del mazzo nel file della cronologia.

<a id="update"></a>
## Aggiornare

Quando esiste una versione più recente, il tracker te lo dice (vedi [USAGE.md](USAGE.md#update-hint)). Per aggiornare:

1. Scarica il file ZIP della nuova versione e sbloccalo (vedi sopra).
2. Chiudi il tracker: fai clic destro sull'icona dorata "XP" e scegli "Esci".
3. Estrai di nuovo lo ZIP nella stessa cartella e sostituisci i vecchi file.
4. Avvia `HearthstoneXpTracker.exe`.

La cronologia delle partite e le impostazioni restano. Sono salvate in un'altra cartella (vedi sotto).

Per tornare a una versione precedente, fai lo stesso con il suo ZIP.

<a id="uninstall"></a>
## Disinstallare

1. Nel pannello, togli la spunta da "Avvia con Windows" e "Collegamento nel menu Start".
2. Chiudi il tracker: fai clic destro sull'icona dorata "XP" e scegli "Esci".
3. Elimina la cartella `HearthstoneXpTracker` che hai estratto.

Il tracker salva i tuoi dati in `%APPDATA%\HearthstoneXpTracker`:

| File | Contenuto |
|---|---|
| `hs_games.csv` | Cronologia delle partite |
| `settings.ini` | Impostazioni |
| `xptracker.log` | Errori imprevisti, se ce ne sono |

Elimina anche questa cartella se non vuoi tenere i dati.

La sezione `[Power]` resta in `log.config`, perché la usano anche altri strumenti come HDT. Serve solo a far scrivere il log a Hearthstone. Per ripristinare il file com'era prima, sostituisci `log.config` con `log.config.bak`.

<a id="troubleshooting"></a>
## Risoluzione dei problemi

**Windows mostra "PC protetto da Windows" (*Windows protected your PC*).**

- Scegli *Ulteriori informazioni* (*More info*) e poi *Esegui comunque* (*Run anyway*). Windows mostra questo messaggio per i programmi che non conosce ancora. Il tracker è open source: puoi leggere il codice su GitHub e controllare il checksum SHA-256 dello ZIP.

**Quando avvio il tracker non succede nulla.**

- Forse è già in esecuzione. Avviarlo di nuovo apre il pannello del tracker già in esecuzione. Cerca l'icona dorata "XP" nell'area di notifica (*notification area*), anche dietro la piccola freccia (^).
- Controlla che tutti i file dello ZIP siano nella stessa cartella e che lo ZIP sia stato sbloccato (vedi [Installare](#install)).

**L'overlay non compare.**

- L'overlay compare solo in Classificata, Amichevole, Rissa e Arena. Le altre modalità non sono ancora misurate.
- Durante la scelta della mano iniziale l'overlay è nascosto. Compare con il primo turno.
- L'overlay compare solo mentre Hearthstone è la finestra attiva. Fai clic una volta nel gioco.
- Guarda la riga di stato nel pannello, sotto i pulsanti dei periodi. Deve dire "Registro di gioco (Power.log): attivo". Se ti chiede di riavviare Hearthstone, fallo.

**Il pannello dice che non è stato possibile attivare il registro di gioco.**

- Il tracker non è riuscito a scrivere `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. Chiudi Hearthstone, apri il file con Blocco note (*Notepad*) (crealo se manca), aggiungi queste righe alla fine e salvalo:

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**C'è un altro problema.**

[Apri una segnalazione](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) e indica la versione del tracker (il pannello la mostra nell'angolo in alto a destra), la tua versione di Windows e cosa ti aspettavi di vedere. Se esiste `%APPDATA%\HearthstoneXpTracker\xptracker.log`, anche le sue ultime righe sono utili.

<a id="build-from-source"></a>
## Compilare dal codice sorgente

Vedi [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

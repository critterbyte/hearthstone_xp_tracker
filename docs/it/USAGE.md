# Uso

> Traduzione assistita da macchina. Se differisce dalla [versione inglese](../USAGE.md), vale la versione inglese. Le correzioni sono benvenute: [apri una segnalazione](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

Il tracker funziona in background, con un'icona dorata "XP" nell'area di notifica (*notification area*), in basso a destra nella barra delle applicazioni. Ha tre parti:

- l'**overlay** nel gioco
- la finestra del **pannello**
- il file della **cronologia delle partite**

I testi tra virgolette, come "Tempo PE:", sono i testi che mostra il tracker.

<a id="overlay"></a>
## Overlay

L'overlay è un piccolo riquadro in basso a destra, sopra il pulsante delle impostazioni di Hearthstone (l'ingranaggio nell'angolo in basso a destra). Nel pannello puoi spostarlo in basso a sinistra, sopra il tuo ritratto ("You" con il tuo grado). Le etichette sono a sinistra. I valori sono a destra, in barre blu.

L'overlay compare solo mentre Hearthstone è la finestra attiva. Non prende mai il focus, e ogni clic lo attraversa e arriva al gioco.

L'overlay compare solo in **Classificata, Amichevole, Rissa e Arena**. Le altre modalità non sono ancora misurate, quindi lì il tracker non mostra nulla. Una Rissa contro il computer (PvE, per esempio contro il Locandiere) non dà PE, quindi lì l'overlay mostra "+0".

<a id="during-a-game"></a>
### Durante una partita

Le righe compaiono con il primo turno. Durante la scelta della mano iniziale l'overlay è nascosto.

| Riga | Significato |
|---|---|
| "Tempo PE:" | Il tempo che finora conta per i PE, in minuti e secondi |
| "Se vinci:" | I PE che ottieni se vinci adesso |
| "Se perdi:" | I PE che ottieni se perdi o abbandoni adesso |

Il tempo PE può avere una nota:

| Nota | Significato |
|---|---|
| "(in pausa)" | Hai lasciato scadere il tuo turno senza fare nessuna azione. Quel turno e il turno successivo dell'avversario non contano, quindi il tempo PE è tornato indietro e aspetta. Riparte nel tuo turno successivo. |
| "(max)" | Il tempo PE ha raggiunto 30 minuti. Altro tempo non dà altri PE. |

Durante un evento di Hearthstone, attiva "Evento in corso (mostra i PE Evento)" nel pannello. L'overlay mostra allora una riga in più, dal vivo e nel riepilogo dopo la partita:

| Riga | Significato |
|---|---|
| "PE Evento:" | I PE Evento finora, con un cristallo rosso: uno per ogni minuto di tempo PE (vedi [FORMULA.md](../FORMULA.md#event-xp)). Non dipendono dal risultato. |

Disattivalo quando l'evento finisce.

Con "Pass Locanda (bonus PE)" attivo nel pannello, "Se vinci:", "Se perdi:" e "Ottenuti:" includono il bonus del pass per il tuo livello nel Percorso Ricompense. I PE Evento non ricevono bonus.

Le ultime due righe mostrano i danni, dal vivo e nel riepilogo dopo la partita. Puoi disattivarle con "Mostra i danni nell'overlay" nel pannello.

| Riga | Significato |
|---|---|
| "Danni totali:" | Tutti i danni che la tua parte ha inflitto finora all'eroe e ai servitori dell'avversario: i tuoi attacchi, gli attacchi dei tuoi servitori, magie, armi, Potere Eroe e altri effetti. Ogni colpo conta con il valore che mostra il gioco, anche quando supera la salute del bersaglio o colpisce l'Armatura. I danni al tuo eroe o ai tuoi servitori non contano, e nemmeno i danni senza una fonte, come l'Fatica. Un colpo su uno Scudo Divino vale 0. |
| "Danni all'eroe:" | La parte di "Danni totali:" che ha colpito l'eroe avversario, compresa la parte assorbita dall'Armatura |

I danni vengono letti quando il server conferma un'azione, quindi possono salire un attimo prima dell'animazione sul tabellone.

Finché il tempo PE è sotto 60 secondi, entrambi i valori PE mostrano "+0". Le partite sotto un minuto non danno PE. Vedi [FORMULA.md](../FORMULA.md) per le regole.

Se avvii il tracker a metà partita, legge comunque la partita dall'inizio.

<a id="after-a-game"></a>
### Dopo una partita

Dopo la partita, l'overlay mostra un riepilogo fino alla mano iniziale della tua partita successiva:

| Riga | Significato |
|---|---|
| "Ultima partita:" | "Vittoria" o "Sconfitta" |
| "Tempo PE:" | Il tempo che ha contato per i PE |
| "Tempo totale:" | L'intera partita, mano iniziale compresa |
| "Ottenuti:" | I PE che ha dato la partita |

<a id="update-hint"></a>
### Avviso di aggiornamento

Quando esiste una versione più recente del tracker, l'overlay mostra una riga in più mentre non c'è una partita in corso:

| Riga | Significato |
|---|---|
| "Aggiornamento disponibile:" | Il numero della nuova versione, per esempio 0.2.0 |

Il pannello mostra un link alla pagina di download. Vedi [INSTALL.md](INSTALL.md#update) per sapere come aggiornare.

<a id="dashboard"></a>
## Pannello

La finestra del pannello si chiama "HS XP Tracker". Si apre all'avvio del tracker. Per riaprirla:

- fai clic sull'icona dorata "XP" nell'area di notifica (*notification area*), oppure
- fai clic destro sull'icona e scegli "Pannello PE", oppure
- avvia di nuovo `HearthstoneXpTracker.exe`.

Chiudere il pannello lascia in esecuzione il tracker e l'overlay. Per chiudere il tracker, fai clic destro sull'icona e scegli "Esci".

<a id="toolbar"></a>
### Barra degli strumenti

| Controllo | Significato |
|---|---|
| "Sessione" | Partite della sessione di Hearthstone in corso, dall'avvio del gioco fino alla sua chiusura. Dopo aver chiuso Hearthstone, l'ultima sessione resta visibile fino all'avvio successivo. È il periodo mostrato quando si apre il pannello. |
| "Oggi" | Partite dalla mezzanotte |
| "Questa settimana" | Partite da lunedì, 00:00 |
| "Questa stagione" | Partite dal primo del mese. In Hearthstone le stagioni della Classificata durano un mese di calendario. |
| "Ultimi 30 giorni" | Partite degli ultimi 30 giorni, oggi compreso |
| "Sempre" | Tutte le partite della cronologia |
| "Aggiorna" | Rilegge la cronologia |
| "Apri la cartella dei dati" | Apre in Esplora file di Windows la cartella con il file della cronologia |
| "Lingua (Language)" | Lingua dell'overlay, del pannello e del menu dell'icona. L'inglese è la lingua predefinita. La modifica vale subito; il pannello si riapre nella nuova lingua. Nelle lingue diverse dall'inglese l'etichetta mantiene la parola inglese, come qui "Lingua (Language)". |
| "Posizione dell'overlay" | Dove si trova l'overlay: "A destra, sopra il pulsante delle opzioni" (predefinito) oppure "A sinistra, sopra il tuo ritratto", così il lato destro resta libero, per esempio per l'elenco del mazzo di Hearthstone Deck Tracker. L'overlay si sposta subito. |
| "Evento in corso (mostra i PE Evento)" | Attivalo durante un evento di Hearthstone: l'overlay mostra i PE Evento e le nuove partite li salvano. Il tracker non può sapere quando è in corso un evento, quindi l'opzione è disattivata per impostazione predefinita; finché è disattivata, sotto compare un breve promemoria. |
| "Pass Locanda (bonus PE)" | Attivalo se possiedi il Pass Locanda dell'espansione attuale. Il bonus è calcolato, non misurato: con il pass, i PE mostrati possono sbagliare di 1 PE per partita (vedi [FORMULA.md](../FORMULA.md#tavern-pass)). |
| "Livello" con "−", un numero e "+" | Il tuo livello nel Percorso Ricompense. Fai clic su "−" o "+", oppure scrivi il numero. La riga sotto mostra il bonus e il passo successivo, per esempio "Bonus PE del 10%, ancora 2 livelli per il 15%". Il bonus è del 10%, dal livello 20 del 15% e dal livello 70 del 20% (vedi [FORMULA.md](../FORMULA.md#tavern-pass)). Aggiorna il livello quando superi il livello 20 o 70. |
| "Mostra i danni nell'overlay" | Mostra o nasconde le righe "Danni totali:" e "Danni all'eroe:". Attivo di default. |
| "Cerca aggiornamenti all'avvio del tracker" | Attiva o disattiva il controllo degli aggiornamenti. Vedi [Privacy](#privacy). |
| "Avvia con Windows" | Avvia il tracker quando accedi a Windows. In questo caso si avvia solo nell'area di notifica; fai clic sull'icona per aprire il pannello. Disattivato per impostazione predefinita. Puoi disattivarlo anche in Gestione attività, in *App di avvio* (*Startup apps*). |
| "Collegamento nel menu Start" | Aggiunge "Hearthstone XP Tracker" al menu Start o lo rimuove. Al primo avvio, una barra in alto nel pannello lo chiede una sola volta. |

Una partita finita compare da sola in un pannello aperto. Non serve fare clic su "Aggiorna".

Per "Sessione", una riga sotto la barra degli strumenti indica la sessione, per esempio "Sessione dal mar 2026-11-10, 19:56 (in corso)" o "(terminata)". Una sessione può continuare oltre la mezzanotte.

Sotto, una riga di stato indica se Hearthstone scrive il registro di gioco che il tracker legge: "Registro di gioco (Power.log): attivo". Se il tracker ha dovuto attivare il registro mentre Hearthstone era in esecuzione, la riga ti chiede in rosso di riavviare Hearthstone una volta. Se non è riuscito ad attivarlo, lo dice in rosso e indica il file (vedi [INSTALL.md](INSTALL.md#troubleshooting)).

<a id="tabs"></a>
### Schede

**Panoramica**

| Riquadro | Significato |
|---|---|
| "PE" | PE ottenuti nel periodo |
| "Tempo PE" | Tempo che ha contato per i PE |
| "Tempo di gioco con mano iniziale" | Tempo nelle partite, mano iniziale compresa |
| "Tempo online" | Tempo in cui Hearthstone era aperto. Vedi sotto. |
| "Partite" | Numero di partite, con vittorie e sconfitte |
| "PE all'ora" | PE per ora di tempo PE |
| "PE per ora online" | PE per ora di tempo online, quindi compresi menu e coda |
| "Percentuale di vittorie" | Quota di partite vinte |
| "PE Evento" | PE Evento del periodo; compare solo se ci sono partite giocate durante un evento |

Il tempo online si conta per ogni sessione di Hearthstone: dall'avvio di Hearthstone alla fine dell'ultima partita. Mentre Hearthstone è aperto, conta fino ad adesso. Contano solo le sessioni con almeno una partita nel periodo.

Le partite in modalità senza tassi misurati, per esempio Battaglia, vengono salvate senza PE. Compaiono in "Partite" e contano nel tempo di gioco e nel tempo online, ma non negli altri riquadri né nei grafici.

**PE persi**

| Riquadro | Significato |
|---|---|
| "Turni AFK" | PE persi a causa dei turni AFK: turni senza nessuna azione scaduti senza "FINE TURNO", e i turni dell'avversario che li seguono |
| "Partite sotto 1 minuto" | PE che la formula darebbe per le partite sotto 60 secondi, che il gioco non paga |
| "Tempo fuori dalle partite" | Tempo online passato fuori dalle partite (menu, coda, pause), valutato con i tuoi PE all'ora del periodo |

**Partite**

Tutte le partite del periodo, dalla più recente: data, inizio, modalità, risultato, tempo PE, tempo totale, PE, PE Evento, turni AFK e classe dell'avversario. Fai clic sull'intestazione di una colonna per ordinare.

**Grafici**

Due curve sullo stesso asse del tempo. L'asse copre sempre l'intero periodo, quindi restano visibili anche le ore o i giorni senza partite:

| Periodo | Asse del tempo |
|---|---|
| "Sessione" | ogni ora dall'inizio della sessione alla sua fine, o fino ad adesso mentre è in corso |
| "Oggi" | le 24 ore del giorno |
| "Questa settimana" | da lunedì a domenica |
| "Questa stagione" | ogni giorno del mese |
| "Ultimi 30 giorni" | gli ultimi 30 giorni, oggi compreso |
| "Sempre" | ogni giorno dalla prima partita della cronologia |

| Curva | Significato |
|---|---|
| "PE ottenuti" (oro) | PE delle partite iniziate in ogni ora o in ogni giorno. I punti senza partite sono a 0. |
| "PE all'ora" (blu) | Ritmo in ogni ora o in ogni giorno: PE diviso per il tempo PE. Solo i punti con partite hanno un valore. La linea tratteggiata è la media dell'intero periodo. |

Una partita conta nell'ora o nel giorno in cui è iniziata.

<a id="game-history-file"></a>
## File della cronologia delle partite

Ogni partita finita viene aggiunta a un file CSV:

```
%APPDATA%\HearthstoneXpTracker\hs_games.csv
```

Il file usa le virgole tra i campi e il punto come separatore decimale. Gli orari sono in ora locale.

| Colonna | Contenuto |
|---|---|
| `Session` | Nome della cartella dei log di Hearthstone per la sessione, per esempio `Hearthstone_2026_11_08_09_10_51` |
| `Start` | Inizio del primo turno, `yyyy-MM-ddTHH:mm:ss.fff` |
| `End` | Fine della partita, stesso formato |
| `Mode` | Modalità di gioco, per esempio `Ranked` |
| `Format` | Formato, per esempio Standard o Wild; vuoto se la modalità non ne ha uno |
| `Result` | Per esempio `Win` o `Loss` |
| `XpSeconds` | Tempo che ha contato per i PE |
| `PlaySeconds` | Tempo dal primo turno alla fine, turni AFK compresi |
| `WithMulliganSeconds` | Tempo con la mano iniziale compresa |
| `AfkTurns` | Numero dei tuoi turni AFK |
| `Xp` | PE calcolati dal tracker; vuoto per le modalità senza valori misurati |
| `Deck` | Nome del mazzo, solo per le partite registrate dal vecchio plugin per HDT; vuoto per le partite più recenti |
| `PlayerClass` | La tua classe |
| `OpponentClass` | La classe dell'avversario |
| `EventXp` | PE Evento calcolati dal tracker; vuoto per le partite fuori dagli eventi. I file di versioni precedenti senza questa colonna vengono comunque letti, e la colonna viene aggiunta con la partita successiva. |
| `XpBonus` | Bonus PE del Pass Locanda in percentuale, valido quando la partita è stata giocata, per esempio `15`; `0` senza il pass. I file più vecchi senza questa colonna vengono letti come `0`. |

La colonna `Xp` è calcolata dal tracker a partire dal tempo PE. Il gioco stesso non registra i PE nei log. Quando si apre il pannello, il tracker ricalcola i PE di tutte le partite salvate con i valori attuali, quindi un valore corretto sistema anche le partite più vecchie. Ogni partita mantiene il proprio `XpBonus`, quindi cambiare più tardi il pass o il livello non modifica le partite più vecchie.

Una partita letta due volte dallo stesso log viene salvata una sola volta. Installare una nuova versione non cancella il file. A ogni avvio il tracker aggiunge anche le partite della cronologia del vecchio plugin per HDT che non ha ancora (vedi [INSTALL.md](INSTALL.md#coming-from-the-hdt-plugin)).

<a id="privacy"></a>
## Privacy

- La cronologia delle partite e le impostazioni restano sul tuo computer. Il tracker non carica nulla.
- L'unica richiesta di rete è il controllo degli aggiornamenti. Quando il tracker si avvia, chiede una volta a GitHub se esiste una versione più recente. La richiesta invia solo il nome e la versione del programma, per esempio `HearthstoneXpTracker/0.2.0`. Come per ogni richiesta web, GitHub vede il tuo indirizzo IP.
- Per disattivare il controllo degli aggiornamenti, togli la spunta da "Cerca aggiornamenti all'avvio del tracker" nel pannello. L'impostazione è salvata in `%APPDATA%\HearthstoneXpTracker\settings.ini`.
- Il tracker legge solo il log di Hearthstone. Non legge la memoria del gioco e non modifica i file del gioco. Scrive solo nella propria cartella dei dati e, una volta, la sezione `[Power]` di `log.config` (vedi [INSTALL.md](INSTALL.md#requirements)).

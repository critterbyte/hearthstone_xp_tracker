# Utilisation

> Traduction assistée par machine. En cas de différence avec la [version anglaise](../USAGE.md), la version anglaise fait foi. Les corrections sont les bienvenues : [ouvrez une issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

Le tracker fonctionne en arrière-plan, avec une icône dorée « XP » dans la zone de notification (*notification area*), en bas à droite de la barre des tâches. Il comporte trois parties :

- l'**overlay** dans le jeu
- la fenêtre du **tableau de bord**
- le fichier d'**historique des parties**

Les textes entre guillemets, comme « Temps d'EXP : », sont les textes affichés par le tracker.

<a id="overlay"></a>
## Overlay

L'overlay est un petit cadre en bas à droite, au-dessus du bouton des options de Hearthstone (la roue dentée en bas à droite). Dans le tableau de bord, vous pouvez le placer plutôt en bas à gauche, au-dessus de votre portrait (« You » avec votre rang). Les libellés sont à gauche. Les valeurs sont à droite, dans des barres bleues.

L'overlay ne s'affiche que lorsque Hearthstone est la fenêtre active. Il ne prend jamais le focus, et chaque clic le traverse jusqu'au jeu.

L'overlay ne s'affiche qu'en **Classé, Non classé, Bras de fer et Arène**. Les autres modes ne sont pas encore mesurés, le tracker n'y affiche donc rien. Un Bras de fer contre l'ordinateur (JcE, par exemple contre l'aubergiste) ne rapporte pas d'EXP, l'overlay y affiche donc « +0 ».

<a id="during-a-game"></a>
### Pendant une partie

Les lignes apparaissent au premier tour. Pendant le choix de la main de départ, l'overlay est masqué.

| Ligne | Signification |
|---|---|
| « Temps d'EXP : » | Le temps qui compte pour l'EXP jusqu'ici, en minutes et secondes |
| « Si victoire : » | L'EXP que vous obtenez si vous gagnez maintenant |
| « Si défaite : » | L'EXP que vous obtenez si vous perdez ou capitulez maintenant |

Le temps d'EXP peut porter une mention :

| Mention | Signification |
|---|---|
| « (en pause) » | Vous avez laissé votre tour s'écouler sans aucune action. Ce tour et le tour suivant de l'adversaire ne comptent pas, le temps d'EXP a donc reculé et attend. Il repart à votre prochain tour. |
| « (max.) » | Le temps d'EXP a atteint 30 minutes. Plus de temps ne rapporte pas plus d'EXP. |

Pendant un évènement Hearthstone, activez « Évènement en cours (afficher l'EXP d'évènement) » dans le tableau de bord. L'overlay affiche alors une ligne de plus, en direct et dans le récapitulatif après la partie :

| Ligne | Signification |
|---|---|
| « EXP d'évènement : » | L'EXP d'évènement jusqu'ici, avec un cristal rouge : un point par minute de temps d'EXP (voir [FORMULA.md](../FORMULA.md#event-xp)). Elle ne dépend pas du résultat. |

Désactivez l'option quand l'évènement se termine.

Avec « Ticket de la taverne (bonus d'EXP) » activé dans le tableau de bord, « Si victoire : », « Si défaite : » et « Gagnée : » incluent le bonus du ticket pour votre niveau dans le Parcours de récompenses. L'EXP d'évènement ne reçoit pas de bonus.

Les deux dernières lignes affichent les dégâts, en direct et dans le récapitulatif après la partie. Désactivez-les avec « Afficher les dégâts dans l'overlay » dans le tableau de bord.

| Ligne | Signification |
|---|---|
| « Dégâts totaux : » | Tous les dégâts infligés par votre camp au héros et aux serviteurs adverses jusqu'ici : vos attaques, les attaques de vos serviteurs, les sorts, les armes, le pouvoir héroïque et les autres effets. Chaque coup compte pour la valeur affichée par le jeu, même quand elle dépasse la vie de la cible ou touche l'armure. Les dégâts infligés à votre propre héros ou à vos serviteurs ne comptent pas, pas plus que les dégâts sans source, comme la fatigue. Un coup sur un Bouclier divin compte pour 0. |
| « Dégâts au héros : » | La part de « Dégâts totaux : » qui a touché le héros adverse, y compris la part absorbée par l'armure |

Les dégâts sont lus quand le serveur confirme une action, ils peuvent donc augmenter un instant avant l'animation sur le plateau.

Tant que le temps d'EXP est inférieur à 60 secondes, les deux valeurs d'EXP affichent « +0 ». Les parties de moins d'une minute ne rapportent pas d'EXP. Voir [FORMULA.md](../FORMULA.md) pour les règles.

Si vous lancez le tracker au milieu d'une partie, il lit quand même la partie depuis son début.

<a id="after-a-game"></a>
### Après une partie

Après la partie, l'overlay affiche un récapitulatif jusqu'à la main de départ de votre partie suivante :

| Ligne | Signification |
|---|---|
| « Dernière partie : » | Victoire ou Défaite |
| « Temps d'EXP : » | Le temps qui a compté pour l'EXP |
| « Temps total : » | Toute la partie, main de départ comprise |
| « Gagnée : » | L'EXP rapportée par la partie |

<a id="update-hint"></a>
### Avis de mise à jour

Quand une version plus récente du tracker existe, l'overlay affiche une ligne de plus tant qu'aucune partie n'est en cours :

| Ligne | Signification |
|---|---|
| « Mise à jour disponible : » | Le numéro de la nouvelle version, par exemple 0.2.0 |

Le tableau de bord affiche un lien vers la page de téléchargement. Voir [INSTALL.md](INSTALL.md#update) pour savoir comment mettre à jour.

<a id="dashboard"></a>
## Tableau de bord

La fenêtre du tableau de bord s'appelle « HS XP Tracker ». Elle s'ouvre au démarrage du tracker. Pour la rouvrir :

- cliquez sur l'icône dorée « XP » dans la zone de notification (*notification area*), ou
- faites un clic droit sur l'icône et choisissez « Tableau de bord EXP », ou
- relancez `HearthstoneXpTracker.exe`.

Fermer le tableau de bord laisse le tracker et l'overlay en marche. Pour arrêter le tracker, faites un clic droit sur l'icône et choisissez « Quitter ».

<a id="toolbar"></a>
### Barre d'outils

| Élément | Signification |
|---|---|
| « Session » | Parties de la session Hearthstone en cours, du lancement du jeu jusqu'à sa fermeture. Après la fermeture de Hearthstone, la dernière session reste visible jusqu'au prochain lancement. C'est la période affichée à l'ouverture du tableau de bord. |
| « Aujourd'hui » | Parties depuis minuit |
| « Cette semaine » | Parties depuis lundi, 00:00 |
| « Cette saison » | Parties depuis le 1er du mois. Les saisons du mode Classé de Hearthstone durent un mois civil. |
| « 30 derniers jours » | Parties des 30 derniers jours, aujourd'hui compris |
| « Tout » | Toutes les parties de l'historique |
| « Actualiser » | Relit l'historique |
| « Ouvrir le dossier des données » | Ouvre le dossier du fichier d'historique dans l'Explorateur de fichiers Windows |
| « Langue (Language) » | Langue de l'overlay, du tableau de bord et du menu de l'icône. L'anglais est la langue par défaut. Le changement s'applique immédiatement ; le tableau de bord se rouvre dans la nouvelle langue. Dans les langues autres que l'anglais, le libellé garde le mot anglais, comme ici : « Langue (Language) ». |
| « Position de l'overlay » | Emplacement de l'overlay : « À droite, au-dessus du bouton des options » (par défaut) ou « À gauche, au-dessus de votre portrait », pour laisser libre le côté droit, par exemple pour la liste du deck de Hearthstone Deck Tracker. L'overlay se déplace aussitôt. |
| « Évènement en cours (afficher l'EXP d'évènement) » | À activer pendant un évènement Hearthstone : l'overlay affiche l'EXP d'évènement et les nouvelles parties l'enregistrent. Le tracker ne peut pas savoir quand un évènement a lieu, l'option est donc désactivée par défaut ; tant qu'elle l'est, un court rappel s'affiche en dessous. |
| « Ticket de la taverne (bonus d'EXP) » | À activer si vous possédez le Ticket de la taverne de l'extension actuelle. Le bonus est calculé, pas mesuré : avec le ticket, l'EXP affichée peut différer de 1 EXP par partie (voir [FORMULA.md](../FORMULA.md#tavern-pass)). |
| « Niveau » avec « − », un nombre et « + » | Votre niveau dans le Parcours de récompenses. Cliquez sur « − » ou « + », ou saisissez le nombre. La ligne en dessous indique le bonus et le palier suivant, par exemple « 10 % de bonus d'EXP, encore 2 niveaux avant 15 % ». Le bonus est de 10 %, de 15 % à partir du niveau 20 et de 20 % à partir du niveau 70 (voir [FORMULA.md](../FORMULA.md#tavern-pass)). Mettez le niveau à jour quand vous passez le niveau 20 ou 70. |
| « Afficher les dégâts dans l'overlay » | Affiche ou masque les lignes « Dégâts totaux : » et « Dégâts au héros : ». Activé par défaut. |
| « Rechercher des mises à jour au démarrage du tracker » | Active ou désactive la recherche de mises à jour. Voir [Confidentialité](#privacy). |
| « Lancer avec Windows » | Lancer le tracker à l'ouverture de session Windows. Il démarre alors uniquement dans la zone de notification ; cliquez sur l'icône pour ouvrir le tableau de bord. Désactivé par défaut. Vous pouvez aussi le désactiver dans le Gestionnaire des tâches, sous *Applications de démarrage* (*Startup apps*). |
| « Raccourci dans le menu Démarrer » | Ajouter « Hearthstone XP Tracker » au menu Démarrer, ou l'en retirer. Au premier lancement, une barre en haut du tableau de bord le demande une seule fois. |

Une partie terminée apparaît d'elle-même dans un tableau de bord ouvert. Inutile de cliquer sur « Actualiser ».

Pour « Session », une ligne sous la barre d'outils indique la session, par exemple « Session depuis mar. 2026-11-10, 19:56 (en cours) » ou « (terminée) ». Une session peut se poursuivre après minuit.

En dessous, une ligne d'état indique si Hearthstone écrit le journal de partie que lit le tracker : « Journal de partie (Power.log) : activé ». Si le tracker a dû activer le journal pendant que Hearthstone était lancé, la ligne vous demande en rouge de redémarrer Hearthstone une fois. S'il n'a pas pu activer le journal, elle l'indique en rouge et nomme le fichier (voir [INSTALL.md](INSTALL.md#troubleshooting)).

<a id="tabs"></a>
### Onglets

**Vue d'ensemble**

| Tuile | Signification |
|---|---|
| « EXP » | EXP gagnée pendant la période |
| « Temps d'EXP » | Temps qui a compté pour l'EXP |
| « Temps de jeu avec la main de départ » | Temps passé en partie, main de départ comprise |
| « Temps en ligne » | Temps pendant lequel Hearthstone était lancé. Voir plus bas. |
| « Parties » | Nombre de parties, avec victoires et défaites |
| « EXP par heure » | EXP par heure de temps d'EXP |
| « EXP par heure en ligne » | EXP par heure de temps en ligne, donc menus et file d'attente compris |
| « Taux de victoire » | Part des parties gagnées |
| « EXP d'évènement » | EXP d'évènement de la période ; affichée uniquement si des parties ont été jouées pendant un évènement |

Le temps en ligne est compté par session Hearthstone : du lancement de Hearthstone à la fin de la dernière partie. Tant que Hearthstone est lancé, il compte jusqu'à maintenant. Seules les sessions avec au moins une partie dans la période comptent.

Les parties dans des modes sans taux mesurés, par exemple les Champs de bataille, sont enregistrées sans EXP. Elles figurent sous « Parties » et comptent dans le temps de jeu et le temps en ligne, mais pas dans les autres tuiles ni les graphiques.

**EXP perdue**

| Tuile | Signification |
|---|---|
| « Tours AFK » | EXP perdue à cause des tours AFK : les tours sans aucune action qui se sont écoulés sans « Fin du tour », et les tours adverses qui les suivent |
| « Parties de moins d'1 minute » | EXP que la formule donnerait pour les parties de moins de 60 secondes, que le jeu ne paie pas |
| « Temps hors des parties » | Temps en ligne passé hors des parties (menus, file d'attente, pauses), valorisé à votre EXP par heure de la période |

**Parties**

Toutes les parties de la période, de la plus récente à la plus ancienne : date, début, mode, résultat, temps d'EXP, temps total, EXP, EXP d'évènement, tours AFK et classe de l'adversaire. Cliquez sur l'en-tête d'une colonne pour trier.

**Graphiques**

Deux courbes sur le même axe du temps. L'axe couvre toujours toute la période, les heures ou les jours sans partie restent donc visibles :

| Période | Axe du temps |
|---|---|
| « Session » | chaque heure du début de la session à sa fin, ou jusqu'à maintenant tant qu'elle est en cours |
| « Aujourd'hui » | les 24 heures de la journée |
| « Cette semaine » | du lundi au dimanche |
| « Cette saison » | chaque jour du mois |
| « 30 derniers jours » | les 30 derniers jours, aujourd'hui compris |
| « Tout » | chaque jour depuis la première partie de l'historique |

| Courbe | Signification |
|---|---|
| « EXP gagnée » (or) | EXP des parties commencées à chaque heure ou chaque jour. Les intervalles sans partie sont à 0. |
| « EXP par heure » (bleu) | Rythme à chaque heure ou chaque jour : EXP divisée par le temps d'EXP. Seuls les intervalles avec des parties ont un point. La ligne pointillée est la moyenne de toute la période. |

Une partie compte dans l'heure ou le jour où elle a commencé.

<a id="game-history-file"></a>
## Fichier d'historique des parties

Chaque partie terminée est ajoutée à un fichier CSV :

```
%APPDATA%\HearthstoneXpTracker\hs_games.csv
```

Le fichier utilise des virgules entre les champs et un point comme séparateur décimal. Les heures sont en heure locale.

| Colonne | Contenu |
|---|---|
| `Session` | Nom du dossier de journaux de Hearthstone pour la session, par exemple `Hearthstone_2026_11_08_09_10_51` |
| `Start` | Début du premier tour, `yyyy-MM-ddTHH:mm:ss.fff` |
| `End` | Fin de la partie, même format |
| `Mode` | Mode de jeu, par exemple `Ranked` |
| `Format` | Format, par exemple Standard ou Wild ; vide si le mode n'en a pas |
| `Result` | Par exemple `Win` ou `Loss` |
| `XpSeconds` | Temps qui a compté pour l'EXP |
| `PlaySeconds` | Temps du premier tour à la fin, tours AFK compris |
| `WithMulliganSeconds` | Temps main de départ comprise |
| `AfkTurns` | Nombre de vos propres tours AFK |
| `Xp` | EXP calculée par le tracker ; vide pour les modes sans taux mesurés |
| `Deck` | Nom du deck, uniquement pour les parties enregistrées par l'ancien plugin HDT ; vide pour les parties plus récentes |
| `PlayerClass` | Votre classe |
| `OpponentClass` | La classe de l'adversaire |
| `EventXp` | EXP d'évènement calculée par le tracker ; vide pour les parties hors évènement. Les fichiers des anciennes versions sans cette colonne sont toujours lus, et la colonne est ajoutée avec la partie suivante. |
| `XpBonus` | Bonus d'EXP du Ticket de la taverne en pourcentage qui s'appliquait au moment de la partie, par exemple `15` ; `0` sans le ticket. Les anciens fichiers sans cette colonne sont lus comme `0`. |

La colonne `Xp` est calculée par le tracker à partir du temps d'EXP. Le jeu lui-même n'enregistre pas l'EXP dans ses journaux. À l'ouverture du tableau de bord, le tracker recalcule l'EXP de toutes les parties enregistrées avec les taux actuels, si bien qu'un taux corrigé corrige aussi les anciennes parties. Chaque partie garde son propre `XpBonus`, donc changer plus tard le ticket ou le niveau ne modifie pas les anciennes parties.

Une partie lue deux fois depuis le même journal n'est enregistrée qu'une fois. L'installation d'une nouvelle version conserve le fichier. À chaque démarrage, le tracker ajoute aussi les parties de l'historique de l'ancien plugin HDT qu'il n'a pas encore (voir [INSTALL.md](INSTALL.md#coming-from-the-hdt-plugin)).

<a id="privacy"></a>
## Confidentialité

- L'historique des parties et les réglages restent sur votre ordinateur. Le tracker n'envoie rien.
- La seule requête réseau est la recherche de mises à jour. Au démarrage du tracker, il demande une fois à GitHub si une version plus récente existe. La requête n'envoie que le nom et la version du programme, par exemple `HearthstoneXpTracker/0.2.0`. Comme pour toute requête web, GitHub voit votre adresse IP.
- Pour désactiver la recherche de mises à jour, décochez « Rechercher des mises à jour au démarrage du tracker » dans le tableau de bord. Le réglage est enregistré dans `%APPDATA%\HearthstoneXpTracker\settings.ini`.
- Le tracker lit seulement le journal de Hearthstone. Il ne lit pas la mémoire du jeu et ne modifie aucun fichier du jeu. Il écrit uniquement dans son propre dossier de données et, une seule fois, la section `[Power]` de `log.config` (voir [INSTALL.md](INSTALL.md#requirements)).

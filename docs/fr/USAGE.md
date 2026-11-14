# Utilisation

> Traduction assistée par machine. En cas de différence avec la [version anglaise](../USAGE.md), la version anglaise fait foi. Les corrections sont les bienvenues : [ouvrez une issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

Le plugin comporte trois parties :

- l'**overlay** dans le jeu
- la fenêtre du **tableau de bord**
- le fichier d'**historique des parties**

Les textes entre guillemets, comme « Temps d'EXP : », sont les textes affichés par le plugin.

<a id="overlay"></a>
## Overlay

L'overlay est un petit cadre au-dessus du bouton des options de Hearthstone (la roue dentée en bas à droite). Les libellés sont à gauche. Les valeurs sont à droite, dans des barres bleues.

L'overlay ne s'affiche qu'en **Classé, Non classé et Bras de fer**. Les autres modes ne sont pas encore mesurés, le plugin n'y affiche donc rien. Un Bras de fer contre l'ordinateur (JcE, par exemple contre l'aubergiste) ne rapporte pas d'EXP, l'overlay y affiche donc « +0 ».

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

Si vous lancez HDT au milieu d'une partie, le plugin lit quand même la partie depuis son début.

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

Quand une version plus récente du plugin existe, l'overlay affiche une ligne de plus tant qu'aucune partie n'est en cours :

| Ligne | Signification |
|---|---|
| « Mise à jour disponible : » | Le numéro de la nouvelle version, par exemple 0.2.0 |

Le tableau de bord affiche un lien vers la page de téléchargement. Voir [INSTALL.md](INSTALL.md#update) pour savoir comment mettre à jour.

<a id="dashboard"></a>
## Tableau de bord

La fenêtre du tableau de bord s'appelle « HS XP Tracker ». Ouvrez-la dans HDT de l'une de ces deux façons :

- le menu *Plugins* → « Tableau de bord EXP »
- *Settings* (l'icône en forme de roue dentée à droite de la barre du haut) *→ Tracker → Plugins* → le bouton « Tableau de bord EXP » du plugin

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
| « Langue (Language) » | Langue de l'overlay, du tableau de bord et de l'entrée de menu du plugin. L'anglais est la langue par défaut. Le changement s'applique immédiatement ; le tableau de bord se rouvre dans la nouvelle langue. Dans les langues autres que l'anglais, le libellé garde le mot anglais, comme ici : « Langue (Language) ». |
| « Évènement en cours (afficher l'EXP d'évènement) » | À activer pendant un évènement Hearthstone : l'overlay affiche l'EXP d'évènement et les nouvelles parties l'enregistrent. |
| « Ticket de la taverne (bonus d'EXP) » | À activer si vous possédez le Ticket de la taverne de l'extension actuelle. Le bonus est calculé, pas mesuré : avec le ticket, l'EXP affichée peut différer de 1 EXP par partie (voir [FORMULA.md](../FORMULA.md#tavern-pass)). |
| « Niveau » avec « − », un nombre et « + » | Votre niveau dans le Parcours de récompenses. Cliquez sur « − » ou « + », ou saisissez le nombre. La ligne en dessous indique le bonus et le palier suivant, par exemple « 10 % de bonus d'EXP, encore 2 niveaux avant 15 % ». Le bonus est de 10 %, de 15 % à partir du niveau 20 et de 20 % à partir du niveau 70 (voir [FORMULA.md](../FORMULA.md#tavern-pass)). Mettez le niveau à jour quand vous passez le niveau 20 ou 70. |
| « Afficher les dégâts dans l'overlay » | Affiche ou masque les lignes « Dégâts totaux : » et « Dégâts au héros : ». Activé par défaut. |
| « Rechercher des mises à jour au démarrage de HDT » | Active ou désactive la recherche de mises à jour. Voir [Confidentialité](#privacy). |

Une partie terminée apparaît d'elle-même dans un tableau de bord ouvert. Inutile de cliquer sur « Actualiser ».

Pour « Session », une ligne sous la barre d'outils indique la session, par exemple « Session depuis mar. 2026-11-10, 19:56 (en cours) » ou « (terminée) ». Une session peut se poursuivre après minuit.

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

**Decks**

Une ligne par deck : parties, taux de victoire, EXP, EXP par heure et durée moyenne des parties. Le deck avec le plus d'EXP par heure est en haut.

Les noms des decks proviennent des statistiques de HDT. Une partie que HDT n'a pas enregistrée affiche « Aucun deck enregistré ».

**EXP perdue**

| Tuile | Signification |
|---|---|
| « Tours AFK » | EXP perdue à cause des tours AFK : les tours sans aucune action qui se sont écoulés sans « Fin du tour », et les tours adverses qui les suivent |
| « Parties de moins d'1 minute » | EXP que la formule donnerait pour les parties de moins de 60 secondes, que le jeu ne paie pas |
| « Temps hors des parties » | Temps en ligne passé hors des parties (menus, file d'attente, pauses), valorisé à votre EXP par heure de la période |

**Parties**

Toutes les parties de la période, de la plus récente à la plus ancienne : date, début, mode, résultat, temps d'EXP, temps total, EXP, EXP d'évènement, tours AFK, deck et classe de l'adversaire. Cliquez sur l'en-tête d'une colonne pour trier.

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
%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv
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
| `Xp` | EXP calculée par le plugin ; vide pour les modes sans taux mesurés |
| `Deck` | Nom du deck dans HDT |
| `PlayerClass` | Votre classe |
| `OpponentClass` | La classe de l'adversaire |
| `EventXp` | EXP d'évènement calculée par le plugin ; vide pour les parties hors évènement. Les fichiers des anciennes versions sans cette colonne sont toujours lus, et la colonne est ajoutée avec la partie suivante. |
| `XpBonus` | Bonus d'EXP du Ticket de la taverne en pourcentage qui s'appliquait au moment de la partie, par exemple `15` ; `0` sans le ticket. Les anciens fichiers sans cette colonne sont lus comme `0`. |

La colonne `Xp` est calculée par le plugin à partir du temps d'EXP. Le jeu lui-même n'enregistre pas l'EXP dans ses journaux. À l'ouverture du tableau de bord, le plugin recalcule l'EXP de toutes les parties enregistrées avec les taux actuels, si bien qu'un taux corrigé corrige aussi les anciennes parties. Chaque partie garde son propre `XpBonus`, donc changer plus tard le ticket ou le niveau ne modifie pas les anciennes parties.

Une partie lue deux fois depuis le même journal n'est enregistrée qu'une fois. L'installation d'une nouvelle version du plugin conserve le fichier.

<a id="privacy"></a>
## Confidentialité

- L'historique des parties et les réglages restent sur votre ordinateur. Le plugin n'envoie rien.
- La seule requête réseau est la recherche de mises à jour. Au démarrage de HDT, le plugin demande une fois à GitHub si une version plus récente existe. La requête n'envoie que le nom et la version du plugin, par exemple `HearthstoneXpTracker/0.1.0`. Comme pour toute requête web, GitHub voit votre adresse IP.
- Pour désactiver la recherche de mises à jour, décochez « Rechercher des mises à jour au démarrage de HDT » dans le tableau de bord. Le réglage est enregistré dans `%APPDATA%\HearthstoneDeckTracker\XpTracker\settings.ini`.

# Installation

> Traduction assistée par machine. En cas de différence avec la [version anglaise](../INSTALL.md), la version anglaise fait foi. Les corrections sont les bienvenues : [ouvrez une issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Configuration requise

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT). Testé avec HDT 1.60.1.
- Hearthstone

Le plugin lit le fichier `Power.log` de Hearthstone. HDT active ce journal tout seul, vous n'avez donc aucun réglage de Hearthstone à modifier.

<a id="install"></a>
## Installer

1. Téléchargez le fichier ZIP de la dernière version sur la [page des Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), par exemple `HearthstoneXpTracker-0.1.0.zip`.
2. Fermez HDT.
3. Faites un clic droit sur le fichier ZIP, choisissez *Propriétés* et cochez *Débloquer* si cette option apparaît. Sinon, Windows risque de bloquer le plugin.
4. Ouvrez le dossier des plugins de HDT. Vous pouvez le trouver de deux façons :
   - Dans HDT : *Settings* (l'icône en forme de roue dentée à droite de la barre du haut) *→ Tracker → Plugins → Plugins Folder*
   - Dans l'Explorateur de fichiers Windows : tapez `%APPDATA%\HearthstoneDeckTracker\Plugins` dans la barre d'adresse
5. Extrayez le ZIP dans le dossier des plugins. Cela crée le dossier `HearthstoneXpTracker` avec ces fichiers :
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` et `LICENSE.txt`
6. Lancez HDT.
7. Ouvrez *Settings → Tracker → Plugins* et activez **Hearthstone XP Tracker**.

L'overlay apparaît au premier tour de votre prochaine partie. Consultez [USAGE.md](USAGE.md) pour savoir ce qu'il affiche.

Les deux fichiers DLL doivent se trouver dans le même dossier. Sans `XpTracker.Core.dll`, le plugin ne démarre pas.

Chaque version indique la somme de contrôle SHA-256 du ZIP. Pour vérifier votre téléchargement, exécutez `Get-FileHash HearthstoneXpTracker-0.1.0.zip` dans PowerShell et comparez le résultat.

<a id="update"></a>
## Mettre à jour

Quand une version plus récente existe, le plugin vous le signale (voir [USAGE.md](USAGE.md#update-hint)). Pour mettre à jour :

1. Téléchargez le fichier ZIP de la nouvelle version et débloquez-le (voir plus haut).
2. Fermez HDT.
3. Extrayez à nouveau le ZIP dans le dossier des plugins et remplacez les anciens fichiers.
4. Lancez HDT.

Votre historique de parties et vos réglages sont conservés. Ils sont enregistrés dans un autre dossier (voir plus bas).

<a id="uninstall"></a>
## Désinstaller

1. Dans HDT, ouvrez *Settings → Tracker → Plugins* et désactivez le plugin.
2. Fermez HDT.
3. Supprimez le dossier `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.

Le plugin conserve vos données dans `%APPDATA%\HearthstoneDeckTracker\XpTracker` :

| Fichier | Contenu |
|---|---|
| `hs_games.csv` | Historique des parties |
| `settings.ini` | Réglages du plugin |

Supprimez aussi ce dossier si vous ne voulez pas garder les données.

<a id="troubleshooting"></a>
## Dépannage

**Le plugin n'apparaît pas dans la liste, ou HDT affiche une erreur au chargement.**

- Vérifiez que les deux fichiers DLL se trouvent dans `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` et non un dossier plus bas (par exemple `...\HearthstoneXpTracker\HearthstoneXpTracker`).
- Windows peut bloquer les fichiers téléchargés sur Internet. Faites un clic droit sur chaque DLL, choisissez *Propriétés* et, s'il y a une case *Débloquer*, cochez-la et cliquez sur *OK*. Redémarrez ensuite HDT.

**L'overlay n'apparaît pas.**

- L'overlay ne s'affiche qu'en Classé, Non classé, Bras de fer et Arène. Les autres modes ne sont pas encore mesurés.
- Pendant le choix de la main de départ, l'overlay est masqué. Il apparaît au premier tour.
- Vérifiez que l'overlay de HDT lui-même fonctionne. Le plugin dessine sur l'overlay de HDT.

**Après l'installation d'une version plus ancienne, HDT utilise toujours la plus récente.**

- HDT ne charge pas le plugin depuis `%APPDATA%\HearthstoneDeckTracker\Plugins`, mais depuis une copie dans son propre dossier de programme, `%LOCALAPPDATA%\HearthstoneDeckTracker\app-<version de HDT>\Plugins`. Il n'y remplace un fichier que si le fichier dans `%APPDATA%` est plus récent. Les fichiers d'une version plus ancienne sont plus anciens, HDT garde donc la copie plus récente.
- Pour revenir à une version plus ancienne : fermez HDT, installez l'ancienne version comme décrit plus haut, supprimez le dossier `HearthstoneXpTracker` dans `%LOCALAPPDATA%\HearthstoneDeckTracker\app-<version de HDT>\Plugins` et redémarrez HDT. HDT copie alors à nouveau le plugin.
- À partir de la version 0.1.2, le tableau de bord affiche la version en cours en haut à droite.
- Les mises à jour vers une version plus récente ne sont pas concernées.

**Un autre problème.**

Merci d'[ouvrir une issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) en indiquant votre version de HDT et ce que vous vous attendiez à voir.

<a id="build-from-source"></a>
## Compiler depuis les sources

Voir [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

# Installation

> Traduction assistée par machine. En cas de différence avec la [version anglaise](../INSTALL.md), la version anglaise fait foi. Les corrections sont les bienvenues : [ouvrez une issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Configuration requise

- Windows 10 ou 11. Le tracker a besoin de .NET Framework 4.7.2, qui est inclus dans les deux.
- Hearthstone

Hearthstone Deck Tracker (HDT) n'est pas nécessaire. Le tracker fonctionne seul, ou à côté de HDT.

Le tracker lit le fichier `Power.log` de Hearthstone. Hearthstone n'écrit ce journal que s'il est activé dans son propre fichier de réglages, `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. Le tracker l'active à son premier démarrage : il ajoute la section `[Power]` et ne modifie rien d'autre dans le fichier. Avant cette modification, il garde une copie de l'ancien fichier sous le nom `log.config.bak`. Si HDT est installé, le journal est déjà activé.

<a id="install"></a>
## Installer

1. Téléchargez le fichier ZIP de la dernière version sur la [page des Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), par exemple `HearthstoneXpTracker-0.2.0.zip`.
2. Faites un clic droit sur le fichier ZIP, choisissez *Propriétés* (*Properties*) et cochez *Débloquer* (*Unblock*) si cette option apparaît.
3. Extrayez le ZIP dans le dossier de votre choix, par exemple *Documents*. Cela crée le dossier `HearthstoneXpTracker` avec ces fichiers :
   - `HearthstoneXpTracker.exe` : le tracker
   - `HearthstoneXpTracker.exe.config`, `XpTracker.Core.dll`, `LiveCharts.dll`, `LiveCharts.Wpf.dll` et `Newtonsoft.Json.dll` : nécessaires au tracker
   - `INSTALL.txt`, `LICENSE.txt` et `THIRD-PARTY-NOTICES.txt`

   Choisissez un dossier dans lequel vous pouvez écrire. Dans *Program Files*, chaque mise à jour demande des droits d'administrateur.
4. Lancez `HearthstoneXpTracker.exe`. Si Windows affiche « Windows a protégé votre ordinateur » (*Windows protected your PC*), choisissez *Informations complémentaires* (*More info*), puis *Exécuter quand même* (*Run anyway*). Le tracker n'est pas encore signé, Windows ne le connaît donc pas.
5. Le tableau de bord s'ouvre et une icône dorée « XP » apparaît dans la zone de notification (*notification area*), en bas à droite de la barre des tâches. Elle peut être cachée derrière la petite flèche (^).

Si Hearthstone était déjà lancé lors du premier démarrage, redémarrez-le une fois. Hearthstone ne lit `log.config` qu'à son lancement.

L'overlay apparaît au premier tour de votre prochaine partie. Consultez [USAGE.md](USAGE.md) pour savoir ce qu'il affiche.

Tous les fichiers doivent rester ensemble dans un même dossier. Au premier lancement, une barre en haut du tableau de bord demande une seule fois si le tracker doit être ajouté au menu Démarrer. Pour que le tracker se lance automatiquement à l'ouverture de session Windows, cochez « Lancer avec Windows » dans le tableau de bord. Si vous déplacez le dossier plus tard, lancez le tracker une fois depuis son nouvel emplacement ; il met alors à jour le raccourci du menu Démarrer et l'entrée de démarrage automatique.

Chaque version indique la somme de contrôle SHA-256 du ZIP. Pour vérifier votre téléchargement, exécutez `Get-FileHash HearthstoneXpTracker-0.2.0.zip` dans PowerShell et comparez le résultat.

<a id="coming-from-the-hdt-plugin"></a>
## Si vous utilisiez le plugin HDT

Jusqu'à la version 0.1.2, le tracker était un plugin pour HDT. HDT cesse de prendre en charge les plugins le 30 novembre 2026 ; le tracker est donc désormais un programme à part entière, avec le même overlay et le même tableau de bord.

- **Vos données vous suivent.** À chaque démarrage, le tracker ajoute les parties de l'historique du plugin (`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`) qu'il n'a pas encore. Il reprend aussi une fois les réglages du plugin, s'il n'a pas encore les siens. Les fichiers du plugin sont seulement lus, jamais modifiés.
- **Désactivez le plugin.** Dans HDT, ouvrez *Settings* (l'icône en forme de roue dentée à droite de la barre du haut) *→ Tracker → Plugins* et désactivez **Hearthstone XP Tracker**. Sinon, deux overlays affichent les mêmes chiffres. Vous pouvez aussi supprimer le dossier `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.
- **Pas de noms de decks pour l'instant.** C'est HDT qui fournissait les noms des decks, l'onglet *Decks* et la colonne du deck ont donc disparu pour l'instant. Les anciennes parties gardent leur nom de deck dans le fichier d'historique.

<a id="update"></a>
## Mettre à jour

Quand une version plus récente existe, le tracker vous le signale (voir [USAGE.md](USAGE.md#update-hint)). Pour mettre à jour :

1. Téléchargez le fichier ZIP de la nouvelle version et débloquez-le (voir plus haut).
2. Quittez le tracker : faites un clic droit sur l'icône dorée « XP » et choisissez « Quitter ».
3. Extrayez à nouveau le ZIP dans le même dossier et remplacez les anciens fichiers.
4. Lancez `HearthstoneXpTracker.exe`.

Votre historique de parties et vos réglages sont conservés. Ils sont enregistrés dans un autre dossier (voir plus bas).

Pour revenir à une version plus ancienne, procédez de la même façon avec son ZIP.

<a id="uninstall"></a>
## Désinstaller

1. Dans le tableau de bord, décochez « Lancer avec Windows » et « Raccourci dans le menu Démarrer ».
2. Quittez le tracker : faites un clic droit sur l'icône dorée « XP » et choisissez « Quitter ».
3. Supprimez le dossier `HearthstoneXpTracker` que vous avez extrait.

Le tracker conserve vos données dans `%APPDATA%\HearthstoneXpTracker` :

| Fichier | Contenu |
|---|---|
| `hs_games.csv` | Historique des parties |
| `settings.ini` | Réglages |
| `xptracker.log` | Erreurs inattendues, s'il y en a |

Supprimez aussi ce dossier si vous ne voulez pas garder les données.

La section `[Power]` reste dans `log.config`, car d'autres outils comme HDT l'utilisent aussi. Elle sert seulement à ce que Hearthstone écrive son journal. Pour remettre le fichier dans son état d'avant, remplacez `log.config` par `log.config.bak`.

<a id="troubleshooting"></a>
## Dépannage

**Windows affiche « Windows a protégé votre ordinateur » (*Windows protected your PC*).**

- Choisissez *Informations complémentaires* (*More info*), puis *Exécuter quand même* (*Run anyway*). Windows affiche ce message pour les programmes qu'il ne connaît pas encore. Le tracker est open source : vous pouvez lire le code sur GitHub et vérifier la somme de contrôle SHA-256 du ZIP.

**Rien ne se passe quand je lance le tracker.**

- Il est peut-être déjà lancé. Le relancer ouvre le tableau de bord du tracker en cours d'exécution. Cherchez l'icône dorée « XP » dans la zone de notification (*notification area*), y compris derrière la petite flèche (^).
- Vérifiez que tous les fichiers du ZIP se trouvent dans le même dossier et que le ZIP a été débloqué (voir [Installer](#install)).

**L'overlay n'apparaît pas.**

- L'overlay ne s'affiche qu'en Classé, Non classé, Bras de fer et Arène. Les autres modes ne sont pas encore mesurés.
- Pendant le choix de la main de départ, l'overlay est masqué. Il apparaît au premier tour.
- L'overlay ne s'affiche que lorsque Hearthstone est la fenêtre active. Cliquez une fois dans le jeu.
- Regardez la ligne d'état du tableau de bord, sous les boutons de période. Elle doit indiquer « Journal de partie (Power.log) : activé ». Si elle vous demande de redémarrer Hearthstone, faites-le.

**Le tableau de bord indique que le journal de partie n'a pas pu être activé.**

- Le tracker n'a pas pu écrire dans `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. Fermez Hearthstone, ouvrez le fichier dans le Bloc-notes (*Notepad*) (créez-le s'il n'existe pas), ajoutez ces lignes à la fin et enregistrez :

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**Un autre problème.**

Merci d'[ouvrir une issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) en indiquant la version du tracker (le tableau de bord l'affiche en haut à droite), votre version de Windows et ce que vous vous attendiez à voir. Si `%APPDATA%\HearthstoneXpTracker\xptracker.log` existe, ses dernières lignes aident aussi.

<a id="build-from-source"></a>
## Compiler depuis les sources

Voir [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

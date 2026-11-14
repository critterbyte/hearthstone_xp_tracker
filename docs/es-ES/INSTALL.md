# Instalación

> Traducción asistida por máquina. Si difiere de la [versión en inglés](../INSTALL.md), prevalece la versión en inglés. Las correcciones son bienvenidas: [abre una incidencia](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Requisitos

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT). Probado con HDT 1.60.1.
- Hearthstone

El plugin lee el `Power.log` de Hearthstone. HDT activa este log por sí solo, así que no hace falta cambiar ningún ajuste de Hearthstone.

<a id="install"></a>
## Instalar

1. Descarga el archivo ZIP de la versión más reciente desde la [página de Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), por ejemplo `HearthstoneXpTracker-0.1.0.zip`.
2. Cierra HDT.
3. Haz clic con el botón derecho en el archivo ZIP, elige *Propiedades* y marca *Desbloquear* si aparece. Si no, Windows puede bloquear el plugin.
4. Abre la carpeta de plugins de HDT. Puedes encontrarla de dos maneras:
   - En HDT: *Settings* (el icono del engranaje a la derecha de la barra superior) *→ Tracker → Plugins → Plugins Folder*
   - En el Explorador de Windows: escribe `%APPDATA%\HearthstoneDeckTracker\Plugins` en la barra de direcciones
5. Descomprime el ZIP en la carpeta de plugins. Así se crea la carpeta `HearthstoneXpTracker` con estos archivos:
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` y `LICENSE.txt`
6. Inicia HDT.
7. Abre *Settings → Tracker → Plugins* y activa **Hearthstone XP Tracker**.

El overlay aparece con el primer turno de tu próxima partida. Consulta [USAGE.md](USAGE.md) para ver qué muestra.

Los dos archivos DLL deben estar en la misma carpeta. Sin `XpTracker.Core.dll` el plugin no se inicia.

Cada versión indica la suma de comprobación SHA-256 del ZIP. Para comprobar tu descarga, ejecuta `Get-FileHash HearthstoneXpTracker-0.1.0.zip` en PowerShell y compara el resultado.

<a id="update"></a>
## Actualizar

Cuando hay una versión más reciente, el plugin te avisa (consulta [USAGE.md](USAGE.md#update-hint)). Para actualizar:

1. Descarga el archivo ZIP de la nueva versión y desbloquéalo (ver arriba).
2. Cierra HDT.
3. Vuelve a descomprimir el ZIP en la carpeta de plugins y sustituye los archivos antiguos.
4. Inicia HDT.

Tu historial de partidas y tus ajustes se mantienen. Se guardan en otra carpeta (ver abajo).

<a id="uninstall"></a>
## Desinstalar

1. En HDT, abre *Settings → Tracker → Plugins* y desactiva el plugin.
2. Cierra HDT.
3. Elimina la carpeta `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.

El plugin guarda tus datos en `%APPDATA%\HearthstoneDeckTracker\XpTracker`:

| Archivo | Contenido |
|---|---|
| `hs_games.csv` | Historial de partidas |
| `settings.ini` | Ajustes del plugin |

Elimina también esta carpeta si no quieres conservar los datos.

<a id="troubleshooting"></a>
## Solución de problemas

**El plugin no aparece en la lista o HDT muestra un error al cargarlo.**

- Comprueba que los dos archivos DLL están en `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` y no una carpeta más abajo (por ejemplo `...\HearthstoneXpTracker\HearthstoneXpTracker`).
- Windows puede bloquear los archivos descargados de internet. Haz clic con el botón derecho en cada DLL, elige *Propiedades* y, si hay una casilla *Desbloquear*, márcala y pulsa *Aceptar*. Después reinicia HDT.

**El overlay no aparece.**

- El overlay solo se muestra en Con rango, Aficionado y Pelea de taberna. Los demás modos aún no se miden.
- Mientras eliges tu mano inicial, el overlay está oculto. Aparece con el primer turno.
- Comprueba que el overlay del propio HDT funciona. El plugin dibuja sobre el overlay de HDT.

**Falla otra cosa.**

Por favor, [abre una incidencia](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) e indica tu versión de HDT y lo que esperabas ver.

<a id="build-from-source"></a>
## Compilar a partir del código fuente

Consulta [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

# Instalación

> Traducción asistida por máquina. Si difiere de la [versión en inglés](../INSTALL.md), rige la versión en inglés. Las correcciones son bienvenidas: [abre un issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Requisitos

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT). Probado con HDT 1.60.1.
- Hearthstone

El plugin lee el `Power.log` de Hearthstone. HDT activa este log por su cuenta, así que no necesitas cambiar ninguna configuración de Hearthstone.

<a id="install"></a>
## Instalar

1. Descarga el archivo ZIP de la versión más reciente desde la [página de Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), por ejemplo `HearthstoneXpTracker-0.1.0.zip`.
2. Cierra HDT.
3. Haz clic derecho en el archivo ZIP, elige *Propiedades* y marca *Desbloquear* si aparece. Si no, Windows podría bloquear el plugin.
4. Abre la carpeta de plugins de HDT. Puedes encontrarla de dos formas:
   - En HDT: *Settings* (el ícono de engrane a la derecha de la barra superior) *→ Tracker → Plugins → Plugins Folder*
   - En el Explorador de Windows: escribe `%APPDATA%\HearthstoneDeckTracker\Plugins` en la barra de direcciones
5. Extrae el ZIP en la carpeta de plugins. Esto crea la carpeta `HearthstoneXpTracker` con estos archivos:
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` y `LICENSE.txt`
6. Inicia HDT.
7. Abre *Settings → Tracker → Plugins* y activa **Hearthstone XP Tracker**.

El overlay aparece con el primer turno de tu próxima partida. Consulta [USAGE.md](USAGE.md) para ver qué muestra.

Los dos archivos DLL tienen que estar en la misma carpeta. Sin `XpTracker.Core.dll` el plugin no arranca.

Cada versión indica la suma de verificación SHA-256 del ZIP. Para revisar tu descarga, ejecuta `Get-FileHash HearthstoneXpTracker-0.1.0.zip` en PowerShell y compara el resultado.

<a id="update"></a>
## Actualizar

Cuando hay una versión más nueva, el plugin te avisa (consulta [USAGE.md](USAGE.md#update-hint)). Para actualizar:

1. Descarga el archivo ZIP de la nueva versión y desbloquéalo (ver arriba).
2. Cierra HDT.
3. Vuelve a extraer el ZIP en la carpeta de plugins y reemplaza los archivos anteriores.
4. Inicia HDT.

Tu historial de partidas y tu configuración se conservan. Se guardan en otra carpeta (ver abajo).

<a id="uninstall"></a>
## Desinstalar

1. En HDT, abre *Settings → Tracker → Plugins* y desactiva el plugin.
2. Cierra HDT.
3. Borra la carpeta `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.

El plugin guarda tus datos en `%APPDATA%\HearthstoneDeckTracker\XpTracker`:

| Archivo | Contenido |
|---|---|
| `hs_games.csv` | Historial de partidas |
| `settings.ini` | Configuración del plugin |

Borra también esta carpeta si no quieres conservar los datos.

<a id="troubleshooting"></a>
## Solución de problemas

**El plugin no aparece en la lista o HDT muestra un error al cargarlo.**

- Revisa que los dos archivos DLL estén en `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` y no una carpeta más adentro (por ejemplo `...\HearthstoneXpTracker\HearthstoneXpTracker`).
- Windows puede bloquear archivos descargados de internet. Haz clic derecho en cada DLL, elige *Propiedades* y, si hay una casilla *Desbloquear*, márcala y haz clic en *Aceptar*. Después reinicia HDT.

**El overlay no aparece.**

- El overlay solo se muestra en Clasificado, Casual y Gresca de la taberna. Los demás modos todavía no se miden.
- Mientras eliges tu mano inicial, el overlay está oculto. Aparece con el primer turno.
- Revisa que el overlay propio de HDT funcione. El plugin dibuja sobre el overlay de HDT.

**Algo más no funciona.**

Por favor, [abre un issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) e indica tu versión de HDT y lo que esperabas ver.

<a id="build-from-source"></a>
## Compilar desde el código fuente

Consulta [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

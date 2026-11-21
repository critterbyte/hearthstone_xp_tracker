# Instalación

> Traducción asistida por máquina. Si difiere de la [versión en inglés](../INSTALL.md), prevalece la versión en inglés. Las correcciones son bienvenidas: [abre una incidencia](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Requisitos

- Windows 10 u 11. El tracker necesita .NET Framework 4.7.2, que viene incluido en ambos.
- Hearthstone

El tracker lee el `Power.log` de Hearthstone. Hearthstone solo escribe este log si está activado en su propio archivo de ajustes, `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. El tracker lo activa la primera vez que se inicia: añade la sección `[Power]` y no cambia nada más del archivo. Antes de ese cambio guarda una copia del archivo antiguo como `log.config.bak`.

<a id="install"></a>
## Instalar

1. Descarga el archivo ZIP de la versión más reciente desde la [página de Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), por ejemplo `HearthstoneXpTracker-0.2.0.zip`.
2. Haz clic con el botón derecho en el archivo ZIP, elige *Propiedades* (*Properties*) y marca *Desbloquear* (*Unblock*) si aparece.
3. Descomprime el ZIP en la carpeta que quieras, por ejemplo *Documentos*. Así se crea la carpeta `HearthstoneXpTracker` con estos archivos:
   - `HearthstoneXpTracker.exe`: el tracker
   - `HearthstoneXpTracker.exe.config`, `XpTracker.Core.dll`, `LiveCharts.dll`, `LiveCharts.Wpf.dll` y `Newtonsoft.Json.dll`: los necesita el tracker
   - `INSTALL.txt`, `LICENSE.txt` y `THIRD-PARTY-NOTICES.txt`

   Elige una carpeta en la que puedas escribir. En *Program Files*, cada actualización necesita permisos de administrador.
4. Inicia `HearthstoneXpTracker.exe`. Si Windows muestra "Windows protegió tu PC" (*Windows protected your PC*), elige *Más información* (*More info*) y después *Ejecutar de todas formas* (*Run anyway*). El tracker todavía no está firmado, así que Windows no lo conoce.
5. Se abre el panel y aparece un icono dorado "XP" en el área de notificación (*notification area*), abajo a la derecha de la barra de tareas. Puede estar oculto detrás de la flechita (^).

Si Hearthstone ya estaba abierto la primera vez que iniciaste el tracker, reinícialo una vez. Hearthstone solo lee `log.config` al iniciarse.

El overlay aparece con el primer turno de tu próxima partida. Consulta [USAGE.md](USAGE.md) para ver qué muestra.

Todos los archivos deben quedarse juntos en una carpeta. En el primer inicio, una barra en la parte superior del panel pregunta una vez si quieres añadir el tracker al menú Inicio. Para que el tracker se inicie automáticamente al iniciar sesión en Windows, marca "Iniciar con Windows" en el panel. Si más adelante mueves la carpeta, abre el tracker una vez desde la nueva ubicación; así actualiza el acceso directo del menú Inicio y la entrada de inicio automático.

Cada versión indica la suma de comprobación SHA-256 del ZIP. Para comprobar tu descarga, ejecuta `Get-FileHash HearthstoneXpTracker-0.2.0.zip` en PowerShell y compara el resultado.

<a id="coming-from-the-hdt-plugin"></a>
## Si usabas el plugin de HDT

Hasta la versión 0.1.2, el tracker era un plugin para HDT. HDT deja de admitir plugins el 30 de noviembre de 2026, así que ahora el tracker es un programa independiente, con el mismo overlay y el mismo panel.

- **Tus datos te acompañan.** Cada vez que se inicia, el tracker añade las partidas del historial del plugin (`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`) que aún no tiene. También adopta una vez los ajustes del plugin, si todavía no tiene los suyos. Los archivos del plugin solo se leen, nunca se modifican.
- **Desactiva el plugin.** En HDT, abre *Settings* (el icono del engranaje a la derecha de la barra superior) *→ Tracker → Plugins* y desactiva **Hearthstone XP Tracker**. Si no, dos overlays muestran las mismas cifras. También puedes eliminar la carpeta `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.
- **De momento, sin nombres de mazos.** Los nombres de los mazos los proporcionaba HDT, así que de momento ya no están la pestaña *Mazos* ni la columna del mazo. Las partidas anteriores conservan el nombre de su mazo en el archivo del historial.

<a id="update"></a>
## Actualizar

Cuando hay una versión más reciente, el tracker te avisa (consulta [USAGE.md](USAGE.md#update-hint)). Para actualizar:

1. Descarga el archivo ZIP de la nueva versión y desbloquéalo (ver arriba).
2. Cierra el tracker: haz clic con el botón derecho en el icono dorado "XP" y elige "Salir".
3. Vuelve a descomprimir el ZIP en la misma carpeta y sustituye los archivos antiguos.
4. Inicia `HearthstoneXpTracker.exe`.

Tu historial de partidas y tus ajustes se mantienen. Se guardan en otra carpeta (ver abajo).

Para volver a una versión anterior, haz lo mismo con su ZIP.

<a id="uninstall"></a>
## Desinstalar

1. En el panel, desmarca "Iniciar con Windows" y "Acceso directo en el menú Inicio".
2. Cierra el tracker: haz clic con el botón derecho en el icono dorado "XP" y elige "Salir".
3. Elimina la carpeta `HearthstoneXpTracker` que descomprimiste.

El tracker guarda tus datos en `%APPDATA%\HearthstoneXpTracker`:

| Archivo | Contenido |
|---|---|
| `hs_games.csv` | Historial de partidas |
| `settings.ini` | Ajustes |
| `xptracker.log` | Errores inesperados, si los hay |

Elimina también esta carpeta si no quieres conservar los datos.

La sección `[Power]` se queda en `log.config`, porque otros programas también pueden usarla. Solo hace que Hearthstone escriba su log. Para dejar el archivo como estaba antes, sustituye `log.config` por `log.config.bak`.

<a id="troubleshooting"></a>
## Solución de problemas

**Windows muestra "Windows protegió tu PC" (*Windows protected your PC*).**

- Elige *Más información* (*More info*) y después *Ejecutar de todas formas* (*Run anyway*). Windows muestra este aviso con los programas que aún no conoce. El tracker es de código abierto: puedes leer el código en GitHub y comprobar la suma de comprobación SHA-256 del ZIP.

**No pasa nada cuando inicio el tracker.**

- Puede que ya esté abierto. Al iniciarlo otra vez se abre el panel del tracker que ya está en marcha. Busca el icono dorado "XP" en el área de notificación (*notification area*), también detrás de la flechita (^).
- Comprueba que todos los archivos del ZIP están en la misma carpeta y que el ZIP se desbloqueó (consulta [Instalar](#install)).

**El overlay no aparece.**

- El overlay solo se muestra en Con rango, Aficionado, Pelea de taberna y Arena. Los demás modos aún no se miden.
- Mientras eliges tu mano inicial, el overlay está oculto. Aparece con el primer turno.
- El overlay solo se muestra mientras Hearthstone es la ventana activa. Haz clic una vez dentro del juego.
- Mira la línea de estado del panel, debajo de los botones de periodo. Debería decir "Registro de partida (Power.log): activado". Si te pide reiniciar Hearthstone, hazlo.

**El panel dice que no se pudo activar el registro de partida.**

- El tracker no pudo escribir en `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. Cierra Hearthstone, abre el archivo con el Bloc de notas (*Notepad*) (créalo si no existe), añade estas líneas al final y guárdalo:

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**Falla otra cosa.**

Por favor, [abre una incidencia](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) e indica la versión del tracker (el panel la muestra en la esquina superior derecha), tu versión de Windows y lo que esperabas ver. Si existe `%APPDATA%\HearthstoneXpTracker\xptracker.log`, sus últimas líneas también ayudan.

<a id="build-from-source"></a>
## Compilar a partir del código fuente

Consulta [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

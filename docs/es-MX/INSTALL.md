# Instalación

> Traducción asistida por máquina. Si difiere de la [versión en inglés](../INSTALL.md), rige la versión en inglés. Las correcciones son bienvenidas: [abre un issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Requisitos

- Windows 10 u 11. El tracker necesita .NET Framework 4.7.2, que ya viene incluido en ambos.
- Hearthstone

No necesitas Hearthstone Deck Tracker (HDT). El tracker funciona por su cuenta o junto con HDT.

El tracker lee el `Power.log` de Hearthstone. Hearthstone solo escribe este log si está activado en su propio archivo de configuración, `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. El tracker lo activa la primera vez que arranca: agrega la sección `[Power]` y no cambia nada más en el archivo. Antes de ese cambio guarda una copia del archivo anterior como `log.config.bak`. Si tienes HDT instalado, el log ya está activado.

<a id="install"></a>
## Instalar

1. Descarga el archivo ZIP de la versión más reciente desde la [página de Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), por ejemplo `HearthstoneXpTracker-0.2.0.zip`.
2. Haz clic derecho en el archivo ZIP, elige *Propiedades* (*Properties*) y marca *Desbloquear* (*Unblock*) si aparece.
3. Extrae el ZIP en la carpeta que quieras, por ejemplo *Documentos*. Esto crea la carpeta `HearthstoneXpTracker` con estos archivos:
   - `HearthstoneXpTracker.exe`: el tracker
   - `HearthstoneXpTracker.exe.config`, `XpTracker.Core.dll`, `LiveCharts.dll`, `LiveCharts.Wpf.dll` y `Newtonsoft.Json.dll`: los necesita el tracker
   - `INSTALL.txt`, `LICENSE.txt` y `THIRD-PARTY-NOTICES.txt`

   Elige una carpeta en la que puedas escribir. En *Program Files*, cada actualización necesita permisos de administrador.
4. Inicia `HearthstoneXpTracker.exe`. Si Windows muestra "Windows protegió tu PC" (*Windows protected your PC*), elige *Más información* (*More info*) y luego *Ejecutar de todas formas* (*Run anyway*). El tracker todavía no está firmado, así que Windows no lo conoce.
5. Se abre el panel y aparece un ícono dorado "XP" en el área de notificación (*notification area*), abajo a la derecha de la barra de tareas. Puede estar escondido detrás de la flechita (^).

Si Hearthstone ya estaba abierto la primera vez que iniciaste el tracker, reinícialo una vez. Hearthstone solo lee `log.config` cuando arranca.

El overlay aparece con el primer turno de tu próxima partida. Consulta [USAGE.md](USAGE.md) para ver qué muestra.

Todos los archivos deben quedarse juntos en una carpeta. En el primer inicio, una barra en la parte superior del panel pregunta una vez si quieres agregar el tracker al menú Inicio. Para que el tracker se inicie automáticamente al iniciar sesión en Windows, marca "Iniciar con Windows" en el panel. Si más adelante mueves la carpeta, abre el tracker una vez desde la nueva ubicación; así actualiza el acceso directo del menú Inicio y la entrada de inicio automático.

Cada versión indica la suma de verificación SHA-256 del ZIP. Para revisar tu descarga, ejecuta `Get-FileHash HearthstoneXpTracker-0.2.0.zip` en PowerShell y compara el resultado.

<a id="coming-from-the-hdt-plugin"></a>
## Si usabas el plugin de HDT

Hasta la versión 0.1.2, el tracker era un plugin para HDT. HDT deja de dar soporte a los plugins el 30 de noviembre de 2026, así que ahora el tracker es un programa independiente, con el mismo overlay y el mismo panel.

- **Tus datos se vienen contigo.** Cada vez que arranca, el tracker agrega las partidas del historial del plugin (`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`) que todavía no tiene. También toma una vez la configuración del plugin, si todavía no tiene la suya. Los archivos del plugin solo se leen, nunca se modifican.
- **Desactiva el plugin.** En HDT, abre *Settings* (el ícono de engrane a la derecha de la barra superior) *→ Tracker → Plugins* y desactiva **Hearthstone XP Tracker**. Si no, dos overlays muestran los mismos números. También puedes borrar la carpeta `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.
- **Por ahora, sin nombres de mazos.** Los nombres de los mazos venían de HDT, así que por ahora ya no están la pestaña *Mazos* ni la columna del mazo. Las partidas anteriores conservan el nombre de su mazo en el archivo del historial.

<a id="update"></a>
## Actualizar

Cuando hay una versión más nueva, el tracker te avisa (consulta [USAGE.md](USAGE.md#update-hint)). Para actualizar:

1. Descarga el archivo ZIP de la nueva versión y desbloquéalo (ver arriba).
2. Cierra el tracker: haz clic derecho en el ícono dorado "XP" y elige "Salir".
3. Vuelve a extraer el ZIP en la misma carpeta y reemplaza los archivos anteriores.
4. Inicia `HearthstoneXpTracker.exe`.

Tu historial de partidas y tu configuración se conservan. Se guardan en otra carpeta (ver abajo).

Para volver a una versión anterior, haz lo mismo con su ZIP.

<a id="uninstall"></a>
## Desinstalar

1. En el panel, desmarca "Iniciar con Windows" y "Acceso directo en el menú Inicio".
2. Cierra el tracker: haz clic derecho en el ícono dorado "XP" y elige "Salir".
3. Borra la carpeta `HearthstoneXpTracker` que extrajiste.

El tracker guarda tus datos en `%APPDATA%\HearthstoneXpTracker`:

| Archivo | Contenido |
|---|---|
| `hs_games.csv` | Historial de partidas |
| `settings.ini` | Configuración |
| `xptracker.log` | Errores inesperados, si los hay |

Borra también esta carpeta si no quieres conservar los datos.

La sección `[Power]` se queda en `log.config`, porque otras herramientas como HDT también la usan. Solo hace que Hearthstone escriba su log. Para dejar el archivo como estaba antes, reemplaza `log.config` por `log.config.bak`.

<a id="troubleshooting"></a>
## Solución de problemas

**Windows muestra "Windows protegió tu PC" (*Windows protected your PC*).**

- Elige *Más información* (*More info*) y luego *Ejecutar de todas formas* (*Run anyway*). Windows muestra este aviso con los programas que todavía no conoce. El tracker es de código abierto: puedes leer el código en GitHub y revisar la suma de verificación SHA-256 del ZIP.

**No pasa nada cuando inicio el tracker.**

- Puede que ya esté abierto. Si lo inicias otra vez, se abre el panel del tracker que ya está corriendo. Busca el ícono dorado "XP" en el área de notificación (*notification area*), también detrás de la flechita (^).
- Revisa que todos los archivos del ZIP estén en la misma carpeta y que el ZIP se haya desbloqueado (consulta [Instalar](#install)).

**El overlay no aparece.**

- El overlay solo se muestra en Clasificado, Casual, Gresca de la taberna y Arena. Los demás modos todavía no se miden.
- Mientras eliges tu mano inicial, el overlay está oculto. Aparece con el primer turno.
- El overlay solo se muestra mientras Hearthstone es la ventana activa. Haz clic una vez dentro del juego.
- Mira la línea de estado del panel, debajo de los botones de periodo. Debería decir "Registro de partida (Power.log): activado". Si te pide reiniciar Hearthstone, hazlo.

**El panel dice que no se pudo activar el registro de partida.**

- El tracker no pudo escribir en `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. Cierra Hearthstone, abre el archivo con el Bloc de notas (*Notepad*) (créalo si no existe), agrega estas líneas al final y guárdalo:

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**Algo más no funciona.**

Por favor, [abre un issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) e indica la versión del tracker (el panel la muestra en la esquina superior derecha), tu versión de Windows y lo que esperabas ver. Si existe `%APPDATA%\HearthstoneXpTracker\xptracker.log`, sus últimas líneas también ayudan.

<a id="build-from-source"></a>
## Compilar desde el código fuente

Consulta [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

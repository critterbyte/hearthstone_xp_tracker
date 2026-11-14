# Установка

> Текст переведён с помощью машинного перевода. Если он расходится с [английской версией](../INSTALL.md), действует английская версия. Исправления приветствуются: [создайте issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Требования

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT). Проверено с HDT 1.60.1.
- Hearthstone

Плагин читает файл `Power.log` игры Hearthstone. HDT сам включает этот журнал, поэтому менять настройки Hearthstone не нужно.

<a id="install"></a>
## Установка

1. Скачайте ZIP-файл последнего выпуска со [страницы Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), например `HearthstoneXpTracker-0.1.0.zip`.
2. Закройте HDT.
3. Щёлкните ZIP-файл правой кнопкой мыши, выберите *Свойства* (*Properties*) и отметьте *Разблокировать* (*Unblock*), если этот пункт есть. Иначе Windows может заблокировать плагин.
4. Откройте папку плагинов HDT. Найти её можно двумя способами:
   - В HDT: *Settings* (значок шестерёнки справа на верхней панели) *→ Tracker → Plugins → Plugins Folder*
   - В Проводнике Windows: введите `%APPDATA%\HearthstoneDeckTracker\Plugins` в адресную строку
5. Распакуйте ZIP в папку плагинов. Появится папка `HearthstoneXpTracker` с такими файлами:
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` и `LICENSE.txt`
6. Запустите HDT.
7. Откройте *Settings → Tracker → Plugins* и включите **Hearthstone XP Tracker**.

Оверлей появится с первым ходом вашего следующего матча. Что он показывает, описано в [USAGE.md](USAGE.md).

Оба DLL-файла должны лежать в одной папке. Без `XpTracker.Core.dll` плагин не запустится.

Для каждого выпуска указана контрольная сумма SHA-256 ZIP-файла. Чтобы проверить загрузку, выполните в PowerShell `Get-FileHash HearthstoneXpTracker-0.1.0.zip` и сравните результат.

<a id="update"></a>
## Обновление

Когда выходит новая версия, плагин сообщает об этом (см. [USAGE.md](USAGE.md#update-hint)). Чтобы обновиться:

1. Скачайте ZIP-файл нового выпуска и разблокируйте его (см. выше).
2. Закройте HDT.
3. Снова распакуйте ZIP в папку плагинов и замените старые файлы.
4. Запустите HDT.

История матчей и настройки сохраняются. Они хранятся в другой папке (см. ниже).

<a id="uninstall"></a>
## Удаление

1. В HDT откройте *Settings → Tracker → Plugins* и отключите плагин.
2. Закройте HDT.
3. Удалите папку `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.

Плагин хранит ваши данные в `%APPDATA%\HearthstoneDeckTracker\XpTracker`:

| Файл | Содержимое |
|---|---|
| `hs_games.csv` | История матчей |
| `settings.ini` | Настройки плагина |

Удалите и эту папку, если не хотите сохранять данные.

<a id="troubleshooting"></a>
## Устранение неполадок

**Плагина нет в списке, или HDT показывает ошибку при его загрузке.**

- Проверьте, что оба DLL-файла лежат в `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`, а не на одну папку глубже (например, `...\HearthstoneXpTracker\HearthstoneXpTracker`).
- Windows может блокировать файлы, скачанные из интернета. Щёлкните каждый DLL-файл правой кнопкой мыши, выберите *Свойства* (*Properties*) и, если там есть флажок *Разблокировать* (*Unblock*), отметьте его и нажмите *OK*. Затем перезапустите HDT.

**Оверлей не появляется.**

- Оверлей показывается только в режимах Рейтинг, Обычный, Потасовка в таверне и Арена. Другие режимы пока не измеряются.
- Во время выбора стартовой руки оверлей скрыт. Он появляется с первым ходом.
- Проверьте, работает ли собственный оверлей HDT. Плагин рисует поверх оверлея HDT.

**Что-то другое работает не так.**

Пожалуйста, [создайте issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) и укажите свою версию HDT и то, что вы ожидали увидеть.

<a id="build-from-source"></a>
## Сборка из исходного кода

См. [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

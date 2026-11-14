# Instalacja

> Tłumaczenie wspomagane maszynowo. Jeśli różni się od [wersji angielskiej](../INSTALL.md), obowiązuje wersja angielska. Poprawki są mile widziane: [zgłoś problem](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Wymagania

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT). Przetestowano z HDT 1.60.1.
- Hearthstone

Wtyczka czyta plik `Power.log` gry Hearthstone. HDT sam włącza ten log, więc nie musisz zmieniać żadnych ustawień w Hearthstone.

<a id="install"></a>
## Instalacja

1. Pobierz plik ZIP najnowszego wydania ze [strony Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), na przykład `HearthstoneXpTracker-0.1.0.zip`.
2. Zamknij HDT.
3. Kliknij plik ZIP prawym przyciskiem myszy, wybierz *Właściwości* (*Properties*) i zaznacz *Odblokuj* (*Unblock*), jeśli ta opcja jest widoczna. W przeciwnym razie Windows może zablokować wtyczkę.
4. Otwórz folder wtyczek HDT. Znajdziesz go na dwa sposoby:
   - W HDT: *Settings* (ikona koła zębatego po prawej stronie górnego paska) *→ Tracker → Plugins → Plugins Folder*
   - W Eksploratorze Windows: wpisz `%APPDATA%\HearthstoneDeckTracker\Plugins` w pasku adresu
5. Rozpakuj ZIP do folderu wtyczek. Powstanie folder `HearthstoneXpTracker` z tymi plikami:
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` i `LICENSE.txt`
6. Uruchom HDT.
7. Otwórz *Settings → Tracker → Plugins* i włącz **Hearthstone XP Tracker**.

Nakładka pojawi się w pierwszej turze twojej następnej gry. Co pokazuje, opisuje [USAGE.md](USAGE.md).

Oba pliki DLL muszą być w tym samym folderze. Bez `XpTracker.Core.dll` wtyczka się nie uruchomi.

Każde wydanie podaje sumę kontrolną SHA-256 pliku ZIP. Aby sprawdzić pobrany plik, uruchom w PowerShell `Get-FileHash HearthstoneXpTracker-0.1.0.zip` i porównaj wynik.

<a id="update"></a>
## Aktualizacja

Gdy pojawi się nowsza wersja, wtyczka cię o tym poinformuje (zobacz [USAGE.md](USAGE.md#update-hint)). Aby zaktualizować:

1. Pobierz plik ZIP nowego wydania i odblokuj go (zobacz wyżej).
2. Zamknij HDT.
3. Ponownie rozpakuj ZIP do folderu wtyczek i zastąp stare pliki.
4. Uruchom HDT.

Historia gier i ustawienia zostają zachowane. Są zapisane w innym folderze (zobacz niżej).

<a id="uninstall"></a>
## Odinstalowanie

1. W HDT otwórz *Settings → Tracker → Plugins* i wyłącz wtyczkę.
2. Zamknij HDT.
3. Usuń folder `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.

Wtyczka przechowuje twoje dane w `%APPDATA%\HearthstoneDeckTracker\XpTracker`:

| Plik | Zawartość |
|---|---|
| `hs_games.csv` | Historia gier |
| `settings.ini` | Ustawienia wtyczki |

Usuń też ten folder, jeśli nie chcesz zachować danych.

<a id="troubleshooting"></a>
## Rozwiązywanie problemów

**Wtyczki nie ma na liście albo HDT pokazuje błąd przy jej wczytywaniu.**

- Sprawdź, czy oba pliki DLL leżą w `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`, a nie o jeden folder głębiej (na przykład `...\HearthstoneXpTracker\HearthstoneXpTracker`).
- Windows może blokować pliki pobrane z internetu. Kliknij każdy plik DLL prawym przyciskiem myszy, wybierz *Właściwości* (*Properties*), a jeśli jest pole wyboru *Odblokuj* (*Unblock*), zaznacz je i kliknij *OK*. Potem uruchom HDT ponownie.

**Nakładka się nie pojawia.**

- Nakładka jest widoczna tylko w trybach Ranking, Rozrywka, Karczemna bójka i Arena. Inne tryby nie są jeszcze mierzone.
- Podczas wyboru ręki startowej nakładka jest ukryta. Pojawia się w pierwszej turze.
- Sprawdź, czy działa nakładka samego HDT. Wtyczka rysuje na nakładce HDT.

**Coś innego nie działa.**

[Zgłoś problem](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) i podaj swoją wersję HDT oraz to, czego się spodziewałeś.

<a id="build-from-source"></a>
## Budowanie ze źródeł

Zobacz [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

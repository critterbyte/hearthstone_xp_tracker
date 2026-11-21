# Instalacja

> Tłumaczenie wspomagane maszynowo. Jeśli różni się od [wersji angielskiej](../INSTALL.md), obowiązuje wersja angielska. Poprawki są mile widziane: [zgłoś problem](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Wymagania

- Windows 10 lub 11. Tracker potrzebuje .NET Framework 4.7.2, który jest częścią obu systemów.
- Hearthstone

Tracker czyta plik `Power.log` gry Hearthstone. Hearthstone zapisuje ten log tylko wtedy, gdy jest on włączony we własnym pliku ustawień gry, `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. Tracker włącza go przy pierwszym uruchomieniu: dodaje sekcję `[Power]` i niczego więcej w pliku nie zmienia. Przed tą zmianą zachowuje kopię starego pliku jako `log.config.bak`.

<a id="install"></a>
## Instalacja

1. Pobierz plik ZIP najnowszego wydania ze [strony Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), na przykład `HearthstoneXpTracker-0.2.0.zip`.
2. Kliknij plik ZIP prawym przyciskiem myszy, wybierz *Właściwości* (*Properties*) i zaznacz *Odblokuj* (*Unblock*), jeśli ta opcja jest widoczna.
3. Rozpakuj ZIP do wybranego folderu, na przykład *Dokumenty* (*Documents*). Powstanie folder `HearthstoneXpTracker` z tymi plikami:
   - `HearthstoneXpTracker.exe`: tracker
   - `HearthstoneXpTracker.exe.config`, `XpTracker.Core.dll`, `LiveCharts.dll`, `LiveCharts.Wpf.dll` i `Newtonsoft.Json.dll`: potrzebne trackerowi
   - `INSTALL.txt`, `LICENSE.txt` i `THIRD-PARTY-NOTICES.txt`

   Wybierz folder, w którym możesz zapisywać pliki. W folderze *Pliki programów* (*Program Files*) każda aktualizacja wymaga uprawnień administratora.
4. Uruchom `HearthstoneXpTracker.exe`. Jeśli Windows pokaże komunikat „System Windows ochronił Twój komputer” (*Windows protected your PC*), wybierz *Więcej informacji* (*More info*), a potem *Uruchom mimo to* (*Run anyway*). Tracker nie jest jeszcze podpisany, więc Windows go nie zna.
5. Otworzy się panel, a w obszarze powiadomień (*notification area*) w prawym dolnym rogu paska zadań pojawi się złota ikona „XP”. Może być ukryta pod małą strzałką (^).

Jeśli Hearthstone był już uruchomiony przy pierwszym starcie trackera, uruchom go raz ponownie. Hearthstone czyta `log.config` tylko przy starcie.

Nakładka pojawi się w pierwszej turze twojej następnej gry. Co pokazuje, opisuje [USAGE.md](USAGE.md).

Wszystkie pliki muszą pozostać razem w jednym folderze. Przy pierwszym uruchomieniu pasek u góry panelu raz zapyta, czy dodać tracker do menu Start. Aby tracker uruchamiał się automatycznie po zalogowaniu do Windows, zaznacz w panelu „Uruchamiaj z systemem Windows”. Jeśli później przeniesiesz folder, uruchom tracker raz z nowego miejsca; wtedy zaktualizuje skrót w menu Start i wpis autostartu.

Każde wydanie podaje sumę kontrolną SHA-256 pliku ZIP. Aby sprawdzić pobrany plik, uruchom w PowerShell `Get-FileHash HearthstoneXpTracker-0.2.0.zip` i porównaj wynik.

<a id="coming-from-the-hdt-plugin"></a>
## Przejście z wtyczki HDT

Do wersji 0.1.2 tracker był wtyczką do HDT. HDT kończy obsługę wtyczek 30 listopada 2026, dlatego tracker jest teraz osobnym programem z tą samą nakładką i tym samym panelem.

- **Twoje dane przechodzą razem z tobą.** Przy każdym uruchomieniu tracker dodaje z historii wtyczki (`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`) gry, których jeszcze nie ma. Przejmuje też jednorazowo ustawienia wtyczki, jeśli nie ma jeszcze własnych. Pliki wtyczki są tylko odczytywane, nigdy zmieniane.
- **Wyłącz wtyczkę.** W HDT otwórz *Settings* (ikona koła zębatego po prawej stronie górnego paska) *→ Tracker → Plugins* i wyłącz **Hearthstone XP Tracker**. W przeciwnym razie dwie nakładki pokażą te same liczby. Możesz też usunąć folder `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.
- **Na razie bez nazw talii.** Nazwy talii pochodziły z HDT, dlatego karta *Talie* i kolumna talii na razie zniknęły. Starsze gry zachowują nazwy talii w pliku historii.

<a id="update"></a>
## Aktualizacja

Gdy pojawi się nowsza wersja, tracker cię o tym poinformuje (zobacz [USAGE.md](USAGE.md#update-hint)). Aby zaktualizować:

1. Pobierz plik ZIP nowego wydania i odblokuj go (zobacz wyżej).
2. Zakończ działanie trackera: kliknij złotą ikonę „XP” prawym przyciskiem myszy i wybierz „Zakończ”.
3. Ponownie rozpakuj ZIP do tego samego folderu i zastąp stare pliki.
4. Uruchom `HearthstoneXpTracker.exe`.

Historia gier i ustawienia zostają zachowane. Są zapisane w innym folderze (zobacz niżej).

Aby wrócić do starszej wersji, zrób to samo z jej plikiem ZIP.

<a id="uninstall"></a>
## Odinstalowanie

1. W panelu odznacz „Uruchamiaj z systemem Windows” i „Skrót w menu Start”.
2. Zakończ działanie trackera: kliknij złotą ikonę „XP” prawym przyciskiem myszy i wybierz „Zakończ”.
3. Usuń rozpakowany folder `HearthstoneXpTracker`.

Tracker przechowuje twoje dane w `%APPDATA%\HearthstoneXpTracker`:

| Plik | Zawartość |
|---|---|
| `hs_games.csv` | Historia gier |
| `settings.ini` | Ustawienia |
| `xptracker.log` | Nieoczekiwane błędy, jeśli wystąpiły |

Usuń też ten folder, jeśli nie chcesz zachować danych.

Sekcja `[Power]` pozostaje w `log.config`, bo mogą z niej korzystać także inne programy. Sprawia ona tylko, że Hearthstone zapisuje swój log. Aby przywrócić plik do poprzedniego stanu, zastąp `log.config` plikiem `log.config.bak`.

<a id="troubleshooting"></a>
## Rozwiązywanie problemów

**Windows pokazuje komunikat „System Windows ochronił Twój komputer” (*Windows protected your PC*).**

- Wybierz *Więcej informacji* (*More info*), a potem *Uruchom mimo to* (*Run anyway*). Windows pokazuje ten komunikat przy programach, których jeszcze nie zna. Tracker ma otwarty kod źródłowy; możesz przeczytać kod na GitHubie i sprawdzić sumę kontrolną SHA-256 pliku ZIP.

**Po uruchomieniu trackera nic się nie dzieje.**

- Możliwe, że już działa. Ponowne uruchomienie otwiera panel działającego trackera. Poszukaj złotej ikony „XP” w obszarze powiadomień, także pod małą strzałką (^).
- Sprawdź, czy wszystkie pliki z ZIP są w tym samym folderze i czy ZIP został odblokowany (zobacz [Instalacja](#install)).

**Nakładka się nie pojawia.**

- Nakładka jest widoczna tylko w trybach Ranking, Rozrywka, Karczemna bójka i Arena. Inne tryby nie są jeszcze mierzone.
- Podczas wyboru ręki startowej nakładka jest ukryta. Pojawia się w pierwszej turze.
- Nakładka jest widoczna tylko wtedy, gdy Hearthstone jest aktywnym oknem. Kliknij raz w grę.
- Spójrz na wiersz stanu w panelu, pod przyciskami okresów. Powinno tam być „Dziennik gry (Power.log): włączony”. Jeśli prosi o ponowne uruchomienie Hearthstone, zrób to.

**Panel informuje, że nie udało się włączyć dziennika gry.**

- Tracker nie mógł zapisać pliku `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. Zamknij Hearthstone, otwórz plik w *Notatniku* (*Notepad*) (utwórz go, jeśli go brakuje), dodaj na końcu te wiersze i zapisz go:

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**Coś innego nie działa.**

[Zgłoś problem](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) i podaj wersję trackera (panel pokazuje ją w prawym górnym rogu), swoją wersję Windows oraz to, czego się spodziewałeś. Jeśli istnieje plik `%APPDATA%\HearthstoneXpTracker\xptracker.log`, pomogą też jego ostatnie wiersze.

<a id="build-from-source"></a>
## Budowanie ze źródeł

Zobacz [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

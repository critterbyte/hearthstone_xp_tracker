# Obsługa

> Tłumaczenie wspomagane maszynowo. Jeśli różni się od [wersji angielskiej](../USAGE.md), obowiązuje wersja angielska. Poprawki są mile widziane: [zgłoś problem](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

Wtyczka ma trzy części:

- **nakładkę** w grze
- okno **panelu**
- plik z **historią gier**

Teksty w cudzysłowie, na przykład „Czas PD:”, to teksty, które pokazuje wtyczka.

<a id="overlay"></a>
## Nakładka

Nakładka to małe pole nad przyciskiem ustawień Hearthstone (koło zębate w prawym dolnym rogu). Etykiety są po lewej. Wartości są po prawej, na niebieskich paskach.

Nakładka jest widoczna tylko w trybach **Ranking, Rozrywka i Karczemna bójka**. Inne tryby nie są jeszcze mierzone, więc wtyczka nic tam nie pokazuje. Karczemna bójka przeciwko komputerowi (PvE, na przykład przeciwko Karczmarzowi) nie daje PD, więc nakładka pokazuje tam „+0”.

<a id="during-a-game"></a>
### W trakcie gry

Wiersze pojawiają się w pierwszej turze. Podczas wyboru ręki startowej nakładka jest ukryta.

| Wiersz | Znaczenie |
|---|---|
| „Czas PD:” | Czas, który do tej pory liczy się do PD, w minutach i sekundach |
| „Za zwycięstwo:” | PD, które dostaniesz, jeśli teraz wygrasz |
| „Za porażkę:” | PD, które dostaniesz, jeśli teraz przegrasz lub się poddasz |

Czas PD może mieć dopisek:

| Dopisek | Znaczenie |
|---|---|
| „(wstrzymane)” | Twoja tura minęła bez żadnej akcji. Ta tura i następna tura przeciwnika się nie liczą, więc czas PD cofnął się i czeka. Ruszy znowu w twojej następnej turze. |
| „(maks.)” | Czas PD osiągnął 30 minut. Dalszy czas nie daje już PD. |

W trakcie wydarzenia w Hearthstone włącz w panelu „Trwa wydarzenie (pokaż PD wydarzenia)”. Nakładka pokaże wtedy jeszcze jeden wiersz, na bieżąco i w podsumowaniu po grze:

| Wiersz | Znaczenie |
|---|---|
| „PD wydarzenia:” | Dotychczasowe PD wydarzenia, z czerwonym kryształem: jeden za minutę czasu PD (zobacz [FORMULA.md](../FORMULA.md#event-xp)). Nie zależą od wyniku. |

Wyłącz tę opcję, gdy wydarzenie się skończy.

Gdy w panelu jest włączony „Karnet Karczmarza (premia PD)”, wartości „Za zwycięstwo:”, „Za porażkę:” i „Zdobyte:” uwzględniają premię karnetu dla twojego poziomu na Ścieżce nagród. PD wydarzenia nie dostają premii.

Ostatnie dwa wiersze pokazują obrażenia, na bieżąco i w podsumowaniu po grze. Wyłączysz je opcją „Pokaż obrażenia na nakładce” w panelu.

| Wiersz | Znaczenie |
|---|---|
| „Obrażenia łącznie:” | Wszystkie obrażenia, które twoja strona zadała do tej pory bohaterowi i stronnikom przeciwnika: twoje ataki, ataki twoich stronników, zaklęcia, broń, moc specjalna bohatera i inne efekty. Każde trafienie liczy się z wartością, którą pokazuje gra, także gdy przekracza zdrowie celu albo trafia w pancerz. Obrażenia zadane własnemu bohaterowi lub własnym stronnikom się nie liczą, podobnie jak obrażenia bez źródła, na przykład zmęczenie. Trafienie w Boską tarczę liczy się jako 0. |
| „Obrażenia bohatera:” | Część „Obrażenia łącznie:”, która trafiła bohatera przeciwnika, łącznie z częścią pochłoniętą przez pancerz |

Obrażenia są odczytywane, gdy serwer potwierdzi akcję, więc mogą wzrosnąć chwilę przed animacją na planszy.

Dopóki czas PD jest poniżej 60 sekund, obie wartości PD pokazują „+0”. Gry krótsze niż minuta nie dają PD. Zasady opisuje [FORMULA.md](../FORMULA.md).

Jeśli uruchomisz HDT w trakcie gry, wtyczka i tak odczyta grę od początku.

<a id="after-a-game"></a>
### Po grze

Po grze nakładka pokazuje podsumowanie aż do wyboru ręki startowej w twojej następnej grze:

| Wiersz | Znaczenie |
|---|---|
| „Ostatnia gra:” | Zwycięstwo lub Porażka |
| „Czas PD:” | Czas, który liczył się do PD |
| „Czas całkowity:” | Cała gra łącznie z wyborem ręki startowej |
| „Zdobyte:” | PD, które dała gra |

<a id="update-hint"></a>
### Informacja o aktualizacji

Gdy istnieje nowsza wersja wtyczki, nakładka pokazuje jeszcze jeden wiersz, kiedy nie trwa żadna gra:

| Wiersz | Znaczenie |
|---|---|
| „Dostępna aktualizacja:” | Numer nowej wersji, na przykład 0.2.0 |

Panel pokazuje link do strony pobierania. Jak zaktualizować, opisuje [INSTALL.md](INSTALL.md#update).

<a id="dashboard"></a>
## Panel

Okno panelu nazywa się „HS XP Tracker”. Otworzysz je w HDT na jeden z dwóch sposobów:

- menu *Plugins* → „Panel PD”
- *Settings* (ikona koła zębatego po prawej stronie górnego paska) *→ Tracker → Plugins* → przycisk wtyczki „Panel PD”

<a id="toolbar"></a>
### Pasek narzędzi

| Element | Znaczenie |
|---|---|
| „Sesja” | Gry z bieżącej sesji Hearthstone, od uruchomienia gry do jej zamknięcia. Po zamknięciu Hearthstone ostatnia sesja pozostaje widoczna do następnego uruchomienia. Ten okres panel pokazuje po otwarciu. |
| „Dzisiaj” | Gry od północy |
| „Ten tydzień” | Gry od poniedziałku, 00:00 |
| „Ten sezon” | Gry od 1. dnia miesiąca. Sezony rankingowe w Hearthstone trwają miesiąc kalendarzowy. |
| „Ostatnie 30 dni” | Gry z ostatnich 30 dni, łącznie z dzisiejszym |
| „Cały czas” | Wszystkie gry w historii |
| „Odśwież” | Ponowne odczytanie historii |
| „Otwórz folder danych” | Otwiera folder z plikiem historii w Eksploratorze Windows |
| „Język (Language)” | Język nakładki, panelu i pozycji wtyczki w menu. Domyślnie angielski. Zmiana działa od razu; panel otwiera się ponownie w nowym języku. W innych językach etykieta zachowuje angielskie słowo, na przykład „Język (Language)”. |
| „Trwa wydarzenie (pokaż PD wydarzenia)” | Włącz w trakcie wydarzenia w Hearthstone: nakładka pokazuje PD wydarzenia, a nowe gry je zapisują. |
| „Karnet Karczmarza (premia PD)” | Włącz, jeśli masz Karnet Karczmarza bieżącego rozszerzenia. Premia jest obliczana, a nie mierzona: z karnetem pokazane PD mogą różnić się o 1 PD na grę (zobacz [FORMULA.md](../FORMULA.md#tavern-pass)). |
| „Poziom” z „−”, liczbą i „+” | Twój poziom na Ścieżce nagród. Kliknij „−” lub „+” albo wpisz liczbę. Wiersz poniżej pokazuje premię i następny próg, na przykład „10% premii PD, jeszcze 2 poz. do 15%”. Premia wynosi 10%, od poziomu 20 – 15%, a od poziomu 70 – 20% (zobacz [FORMULA.md](../FORMULA.md#tavern-pass)). Zaktualizuj poziom, gdy przekroczysz poziom 20 lub 70. |
| „Pokaż obrażenia na nakładce” | Pokazuje lub ukrywa wiersze „Obrażenia łącznie:” i „Obrażenia bohatera:”. Domyślnie włączone. |
| „Sprawdzaj aktualizacje przy starcie HDT” | Włącza lub wyłącza sprawdzanie aktualizacji. Zobacz [Prywatność](#privacy). |

Zakończona gra sama pojawia się w otwartym panelu. Nie musisz klikać „Odśwież”.

Dla okresu „Sesja” wiersz pod paskiem narzędzi podaje sesję, na przykład „Sesja od wt. 2026-11-10, 19:56 (trwa)” albo „(zakończona)”. Sesja może trwać po północy.

<a id="tabs"></a>
### Karty

**Przegląd**

| Kafelek | Znaczenie |
|---|---|
| „PD” | PD zdobyte w okresie |
| „Czas PD” | Czas, który liczył się do PD |
| „Czas gry z ręką startową” | Czas w grach, łącznie z wyborem ręki startowej |
| „Czas online” | Czas, przez który działał Hearthstone. Zobacz niżej. |
| „Gry” | Liczba gier, ze zwycięstwami i porażkami |
| „PD na godzinę” | PD na godzinę czasu PD |
| „PD na godzinę online” | PD na godzinę czasu online, czyli łącznie z menu i kolejką |
| „Odsetek zwycięstw” | Udział wygranych gier |
| „PD wydarzenia” | PD wydarzenia w okresie; widoczne tylko, gdy grano w trakcie wydarzenia |

Czas online jest liczony dla każdej sesji Hearthstone: od uruchomienia Hearthstone do końca ostatniej gry. Dopóki Hearthstone działa, liczy się do teraz. Liczą się tylko sesje z co najmniej jedną grą w okresie.

**Talie**

Jeden wiersz na talię: gry, odsetek zwycięstw, PD, PD na godzinę i średnia długość gry. Talia z najwyższym wynikiem PD na godzinę jest na górze.

Nazwy talii pochodzą ze statystyk HDT. Gra, której HDT nie zapisał, pokazuje „Brak zapisanej talii”.

**Utracone PD**

| Kafelek | Znaczenie |
|---|---|
| „Tury AFK” | PD utracone przez tury AFK: tury bez żadnej akcji, które minęły bez „KONIEC TURY”, i następujące po nich tury przeciwnika |
| „Gry poniżej 1 minuty” | PD, które formuła dałaby za gry krótsze niż 60 sekund, a których gra nie wypłaca |
| „Czas poza grami” | Czas online spędzony poza grami (menu, kolejka, przerwy), wyceniony według twoich PD na godzinę w okresie |

**Gry**

Wszystkie gry z okresu, od najnowszej: data, początek, tryb, wynik, czas PD, czas całkowity, PD, PD wydarzenia, tury AFK, talia i klasa przeciwnika. Kliknij nagłówek kolumny, aby posortować.

**Wykresy**

Dwie krzywe na tej samej osi czasu. Oś zawsze obejmuje cały okres, więc godziny lub dni bez gier też są widoczne:

| Okres | Oś czasu |
|---|---|
| „Sesja” | każda godzina od początku sesji do jej końca albo do teraz, jeśli trwa |
| „Dzisiaj” | 24 godziny dnia |
| „Ten tydzień” | od poniedziałku do niedzieli |
| „Ten sezon” | każdy dzień miesiąca |
| „Ostatnie 30 dni” | ostatnie 30 dni, łącznie z dzisiejszym |
| „Cały czas” | każdy dzień od pierwszej gry w historii |

| Krzywa | Znaczenie |
|---|---|
| „Zdobyte PD” (złota) | PD z gier rozpoczętych w danej godzinie lub danego dnia. Kroki bez gier mają wartość 0. |
| „PD na godzinę” (niebieska) | Tempo w każdej godzinie lub każdego dnia: PD podzielone przez czas PD. Punkt mają tylko kroki z grami. Linia przerywana to średnia z całego okresu. |

Gra liczy się w godzinie lub w dniu, w którym się zaczęła.

<a id="game-history-file"></a>
## Plik historii gier

Każda zakończona gra jest dopisywana do pliku CSV:

```
%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv
```

Plik oddziela pola przecinkami, a jako separatora dziesiętnego używa kropki. Czasy są w czasie lokalnym.

| Kolumna | Zawartość |
|---|---|
| `Session` | Nazwa folderu logów Hearthstone dla sesji, na przykład `Hearthstone_2026_11_08_09_10_51` |
| `Start` | Początek pierwszej tury, `yyyy-MM-ddTHH:mm:ss.fff` |
| `End` | Koniec gry, ten sam format |
| `Mode` | Tryb gry, na przykład `Ranked` |
| `Format` | Format, na przykład Standard lub Dzicz; puste, jeśli tryb nie ma formatu |
| `Result` | Na przykład `Win` lub `Loss` |
| `XpSeconds` | Czas, który liczył się do PD |
| `PlaySeconds` | Czas od pierwszej tury do końca, łącznie z turami AFK |
| `WithMulliganSeconds` | Czas łącznie z wyborem ręki startowej |
| `AfkTurns` | Liczba własnych tur AFK |
| `Xp` | PD obliczone przez wtyczkę; puste dla trybów bez zmierzonych stawek |
| `Deck` | Nazwa talii z HDT |
| `PlayerClass` | Twoja klasa |
| `OpponentClass` | Klasa przeciwnika |
| `EventXp` | PD wydarzenia obliczone przez wtyczkę; puste dla gier poza wydarzeniami. Pliki ze starszych wersji bez tej kolumny nadal są odczytywane, a kolumna zostaje dodana przy następnej grze. |
| `XpBonus` | Premia PD z Karnetu Karczmarza w procentach, obowiązująca w chwili rozegrania gry, na przykład `15`; `0` bez karnetu. Starsze pliki bez tej kolumny są odczytywane jako `0`. |

Kolumnę `Xp` wtyczka oblicza z czasu PD. Sama gra nie zapisuje PD w logu. Po otwarciu panelu wtyczka ponownie oblicza PD wszystkich zapisanych gier według aktualnych stawek, więc poprawiona stawka naprawia też starsze gry. Każda gra zachowuje własną wartość `XpBonus`, więc późniejsza zmiana karnetu lub poziomu nie zmienia starszych gier.

Gra odczytana dwa razy z tego samego logu jest zapisywana tylko raz. Instalacja nowej wersji wtyczki zachowuje plik.

<a id="privacy"></a>
## Prywatność

- Historia gier i ustawienia zostają na twoim komputerze. Wtyczka niczego nie wysyła.
- Jedynym zapytaniem sieciowym jest sprawdzanie aktualizacji. Przy starcie HDT wtyczka raz pyta GitHub, czy istnieje nowsze wydanie. Zapytanie wysyła tylko nazwę i wersję wtyczki, na przykład `HearthstoneXpTracker/0.1.0`. Jak przy każdym zapytaniu w sieci, GitHub widzi twój adres IP.
- Aby wyłączyć sprawdzanie aktualizacji, odznacz w panelu „Sprawdzaj aktualizacje przy starcie HDT”. Ustawienie jest zapisywane w `%APPDATA%\HearthstoneDeckTracker\XpTracker\settings.ini`.

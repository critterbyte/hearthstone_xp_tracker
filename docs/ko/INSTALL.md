# 설치

> 기계 번역을 바탕으로 한 번역입니다. [영어판](../INSTALL.md)과 내용이 다르면 영어판이 우선합니다. 수정 제안을 환영합니다: [이슈 열기](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## 요구 사항

- Windows 10 또는 11. 트래커에는 .NET Framework 4.7.2가 필요하며, 두 버전 모두에 포함되어 있습니다.
- 하스스톤

트래커는 하스스톤의 `Power.log`를 읽습니다. 하스스톤은 자체 설정 파일인 `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`에서 이 로그가 켜져 있을 때만 로그를 기록합니다. 트래커는 처음 시작할 때 이 로그를 켭니다. `[Power]` 섹션을 추가할 뿐, 파일의 다른 부분은 바꾸지 않습니다. 바꾸기 전에 이전 파일의 복사본을 `log.config.bak`으로 저장해 둡니다.

<a id="install"></a>
## 설치하기

1. [Releases 페이지](https://github.com/critterbyte/hearthstone_xp_tracker/releases)에서 최신 릴리스의 ZIP 파일을 내려받습니다. 예: `HearthstoneXpTracker-0.2.0.zip`
2. ZIP 파일을 마우스 오른쪽 버튼으로 클릭하고 *속성*(*Properties*)을 선택한 다음, *차단 해제*(*Unblock*)가 보이면 선택합니다.
3. 원하는 폴더(예: *문서*(*Documents*))에 ZIP을 압축 해제합니다. 다음 파일이 들어 있는 `HearthstoneXpTracker` 폴더가 생깁니다.
   - `HearthstoneXpTracker.exe`: 트래커
   - `HearthstoneXpTracker.exe.config`, `XpTracker.Core.dll`, `LiveCharts.dll`, `LiveCharts.Wpf.dll`, `Newtonsoft.Json.dll`: 트래커에 필요한 파일
   - `INSTALL.txt`, `LICENSE.txt`, `THIRD-PARTY-NOTICES.txt`

   쓰기 권한이 있는 폴더를 고르세요. *Program Files*에 두면 업데이트할 때마다 관리자 권한이 필요합니다.
4. `HearthstoneXpTracker.exe`를 시작합니다. Windows에 "Windows의 PC 보호"(*Windows protected your PC*)가 표시되면 *추가 정보*(*More info*)를 선택한 다음 *실행*(*Run anyway*)을 선택합니다. 트래커에는 아직 서명이 없어서 Windows가 이 프로그램을 알지 못합니다.
5. 대시보드가 열리고, 작업 표시줄 오른쪽 아래의 알림 영역(*notification area*)에 금색 "XP" 아이콘이 나타납니다. 작은 화살표(^) 뒤에 숨겨져 있을 수 있습니다.

처음 시작할 때 하스스톤이 이미 실행 중이었다면 하스스톤을 한 번 다시 시작하세요. 하스스톤은 시작할 때만 `log.config`를 읽습니다.

다음 게임의 첫 턴에 오버레이가 나타납니다. 표시되는 내용은 [USAGE.md](USAGE.md)를 참고하세요.

모든 파일은 한 폴더에 함께 있어야 합니다. 처음 실행하면 대시보드 위쪽의 막대에서 트래커를 시작 메뉴에 추가할지 한 번 묻습니다. Windows에 로그인할 때 트래커를 자동으로 실행하려면 대시보드에서 "Windows 시작 시 실행"에 체크하세요. 나중에 폴더를 옮겼다면 새 위치에서 트래커를 한 번 실행하세요. 그러면 시작 메뉴 바로 가기와 자동 실행 항목이 새 위치로 바뀝니다.

각 릴리스에는 ZIP의 SHA-256 체크섬이 적혀 있습니다. 내려받은 파일을 확인하려면 PowerShell에서 `Get-FileHash HearthstoneXpTracker-0.2.0.zip`을 실행하고 결과를 비교하세요.

<a id="coming-from-the-hdt-plugin"></a>
## HDT 플러그인에서 옮겨 오기

버전 0.1.2까지 트래커는 HDT 플러그인이었습니다. HDT가 2026년 11월 30일에 플러그인 지원을 종료하므로, 이제 트래커는 같은 오버레이와 대시보드를 갖춘 독립 프로그램입니다.

- **데이터는 그대로 옮겨집니다.** 트래커는 시작할 때마다 플러그인 기록(`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`)에서 아직 가지고 있지 않은 게임을 추가합니다. 자체 설정이 아직 없으면 플러그인 설정도 한 번 가져옵니다. 플러그인 파일은 읽기만 하고 절대 바꾸지 않습니다.
- **플러그인을 끄세요.** HDT에서 *Settings*(상단 바 오른쪽의 톱니바퀴 아이콘) *→ Tracker → Plugins*를 열고 **Hearthstone XP Tracker**를 끄세요. 그렇지 않으면 오버레이 두 개가 같은 숫자를 표시합니다. `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` 폴더를 삭제해도 됩니다.
- **당분간 덱 이름은 없습니다.** 덱 이름은 HDT가 제공했으므로 *덱* 탭과 덱 열은 당분간 없어졌습니다. 이전 게임의 덱 이름은 기록 파일에 그대로 남아 있습니다.

<a id="update"></a>
## 업데이트

새 버전이 있으면 트래커가 알려 줍니다([USAGE.md](USAGE.md#update-hint) 참고). 업데이트 방법:

1. 새 릴리스의 ZIP 파일을 내려받고 차단을 해제합니다(위 참고).
2. 트래커를 종료합니다. 금색 "XP" 아이콘을 마우스 오른쪽 버튼으로 클릭하고 "종료"를 선택합니다.
3. ZIP을 다시 같은 폴더에 압축 해제하고 이전 파일을 덮어씁니다.
4. `HearthstoneXpTracker.exe`를 시작합니다.

게임 기록과 설정은 그대로 유지됩니다. 이 데이터는 다른 폴더에 저장됩니다(아래 참고).

이전 버전으로 되돌리려면 그 버전의 ZIP으로 같은 과정을 거치세요.

<a id="uninstall"></a>
## 제거

1. 대시보드에서 "Windows 시작 시 실행"과 "시작 메뉴 바로 가기"의 체크를 해제합니다.
2. 트래커를 종료합니다. 금색 "XP" 아이콘을 마우스 오른쪽 버튼으로 클릭하고 "종료"를 선택합니다.
3. 압축 해제한 `HearthstoneXpTracker` 폴더를 삭제합니다.

트래커는 데이터를 `%APPDATA%\HearthstoneXpTracker`에 저장합니다.

| 파일 | 내용 |
|---|---|
| `hs_games.csv` | 게임 기록 |
| `settings.ini` | 설정 |
| `xptracker.log` | 예기치 않은 오류(있는 경우) |

데이터를 남기고 싶지 않으면 이 폴더도 삭제하세요.

`[Power]` 섹션은 `log.config`에 남습니다. 다른 프로그램도 이 섹션을 사용할 수 있기 때문입니다. 이 섹션은 하스스톤이 로그를 기록하게 할 뿐입니다. 파일을 이전 상태로 되돌리려면 `log.config`를 `log.config.bak`으로 바꾸세요.

<a id="troubleshooting"></a>
## 문제 해결

**Windows에 "Windows의 PC 보호"(*Windows protected your PC*)가 표시됩니다.**

- *추가 정보*(*More info*)를 선택한 다음 *실행*(*Run anyway*)을 선택하세요. Windows는 아직 알지 못하는 프로그램에 이 화면을 표시합니다. 트래커는 오픈 소스이므로 GitHub에서 코드를 읽고 ZIP의 SHA-256 체크섬을 확인할 수 있습니다.

**트래커를 시작해도 아무 일도 일어나지 않습니다.**

- 이미 실행 중일 수 있습니다. 다시 시작하면 실행 중인 트래커의 대시보드가 열립니다. 알림 영역(*notification area*)에서 금색 "XP" 아이콘을 찾아보세요. 작은 화살표(^) 뒤도 확인하세요.
- ZIP의 모든 파일이 같은 폴더에 있는지, ZIP의 차단을 해제했는지 확인하세요([설치하기](#install) 참고).

**오버레이가 나타나지 않습니다.**

- 오버레이는 등급전, 일반전, 선술집 난투, 투기장에서만 표시됩니다. 다른 모드는 아직 측정하지 않았습니다.
- 시작 카드를 고르는 동안에는 오버레이가 숨겨집니다. 첫 턴에 나타납니다.
- 오버레이는 하스스톤이 활성 창일 때만 표시됩니다. 게임 화면을 한 번 클릭하세요.
- 대시보드에서 기간 버튼 아래의 상태 줄을 확인하세요. "게임 로그(Power.log): 켜짐"이라고 표시되어야 합니다. 하스스톤을 다시 시작하라는 안내가 나오면 그렇게 하세요.

**대시보드에 게임 로그를 켜지 못했다고 표시됩니다.**

- 트래커가 `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`에 쓰지 못했습니다. 하스스톤을 닫고 메모장(*Notepad*)에서 파일을 연 다음(없으면 새로 만드세요), 끝에 다음 줄을 추가하고 저장하세요.

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**다른 문제가 있습니다.**

[이슈를 열고](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) 트래커 버전(대시보드 오른쪽 위에 표시됨), Windows 버전, 기대한 화면을 적어 주세요. `%APPDATA%\HearthstoneXpTracker\xptracker.log`가 있으면 마지막 몇 줄도 도움이 됩니다.

<a id="build-from-source"></a>
## 소스에서 빌드하기

[CONTRIBUTING.md](../../CONTRIBUTING.md#building)를 참고하세요.

# 설치

> 기계 번역을 바탕으로 한 번역입니다. [영어판](../INSTALL.md)과 내용이 다르면 영어판이 우선합니다. 수정 제안을 환영합니다: [이슈 열기](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## 요구 사항

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/)(HDT). HDT 1.60.1에서 테스트했습니다.
- 하스스톤

플러그인은 하스스톤의 `Power.log`를 읽습니다. HDT가 이 로그를 직접 켜므로 하스스톤 설정을 바꿀 필요가 없습니다.

<a id="install"></a>
## 설치하기

1. [Releases 페이지](https://github.com/critterbyte/hearthstone_xp_tracker/releases)에서 최신 릴리스의 ZIP 파일을 내려받습니다. 예: `HearthstoneXpTracker-0.1.0.zip`
2. HDT를 닫습니다.
3. ZIP 파일을 마우스 오른쪽 버튼으로 클릭하고 *속성*(*Properties*)을 선택한 다음, *차단 해제*(*Unblock*)가 보이면 선택합니다. 그렇지 않으면 Windows가 플러그인을 차단할 수 있습니다.
4. HDT 플러그인 폴더를 엽니다. 두 가지 방법이 있습니다.
   - HDT에서: *Settings*(상단 바 오른쪽의 톱니바퀴 아이콘) *→ Tracker → Plugins → Plugins Folder*
   - Windows 파일 탐색기에서: 주소 표시줄에 `%APPDATA%\HearthstoneDeckTracker\Plugins`를 입력합니다
5. ZIP을 플러그인 폴더에 압축 해제합니다. 다음 파일이 들어 있는 `HearthstoneXpTracker` 폴더가 생깁니다.
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt`와 `LICENSE.txt`
6. HDT를 시작합니다.
7. *Settings → Tracker → Plugins*를 열고 **Hearthstone XP Tracker**를 켭니다.

다음 게임의 첫 턴에 오버레이가 나타납니다. 표시되는 내용은 [USAGE.md](USAGE.md)를 참고하세요.

두 DLL 파일은 같은 폴더에 있어야 합니다. `XpTracker.Core.dll`이 없으면 플러그인이 시작되지 않습니다.

각 릴리스에는 ZIP의 SHA-256 체크섬이 적혀 있습니다. 내려받은 파일을 확인하려면 PowerShell에서 `Get-FileHash HearthstoneXpTracker-0.1.0.zip`을 실행하고 결과를 비교하세요.

<a id="update"></a>
## 업데이트

새 버전이 있으면 플러그인이 알려 줍니다([USAGE.md](USAGE.md#update-hint) 참고). 업데이트 방법:

1. 새 릴리스의 ZIP 파일을 내려받고 차단을 해제합니다(위 참고).
2. HDT를 닫습니다.
3. ZIP을 다시 플러그인 폴더에 압축 해제하고 이전 파일을 덮어씁니다.
4. HDT를 시작합니다.

게임 기록과 설정은 그대로 유지됩니다. 이 데이터는 다른 폴더에 저장됩니다(아래 참고).

<a id="uninstall"></a>
## 제거

1. HDT에서 *Settings → Tracker → Plugins*를 열고 플러그인을 끕니다.
2. HDT를 닫습니다.
3. `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` 폴더를 삭제합니다.

플러그인은 데이터를 `%APPDATA%\HearthstoneDeckTracker\XpTracker`에 저장합니다.

| 파일 | 내용 |
|---|---|
| `hs_games.csv` | 게임 기록 |
| `settings.ini` | 플러그인 설정 |

데이터를 남기고 싶지 않으면 이 폴더도 삭제하세요.

<a id="troubleshooting"></a>
## 문제 해결

**플러그인이 목록에 없거나, 불러올 때 HDT가 오류를 표시합니다.**

- 두 DLL 파일이 `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`에 있는지, 한 단계 더 깊은 폴더(예: `...\HearthstoneXpTracker\HearthstoneXpTracker`)에 있지 않은지 확인하세요.
- Windows는 인터넷에서 내려받은 파일을 차단할 수 있습니다. 각 DLL을 마우스 오른쪽 버튼으로 클릭하고 *속성*(*Properties*)을 선택한 다음, *차단 해제*(*Unblock*) 확인란이 있으면 선택하고 *OK*를 클릭하세요. 그런 다음 HDT를 다시 시작하세요.

**오버레이가 나타나지 않습니다.**

- 오버레이는 등급전, 일반전, 선술집 난투에서만 표시됩니다. 다른 모드는 아직 측정하지 않았습니다.
- 시작 카드를 고르는 동안에는 오버레이가 숨겨집니다. 첫 턴에 나타납니다.
- HDT 자체 오버레이가 작동하는지 확인하세요. 플러그인은 HDT 오버레이 위에 그려집니다.

**다른 문제가 있습니다.**

[이슈를 열고](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) HDT 버전과 기대한 화면을 적어 주세요.

<a id="build-from-source"></a>
## 소스에서 빌드하기

[CONTRIBUTING.md](../../CONTRIBUTING.md#building)를 참고하세요.

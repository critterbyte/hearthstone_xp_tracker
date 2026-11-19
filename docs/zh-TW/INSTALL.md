# 安裝

> 本文為機器輔助翻譯。如與[英文版](../INSTALL.md)有出入，以英文版為準。歡迎指正：[提出 issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)。

<a id="requirements"></a>
## 需求

- Windows 10 或 11。追蹤器需要 .NET Framework 4.7.2，這兩個版本都已內建。
- 爐石戰記

不需要 Hearthstone Deck Tracker（HDT）。追蹤器可以單獨執行，也可以和 HDT 一起執行。

追蹤器會讀取爐石戰記的 `Power.log`。只有在爐石戰記自己的設定檔 `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config` 中開啟這份記錄檔時，爐石戰記才會寫入它。追蹤器會在第一次啟動時開啟它：加入 `[Power]` 區段，不會更改檔案中的其他內容。變更之前，它會將舊檔案備份為 `log.config.bak`。如果已安裝 HDT，這份記錄檔已經是開啟的。

<a id="install"></a>
## 安裝

1. 從 [Releases 頁面](https://github.com/critterbyte/hearthstone_xp_tracker/releases)下載最新版本的 ZIP 檔，例如 `HearthstoneXpTracker-0.2.0.zip`。
2. 在 ZIP 檔上按右鍵，選擇 *內容*（*Properties*），如果有顯示 *解除封鎖*（*Unblock*），請勾選它。
3. 將 ZIP 解壓縮到你選擇的資料夾，例如 *文件*（*Documents*）。這會建立 `HearthstoneXpTracker` 資料夾，內含這些檔案：
   - `HearthstoneXpTracker.exe`：追蹤器本身
   - `HearthstoneXpTracker.exe.config`、`XpTracker.Core.dll`、`LiveCharts.dll`、`LiveCharts.Wpf.dll` 和 `Newtonsoft.Json.dll`：追蹤器需要的檔案
   - `INSTALL.txt`、`LICENSE.txt` 和 `THIRD-PARTY-NOTICES.txt`

   請選擇你可以寫入的資料夾。如果放在 *Program Files* 中，每次更新都需要系統管理員權限。
4. 啟動 `HearthstoneXpTracker.exe`。如果 Windows 顯示「Windows 已保護您的電腦」（*Windows protected your PC*），請選擇 *其他資訊*（*More info*），再選擇 *仍要執行*（*Run anyway*）。追蹤器目前尚未簽署，所以 Windows 還不認識它。
5. 儀表板會開啟，工作列右下角的通知區域（*notification area*）會出現金色的「XP」圖示。它可能藏在小箭頭（^）後面。

如果第一次啟動時爐石戰記已經在執行，請重新啟動爐石戰記一次。爐石戰記只在啟動時讀取 `log.config`。

覆蓋層會在你下一場對戰的第一個回合出現。它顯示哪些內容，請見 [USAGE.md](USAGE.md)。

所有檔案都必須放在同一個資料夾中。為了更方便地啟動追蹤器，可以在 `HearthstoneXpTracker.exe` 上按右鍵，選擇 *釘選到開始畫面*（*Pin to Start*）或 *傳送到 → 桌面 (建立捷徑)*（*Send to → Desktop (create shortcut)*）。

每個版本都會列出 ZIP 檔的 SHA-256 檢查碼。要檢查你下載的檔案，請在 PowerShell 中執行 `Get-FileHash HearthstoneXpTracker-0.2.0.zip`，並比對結果。

<a id="coming-from-the-hdt-plugin"></a>
## 從 HDT 外掛程式轉移

到 0.1.2 版為止，追蹤器都是 HDT 的外掛程式。HDT 將於 2026 年 11 月 30 日停止支援外掛程式，所以追蹤器現在是一個獨立的程式，覆蓋層和儀表板都和以前一樣。

- **你的資料會一起帶過來。** 每次啟動時，追蹤器都會從外掛程式的記錄（`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`）加入它還沒有的對戰。如果它還沒有自己的設定，也會接收外掛程式的設定一次。外掛程式的檔案只會被讀取，絕不會被變更。
- **停用外掛程式。** 在 HDT 中開啟 *Settings*（頂端列右側的齒輪圖示）*→ Tracker → Plugins*，並停用 **Hearthstone XP Tracker**。否則會有兩個覆蓋層顯示相同的數字。你也可以刪除資料夾 `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`。
- **暫時沒有套牌名稱。** 套牌名稱原本由 HDT 提供，所以 *套牌* 分頁和套牌欄位暫時移除了。較舊的對戰在記錄檔中仍保留套牌名稱。

<a id="update"></a>
## 更新

有新版本時，追蹤器會通知你（請見 [USAGE.md](USAGE.md#update-hint)）。更新方式：

1. 下載新版本的 ZIP 檔，並解除封鎖（請見上方）。
2. 結束追蹤器：在金色的「XP」圖示上按右鍵，選擇「結束」。
3. 再次將 ZIP 解壓縮到同一個資料夾，並取代舊檔案。
4. 啟動 `HearthstoneXpTracker.exe`。

你的對戰記錄和設定都會保留。它們儲存在另一個資料夾中（請見下方）。

要回到較舊的版本，請用該版本的 ZIP 進行同樣的步驟。

<a id="uninstall"></a>
## 解除安裝

1. 結束追蹤器：在金色的「XP」圖示上按右鍵，選擇「結束」。
2. 刪除你解壓縮出來的 `HearthstoneXpTracker` 資料夾。

追蹤器將你的資料儲存在 `%APPDATA%\HearthstoneXpTracker`：

| 檔案 | 內容 |
|---|---|
| `hs_games.csv` | 對戰記錄 |
| `settings.ini` | 設定 |
| `xptracker.log` | 非預期的錯誤（如果有） |

如果你不想保留這些資料，也請刪除這個資料夾。

`[Power]` 區段會保留在 `log.config` 中，因為 HDT 等其他工具也會使用它。它只會讓爐石戰記寫入記錄檔。要將檔案還原成先前的樣子，請用 `log.config.bak` 取代 `log.config`。

<a id="troubleshooting"></a>
## 疑難排解

**Windows 顯示「Windows 已保護您的電腦」（*Windows protected your PC*）。**

- 選擇 *其他資訊*（*More info*），再選擇 *仍要執行*（*Run anyway*）。Windows 會對它還不認識的程式顯示這個訊息。追蹤器是開放原始碼的；你可以在 GitHub 上閱讀程式碼，並檢查 ZIP 檔的 SHA-256 檢查碼。

**啟動追蹤器時沒有任何反應。**

- 它可能已經在執行。再次啟動它會開啟執行中追蹤器的儀表板。請在通知區域中尋找金色的「XP」圖示，也請查看小箭頭（^）後面。
- 確認 ZIP 中的所有檔案都在同一個資料夾中，並且 ZIP 已解除封鎖（請見[安裝](#install)）。

**覆蓋層沒有出現。**

- 覆蓋層只會在排名模式、休閒模式、旅店大亂鬥和競技場中顯示。其他模式尚未實測。
- 在選擇起手的手牌時，覆蓋層會隱藏。它會在第一個回合出現。
- 只有當爐石戰記是使用中的視窗時，才會顯示覆蓋層。請在遊戲中點一下。
- 查看儀表板中期間按鈕下方的狀態行。它應該顯示「遊戲記錄（Power.log）：已開啟」。如果它要求你重新啟動爐石戰記，請照做。

**儀表板顯示無法開啟遊戲記錄。**

- 追蹤器無法寫入 `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`。請關閉爐石戰記，用 *記事本*（*Notepad*）開啟這個檔案（如果檔案不存在，請建立它），在檔案結尾加入以下幾行，然後儲存：

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**其他問題。**

請[提出 issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)，並附上追蹤器的版本（儀表板右上角會顯示）、你的 Windows 版本，以及你原本預期看到的內容。如果 `%APPDATA%\HearthstoneXpTracker\xptracker.log` 存在，它的最後幾行也會有幫助。

<a id="build-from-source"></a>
## 從原始碼建置

請見 [CONTRIBUTING.md](../../CONTRIBUTING.md#building)。

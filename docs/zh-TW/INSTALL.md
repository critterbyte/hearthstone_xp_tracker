# 安裝

> 本文為機器輔助翻譯。如與[英文版](../INSTALL.md)有出入，以英文版為準。歡迎指正：[提出 issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)。

<a id="requirements"></a>
## 需求

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/)（HDT）。已在 HDT 1.60.1 上測試。
- 爐石戰記

外掛程式會讀取爐石戰記的 `Power.log`。HDT 會自行開啟這份記錄檔，所以你不需要更改任何爐石戰記設定。

<a id="install"></a>
## 安裝

1. 從 [Releases 頁面](https://github.com/critterbyte/hearthstone_xp_tracker/releases)下載最新版本的 ZIP 檔，例如 `HearthstoneXpTracker-0.1.0.zip`。
2. 關閉 HDT。
3. 在 ZIP 檔上按右鍵，選擇 *內容*（*Properties*），如果有顯示 *解除封鎖*（*Unblock*），請勾選它。否則 Windows 可能會封鎖外掛程式。
4. 開啟 HDT 的外掛程式資料夾。有兩種方法可以找到它：
   - 在 HDT 中：*Settings*（頂端列右側的齒輪圖示）*→ Tracker → Plugins → Plugins Folder*
   - 在 Windows 檔案總管中：在網址列輸入 `%APPDATA%\HearthstoneDeckTracker\Plugins`
5. 將 ZIP 解壓縮到外掛程式資料夾。這會建立 `HearthstoneXpTracker` 資料夾，內含這些檔案：
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` 和 `LICENSE.txt`
6. 啟動 HDT。
7. 開啟 *Settings → Tracker → Plugins*，並啟用 **Hearthstone XP Tracker**。

覆蓋層會在你下一場對戰的第一個回合出現。它顯示哪些內容，請見 [USAGE.md](USAGE.md)。

兩個 DLL 檔必須放在同一個資料夾中。缺少 `XpTracker.Core.dll` 時，外掛程式無法啟動。

每個版本都會列出 ZIP 檔的 SHA-256 檢查碼。要檢查你下載的檔案，請在 PowerShell 中執行 `Get-FileHash HearthstoneXpTracker-0.1.0.zip`，並比對結果。

<a id="update"></a>
## 更新

有新版本時，外掛程式會通知你（請見 [USAGE.md](USAGE.md#update-hint)）。更新方式：

1. 下載新版本的 ZIP 檔，並解除封鎖（請見上方）。
2. 關閉 HDT。
3. 再次將 ZIP 解壓縮到外掛程式資料夾，並取代舊檔案。
4. 啟動 HDT。

你的對戰記錄和設定都會保留。它們儲存在另一個資料夾中（請見下方）。

<a id="uninstall"></a>
## 解除安裝

1. 在 HDT 中開啟 *Settings → Tracker → Plugins*，並停用外掛程式。
2. 關閉 HDT。
3. 刪除資料夾 `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`。

外掛程式將你的資料儲存在 `%APPDATA%\HearthstoneDeckTracker\XpTracker`：

| 檔案 | 內容 |
|---|---|
| `hs_games.csv` | 對戰記錄 |
| `settings.ini` | 外掛程式設定 |

如果你不想保留這些資料，也請刪除這個資料夾。

<a id="troubleshooting"></a>
## 疑難排解

**外掛程式不在清單中，或 HDT 載入它時顯示錯誤。**

- 確認兩個 DLL 檔都在 `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` 中，而不是再深一層的資料夾（例如 `...\HearthstoneXpTracker\HearthstoneXpTracker`）。
- Windows 可能會封鎖從網路下載的檔案。在每個 DLL 上按右鍵，選擇 *內容*（*Properties*），如果有 *解除封鎖*（*Unblock*）核取方塊，請勾選它並按一下 *確定*（*OK*）。然後重新啟動 HDT。

**覆蓋層沒有出現。**

- 覆蓋層只會在排名模式、休閒模式、旅店大亂鬥和競技場中顯示。其他模式尚未實測。
- 在選擇起手的手牌時，覆蓋層會隱藏。它會在第一個回合出現。
- 確認 HDT 本身的覆蓋層可以正常運作。外掛程式是畫在 HDT 的覆蓋層上。

**安裝較舊的版本後，HDT 仍在執行較新的版本。**

- HDT 不是直接從 `%APPDATA%\HearthstoneDeckTracker\Plugins` 載入外掛程式，而是載入它自己程式資料夾中的副本：`%LOCALAPPDATA%\HearthstoneDeckTracker\app-<HDT 版本>\Plugins`。只有當 `%APPDATA%` 中的檔案比較新時，它才會取代那裡的檔案。較舊版本的檔案日期比較舊，所以 HDT 會保留較新的副本。
- 要回到較舊的版本：關閉 HDT，依照上面的說明安裝較舊的版本，刪除 `%LOCALAPPDATA%\HearthstoneDeckTracker\app-<HDT 版本>\Plugins` 中的 `HearthstoneXpTracker` 資料夾，然後重新啟動 HDT。HDT 會重新複製外掛程式。
- 從 0.1.2 版起，儀表板會在右上角顯示目前執行的版本。
- 更新到較新的版本不受影響。

**其他問題。**

請[提出 issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)，並附上你的 HDT 版本，以及你原本預期看到的內容。

<a id="build-from-source"></a>
## 從原始碼建置

請見 [CONTRIBUTING.md](../../CONTRIBUTING.md#building)。

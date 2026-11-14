# インストール

> 機械翻訳を基にした翻訳です。[英語版](../INSTALL.md)と異なる場合は、英語版が優先されます。修正を歓迎します: [Issue を作成](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)。

<a id="requirements"></a>
## 必要なもの

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/)（HDT）。HDT 1.60.1 で動作を確認しています。
- Hearthstone

このプラグインは Hearthstone の `Power.log` を読み取ります。このログは HDT が自動でオンにするため、Hearthstone の設定を変える必要はありません。

<a id="install"></a>
## インストール手順

1. [Releases ページ](https://github.com/critterbyte/hearthstone_xp_tracker/releases)から最新リリースの ZIP ファイルをダウンロードします。例: `HearthstoneXpTracker-0.1.0.zip`
2. HDT を閉じます。
3. ZIP ファイルを右クリックして *プロパティ*（*Properties*）を選び、*許可する*（*Unblock*）が表示されていればチェックを入れます。そうしないと、Windows がプラグインをブロックすることがあります。
4. HDT のプラグインフォルダーを開きます。方法は 2 つあります。
   - HDT で: *Settings*（上部バーの右にある歯車アイコン）*→ Tracker → Plugins → Plugins Folder*
   - Windows のエクスプローラーで: アドレスバーに `%APPDATA%\HearthstoneDeckTracker\Plugins` と入力します
5. ZIP をプラグインフォルダーに展開します。次のファイルが入った `HearthstoneXpTracker` フォルダーができます。
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` と `LICENSE.txt`
6. HDT を起動します。
7. *Settings → Tracker → Plugins* を開き、**Hearthstone XP Tracker** を有効にします。

次の試合の最初のターンでオーバーレイが表示されます。表示内容は [USAGE.md](USAGE.md) を参照してください。

2 つの DLL ファイルは同じフォルダーに置く必要があります。`XpTracker.Core.dll` がないと、プラグインは起動しません。

各リリースには ZIP の SHA-256 チェックサムが記載されています。ダウンロードしたファイルを確認するには、PowerShell で `Get-FileHash HearthstoneXpTracker-0.1.0.zip` を実行し、結果を比べてください。

<a id="update"></a>
## アップデート

新しいバージョンがあると、プラグインが知らせます（[USAGE.md](USAGE.md#update-hint) を参照）。アップデートの手順:

1. 新しいリリースの ZIP ファイルをダウンロードし、ブロックを解除します（上記を参照）。
2. HDT を閉じます。
3. ZIP をもう一度プラグインフォルダーに展開し、古いファイルを置き換えます。
4. HDT を起動します。

試合の履歴と設定はそのまま残ります。これらは別のフォルダーに保存されています（下記を参照）。

<a id="uninstall"></a>
## アンインストール

1. HDT で *Settings → Tracker → Plugins* を開き、プラグインを無効にします。
2. HDT を閉じます。
3. フォルダー `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` を削除します。

プラグインのデータは `%APPDATA%\HearthstoneDeckTracker\XpTracker` に保存されています。

| ファイル | 内容 |
|---|---|
| `hs_games.csv` | 試合の履歴 |
| `settings.ini` | プラグインの設定 |

データを残したくない場合は、このフォルダーも削除してください。

<a id="troubleshooting"></a>
## トラブルシューティング

**プラグインが一覧にない、または読み込み時に HDT がエラーを表示する。**

- 2 つの DLL ファイルが `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` にあり、1 階層深いフォルダー（例: `...\HearthstoneXpTracker\HearthstoneXpTracker`）に入っていないことを確認してください。
- Windows はインターネットからダウンロードしたファイルをブロックすることがあります。各 DLL を右クリックして *プロパティ*（*Properties*）を選び、*許可する*（*Unblock*）のチェックボックスがあればチェックを入れて *OK* をクリックします。その後、HDT を再起動してください。

**オーバーレイが表示されない。**

- オーバーレイはランク戦、カジュアル、酒場の喧嘩でのみ表示されます。ほかのモードはまだ計測されていません。
- 最初の手札の選択中は、オーバーレイは表示されません。最初のターンで表示されます。
- HDT 自体のオーバーレイが動いているか確認してください。このプラグインは HDT のオーバーレイ上に描画します。

**ほかの問題がある。**

[Issue を作成](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)し、HDT のバージョンと、本来表示されるはずだった内容を書いてください。

<a id="build-from-source"></a>
## ソースからビルドする

[CONTRIBUTING.md](../../CONTRIBUTING.md#building) を参照してください。

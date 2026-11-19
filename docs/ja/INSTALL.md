# インストール

> 機械翻訳を基にした翻訳です。[英語版](../INSTALL.md)と異なる場合は、英語版が優先されます。修正を歓迎します: [Issue を作成](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)。

<a id="requirements"></a>
## 必要なもの

- Windows 10 または 11。トラッカーには .NET Framework 4.7.2 が必要ですが、どちらにも含まれています。
- Hearthstone

Hearthstone Deck Tracker（HDT）は必要ありません。トラッカーは単独でも、HDT と並べても動作します。

トラッカーは Hearthstone の `Power.log` を読み取ります。Hearthstone は、自身の設定ファイル `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config` でオンになっている場合にだけ、このログを書き出します。トラッカーは最初の起動時にこれをオンにします。`[Power]` セクションを追加するだけで、ファイルのほかの部分は変更しません。変更の前に、元のファイルのコピーを `log.config.bak` として保存します。HDT がインストールされていれば、ログはすでにオンになっています。

<a id="install"></a>
## インストール手順

1. [Releases ページ](https://github.com/critterbyte/hearthstone_xp_tracker/releases)から最新リリースの ZIP ファイルをダウンロードします。例: `HearthstoneXpTracker-0.2.0.zip`
2. ZIP ファイルを右クリックして *プロパティ*（*Properties*）を選び、*許可する*（*Unblock*）が表示されていればチェックを入れます。
3. ZIP を好きなフォルダー（例: *ドキュメント*（*Documents*））に展開します。次のファイルが入った `HearthstoneXpTracker` フォルダーができます。
   - `HearthstoneXpTracker.exe`: トラッカー本体
   - `HearthstoneXpTracker.exe.config`、`XpTracker.Core.dll`、`LiveCharts.dll`、`LiveCharts.Wpf.dll`、`Newtonsoft.Json.dll`: トラッカーの動作に必要なファイル
   - `INSTALL.txt`、`LICENSE.txt`、`THIRD-PARTY-NOTICES.txt`

   書き込みできるフォルダーを選んでください。*Program Files* に置くと、アップデートのたびに管理者権限が必要になります。
4. `HearthstoneXpTracker.exe` を起動します。Windows に「Windows によって PC が保護されました」（*Windows protected your PC*）と表示された場合は、*詳細情報*（*More info*）を選んでから *実行*（*Run anyway*）を選びます。トラッカーにはまだ署名がないため、Windows はこのプログラムを知りません。
5. ダッシュボードが開き、タスクバー右下の通知領域（*notification area*）に金色の「XP」アイコンが表示されます。小さな矢印（^）の中に隠れていることがあります。

最初の起動時に Hearthstone がすでに起動していた場合は、Hearthstone を一度再起動してください。Hearthstone は起動時にしか `log.config` を読みません。

次の試合の最初のターンでオーバーレイが表示されます。表示内容は [USAGE.md](USAGE.md) を参照してください。

すべてのファイルは 1 つのフォルダーにまとめて置く必要があります。トラッカーを起動しやすくするには、`HearthstoneXpTracker.exe` を右クリックして *スタートにピン留めする*（*Pin to Start*）または *送る → デスクトップ (ショートカットを作成)*（*Send to → Desktop (create shortcut)*）を選びます。

各リリースには ZIP の SHA-256 チェックサムが記載されています。ダウンロードしたファイルを確認するには、PowerShell で `Get-FileHash HearthstoneXpTracker-0.2.0.zip` を実行し、結果を比べてください。

<a id="coming-from-the-hdt-plugin"></a>
## HDT プラグインからの移行

バージョン 0.1.2 までは、トラッカーは HDT のプラグインでした。HDT は 2026 年 11 月 30 日にプラグインのサポートを終了するため、トラッカーは同じオーバーレイとダッシュボードを持つ単独のプログラムになりました。

- **データは引き継がれます。** トラッカーは起動するたびに、プラグインの履歴（`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`）のうち、まだ持っていない試合を追加します。自分の設定がまだない場合は、プラグインの設定も一度だけ引き継ぎます。プラグインのファイルは読み取るだけで、変更することはありません。
- **プラグインを無効にしてください。** HDT で *Settings*（上部バーの右にある歯車アイコン）*→ Tracker → Plugins* を開き、**Hearthstone XP Tracker** を無効にします。そうしないと、2 つのオーバーレイが同じ数値を表示します。フォルダー `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` を削除してもかまいません。
- **デッキ名は当面ありません。** デッキ名は HDT から取得していたため、*デッキ* タブとデッキ列は当面なくなりました。以前の試合のデッキ名は、履歴ファイルに残ります。

<a id="update"></a>
## アップデート

新しいバージョンがあると、トラッカーが知らせます（[USAGE.md](USAGE.md#update-hint) を参照）。アップデートの手順:

1. 新しいリリースの ZIP ファイルをダウンロードし、ブロックを解除します（上記を参照）。
2. トラッカーを終了します。金色の「XP」アイコンを右クリックし、「終了」を選びます。
3. ZIP をもう一度同じフォルダーに展開し、古いファイルを置き換えます。
4. `HearthstoneXpTracker.exe` を起動します。

試合の履歴と設定はそのまま残ります。これらは別のフォルダーに保存されています（下記を参照）。

古いバージョンに戻すには、そのバージョンの ZIP で同じ手順を行います。

<a id="uninstall"></a>
## アンインストール

1. トラッカーを終了します。金色の「XP」アイコンを右クリックし、「終了」を選びます。
2. 展開した `HearthstoneXpTracker` フォルダーを削除します。

トラッカーのデータは `%APPDATA%\HearthstoneXpTracker` に保存されています。

| ファイル | 内容 |
|---|---|
| `hs_games.csv` | 試合の履歴 |
| `settings.ini` | 設定 |
| `xptracker.log` | 予期しないエラー（発生した場合） |

データを残したくない場合は、このフォルダーも削除してください。

`[Power]` セクションは `log.config` に残ります。HDT などほかのツールもこのセクションを使うためです。このセクションは Hearthstone にログを書き出させるだけです。ファイルを元の状態に戻すには、`log.config` を `log.config.bak` で置き換えてください。

<a id="troubleshooting"></a>
## トラブルシューティング

**Windows に「Windows によって PC が保護されました」（*Windows protected your PC*）と表示される。**

- *詳細情報*（*More info*）を選んでから *実行*（*Run anyway*）を選んでください。Windows は、まだ知らないプログラムに対してこの表示を出します。トラッカーはオープンソースです。GitHub でコードを読むことも、ZIP の SHA-256 チェックサムを確認することもできます。

**トラッカーを起動しても何も起こらない。**

- すでに起動している可能性があります。もう一度起動すると、起動中のトラッカーのダッシュボードが開きます。通知領域（*notification area*）に金色の「XP」アイコンがないか、小さな矢印（^）の中も含めて確認してください。
- ZIP のすべてのファイルが同じフォルダーにあること、ZIP のブロックを解除したことを確認してください（[インストール手順](#install) を参照）。

**オーバーレイが表示されない。**

- オーバーレイはランク戦、カジュアル、酒場の喧嘩、闘技場でのみ表示されます。ほかのモードはまだ計測されていません。
- 最初の手札の選択中は、オーバーレイは表示されません。最初のターンで表示されます。
- オーバーレイは、Hearthstone がアクティブなウィンドウのときだけ表示されます。ゲーム画面を一度クリックしてください。
- ダッシュボードの期間ボタンの下にあるステータス行を確認してください。「ゲームログ（Power.log）：オン」と表示されているはずです。Hearthstone の再起動を求められた場合は、再起動してください。

**ゲームログをオンにできなかったとダッシュボードに表示される。**

- トラッカーが `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config` に書き込めませんでした。Hearthstone を閉じ、メモ帳（*Notepad*）でこのファイルを開き（ない場合は作成します）、末尾に次の行を追加して保存してください。

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**ほかの問題がある。**

[Issue を作成](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)し、トラッカーのバージョン（ダッシュボードの右上に表示されます）、Windows のバージョン、本来表示されるはずだった内容を書いてください。`%APPDATA%\HearthstoneXpTracker\xptracker.log` がある場合は、その最後の数行も役に立ちます。

<a id="build-from-source"></a>
## ソースからビルドする

[CONTRIBUTING.md](../../CONTRIBUTING.md#building) を参照してください。

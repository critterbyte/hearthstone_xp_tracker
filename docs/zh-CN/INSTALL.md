# 安装

> 本文为机器辅助翻译。如与[英文版](../INSTALL.md)有出入，以英文版为准。欢迎指正：[提交 issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)。

<a id="requirements"></a>
## 要求

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/)（HDT）。已在 HDT 1.60.1 上测试。
- 炉石传说

插件会读取炉石传说的 `Power.log`。HDT 会自行开启这个日志，所以你不需要更改任何炉石传说设置。

<a id="install"></a>
## 安装

1. 从 [Releases 页面](https://github.com/critterbyte/hearthstone_xp_tracker/releases)下载最新版本的 ZIP 文件，例如 `HearthstoneXpTracker-0.1.0.zip`。
2. 关闭 HDT。
3. 右键单击 ZIP 文件，选择 *属性*（*Properties*），如果显示 *解除锁定*（*Unblock*），请勾选它。否则 Windows 可能会阻止插件。
4. 打开 HDT 的插件文件夹。有两种方法可以找到它：
   - 在 HDT 中：*Settings*（顶部栏右侧的齿轮图标）*→ Tracker → Plugins → Plugins Folder*
   - 在 Windows 文件资源管理器中：在地址栏输入 `%APPDATA%\HearthstoneDeckTracker\Plugins`
5. 将 ZIP 解压到插件文件夹。这会创建 `HearthstoneXpTracker` 文件夹，其中包含这些文件：
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` 和 `LICENSE.txt`
6. 启动 HDT。
7. 打开 *Settings → Tracker → Plugins*，并启用 **Hearthstone XP Tracker**。

叠加层会在你下一局对局的第一个回合出现。它显示哪些内容，请参阅 [USAGE.md](USAGE.md)。

两个 DLL 文件必须放在同一个文件夹中。缺少 `XpTracker.Core.dll` 时，插件无法启动。

每个版本都会列出 ZIP 文件的 SHA-256 校验和。要检查你下载的文件，请在 PowerShell 中运行 `Get-FileHash HearthstoneXpTracker-0.1.0.zip`，并比对结果。

<a id="update"></a>
## 更新

有新版本时，插件会提示你（请参阅 [USAGE.md](USAGE.md#update-hint)）。更新方法：

1. 下载新版本的 ZIP 文件，并解除锁定（见上文）。
2. 关闭 HDT。
3. 再次将 ZIP 解压到插件文件夹，并替换旧文件。
4. 启动 HDT。

你的对局记录和设置都会保留。它们保存在另一个文件夹中（见下文）。

<a id="uninstall"></a>
## 卸载

1. 在 HDT 中打开 *Settings → Tracker → Plugins*，并禁用插件。
2. 关闭 HDT。
3. 删除文件夹 `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`。

插件将你的数据保存在 `%APPDATA%\HearthstoneDeckTracker\XpTracker`：

| 文件 | 内容 |
|---|---|
| `hs_games.csv` | 对局记录 |
| `settings.ini` | 插件设置 |

如果你不想保留这些数据，也请删除这个文件夹。

<a id="troubleshooting"></a>
## 故障排除

**插件不在列表中，或 HDT 加载它时显示错误。**

- 检查两个 DLL 文件是否都在 `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` 中，而不是再深一层的文件夹（例如 `...\HearthstoneXpTracker\HearthstoneXpTracker`）。
- Windows 可能会阻止从互联网下载的文件。右键单击每个 DLL，选择 *属性*（*Properties*），如果有 *解除锁定*（*Unblock*）复选框，请勾选它并单击 *确定*（*OK*）。然后重新启动 HDT。

**叠加层没有出现。**

- 叠加层只会在排名模式、休闲模式、乱斗模式和竞技模式中显示。其他模式尚未实测。
- 在选择起始手牌时，叠加层会隐藏。它会在第一个回合出现。
- 检查 HDT 自己的叠加层是否正常工作。插件是绘制在 HDT 的叠加层上的。

**其他问题。**

请[提交 issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)，并写明你的 HDT 版本，以及你原本期望看到的内容。

<a id="build-from-source"></a>
## 从源代码构建

请参阅 [CONTRIBUTING.md](../../CONTRIBUTING.md#building)。

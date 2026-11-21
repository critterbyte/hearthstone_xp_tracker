# 安装

> 本文为机器辅助翻译。如与[英文版](../INSTALL.md)有出入，以英文版为准。欢迎指正：[提交 issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)。

<a id="requirements"></a>
## 要求

- Windows 10 或 11。追踪器需要 .NET Framework 4.7.2，这两个版本都已自带。
- 炉石传说

追踪器会读取炉石传说的 `Power.log`。只有在炉石传说自己的设置文件 `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config` 中开启这个日志时，炉石传说才会写入它。追踪器会在首次启动时开启它：添加 `[Power]` 部分，不会更改文件中的其他内容。更改之前，它会把旧文件备份为 `log.config.bak`。

<a id="install"></a>
## 安装

1. 从 [Releases 页面](https://github.com/critterbyte/hearthstone_xp_tracker/releases)下载最新版本的 ZIP 文件，例如 `HearthstoneXpTracker-0.2.0.zip`。
2. 右键单击 ZIP 文件，选择 *属性*（*Properties*），如果显示 *解除锁定*（*Unblock*），请勾选它。
3. 将 ZIP 解压到你选择的文件夹，例如 *文档*（*Documents*）。这会创建 `HearthstoneXpTracker` 文件夹，其中包含这些文件：
   - `HearthstoneXpTracker.exe`：追踪器本身
   - `HearthstoneXpTracker.exe.config`、`XpTracker.Core.dll`、`LiveCharts.dll`、`LiveCharts.Wpf.dll` 和 `Newtonsoft.Json.dll`：追踪器需要的文件
   - `INSTALL.txt`、`LICENSE.txt` 和 `THIRD-PARTY-NOTICES.txt`

   请选择你可以写入的文件夹。如果放在 *Program Files* 中，每次更新都需要管理员权限。
4. 启动 `HearthstoneXpTracker.exe`。如果 Windows 显示“Windows 已保护你的电脑”（*Windows protected your PC*），请选择 *更多信息*（*More info*），再选择 *仍要运行*（*Run anyway*）。追踪器目前尚未签名，所以 Windows 还不认识它。
5. 面板会打开，任务栏右下角的通知区域（*notification area*）会出现金色的“XP”图标。它可能藏在小箭头（^）后面。

如果首次启动时炉石传说已经在运行，请重启一次炉石传说。炉石传说只在启动时读取 `log.config`。

叠加层会在你下一局对局的第一个回合出现。它显示哪些内容，请参阅 [USAGE.md](USAGE.md)。

所有文件都必须放在同一个文件夹中。首次启动时，面板顶部的提示栏会询问一次是否将追踪器添加到“开始”菜单。若要在登录 Windows 时自动启动追踪器，请在面板中勾选“随 Windows 启动”。如果以后移动了文件夹，请从新位置启动一次追踪器，它会更新“开始”菜单快捷方式和自动启动项。

每个版本都会列出 ZIP 文件的 SHA-256 校验和。要检查你下载的文件，请在 PowerShell 中运行 `Get-FileHash HearthstoneXpTracker-0.2.0.zip`，并比对结果。

<a id="coming-from-the-hdt-plugin"></a>
## 从 HDT 插件迁移

截至 0.1.2 版，追踪器一直是 HDT 的插件。HDT 将于 2026 年 11 月 30 日停止支持插件，所以追踪器现在是一个独立的程序，叠加层和面板都和以前一样。

- **你的数据会一起迁移过来。** 每次启动时，追踪器都会从插件的记录（`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`）中添加它还没有的对局。如果它还没有自己的设置，也会接管插件的设置一次。插件的文件只会被读取，绝不会被更改。
- **禁用插件。** 在 HDT 中打开 *Settings*（顶部栏右侧的齿轮图标）*→ Tracker → Plugins*，并禁用 **Hearthstone XP Tracker**。否则会有两个叠加层显示相同的数字。你也可以删除文件夹 `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`。
- **暂时没有套牌名称。** 套牌名称原本由 HDT 提供，所以 *套牌* 标签页和套牌列暂时移除了。较旧的对局在记录文件中仍保留套牌名称。

<a id="update"></a>
## 更新

有新版本时，追踪器会提示你（请参阅 [USAGE.md](USAGE.md#update-hint)）。更新方法：

1. 下载新版本的 ZIP 文件，并解除锁定（见上文）。
2. 退出追踪器：右键单击金色的“XP”图标，选择“退出”。
3. 再次将 ZIP 解压到同一个文件夹，并替换旧文件。
4. 启动 `HearthstoneXpTracker.exe`。

你的对局记录和设置都会保留。它们保存在另一个文件夹中（见下文）。

要回到较旧的版本，请用该版本的 ZIP 执行同样的步骤。

<a id="uninstall"></a>
## 卸载

1. 在面板中取消勾选“随 Windows 启动”和“开始菜单快捷方式”。
2. 退出追踪器：右键单击金色的“XP”图标，选择“退出”。
3. 删除你解压出来的 `HearthstoneXpTracker` 文件夹。

追踪器将你的数据保存在 `%APPDATA%\HearthstoneXpTracker`：

| 文件 | 内容 |
|---|---|
| `hs_games.csv` | 对局记录 |
| `settings.ini` | 设置 |
| `xptracker.log` | 意外错误（如果有） |

如果你不想保留这些数据，也请删除这个文件夹。

`[Power]` 部分会保留在 `log.config` 中，因为其他程序也可能会使用它。它只会让炉石传说写入日志。要将文件恢复成之前的样子，请用 `log.config.bak` 替换 `log.config`。

<a id="troubleshooting"></a>
## 故障排除

**Windows 显示“Windows 已保护你的电脑”（*Windows protected your PC*）。**

- 选择 *更多信息*（*More info*），再选择 *仍要运行*（*Run anyway*）。Windows 会对它还不认识的程序显示这个提示。追踪器是开源的；你可以在 GitHub 上阅读代码，并检查 ZIP 文件的 SHA-256 校验和。

**启动追踪器时没有任何反应。**

- 它可能已经在运行。再次启动它会打开正在运行的追踪器的面板。请在通知区域中寻找金色的“XP”图标，也请查看小箭头（^）后面。
- 检查 ZIP 中的所有文件是否都在同一个文件夹中，以及 ZIP 是否已解除锁定（请参阅[安装](#install)）。

**叠加层没有出现。**

- 叠加层只会在排名模式、休闲模式、乱斗模式和竞技模式中显示。其他模式尚未实测。
- 在选择起始手牌时，叠加层会隐藏。它会在第一个回合出现。
- 只有当炉石传说是活动窗口时，才会显示叠加层。请在游戏中单击一下。
- 查看面板中时段按钮下方的状态行。它应该显示“游戏日志（Power.log）：已开启”。如果它要求你重启炉石传说，请照做。

**面板显示无法开启游戏日志。**

- 追踪器无法写入 `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`。请关闭炉石传说，用 *记事本*（*Notepad*）打开这个文件（如果文件不存在，请创建它），在文件末尾添加以下几行，然后保存：

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**其他问题。**

请[提交 issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)，并写明追踪器的版本（面板右上角会显示）、你的 Windows 版本，以及你原本期望看到的内容。如果 `%APPDATA%\HearthstoneXpTracker\xptracker.log` 存在，它的最后几行也会有帮助。

<a id="build-from-source"></a>
## 从源代码构建

请参阅 [CONTRIBUTING.md](../../CONTRIBUTING.md#building)。

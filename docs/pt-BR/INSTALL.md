# Instalação

> Tradução assistida por máquina. Se houver diferença em relação à [versão em inglês](../INSTALL.md), vale a versão em inglês. Correções são bem-vindas: [abra uma issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Requisitos

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT). Testado com o HDT 1.60.1.
- Hearthstone

O plugin lê o `Power.log` do Hearthstone. O HDT ativa esse log sozinho, então você não precisa mudar nenhuma configuração do Hearthstone.

<a id="install"></a>
## Instalar

1. Baixe o arquivo ZIP da versão mais recente na [página de Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), por exemplo `HearthstoneXpTracker-0.1.0.zip`.
2. Feche o HDT.
3. Clique com o botão direito no arquivo ZIP, escolha *Propriedades* e marque *Desbloquear*, se aparecer. Caso contrário, o Windows pode bloquear o plugin.
4. Abra a pasta de plugins do HDT. Você pode encontrá-la de duas formas:
   - No HDT: *Settings* (o ícone de engrenagem à direita da barra superior) *→ Tracker → Plugins → Plugins Folder*
   - No Explorador de Arquivos do Windows: digite `%APPDATA%\HearthstoneDeckTracker\Plugins` na barra de endereço
5. Extraia o ZIP na pasta de plugins. Isso cria a pasta `HearthstoneXpTracker` com estes arquivos:
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` e `LICENSE.txt`
6. Inicie o HDT.
7. Abra *Settings → Tracker → Plugins* e ative o **Hearthstone XP Tracker**.

O overlay aparece no primeiro turno da sua próxima partida. Veja em [USAGE.md](USAGE.md) o que ele mostra.

Os dois arquivos DLL precisam estar na mesma pasta. Sem o `XpTracker.Core.dll` o plugin não inicia.

Cada versão informa a soma de verificação SHA-256 do ZIP. Para conferir o seu download, execute `Get-FileHash HearthstoneXpTracker-0.1.0.zip` no PowerShell e compare o resultado.

<a id="update"></a>
## Atualizar

Quando existe uma versão mais nova, o plugin avisa você (veja [USAGE.md](USAGE.md#update-hint)). Para atualizar:

1. Baixe o arquivo ZIP da nova versão e desbloqueie-o (veja acima).
2. Feche o HDT.
3. Extraia o ZIP de novo na pasta de plugins e substitua os arquivos antigos.
4. Inicie o HDT.

O seu histórico de partidas e as suas configurações são mantidos. Eles ficam em outra pasta (veja abaixo).

<a id="uninstall"></a>
## Desinstalar

1. No HDT, abra *Settings → Tracker → Plugins* e desative o plugin.
2. Feche o HDT.
3. Exclua a pasta `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.

O plugin guarda os seus dados em `%APPDATA%\HearthstoneDeckTracker\XpTracker`:

| Arquivo | Conteúdo |
|---|---|
| `hs_games.csv` | Histórico de partidas |
| `settings.ini` | Configurações do plugin |

Exclua também essa pasta se não quiser manter os dados.

<a id="troubleshooting"></a>
## Solução de problemas

**O plugin não aparece na lista, ou o HDT mostra um erro ao carregá-lo.**

- Verifique se os dois arquivos DLL estão em `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`, e não uma pasta abaixo (por exemplo `...\HearthstoneXpTracker\HearthstoneXpTracker`).
- O Windows pode bloquear arquivos baixados da internet. Clique com o botão direito em cada DLL, escolha *Propriedades* e, se houver uma caixa *Desbloquear*, marque-a e clique em *OK*. Depois reinicie o HDT.

**O overlay não aparece.**

- O overlay só é mostrado em Ranqueado, Casual, Contenda de Taverna e Arena. Os outros modos ainda não são medidos.
- Durante a escolha da mão inicial, o overlay fica oculto. Ele aparece no primeiro turno.
- Verifique se o overlay do próprio HDT funciona. O plugin desenha sobre o overlay do HDT.

**Outro problema.**

[Abra uma issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) e informe a sua versão do HDT e o que você esperava ver.

<a id="build-from-source"></a>
## Compilar a partir do código-fonte

Veja [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

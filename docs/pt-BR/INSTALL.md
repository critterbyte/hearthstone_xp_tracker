# Instalação

> Tradução assistida por máquina. Se houver diferença em relação à [versão em inglês](../INSTALL.md), vale a versão em inglês. Correções são bem-vindas: [abra uma issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

<a id="requirements"></a>
## Requisitos

- Windows 10 ou 11. O tracker precisa do .NET Framework 4.7.2, que já faz parte dos dois.
- Hearthstone

O tracker lê o `Power.log` do Hearthstone. O Hearthstone só grava esse log quando ele está ativado no seu próprio arquivo de configuração, `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. O tracker o ativa na primeira vez que inicia: ele adiciona a seção `[Power]` e não muda mais nada no arquivo. Antes dessa mudança, ele guarda uma cópia do arquivo antigo como `log.config.bak`.

<a id="install"></a>
## Instalar

1. Baixe o arquivo ZIP da versão mais recente na [página de Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases), por exemplo `HearthstoneXpTracker-0.2.0.zip`.
2. Clique com o botão direito no arquivo ZIP, escolha *Propriedades* (*Properties*) e marque *Desbloquear* (*Unblock*), se aparecer.
3. Extraia o ZIP em uma pasta de sua escolha, por exemplo *Documentos* (*Documents*). Isso cria a pasta `HearthstoneXpTracker` com estes arquivos:
   - `HearthstoneXpTracker.exe`: o tracker
   - `HearthstoneXpTracker.exe.config`, `XpTracker.Core.dll`, `LiveCharts.dll`, `LiveCharts.Wpf.dll` e `Newtonsoft.Json.dll`: necessários para o tracker
   - `INSTALL.txt`, `LICENSE.txt` e `THIRD-PARTY-NOTICES.txt`

   Escolha uma pasta em que você possa gravar. Em *Arquivos de Programas* (*Program Files*), cada atualização precisa de direitos de administrador.
4. Inicie o `HearthstoneXpTracker.exe`. Se o Windows mostrar "O Windows protegeu o computador" (*Windows protected your PC*), escolha *Mais informações* (*More info*) e depois *Executar assim mesmo* (*Run anyway*). O tracker ainda não é assinado, então o Windows não o conhece.
5. O painel abre, e um ícone dourado "XP" aparece na área de notificação (*notification area*), no canto inferior direito da barra de tarefas. Ele pode estar escondido atrás da pequena seta (^).

Se o Hearthstone já estava aberto na primeira vez que o tracker iniciou, reinicie o Hearthstone uma vez. O Hearthstone só lê o `log.config` quando inicia.

O overlay aparece no primeiro turno da sua próxima partida. Veja em [USAGE.md](USAGE.md) o que ele mostra.

Todos os arquivos devem ficar juntos em uma pasta. Na primeira inicialização, uma barra no topo do painel pergunta uma vez se você quer adicionar o tracker ao menu Iniciar. Para que o tracker inicie automaticamente quando você entra no Windows, marque "Iniciar com o Windows" no painel. Se você mover a pasta depois, abra o tracker uma vez a partir do novo local; ele então atualiza o atalho do menu Iniciar e a entrada de inicialização automática.

Cada versão informa a soma de verificação SHA-256 do ZIP. Para conferir o seu download, execute `Get-FileHash HearthstoneXpTracker-0.2.0.zip` no PowerShell e compare o resultado.

<a id="coming-from-the-hdt-plugin"></a>
## Vindo do plugin do HDT

Até a versão 0.1.2, o tracker era um plugin do HDT. O HDT deixa de dar suporte a plugins em 30 de novembro de 2026, então agora o tracker é um programa próprio, com o mesmo overlay e o mesmo painel.

- **Os seus dados vêm junto.** A cada início, o tracker adiciona as partidas do histórico do plugin (`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`) que ele ainda não tem. Ele também assume uma vez as configurações do plugin, se ainda não tiver configurações próprias. Os arquivos do plugin são apenas lidos, nunca alterados.
- **Desative o plugin.** No HDT, abra *Settings* (o ícone de engrenagem à direita da barra superior) *→ Tracker → Plugins* e desative o **Hearthstone XP Tracker**. Caso contrário, dois overlays mostram os mesmos números. Você também pode excluir a pasta `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.
- **Por enquanto, sem nomes de deck.** Os nomes dos decks vinham do HDT, então a aba *Decks* e a coluna de deck deixaram de existir por enquanto. As partidas antigas mantêm os nomes dos decks no arquivo de histórico.

<a id="update"></a>
## Atualizar

Quando existe uma versão mais nova, o tracker avisa você (veja [USAGE.md](USAGE.md#update-hint)). Para atualizar:

1. Baixe o arquivo ZIP da nova versão e desbloqueie-o (veja acima).
2. Feche o tracker: clique com o botão direito no ícone dourado "XP" e escolha "Sair".
3. Extraia o ZIP de novo na mesma pasta e substitua os arquivos antigos.
4. Inicie o `HearthstoneXpTracker.exe`.

O seu histórico de partidas e as suas configurações são mantidos. Eles ficam em outra pasta (veja abaixo).

Para voltar a uma versão mais antiga, faça o mesmo com o ZIP dela.

<a id="uninstall"></a>
## Desinstalar

1. No painel, desmarque "Iniciar com o Windows" e "Atalho no menu Iniciar".
2. Feche o tracker: clique com o botão direito no ícone dourado "XP" e escolha "Sair".
3. Exclua a pasta `HearthstoneXpTracker` que você extraiu.

O tracker guarda os seus dados em `%APPDATA%\HearthstoneXpTracker`:

| Arquivo | Conteúdo |
|---|---|
| `hs_games.csv` | Histórico de partidas |
| `settings.ini` | Configurações |
| `xptracker.log` | Erros inesperados, se houver |

Exclua também essa pasta se não quiser manter os dados.

A seção `[Power]` continua no `log.config`, porque outros programas também podem usá-la. Ela só faz o Hearthstone gravar o seu log. Para deixar o arquivo como era antes, substitua o `log.config` pelo `log.config.bak`.

<a id="troubleshooting"></a>
## Solução de problemas

**O Windows mostra "O Windows protegeu o computador" (*Windows protected your PC*).**

- Escolha *Mais informações* (*More info*) e depois *Executar assim mesmo* (*Run anyway*). O Windows mostra isso para programas que ainda não conhece. O tracker é de código aberto; você pode ler o código no GitHub e conferir a soma de verificação SHA-256 do ZIP.

**Nada acontece quando eu inicio o tracker.**

- Talvez ele já esteja em execução. Iniciá-lo de novo abre o painel do tracker que já está em execução. Procure o ícone dourado "XP" na área de notificação (*notification area*), também atrás da pequena seta (^).
- Verifique se todos os arquivos do ZIP estão na mesma pasta e se o ZIP foi desbloqueado (veja [Instalar](#install)).

**O overlay não aparece.**

- O overlay só é mostrado em Ranqueado, Casual, Contenda de Taverna e Arena. Os outros modos ainda não são medidos.
- Durante a escolha da mão inicial, o overlay fica oculto. Ele aparece no primeiro turno.
- O overlay só é mostrado enquanto o Hearthstone é a janela ativa. Clique uma vez no jogo.
- Veja a linha de status no painel, abaixo dos botões de período. Ela deve dizer "Log do jogo (Power.log): ativado". Se ela pedir para reiniciar o Hearthstone, faça isso.

**O painel diz que não foi possível ativar o log do jogo.**

- O tracker não conseguiu gravar `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. Feche o Hearthstone, abra o arquivo no Bloco de Notas (*Notepad*) (crie-o se ele não existir), adicione estas linhas no final e salve:

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**Outro problema.**

[Abra uma issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) e informe a versão do tracker (o painel a mostra no canto superior direito), a sua versão do Windows e o que você esperava ver. Se `%APPDATA%\HearthstoneXpTracker\xptracker.log` existir, as últimas linhas dele também ajudam.

<a id="build-from-source"></a>
## Compilar a partir do código-fonte

Veja [CONTRIBUTING.md](../../CONTRIBUTING.md#building).

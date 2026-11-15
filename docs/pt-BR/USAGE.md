# Uso

> Tradução assistida por máquina. Se houver diferença em relação à [versão em inglês](../USAGE.md), vale a versão em inglês. Correções são bem-vindas: [abra uma issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).

O plugin tem três partes:

- o **overlay** no jogo
- a janela do **painel**
- o arquivo de **histórico de partidas**

Os textos entre aspas, como "Tempo de EXP:", são os textos que o plugin mostra.

<a id="overlay"></a>
## Overlay

O overlay é uma pequena caixa no canto inferior direito, acima do botão de configurações do Hearthstone (a engrenagem no canto inferior direito). No painel, você pode movê-lo para o canto inferior esquerdo, acima do seu retrato ("You" com o seu ranque). Os rótulos ficam à esquerda. Os valores ficam à direita, em barras azuis.

O overlay só é mostrado em **Ranqueado, Casual, Contenda de Taverna e Arena**. Os outros modos ainda não são medidos, então o plugin não mostra nada neles. Uma Contenda de Taverna contra o computador (JxA, por exemplo contra o Estalajadeiro) não dá EXP, então o overlay mostra "+0" nela.

<a id="during-a-game"></a>
### Durante uma partida

As linhas aparecem no primeiro turno. Durante a escolha da mão inicial, o overlay fica oculto.

| Linha | Significado |
|---|---|
| "Tempo de EXP:" | O tempo que conta para a EXP até agora, em minutos e segundos |
| "Se vencer:" | A EXP que você recebe se vencer agora |
| "Se perder:" | A EXP que você recebe se perder ou desistir agora |

O tempo de EXP pode ter uma observação:

| Observação | Significado |
|---|---|
| "(pausado)" | Você deixou o seu turno acabar sem nenhuma ação. Esse turno e o próximo turno do oponente não contam, então o tempo de EXP voltou e está esperando. Ele volta a correr no seu próximo turno. |
| "(máx.)" | O tempo de EXP chegou a 30 minutos. Mais tempo não dá mais EXP. |

Durante um evento do Hearthstone, ative "Evento ativo (mostrar EXP de Evento)" no painel. O overlay passa a mostrar mais uma linha, ao vivo e no resumo após a partida:

| Linha | Significado |
|---|---|
| "EXP de Evento:" | A EXP de Evento até agora, com um cristal vermelho: uma por minuto de tempo de EXP (veja [FORMULA.md](../FORMULA.md#event-xp)). Não depende do resultado. |

Desative quando o evento terminar.

Com "Passe da Taverna (bônus de EXP)" ativado no painel, "Se vencer:", "Se perder:" e "Recebida:" incluem o bônus do passe para o seu nível na Trilha da Recompensa. A EXP de Evento não recebe bônus.

As duas últimas linhas mostram o dano, ao vivo e no resumo após a partida. Desative-as com "Mostrar o dano no overlay" no painel.

| Linha | Significado |
|---|---|
| "Dano total:" | Todo o dano que o seu lado causou ao herói e aos lacaios do oponente até agora: os seus ataques, os ataques dos seus lacaios, feitiços, armas, Poder Heroico e outros efeitos. Cada acerto conta com o valor que o jogo mostra, também quando é maior que a vida do alvo ou atinge armadura. Dano ao seu próprio herói ou aos seus lacaios não conta, nem dano sem origem, como a fadiga. Um acerto em um Escudo Divino conta 0. |
| "Dano ao herói:" | A parte do "Dano total:" que atingiu o herói do oponente, incluindo a parte absorvida pela armadura |

O dano é lido quando o servidor confirma uma ação, então ele pode subir um instante antes da animação no tabuleiro.

Enquanto o tempo de EXP estiver abaixo de 60 segundos, os dois valores de EXP mostram "+0". Partidas com menos de um minuto não dão EXP. Veja as regras em [FORMULA.md](../FORMULA.md).

Se você iniciar o HDT no meio de uma partida, o plugin ainda lê a partida desde o início.

<a id="after-a-game"></a>
### Após uma partida

Após a partida, o overlay mostra um resumo até a mão inicial da sua próxima partida:

| Linha | Significado |
|---|---|
| "Última partida:" | Vitória ou Derrota |
| "Tempo de EXP:" | O tempo que contou para a EXP |
| "Tempo total:" | A partida inteira, incluindo a mão inicial |
| "Recebida:" | A EXP que a partida deu |

<a id="update-hint"></a>
### Aviso de atualização

Quando existe uma versão mais nova do plugin, o overlay mostra mais uma linha enquanto nenhuma partida está em andamento:

| Linha | Significado |
|---|---|
| "Atualização disponível:" | O número da nova versão, por exemplo 0.2.0 |

O painel mostra um link para a página de download. Veja em [INSTALL.md](INSTALL.md#update) como atualizar.

<a id="dashboard"></a>
## Painel

A janela do painel se chama "HS XP Tracker". Abra-a no HDT de uma destas duas formas:

- menu *Plugins* → "Painel de EXP"
- *Settings* (o ícone de engrenagem à direita da barra superior) *→ Tracker → Plugins* → botão "Painel de EXP" do plugin

<a id="toolbar"></a>
### Barra de ferramentas

| Controle | Significado |
|---|---|
| "Sessão" | Partidas da sessão atual do Hearthstone, desde que o jogo é aberto até ele ser fechado. Depois de fechar o Hearthstone, a última sessão continua visível até o próximo início. É o período mostrado quando o painel abre. |
| "Hoje" | Partidas desde a meia-noite |
| "Esta semana" | Partidas desde segunda-feira, 00:00 |
| "Esta temporada" | Partidas desde o dia 1º do mês. As temporadas do Ranqueado no Hearthstone duram um mês do calendário. |
| "Últimos 30 dias" | Partidas dos últimos 30 dias, incluindo hoje |
| "Tudo" | Todas as partidas do histórico |
| "Atualizar" | Lê o histórico de novo |
| "Abrir pasta de dados" | Abre a pasta do arquivo de histórico no Explorador de Arquivos do Windows |
| "Idioma (Language)" | Idioma do overlay, do painel e da entrada do plugin no menu. O padrão é inglês. A mudança vale na hora; o painel abre de novo no novo idioma. Nos idiomas que não são o inglês, o rótulo mantém a palavra em inglês, como aqui: "Idioma (Language)". |
| "Posição do overlay" | Onde o overlay fica: "À direita, acima do botão de opções" (padrão) ou "À esquerda, acima do seu retrato", assim a lista do deck do HDT à direita fica livre. O overlay muda na hora. |
| "Evento ativo (mostrar EXP de Evento)" | Ative durante um evento do Hearthstone: o overlay mostra a EXP de Evento e as novas partidas a salvam. |
| "Passe da Taverna (bônus de EXP)" | Ative se você tem o Passe da Taverna da expansão atual. O bônus é calculado, não medido: com o passe, a EXP mostrada pode errar por 1 EXP por partida (veja [FORMULA.md](../FORMULA.md#tavern-pass)). |
| "Nível" com "−", um número e "+" | O seu nível na Trilha da Recompensa. Clique em "−" ou "+", ou digite o número. A linha abaixo mostra o bônus e o próximo degrau, por exemplo "10% de bônus de EXP, faltam 2 níveis para 15%". O bônus é de 10%, a partir do nível 20 de 15% e a partir do nível 70 de 20% (veja [FORMULA.md](../FORMULA.md#tavern-pass)). Atualize o nível quando passar do nível 20 ou 70. |
| "Mostrar o dano no overlay" | Mostra ou oculta as linhas "Dano total:" e "Dano ao herói:". Ativado por padrão. |
| "Procurar atualizações ao iniciar o HDT" | Liga ou desliga a verificação de atualizações. Veja [Privacidade](#privacy). |

Uma partida concluída aparece sozinha no painel aberto. Você não precisa clicar em "Atualizar".

Em "Sessão", uma linha abaixo da barra de ferramentas indica a sessão, por exemplo "Sessão desde ter 2026-11-10, 19:56 (em andamento)" ou "(encerrada)". Uma sessão pode passar da meia-noite.

<a id="tabs"></a>
### Abas

**Visão geral**

| Bloco | Significado |
|---|---|
| "EXP" | EXP recebida no período |
| "Tempo de EXP" | Tempo que contou para a EXP |
| "Tempo de jogo com mão inicial" | Tempo em partidas, incluindo a mão inicial |
| "Tempo on-line" | Tempo em que o Hearthstone ficou aberto. Veja abaixo. |
| "Partidas" | Número de partidas, com vitórias e derrotas |
| "EXP por hora" | EXP por hora de tempo de EXP |
| "EXP por hora on-line" | EXP por hora de tempo on-line, ou seja, incluindo menus e fila |
| "Taxa de vitórias" | Parcela de partidas vencidas |
| "EXP de Evento" | EXP de Evento do período; só aparece quando houve partidas durante um evento |

O tempo on-line é contado por sessão do Hearthstone: desde o início do Hearthstone até o fim da última partida. Enquanto o Hearthstone está aberto, ele conta até agora. Só contam as sessões com pelo menos uma partida no período.

**Decks**

Uma linha por deck: partidas, taxa de vitórias, EXP, EXP por hora e a duração média das partidas. O deck com mais EXP por hora fica no topo.

Os nomes dos decks vêm das estatísticas do próprio HDT. Uma partida que o HDT não registrou mostra "Nenhum deck registrado".

**EXP perdida**

| Bloco | Significado |
|---|---|
| "Turnos AFK" | EXP que você perdeu por turnos AFK: turnos sem nenhuma ação que acabaram sem "FIM DO TURNO", e os turnos do oponente logo depois deles |
| "Partidas com menos de 1 minuto" | EXP que a fórmula daria para partidas com menos de 60 segundos, que o jogo não paga |
| "Tempo fora de partidas" | Tempo on-line passado fora de partidas (menus, fila, pausas), avaliado pela sua EXP por hora no período |

**Partidas**

Todas as partidas do período, das mais novas para as mais antigas: data, início, modo, resultado, tempo de EXP, tempo total, EXP, EXP de Evento, turnos AFK, deck e classe do oponente. Clique no cabeçalho de uma coluna para ordenar.

**Gráficos**

Duas curvas no mesmo eixo de tempo. O eixo sempre cobre o período inteiro, então horas ou dias sem partidas continuam visíveis:

| Período | Eixo de tempo |
|---|---|
| "Sessão" | cada hora desde o início da sessão até o fim, ou até agora enquanto ela está em andamento |
| "Hoje" | as 24 horas do dia |
| "Esta semana" | de segunda a domingo |
| "Esta temporada" | cada dia do mês |
| "Últimos 30 dias" | os últimos 30 dias, incluindo hoje |
| "Tudo" | cada dia desde a primeira partida do histórico |

| Curva | Significado |
|---|---|
| "EXP recebida" (dourada) | EXP das partidas iniciadas em cada hora ou em cada dia. Intervalos sem partidas ficam em 0. |
| "EXP por hora" (azul) | Ritmo em cada hora ou em cada dia: EXP dividida pelo tempo de EXP. Só os intervalos com partidas têm um ponto. A linha tracejada é a média de todo o período. |

Uma partida conta na hora ou no dia em que começou.

<a id="game-history-file"></a>
## Arquivo de histórico de partidas

Cada partida concluída é adicionada a um arquivo CSV:

```
%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv
```

O arquivo usa vírgulas entre os campos e ponto como separador decimal. Os horários estão no horário local.

| Coluna | Conteúdo |
|---|---|
| `Session` | Nome da pasta de logs do Hearthstone da sessão, por exemplo `Hearthstone_2026_11_08_09_10_51` |
| `Start` | Início do primeiro turno, `yyyy-MM-ddTHH:mm:ss.fff` |
| `End` | Fim da partida, mesmo formato |
| `Mode` | Modo de jogo, por exemplo `Ranked` |
| `Format` | Formato, por exemplo Standard ou Wild; vazio se o modo não tiver formato |
| `Result` | Por exemplo `Win` ou `Loss` |
| `XpSeconds` | Tempo que contou para a EXP |
| `PlaySeconds` | Tempo do primeiro turno até o fim, incluindo os turnos AFK |
| `WithMulliganSeconds` | Tempo incluindo a mão inicial |
| `AfkTurns` | Número de turnos AFK seus |
| `Xp` | EXP calculada pelo plugin; vazia para modos sem taxas medidas |
| `Deck` | Nome do deck no HDT |
| `PlayerClass` | A sua classe |
| `OpponentClass` | A classe do oponente |
| `EventXp` | EXP de Evento calculada pelo plugin; vazia para partidas fora de eventos. Arquivos de versões antigas sem essa coluna continuam sendo lidos, e a coluna é adicionada com a próxima partida. |
| `XpBonus` | Bônus de EXP do Passe da Taverna, em porcentagem, que valia quando a partida foi jogada, por exemplo `15`; `0` sem o passe. Arquivos antigos sem essa coluna são lidos como `0`. |

A coluna `Xp` é calculada pelo plugin a partir do tempo de EXP. O próprio jogo não registra a EXP em log. Quando o painel abre, o plugin recalcula a EXP de todas as partidas salvas com as taxas atuais, então uma taxa corrigida também corrige partidas antigas. Cada partida mantém o seu próprio `XpBonus`, então mudar o passe ou o nível depois não altera partidas antigas.

Uma partida lida duas vezes do mesmo log é salva só uma vez. Instalar uma nova versão do plugin mantém o arquivo.

<a id="privacy"></a>
## Privacidade

- O histórico de partidas e as configurações ficam no seu computador. O plugin não envia nada.
- A única requisição de rede é a verificação de atualizações. Quando o HDT inicia, o plugin pergunta uma vez ao GitHub se existe uma versão mais nova. A requisição envia só o nome e a versão do plugin, por exemplo `HearthstoneXpTracker/0.1.0`. Como em qualquer requisição web, o GitHub vê o seu endereço IP.
- Para desligar a verificação de atualizações, desmarque "Procurar atualizações ao iniciar o HDT" no painel. A configuração fica salva em `%APPDATA%\HearthstoneDeckTracker\XpTracker\settings.ini`.

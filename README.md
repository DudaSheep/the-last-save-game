# The Last Save 💀🎮

**The Last Save** é um jogo de ação e plataforma 2D desenvolvido na engine **Unity** como projeto acadêmico para a disciplina de Jogos Digitais (UFRPE). O jogo combina mecânicas clássicas de plataforma com combates intensos de *Boss Rush*, desafiando o jogador a salvar a própria história dos videogames.

---

## 📜 História e Conceito

No papel da **Dona Morte**, o jogador assume uma missão incomum: em vez de ceifar vidas humanas, sua tarefa é resgatar **jogos eletrônicos esquecidos, cancelados ou cujos servidores foram desligados** antes que eles caiam no esquecimento absoluto (o limbo digital).

A jornada culmina em batalhas épicas contra chefes que manifestam a frustração e os dados corrompidos desses projetos abandonados, sendo o desafio final o temível **O Amalgamado** — uma criatura moldada por restos de códigos e assets esquecidos no subsolo do desenvolvimento.

---

## 🛠️ Especificações Técnicas e Arquitetura

O projeto foi construído seguindo boas práticas de desenvolvimento de jogos em 2D na Unity, com foco em modularidade, desempenho na Web/PC e persistência de dados.

* **Engine:** Unity (Suporte a builds WebGL e PC Standalone).
* **Paradigma:** Arquitetura orientada a componentes (Component-Based).
* **Gerenciamento de Estados (IA do Boss):** Máquina de Estados Finitos (FSM) controlada por gatilhos de vida e transições assíncronas baseadas em Corrotinas.
* **Persistência de Dados:** Uso de `PlayerPrefs` para um sistema dinâmico de **Boss Checkpoints** para transição segura entre a cena do menu de GameOver e a arena do chefe.
* **Padrões de Projeto (Design Patterns):** Implementação de *Singletons* estruturados para gerenciadores globais (`GameController`, `StageManager`) com persistência controlada e travas de segurança contra duplicidade de instâncias.
* **Controle de Tempo:** Manipulação dinâmica de `Time.timeScale` para telas de pausa, transições dramáticas de derrota e vitórias.

---

## 🎮 Funcionalidades Principais

* **Sistema de Vidas Dinâmico:** Controle global de tentativas com reinicialização inteligente de fases ou direcionamento definitivo para a tela de Game Over.
* **Sistemas de Ataque Sincronizados:** Chefes com rotinas de ataques cíclicos (chuva de estacas, ondas de choque no solo com verificação de cena) limpas via eventos de `OnDisable` para evitar vazamento de memória (*memory leaks*).
* **Menu de GameOver Persistente:** Reconhecimento automático se o jogador possuía um checkpoint ativo antes da morte, permitindo a restauração do progresso ou o reinício do estágio atual.
* **UI Dinâmica:** Quantidade de vidas e barra de vida baseada em matrizes de sprites fatiados (*sliced bar UI*).

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
* Unity Hub instalado.
* Unity Editor (versão recomendada com suporte a módulos WebGL e Windows Standalone).

### Passos
1. Clone este repositório:
   ```bash
   git clone [https://github.com/seu-usuario/the-last-save.git](https://github.com/seu-usuario/the-last-save.git)

# CompApp - Compilador e Interpretador Simplificado em C#

## Visão Geral

**CompApp** é um **Compilador e Interpretador Simplificado** desenvolvido em **C#**. Este projeto transforma um código-fonte escrito em uma linguagem semelhante ao Java em um **código objeto** (instruções de máquina simplificadas) e, em seguida, executa essas instruções. O objetivo é demonstrar as etapas fundamentais de um compilador, incluindo análise léxica, sintática, semântica, geração de código e interpretação.

## Funcionalidades

- **Análise Léxica:** Transforma o código-fonte em uma sequência de tokens.
- **Análise Sintática:** Constrói uma Árvore de Sintaxe Abstrata (AST) a partir dos tokens.
- **Análise Semântica:** Verifica a semântica do código, como declarações de variáveis e tipos.
- **Geração de Código:** Converte a AST em um código objeto composto por instruções específicas.
- **Interpretação:** Lê o código objeto e executa as instruções, simulando a execução do programa.

## Explicação dos Arquivos

- **GeradorDeCodigo/CodeGenerator.cs:** Implementação do Gerador de Código Objeto responsável por transformar a AST em instruções de máquina simplificadas.
- **Lexico/Lexer.cs:** Implementação do Analisador Léxico que converte o código-fonte em uma lista de tokens.
- **Sintatico/ASTNodes.cs & Parser.cs:** Implementação do Analisador Sintático que constrói a AST a partir dos tokens.
- **Semantico/SemanticAnalyzer.cs & SymbolTable.cs:** Implementação do Analisador Semântico que verifica a semântica do código e gerencia a tabela de símbolos.
- **Interpreter/InterpreterEngine.cs:** Implementação do Interpretador que executa as instruções do código objeto.
- **Input/codigo_entrada.txt:** Arquivo de entrada onde o usuário coloca o código-fonte que deseja compilar e executar.
- **Output/codigo_objeto.txt:** Arquivo de saída onde o código objeto gerado é armazenado após a compilação.
- **Program.cs:** Ponto de entrada principal que orquestra o processo de compilação e interpretação.
- **CompApp.csproj:** Arquivo de configuração do projeto C#.
- **README.md:** Este arquivo de documentação.

## Pré-requisitos

- **.NET SDK:** Certifique-se de ter o .NET SDK instalado. Você pode baixar a versão mais recente em [dotnet.microsoft.com](https://dotnet.microsoft.com/download).
- **Editor de Código:** Recomenda-se o uso do **Visual Studio** ou **Visual Studio Code** para melhor experiência de desenvolvimento.

## Configuração e Compilação

### Usando o Visual Studio

1. **Abrir o Projeto:**
   - Abra o Visual Studio.
   - Selecione **Arquivo > Abrir > Projeto/Solução** e navegue até a pasta `CompApp` para abrir seu projeto.

2. **Restaurar Pacotes NuGet (se aplicável):**
   - No **Gerenciador de Soluções**, clique com o botão direito no projeto e selecione **Restaurar Pacotes NuGet**.

3. **Compilar o Projeto:**
   - Selecione **Build > Build Solution** ou pressione `Ctrl + Shift + B`.
   - Verifique se a compilação foi bem-sucedida sem erros.

### Usando o .NET CLI

1. **Navegar para a Pasta do Projeto:**

   Abra o **Prompt de Comando** ou **Terminal** e navegue até a pasta `CompApp`:

   ```bash
   cd C:\Users\Lucca\Desktop\compilador\io1\CompApp

**Terminar README e comentar o código**

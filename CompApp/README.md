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

    Criar um Arquivo de Solução (se ainda não existir):

    bash

dotnet new sln

Adicionar o Projeto à Solução:

Se ainda não houver um projeto, crie um projeto de console:

bash

dotnet new console -n CompApp

Adicione o projeto à solução:

bash

dotnet sln add CompApp/CompApp.csproj

Compilar a Solução:

bash

    dotnet build

    Certifique-se de que a compilação seja bem-sucedida.

Como Executar o Compilador e Interpretador

O CompApp pode ser executado com diferentes comandos para compilar, interpretar ou ambos. Todos os comandos devem ser executados a partir da pasta onde o executável CompApp.exe está localizado, geralmente bin\Debug\net8.0\ após a compilação.
Passos para Executar

    Preparar o Arquivo de Entrada:

    Edite o arquivo Input\codigo_entrada.txt com o código-fonte que deseja compilar e executar.

    Exemplo de Código-Fonte:

    java

public class Teste {
    public static void main(String[] args) {
        somar();
    }
    public static double somar(){
        double cont;
        double a,b,c;
        cont = 3;
        while(cont > 0) {
            a = lerDouble();
            b = lerDouble();
            if (a > b) {
                c = a - b;
            } else {
                c = b - a;
            }
            System.out.println(c);
            cont = cont - 1;
        }
        return c;
    }
}

Abrir o Prompt de Comando ou Terminal:

Abra o Prompt de Comando (Windows) ou Terminal (Linux/Mac).

Navegar até a Pasta do Executável:

bash

cd C:\Users\Lucca\Desktop\compilador\io1\CompApp\bin\Debug\net8.0\

Executar o Comando Desejado:

    Compilar e Executar:

    bash

.\CompApp.exe compile_and_run

Fluxo de Execução:

    Lê o código-fonte de Input\codigo_entrada.txt.
    Realiza análise léxica, sintática e semântica.
    Gera o código objeto em Output\codigo_objeto.txt.
    Executa o código objeto usando o interpretador.

Apenas Compilar:

bash

.\CompApp.exe compile

Fluxo de Execução:

    Lê o código-fonte de Input\codigo_entrada.txt.
    Realiza análise léxica, sintática e semântica.
    Gera o código objeto em Output\codigo_objeto.txt.

Apenas Executar:

bash

        .\CompApp.exe run

        Fluxo de Execução:
            Lê o código objeto de Output\codigo_objeto.txt.
            Executa as instruções contidas nele usando o interpretador.

Nota Importante:

Evite conflitos com outros executáveis que possam ter nomes semelhantes. Utilize .\ para garantir que está executando o executável na pasta atual.


O que arrumar: No método arrumar: //encaixar return
                                  //expre
                                  // ;

                                  - verificar se o parâmetro do método declarado é igual ao do método chamado
                                  - loop do while

                                  fazer uma máquina hipotética para rodar o código objeto
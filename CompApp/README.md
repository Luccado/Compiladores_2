# CompApp - Compilador e Interpretador

## Matéria Compiladores 2 - Lucca Souza Di Oliveira

- **RGA**: 202011310024
- **Professor**: RAPHAEL DE SOUZA ROSA GOMES

## Visão Geral

**CompApp** é um **Compilador e Interpretador Simplificado** desenvolvido em **C#**. Este projeto transforma um código-fonte escrito em uma linguagem semelhante ao Java (MinJava) em um **código objeto** (instruções de máquina simplificadas, sendo as instruções correspondentes ao slide do professor) e, em seguida, executa essas instruções. Assim, incluindo análise léxica, sintática, semântica, geração de código e uma máquina hipotética.

## Funcionalidades

- **Análise Léxica:** Transforma o código-fonte em uma sequência de tokens.
- **Análise Sintática:** Constrói uma Árvore de Sintaxe Abstrata (AST) a partir dos tokens.
- **Análise Semântica:** Verifica a semântica do código, como declarações de variáveis e tipos.
- **Print AST:** Printa a Árvore de sintaxe abstrata (AST) no console.
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
- **README.md:** Este arquivo de documentação.

## Pré-requisitos

- **.NET SDK:** Certifique-se de ter o .NET SDK instalado. Você pode baixar a versão mais recente em [dotnet.microsoft.com](https://dotnet.microsoft.com/download).
- **Editor de Código:** Recomenda-se o uso do **Visual Studio 2022** ou **Visual Studio Code** ou **JetBrains Rider** para melhor experiência de desenvolvimento.

## Configuração e Compilação

### Usando o Visual Studio

1. **Abrir o Projeto:**
   - Abra o Visual Studio.
   - Selecione **Arquivo > Abrir > Projeto/Solução** e navegue até a pasta `CompApp` para abrir seu projeto.

2. **Compilar o Projeto:**
   - Selecione **Build > Build Solution** ou pressione `Ctrl + Shift + B`.
   - Verifique se a compilação foi bem-sucedida sem erros.

### Usando o .NET CLI

1. **Navegar para a Pasta do Projeto:**

   Abra o **Prompt de Comando** ou **Terminal** e navegue até a pasta `CompApp`:

   ```bash
     cd C:\Caminho\Para\CompApp

2. **Compile o projeto:**
    ```bash
   dotnet build

### Como Executar o Compilador e Interpretador

O Compilador permite que você escreva seu código-fonte em MiniJava no arquivo ../Input\codigo_entrada.txt, compile e execute o programa.

1. **Editar arquivo de entrada:**

   - Edite o arquivo `Input\codigo_entrada.txt`
   
    **Exemplo de código de entrada disponibilizado pelo professor.**
    ```bash
   public class Teste {
    public static void main(String[] args) {
        somar();
        }
        public static double somar(){
            double cont;
            double a,b,c;
            cont = 10;
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


2. **Compilar e executar o programa**
    
- Usando o Visual Studio

- Compilar o Projeto:
   No Visual Studio, certifique-se de que o projeto está selecionado e pressione F5 para compilar e executar.

- Resultado:

O programa irá ler o código-fonte de `Input\codigo_entrada.txt`, realizar as análises necessárias, gerar o código objeto e executar as instruções, exibindo a saída no console e o código objeto será armazenado em `Output\codigo_objeto.txt`.



O que arrumar:
#### No método:
1. [x] encaixar return
2. [x] verificar se o parâmetro do método declarado é igual ao do método chamado
3. [x] loop do while
4. [x] fazer uma máquina hipotética para rodar o código objeto
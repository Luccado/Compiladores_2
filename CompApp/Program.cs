﻿using System;
using System.Collections.Generic;
using System.IO;
using CompApp.Interpreter;
using CompApp.Compiler.GeradorDeCodigo;
using CompApp.Compiler.Lexico;
using CompApp.Compiler.Semantico;
using CompApp.Compiler.Sintatico;

namespace CompApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Obtém o diretório do executável
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Define os diretórios Input e Output
            string inputDirectory = Path.Combine(exeDirectory, "Input");
            string outputDirectory = Path.Combine(exeDirectory, "Output");

            // Cria as pastas se não existirem
            if (!Directory.Exists(inputDirectory))
            {
                Directory.CreateDirectory(inputDirectory);
                Console.WriteLine($"Pasta 'Input' criada em: {inputDirectory}");
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
                Console.WriteLine($"Pasta 'Output' criada em: {outputDirectory}");
            }

            // Define os caminhos completos dos arquivos
            string inputPath = Path.Combine(inputDirectory, "codigo_entrada.txt");
            string outputPath = Path.Combine(outputDirectory, "codigo_objeto.txt");

            if (args.Length == 0)
            {
                Console.WriteLine("Uso:");
                Console.WriteLine("  CompApp.exe compile_and_run");
                Console.WriteLine("  CompApp.exe compile");
                Console.WriteLine("  CompApp.exe run");
                return;
            }

            string command = args[0].ToLower();

            try
            {
                if (command == "compile" || command == "compile_and_run")
                {
                    // Verificar se o arquivo de entrada existe
                    if (!File.Exists(inputPath))
                    {
                        Console.WriteLine($"Erro: Arquivo de entrada '{inputPath}' não encontrado.");
                        Console.WriteLine("Por favor, coloque o código-fonte no arquivo 'Input\\codigo_entrada.txt' e tente novamente.");
                        return;
                    }

                    // Ler o código-fonte de entrada
                    string code = File.ReadAllText(inputPath);
                    Console.WriteLine("Código-fonte lido:");
                    Console.WriteLine(code);

                    // Análise Léxica
                    Lexer lexer = new Lexer();
                    List<Token> tokens = lexer.Tokenize(code);
                    Console.WriteLine("\nTokens gerados:");
                    foreach (var token in tokens)
                    {
                        Console.WriteLine($"Tipo: {token.Type}, Valor: '{token.Value}'");
                    }

                    // Análise Sintática
                    Parser parser = new Parser(tokens);
                    var ast = parser.Parse();
                    Console.WriteLine("\nAST gerada:");
                    if (ast != null)
                    {
                        Console.WriteLine("AST construída com sucesso.");
                    }
                    else
                    {
                        Console.WriteLine("AST não foi construída.");
                        return;
                    }

                    // Análise Semântica
                    SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer();
                    semanticAnalyzer.Analyze(ast);
                    Console.WriteLine("\nAnálise semântica concluída.");

                    // Geração de Código Objeto
                    CodeGenerator codeGenerator = new CodeGenerator();
                    codeGenerator.GenerateCode(ast);
                    Console.WriteLine("\nInstruções geradas:");
                    foreach (var instr in codeGenerator.GetInstructions())
                    {
                        Console.WriteLine(instr);
                    }
                    codeGenerator.SaveToFile(outputPath);
                    Console.WriteLine($"\nCódigo objeto salvo em '{outputPath}'.");

                    if (command == "compile")
                    {
                        Console.WriteLine("\nCompilação concluída com sucesso.");
                    }
                }

                if (command == "run" || command == "compile_and_run")
                {
                    if (!File.Exists(outputPath))
                    {
                        Console.WriteLine($"\nErro: Arquivo de código objeto '{outputPath}' não encontrado.");
                        Console.WriteLine("Por favor, compile o código antes de executar.");
                        return;
                    }

                    // Execução do Interpretador
                    InterpreterEngine interpreter = new InterpreterEngine();
                    interpreter.LoadInstructions(outputPath);
                    Console.WriteLine("\nExecutando código objeto:");
                    interpreter.Run();
                    Console.WriteLine("\nExecução concluída.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex.StackTrace}");
            }
        }
    }
}

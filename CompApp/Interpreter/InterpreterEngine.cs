using System;
using System.Collections.Generic;
using System.IO;

namespace CompApp.Interpreter
{
    public class InterpreterEngine
    {
        private List<string> instructions;
        private int instructionPointer = 0;
        private Dictionary<int, double> memory;
        private double accumulator = 0;
        private Dictionary<string, int> labels;
        private Stack<int> callStack;

        public InterpreterEngine()
        {
            memory = new Dictionary<int, double>();
            labels = new Dictionary<string, int>();
            callStack = new Stack<int>();
        }

        public void LoadInstructions(string filePath)
        {
            instructions = new List<string>(File.ReadAllLines(filePath));
            // Pré-processar labels
            for (int i = 0; i < instructions.Count; i++)
            {
                string line = instructions[i].Trim();
                if (line.EndsWith(":"))
                {
                    string label = line.TrimEnd(':');
                    labels[label] = i;
                }
            }
        }

        public void Run()
        {
            while (instructionPointer < instructions.Count)
            {
                string line = instructions[instructionPointer].Trim();

                if (string.IsNullOrEmpty(line) || line.EndsWith(":"))
                {
                    instructionPointer++;
                    continue;
                }

                string[] parts = line.Split(' ');
                string instruction = parts[0];
                string argument = parts.Length > 1 ? string.Join(" ", parts, 1, parts.Length - 1) : null;

                Console.WriteLine($"> Executando: {line}");

                try
                {
                    switch (instruction)
                    {
                        case "LOAD_CONST":
                            accumulator = double.Parse(argument);
                            break;
                        case "LOAD":
                            {
                                int address = int.Parse(argument);
                                if (!memory.ContainsKey(address))
                                    throw new Exception($"Endereço de memória {address} não inicializado.");
                                accumulator = memory[address];
                                break;
                            }
                        case "STORE":
                            {
                                int address = int.Parse(argument);
                                memory[address] = accumulator;
                                break;
                            }
                        case "PUSH":
                            Push(accumulator);
                            break;
                        case "POP":
                            accumulator = Pop();
                            break;
                        case "ADD":
                            accumulator = Pop() + accumulator;
                            break;
                        case "SUB":
                            accumulator = Pop() - accumulator;
                            break;
                        case "MUL":
                            accumulator = Pop() * accumulator;
                            break;
                        case "DIV":
                            {
                                double divisor = Pop();
                                if (divisor == 0)
                                    throw new DivideByZeroException("Divisão por zero.");
                                accumulator /= divisor;
                                break;
                            }
                        case "NEG":
                            accumulator = -accumulator;
                            break;
                        case "PRINT":
                            Console.WriteLine($"[OUTPUT]: {accumulator}");
                            break;
                        case "READ":
                            Console.Write("Digite um número: ");
                            if (double.TryParse(Console.ReadLine(), out double input))
                            {
                                accumulator = input;
                            }
                            else
                            {
                                throw new Exception("Entrada inválida.");
                            }
                            break;
                        case "JMP":
                            if (!labels.ContainsKey(argument))
                                throw new Exception($"Label '{argument}' não encontrada.");
                            instructionPointer = labels[argument];
                            continue;
                        case "JZ":
                            if (accumulator == 0)
                            {
                                if (!labels.ContainsKey(argument))
                                    throw new Exception($"Label '{argument}' não encontrada.");
                                instructionPointer = labels[argument];
                                continue;
                            }
                            break;
                        case "CALL":
                            if (!labels.ContainsKey(argument))
                                throw new Exception($"Função '{argument}' não encontrada.");
                            callStack.Push(instructionPointer + 1);
                            instructionPointer = labels[argument];
                            continue;
                        case "RET":
                            if (callStack.Count > 0)
                            {
                                instructionPointer = callStack.Pop();
                                continue;
                            }
                            else
                            {
                                instructionPointer = instructions.Count; // Termina a execução
                                break;
                            }
                        case "GT":
                            accumulator = Pop() > accumulator ? 1 : 0;
                            break;
                        case "LT":
                            accumulator = Pop() < accumulator ? 1 : 0;
                            break;
                        case "EQ":
                            accumulator = Pop() == accumulator ? 1 : 0;
                            break;
                        case "NE":
                            accumulator = Pop() != accumulator ? 1 : 0;
                            break;
                        case "GE":
                            accumulator = Pop() >= accumulator ? 1 : 0;
                            break;
                        case "LE":
                            accumulator = Pop() <= accumulator ? 1 : 0;
                            break;
                        default:
                            throw new Exception($"Instrução desconhecida: {instruction}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro na instrução '{line}' na linha {instructionPointer + 1}: {ex.Message}");
                }

                instructionPointer++;
            }
        }

        private Stack<double> stack = new Stack<double>();

        private void Push(double value)
        {
            stack.Push(value);
        }

        private double Pop()
        {
            if (stack.Count == 0)
            {
                throw new Exception("Pilha vazia.");
            }
            return stack.Pop();
        }
    }
}

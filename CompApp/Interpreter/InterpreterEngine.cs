using System;
using System.Collections.Generic;
using System.IO;

namespace CompApp.Interpreter
{
    public class InterpreterEngine
    {
        private List<string[]> instructions;
        private int instructionPointer = 0;
        private Stack<Dictionary<string, double>> registersStack;
        private Stack<double> dataStack;

        public InterpreterEngine()
        {
            registersStack = new Stack<Dictionary<string, double>>();
            registersStack.Push(new Dictionary<string, double>());
            dataStack = new Stack<double>();
            instructions = new List<string[]>();
        }

        public void LoadInstructions(string filePath)
        {
            var list = new List<string>(File.ReadAllLines(filePath));
            // Pré-processar labels
            for (int i = 0; i < list.Count; i++)
            {
                string line = list[i].Trim();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    instructions.Add(line.Split(' '));
                }
            }
        }

        public void Run() // Interpretador do gerador de código
        {
            while (true)
            {
                string[] parts = instructions[instructionPointer];
                
                string instruction = parts[0];
                string argument = "";
                if (parts.Length > 1)
                {
                    argument = parts[1];
                }

                //Console.WriteLine($"> Executando: {line}");
                //Console.WriteLine($"TESTEEEEEEEEEEEEE-----");
                try
                    {
                        double pos = 0.0;
                        switch (instruction)
                        {
                            case "ALME":
                                AllocateRegister(argument);
                                break;
                            case "PSHR":
                                pos = double.Parse(argument);
                                Push(pos);
                                break;
                            case "CHPR":
                                instructionPointer = int.Parse(argument) - 1;
                                registersStack.Push(new Dictionary<string, double>());
                                break;
                            case "RTPR":
                                dataStack.Pop();
                                instructionPointer = Convert.ToInt32(dataStack.Pop()) - 1;
                                registersStack.Pop();
                                break;
                            case "ARMZ":
                                double valueToStore = dataStack.Pop();
                                registersStack.Peek()[argument] = valueToStore;
                                break;
                            case "DSVI":
                                instructionPointer = int.Parse(argument) - 1;
                                break;
                            case "DSVF":
                                double condition = dataStack.Pop();
                                if (condition == 0)
                                    instructionPointer = int.Parse(argument) - 1;
                                break;
                            case "CRCT":
                                Push(double.Parse(argument));
                                break;
                            case "CRVL":
                                double registerValue = registersStack.Peek()[argument];
                                Push(registerValue);
                                break;
                            case "SOMA":
                                double sumFirst = dataStack.Pop();
                                double sumSecond = dataStack.Pop();
                                Push(sumSecond + sumFirst);
                                break;
                            case "SUBT":
                                double subFirst = dataStack.Pop();
                                double subSecond = dataStack.Pop();
                                Push(subSecond - subFirst);
                                break;
                            case "MULT":
                                double multFirst = dataStack.Pop();
                                double multSecond = dataStack.Pop();
                                Push(multSecond * multFirst);
                                break;
                            case "DIVI":
                                double divFirst = dataStack.Pop();
                                double divSecond = dataStack.Pop();
                                Push(divSecond / divFirst);
                                break;
                            case "INVE":
                                double invertValue = dataStack.Pop();
                                Push(-invertValue);
                                break;
                            case "CPME":
                                double cmpLessFirst = dataStack.Pop();
                                double cmpLessSecond = dataStack.Pop();
                                Push(cmpLessSecond < cmpLessFirst ? 1 : 0);
                                break;
                            case "CPMA":
                                double cmpGreaterFirst = dataStack.Pop();
                                double cmpGreaterSecond = dataStack.Pop();
                                Push(cmpGreaterSecond > cmpGreaterFirst ? 1 : 0);
                                break;
                            case "CPIG":
                                double cmpEqualFirst = dataStack.Pop();
                                double cmpEqualSecond = dataStack.Pop();
                                Push(cmpEqualSecond == cmpEqualFirst ? 1 : 0);
                                break;
                            case "CDES":
                                double cmpNotEqualFirst = dataStack.Pop();
                                double cmpNotEqualSecond = dataStack.Pop();
                                Push(cmpNotEqualSecond != cmpNotEqualFirst ? 1 : 0);
                                break;
                            case "CPMI":
                                double cmpLessEqualFirst = dataStack.Pop();
                                double cmpLessEqualSecond = dataStack.Pop();
                                Push(cmpLessEqualSecond <= cmpLessEqualFirst ? 1 : 0);
                                break;
                            case "CMAI":
                                double cmpGreaterEqualFirst = dataStack.Pop();
                                double cmpGreaterEqualSecond = dataStack.Pop();
                                Push(cmpGreaterEqualSecond >= cmpGreaterEqualFirst ? 1 : 0);
                                break;
                            case "LEIT":
                                Console.Write("Digite um número: ");
                                double input = double.Parse(Console.ReadLine());
                                Push(input);
                                break;
                            case "IMPR":
                                double printValue = dataStack.Pop();
                                Console.WriteLine(printValue);
                                break;
                            case "PARA":
                                return;
                            default:
                                Console.WriteLine($"Instrução '{instruction}' não é aceita!");
                                return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro na execução: {ex.Message}");
                        return;
                    }

                    instructionPointer++;

                
            
            }
        }
        
        private void AllocateRegister(string register)
        {
            registersStack.Peek()[register] = 0;
        }
        

        private void Push(double value)
        {
            dataStack.Push(value);
        }

        private double Pop()
        {
            if (dataStack.Count == 0)
            {
                throw new Exception("Pilha vazia.");
            }
            return dataStack.Pop();
        }
    }
}

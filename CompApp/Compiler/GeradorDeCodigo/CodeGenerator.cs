using System;
using System.Collections.Generic;
using System.IO;
using CompApp.Compiler.Sintatico;

namespace CompApp.Compiler.GeradorDeCodigo
{
    public class CodeGenerator
    {
        private List<string> instructions;
        private int labelCount = 0;
        private Dictionary<string, int> variableAddresses;
        private int nextAddress = 0;

        public CodeGenerator()
        {
            instructions = new List<string>();
            variableAddresses = new Dictionary<string, int>();
        }

        public void GenerateCode(ProgramNode program)
        {
            // Gerar código para o método principal
            GenerateStatements(program.MainMethod.Statements);

            // Gerar código para o método adicional se existir
            if (program.Method != null)
            {
                GenerateMethod(program.Method);
            }
        }

        private void GenerateMethod(MethodNode method)
        {
            // Adicionar um rótulo para o método
            instructions.Add($"{method.Name}:");
            GenerateStatements(method.Statements);
            instructions.Add("RET");
        }

        private void GenerateStatements(List<StatementNode> statements)
        {
            foreach (var stmt in statements)
            {
                GenerateStatement(stmt);
            }
        }

        private void GenerateStatement(StatementNode stmt)
        {
            if (stmt is DeclarationNode decl)
            {
                foreach (var varName in decl.Variables)
                {
                    if (!variableAddresses.ContainsKey(varName))
                    {
                        variableAddresses[varName] = nextAddress++;
                        // Inicializar variável com zero
                        instructions.Add($"LOAD_CONST 0");
                        instructions.Add($"STORE {variableAddresses[varName]}");
                    }
                }
            }
            else if (stmt is AssignmentNode assign)
            {
                GenerateExpression(assign.Expression);
                int address = GetVariableAddress(assign.Variable);
                instructions.Add($"STORE {address}");
            }
            else if (stmt is IfNode ifNode)
            {
                GenerateExpression(ifNode.Condition);
                instructions.Add("PUSH");
                instructions.Add("LOAD_CONST 0");
                instructions.Add("EQ");
                string elseLabel = GenerateLabel();
                string endLabel = GenerateLabel();

                instructions.Add($"JZ {elseLabel}");
                GenerateStatements(ifNode.TrueBranch);
                instructions.Add($"JMP {endLabel}");
                instructions.Add($"{elseLabel}:");
                if (ifNode.FalseBranch != null)
                {
                    GenerateStatements(ifNode.FalseBranch);
                }
                instructions.Add($"{endLabel}:");
            }
            else if (stmt is WhileNode whileNode)
            {
                string startLabel = GenerateLabel();
                string endLabel = GenerateLabel();

                instructions.Add($"{startLabel}:");
                GenerateExpression(whileNode.Condition);
                instructions.Add("PUSH");
                instructions.Add("LOAD_CONST 0");
                instructions.Add("EQ");
                instructions.Add($"JZ {endLabel}");
                GenerateStatements(whileNode.Body);
                instructions.Add($"JMP {startLabel}");
                instructions.Add($"{endLabel}:");
            }
            else if (stmt is PrintNode print)
            {
                GenerateExpression(print.Expression);
                instructions.Add("PRINT");
            }
            else if (stmt is MethodCallNode call)
            {
                instructions.Add($"CALL {call.MethodName}");
            }
            else if (stmt is ReturnNode returnNode)
            {
                GenerateExpression(returnNode.Expression);
                instructions.Add("RET");
            }
        }

        private void GenerateExpression(ExpressionNode expr)
        {
            if (expr is NumberNode num)
            {
                instructions.Add($"LOAD_CONST {num.Value}");
            }
            else if (expr is VariableNode varNode)
            {
                int address = GetVariableAddress(varNode.Name);
                instructions.Add($"LOAD {address}");
            }
            else if (expr is BinaryExpressionNode binExpr)
            {
                GenerateExpression(binExpr.Left);
                instructions.Add("PUSH"); // Empilha o valor do operando esquerdo
                GenerateExpression(binExpr.Right);
                switch (binExpr.Operator)
                {
                    case "+":
                        instructions.Add("ADD");
                        break;
                    case "-":
                        instructions.Add("SUB");
                        break;
                    case "*":
                        instructions.Add("MUL");
                        break;
                    case "/":
                        instructions.Add("DIV");
                        break;
                }
            }
            else if (expr is UnaryExpressionNode unExpr)
            {
                GenerateExpression(unExpr.Expression);
                if (unExpr.Operator == "-")
                {
                    instructions.Add("NEG");
                }
            }
            else if (expr is ReadDoubleNode)
            {
                instructions.Add("READ");
            }
            else if (expr is ConditionNode cond)
            {
                GenerateExpression(cond.Left);
                instructions.Add("PUSH"); // Empilha o valor do operando esquerdo
                GenerateExpression(cond.Right);
                switch (cond.Operator)
                {
                    case ">":
                        instructions.Add("GT");
                        break;
                    case "<":
                        instructions.Add("LT");
                        break;
                    case "==":
                        instructions.Add("EQ");
                        break;
                    case "!=":
                        instructions.Add("NE");
                        break;
                    case ">=":
                        instructions.Add("GE");
                        break;
                    case "<=":
                        instructions.Add("LE");
                        break;
                }
            }
            else if (expr is FunctionCallNode funcCall)
            {
                if (funcCall.FunctionName == "lerDouble")
                {
                    instructions.Add("READ");
                }
                else
                {
                    instructions.Add($"CALL {funcCall.FunctionName}");
                }
            }
        }

        private int GetVariableAddress(string name)
        {
            if (!variableAddresses.TryGetValue(name, out int address))
            {
                variableAddresses[name] = nextAddress++;
                address = variableAddresses[name];
                // Inicializar variável com zero
                instructions.Add($"LOAD_CONST 0");
                instructions.Add($"STORE {address}");
            }
            return address;
        }

        private string GenerateLabel()
        {
            return $"LABEL_{labelCount++}";
        }

        public void SaveToFile(string filePath)
        {
            Console.WriteLine($"Salvando {instructions.Count} instruções no arquivo {filePath}");
            File.WriteAllLines(filePath, instructions);
        }

        public List<string> GetInstructions()
        {
            return instructions;
        }
    }
}

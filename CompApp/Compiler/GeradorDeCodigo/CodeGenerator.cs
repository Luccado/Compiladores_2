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
        private Dictionary<string, int> variableAddresses; //nome e posição
        private int nextAddress = 0; //endereço único

        //construtor
        public CodeGenerator()
        {
            instructions = new List<string>();
            variableAddresses = new Dictionary<string, int>();
        }

        public void GenerateCode(ProgramNode program)
        {
            GenerateStatements(program.MainMethod.Statements); // para o main

            if (program.Method != null) // para métodos adicionais
            {
                GenerateMethod(program.Method);
            }
        }

        private void GenerateMethod(MethodNode method)
        {
            instructions.Add($"{method.Name}:");
            GenerateStatements(method.Statements);
            instructions.Add("RET"); //Retornar do método
        }

        private void GenerateStatements(List<StatementNode> statements) //Gerar código da lista
        {
            foreach (var stmt in statements)
            {
                GenerateStatement(stmt);
            }
        }

        private void GenerateStatement(StatementNode stmt)
        {
            if (stmt is DeclarationNode decl) // para declarar variável + armazena
            {
                foreach (var varName in decl.Variables)
                {
                    if (!variableAddresses.ContainsKey(varName)) // inicializado com o LOAD_CONST 0
                    {
                        variableAddresses[varName] = nextAddress++;
                        instructions.Add($"LOAD_CONST 0");
                        instructions.Add($"STORE {variableAddresses[varName]}");
                    }
                }
            }
            else if (stmt is AssignmentNode assign) // expressão + armazena
            {
                GenerateExpression(assign.Expression);
                int address = GetVariableAddress(assign.Variable);
                instructions.Add($"STORE {address}");
            }
            else if (stmt is IfNode ifNode) // IF
            {
                GenerateExpression(ifNode.Condition);
                instructions.Add("PUSH");
                instructions.Add("LOAD_CONST 0");
                instructions.Add("EQ"); // Compara com 0
                string elseLabel = GenerateLabel();
                string endLabel = GenerateLabel();

                instructions.Add($"JZ {elseLabel}"); // falso = salta para else
                GenerateStatements(ifNode.TrueBranch);
                instructions.Add($"JMP {endLabel}"); // true = salta para o fim
                instructions.Add($"{elseLabel}:"); // ELSE
                if (ifNode.FalseBranch != null)
                {
                    GenerateStatements(ifNode.FalseBranch);
                }
                instructions.Add($"{endLabel}:");
            }
            else if (stmt is WhileNode whileNode) // WHILE
            {
                string startLabel = GenerateLabel();
                string endLabel = GenerateLabel();

                instructions.Add($"{startLabel}:");
                GenerateExpression(whileNode.Condition);
                instructions.Add("PUSH");
                instructions.Add("LOAD_CONST 0");
                instructions.Add("EQ"); // compara
                instructions.Add($"JZ {endLabel}"); // false = sai do loop
                GenerateStatements(whileNode.Body);
                instructions.Add($"JMP {startLabel}"); //return para o início para checar a condição
                instructions.Add($"{endLabel}:");
            }
            else if (stmt is PrintNode print) // PRINT
            {
                GenerateExpression(print.Expression);
                instructions.Add("PRINT");
            }
            else if (stmt is MethodCallNode call) // CALL
            {
                instructions.Add($"CALL {call.MethodName}");
            }
            else if (stmt is ReturnNode returnNode) // RETURN
            {
                GenerateExpression(returnNode.Expression);
                instructions.Add("RET");
            }
        }

        private void GenerateExpression(ExpressionNode expr)
        {
            if (expr is NumberNode num) // valor constante na pilha
            {
                instructions.Add($"LOAD_CONST {num.Value}");
            }
            else if (expr is VariableNode varNode) // load da variável
            {
                int address = GetVariableAddress(varNode.Name);
                instructions.Add($"LOAD {address}");
            }
            else if (expr is BinaryExpressionNode binExpr) // operadores binários (+, -, * e /)
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
            else if (expr is UnaryExpressionNode unExpr) // operadore unário (-)
            {
                GenerateExpression(unExpr.Expression);
                if (unExpr.Operator == "-")
                {
                    instructions.Add("NEG");
                }
            }
            else if (expr is ReadDoubleNode) // READ
            {
                instructions.Add("READ");
            }
            else if (expr is ConditionNode cond) // Condições (> , < , == , != , >= , <= )
            {
                GenerateExpression(cond.Left); // gerar a instrução
                instructions.Add("PUSH"); // Empilha o valor do operando esquerdo
                GenerateExpression(cond.Right); // gera dnv
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
            else if (expr is FunctionCallNode funcCall) // Função específica "LerDouble" que foi solicitada na gramática senão CALL com o nome da função que vai ser chamada
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

        private int GetVariableAddress(string name) // Adressa a variável se possível, senão adressa um novo endereçamento
        {
            if (!variableAddresses.TryGetValue(name, out int address))
            {
                variableAddresses[name] = nextAddress++;
                address = variableAddresses[name];
                instructions.Add($"LOAD_CONST 0"); // começa com 0
                instructions.Add($"STORE {address}"); // armazena o endere
            }
            return address;
        }

        private string GenerateLabel() // Label
        {
            return $"LABEL_{labelCount++}";
        }

        public void SaveToFile(string filePath) // cw com local salvo
        {
            Console.WriteLine($"Salvando {instructions.Count} instruções no arquivo {filePath}");
            File.WriteAllLines(filePath, instructions);
        }

        public List<string> GetInstructions() // Lista de instruções 
        {
            return instructions;
        }
    }
}

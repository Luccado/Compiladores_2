using System;
using System.Collections.Generic;
using System.IO;
using CompApp.Compiler.Sintatico;

namespace CompApp.Compiler.GeradorDeCodigo
{
    public class CodeGenerator
    {
        private List<string> instructions;
        private int position;
        private int labelCount = 0; //label único
        private Dictionary<string, int> variableAddresses; //nome e posição
        private int nextAddress = 0; //endereço único
        
        
        
        //construtor
        public CodeGenerator()
        {
            instructions = new List<string>();
            variableAddresses = new Dictionary<string, int>();
        }

        public void AddInstruction(string instruction)
        {
            instructions.Add(instruction);
            position++;
        }

        public void GenerateCode(ProgramNode program)
        {
            GenerateStatements(program.MainMethod.Statements); // para o main
            AddInstruction($"PARA");
            if (program.Method != null) // para métodos adicionais
            {
                GenerateMethod(program.Method);
                int j = 0, positionM = -1;
                var nM = program.Method.Name;
                
                foreach (var instruction in instructions.ToList())
                {
                    if (instruction.Equals("PARA"))
                    {
                        positionM = j + 1;
                        break;
                    }
                    j++;
                }
                
                var i = 0;
                foreach (var instruction in instructions.ToList())
                {
                    // Substituir todas as ocorrências de nM pelo nome do método
                    instructions[i] = instruction.Replace(nM, positionM.ToString());
                    i++;
                }

                
            }
            
            
        }

        private void GenerateMethod(MethodNode method)
        {
            for (int i = method.Parameters.Count - 1; i >= 0; i--)
            {
                var parameter = method.Parameters[i];
                AddInstruction($"ALME {parameter.Name}");
                AddInstruction($"ARMZ {parameter.Name}");
            }
            GenerateStatements(method.Statements);
            AddInstruction("RTPR"); //Retornar do método
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
                    
                        variableAddresses[varName] = nextAddress++;
                        //AddInstruction($"LOAD_CONST 0");
                        //AddInstruction($"STORE {variableAddresses[varName]}");
                        AddInstruction($"ALME {varName}");
                    
                }
            }
            else if (stmt is AssignmentNode assign) // expressão + armazena
            {
                GenerateExpression(assign.Expression);
                int address = GetVariableAddress(assign.Variable);
                //AddInstruction($"STORE {address}");
                AddInstruction($"ARMZ {assign.Variable}");
            }
            else if (stmt is IfNode ifNode) // IF
            {

                
                    
                GenerateExpression(ifNode.Condition);
                
                int positionDsvf = position;
                AddInstruction($"DSVF ");
                

                string elseLabel = GenerateLabel();
                string endLabel = GenerateLabel();
                
                GenerateStatements(ifNode.TrueBranch);
                
                if (ifNode.FalseBranch != null)
                {
                    int positionDsvi = position;
                    AddInstruction($"DSVI ");
                    instructions[positionDsvf] += $"{position}";
                    GenerateStatements(ifNode.FalseBranch);
                    instructions[positionDsvi] += $"{position}";
                }
                else
                {
                    instructions[positionDsvf] += $"{position}";
                }
                
            }
            else if (stmt is WhileNode whileNode) // WHILE
            {
                string startLabel = GenerateLabel();
                string endLabel = GenerateLabel();
                int positionCondition = position;
                
                GenerateExpression(whileNode.Condition);
                int positionDsvf = position;
                AddInstruction($"DSVF ");
                GenerateStatements(whileNode.Body);
                AddInstruction($"DSVI {positionCondition}");
                instructions[positionDsvf] += $"{position}";

            }
            else if (stmt is PrintNode print) // PRINT
            {
                GenerateExpression(print.Expression);
                AddInstruction("IMPR");
            }
            else if (stmt is MethodCallNode call) // CALL
            {
                int positionPshr = position;
                AddInstruction($"PSHR ");

                foreach (var argument in call.Arguments)
                {
                    AddInstruction($"CRVL {argument.Name}");
                }
                
                AddInstruction($"CHPR {call.MethodName}");
                instructions[positionPshr] += $"{position}";
                //AddInstruction($"PARA");


            }
            else if (stmt is ReturnNode returnNode) // RETURN
            {
                GenerateExpression(returnNode.Expression);
                //AddInstruction("RTPR");
            }
        }

        private void GenerateExpression(ExpressionNode expr)
        {
            if (expr is NumberNode num) // valor constante na pilha
            {
                AddInstruction($"CRCT {num.Value}");
            }
            else if (expr is VariableNode varNode) // load da variável
            {
                int address = GetVariableAddress(varNode.Name);
                AddInstruction($"CRVL {varNode.Name}");
            }
            else if (expr is BinaryExpressionNode binExpr) // operadores binários (+, -, * e /)
            {
                GenerateExpression(binExpr.Left);
                //AddInstruction("PUSH"); // Empilha o valor do operando esquerdo
                GenerateExpression(binExpr.Right);
                switch (binExpr.Operator)
                {
                    case "+":
                        AddInstruction("SOMA");
                        break;
                    case "-":
                        AddInstruction("SUBT");
                        break;
                    case "*":
                        AddInstruction("MULT");
                        break;
                    case "/":
                        AddInstruction("DIVI");
                        break;
                }
            }
            else if (expr is UnaryExpressionNode unExpr) // operadore unário (-)
            {
                GenerateExpression(unExpr.Expression);
                if (unExpr.Operator == "-")
                {
                    AddInstruction("INVE");
                }
            }
            else if (expr is ReadDoubleNode) // READ
            {
                AddInstruction("LEIT");
            }
            else if (expr is ConditionNode cond) // Condições (> , < , == , != , >= , <= )
            {
                GenerateExpression(cond.Left); // gerar a instrução
                //AddInstruction("PUSH"); // Empilha o valor do operando esquerdo
                GenerateExpression(cond.Right); // gera dnv
                switch (cond.Operator)
                {
                    case ">":
                        AddInstruction("CPMA");
                        break;
                    case "<":
                        AddInstruction("CPME");
                        break;
                    case "==":
                        AddInstruction("CPIG");
                        break;
                    case "!=":
                        AddInstruction("CDES");
                        break;
                    case ">=":
                        AddInstruction("CMAI");
                        break;
                    case "<=":
                        AddInstruction("CPMI");
                        break;
                }
            }
            else if (expr is FunctionCallNode funcCall) // Função específica "LerDouble" que foi solicitada na gramática senão CALL com o nome da função que vai ser chamada
            {
                if (funcCall.FunctionName == "lerDouble")
                {
                    AddInstruction("LEIT");
                }
                else
                {
                    AddInstruction($"CALL {funcCall.FunctionName}");
                }
            }
        }

        private int GetVariableAddress(string name) // Adressa a variável se possível, senão adressa um novo endereçamento
        {
            if (!variableAddresses.TryGetValue(name, out int address))
            {
                variableAddresses[name] = nextAddress++;
                address = variableAddresses[name];
                //AddInstruction($"LOAD_CONST 0"); // começa com 0
                //AddInstruction($"STORE {address}"); // armazena o endere
                //AddInstruction($"ALME");
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

using System;
using System.Collections.Generic;
using CompApp.Compiler.Sintatico;

namespace CompApp.Compiler.Semantico
{
    public class SemanticAnalyzer
    {
        public SymbolTable symbolTable;

        public SemanticAnalyzer()
        {
            symbolTable = new SymbolTable();
        }

        public void Analyze(ProgramNode program)
        {
            // Analisar o método principal
            symbolTable.EnterScope();
            AnalyzeStatements(program.MainMethod.Statements);
            symbolTable.ExitScope();

            // Analisar o método adicional se tiver
            if (program.Method != null)
            {
                symbolTable.EnterScope();
                // Adicionar parâmetros na tabela de símbolos
                foreach (var param in program.Method.Parameters)
                {
                    if (!symbolTable.AddSymbol(new Symbol
                    {
                        Name = param.Name,
                        Type = param.Type
                    }))
                    {
                        throw new Exception($"Parâmetro '{param.Name}' já declarado no método '{program.Method.Name}'.");
                    }
                }

                AnalyzeStatements(program.Method.Statements);
                symbolTable.ExitScope();
            }
        }

        private void AnalyzeStatements(List<StatementNode> statements)
        {
            foreach (var stmt in statements)
            {
                AnalyzeStatement(stmt);
            }
        }

        private void AnalyzeStatement(StatementNode stmt) // Verificação semântica e de tipo. Se eu tiver problema futuro com declaração de variável vai ser aqui o problema
        {
            if (stmt is DeclarationNode decl)
            {
                foreach (var varName in decl.Variables)
                {
                    if (!symbolTable.AddSymbol(new Symbol { Name = varName, Type = decl.Type }))
                    {
                        throw new Exception($"Variável '{varName}' já declarada.");
                    }
                }
            }
            else if (stmt is AssignmentNode assign)
            {
                Symbol symbol = symbolTable.GetSymbol(assign.Variable);
                if (symbol == null)
                {
                    throw new Exception($"Variável '{assign.Variable}' não declarada.");
                }
                AnalyzeExpression(assign.Expression);
            }
            else if (stmt is IfNode ifNode)
            {
                AnalyzeExpression(ifNode.Condition);
                symbolTable.EnterScope();
                AnalyzeStatements(ifNode.TrueBranch);
                symbolTable.ExitScope();

                if (ifNode.FalseBranch != null)
                {
                    symbolTable.EnterScope();
                    AnalyzeStatements(ifNode.FalseBranch);
                    symbolTable.ExitScope();
                }
            }
            else if (stmt is WhileNode whileNode)
            {
                AnalyzeExpression(whileNode.Condition);
                symbolTable.EnterScope();
                AnalyzeStatements(whileNode.Body);
                symbolTable.ExitScope();
            }
            else if (stmt is PrintNode print)
            {
                AnalyzeExpression(print.Expression);
            }
            else if (stmt is MethodCallNode call)
            {
                if (!call.MethodName.Equals(symbolTable.methodName))
                {
                    throw new Exception($"Método '{call.MethodName}' não declarado.");
                }
                
                if (call.Arguments.Count != symbolTable.parametersNumber)
                {
                    throw new Exception($"{call.Arguments.Count} é a quantidade de argumentos errada.");
                }
                foreach (var varName in call.Arguments)
                {
                    Symbol symbol = symbolTable.GetSymbol(varName.Name);
                    if (symbol == null)
                    {
                        throw new Exception($"Variável '{varName.Name}' não declarada.");
                    }
                }
            }
            else if (stmt is ReturnNode returnNode)
            {
                AnalyzeExpression(returnNode.Expression);
            }
        }

        private void AnalyzeExpression(ExpressionNode expr) // Verificação de expressão
        {
            if (expr is BinaryExpressionNode binExpr)
            {
                AnalyzeExpression(binExpr.Left);
                AnalyzeExpression(binExpr.Right);
            }
            else if (expr is UnaryExpressionNode unExpr)
            {
                AnalyzeExpression(unExpr.Expression);
            }
            else if (expr is VariableNode varNode)
            {
                Symbol symbol = symbolTable.GetSymbol(varNode.Name);
                if (symbol == null)
                {
                    throw new Exception($"Variável '{varNode.Name}' não declarada.");
                }
            }
            else if (expr is NumberNode)
            {
                // Nada a fazer
            }
            else if (expr is ReadDoubleNode)
            {
                // Nada a fazer
            }
            else if (expr is ConditionNode cond)
            {
                AnalyzeExpression(cond.Left);
                AnalyzeExpression(cond.Right);
            }
            else if (expr is FunctionCallNode funcCall)
            {
                if (funcCall.FunctionName == "lerDouble")
                {
                    // Não entendi direito o lerDouble mas assim deu
                }
                else
                {
                    // Só para ver se foi declarada mesmo
                }

                foreach (var arg in funcCall.Arguments)
                {
                    AnalyzeExpression(arg);
                }
            }
        }
    }
}

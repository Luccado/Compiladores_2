using System;
using System.Collections.Generic;
using CompApp.Compiler.Sintatico;

namespace CompApp.Compiler.Semantico
{
    public class SemanticAnalyzer
    {
        private SymbolTable symbolTable;

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

            // Analisar o método adicional se existir
            if (program.Method != null)
            {
                symbolTable.EnterScope();
                // Adicionar parâmetros à tabela de símbolos
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

        private void AnalyzeStatement(StatementNode stmt)
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
                // Implementar verificação se necessário
                // Por exemplo, verificar se a função chamada existe
            }
            else if (stmt is ReturnNode returnNode)
            {
                AnalyzeExpression(returnNode.Expression);
                // Verificações adicionais podem ser adicionadas aqui
            }
        }

        private void AnalyzeExpression(ExpressionNode expr)
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
                    // Função embutida, nenhum tratamento adicional necessário
                }
                else
                {
                    // Verificar se a função foi declarada
                    // Para simplificar, assumimos que todas as funções estão definidas
                }

                foreach (var arg in funcCall.Arguments)
                {
                    AnalyzeExpression(arg);
                }
            }
        }
    }
}

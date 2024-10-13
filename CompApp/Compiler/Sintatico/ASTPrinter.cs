using System;
using CompApp.Compiler.Sintatico;

namespace CompApp.Compiler.Sintatico
{
    public class ASTPrinter
    {
        private int indentLevel = 0;

        public void Print(ProgramNode node) // Basicamente percorre a árvore e imprime os nó (falta identação)
        {
            Console.WriteLine("Program");
            indentLevel++;
            Console.WriteLine($"{Indent()}ClassName: {node.ClassName}");
            PrintMainMethod(node.MainMethod);
            if (node.Method != null)
            {
                PrintMethod(node.Method);
            }
            indentLevel--;
        }

        private void PrintMainMethod(MainMethodNode main)
        {
            Console.WriteLine($"{Indent()}MainMethod");
            indentLevel++;
            Console.WriteLine($"{Indent()}ArgsName: {main.ArgsName}");
            Console.WriteLine($"{Indent()}Statements:");
            indentLevel++;
            foreach (var stmt in main.Statements)
            {
                PrintStatement(stmt);
            }
            indentLevel--;
            indentLevel--;
        }

        private void PrintMethod(MethodNode method)
        {
            Console.WriteLine($"{Indent()}Method: {method.Name}");
            indentLevel++;
            Console.WriteLine($"{Indent()}ReturnType: {method.ReturnType}");
            Console.WriteLine($"{Indent()}Parameters:");
            indentLevel++;
            foreach (var param in method.Parameters)
            {
                Console.WriteLine($"{Indent()}- {param.Type} {param.Name}");
            }
            indentLevel--;
            Console.WriteLine($"{Indent()}Statements:");
            indentLevel++;
            foreach (var stmt in method.Statements)
            {
                PrintStatement(stmt);
            }
            indentLevel--;
            indentLevel--;
        }

        private void PrintStatement(StatementNode stmt)
        {
            switch (stmt)
            {
                case DeclarationNode decl:
                    Console.WriteLine($"{Indent()}Declaration: {decl.Type} {string.Join(", ", decl.Variables)}");
                    break;
                case AssignmentNode assign:
                    Console.WriteLine($"{Indent()}Assignment: {assign.Variable} =");
                    indentLevel++;
                    PrintExpression(assign.Expression);
                    indentLevel--;
                    break;
                case IfNode ifNode:
                    Console.WriteLine($"{Indent()}If");
                    indentLevel++;
                    Console.WriteLine($"{Indent()}Condition:");
                    indentLevel++;
                    PrintExpression(ifNode.Condition);
                    indentLevel--;
                    Console.WriteLine($"{Indent()}TrueBranch:");
                    indentLevel++;
                    foreach (var trueStmt in ifNode.TrueBranch)
                    {
                        PrintStatement(trueStmt);
                    }
                    indentLevel--;
                    if (ifNode.FalseBranch != null)
                    {
                        Console.WriteLine($"{Indent()}FalseBranch:");
                        indentLevel++;
                        foreach (var falseStmt in ifNode.FalseBranch)
                        {
                            PrintStatement(falseStmt);
                        }
                        indentLevel--;
                    }
                    indentLevel--;
                    break;
                case WhileNode whileNode:
                    Console.WriteLine($"{Indent()}While");
                    indentLevel++;
                    Console.WriteLine($"{Indent()}Condition:");
                    indentLevel++;
                    PrintExpression(whileNode.Condition);
                    indentLevel--;
                    Console.WriteLine($"{Indent()}Body:");
                    indentLevel++;
                    foreach (var bodyStmt in whileNode.Body)
                    {
                        PrintStatement(bodyStmt);
                    }
                    indentLevel--;
                    indentLevel--;
                    break;
                case PrintNode print:
                    Console.WriteLine($"{Indent()}Print:");
                    indentLevel++;
                    PrintExpression(print.Expression);
                    indentLevel--;
                    break;
                case MethodCallNode call:
                    //String args = string.Join(", ", call.Arguments.Name);
                    Console.WriteLine($"{Indent()}MethodCall: {call.MethodName}()");
                    
                    break;
                case ReturnNode returnNode:
                    Console.WriteLine($"{Indent()}Return:");
                    indentLevel++;
                    PrintExpression(returnNode.Expression);
                    indentLevel--;
                    break;
                default:
                    Console.WriteLine($"{Indent()}Unknown Statement");
                    break;
            }
        }

        private void PrintExpression(ExpressionNode expr)
        {
            switch (expr)
            {
                case BinaryExpressionNode binExpr:
                    Console.WriteLine($"{Indent()}BinaryExpression: {binExpr.Operator}");
                    indentLevel++;
                    Console.WriteLine($"{Indent()}Left:");
                    indentLevel++;
                    PrintExpression(binExpr.Left);
                    indentLevel--;
                    Console.WriteLine($"{Indent()}Right:");
                    indentLevel++;
                    PrintExpression(binExpr.Right);
                    indentLevel--;
                    indentLevel--;
                    break;
                case UnaryExpressionNode unExpr:
                    Console.WriteLine($"{Indent()}UnaryExpression: {unExpr.Operator}");
                    indentLevel++;
                    PrintExpression(unExpr.Expression);
                    indentLevel--;
                    break;
                case NumberNode num:
                    Console.WriteLine($"{Indent()}Number: {num.Value}");
                    break;
                case VariableNode varNode:
                    Console.WriteLine($"{Indent()}Variable: {varNode.Name}");
                    break;
                case ConditionNode cond:
                    Console.WriteLine($"{Indent()}Condition: {cond.Operator}");
                    indentLevel++;
                    Console.WriteLine($"{Indent()}Left:");
                    indentLevel++;
                    PrintExpression(cond.Left);
                    indentLevel--;
                    Console.WriteLine($"{Indent()}Right:");
                    indentLevel++;
                    PrintExpression(cond.Right);
                    indentLevel--;
                    indentLevel--;
                    break;
                case FunctionCallNode funcCall:
                    Console.WriteLine($"{Indent()}FunctionCall: {funcCall.FunctionName}()");
                    break;
                case ReadDoubleNode:
                    Console.WriteLine($"{Indent()}ReadDouble");
                    break;
                default:
                    Console.WriteLine($"{Indent()}Unknown Expression");
                    break;
            }
        }

        private string Indent()
        {
            return new string(' ', indentLevel * 2);
        }
    }
}

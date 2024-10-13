using System;
using System.Collections.Generic;

namespace CompApp.Compiler.Sintatico
{
    public abstract class ASTNode { }

    public class ProgramNode : ASTNode
    {
        public string ClassName { get; set; }
        public MainMethodNode MainMethod { get; set; }
        public MethodNode Method { get; set; }
    }

    public class MainMethodNode : ASTNode
    {
        public string ArgsName { get; set; }
        public List<StatementNode> Statements { get; set; }
    }

    public class MethodNode : ASTNode
    {
        public string ReturnType { get; set; }
        public string Name { get; set; }
        public List<ParameterNode> Parameters { get; set; }
        public List<StatementNode> Statements { get; set; }
    }

    public class ParameterNode : ASTNode
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }

    public abstract class StatementNode : ASTNode { }

    public class DeclarationNode : StatementNode
    {
        public string Type { get; set; }
        public List<string> Variables { get; set; }
    }

    public class AssignmentNode : StatementNode
    {
        public string Variable { get; set; }
        public ExpressionNode Expression { get; set; }
    }

    public class IfNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public List<StatementNode> TrueBranch { get; set; }
        public List<StatementNode> FalseBranch { get; set; }
    }

    public class WhileNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public List<StatementNode> Body { get; set; }
    }

    public class PrintNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }
    }

    public class MethodCallNode : StatementNode
    {
        public string MethodName { get; set; }
        
        public List<VariableNode> Arguments { get; set; }
        
    }

    public class ReturnNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }
    }

    public abstract class ExpressionNode : ASTNode { }

    public class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public string Operator { get; set; }
        public ExpressionNode Right { get; set; }
    }

    public class UnaryExpressionNode : ExpressionNode
    {
        public string Operator { get; set; }
        public ExpressionNode Expression { get; set; }
    }

    public class NumberNode : ExpressionNode
    {
        public double Value { get; set; }
    }

    public class VariableNode : ExpressionNode
    {
        public string Name { get; set; }
    }

    public class ConditionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public string Operator { get; set; }
        public ExpressionNode Right { get; set; }
    }

    public class ReadDoubleNode : ExpressionNode { }

    public class FunctionCallNode : ExpressionNode
    {
        public string FunctionName { get; set; }
        public List<VariableNode> Arguments { get; set; }
    }
}

using System;
using System.Collections.Generic;
using CompApp.Compiler.Lexico;
using CompApp.Compiler.Sintatico;

namespace CompApp.Compiler.Sintatico
{
    public class Parser //mesma lógica do lexer
    {
        private List<Token> tokens; // tokens gerados
        private int position = 0; //posição inicializada no 0
        private Token currentToken; //token atual

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            currentToken = tokens[position];
        }

        public ProgramNode Parse() //inicializa a árvore e faz a análise
        {
            ProgramNode root = new ProgramNode();
            Prog(root);
            return root;
        }

        private void Prog(ProgramNode root) // Prog com CMDS nele
        {
            Match(TokenType.Keyword, "public");
            Match(TokenType.Keyword, "class");
            string className = Match(TokenType.Identifier).Value;
            Match(TokenType.Symbol, "{");

            // Main
            Match(TokenType.Keyword, "public");
            Match(TokenType.Keyword, "static");
            Match(TokenType.Keyword, "void");
            Match(TokenType.Keyword, "main");
            Match(TokenType.Symbol, "(");
            Match(TokenType.Keyword, "String");
            Match(TokenType.Symbol, "[");
            Match(TokenType.Symbol, "]");
            string argsName = Match(TokenType.Identifier).Value;
            Match(TokenType.Symbol, ")");

            Match(TokenType.Symbol, "{");
            // CMDS
            List<StatementNode> mainCommands = CMDS();
            Match(TokenType.Symbol, "}");

            // Método
            MethodNode method = METODO();

            Match(TokenType.Symbol, "}");

            // Construir a árvore
            root.ClassName = className;
            root.MainMethod = new MainMethodNode
            {
                ArgsName = argsName,
                Statements = mainCommands
            };
            root.Method = method;
        }

        private MethodNode METODO() //Metodo
        {
            if (Lookahead(TokenType.Keyword, "public"))
            {
                Match(TokenType.Keyword, "public");
                Match(TokenType.Keyword, "static");
                string returnType = TIPO();
                string methodName = Match(TokenType.Identifier).Value;
                Match(TokenType.Symbol, "(");
                List<ParameterNode> parameters = PARAMS();
                Match(TokenType.Symbol, ")");

                Match(TokenType.Symbol, "{");
                List<StatementNode> commands = CMDS();
                //encaixar return
                //expre
                // ;
                Match(TokenType.Symbol, "}"); 

                return new MethodNode
                {
                    ReturnType = returnType,
                    Name = methodName,
                    Parameters = parameters,
                    Statements = commands
                };
            }
            else
            {
                return null; // retorna nada, sem método
            }
        }

        private List<ParameterNode> PARAMS() //PARAMS
        {
            if (Lookahead(TokenType.Keyword, "double"))
            {
                string type = TIPO();
                string paramName = Match(TokenType.Identifier).Value;
                List<ParameterNode> parameters = MAIS_PARAMS();
                parameters.Insert(0, new ParameterNode { Type = type, Name = paramName });
                return parameters;
            }
            else
            {
                return new List<ParameterNode>(); // sem parâmetro
            }
        }

        private List<ParameterNode> MAIS_PARAMS() //MAIS_PARAMS
        {
            if (Lookahead(TokenType.Symbol, ","))
            {
                Match(TokenType.Symbol, ",");
                return PARAMS();
            }
            else
            {
                return new List<ParameterNode>(); // sem mais parâmetros
            }
        }

        private string TIPO()
        {
            if (Lookahead(TokenType.Keyword, "double"))
            {
                Match(TokenType.Keyword, "double");
                return "double";
            }
            else
            {
                throw new Exception($"Tipo inválido na linha {currentToken.Line}, coluna {currentToken.Column}");
            }
        }

        private List<StatementNode> CMDS() // tudo que tá dentro do {}
        {
            List<StatementNode> commands = new List<StatementNode>();

            while (true)
            {
                if (Lookahead(TokenType.Symbol, "}"))
                {
                    break;
                }
                else if (Lookahead(TokenType.Keyword, "double"))
                {
                    // Declaração de variável se double
                    StatementNode decl = DC();
                    commands.Add(decl);
                }
                else if (Lookahead(TokenType.Keyword, "if") || Lookahead(TokenType.Keyword, "while")) //condicionais
                {
                    StatementNode cond = CMD_COND();
                    commands.Add(cond);
                }
                else if (Lookahead(TokenType.Identifier, "System")) //print
                {
                    StatementNode printStmt = CMD_PRINT();
                    commands.Add(printStmt);
                }
                else if (Lookahead(TokenType.Keyword, "return")) // return
                {
                    StatementNode returnStmt = RETURN_STATEMENT();
                    commands.Add(returnStmt);
                }
                else if (Lookahead(TokenType.Identifier)) // identificador
                {
                    string id = Match(TokenType.Identifier).Value;
                    StatementNode stmt = RESTO_IDENT(id);
                    commands.Add(stmt);
                    Match(TokenType.Symbol, ";");
                }
                else
                {
                    throw new Exception($"Comando inválido na linha {currentToken.Line}, coluna {currentToken.Column}");
                }
            }

            return commands;
        }

        private StatementNode RETURN_STATEMENT() // analisa o return
        {
            Match(TokenType.Keyword, "return");
            ExpressionNode expr = EXPRESSAO();
            Match(TokenType.Symbol, ";");

            return new ReturnNode { Expression = expr };
        }

        private StatementNode CMD_PRINT() // print + expressão
        {
            Match(TokenType.Identifier, "System");
            Match(TokenType.Symbol, ".");
            Match(TokenType.Identifier, "out");
            Match(TokenType.Symbol, ".");
            Match(TokenType.Identifier, "println");
            Match(TokenType.Symbol, "(");
            ExpressionNode expr = EXPRESSAO();
            Match(TokenType.Symbol, ")");
            Match(TokenType.Symbol, ";");

            return new PrintNode { Expression = expr };
        }

        private StatementNode RESTO_IDENT(string id) // método para identificar se é uma atribuição ou uma chamada de método
        {
            if (Lookahead(TokenType.Operator, "="))
            {
                Match(TokenType.Operator, "=");
                ExpressionNode expr = EXP_IDENT();
                return new AssignmentNode { Variable = id, Expression = expr }; // retorna atribuição
            }
            else if (Lookahead(TokenType.Symbol, "("))
            {
                Match(TokenType.Symbol, "(");
                List<ExpressionNode> args = LISTA_ARG();
                Match(TokenType.Symbol, ")");

                return new MethodCallNode { MethodName = id, Arguments = args }; // retorna a chamada do método
            }
            else
            {
                throw new Exception($"Sintaxe inválida na linha {currentToken.Line}, coluna {currentToken.Column}");
            }
        }

        private List<ExpressionNode> LISTA_ARG() // analisa a lista de argumento na chamada do método enquanto semparado por ,
        {
            List<ExpressionNode> args = new List<ExpressionNode>();

            if (!Lookahead(TokenType.Symbol, ")"))
            {
                args.Add(EXPRESSAO());
                while (Lookahead(TokenType.Symbol, ","))
                {
                    Match(TokenType.Symbol, ",");
                    args.Add(EXPRESSAO());
                }
            }

            return args;
        }

        private ExpressionNode EXP_IDENT() //EXP_IDENT
        {
            return EXPRESSAO();
        }

        private StatementNode DC() // Declaração de variável
        {
            string type = TIPO(); // tipo
            List<string> vars = VARS(); //Nome da var
            Match(TokenType.Symbol, ";");

            return new DeclarationNode { Type = type, Variables = vars };
        }

        private List<string> VARS() // Analisa o primeiro nome de variável e chama mais vars para ver se tem mais separados por ","
        {
            List<string> vars = new List<string>();
            vars.Add(Match(TokenType.Identifier).Value);
            vars.AddRange(MAIS_VAR());
            return vars;
        }

        private List<string> MAIS_VAR() // Vê se tem mais variáveis a partir da vírgula
        {
            if (Lookahead(TokenType.Symbol, ","))
            {
                Match(TokenType.Symbol, ",");
                List<string> vars = new List<string>(); // Variáveis adicionais em lista
                vars.Add(Match(TokenType.Identifier).Value);
                vars.AddRange(MAIS_VAR()); // Chamada recursiva checagem mais variáveis
                return vars;
            }
            else
            {
                return new List<string>(); // vazio sem mais vars
            }
        }

        private StatementNode CMD_COND() //condicionais if e while
        {
            if (Lookahead(TokenType.Keyword, "if"))
            {
                Match(TokenType.Keyword, "if");
                Match(TokenType.Symbol, "(");
                ExpressionNode condition = CONDICAO();
                Match(TokenType.Symbol, ")");
                Match(TokenType.Symbol, "{");
                List<StatementNode> trueBranch = CMDS();
                Match(TokenType.Symbol, "}");
                List<StatementNode> falseBranch = PFALSA();

                return new IfNode
                {
                    Condition = condition,
                    TrueBranch = trueBranch,
                    FalseBranch = falseBranch
                };
            }
            else if (Lookahead(TokenType.Keyword, "while"))
            {
                Match(TokenType.Keyword, "while");
                Match(TokenType.Symbol, "(");
                ExpressionNode condition = CONDICAO();
                Match(TokenType.Symbol, ")");
                Match(TokenType.Symbol, "{");
                List<StatementNode> body = CMDS();
                Match(TokenType.Symbol, "}");

                return new WhileNode
                {
                    Condition = condition,
                    Body = body
                };
            }
            else
            {
                throw new Exception($"Comando condicional inválido na linha {currentToken.Line}, coluna {currentToken.Column}");
            }
        }

        private List<StatementNode> PFALSA() // else do if
        {
            if (Lookahead(TokenType.Keyword, "else"))
            {
                Match(TokenType.Keyword, "else");
                Match(TokenType.Symbol, "{");
                List<StatementNode> falseBranch = CMDS();
                Match(TokenType.Symbol, "}");

                return falseBranch;
            }
            else
            {
                return null; // Sem else
            }
        }

        private ExpressionNode CONDICAO() // analisa expressão à esquerda, operador e expressão à direita basicamente
        {
            ExpressionNode left = EXPRESSAO();
            string relation = RELACAO();
            ExpressionNode right = EXPRESSAO();

            return new ConditionNode // para a árvore
            {
                Left = left,
                Operator = relation,
                Right = right
            };
        }

        private string RELACAO() // operador relacional "==" "!=" ">=" "<=" ">" "<"
        {
            if (currentToken.Type == TokenType.Operator && (currentToken.Value == "==" || currentToken.Value == "!=" ||
                currentToken.Value == ">=" || currentToken.Value == "<=" || currentToken.Value == ">" || currentToken.Value == "<"))
            {
                string op = currentToken.Value;
                Match(TokenType.Operator);
                return op;
            }
            else
            {
                throw new Exception($"Operador relacional inválido na linha {currentToken.Line}, coluna {currentToken.Column}");
            }
        }

        private ExpressionNode EXPRESSAO()
        {
            ExpressionNode term = TERMO();
            return OUTROS_TERMOS(term); // envia para OUTROS_TERMOS para verificação
        }

        private ExpressionNode OUTROS_TERMOS(ExpressionNode left) // + -
        {
            if (currentToken.Type == TokenType.Operator && (currentToken.Value == "+" || currentToken.Value == "-"))
            {
                string op = OP_AD();
                ExpressionNode right = TERMO();
                ExpressionNode expr = new BinaryExpressionNode
                {
                    Left = left,
                    Operator = op,
                    Right = right
                };
                return OUTROS_TERMOS(expr);
            }
            else
            {
                return left; // Sem mais termos
            }
        }

        private ExpressionNode TERMO()
        {
            string op_un = OP_UN();
            ExpressionNode factor = FATOR();
            ExpressionNode term = MAIS_FATORES(factor);

            if (op_un == "-")
            {
                return new UnaryExpressionNode
                {
                    Operator = "-",
                    Expression = term
                };
            }
            else
            {
                return term;
            }
        }

        private ExpressionNode MAIS_FATORES(ExpressionNode left) // * e /
        {
            if (currentToken.Type == TokenType.Operator && (currentToken.Value == "*" || currentToken.Value == "/"))
            {
                string op = OP_MUL();
                ExpressionNode right = FATOR();
                ExpressionNode expr = new BinaryExpressionNode
                {
                    Left = left,
                    Operator = op,
                    Right = right
                };
                return MAIS_FATORES(expr);
            }
            else
            {
                return left; 
            }
        }

        private string OP_UN() // verificação e consome o -
        {
            if (currentToken.Type == TokenType.Operator && currentToken.Value == "-")
            {
                Match(TokenType.Operator, "-");
                return "-";
            }
            else
            {
                return null; 
            }
        }

        private string OP_AD() // Verificação e consome o +
        {
            if (currentToken.Type == TokenType.Operator && (currentToken.Value == "+" || currentToken.Value == "-"))
            {
                string op = currentToken.Value;
                Match(TokenType.Operator);
                return op;
            }
            else
            {
                throw new Exception($"Operador '+' ou '-' esperado na linha {currentToken.Line}, coluna {currentToken.Column}");
            }
        }

        private string OP_MUL() // *
        {
            if (currentToken.Type == TokenType.Operator && (currentToken.Value == "*" || currentToken.Value == "/"))
            {
                string op = currentToken.Value;
                Match(TokenType.Operator);
                return op;
            }
            else
            {
                throw new Exception($"Operador '*' ou '/' esperado na linha {currentToken.Line}, coluna {currentToken.Column}");
            }
        }

        private ExpressionNode FATOR()
        {
            if (currentToken.Type == TokenType.Identifier)
            {
                string name = Match(TokenType.Identifier).Value;
                if (Lookahead(TokenType.Symbol, "("))
                {
                    Match(TokenType.Symbol, "(");
                    List<ExpressionNode> args = LISTA_ARG();
                    Match(TokenType.Symbol, ")");
                    return new FunctionCallNode { FunctionName = name, Arguments = args };
                }
                else
                {
                    return new VariableNode { Name = name };
                }
            }
            else if (currentToken.Type == TokenType.Number)
            {
                string value = Match(TokenType.Number).Value;
                return new NumberNode { Value = double.Parse(value) };
            }
            else if (currentToken.Type == TokenType.Symbol && currentToken.Value == "(")
            {
                Match(TokenType.Symbol, "(");
                ExpressionNode expr = EXPRESSAO();
                Match(TokenType.Symbol, ")");
                return expr;
            }
            else
            {
                throw new Exception($"Fator inválido na linha {currentToken.Line}, coluna {currentToken.Column}");
            }
        }

        // Funções auxiliares

        private Token Match(TokenType expectedType, string expectedValue = null)
        {
            if (currentToken.Type == expectedType && (expectedValue == null || currentToken.Value == expectedValue))
            {
                Token matchedToken = currentToken;
                Advance();
                return matchedToken;
            }
            else
            {
                throw new Exception($"Esperado {expectedType} '{expectedValue}', encontrado '{currentToken.Value}' na linha {currentToken.Line}, coluna {currentToken.Column}");
            }
        }

        private void Advance()
        {
            position++;
            if (position < tokens.Count)
            {
                currentToken = tokens[position];
            }
            else
            {
                currentToken = new Token { Type = TokenType.EOF, Value = "", Line = currentToken.Line, Column = currentToken.Column };
            }
        }

        private bool Lookahead(TokenType type, string value = null)
        {
            if (currentToken.Type == type)
            {
                if (value == null || currentToken.Value == value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

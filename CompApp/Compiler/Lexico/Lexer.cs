using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CompApp.Compiler.Lexico
{
    public enum TokenType
    {
        Keyword,
        Identifier,
        Number,
        Symbol,
        Operator,
        EOF,
        // ... outros tipos de tokens
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }      // Linha no código-fonte
        public int Column { get; set; }    // Coluna no código-fonte
    }

    public class Lexer
    {
        private List<TokenDefinition> tokenDefinitions;
        private List<Token> tokens;
        private string source;
        private int position;
        private int line;
        private int column;

        public Lexer()
        {
            tokenDefinitions = new List<TokenDefinition>
            {
                // Símbolos
                new TokenDefinition(TokenType.Symbol, @"\{"),
                new TokenDefinition(TokenType.Symbol, @"\}"),
                new TokenDefinition(TokenType.Symbol, @"\("),
                new TokenDefinition(TokenType.Symbol, @"\)"),
                new TokenDefinition(TokenType.Symbol, @"\["),
                new TokenDefinition(TokenType.Symbol, @"\]"),
                new TokenDefinition(TokenType.Symbol, @";"),
                new TokenDefinition(TokenType.Symbol, @","),
                new TokenDefinition(TokenType.Symbol, @"\."), // Adicionado para reconhecer o ponto '.'

                // Operadores relacionais
                new TokenDefinition(TokenType.Operator, @"==|!=|>=|<=|>|<"),
                // Operadores aritméticos e atribuição
                new TokenDefinition(TokenType.Operator, @"\+|\-|\*|\/|="),

                // Palavras-chave
                new TokenDefinition(TokenType.Keyword, @"\bpublic\b"),
                new TokenDefinition(TokenType.Keyword, @"\bclass\b"),
                new TokenDefinition(TokenType.Keyword, @"\bstatic\b"),
                new TokenDefinition(TokenType.Keyword, @"\bvoid\b"),
                new TokenDefinition(TokenType.Keyword, @"\bmain\b"),
                new TokenDefinition(TokenType.Keyword, @"\bString\b"),
                new TokenDefinition(TokenType.Keyword, @"\bdouble\b"),
                new TokenDefinition(TokenType.Keyword, @"\bif\b"),
                new TokenDefinition(TokenType.Keyword, @"\belse\b"),
                new TokenDefinition(TokenType.Keyword, @"\bwhile\b"),
                new TokenDefinition(TokenType.Keyword, @"\breturn\b"),
                // Não incluir 'lerDouble' ou 'System.out.println' como palavras-chave

                // Identificadores
                new TokenDefinition(TokenType.Identifier, @"\b[a-zA-Z_][a-zA-Z0-9_]*\b"),

                // Literais numéricos (inteiros e reais)
                new TokenDefinition(TokenType.Number, @"\b\d+(\.\d+)?\b"),
            };
        }

        public List<Token> Tokenize(string sourceCode)
        {
            tokens = new List<Token>();
            source = sourceCode;
            position = 0;
            line = 1;
            column = 1;

            while (position < source.Length)
            {
                if (char.IsWhiteSpace(source[position]))
                {
                    HandleWhitespace();
                    continue;
                }

                bool matchFound = false;

                foreach (var tokenDefinition in tokenDefinitions)
                {
                    var match = tokenDefinition.Regex.Match(source.Substring(position));
                    if (match.Success && match.Index == 0)
                    {
                        tokens.Add(new Token
                        {
                            Type = tokenDefinition.Type,
                            Value = match.Value,
                            Line = line,
                            Column = column
                        });

                        AdvancePosition(match.Length);
                        matchFound = true;
                        break;
                    }
                }

                if (!matchFound)
                {
                    throw new Exception($"Token inválido próximo a '{source[position]}' na linha {line}, coluna {column}");
                }
            }

            // Adiciona token de fim de arquivo
            tokens.Add(new Token
            {
                Type = TokenType.EOF,
                Value = string.Empty,
                Line = line,
                Column = column
            });

            return tokens;
        }

        private void HandleWhitespace()
        {
            if (source[position] == '\n')
            {
                line++;
                column = 1;
            }
            else
            {
                column++;
            }
            position++;
        }

        private void AdvancePosition(int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (source[position] == '\n')
                {
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
                position++;
            }
        }
    }

    public class TokenDefinition
    {
        public TokenType Type { get; set; }
        public Regex Regex { get; set; }

        public TokenDefinition(TokenType type, string pattern)
        {
            Type = type;
            Regex = new Regex("^" + pattern);
        }
    }
}

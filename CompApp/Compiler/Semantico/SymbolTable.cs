using System;
using System.Collections.Generic;
using CompApp.Compiler.Sintatico;

namespace CompApp.Compiler.Semantico
{
    public class Symbol 
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class SymbolTable // Apemnas a tabela de símbolo para checkar os escopos, metodos e variaveis
    {
        private Stack<Dictionary<string, Symbol>> scopes;
        public string methodName;
        public int parametersNumber;

        public SymbolTable()
        {
            scopes = new Stack<Dictionary<string, Symbol>>();
        }

        public void EnterScope()
        {
            scopes.Push(new Dictionary<string, Symbol>());
        }

        public void ExitScope()
        {
            if (scopes.Count > 0)
            {
                scopes.Pop();
            }
            else
            {
                throw new Exception("Tentativa de sair de um escopo inexistente.");
            }
        }

        public bool AddSymbol(Symbol symbol)
        {
            if (scopes.Count == 0)
                throw new Exception("Nenhum escopo aberto para adicionar símbolos.");

            var currentScope = scopes.Peek();
            if (currentScope.ContainsKey(symbol.Name))
            {
                return false; // Já existe no escopo atual
            }
            currentScope[symbol.Name] = symbol;
            return true;
        }

        public Symbol GetSymbol(string name)
        {
            foreach (var scope in scopes)
            {
                if (scope.TryGetValue(name, out Symbol symbol))
                {
                    return symbol;
                }
            }
            return null; // Não encontrado
        }
    }
}

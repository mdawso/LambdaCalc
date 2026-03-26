using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;

namespace LambdaCalc
{
    public abstract record Expression
    {
        public abstract override string ToString();
    }
    public record Variable(string name) : Expression
    {
        public override string ToString() => name;
    }
    public record Function(string parameter, Expression body) : Expression
    {
        public override string ToString() => $"\\{parameter}.{body.ToString()}";
    }
    public record Application(Expression function, Expression argument) : Expression
    {
        public override string ToString() => $"({function.ToString()}) {argument.ToString()}";
    }
    static class LambdaCalc
    {
        public static HashSet<string> GetFreeVars(Expression expr) => expr switch
        {
            Variable v => new HashSet<string> { v.name },
            Function f => GetFreeVars(f.body).Where(n => n != f.parameter).ToHashSet(),
            Application a => GetFreeVars(a.function).Union(GetFreeVars(a.argument)).ToHashSet(),
            _ => new HashSet<string>()
        };
        private static Expression HandleFunctionSubstitution(Function f, string target, Expression replacement)
        {
            var freeInReplacement = GetFreeVars(replacement);
            if (freeInReplacement.Contains(f.parameter))
            {
                string newParam = f.parameter + "'";
                var renamedBody = Substitute(f.body, f.parameter, new Variable(newParam));
                return new Function(newParam, Substitute(renamedBody, target, replacement));
            }
            return f with { body = Substitute(f.body, target, replacement) };
        }
        public static Expression Substitute(Expression expr, string target, Expression replacement) => expr switch
        {
            Variable v => (v.name == target) ? replacement : v,
            Function f when f.parameter == target => f,
            Function f => HandleFunctionSubstitution(f, target, replacement),
            Application a => new Application(
                Substitute(a.function, target, replacement),
                Substitute(a.argument, target, replacement)
            ),
            _ => throw new ArgumentException("Unknown type")
        };
        public static Expression Evaluate(Expression expr)
        {
            if (expr is Application app)
            {
                var func = Evaluate(app.function);
                var arg = Evaluate(app.argument);
                if (func is Function f)
                {
                    return Evaluate(Substitute(f.body, f.parameter, arg));
                }
                return new Application(func, arg);
            }
            return expr;
        }
        public enum TokenType
        {
            OPEN_BRACKET,
            CLOSE_BRACKET,
            LAMBDA,
            DOT,
            ID,
            EOF
        }
        public record Token(TokenType type, string value = "");
        public class Lexer(string input)
        {
            private int _pos = 0;
            private Token Next()
            {
                while (_pos < input.Length && char.IsWhiteSpace(input[_pos])) _pos++;
                if (_pos >= input.Length) return new Token(TokenType.EOF); 
                char c = input[_pos++];
                return c switch
                {
                    '\\' => new Token(TokenType.LAMBDA),
                    '.' => new Token(TokenType.DOT),
                    '(' => new Token(TokenType.OPEN_BRACKET),
                    ')' => new Token(TokenType.CLOSE_BRACKET),
                    _ => new Token(TokenType.ID, c.ToString())
                };
            }
            public Token[] Tokenise()
            {
                List<Token> tokens = [];
                while (true)
                {
                    Token current_token = Next();
                    tokens.Add(current_token);
                    if (current_token.type == TokenType.EOF) break;
                }
                return tokens.ToArray();
            }
        }
    }
}

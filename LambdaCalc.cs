using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace LambdaCalc
{
    public abstract record Expression;
    public record Variable(string name) : Expression;
    public record Function(string parameter, Expression body) : Expression;
    public record Application(Expression function, Expression argument) : Expression;
    static class LambdaCalc
    {
        public static string ExpressionToString(Expression expr)
        {
            return expr switch
            {
                Variable v => $"{v.name}",
                Function f => $"\\{f.parameter}.{ExpressionToString(f.body)}",
                Application a => $"({ExpressionToString(a.function)}){ExpressionToString(a.argument)}",
                _ => throw new ArgumentException("Unknown expression type")
            };
        }
        public static Expression Substitute(Expression expr, string target, Expression replacement)
        {
            return expr switch
            {
                Variable v => (v.name == target) ? replacement : v,
                Function f => (f.parameter == target) ? f : f with { body = Substitute(f.body, target, replacement) },
                Application a => new Application(
                    Substitute(a.function, target, replacement),
                    Substitute(a.argument, target, replacement)
                    ),
                _ => throw new ArgumentException("Unknown expression type")
            };
        }

        public static Expression Reduce(Expression expr)
        {
            if (expr is Application app)
            {
                var func = Reduce(app.function);
                var arg = Reduce(app.argument);
                if (func is Function f)
                {
                    return Reduce(Substitute(f.body, f.parameter, arg));
                }
                return new Application(func, arg);
            }
            return expr;
        }
    }
}

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
        public static Expression BetaReduce(Expression expr, string target, Expression replacement)
        {
            return expr switch
            {
                Variable v => (v.name == target) ? replacement : v,
                Function f => (f.parameter == target) ? f : f with { body = BetaReduce(f.body, target, replacement) },
                Application a => new Application(
                    BetaReduce(a.function, target, replacement),
                    BetaReduce(a.argument, target, replacement)
                    ),
                _ => throw new ArgumentException("Unknown expression type")
            };
        }
        public static Expression Evaluate(Expression expr)
        {
            if (expr is Application app)
            {
                var func = Evaluate(app.function);
                var arg = Evaluate(app.argument);
                if (func is Function f)
                {
                    return Evaluate(BetaReduce(f.body, f.parameter, arg));
                }
                return new Application(func, arg);
            }
            return expr;
        }
    }
}

using LambdaCalc;
using static LambdaCalc.LambdaCalc;

Function identity = new("I", new Variable("I"));
Variable v = new("V");

Application a = new(identity, v);

Console.WriteLine(ExpressionToString(a));

Console.WriteLine(ExpressionToString(Reduce(a)));
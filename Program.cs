using LambdaCalc;
using System.Runtime.CompilerServices;
using static LambdaCalc.LambdaCalc;

Function identity = new("I", new Variable("I"));
Variable v = new("V");

Expression a = new Application(identity, new Application(identity, v));

Console.WriteLine(a);
Console.WriteLine(Evaluate(a));

var program = "((\\x.x)a)";
var lexer = new Lexer(program);

Token[] tokens = lexer.Tokenise();
Console.WriteLine("Done.");

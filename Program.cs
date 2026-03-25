using LambdaCalc;
using static LambdaCalc.LambdaCalc;

Function identity = new("I", new Variable("I"));
Variable v = new("V");

Expression a = new Application(identity, new Application(identity, v));
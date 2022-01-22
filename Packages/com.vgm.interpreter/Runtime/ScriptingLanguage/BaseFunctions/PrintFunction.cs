using System;
using ScriptingLanguage.Interpreter;

namespace ScriptingLanguage.BaseFunctions
{
	class PrintFunction : IFunction
	{
		public Scope ScopeTemplate
		{
			get
			{
				var scope = new Scope();
				scope.AddVariable("str", null);
				return scope;
			}
		}

		public string[] ParameterNames { get; set; } = new string[] { "str" };

		public object Execute(Scope scope)
		{
			Console.WriteLine($"{scope.GetVariable("str")}");
			return null;
		}
	}
}

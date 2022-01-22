using ScriptingLanguage.Interpreter;
using ScriptingLanguage.Parser;
using ScriptingLanguage.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static ScriptingLanguage.Parser.Parser;
using static ScriptingLanguage.Utils;

namespace ScriptingLanguage.REPL
{
    public class REPL
    {
        public class PrintBuffer : IFunction
        {
            const string paramName = "arg";
            public Scope ScopeTemplate
            {
                get
                { 
                    var scope = new Scope();
                    scope.AddVariable(paramName, null);
                    return scope;
                }
            }

            public string[] ParameterNames => new[] { paramName };

            public List<string> Buffer { get; } = new List<string>();

            public void Clear() 
            {
                Buffer.Clear();
            }

            public object Execute(Scope scope)
            {
                Buffer.Add($"{scope.GetVariable(paramName)}");
                return null;
            }
        }

        private Parser.Parser parser;
        private Session session;
        private Interpreter.Interpreter interpreter;
        private string buffer = "";
        private readonly PrintBuffer printBuffer = new PrintBuffer();

        public REPL(ParserTable pt)
        {
            parser = new Parser.Parser { ParserTable = pt };
            InitSession("");
        }

        private void InitSession(string workingDir)
        {
            session = new Session(workingDir, parser, new Session.SessionFunc { Name = "print", Function = printBuffer });
            interpreter = new Interpreter.Interpreter(session);
        }

        MethodInfo[] metaCommandMethods;
        IEnumerable<MethodInfo> MetaCommands
        {
            get 
            {
                IEnumerable<MethodInfo> findMetaCommands()
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

                    foreach (var assembly in assemblies)
                    {
                        var types = assembly.GetTypes();
                        foreach (var type in types) 
                        {
                            var methods = type.GetMethods(bindingFlags);
                            foreach (var method in methods) 
                            {
                                var attribute = method.GetCustomAttribute<MetaCommandAttribute>();
                                if (attribute != null) 
                                {
                                    yield return method;
                                }
                            }
                        }
                    }
                }

                if (metaCommandMethods == null) 
                {
                    metaCommandMethods = findMetaCommands().ToArray();
                }

                return metaCommandMethods;
            }
        }

        bool HandleMetaCommand(string command, out IEnumerable<string> output)
        {
            var trimmed = command.Trim();
            IEnumerable<string> createEnumerable(params string[] args) 
            {
                foreach (var arg in args)
                    yield return arg;
            }

            if (!trimmed.StartsWith(":") || trimmed.Length == 1 || trimmed[1] == ' ')
            {
                output = createEnumerable();
                return false;
            }



            trimmed = trimmed.Substring(1);
            var words = trimmed.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x));
            var commandName = words.First();
            var commandArgs = words.Skip(1);

            var method = MetaCommands.FirstOrDefault(x => x.GetCustomAttribute<MetaCommandAttribute>().Name == commandName);
            if (method == null) 
            {
                output = createEnumerable($"Command {commandName} not found!");
                return true;
            }

            output = method.Invoke(null, new object[] { this, commandArgs }) as IEnumerable<string>;
            return true;
        }

        public IEnumerable<string> HandleCommand(string command) 
        {
            IEnumerable<string> output;
            bool metaCommand = HandleMetaCommand(command, out output);
            if (metaCommand) 
            {
                yield return command;
                foreach (var str in output) 
                {
                    yield return str;
                }
                yield break;
            }

            if (string.IsNullOrWhiteSpace(command)) 
            {
                buffer = "";
                yield break;
            }
            buffer += $"\n{command}";
            var scriptId = new ScriptId { Script = buffer };
            printBuffer.Clear();

            yield return command;
            
            LanguageException exception = null;
            object res = null;
            try
            {
                res = interpreter.RunScript(session.GetWorkingScope(), scriptId);
            }
            catch (LanguageException e) 
            {
                exception = e;
            }

            foreach (var str in printBuffer.Buffer) 
            {
                yield return str;
            }
          
            if (exception is ExpectsSymbolException
                && exception.ScriptId == scriptId
                && exception.CodeIndex.Index == buffer.Length) 
            {
                yield return "...";
                yield break;
            }

            buffer = "";
            if (exception != null)
            {
                yield return exception.Message;
                throw exception;
            }
            yield return $"{(res == null ? "null" : res)}";
        }

        [MetaCommand(Name = "reset_session")]
        public static IEnumerable<string> ResetSession(object context, IEnumerable<string> args)
        {
            var repl = context as REPL;
            if (repl == null)
            {
                yield break;
            }

            var workingDir = args.FirstOrDefault();
            if (string.IsNullOrEmpty(workingDir)) {
                yield break;
            }

            repl.InitSession(workingDir);
            yield return $"Opened REPL session with working directory: {workingDir}";
        }
    }
}

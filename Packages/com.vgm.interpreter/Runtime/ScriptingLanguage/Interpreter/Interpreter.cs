using System;
using System.IO;
using System.Linq;
using ScriptingLanguage.BaseFunctions;
using ScriptingLanguage.Parser;
using ScriptingLanguage.Tokenizer;

namespace ScriptingLanguage.Interpreter
{
    public class Interpreter
    {
        const string grammarDefinition = "grammar.txt";
        const string parserTableData = "parserTable.dat";

        public static Scope GetStaticScope()
        {
            return StaticScope;
        }

        private static Scope StaticScope
        {
            get
            {
                var staticScope = new Scope();
                staticScope.AddVariable("print", new PrintFunction());
                staticScope.AddVariable("len", new LengthFunction());
                staticScope.AddVariable("new", new CreateObjectFunction());
                staticScope.AddVariable("getDelegate", new GetEventType());
                staticScope.AddVariable("createDelegate", new CreateDelegateFunction());
                staticScope.AddVariable("subscribeToEvent", new SubscribeToEventFunction());
                staticScope.AddVariable("unsubscribeFromEvent", new UnsubscribeFromEventFunction());
                staticScope.AddVariable("getTypeMembers", new GetTypeMembersFunction());
                staticScope.AddVariable("getType", new GetTypeFunction());
                staticScope.AddVariable("createCoroutine", new CreateCoroutineFunction());
                staticScope.AddVariable("foreach", new ForeachFunction());
                return staticScope;
            }
        }

        public IProgramNodeProcessor OperationGroupProcessor = new OperationGroupProcessor();

        ITokenizer tokenizer = Tokenizer.CombinedTokenizer.DefaultTokenizer;
        Parser.Parser parser;
        public Interpreter(Session session)
        {
            if (session.Parser != null)
            {
                parser = session.Parser;
                return;
            }
            if (!File.Exists(session.WorkingDir + parserTableData)) 
            {
                var grammar = Grammar.ReadGrammarFromFile(session.WorkingDir + grammarDefinition);
                var parserTable = new ParserTable(grammar);
                bool validTable =  parserTable.Validate();
                parserTable.Serialize(session.WorkingDir + parserTableData);
                parser = new Parser.Parser { ParserTable = parserTable };
            }
            else 
            {
                var parserTable = ParserTable.Deserialize(session.WorkingDir + parserTableData);
                parser = new Parser.Parser { ParserTable = parserTable };
            }
        }
        
        public object RunScriptFile(string scriptToRunFullPath, Scope scope)
        {
            var entryScript = File.ReadAllText(scriptToRunFullPath);
            return RunScript(scope, new ScriptId { Filename = scriptToRunFullPath, Script = entryScript });
        }
        
        public object RunScript(Scope scope, ScriptId scriptId)
        {
            var tokenized = Utils.TokenizeText(scriptId, new SimpleToken { Name = "Terminal" }).ToList();
            scriptId.TokenizedScript = tokenized;
            var processed = tokenizer.Tokenize(tokenized).ToList();
            var programTree = parser.ParseProgram(processed);
            
            return EvaluateProgramNode(programTree, scope);
        }

        public object EvaluateProgramNode(ProgramNode programNode, Scope scope) 
        {
            var curNode = programNode;
            if (curNode.Token.Name != "Root") 
            {
                throw new Exception("Invalid Program");
            }
            curNode = curNode.Children[0];
            object res = null;
            NodeProcessor.ExecuteProgramNodeProcessor(OperationGroupProcessor, curNode, scope, ref res);
            return res;
        }
    }
}

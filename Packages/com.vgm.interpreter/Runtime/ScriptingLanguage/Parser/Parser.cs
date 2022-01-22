using System;
using System.Collections.Generic;
using System.Linq;
using ScriptingLanguage.Tokenizer;
using static ScriptingLanguage.Utils;

namespace ScriptingLanguage.Parser
{
    public class Parser
    {
        public interface IParseException
        {
        }

        public class ExpectsSymbolException : LanguageException, IParseException
        {
            public IEnumerable<string> ExpectedSymbols;
            public ExpectsSymbolException(ScriptId scriptId, IIndexed codeIndex, IEnumerable<string> expectedSymbols) : base("", scriptId, codeIndex)
            {
                ExpectedSymbols = expectedSymbols;
            }

            public override string GetErrorMessage(bool printLineNumbers)
            {
                string expecting = "";
                foreach (var symbol in ExpectedSymbols) 
                {
                    expecting += $", {symbol}";
                }
                expecting = expecting.Substring(2);
                return $"Expecting one of: {expecting}\n{GetCodeSample(CodeIndex, ScriptId, printLineNumbers)}";
            }
        }

        public class CantProceedParsingException : LanguageException, IParseException
        {
            public CantProceedParsingException(ScriptId scriptId, IIndexed codeIndex) : base("", scriptId, codeIndex) { }

            public override string GetErrorMessage(bool printLineNumbers)
            {
                return $"Syntax error\n{GetCodeSample(CodeIndex, ScriptId, printLineNumbers)}";
            }
        }

        public ParserTable ParserTable;
        public ProgramNode ParseProgram(IEnumerable<IToken> program)
        {
            IEnumerator<IToken> script = program.GetEnumerator();
            script.MoveNext();

            Stack<int> stateStack = new Stack<int>();
            stateStack.Push(ParserTable.InitialState);

            Stack<ProgramNode> treeStack = new Stack<ProgramNode>();

            bool endOfProgram = false;
            IToken lastRead = null;

            while (stateStack.Peek() != ParserTable.FinalState) 
            {
                if (endOfProgram)
                {
                    var nextSymbols = ParserTable.ParserActions.Where(x => x.CurrentState == stateStack.Peek()).Select(x => x.NextSymbol);
                    IIndexed index = lastRead as IIndexed;
                    var scriptSource = (lastRead as IScriptSourceHolder).ScriptSource;
                    if (!nextSymbols.Any())
                    {
                        throw new CantProceedParsingException(scriptSource, index);
                    }
                    else 
                    {
                        throw new ExpectsSymbolException(scriptSource, index, nextSymbols);
                    }
                }
                var curToken = script.Current;
                lastRead = curToken;
                var action = ParserTable.ParserActions.FirstOrDefault(x => x.CurrentState == stateStack.Peek() && x.NextSymbol == curToken.Name);
                if (action == null) 
                {
                    IIndexed index = curToken as IIndexed;
                    var scriptId = (curToken as IScriptSourceHolder).ScriptSource;
                    var nextSymbols = ParserTable.ParserActions.Where(x => x.CurrentState == stateStack.Peek()).Select(x => x.NextSymbol);
                    if (!nextSymbols.Any())
                    {
                        throw new CantProceedParsingException(scriptId, index);
                    }
                    else
                    {
                        throw new ExpectsSymbolException(scriptId, index, nextSymbols);
                    }
                }

                if (action.ActionType == ActionType.Shift) 
                {
                    endOfProgram = !script.MoveNext();
                    stateStack.Push(action.NextState);
                    treeStack.Push(new ProgramNode { Token = curToken, RuleId = -1 });
                    continue;
                }

                var tmpStateStack = new Stack<int>();
                var tmpTreeStack = new Stack<ProgramNode>();
                for (int i = 0; i < action.ReduceSymbols; ++i) 
                {
                    tmpStateStack.Push(stateStack.Pop());
                    tmpTreeStack.Push(treeStack.Pop());
                }
                var reduceSymbol = action.ReduceSymbol;
                var reduceNode = new ProgramNode { RuleId = action.RuleId, Token = new SimpleToken { Name = reduceSymbol } };
                while (tmpTreeStack.Any()) 
                {
                    reduceNode.Children.Add(tmpTreeStack.Pop());
                }
                var shiftAfterReduceAction = ParserTable.ParserActions.FirstOrDefault(x => x.CurrentState == stateStack.Peek() && x.NextSymbol == reduceSymbol);
                if (shiftAfterReduceAction == null) 
                {
                    var nextSymbols = ParserTable.ParserActions.Where(x => x.CurrentState == stateStack.Peek()).Select(x => x.NextSymbol);
                    if (!nextSymbols.Any())
                    {
                        throw new CantProceedParsingException(reduceNode.GetScriptSource(), reduceNode.GetCodeIndex());
                    }
                    else
                    {
                        throw new ExpectsSymbolException(reduceNode.GetScriptSource(), reduceNode.GetCodeIndex(), nextSymbols);
                    }
                }
                treeStack.Push(reduceNode);
                stateStack.Push(shiftAfterReduceAction.NextState);
            }

            return treeStack.Last();
        }
    }
}

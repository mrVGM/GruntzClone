using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ScriptingLanguage.Parser
{
    public enum ActionType
    {
        Shift,
        Reduce
    }
    [Serializable]
    public class ParserAction
    {
        public ActionType ActionType;
        public string NextSymbol;
        public int CurrentState;
        public int NextState;
        public int ReduceSymbols;
        public int RuleId;
        public string ReduceSymbol;
    }
    [Serializable]
    public class ParserTable
    {
        public List<ParserAction> ParserActions = new List<ParserAction>();
        public int InitialState;
        public int FinalState;
        public ParserTable() { }
        public static ParserTable Deserialize(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            var f = File.Open(fileName, FileMode.Open);
            var res = bf.Deserialize(f) as ParserTable;
            f.Close();
            return res;
        }

        public static ParserTable Deserialize(byte[] bytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            var stream = new MemoryStream(bytes);
            return bf.Deserialize(stream) as ParserTable;
        }

        public void Serialize(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            var f = File.Open(fileName, FileMode.Create);
            bf.Serialize(f, this);
            f.Close();
        }

        public ParserTable(Grammar grammar)
        {
            var states = grammar.GenerateParserStates();

            Dictionary<ParserState, int> stateIds = new Dictionary<ParserState, int>();
            int id = 0;
            foreach (var state in states) 
            {
                stateIds[state] = id++;
            }

            var initialState = new ParserState();
            initialState.Rules.Add(new AugmentedRule { Rule = grammar.InitialRule, DotPosition = 0, LookAheadSymbols = new HashSet<string>() });
            initialState.ExtendState(grammar);

            InitialState = stateIds[initialState];

            var finalState = new ParserState();
            finalState.Rules.Add(new AugmentedRule { Rule = grammar.InitialRule, DotPosition = grammar.InitialRule.RHS.Length, LookAheadSymbols = new HashSet<string>() });
            finalState.ExtendState(grammar);

            FinalState = stateIds[finalState];

            foreach (var state in states) 
            {
                var stateId = stateIds[state];
                var transtionSymbols = state.GetTransitionSymbols();
                foreach (var symbol in transtionSymbols) 
                {
                    var nextState = state.TransitionWith(symbol, grammar);
                    var nextStateId = stateIds[nextState];
                    var parserAction = new ParserAction { 
                        ActionType = ActionType.Shift, 
                        NextSymbol = symbol,
                        CurrentState = stateId, 
                        NextState = nextStateId,
                    };
                    ParserActions.Add(parserAction);
                }

                var reduceRules = state.Rules.Where(x => x.DotPosition == x.Rule.RHS.Length);
                foreach (var reduceRule in reduceRules) 
                {
                    foreach (var la in reduceRule.LookAheadSymbols) 
                    {
                        var parserAction = new ParserAction {
                            ActionType = ActionType.Reduce,
                            NextSymbol = la,
                            CurrentState = stateId,
                            ReduceSymbols = reduceRule.Rule.RHS.Length,
                            RuleId = Array.IndexOf(grammar.Rules, reduceRule.Rule),
                            ReduceSymbol = reduceRule.Rule.LHS,
                        };
                        ParserActions.Add(parserAction);
                    }
                }
            }
        }

        public bool Validate() 
        {
            foreach (var parserAction in ParserActions) 
            {
                int stateId = parserAction.CurrentState;
                string nextSymbol = parserAction.NextSymbol;

                var allActions = ParserActions.Where(x => x.CurrentState == stateId && x.NextSymbol == nextSymbol);
                if (allActions.Count() > 1) 
                {
                    return false;
                }
            }
            return true;
        }
    }
}

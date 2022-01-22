using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ScriptingLanguage.Parser
{
    public class ParserState
    {
        public HashSet<AugmentedRule> Rules = new HashSet<AugmentedRule>();

        public bool Merge(ParserState other) 
        {
            bool res = false;
            foreach (var augmentRule in other.Rules) 
            {
                var assocciate = Rules.First(x => x == augmentRule);
                res |= assocciate.Merge(augmentRule);
            }
            return res;
        }

        public void ExtendState(Grammar grammar)
        {
            bool extensionStep() 
            {
                bool extended = false;
                foreach (var augmentedRule in Rules.ToArray())
                {
                    string curSymbol = null;
                    string nextSymbol = null;
                    int dotPos = augmentedRule.DotPosition;
                    if (dotPos < augmentedRule.Rule.RHS.Count()) 
                    {
                        curSymbol = augmentedRule.Rule.RHS[dotPos];
                    }
                    if (dotPos + 1 < augmentedRule.Rule.RHS.Count()) 
                    {
                        nextSymbol = augmentedRule.Rule.RHS[dotPos + 1];
                    }

                    if (curSymbol != null)
                    {
                        var rulesToExtend = grammar.Rules.Where(x => x.LHS == curSymbol);
                        foreach (var rule in rulesToExtend) 
                        {
                            var newRule = new AugmentedRule();
                            newRule.Rule = rule;
                            newRule.DotPosition = 0;
                            IEnumerable<string> la = augmentedRule.LookAheadSymbols;
                            if (nextSymbol != null) 
                            {
                                la = grammar.ProducibleTerminalsFrom(nextSymbol);
                            }
                            newRule.LookAheadSymbols = new HashSet<string>(la);

                            var associatedRule = Rules.FirstOrDefault(x => x == newRule);
                            if (associatedRule != null)
                            {
                                associatedRule.Merge(newRule);
                            }
                            else
                            {
                                Rules.Add(newRule);
                                extended = true;
                            }
                        }
                    }
                }
                return extended;
            }

            while (extensionStep()) { };
        }

        public IEnumerable<string> GetTransitionSymbols() 
        {
            IEnumerable<string> getSymbols()
            {
                foreach (var rule in Rules)
                {
                    if (rule.DotPosition < rule.Rule.RHS.Count())
                    {
                        yield return rule.Rule.RHS[rule.DotPosition];
                    }
                }
            }
            return getSymbols().Distinct();
        }

        public ParserState TransitionWith(string symbol, Grammar grammar) 
        {
            var transitionBy = Rules.Where(x => {
                if (x.DotPosition == x.Rule.RHS.Count())
                {
                    return false;
                }
                return x.Rule.RHS[x.DotPosition] == symbol;
            });

            var newRules = transitionBy.Select(x => {
                var rule = new AugmentedRule();
                rule.Rule = x.Rule;
                rule.DotPosition = x.DotPosition + 1;
                rule.LookAheadSymbols = new HashSet<string>(x.LookAheadSymbols);
                return rule;
            });

            var parserState = new ParserState();
            parserState.Rules = new HashSet<AugmentedRule>(newRules);
            parserState.ExtendState(grammar);
            return parserState;
        }

        public override bool Equals(object obj)
        {
            var otherHashSet = obj as ParserState;
            if (otherHashSet == null) 
            {
                return false;
            }

            if (Rules.Any(x => !otherHashSet.Rules.Contains(x))) 
            {
                return false;
            }

            if (otherHashSet.Rules.Any(x => !Rules.Contains(x)))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return 2045102464 + Rules.Count;
        }

        public static bool operator== (ParserState ps1, ParserState ps2) 
        {
            if ((object)ps1 == null) 
            {
                return (object)ps2 == null;
            }
            return ps1.Equals(ps2);
        }
        public static bool operator !=(ParserState ps1, ParserState ps2) 
        {
            return !(ps1 == ps2);
        }

        public override string ToString()
        {
            string str = "";
            foreach (var rule in Rules) 
            {
                str += rule + "\n";
            }
            return str;
        }
    }
}

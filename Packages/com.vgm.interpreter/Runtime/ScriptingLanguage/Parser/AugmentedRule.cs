using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptingLanguage.Parser
{
    public class AugmentedRule
    {
        public Rule Rule;
        public int DotPosition;
        public HashSet<string> LookAheadSymbols;

        public bool Merge(AugmentedRule other) 
        {
            if (this != other) 
            {
                throw new Exception("Merging different Rules!");
            }
            bool res = false;
            foreach (var symbol in other.LookAheadSymbols) 
            {
                res |= LookAheadSymbols.Add(symbol);
            }
            return res;
        }
        public override bool Equals(object obj)
        {
            return obj is AugmentedRule rule &&
                   Rule == rule.Rule &&
                   DotPosition == rule.DotPosition;
        }

        public override int GetHashCode()
        {
            int hashCode = 1007252558;
            hashCode = hashCode * -1521134295 + EqualityComparer<Rule>.Default.GetHashCode(Rule);
            hashCode = hashCode * -1521134295 + DotPosition.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(AugmentedRule r1, AugmentedRule r2)
        {
            if ((object)r1 == null) 
            {
                return (object)r2 == null;
            }
            return r1.Equals(r2);
        }

        public static bool operator !=(AugmentedRule r1, AugmentedRule r2)
        {
            return !(r1 == r2);
        }

        public override string ToString()
        {
            List<string> rhsList = Rule.RHS.ToList();
            rhsList.Insert(DotPosition, ".");
            string rhs = "";
            foreach (var s in rhsList) 
            {
                rhs += $"{s}|";
            }
            rhs = rhs.Substring(0, rhs.Length - 1);
            string la = "";
            foreach (var s in LookAheadSymbols) 
            {
                la += $"{s} ";
            }
            if (!string.IsNullOrEmpty(la))
            { 
                la = la.Substring(0, la.Length - 1); 
            }
            return $"{Rule.LHS} -> {rhs} {{{la}}}";
        }
    }
}

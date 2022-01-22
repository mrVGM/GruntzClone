using System.Collections.Generic;
using System.Linq;
using ScriptingLanguage.Tokenizer;

namespace ScriptingLanguage.Parser
{
    public class ProgramNode
    {
        public int RuleId = -1;
        public IToken Token;
        public List<ProgramNode> Children = new List<ProgramNode>();

        public bool MatchChildren(params string[] template) 
        {
            if (template.Length != Children.Count)
            {
                return false;
            }

            for (int i = 0; i < template.Length; ++i) 
            {
                if (Children[i].Token.Name != template[i]) 
                {
                    return false;
                }
            }
            return true;
        }

        public IIndexed GetCodeIndex() 
        {
            var indexed = Token as IIndexed;
            if (indexed != null) 
            {
                return indexed;
            }

            return Children.FirstOrDefault().GetCodeIndex();
        }
        public ScriptId GetScriptSource()
        {
            var indexed = Token as IScriptSourceHolder;
            if (indexed != null)
            {
                return indexed.ScriptSource;
            }

            return Children.FirstOrDefault().GetScriptSource();
        }
    }
}

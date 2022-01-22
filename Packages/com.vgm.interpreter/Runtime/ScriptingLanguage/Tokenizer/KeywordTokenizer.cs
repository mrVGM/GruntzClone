using System.Collections.Generic;
using System.Linq;

namespace ScriptingLanguage.Tokenizer
{
    public class KeywordTokenizer : ITokenizer
    {
        string[] Keywords;
        public KeywordTokenizer(params string[] keywords)
        {
            Keywords = keywords;
        }

        bool ReadKeyword(string keyword, IEnumerable<IndexedToken> script) 
        {
            var str = "";
            var beginning = script.Take(keyword.Length);
            var next = script.Skip(keyword.Length);
            foreach (var token in beginning) 
            {
                if (!Utils.IsNameSymbol(token))
                {
                    return false;
                }
                str += token.Name;
            }
            if (str != keyword) 
            {
                return false;
            }

            var nextToken = next.FirstOrDefault();
            if (nextToken != null && Utils.IsNameSymbol(nextToken))
            {
                return false;
            }

            return true;
        }

        IEnumerable<IndexedToken> TryReadSingleKeyword(IEnumerable<IndexedToken> script, out IndexedToken processedToken) 
        {
            var firstToken = script.FirstOrDefault();
            int index = firstToken.Index;
            var scriptSource = firstToken.ScriptSource;
            foreach (var keyword in Keywords) 
            {
                if (ReadKeyword(keyword, script)) 
                {
                    processedToken = new IndexedToken (index, scriptSource) { Name = keyword };
                    return script.Skip(keyword.Length);
                }
            }

            processedToken = script.First();
            return script.Skip(1); 
        }
        public IEnumerable<IndexedToken> Tokenize(IEnumerable<IndexedToken> script)
        {
            var left = script;
            while (left.Any()) 
            {
                IndexedToken token = null;
                left = TryReadSingleKeyword(left, out token);
                yield return token;
            }
        }
    }
}

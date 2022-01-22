using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptingLanguage.Tokenizer
{
    public class WordTokenizer : ITokenizer
    {
        IEnumerable<string> Words;
        public WordTokenizer(params string[] words)
        {
            Words = words.OrderByDescending(x => x.Length);
        }

        bool TryReadWord(string word, IEnumerable<IndexedToken> script)
        {
            var beginning = script.Take(word.Length);
            if (beginning.Count() < word.Length || beginning.Any(x => x.Name.Length > 1))
            {
                return false;
            }

            string str = "";
            foreach (var symbol in beginning) 
            {
                str += symbol.Name;
            }

            if (str != word) 
            {
                return false;
            }

            return true;
        }

        int ReadWord(IEnumerable<IndexedToken> script, out IndexedToken token)
        {
            var firstToken = script.FirstOrDefault();
            int index = script.FirstOrDefault().Index;
            var scriptSource = firstToken.ScriptSource;
            foreach (var word in Words)
            {
                if (TryReadWord(word, script)) 
                {
                    token = new IndexedToken(index, scriptSource) { Name = word };
                    return word.Length;
                }
            }

            token = script.First();
            return 1;
        }
        public IEnumerable<IndexedToken> Tokenize(IEnumerable<IndexedToken> script)
        {
            var left = script;
            while (left.Any()) 
            {
                IndexedToken token = null;
                int read = ReadWord(left, out token);
                yield return token;
                left = left.Skip(read);
            }
        }
    }
}

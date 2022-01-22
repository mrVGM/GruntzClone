using System;
using System.Collections.Generic;
using System.Linq;

namespace ScriptingLanguage.Tokenizer
{
    public class NumberTokenizer : ITokenizer
    {
        public IEnumerable<IndexedToken> TokenizeInt(IEnumerable<IndexedToken> script)
        {
            List<IndexedToken> buffer = new List<IndexedToken>();

            foreach (var token in script)
            {
                if (Utils.IsDigit(token))
                {
                    buffer.Add(token);
                }
                else if (buffer.Any())
                {
                    string str = "";
                    foreach (var t in buffer)
                    {
                        str += t.Name;
                    }
                    int index = buffer.FirstOrDefault().Index;
                    yield return new IndexedToken(index, token.ScriptSource) { Name = "Number", Data = str };
                    yield return token;
                    buffer.Clear();
                }
                else 
                {
                    yield return token;
                }
            }

            if (buffer.Any()) 
            {
                string str = "";
                foreach (var t in buffer)
                {
                    str += t.Name;
                }
                int index = buffer.FirstOrDefault().Index;
                yield return new IndexedToken(index, buffer.FirstOrDefault().ScriptSource) { Name = "Number", Data = str };
            }
        }

        int TryReadNumber(IEnumerable<IndexedToken> script, out IndexedToken processed) 
        {
            var tmp = script.Take(3).ToArray();
            if (tmp.Length == 3)
            {
                if (tmp[0].Name == "Number" && tmp[1].Name == "." && tmp[2].Name == "Number")
                {
                    int index = tmp[0].Index;
                    processed = new IndexedToken(index, tmp[0].ScriptSource) { Name = "Number", Data = $"{tmp[0].Data}.{tmp[2].Data}" };
                    return 3;
                }
            }

            processed = tmp[0];
            return 1;
        }
        public IEnumerable<IndexedToken> Tokenize(IEnumerable<IndexedToken> script)
        {
            var left = TokenizeInt(script);
            while (left.Any())
            {
                IndexedToken token = null;
                int toSkip = TryReadNumber(left, out token);
                yield return token;
                left = left.Skip(toSkip);
            }
        }
    }
}

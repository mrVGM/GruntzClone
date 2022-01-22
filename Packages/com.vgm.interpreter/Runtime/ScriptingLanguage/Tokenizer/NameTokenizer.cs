using System;
using System.Collections.Generic;
using System.Linq;

namespace ScriptingLanguage.Tokenizer
{
    public class NameTokenizer : ITokenizer
    {
        public IEnumerable<IndexedToken> Tokenize(IEnumerable<IndexedToken> script)
        {
            List<IndexedToken> buffer = new List<IndexedToken>();

            bool reading = false;
            foreach (var token in script) 
            {
                if (reading)
                {
                    if (Utils.IsNameSymbol(token))
                    {
                        buffer.Add(token);
                    }
                    else 
                    {
                        string str = "";
                        foreach (var t in buffer) 
                        {
                            str += t.Name;
                        }
                        int index = buffer.FirstOrDefault().Index;
                        yield return new IndexedToken(index, token.ScriptSource) { Name = "Name", Data = str };
                        buffer.Clear();
                        reading = false;
                        yield return token;
                    }
                }
                else
                {
                    if (Utils.IsNameStartSymbol(token))
                    {
                        buffer.Add(token);
                        reading = true;
                    }
                    else 
                    {
                        yield return token;
                    }
                }
            }
            if (buffer.Any())
            {
                string str = "";
                foreach (var t in buffer)
                {
                    str += t.Name;
                }
                var firstToken = buffer.FirstOrDefault();
                int index = firstToken.Index;
                var scriptSource = firstToken.ScriptSource;
                yield return new IndexedToken(index, scriptSource) { Name = "Name", Data = str };
            }
        }
    }
}

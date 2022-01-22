using System;
using System.Collections.Generic;

namespace ScriptingLanguage.Tokenizer
{
    public class BlankSpaceSkipTokenizer : ITokenizer
    {
        public IEnumerable<IndexedToken> Tokenize(IEnumerable<IndexedToken> script)
        {
            foreach (var token in script) 
            {
                if (token.Name == NewLineTokenizer.LF || token.Name == NewLineTokenizer.CRLF
                    || token.Name == " " || token.Name == "\t")
                {
                    continue;
                }
                yield return token;
            }
        }
    }
}

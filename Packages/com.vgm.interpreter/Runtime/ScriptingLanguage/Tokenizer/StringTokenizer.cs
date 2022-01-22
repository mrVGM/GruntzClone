using System;
using System.Collections.Generic;

namespace ScriptingLanguage.Tokenizer
{
    public class StringTokenizer : ITokenizer
    {
        enum State 
        {
            Default,
            InString,
            EscapingToken
        };
        public IEnumerable<IndexedToken> Tokenize(IEnumerable<IndexedToken> script)
        {
            var state = State.Default;
            List<IndexedToken> buffer = new List<IndexedToken>();
            int stringIndex = -1;

            foreach (var token in script)
            {
                if (token.Name == NewLineTokenizer.LF || token.Name == NewLineTokenizer.CRLF)
                {
                    if (state != State.Default)
                    {
                        throw new TokenizerException("Open Quotes!");
                    }

                    yield return token;
                    continue;
                }

                if (token.Name == "\\") 
                {
                    if (state == State.InString) 
                    {
                        state = State.EscapingToken;
                        continue;
                    }
                }

                if (token.Name == "\"") 
                {
                    if (state == State.Default) 
                    {
                        buffer.Clear();
                        state = State.InString;
                        stringIndex = token.Index;
                        continue;
                    }

                    if (state == State.InString) 
                    {
                        var dataStr = "";
                        foreach (var symbol in buffer) 
                        {
                            dataStr += symbol.Name;
                        }

                        var stringToken = new IndexedToken (stringIndex, token.ScriptSource) {
                            Name = "String",
                            Data = dataStr
                        };
                        yield return stringToken;
                        state = State.Default;
                        continue;
                    }
                }

                if (state != State.Default)
                {
                    buffer.Add(token);
                    if (state == State.EscapingToken)
                    {
                        state = State.InString;
                    }
                    continue;
                }

                yield return token;
            }

            if (state != State.Default) 
            {
                throw new TokenizerException("Open Quotes!");
            }
        }
    }
}

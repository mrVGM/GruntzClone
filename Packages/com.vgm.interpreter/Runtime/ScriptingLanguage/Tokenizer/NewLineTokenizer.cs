using System.Collections.Generic;

namespace ScriptingLanguage.Tokenizer
{
    public class NewLineTokenizer : ITokenizer
    {
        public const string CRLF = "\r\n";
        public const string LF = "\n";
        IEnumerable<IndexedToken> TwoSymbolNewLine(IEnumerable<IndexedToken> script) 
        {
            IndexedToken oneSymbolRead = null;
            string firstSymbol = CRLF[0].ToString();
            string secondSymbol = CRLF[1].ToString();
            foreach (var symbol in script)
            {
                if (oneSymbolRead == null && symbol.Name == firstSymbol)
                {
                    oneSymbolRead = symbol;
                    continue;
                }
                if (oneSymbolRead != null) 
                {
                    if (symbol.Name == secondSymbol) 
                    {
                        yield return new IndexedToken(oneSymbolRead.Index, symbol.ScriptSource) { Name = CRLF };
                    }
                    else 
                    {
                        yield return oneSymbolRead;
                        yield return symbol;
                    }
                    oneSymbolRead = null;
                    continue;
                }

                yield return symbol;
            }
        }
        public IEnumerable<IndexedToken> Tokenize(IEnumerable<IndexedToken> script)
        {
            return TwoSymbolNewLine(script);
        }
    }
}

using System.Collections.Generic;

namespace ScriptingLanguage.Tokenizer
{
    public interface ITokenizer
    {
        IEnumerable<IndexedToken> Tokenize(IEnumerable<IndexedToken> script);
    }
}

using System.Collections.Generic;
using System.Linq;

namespace ScriptingLanguage.Tokenizer
{
    public class LineTokenizer : ITokenizer
    {
        public IEnumerable<IndexedToken> Tokenize(IEnumerable<IndexedToken> script)
        {
            if (!script.Any()) {
                yield break;
            }

            var scriptSource = script.First().ScriptSource;

            int index = 1;
            var curLine = new List<IndexedToken>();
            foreach (var token in script) {
                curLine.Add(token);
                if (token.Name == NewLineTokenizer.LF || token.Name == NewLineTokenizer.CRLF) {
                    var line = new IndexedToken(index++, scriptSource) { Name = "Line", Data = curLine };
                    yield return line;
                    curLine = new List<IndexedToken>();
                }
            }
            if (curLine.Any()) {
                var line = new IndexedToken(index, scriptSource) { Name = "Line", Data = curLine };
                yield return line;
            }
        }
    }
}

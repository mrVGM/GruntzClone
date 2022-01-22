using System.Collections.Generic;
using System.Linq;

namespace ScriptingLanguage.Tokenizer
{
    public class CommentsTokenizer : ITokenizer
    {
        const string openCommentSequence = "//";
        enum State
        {
            OutsideComment,
            FirstCharacterRead,
            InsideComment,
        };
        public IEnumerable<IndexedToken> Tokenize(IEnumerable<IndexedToken> script)
        {   
            var state = State.OutsideComment;
            IndexedToken tmpToken = null;

            foreach (var token in script) {
                if (state == State.OutsideComment) {
                    if (token.Name == openCommentSequence[0].ToString()) {
                        tmpToken = token;
                        state = State.FirstCharacterRead;
                    } else {
                        yield return token;
                    }
                    continue;
                }
                if (state == State.FirstCharacterRead) {
                    if (token.Name != openCommentSequence[1].ToString()) {
                        yield return tmpToken;
                        yield return token;
                        state = State.OutsideComment;
                    } else {
                        state = State.InsideComment;
                    }
                    continue;
                }
                if (state == State.InsideComment) {
                    if (token.Name == NewLineTokenizer.LF || token.Name == NewLineTokenizer.CRLF) {
                        state = State.OutsideComment;
                    }
                    continue;
                }
            }
        }
    }
}

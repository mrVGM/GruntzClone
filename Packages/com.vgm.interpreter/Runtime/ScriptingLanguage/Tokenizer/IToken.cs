using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptingLanguage.Tokenizer
{
	public interface IToken
	{
		string Name { get; }
		object Data { get; }
	}

	public interface IIndexed 
	{
		int Index { get; set; }
	}

	public interface IScriptSourceHolder
	{
		ScriptId ScriptSource { get; }
	}

	public class ScriptId 
	{
        public string Filename;
		public string Script;
		public IEnumerable<IndexedToken> TokenizedScript = Enumerable.Empty<IndexedToken>();

		private ITokenizer _tokenizer = new CombinedTokenizer(new NewLineTokenizer(), new LineTokenizer());
		public IEnumerable<IndexedToken> Lines => _tokenizer.Tokenize(TokenizedScript);

		public int GetLineNumber(IIndexed index)
		{
			var line = GetLine(index);
			return line.Index;
		}
		public IndexedToken GetLine(IIndexed index)
		{
			foreach (var line in Lines) {
				var contents = line.Data as List<IndexedToken>;
				if (contents.Any(x => x.Index == index.Index)) {
					return line;
				}
			}
			return null;
		}

		public int GetLineOffset(IIndexed index)
		{
			var line = GetLine(index);
			var contents = line.Data as List<IndexedToken>;
			var first = contents.First();
			return index.Index - first.Index;
		}

		public string GetStringOfLine(IndexedToken line)
		{
			string str = "";
			var contents = line.Data as List<IndexedToken>;
			foreach (var token in contents)
			{
				if (token.Name == "Terminal") {
					continue;
				}
				str += token.Name;
			}

			return str;
		}
	}

	public class SimpleToken : IToken
	{
		public string Name { get; set; }
		public object Data { get; set; }
	}

	public class IndexedToken : IToken, IIndexed, IScriptSourceHolder
	{
		public string Name { get; set; }
		public object Data { get; set; }
		public int Index { get; set; }

		public ScriptId ScriptSource { get; private set; }

		public IndexedToken(int index, ScriptId scriptSource) 
		{
			Index = index;
			ScriptSource = scriptSource;
		}
	}
}

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Windows.Forms;
using static SEBasicIV.SyntaxEditor;

namespace SEBasicIV
{
    internal class parserErrorListener : IAntlrErrorListener<IToken>
    {
        private List<string> syntaxErrors = new List<string>();

        public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            string myMsg = "Parser Error at Line: " + line + " - Position: " + charPositionInLine + " - " + msg;
            syntaxErrors.Add(myMsg);
        }

        public List<string> getSyntaxErrors()
        {
            return syntaxErrors;
        }
    }
}

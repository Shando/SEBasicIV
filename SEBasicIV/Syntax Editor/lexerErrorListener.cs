using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SEBasicIV
{
    internal class lexerErrorListener : ConsoleErrorListener<int>
    {
        private List<string> syntaxErrors = new List<string>();

        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] int offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            string myMsg = "Lexer Error at Line: " + line + " - Position: " + charPositionInLine + " - " + msg;

//            MessageBox.Show(myMsg, msg);
            syntaxErrors.Add(myMsg);
        }

        public List<string> getSyntaxErrors()
        {
            return syntaxErrors;
        }
    }
}
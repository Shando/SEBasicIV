using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FastColoredTextBoxNS;

namespace SEBasicIV
{
    public partial class SyntaxEditor : Form
    {
        public bool InfoShown = false;
        private string _path = "";
        private readonly About about = new About();
        private bool _isFullScreen = true;
        public bool bBasic = true;
        public bool bLightTheme = true;

        private readonly Regex basicLineNumRegex = new Regex(@"^[0-9]*[\s]*", RegexOptions.Multiline | RegexOptions.Compiled);
        private readonly Regex basicStringRegex = new Regex(@"""[^""]*\""", RegexOptions.Singleline | RegexOptions.Compiled);
        private readonly Regex basicCommentRegex = new Regex(@"('|REM)(\s+)?.*", RegexOptions.Compiled);
        private readonly Regex basicOperatorRegex = new Regex(@"\*|-|\/|=|<=|>=|<|>|<>", RegexOptions.Compiled);
        private readonly Regex basicKeywordsRegex = new Regex(@"\b(BEEP|BSAVE|CALL|CHAIN|CIRCLE|CLOSE|CLOSE#|CLO\.|CLS|COLOR|COL\.|COM|COPY|C\.|DATA|DA\.|DATE\$|DEF SEG|DEF\.|DIM|DOKE|D\.|DRAW|EDIT|ED\.|ELSE|EL\.|END|ERASE|FIELD|FOR|F\.|GET|GOSUB|GOS\.|GO SUB|GOTO|G\.|IF|INPUT|I\.|KEY|LET|LINE INPUT|LINE|LIST|LOCATE|L\.|LSET|NEXT|NOISE|OLD|ON ERROR|ERROR|E\.|ON|OPEN|OP\.|OPTION BASE|OUT|O\.|PAINT|PALETTE USING|PALETTE|PA\.|PEN|PLAY|POKE|PO\.|PRESET|PSET|PUT|RANDOMIZE|RA\.|READ|RESTORE|RES\.|RESUME|RETURN|RET\.|RSET|SCREEN|SC\.|SEEK|SE\.|SOUND|SO\.|STEP|STRIG|SWAP|THEN|TH\.|TIME\$|TIMER|TO|VIEW PRINT|PRINT|PR\.|VIEW|WAIT|WA\.|WEND|WE\.|WHILE|W\.|WIDTH|WINDOW|WRITE)\b", RegexOptions.Compiled);
        private readonly Regex basicDefFnRegex = new Regex(@"\b(DEF FN)", RegexOptions.Compiled);
        private readonly Regex basicFunctionsRegex = new Regex(@"\b(ABS|ASC|ACOS|AC\.|ASIN|AS\.|ATAN|AT.|CHR\$|COS|DEEK|EOF|EO\.|EXP|FIX|FN|FRE|INKEY\$|INK\.|INPUT\$|INP|INSTR|INS\.|INT|LEFT\$|LEF.|LEN|LOC|LOF|LOG|MID\$|MI\.|MOUSE|NMI|PEEK|PEN|PE\.|PI|PMAP|POINT|POS|RIGHT$|RI\.|RND|SGN|SIN|SPACE\$|SPC|SQR|STICK|STRING\$|STRIG|STR\$|STR\.|TAB|TAN|TIMER|USR|U\.|VAL|VAL$|V\.|VARPTR\$|VARPTR)\b", RegexOptions.Compiled);
        private readonly Regex basicCommandsRegex = new Regex(@"\b(AUTO|BLOAD|BL\.|CHDIR|CH\.|CLEAR|CLE\.|CONT|DELETE|DE\.|FILES|FO\.|KILL|K\.|LIST|LI\.|LOAD|LOCK|MERGE|ME\.|MKDIR|M\.|NAME|NA\.|NEW|N\.|OLD|PCOPY|RENUM|REN\.|RESET|RMDIR|RM\.|RUN|SAVE|SA\.|STOP|S\.|TERM|TRACE|T\.|UNLOCK)\b", RegexOptions.Compiled);
        private readonly Regex basicVariablesRegex = new Regex(@"\b(AND|A\.|CSRLIN|ERL|ERR|OR|XOR|X\.|[A-Z][0-9A-Z]*\$?)\b", RegexOptions.Compiled);
        private readonly Regex basicNumbersRegex = new Regex(@"\b[\d+]?[\.]?\d*([eE]\-?\d+)?\b|$[a-fA-F\d]+\b|\b\@[0-7]+\b|\b\%[0|1]+", RegexOptions.Compiled);

        private readonly Regex assemblyStringRegex = new Regex(@"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
        private readonly Regex assemblyCommentRegex = new Regex(@"(;.*)", RegexOptions.Multiline);
        private readonly Regex assemblyKeywordsRegex = new Regex(@"\b(org|endm|defb|defm|defs|defw|db|dm|ds|dw|else|endif|if|endm|end|seek|incbin|macro)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex assemblyInstructionsRegex = new Regex(@"\b(ADC|ADD|AND|BIT|CALL C|CALL M|CALL NC|CALL NZ|CALL PE|CALL PO|CALL P|CALL Z|CALL|CCF|CPDR|CPD|CPIR|CPI|CPL|CP|DAA|DEC|DI|DJNZ|EI|EXX|EX|HALT|IM|INC|INDR|IND|INIR|INI|IN|JP C|JP M|JP NC|JP NZ|JP PE|JP PO|JP P|JP Z|JP|JR C|JR NC|JR NZ|JR Z|JR|LDDR|LDD|LDIR|LDI|LD|LD|NEG|NOP|OR|OTDR|OTIR|OUTD|OUTI|OUT|POP|PUSH|RES|RET C|RET M|RET NC|RET NZ|RET PE|RET PO|RET P|RET Z|RETI|RETN|RET|RLA|RLCA|RLD|RLC|RL|RRA|RRCA|RRC|RRD|RR|RST|SBC|SCF|SET|SLA|SLL|SL1|SRA|SRL|SUB|XOR)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex assemblyRegisterRegex = new Regex(@"\b(ixh|ixl|iyh|iyl|pc|sp|ix|iy|af|bc|de|hl|a|b|c|d|e|f|h|l|i|r)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex assemblyNumberRegex = new Regex(@"\$?\b[a-fA-F\d]+('H'|'h')?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex assemblyAttributeRegex = new Regex(@"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline | RegexOptions.Compiled);
        
        private static readonly Brush keywordsBrushL = new SolidBrush(ColorTranslator.FromHtml("#0098dd"));
        private readonly Style keywordsStyleL = new TextStyle(keywordsBrushL, null, FontStyle.Regular);
        private static readonly Brush opcodesBrushL = new SolidBrush(ColorTranslator.FromHtml("#0098dd"));
        private readonly Style opcodesStyleL = new TextStyle(opcodesBrushL, null, FontStyle.Regular);
        private static readonly Brush registersBrushL = new SolidBrush(ColorTranslator.FromHtml("#23974a"));
        private readonly Style registersStyleL = new TextStyle(registersBrushL, null, FontStyle.Regular);
        private static readonly Brush functionsBrushL = new SolidBrush(ColorTranslator.FromHtml("#23974a"));
        private readonly Style functionsStyleL = new TextStyle(functionsBrushL, null, FontStyle.Regular);
        private static readonly Brush stringBrushL = new SolidBrush(ColorTranslator.FromHtml("#c5a332"));
        private readonly Style stringsStyleL = new TextStyle(stringBrushL, null, FontStyle.Italic | FontStyle.Bold);
        private static readonly Brush commandsBrushL = new SolidBrush(ColorTranslator.FromHtml("#275fe4"));
        private readonly Style commandsStyleL = new TextStyle(commandsBrushL, null, FontStyle.Regular);
        private static readonly Brush directivesBrushL = new SolidBrush(ColorTranslator.FromHtml("#d52753"));
        private readonly Style directivesStyleL = new TextStyle(directivesBrushL, null, FontStyle.Regular);
        private static readonly Brush variablesBrushL = new SolidBrush(ColorTranslator.FromHtml("#823ff1"));
        private readonly Style variablesStyleL = new TextStyle(variablesBrushL, null, FontStyle.Regular);
        private static readonly Brush commentBrushL = new SolidBrush(ColorTranslator.FromHtml("#a0a1a7"));
        private readonly Style commentsStyleL = new TextStyle(commentBrushL, null, FontStyle.Italic);
        private static readonly Brush lineNumsBrushL = new SolidBrush(ColorTranslator.FromHtml("#0000ff"));
        private readonly Style lineNumsStyleL = new TextStyle(lineNumsBrushL, null, FontStyle.Regular);
        private static readonly Brush numberBrushL = new SolidBrush(ColorTranslator.FromHtml("#ce33c0"));
        private readonly Style numbersStyleL = new TextStyle(numberBrushL, null, FontStyle.Regular);
        private readonly Style invisibleCharsStyleL = new SyntaxEditor.invisibleCharsRenderer(Pens.BurlyWood);
        private static readonly Brush operatorsBrushL = new SolidBrush(ColorTranslator.FromHtml("#7a82da"));
        private readonly Style operatorsStyleL = new TextStyle(operatorsBrushL, null, FontStyle.Regular);
        private readonly Color currentLineColorL = ColorTranslator.FromHtml("#64d2d2");

        private static readonly Brush keywordsBrushD = new SolidBrush(ColorTranslator.FromHtml("#10b1fe"));
        private readonly Style keywordsStyleD = new TextStyle(keywordsBrushD, null, FontStyle.Regular);
        private static readonly Brush opcodesBrushD = new SolidBrush(ColorTranslator.FromHtml("#10b1fe"));
        private readonly Style opcodesStyleD = new TextStyle(opcodesBrushD, null, FontStyle.Regular);
        private static readonly Brush registersBrushD = new SolidBrush(ColorTranslator.FromHtml("#3fc56b"));
        private readonly Style registersStyleD = new TextStyle(registersBrushD, null, FontStyle.Regular);
        private static readonly Brush functionsBrushD = new SolidBrush(ColorTranslator.FromHtml("#3fc56b"));
        private readonly Style functionsStyleD = new TextStyle(functionsBrushD, null, FontStyle.Regular);
        private static readonly Brush stringBrushD = new SolidBrush(ColorTranslator.FromHtml("#f9c859"));
        private readonly Style stringsStyleD = new TextStyle(stringBrushD, null, FontStyle.Italic | FontStyle.Bold);
        private static readonly Brush commandsBrushD = new SolidBrush(ColorTranslator.FromHtml("#3691ff"));
        private readonly Style commandsStyleD = new TextStyle(commandsBrushD, null, FontStyle.Regular);
        private static readonly Brush directivesBrushD = new SolidBrush(ColorTranslator.FromHtml("#ff6480"));
        private readonly Style directivesStyleD = new TextStyle(directivesBrushD, null, FontStyle.Regular);
        private static readonly Brush variablesBrushD = new SolidBrush(ColorTranslator.FromHtml("#9f7efe"));
        private readonly Style variablesStyleD = new TextStyle(variablesBrushD, null, FontStyle.Regular);
        private static readonly Brush commentBrushD = new SolidBrush(ColorTranslator.FromHtml("#efefef"));
        private readonly Style commentsStyleD = new TextStyle(commentBrushD, null, FontStyle.Italic);
        private static readonly Brush lineNumsBrushD = new SolidBrush(ColorTranslator.FromHtml("#b200ff"));
        private readonly Style lineNumsStyleD = new TextStyle(lineNumsBrushD, null, FontStyle.Regular);
        private static readonly Brush numberBrushD = new SolidBrush(ColorTranslator.FromHtml("#ce33c0"));
        private readonly Style numbersStyleD = new TextStyle(numberBrushD, null, FontStyle.Regular);
        private readonly Style invisibleCharsStyleD = new SyntaxEditor.invisibleCharsRenderer(Pens.WhiteSmoke);
        private static readonly Brush operatorsBrushD = new SolidBrush(ColorTranslator.FromHtml("#7a82da"));
        private readonly Style operatorsStyleD = new TextStyle(operatorsBrushD, null, FontStyle.Regular);
        private readonly Color currentLineColorD = ColorTranslator.FromHtml("#ffffff");

        public SyntaxEditor()
        {
            InitializeComponent();

            synBox1.CurrentLineColor = currentLineColorL;
            toolStripButton5.ToolTipText = "";
            toolStripButton7.ToolTipText = "";
            btInvisibleChars.ToolTipText = "";
            btInvisibleChars_Click(synBox1, null);
            synBox1.Refresh();

            popupMenu = new AutocompleteMenu(synBox1);
            popupMenu.Items.ImageList = imageList1;
            popupMenu.SearchPattern = @"[\w\.:=!<>]";
            popupMenu.AllowTabKey = true;

            BuildAutocompleteMenu(true);
        }

        public SyntaxEditor(string text, string path)
        {
            InitializeComponent();
            synBox1.CurrentLineColor = currentLineColorL;
            toolStripButton5.ToolTipText = "";
            toolStripButton7.ToolTipText = "";
            btInvisibleChars.ToolTipText = "";
            btInvisibleChars_Click(synBox1, null);
            synBox1.Refresh();

            popupMenu = new AutocompleteMenu(synBox1);
            popupMenu.Items.ImageList = imageList1;
            popupMenu.SearchPattern = @"[\w\.:=!<>]";
            popupMenu.AllowTabKey = true;
            
            BuildAutocompleteMenu(true);
            synBox1.Text = text;
            _path = path;
        }

        public SyntaxEditor(bool inBas, bool inTheme)
        {
            InitializeComponent();

            bBasic = inBas;
            bLightTheme = inTheme;

            if (bBasic)
            {
                toolStripButton5.Text = "Swap to Z80 Editor";
            }
            else { 
                toolStripButton5.Text = "Swap to SE BASIC IV Editor";
            }

            if (bLightTheme)
            {
                toolStripButton7.Text = "Swap to Dark Theme";
                synBox1.CurrentLineColor = currentLineColorL;
            }
            else {
                toolStripButton7.Text = "Swap to Light Theme";
                synBox1.CurrentLineColor = currentLineColorD;
            }

            toolStripButton5.ToolTipText = "";
            toolStripButton7.ToolTipText = "";
            btInvisibleChars.ToolTipText = "";
            btInvisibleChars_Click(synBox1, null);
            synBox1.Refresh();

            popupMenu = new AutocompleteMenu(synBox1);
            popupMenu.Items.ImageList = imageList1;
            popupMenu.SearchPattern = @"[\w\.:=!<>]";
            popupMenu.AllowTabKey = true;

            BuildAutocompleteMenu(inBas);
        }

        private void SyntaxEditor_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void openFile(string sFile)
        {
            try
            {
                StreamReader StreamReader1 = new StreamReader(sFile);
                synBox1.Text = StreamReader1.ReadToEnd();
                StreamReader1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error loading from file");
            }

        }

        private void synBox1_SelectionChanged(object sender, EventArgs e)
        {
            var currentColNum = synBox1.Selection.Start.iChar + 1;
            var currentLineNum = synBox1.Selection.Start.iLine + 1;

            currentCol.Text = @"Col: " + currentColNum;
            CurrentLine.Text = @"Line: " + currentLineNum;
        }

        private void fctb_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            highlightVisibleRange();

            if (bBasic)
            {
                const string startMarker = "FOR";
                const string endMarker = "NEXT";
                var currentSelection = synBox1.Selection.Clone();
                currentSelection.Normalize();

                if (currentSelection.Start.iLine != currentSelection.End.iLine)
                {
                    synBox1[currentSelection.Start.iLine].FoldingStartMarker = startMarker;
                    synBox1[currentSelection.End.iLine].FoldingEndMarker = endMarker;
                    synBox1.Invalidate();
                }
            }
        }

        private void synBox1_KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (int)Keys.Enter && bBasic)
            {
                var currentLineNum = synBox1.Selection.Start.iLine;
                var currentLine = synBox1.GetLineText(currentLineNum - 1);

                var crntLineNum = Regex.Matches(currentLine, "[| ]*[0-9]*");
                try
                {
                    var nextLineNum = int.Parse(crntLineNum[0].ToString()) + 10;
                    synBox1.SelectedText = nextLineNum + @" ";
                }
                catch (Exception)
                {

                    MessageBox.Show(@"Unable to find line Number.", @"Warning", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
        }

        #region DragDrop

        private void synBox1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
                if (a != null)
                {
                    string s = a.GetValue(0).ToString();
                    this.Activate();
                    openFile(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in DragDrop function: " + ex.Message);
            }
        }

        private void synBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        #endregion Drag


        #region scrollbar
        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            synBox1.OnScroll(e, e.Type != ScrollEventType.ThumbTrack && e.Type != ScrollEventType.ThumbPosition);
        }

        private void hMyScrollBar_MouseLeave(object sender, EventArgs e)
        {
            hMyScrollBar.ThumbColor = Color.LightGray;
        }

        private void hMyScrollBar_MouseMove(object sender, MouseEventArgs e)
        {
            hMyScrollBar.ThumbColor = Color.Gray;
        }

        private void pictureBox6_MouseClick(object sender, MouseEventArgs e)
        {
            if (hMyScrollBar.Visible)
            {
                hMyScrollBar.Value -= 20;
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (hMyScrollBar.Visible)
            {
                hMyScrollBar.Value += 20;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (vMyScrollBar.Visible)
            {
                vMyScrollBar.Value -= 20;
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (vMyScrollBar.Visible)
            {
                vMyScrollBar.Value += 20;
            }
        }

        #endregion scrollbar


        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                if (bBasic)
                {
                    sfd.Filter = "SE BASIC File (*.PRG)|*.PRG";
                } else
                {
                    sfd.Filter = "Z80 Assembler File (*.ASM)|*.ASM";
                }

                sfd.FilterIndex = 2;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, synBox1.Text);
                    _path = sfd.FileName;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_path == "")
            {
                saveAsToolStripMenuItem_Click(saveAsToolStripMenuItem, e);
            }
            else
            {
                if (!File.Exists(_path))
                {
                    using (StreamWriter sw = File.CreateText(_path))
                    {
                        sw.Write(synBox1.Text);
                    }
                }
                else
                {
                    File.WriteAllText(_path, synBox1.Text);
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string sTmp;

            DialogResult result;

            if (bBasic)
            {
                result = MessageBox.Show(@"Do you want to save changes?", @"SE BASIC Syntax Highlighter",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }
            else
            {
                result = MessageBox.Show(@"Do you want to save changes?", @"Z80 Syntax Highlighter",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }

            if (result == DialogResult.Yes)
            {
                saveToolStripMenuItem_Click(New, e);
            }

            if (bBasic)
            {
                sTmp = "10 REM Start writing your SE BASIC IV code here:\n20 ";
            }
            else
            {
                sTmp = "; Start writing your Z80 assembly language here:\n\nstart: ";
            }

            synBox1.Text = "";
            synBox1.SelectedText = sTmp;
        }

        public void toolStripButton2_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "c:\\";

            if (bBasic)
            {
                openFileDialog1.Filter = @"SE BASIC File (*.PRG)|*.PRG|All files (*.*)|*.*";
            } else
            {
                openFileDialog1.Filter = @"Z80 Assembler File (*.ASM)|*.ASM|All files (*.*)|*.*";
            }

            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                synBox1.OpenBindingFile(openFileDialog1.FileName, Encoding.UTF8);
                synBox1.IsChanged = false;
                synBox1.ClearUndo();
                GC.Collect();
                GC.GetTotalMemory(true);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(Save, e);
        }

        private void Print_Click(object sender, EventArgs e)
        {
            var settings = new PrintDialogSettings
            {
                Header = "&b&w&b",
                Footer = "&b&p"
            };

            synBox1.Print(settings);
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            var text = synBox1.SelectedText;
            Clipboard.SetText(text);
            synBox1.SelectedText = "";
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            var text = synBox1.SelectedText;
            Clipboard.SetText(text);
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            synBox1.SelectedText = Clipboard.GetText();
        }

        private void btInvisibleChars_Click(object sender, EventArgs e)
        {
            highlightInvisibleChars(synBox1.Range);
        }

        private void highlightInvisibleChars(Range range)
        {
            if (bLightTheme)
            {
                range.ClearStyle(invisibleCharsStyleL);

                if (btInvisibleChars.Checked)
                    range.SetStyle(invisibleCharsStyleL, @".$|.\r\n|\s");
            }
            else
            {
                range.ClearStyle(invisibleCharsStyleD);

                if (btInvisibleChars.Checked)
                    range.SetStyle(invisibleCharsStyleD, @".$|.\r\n|\s");
            }
        }

        private void btHighlightCurrentLine_Click(object sender, EventArgs e)
        {
            if (btHighlightCurrentLine.Checked)
            {
                if (bLightTheme)
                {
                    synBox1.CurrentLineColor = currentLineColorL;
                } else
                {
                    synBox1.CurrentLineColor = currentLineColorD;
                }
            }
            else
            {
                synBox1.CurrentLineColor = Color.Transparent;
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton1_Click(synBox1, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton2_Click(synBox1, e);
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Print_Click(synBox1, e);
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synBox1.ShowFindDialog();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synBox1.ShowReplaceDialog();
        }

        private void commentCurrentLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bBasic)
            {
                synBox1.CommentPrefix = "99999 REM ";
            } else
            {
                synBox1.CommentPrefix = ";";
            }

            synBox1.InsertLinePrefix(synBox1.CommentPrefix);
        }

        private void uncommentCurrentLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bBasic)
            {
                synBox1.CommentPrefix = "99999 REM ";
            }
            else
            {
                synBox1.CommentPrefix = ";";
            }

            synBox1.RemoveLinePrefix(synBox1.CommentPrefix);
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            commentCurrentLineToolStripMenuItem_Click(sender, e);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synBox1.Undo();
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            synBox1.Undo();
        }

        private void Redo_Click(object sender, EventArgs e)
        {
            synBox1.Redo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synBox1.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var text = synBox1.SelectedText;
            Clipboard.SetText(text);
            synBox1.SelectedText = "";
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy_Click(synBox1, e);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste_Click(synBox1, e);
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synBox1.SelectAll();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            findToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            replaceToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            uncommentCurrentLineToolStripMenuItem_Click(sender, e);
        }

        private void shortKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new HotkeysEditorForm(synBox1.HotkeysMapping);

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                synBox1.HotkeysMapping = form.GetHotkeys();
        }

        private void startStopMacroRecordingCtrlMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synBox1.MacrosManager.IsRecording = !synBox1.MacrosManager.IsRecording;
        }

        private void executeMacroCtrlEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synBox1.MacrosManager.ExecuteMacros();
        }

        private void setSelectedAsReadOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synBox1.Selection.ReadOnly = true;
        }

        private void setSelectedAsWritableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synBox1.Selection.ReadOnly = false;
        }

        #region Highlight

        public void highlightVisibleRange()
        {
            synBox1.Range.ClearStyle(keywordsStyleL, functionsStyleL, commandsStyleL, variablesStyleL, stringsStyleL,
                lineNumsStyleL, numbersStyleL, commentsStyleL, operatorsStyleL, registersStyleL, opcodesStyleL,
                directivesStyleL, keywordsStyleD, functionsStyleD, commandsStyleD, variablesStyleD, stringsStyleD,
                lineNumsStyleD, numbersStyleD, commentsStyleD, operatorsStyleD, registersStyleD, opcodesStyleD,
                directivesStyleD);

            if (bLightTheme)
            {
                synBox1.BackColor = ColorTranslator.FromHtml("#F9F9F9");
                synBox1.ForeColor = ColorTranslator.FromHtml("#383a42");
            }
            else
            {
                synBox1.BackColor = ColorTranslator.FromHtml("#282c34");
                synBox1.ForeColor = ColorTranslator.FromHtml("#abb2bf");
            }

            if (bBasic)
            {
                if (bLightTheme)
                {
                    synBox1.Range.SetStyle(lineNumsStyleL, basicLineNumRegex);
                    synBox1.Range.SetStyle(stringsStyleL, basicStringRegex);
                    synBox1.Range.SetStyle(commentsStyleL, basicCommentRegex);
                    synBox1.Range.SetStyle(operatorsStyleL, basicOperatorRegex);
                    synBox1.Range.SetStyle(numbersStyleL, basicNumbersRegex);
                    synBox1.Range.SetStyle(keywordsStyleL, basicKeywordsRegex);
                    synBox1.Range.SetStyle(keywordsStyleL, basicDefFnRegex);
                    synBox1.Range.SetStyle(functionsStyleL, basicFunctionsRegex);
                    synBox1.Range.SetStyle(commandsStyleL, basicCommandsRegex);
                    synBox1.Range.SetStyle(variablesStyleL, basicVariablesRegex);
                } else
                {
                    synBox1.Range.SetStyle(lineNumsStyleD, basicLineNumRegex);
                    synBox1.Range.SetStyle(stringsStyleD, basicStringRegex);
                    synBox1.Range.SetStyle(commentsStyleD, basicCommentRegex);
                    synBox1.Range.SetStyle(operatorsStyleD, basicOperatorRegex);
                    synBox1.Range.SetStyle(numbersStyleD, basicNumbersRegex);
                    synBox1.Range.SetStyle(keywordsStyleD, basicKeywordsRegex);
                    synBox1.Range.SetStyle(keywordsStyleD, basicDefFnRegex);
                    synBox1.Range.SetStyle(functionsStyleD, basicFunctionsRegex);
                    synBox1.Range.SetStyle(commandsStyleD, basicCommandsRegex);
                    synBox1.Range.SetStyle(variablesStyleD, basicVariablesRegex);
                }
            }
            else
            {
                if (bLightTheme)
                {
                    synBox1.Range.SetStyle(commentsStyleL, assemblyCommentRegex);
                    synBox1.Range.SetStyle(stringsStyleL, assemblyStringRegex);
                    synBox1.Range.SetStyle(variablesStyleL, assemblyAttributeRegex);
                    synBox1.Range.SetStyle(directivesStyleL, assemblyKeywordsRegex);
                    synBox1.Range.SetStyle(opcodesStyleL, assemblyInstructionsRegex);
                    synBox1.Range.SetStyle(registersStyleL, assemblyRegisterRegex);
                    synBox1.Range.SetStyle(numbersStyleL, assemblyNumberRegex);
                } else
                {
                    synBox1.Range.SetStyle(commentsStyleD, assemblyCommentRegex);
                    synBox1.Range.SetStyle(stringsStyleD, assemblyStringRegex);
                    synBox1.Range.SetStyle(variablesStyleD, assemblyAttributeRegex);
                    synBox1.Range.SetStyle(directivesStyleD, assemblyKeywordsRegex);
                    synBox1.Range.SetStyle(opcodesStyleD, assemblyInstructionsRegex);
                    synBox1.Range.SetStyle(registersStyleD, assemblyRegisterRegex);
                    synBox1.Range.SetStyle(numbersStyleD, assemblyNumberRegex);
                }
            }
        }
        #endregion Highlight

        #region InvisibleCharsRenderer
        public class invisibleCharsRenderer : Style
        {
            readonly Pen pen;

            public invisibleCharsRenderer(Pen pen)
            {
                this.pen = pen;
            }

            public override void Draw(Graphics gr, Point position, Range range)
            {
                var tb = range.tb;
                using (Brush brush = new SolidBrush(pen.Color))
                    foreach (var place in range)
                    {
                        switch (tb[place].c)
                        {
                            case ' ':
                                var point = tb.PlaceToPoint(place);
                                point.Offset(tb.CharWidth / 2, tb.CharHeight / 2);
                                gr.DrawLine(pen, point.X, point.Y, point.X + 1, point.Y);
                                break;
                        }

                        if (tb[place.iLine].Count - 1 == place.iChar)
                        {
                            var point = tb.PlaceToPoint(place);
                            point.Offset(tb.CharWidth, 0);
                            gr.DrawString("¶", tb.Font, brush, point);
                        }
                    }
            }
        }
        #endregion InvisibleCharsRenderer

        #region close

        private void close_MouseMove(object sender, MouseEventArgs e)
        {
            close.Image = Properties.Resources.clsh;
            status.ForeColor = Color.FromArgb(255, 255, 100, 100);
            status.Text = "Close";
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void close_MouseLeave(object sender, EventArgs e)
        {
            close.Image = Properties.Resources.CtrlBtn;
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "";
        }

        #endregion close

        #region max

        private void max_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {

                this.WindowState = FormWindowState.Maximized;
                this.Refresh();
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.Refresh();
            }
        }

        private void max_MouseMove(object sender, MouseEventArgs e)
        {
            max.Image = Properties.Resources.minmaxh;
            if (this.WindowState == FormWindowState.Normal)
            {
                status.ForeColor = Color.FromArgb(255, 136, 136, 136);
                status.Text = "Maximize";
            }
            else
            {
                status.ForeColor = Color.FromArgb(255, 136, 136, 136);
                status.Text = "Restore";
            }
        }

        private void max_MouseLeave(object sender, EventArgs e)
        {
            max.Image = Properties.Resources.CtrlBtn;
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "";
        }

        #endregion max


        #region min

        private void min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void min_MouseMove(object sender, MouseEventArgs e)
        {
            min.Image = Properties.Resources.minmaxh;
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Minimize";
        }

        private void min_MouseLeave(object sender, EventArgs e)
        {
            min.Image = Properties.Resources.CtrlBtn;
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "";
        }

        #endregion min


        #region ZoomIn

        private void ZoomIn_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Zoom In";
            ZoomIn.BackColor = ColorTranslator.FromHtml("#F9F9F9");
        }

        private void ZoomIn_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
            ZoomIn.BackColor = Color.Transparent;
        }

        private void ZoomIn_Click(object sender, EventArgs e)
        {
            synBox1.Zoom += 10;
        }

        #endregion ZoomIn

        #region ZoomOut

        private void ZoomOut_Click(object sender, EventArgs e)
        {
            synBox1.Zoom -= 10;
        }

        private void ZoomOut_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Zoom Out";
            ZoomOut.BackColor = ColorTranslator.FromHtml("#F9F9F9");
        }

        private void ZoomOut_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
            ZoomOut.BackColor = Color.Transparent;
        }

        #endregion ZoomOut

        #region new

        private void New_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Create New Document";
        }

        private void New_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }

        #endregion new


        #region Open
        private void Open_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Open New Document";
        }

        private void Open_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }
        #endregion Open


        #region Save
        private void Save_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Save Current Document";
        }

        private void Save_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }
        #endregion Save


        #region Print
        private void Print_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }

        private void Print_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Print Current Document";
        }
        #endregion print


        #region cut
        private void Cut_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Cut Selected Text";
        }

        private void Cut_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }
        #endregion cut


        #region copy
        private void Copy_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Copy Selected Text";
        }

        private void Copy_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }
        #endregion copy


        #region Paste
        private void Paste_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }

        private void Paste_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Paste Copied Text";
        }
        #endregion Paste


        #region invisible char
        private void btInvisibleChars_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }

        private void btInvisibleChars_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Show Invisible Characters";
            btInvisibleChars.ToolTipText = "Show Invisible Characters";
        }
        #endregion invisible char


        #region HighlightCurrentLine
        private void btHighlightCurrentLine_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Highlight Current Line";
        }

        private void btHighlightCurrentLine_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }
        #endregion HighlightCurrentLine


        #region undo
        private void Undo_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Undo Recent Change";
        }

        private void Undo_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }
        #endregion undo


        #region redo
        private void Redo_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }

        private void Redo_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Redo Recent Undo";
        }
        #endregion redo


        #region comment
        private void toolStripButton1_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Comment Out Current Line";
        }

        private void toolStripButton1_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }
        #endregion comment


        #region uncomment
        private void toolStripButton2_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Uncomment Current Line";
        }

        private void toolStripButton2_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }
        #endregion uncomment


        #region find

        private void toolStripButton3_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Open Find Dialog";
        }

        private void toolStripButton3_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }
        #endregion find


        #region replace
        private void toolStripButton4_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Open Replace Dialog";
        }

        private void toolStripButton4_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }
        #endregion replace

        #region Execute
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            DialogResult result;

            if (bBasic)
            {
                result = MessageBox.Show("Are you sure? Any unsaved changes will be lost!", "SE BASIC Syntax Highlighter",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }
            else
            {
                result = MessageBox.Show("Are you sure? Any unsaved changes will be lost!", "Z80 Syntax Highlighter",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }

            if (result == DialogResult.Yes)
            {
                if (bBasic)
                {
                    this.Hide();

                    SyntaxEditor syntaxEditor = new SyntaxEditor(!bBasic, bLightTheme);
                    syntaxEditor.synBox1.ShowLineNumbers = true;
                    syntaxEditor.synBox1.Text = "; Start writing your Z80 assembly language here:\n\nstart: ";
                    syntaxEditor.Show();
                } else
                {
                    this.Hide();

                    SyntaxEditor syntaxEditor = new SyntaxEditor(!bBasic, bLightTheme);
                    syntaxEditor.synBox1.ShowLineNumbers = false;
                    syntaxEditor.synBox1.Text = "10 REM Start writing your SE BASIC IV code here:\n20 ";
                    syntaxEditor.Show();
                }
            }
        }

        private void toolStripButton5_MouseMove(object sender, MouseEventArgs e)
        {
            if (bBasic)
            {
                status.Text = "Swap to Z80 Editor";
                toolStripButton5.ToolTipText = "Swap to Z80 Editor";
            } else
            {
                status.Text = "Swap to SE BASIC IV Editor";
                toolStripButton5.ToolTipText = "Swap to SE BASIC IV Editor";
            }
        }

        private void toolStripButton5_MouseLeave(object sender, EventArgs e)
        {
            status.Text = "";
        }
        #endregion 


        private void aboutSyntaxEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showAbout();
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            showAbout();
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            Form1 fm1 = new Form1();
            fm1.Show();
            this.Hide();
        }

        private void pictureBox10_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox10.Image = Properties.Resources.info_h;
            status.Text = "Info";
        }

        private void pictureBox10_MouseLeave(object sender, EventArgs e)
        {
            pictureBox10.Image = Properties.Resources.info;
            status.Text = "";
        }

        private void pictureBox9_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox9.Image = Properties.Resources.Home_h;
            status.Text = "Goto Home Page";
        }

        private void pictureBox9_MouseLeave(object sender, EventArgs e)
        {
            pictureBox9.Image = Properties.Resources.Home;
            status.Text = "";
        }

        private void SyntaxEditor_Activated(object sender, EventArgs e)
        {
            if (InfoShown)
            {
                about.Activate();
            }
        }

        private void showAbout()
        {
            var pstate = this.WindowState;
            this.Size = new Size(867, 551);
            var xposition = (Screen.PrimaryScreen.WorkingArea.Width - Size.Width) / 2;
            var yposition = (Screen.PrimaryScreen.WorkingArea.Height - Size.Height) / 2;
            this.Location = new Point(xposition, yposition);
            this.Refresh();

            about.Size = this.Size;
            about.StartPosition = FormStartPosition.CenterScreen;
            about.WindowState = pstate;

            about.Show();
            InfoShown = true;
        }

        private void synBox1_DoubleClick(object sender, EventArgs e)
        {
            if (_isFullScreen)
            {
                var height = this.Size.Height - (6 + 17);
                var widht = this.Size.Width - (6 + 17);

                synBox1.Size = new Size(widht, height);
                synBox1.Location = new Point(3, 3);
                synBox1.BringToFront();

                pictureBox1.BringToFront();
                pictureBox1.Location = new Point(3, Size.Height - 20);

                pictureBox2.BringToFront();
                pictureBox2.Height = Size.Height - (6 + 17);
                pictureBox2.Location = new Point(Size.Width - 20, 3);

                pictureBox3.BringToFront();
                pictureBox3.Location = new Point(Size.Width - 18, 3);

                pictureBox4.BringToFront();
                pictureBox4.Location = new Point(Size.Width - 18, Size.Height - (16 + 17));

                pictureBox5.BringToFront();
                pictureBox5.Location = new Point(pictureBox5.Location.X, Size.Height - 18);

                pictureBox6.BringToFront();
                pictureBox6.Location = new Point(pictureBox6.Location.X, Size.Height - 18);

                vMyScrollBar.BringToFront();
                vMyScrollBar.Height = Size.Height - (6 + 17 + 13 * 2);
                vMyScrollBar.Location = new Point(Size.Width - 18, 16);

                hMyScrollBar.BringToFront();
                hMyScrollBar.Location = new Point(hMyScrollBar.Location.X, Size.Height - 17);

                currentCol.BringToFront();
                currentCol.Location = new Point(currentCol.Location.X, Size.Height - 18);

                CurrentLine.BringToFront();
                CurrentLine.Location = new Point(CurrentLine.Location.X, Size.Height - 18);

                ZoomIn.BringToFront();
                ZoomIn.Location = new Point(ZoomIn.Location.X, Size.Height - 18);

                ZoomOut.BringToFront();
                ZoomOut.Location = new Point(ZoomOut.Location.X, Size.Height - 18);

                _isFullScreen = false;
            }
            else
            {
                synBox1.Location = new Point(4, 89);
                synBox1.Size = new Size(Size.Width - 27, Size.Height - 153);
                synBox1.SendToBack();

                pictureBox1.Location = new Point(3, Size.Height - 63);

                pictureBox2.Height = Size.Height - (63 + 89);
                pictureBox2.Location = new Point(Size.Width - 20, 89);

                pictureBox3.Location = new Point(Size.Width - 18, 89);
                pictureBox4.Location = new Point(Size.Width - 18, Size.Height - (17 + 63));

                pictureBox5.Location = new Point(pictureBox5.Location.X, Size.Height - 61);
                pictureBox6.Location = new Point(pictureBox6.Location.X, Size.Height - 61);

                vMyScrollBar.Height = Size.Height - (6 + 17 + 13 * 2);
                vMyScrollBar.Location = new Point(Size.Width - 18, 16);
                hMyScrollBar.Location = new Point(hMyScrollBar.Location.X, Size.Height - 60);

                currentCol.Location = new Point(currentCol.Location.X, Size.Height - 61);
                CurrentLine.Location = new Point(CurrentLine.Location.X, Size.Height - 61);

                ZoomIn.Location = new Point(ZoomIn.Location.X, Size.Height - 61);
                ZoomOut.Location = new Point(ZoomOut.Location.X, Size.Height - 61);

                vScrollBar.SendToBack();
                hScrollBar.SendToBack();
                _isFullScreen = true;
            }
        }

        private void SyntaxEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && Control.ModifierKeys == Keys.Control)
                Save_Click(synBox1, null);

            else if (e.KeyCode == Keys.O && Control.ModifierKeys == Keys.Control)
                openToolStripMenuItem_Click(synBox1, null);

            else if (e.KeyCode == Keys.N && Control.ModifierKeys == Keys.Control)
                newToolStripMenuItem_Click(synBox1, null);

            else if (e.KeyCode == Keys.P && Control.ModifierKeys == Keys.Control)
                printToolStripMenuItem_Click(synBox1, null);

            else if (e.KeyCode == Keys.R && Control.ModifierKeys == Keys.Control)
                toolStripButton5_Click(synBox1, null);

            else if (e.KeyCode == Keys.F5)
                toolStripButton5_Click(synBox1, null);
        }

        private void synBox1_ToolTipNeeded(object sender, ToolTipNeededEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.HoveredWord))
            {
                e.ToolTipTitle = e.HoveredWord.ToUpper();
                e.ToolTipText = getToolTip(e.HoveredWord);
            }
        }

        private string getToolTip(String inS)
        {
            if (bBasic)
            {
                switch (inS)
                {
                    case "BEEP":
                        return
                            "TODO: To sound the speaker at 800 Hz(800 cycles per second) for one - quarter of a second.\n\nSyntax:\nBEEP";
                    case "BSAVE":
                        return
                            "Syntax:\n"
                            + "\tBSAVE file_spec, offset, length\n\n"
                            + "Saves a region of memory to an image file.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expression 'file_spec' is a valid file specification indicating the file to write to.\n"
                            + "\t'offset' is a numeric expression in the range [-32768 to 65535] indicating the offset into the current segment from where to start reading.\n"
                            + "\t'length' is a numeric expression in the range [-32768 to 65535] indicating the number of bytes to read.\n"
                            + "\tIf 'offset' or 'length' are negative, their two's complement will be used.\n\n"
                            + "Errors:\n"
                            + "\t'file_spec' has a numeric value: Type mismatch.\n"
                            + "\t'file_spec' contains disallowed characters: Bad file number (on CAS1:); Bad file name (on disk devices).\n"
                            + "\t'offset' is not in the range [-32768 to 65535]: Overflow.\n"
                            + "\t'length' is not in the range [-32768 to 65535]: Overflow.";
                    case "CALL":
                        return
                            "Syntax:\n"
                            + "\tCALL address_var [, p0, p1, ...]\n\n"
                            + "Executes a machine language subroutine.\n\n"
                            + "Parameters:\n"
                            + "\t'address_var' is a numeric variable.\n"
                            + "\t'p0', 'p1', ... are variables.\n\n"
                            + "Errors:\n"
                            + "\t'address_var' is a string variable: Type mismatch.\n"
                            + "\t'address_var' is a literal: Syntax error.";
                    case "CHAIN":
                        return
                            "TODO: To transfer control to the specified program and pass (chain) variables to it from the current program.\n\nSyntax:\nCHAIN[MERGE] filename[,[line][,[ALL][,DELETE range]]]";
                    case "CIRCLE":
                        return
                            "TODO: To draw a circle, ellipse, and angles on the screen during use of the Graphics mode.\n\nSyntax:\nCIRCLE(xcenter, ycenter), radius[,[color][,[start],[end][,aspect]]]";
                    case "CLOSE":
                        return
                            "Syntax:\n"
                            + "\tCLOSE [[#] file_0 [, [#] file_1] ...]\n\n"
                            + "Closes streams. If no file numbers are specified, all open streams [3 to 15] are closed.\n"
                            + "\tThe hash ('#') is optional and has no effect.\n\n"
                            + "Parameters:\n"
                            + "\t'file_1', 'file_2', ... are numeric expressions yielding stream numbers.\n\n"
                            + "Errors:\n"
                            + "\t'file_1', 'file_2', ... are not in [0 to 15]: Bad I/O device.\n"
                            + "\t'file_1', 'file_2', ... are not open streams: Undefined stream.\n"
                            + "\t'file_1', 'file_2', ... have a string value: Type mismatch.\n"
                            + "\tThe statement ends in a comma, Syntax error.\n"
                            + "\tIf an error occurs, only the files before the erratic value are closed.";
                    case "CLS":
                        return
                            "Syntax:\n"
                            + "\tCLS [x]\n\n"
                            + "Clears the screen or part of it. If x is not specified, in SCREEN 0 the text view region is cleared;\n"
                            + "\tin other screens, the graphics view region is cleared. The comma is optional and has no effect.\n\n"
                            + "Parameters:\n"
                            + "\t'x' is a numeric valued expression that determines what is cleared:\n"
                            + "\t\tIf 'x' = 0, the whole screen is cleared.\n"
                            + "\t\tIf 'x' = 1, the graphics view region is cleared.\n"
                            + "\t\tIf 'x' = 2, the text view region is cleared.\n\n"
                            + "Errors:\n"
                            + "\t'x' is has a string value: Type mismatch.\n"
                            + "\t'x' is not in [-32768 to 32767]: Overflow.\n"
                            + "\t'x' is not in [0, 1, 2]: Illegal function call.\n"
                            + "\tIf an error occurs, the screen is not cleared.";
                    case "COLOR":
                        return
                            "Syntax:\n"
                            + "\tCOLOR [foreground] [, [background] [, border]]\n\n"
                            + "Changes the current foreground and background attributes.\n"
                            + "All new characters printed will take the newly set attributes.\n"
                            + "Existing characters on the screen are not affected.\n\n"
                            + "Parameters:\n"
                            + "\t'foreground' is a numeric expression in [0 to 15]. This specifies the new foreground attribute.\n"
                            + "\t'background' is a numeric expression in [0 to 15]. This specifies the new background attribute.\n"
                            + "\t'border' is a numeric expression in [0 to 15] specifying the border attribute.\n"
                            + "\t\tIt is taken MOD 8: Values 8 to 15 produce the same colour as 0 to 7.\n\n"
                            + "Errors:\n"
                            + "\tAny of the parameters has a string value: Type mismatch.\n"
                            + "\tAny of the parameters is not in [-32768 to 32767]: Overflow.\n"
                            + "\t'foreground' is not in [0 to 31], background is not in [0 to 15] or border is not in [0 to 15]: Illegal function call.\n"
                            + "\tStatement is used in SCREEN 2: Illegal function call.";
                    case "COM":
                        return
                            "TODO: To enable or disable trapping of communications activity to the specified communications adapter.\n\nSyntax:\nCOM(n) ON\nCOM(n) OFF\nCOM(n)STOP";
                    case "COPY":
                        return
                            "Syntax:\n"
                            + "\tCOPY file_spec_1 TO file_spec_2\n\n"
                            + "Copies the disk file 'file_spec_1' to 'file_spec_2'.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expressions 'file_spec_1' and 'file_spec_2' are valid file specifications indicating the source and destination files.\n"
                            + "\t\tThe first must point to an existing file on a disk device.\n\n"
                            + "Notes:\n"
                            + "\tTypically, this command is not present in Microsoft BASIC.\n\n"
                            + "Errors:\n"
                            + "\t'file_spec_1' or 'file_spec_2' have number values: Type mismatch\n"
                            + "\t'file_spec_1' does not exist: File not found.";
                    case "DATA":
                        return
                            "Syntax:\n"
                            + "\tDATA [const_0] [, [const_1]] ...\n\n"
                            + "Specifies data that can be read by a READ statement.\n\n"
                            + "Parameters:\n"
                            + "\t'const_0', 'const_1', ... are string and number literals or may be empty.\n"
                            + "\t\tString literals can be given with or without quotation marks.\n"
                            + "\t\tIf quotation marks are omitted, leading and trailing whitespace is ignored and commas or colons will terminate the data statement.\n\n"
                            + "Errors:\n"
                            + "\tIf the type of the literal does not match that of the corresponding READ statement, a Syntax error occurs on the DATA statement.";
                    case "DATE$":
                    case "DATE":
                        return
                            "TODO: To set or retrieve the current date.\n\nSyntax:\nDATE$ = variable$\nvariable = DATE$";
                    case "DIM":
                        return
                            "Syntax:\n"
                            + "\tDIM name {(|[} limit_0 [, limit_1] ... {)|]}\n\n"
                            + "Allocates memory for arrays. The DIM statement also fixes the number of indices of the array.\n"
                            + "An array can only be allocated once; to re-allocate an array, ERASE or CLEAR must be executed first.\n"
                            + "If an array is first used without a DIM statement, it is automatically allocated with its maximum indices set at 10 for each index position used.\n"
                            + "If an array's DIM statement specifies no indices, it is allocated with a single index with maximum 10.\n"
                            + "The least index allowed is determined by OPTION BASE.\n\n"
                            + "Parameters:\n"
                            + "\t'name' is a legal variable name specifying the array to be allocated.\n"
                            + "\t'limit_0', 'limit_1', ... are numeric expressions that specify the greatest index allowed at that position.\n\n"
                            + "Notes:\n"
                            + "\tMixed brackets are allowed.\n"
                            + "\tThe size of arrays is limited by the available BASIC memory.\n"
                            + "\tThe maximum number of indices is, theoretically, 255.\n"
                            + "\t\tIn practice, it is limited by the 255-byte limit on the length of program lines.\n\n"
                            + "Errors:\n"
                            + "\t'name' has already been dimensioned: Duplicate definition.\n"
                            + "\tAn index is empty: Syntax error.\n"
                            + "\tAn index is missing at the end: Missing operand.\n"
                            + "\t'limit_0', 'limit_1', ... have a string value: Type mismatch.\n"
                            + "\t'limit_0', 'limit_1', ... are not within [-32768 to 32767]: Overflow.\n"
                            + "\t'limit_0', 'limit_1', ... are negative: Illegal function call.\n"
                            + "\tThe array exceeds the size of available variable space: Out of memory.";
                    case "DOKE":
                        return
                            "Syntax:\n"
                            + "\tDOKE address, value\n\n"
                            + "Sets the 16-bit value of the memory byte pair at 'segment' * 16 + 'address' to 'value',\n"
                            + "\twhere 'segment' is the current segment set with DEF SEG.\n\n"
                            + "Parameters:\n"
                            + "\t'address' is a numeric expression in [0 to 65535]. Negative values are interpreted as their two's complement.\n"
                            + "\t'value' is a numeric expression in [0 to 65535].\n\n"
                            + "Notes:\n"
                            + "\tDEF SEG is not yet implemented in SE Basic IV.\n\n"
                            + "Errors:\n"
                            + "\t'address' or 'value' has a string value: Type mismatch.\n"
                            + "\t'address' is not in [-32768 to 65535]: Overflow.\n"
                            + "\t'value' is not in [-32768 to 32767]: Overflow.\n"
                            + "\t'value' is not in [0 to 65535]: Illegal function call.";
                    case "DRAW":
                        return
                            "TODO: To draw a figure.\n\nSyntax:\nDRAW string expression\n\nString commands are\nUp\tup\nDn\tdown\nLn\tleft\nRn\tright\nEn\tdiagonally up and right\nFn\tdiagonally down and right\nGn\tdiagonally down and left\nHn\tdiagonally up and left";
                    case "EDIT":
                        return
                            "Syntax:\n"
                            + "\tEDIT {line_number|.}\n\n"
                            + "Displays the specified program line with the cursor positioned for editing.\n"
                            + "'line_number' must be a line that exists in the program, or a period ('.') to indicate the last line stored.\n\n"
                            + "Errors:\n"
                            + "\tNo 'line_number' is specified: Undefined line number.\n"
                            + "\tMore characters are written after the line number: Illegal function call.\n"
                            + "\t'line_number' is not in [0 to 65529]: Illegal function call.\n"
                            + "\tThe specified 'line_number' does not exist: Undefined line number.";
                    case "END":
                        return
                            "Syntax:\n"
                            + "\tEND\n\n"
                            + "Closes all files, stops program execution and returns control to the user.\n"
                            + "No message is printed. It is possible to resume execution at the next statement using CONT.";
                    case "ERASE":
                        return
                            "TODO: To eliminate arrays from a program.\n\nSyntax:\nERASE list of array variables";
                    case "ERROR":
                        return
                            "Syntax:\n"
                            + "\tERROR error_number\n"
                            + "Raises the error with number 'error_number'.\n\n"
                            + "Parameters:\n"
                            + "\t'error_number' is an expression with a numeric value.\n\n"
                            + "Errors:\n"
                            + "\t'error_number' has a string value: Type mismatch.\n"
                            + "\t'error_number' is not in [-32768 to 32767]: Overflow.\n"
                            + "\t'error_number' is not in 1 to 255]: Illegal function call.";
                    case "FIELD":
                        return
                            "TODO: To allocate space for variables in a random file buffer.\n\nSyntax:\nFIELD [#] filenum, width AS stringvar [,width AS stringvar]...";
                    case "FOR":
                    case "TO":
                    case "STEP":
                        return
                            "Syntax:\n"
                            + "\tFOR loop_var = start TO stop [STEP step]\n\n"
                            + "Initiates a FOR to NEXT loop.\n"
                            + "Initially, 'loop_var' is set to 'start'.\n"
                            + "Then, the statements between the FOR statement and the NEXT statement are executed and\n"
                            + "'loop_var' is incremented by 'step' (if 'step' is not specified, by 1).\n"
                            + "This is repeated until 'loop_var' has become greater than 'stop'.\n"
                            + "Execution then continues at the statement following NEXT.\n"
                            + "The value of 'loop_var' equals 'stop' + 'step' after the loop.\n\n"
                            + "Parameters:\n"
                            + "\t'loop_var' is a numeric variable.\n"
                            + "\t'start', 'stop' and 'step' are numeric expressions.\n\n"
                            + "Errors:\n"
                            + "\tNo NEXT statement is found to match the FOR statement: FOR without NEXT occurs at the FOR statement.\n"
                            + "\t'loop_var' is a string variable or 'start', 'stop', or 'end' has a string value: Type mismatch.\n"
                            + "\t'loop_var' is an array element: Syntax error.\n"
                            + "\t'loop_var' is an integer variable and 'start', 'stop' or 'step' is outside the range [-32768, 32767]: Overflow.";
                    case "GET":
                        return
                            "TODO: To transfer graphics images from the screen.\nOR\nTo read a record from a random disk file into a random buffer.\n\nSyntax:\nGET (x1,y1)-(x2,y2),array name\nOR\nGET [#]file number[,record number]";
                    case "GOSUB":
                    case "GO":
                    case "SUB":
                        return
                            "Syntax:\n"
                            + "\tGO[ ]SUB line_number [anything]\n\n"
                            + "Jumps to a subroutine at line_number. The next RETURN statement jumps back to the statement after GOSUB.\n"
                            + "Anything after line_number until the end of the statement is ignored.\n"
                            + "If executed from a direct line, GOSUB runs the subroutine and the following RETURN returns execution to the direct line.\n\n"
                            + "Parameters:\n"
                            + "\t'line_number' is an existing line number literal.\n"
                            + "Notes:\n"
                            + "\tFurther characters on the line are ignored until end of statement.\n"
                            + "\tIf no RETURN is encountered, no problem.\n"
                            + "\tOne optional space is allowed between GO and SUB; it will not be retained in the program.\n\n"
                            + "Errors:\n"
                            + "\tIf 'line_number' does not exist: Undefined line number.\n"
                            + "\tIf 'line_number' is greater than 65529, only the first 4 characters are read (for example, 6553)";
                    case "GOTO":
                        return
                            "Syntax:\n"
                            + "\tGOTO line_number [anything]\n\n"
                            + "Jumps to 'line_number'. Anything after line_number until the end of the statement is ignored.\n"
                            + "If executed from a direct line, GOTO starts execution of the program at the specified line.\n\n"
                            + "Parameters:\n"
                            + "\t'line_number' is an existing line number literal.\n\n"
                            + "Notes:\n"
                            + "\tFurther characters on the line are ignored until end of statement.\n"
                            + "\tNo spaces are allowed between GO and TO.\n\n"
                            + "Errors:\n"
                            + "\t'line_number' does not exist: Undefined line number.";
                    case "IF":
                    case "THEN":
                    case "ELSE":
                        return
                            "Syntax:\n"
                            + "\tIF truth_value {THEN|GOTO} [compound_statement_true|line_number_true [anything]] [ELSE [compound_statement_false|line_number_false [anything]]]\n\n"
                            + "If 'truth_value' is non-zero, executes 'compound_statement_true' or jumps to 'line_number_true'.\n"
                            + "If it is zero, executes 'compound_statement_false' or jumps to 'line_number_false'.\n\n"
                            + "Parameters:\n"
                            + "\t'truth_value' is a numeric expression.\n"
                            + "\t'line_number_false' and 'line_number_true' are existing line numbers.\n"
                            + "\t'compound_statement_false' and 'compound_statement_true' are compound statements,\n"
                            + "\t\tconsisting of at least one statement, optionally followed by further statements separated by colons ':'.\n"
                            + "\t\tThe compound statements may contain nested IF to THEN to ELSE statements.\n\n"
                            + "Notes:\n"
                            + "\tThe comma is optional and ignored.\n"
                            + "\tELSE clauses are optional; they are bound to the innermost free IF statement if nested.\n"
                            + "\t\tAdditional ELSE clauses that have no matching IF are ignored.\n"
                            + "\tAll clauses must be on the same program line.\n"
                            + "\tTHEN and GOTO are interchangeable; which one is chosen is independent of whether\n"
                            + "\t\ta statement or a line number is given. GOTO PRINT 1 is fine.\n"
                            + "\tAs in GOTO, anything after the line number is ignored.\n\n"
                            + "Errors:\n"
                            + "\tIf 'truth_value' has a string value: Type mismatch.\n"
                            + "\t'truth_value' equals 0 and 'line_number_false' is a non-existing line number,\n"
                            + "\t\tor 'truth_value' is nonzero and 'line_number_true' is a non-existing line number: Undefined line number.";
                    case "KEY":
                        return
                            "Syntax:\n"
                            + "\t1) KEY key_id, string_value\n"
                            + "\t2) KEY LIST\n"
                            + "\t3) KEY {ON|OFF}\n\n"
                            + "1) Defines the string macro for function key 'key_id'. Only the first 15 characters of 'string_value' are stored.\n\n"
                            + "Parameters:\n"
                            + "\t'key_id' is a numeric expression in the range [1 to 15].\n"
                            + "\t'string_value' is a string expression.\n\n"
                            + "Notes:\n"
                            + "\tIf 'key_id' is not in the prescribed range, an error is raised.\n"
                            + "\tIf 'string_value' is the empty string or the first character of 'string_value' is CHR$(0),\n"
                            + "\t\tthe function key macro is switched off and subsequent catching of the associated function key with INKEY$ is enabled.\n\n"
                            + "Errors:\n"
                            + "\t'key_id' is not in [-32768 to 32767]: Overflow.\n"
                            + "\t'key_id' is not in [1 to 255]: Illegal function call.\n"
                            + "\t'key_id' has a string value: Type mismatch.\n\n"
                            + "2) Prints a list of the 15 function keys with the function-key macros defined for those keys to the console.\n\n"
                            + "Most characters are represented by their symbol equivalent in the current codepage.\n"
                            + "\tHowever, some characters get a different represenation, which is a symbolic representation of the effect as control characters on the screen.\n\n"
                            + "3) Toggles function-key macros ON or OFF.";
                    case "LET":
                        return
                            "Syntax:\n"
                            + "\t[LET] name = expression\n\n"
                            + "Assigns the value of 'expression' to the variable or array element 'name'.\n\n"
                            + "Parameters:\n"
                            + "\t'name' is a variable that may or may not already exist.\n"
                            + "\tThe type of expression matches that of 'name':\n"
                            + "\t\tthat is, all numeric types can be assigned to each other but strings can only be assigned to strings.\n\n"
                            + "Errors:\n"
                            + "\t'name' and 'expression' are not of matching types: Type mismatch.";
                    case "LINE":
                        return
                            "TODO: To draw lines and boxes on the screen.\n\nSyntax:\nLINE [(x1,y1)]-(x2,y2) [,[attribute][,B[F]][,style]]";
                    case "LOCATE":
                        return
                            "TODO: To move the cursor to the specified position on the active screen.\n\nSyntax:\nLOCATE [row][,[col][,[cursor][,[start] [,stop]]]]";
                    case "RSET":
                        return
                            "TODO: To move data from memory to a random-file buffer and right-justify it in preparation for a PUT statement.\n\nSyntax:\nRSET string variable=string expression";
                    case "LSET":
                        return
                            "TODO: To move data from memory to a random-file buffer and left-justify it in preparation for a PUT statement.\n\nSyntax:\nLSET string variable=string expression";
                    case "NEXT":
                        return
                            "Syntax:\n"
                            + "\tNEXT [var_0 [, var_1] ...]\n\n"
                            + "Iterates a FOR to NEXT loop: increments the loop variable and jumps to the FOR statement.\n"
                            + "If no variables are specified, next matches the most recent FOR statement.\n"
                            + "Several nested NEXT statements can be consolidated into one by using the variable list.\n"
                            + "If one or more variables are specified, their order must match the order of earlier FOR statements.\n\n"
                            + "Parameters:\n"
                            + "\t'var_0', 'var_1', ... are numeric variables which are loop counters in a FOR statement.\n\n"
                            + "Errors:\n"
                            + "\tNo FOR statement is found to match the NEXT statement and variables: NEXT without FOR.\n"
                            + "\t'var_0', 'var_1', ... are string variables: NEXT without FOR.\n"
                            + "\tThe (implicit or explicit) loop variable is an integer variable and is taken outside the range [-32768, 32767]\n"
                            + "\t\twhen incremented after the final iteration: Overflow.";
                    case "NOISE":
                        return
                            "TODO: Syntax:\n";
                    case "ON":
                        return
                            "Syntax:\n"
                            + "\tON n {GOTO|GOSUB} line_number_0 [, line_number_1] ...\n\n"
                            + "Jumps to the nth line number specified in the list.\n"
                            + "If n is 0 or greater than the number of line numbers in the list, no jump is performed.\n"
                            + "If GOTO is specified, the jump is unconditional; if GOSUB is specified, jumps to a subroutine.\n\n"
                            + "Parameters:\n"
                            + "\t'n' is a numeric expression in [0 to 255].\n"
                            + "\t'line_number_0', 'line_number_1', ... are existing line numbers in the program.\n\n"
                            + "Errors:\n"
                            + "\t'n' has a string value: Type mismatch.\n"
                            + "\t'n' is not in [-32768 to 32767], Overflow.\n"
                            + "\t'n' is not in [0 to 255]: Illegal function call.\n"
                            + "\tThe line number jumped to does not exist: Undefined line number.";
                    case "OPEN":
                        return
                            "Syntax:\n"
                            + "\tOPEN # file_num, \"mode_char\" [,file_spec]\n\n"
                            + "Opens a data file on a device.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expression 'file_spec' is a valid file specification.\n"
                            + "\t'file_num' is a valid stream [3 to 15].\n"
                            + "\t'mode_char' is a string expression of which the first character is one of:\n"
                            + "\t\t[\"K\", \"S\"] - system channel (keyboard or screen).\n"
                            + "\t\t[\"I\", \"O\", \"A\", \"R\"] - access mode for a disk file.\n"
                            + "\t\tother alpha character - a user defined channel.\n\n"
                            + "Access modes:\n"
                            + "\tThe 'mode_char' are as follows:\n"
                            + "\t\tmode_char\tMeaning\tEffect\n"
                            + "\t\t\"I\"\tINPUT\tOpens a text file for reading and positions the file pointer at the start.\n"
                            + "\t\t\"O\"\tOUTPUT\tTruncates a text file at the start and opens it for writing.\n"
                            + "\t\t\t\tAny data previously present in the file will be deleted.\n"
                            + "\t\t\"A\"\tAPPEND\tOpens a text file for writing at the end of any existing data.\n"
                            + "\t\t\"R\"\tRANDOM\tOpens a file for random access.\n\n"
                            + "A single character can be read with c$=INKEY$ #file_num or written with PRINT #file_num;c$;.\n"
                            + "Strings are terminated with a carriage return. A string can be read with INPUT #file_num;s$ or written with PRINT #file_num;s$.\n\n"
                            + "File specification:\n"
                            + "\t'file_spec' is a non-empty string expression of the form \"parameters\".\n"
                            + "\t\t'parameters' must specify a valid file path of the form [/][dirname/] ... filename.\n"
                            + "\tIn SE Basic IV, file support is provided using the OS kernel.\n"
                            + "\t\tUnoDOS 3 follows MS-DOS file system conventions with the exception that folder names are separated with forward slashes (/).\n"
                            + "\t\tSE Basic IV adds syntactic sugar to the short filename format. File names consist of an 8-character name and 3-character extension.\n"
                            + "\t\tFolder names consist of an 11-character name. Permissible characters are the printable ASCII characters in the range [$20 to $7E]\n"
                            + "\t\texcluding the characters \" * + . , / : ; < = > ? \\ [ ] |. Spaces are allowed but are converted to underscores.\n\n"
                            + "A path starting with a forward slash is interpreted as an absolute path, starting at the root of the specified disk device.\n"
                            + "Otherwise, the path is interpreted as relative to the current folder on the specified device.\n"
                            + "The special folder name '..' refers to the parent folder of a preceding path, or the parent folder of the current folder if no path is given.\n"
                            + "The special folder name '.' refers to the same folder as given by the preceding path, or the current folder if no preceding path is given.\n"
                            + "The LOAD and SAVE statements do not currently implicitly add a default extension .BAS if no extension is specified.\n\n"
                            + "Compatibility notes:\n"
                            + "\tUnoDOS 3 allows certain characters in the range $7F to $FF. However, their permissibility and interpretation depends on the console code page,\n"
                            + "\t\twhich may be different from the display code page currently in use. Therefore you should avoid using these characters.\n\n"
                            + "Errors:\n"
                            + "\t'file_num' is a non-existent stream: Undefined stream.\n"
                            + "\t'file_num' is not in range [0 to 15]: Bad I/O device.\n"
                            + "\t'mode_char' is a non-existent channel: Undefined channel.\n"
                            + "\t'file_spec' is non-existent in input or append mode: File not found.";
                    case "OPTION":
                    case "BASE":
                        return
                            "TODO: To declare the minimum value for array subscripts.\n\nSyntax:\nOPTION BASE n";
                    case "OUT":
                        return
                            "Syntax:\n"
                            + "\tOUT port, value\n\n"
                            + "Sends a byte to an emulated machine port.\n\n"
                            + "Parameters:\n"
                            + "\t'port' is a numeric expression in [0 to 65535].\n"
                            + "\t'value' is a numeric expression in [0 to 255].\n\n"
                            + "Errors:\n"
                            + "\t'port' or 'value' has a string value: Type mismatch.\n"
                            + "\t'port' is not in [0 to 65535]: Overflow.\n"
                            + "\t'value' is not in [0 to 32767]: Overflow.\n"
                            + "\t'value' is not in [0 to 255]: Illegal function call.";
                    case "PAINT":
                        return
                            "TODO: To fill in a graphics figure with the selected attribute.\n\nSyntax:\nPAINT (x start,y start)[,paint attribute[,border attribute][,bckgrnd attribute]]";
                    case "PALETTE":
                        return
                            "Syntax:\n"
                            + "\tPALETTE [attrib, colour]\n\n"
                            + "Assigns a colour to an attribute. All pixels with that attribute will change colour immediately.\n"
                            + "If no parameters are specified, PALETTE resets to the initial setting.\n\n"
                            + "Parameters:\n"
                            + "\t'attrib' is a numeric expression from [0 to 63].\n"
                            + "\t'colour' is a numeric expression between [0 and 255]\n\n"
                            + "Notes:\n"
                            + "\tColours are entered in compressed RGB format (lowest to highest bit).\n"
                            + "\tThe red and green levels are each stored in three bits (0 to 7) while the blue level is stored in two bits (0 to 3).\n"
                            + "\tThe easiest way to enter values is in octal (@BGR). For example, to set attribute to maximum blue, you would enter PALETTE attribute, @300.\n\n"
                            + "Errors:\n"
                            + "\t'attrib' or 'colour' has a string value: Type mismatch.\n"
                            + "\t'attrib' or 'colour' is not in [ to 32767]: Overflow\n"
                            + "\t'attrib' or 'colour' is not in range: Illegal function call";
                    case "PEN":
                        return
                            "TODO: To read the light pen.\n\nSyntax:\nPEN ON\nPEN OFF\nPEN STOP";
                    case "PLAY":
                        return
                            "TODO: To play music by embedding a music macro language into the string data type.\n\nSyntax:\nPLAY string expression";
                    case "POKE":
                        return
                            "Syntax:\n"
                            + "\tPOKE address, value\n\n"
                            + "Sets the value of the memory byte at 'segment' * 16 + 'address' to 'value', where 'segment' is the current segment set with DEF SEG.\n\n"
                            + "Parameters:\n"
                            + "\t'address' is a numeric expression in [-32768 to 65535]. Negative values are interpreted as their two's complement.\n"
                            + "\t'value' is a numeric expression in [0 to 255].\n\n"
                            + "Notes:\n"
                            + "\tDEF SEG is not yet implemented in SE Basic IV.\n\n"
                            + "Errors:\n"
                            + "\t'address' or 'value' has a string value: Type mismatch.\n"
                            + "\t'address' is not in [-32768 to 65535]: Overflow.\n"
                            + "\t'value' is not in [-32768 to 32767]: Overflow.\n"
                            + "\t'value' is not in [0 to 255]: Illegal function call.";
                    case "PRESET":
                        return
                            "TODO: To display a point at a specified place on the screen during use of the graphics mode.\n\nSyntax:\nPRESET(x,y)[,color]";
                    case "PSET":
                        return
                            "TODO: To display a point at a specified place on the screen during use of the graphics mode.\n\nSyntax:\nPSET(x,y)[,color]";
                    case "PRINT":
                        return
                            "Syntax:\n"
                            + "\tPRINT [# stream,] [expr_0|;|,|SPC( n)|TAB( n)] ... [USING format; uexpr_0 [{;|,} uexpr_1] ... [;|,]]\n\n"
                            + "Writes expressions to the screen, a file or another device.\n"
                            + "If stream is specified, output goes to the file or device open under that number.\n"
                            + "'?' is a shorthand for PRINT.\n\n"
                            + "When writing a string expression to the screen, the following control characters have special meaning.\n"
                            + "Other characters are shown as their corresponding glyph in the current codepage:\n\n"
                            + "Code point\tControl character\t\tEffect\n"
                            + "$07\t\tBEL\t\t\tBeep the speaker.\n"
                            + "$08\t\tBS\t\t\tErase the character in the previous column and move the cursor back.\n"
                            + "$09\t\tHT\t\t\tJump to the next 8-cell tab stop.\n"
                            + "$0A\t\tLF\t\t\tGo to the leftmost column in the next row; connect the rows to one logical line.\n"
                            + "$0B\t\tVT\t\t\tMove the cursor to the top left of the screen.\n"
                            + "$0C\t\tFF\t\t\tClear the screen.\n"
                            + "$0D\t\tCR\t\t\tGo to the leftmost column in the next row.\n"
                            + "$1C\t\tFS\t\t\tMove the cursor one column to the right.\n"
                            + "$1D\t\tGS\t\t\tMove the cursor one column to the left.\n"
                            + "$1E\t\tRS\t\t\tMove the cursor one row up.\n"
                            + "$1F\t\tUS\t\t\tMove the cursor one row down.\n"
                            + "Note: In SE Basic IV, anything after PRINT CHR$(12) is not printed.\n\n"
                            + "Expressions can optionally be separated by one or more of the following keywords:\n\n"
                            + "Keyword\t\tEffect\n"
                            + ";\t\tAttaches two expressions tight together;\n"
                            + "\t\t\tstrings will be printed without any space in between, numbers will have one space separating them,\n"
                            + "\t\t\tin addition to the space or minus sign that indicate the sign of the number.\n"
                            + ",\t\tThe expression after will be positioned at the next available tab stop.\n"
                            + "'\t\tInserts a newline.\n"
                            + "SPC(n)\t\tProduces 'n' spaces, where 'n' is a numeric expression. If 'n' is less than zero, it defaults to zero.\n"
                            + "\t\t\tIf 'n' is greater than the file width, it is taken modulo the file width.\n"
                            + "TAB(n)\t\tMoves to column 'n', where 'n' is a numeric expression. if 'n' is less than zero, it defaults to zero.\n"
                            + "\t\t\tIf 'n' is greater than the file width, it is taken modulo the file width.\n"
                            + "\t\t\tIf the current column is greater than 'n', TAB moves to column 'n' on the next line.\n\n"
                            + "If the print statement does not end in one of these four separation tokens, a newline is printed after the last expression.\n"
                            + "String expressions can be separated by one or more spaces, which has the same effect as separating by semicolons.";
                    case "PRINT#":
                        return
                            "Syntax:\n"
                            + "\tPRINT [# stream,] [expr_0|;|,|SPC( n)|TAB( n)] ... [USING format; uexpr_0 [{;|,} uexpr_1] ... [;|,]]\n\n"
                            + "Writes expressions to the screen, a file or another device.\n"
                            + "If stream is specified, output goes to the file or device open under that number.\n"
                            + "'?' is a shorthand for PRINT.\n\n"
                            + "When writing a string expression to the screen, the following control characters have special meaning.\n"
                            + "Other characters are shown as their corresponding glyph in the current codepage:\n\n"
                            + "Code point\tControl character\t\tEffect\n"
                            + "$07\t\tBEL\t\t\tBeep the speaker.\n"
                            + "$08\t\tBS\t\t\tErase the character in the previous column and move the cursor back.\n"
                            + "$09\t\tHT\t\t\tJump to the next 8-cell tab stop.\n"
                            + "$0A\t\tLF\t\t\tGo to the leftmost column in the next row; connect the rows to one logical line.\n"
                            + "$0B\t\tVT\t\t\tMove the cursor to the top left of the screen.\n"
                            + "$0C\t\tFF\t\t\tClear the screen.\n"
                            + "$0D\t\tCR\t\t\tGo to the leftmost column in the next row.\n"
                            + "$1C\t\tFS\t\t\tMove the cursor one column to the right.\n"
                            + "$1D\t\tGS\t\t\tMove the cursor one column to the left.\n"
                            + "$1E\t\tRS\t\t\tMove the cursor one row up.\n"
                            + "$1F\t\tUS\t\t\tMove the cursor one row down.\n"
                            + "Note: In SE Basic IV, anything after PRINT CHR$(12) is not printed.\n\n"
                            + "Expressions can optionally be separated by one or more of the following keywords:\n\n"
                            + "Keyword\t\tEffect\n"
                            + ";\t\tAttaches two expressions tight together;\n"
                            + "\t\t\tstrings will be printed without any space in between, numbers will have one space separating them,\n"
                            + "\t\t\tin addition to the space or minus sign that indicate the sign of the number.\n"
                            + ",\t\tThe expression after will be positioned at the next available tab stop.\n"
                            + "'\t\tInserts a newline.\n"
                            + "SPC(n)\t\tProduces 'n' spaces, where 'n' is a numeric expression. If 'n' is less than zero, it defaults to zero.\n"
                            + "\t\t\tIf 'n' is greater than the file width, it is taken modulo the file width.\n"
                            + "TAB(n)\t\tMoves to column 'n', where 'n' is a numeric expression. if 'n' is less than zero, it defaults to zero.\n"
                            + "\t\t\tIf 'n' is greater than the file width, it is taken modulo the file width.\n"
                            + "\t\t\tIf the current column is greater than 'n', TAB moves to column 'n' on the next line.\n\n"
                            + "If the print statement does not end in one of these four separation tokens, a newline is printed after the last expression.\n"
                            + "String expressions can be separated by one or more spaces, which has the same effect as separating by semicolons.";
                    case "PUT":
                        return
                            "TODO: To write a record from a random buffer to a random disk file.\n\tOR\nTo transfer graphics images to the screen.\n\nSyntax:\nPUT[#]file number[,record number]\n\tOR\nPUT(x,y),array,[,action verb]";
                    case "RANDOMIZE":
                        return
                            "Syntax:\n"
                            + "\tRANDOMIZE [expr]\n\n"
                            + "Seeds the random number generator with expr. If no seed is specified, RANDOMIZE will prompt the user to enter a random seed.\n"
                            + "The user-provided value is rounded to an integer. The random seed is formed of the last two bytes of that integer or expr.\n"
                            + "If 'expr' is a float (4 or 8 bytes), these are XORed with the preceding 2. The first 4 bytes of a double are ignored.\n"
                            + "The same random seed will lead to the same sequence of pseudorandom numbers being generated by the RND function.\n\n"
                            + "Parameters:\n"
                            + "\t'expr' is a numeric expression.\n\n"
                            + "Notes:\n"
                            + "\tFor the same seed, SE Basic IV produces the same pseudorandom numbers as Microsoft BASIC 3.23.\n"
                            + "\tThe random number generator is very poor and should not be used for serious purposes. See RND for details.\n\n"
                            + "Errors:\n"
                            + "\t'expr' has a string value: Illegal function call.\n"
                            + "\tThe user provides a seed outside [-32768 to 32767] at the prompt: Overflow.";
                    case "READ":
                        return
                            "Syntax:\n"
                            + "\tREAD var_0 [, var_1] ...\n\n"
                            + "Assigns data from a DATA statement to variables.\n"
                            + "Reading starts at the current DATA position, which is the DATA entry immediately after the last one read by previous READ statements.\n"
                            + "The DATA position is reset to the start by the RUN and RESTORE statements.\n\n"
                            + "Parameters:\n"
                            + "\t'var_0', 'var_1' are variables or array elements.\n\n"
                            + "Errors:\n"
                            + "\tNot enough data is present in DATA statements: Out of DATA.\n"
                            + "\tThe type of the variable is not compatible with that of the data entry being read: a Syntax error occurs on the DATA line.";
                    case "REM":
                        return
                            "Syntax:\n"
                            + "\t{REM|'} [anything]\n\n"
                            + "Ignores everything until the end of the line. The REM statement is intended for comments.\n"
                            + "Everything after REM will be stored in the program unaltered and uninterpreted. Apostrophe (') is an alias for REM.\n\n"
                            + "Note that a colon : does not terminate the REM statement; the colon and everything after it will be treated as part of the comment.";
                    case "RESTORE":
                        return
                            "Syntax:\n"
                            + "\tRESTORE [line]\n\n"
                            + "Resets the DATA pointer.\n\n"
                            + "Parameters:\n"
                            + "\t'line' is a line number.\n"
                            + "\t\tIf 'line' is not specified, the DATA pointer is reset to the first DATA entry in the program.\n"
                            + "\t\tIf it is specified, the DATA pointer is reset to the first DATA entry in or after 'line'.\n\n"
                            + "Errors:\n"
                            + "\t'line' is not an existing line number: Undefined line number.";
                    case "RESUME":
                        return
                            "TODO: To continue program execution after an error-recovery procedure has been  performed.\n\nSyntax:\nRESUME\nRESUME 0\nRESUME NEXT\nRESUME line number";
                    case "RETURN":
                        return
                            "Syntax:\n"
                            + "\tRETURN [line]\n\n"
                            + "Returns from a GOSUB subroutine.\n"
                            + "Parameters:\n"
                            + "\t'line' is a line number.\n"
                            + "If 'line' is not specified, RETURN jumps back to the statement after the GOSUB that jumped into the subroutine.\n"
                            + "If line is specified, it must be a valid line number. RETURN jumps to that line (and pops the GOSUB stack).\n"
                            + "When returning from an error trapping routine, RETURN re-enables the event trapping which was stopped on entering the trap routine.\n\n"
                            + "Errors:\n"
                            + "\t'line' is not an existing line number: Undefined line number.";
                    case "SCREEN":
                        return
                            "Syntax:\n"
                            + "\tSCREEN [mode] [, [colorburst] [, [apage] [, [vpage] [, erase]]]]\n\n"
                            + "Change the video mode, composite colorburst, active page and visible page.\n\n"
                            + "Parameters:\n"
                            + "\t'mode' is a numeric expression that sets the screen mode.\n\n"
                            + "Video mode\tNotes\n"
                            + "0\t\tText mode. 80x24 characters. Two attributes picked from 16 colors.\n"
                            + "1\t\tBitmap mode. 240 x 192 pixles. 40x24 characters of 6x8 pixels. 8x1 attributes from 16 colors.\n\n"
                            + "Notes:\n"
                            + "\tThe driver for SCREEN 1 is stored in RAM and can be replaced with a driver for any screen mode supported by the hardware.\n\n"
                            + "Errors:\n"
                            + "\tNo parameters are specified: Missing operand.\n"
                            + "\tAny parameter has a string value: Type mismatch.\n"
                            + "\tAny parameter is not in [-32768 to 32767]: Overflow.\n"
                            + "\t'mode' is not an available video mode number for your video card setting: Illegal function call.";
                    case "SEEK":
                        return
                            "Syntax:\n"
                            + "\tSEEK #file_num, file_pos\n\n"
                            + "Moves the current position in 'file_num' to 'file_pos'.\n\n"
                            + "Parameters:\n"
                            + "'file_num' is a numeric expressions yielding a file number.\n"
                            + "'file_pos' is a numeric expressions.\n\n"
                            + "Notes:\n"
                            + "\tNo error is raised if the specified file number is not open.";
                    case "SOUND":
                        return
                            "TODO: To generate sound through the speaker.\n\nSyntax:\nSOUND freq,duration";
                    case "STRIG":
                        return
                            "TODO: To set or return the status of the joystick triggers.\n\nSyntax:\nSTRIG ON\nSTRIG OFF";
                    case "SWAP":
                        return
                            "TODO: To exchange the values of two variables.\n\nSyntax:\nSWAP variable1,variable2";
                    case "TIME$":
                    case "TIME":
                        return
                            "TODO: To set or retrieve the current time.\n\nSyntax:\nTIME$ = string exp\nstring = TIME$";
                    case "USING":
                        return
                            "TODO:";
                    case "VIEW":
                        return
                            "TODO: To define a physical viewport limit from x1,y1 (upper-left x,y coordinates) to x2,y2 (lower-right x,y coordinates).\n\nSyntax:\nVIEW [[SCREEN][(x1,y1)-(x2,y2) [,[fill][,[border]]]]";
                    case "WAIT":
                        return
                            "Syntax:\n"
                            + "\tWAIT frames\n\n"
                            + "Pauses for (frames/60) seconds.\n\n"
                            + "Parameters:\n"
                            + "\t'frames' is in [0 to 65535].\n\n"
                            + "Notes:\n"
                            + "\tIn Microsoft BASIC, WAIT waits for input from a given port.\n\n"
                            + "Errors:\n"
                            + "\tAny parameter has a string value: Type mismatch.";
                    case "WEND":
                        return
                            "Syntax:\n"
                            + "\tWEND\n\n"
                            + "Iterates a WHILE—WEND loop: jumps to the matching WHILE statement, where its condition can be checked.\n\n"
                            + "Notes:\n"
                            + "\tWHILE—WEND loops can be nested. WEND jumps to the most recent WHILE statement that has not been closed by another WEND.\n\n"
                            + "Errors:\n"
                            + "\tAll previous WHILE statements have been closed by another WEND or no WHILE statement has been executed before: WEND without WHILE.";
                    case "WHILE":
                        return
                            "Syntax:\n"
                            + "\tWHILE expr\n\n"
                            + "Initiates a WHILE—WEND loop.\n"
                            + "If 'expr' evaluates to zero, WHILE jumps to the statement immediately after the matching WEND. If not, execution continues.\n\n"
                            + "Parameters:\n"
                            + "\t'expr' is a numeric expression.\n\n"
                            + "Errors:\n"
                            + "\tNo matching WEND is found: WHILE without WEND.\n"
                            + "\t'expr' has a string value: Type mismatch.";
                    case "WIDTH":
                        return
                            "TODO: To set the printed line width in number of characters for the screen and line printer.\n\nSyntax:\nWIDTH size\nWIDTH file number, size\nWIDTH \"dev\", size";
                    case "WINDOW":
                        return
                            "TODO: To draw lines, graphics, and objects in space not bounded by the physical  limits of the screen.\n\nSyntax:\nWINDOW[[SCREEN](x1,y1)-(x2,y2)]";
                    case "WRITE":
                        return
                            "TODO: To output data to the screen.\n\nSyntax:\nWRITE[list of expressions]";
                    case "WRITE#":
                        return
                            "TODO: To write data to a sequential file.\n\nSyntax:\nWRITE #filenum, list of expressions";
                    case "ABS":
                        return
                            "Syntax:\n"
                            + "\ty = ABS(x)\n\n"
                            + "Returns the absolute value of 'x' if 'x' is a number and the value of 'x' if 'x' is a string.\n\n"
                            + "Parameters:\n"
                            + "\t'x' is an expression.";
                    case "ACOS":
                        return
                            "Syntax:\n"
                            + "\ty = ACOS(x)\n\n"
                            + "Returns the inverse cosine of 'x'.\n\n"
                            + "Parameters:\n"
                            + "\t'x' is a numeric expression that gives the angle in radians.\n\n"
                            + "Errors:\n"
                            + "'x' has a string value: Type mismatch.";
                    case "ASC":
                        return
                            "Syntax:\n"
                            + "\tval = ASC(char)\n\n"
                            + "Returns the code point (ASCII value) for the first character of 'char'.\n\n"
                            + "Parameters:\n"
                            + "\t'char' is an expression with a string value.\n\n"
                            + "Errors:\n"
                            + "\t'char' has a numeric value: Type mismatch.\n"
                            + "\t'char' equals \"\": Illegal function call.";
                    case "ASIN":
                        return
                            "Syntax:\n"
                            + "\ty = ASIN(x)\n\n"
                            + "Returns the inverse sine of 'x'.\n\n"
                            + "Parameters:\n"
                            + "\t'x' is a numeric expression that gives the angle in radians.\n\n"
                            + "Errors:\n"
                            + "\t'x' has a string value: Type mismatch.";
                    case "ATAN":
                        return
                            "Syntax:\n"
                            + "\ty = ATAN(x)\n\n"
                            + "Returns the inverse tangent of 'x'.\n\n"
                            + "Parameters:\n"
                            + "\t'x' is a numeric expression that gives the angle in radians.\n\n"
                            + "Errors:\n"
                            + "\t'x' has a string value: Type mismatch.";
                    case "CHR$":
                    case "CHR":
                        return
                            "Syntax:\n"
                            + "\tchar = CHR$(x)\n\n"
                            + "Returns the character with code point 'x'.\n\n"
                            + "Parameters:\n"
                            + "\t'x' is a numeric expression in the range [0 to 255].\n\n"
                            + "Errors:\n"
                            + "\t'x' has a string value: Type mismatch.\n"
                            + "\t'x' is not in [-32768 to 32767]: Overflow.\n"
                            + "\t'x' is not in 0 to 255: Illegal function call.";
                    case "COS":
                        return
                            "Syntax:\n"
                            + "\tcosine = COS(angle)\n\n"
                            + "Returns the cosine of angle.\n\n"
                            + "Parameters:\n"
                            + "\t'angle' is a numeric expression that gives the angle in radians.\n\n"
                            + "Errors:\n"
                            + "\t'angle' has a string value: Type mismatch.";
                    case "DEEK":
                        return
                            "Syntax:\n"
                            + "\tvalue = DEEK(address)\n\n"
                            + "Returns the 16-bit value of the memory at 'segment' * 16 + 'address' where 'segment' is the current segment set with DEF SEG.\n\n"
                            + "Parameters:\n"
                            + "\t'address' is a numeric expression in [-32768 to 65535]. Negative values are interpreted as their two's complement.";
                    case "EOF":
                        return
                            "TODO: To return -1 (true) when the end of a sequential or a communications file has been reached, or to return 0 if end of file (EOF) has not been found.\n\nSyntax:\nEOF(file number)";
                    case "EXP":
                        return
                            "Syntax:\n"
                            + "\ty = EXP(x)\n\n"
                            + "Returns the exponential of 'x', that is e to the power 'x'.\n\n"
                            + "Parameters:\n"
                            + "\t'x' is a numeric expression.\n\n"
                            + "Errors:\n"
                            + "\t'x' has a string value: Type mismatch.\n"
                            + "\t'x' is larger than the natural logarithm of the maximum single-precision value: Overflow.";
                    case "FIX":
                        return
                            "Syntax:\n"
                            + "\twhole = FIX(number)\n\n"
                            + "Returns a number truncated towards zero.\n\n"
                            + "Parameters:\n"
                            + "\t'number' is a numeric expression.\n\n"
                            + "Notes:\n"
                            + "\tFIX truncates towards zero: it removes the fractional part. By contrast, INT truncates towards negative infinity.\n\n"
                            + "Errors:\n"
                            + "'number' is a string expression: Type mismatch.";
                    case "FN":
                        return
                            "Syntax:\n"
                            + "\tresult = FN[ ]name [(arg_0 [, arg_1] ...)\n\n"
                            + "Evaluates the user-defined function previously defined with DEF FN name. Spaces between FN and name are required.\n\n"
                            + "Parameters:\n"
                            + "\t'name' is the name of a previously defined function.\n"
                            + "\t'arg_0', 'arg_1', ... are expressions, given as parameters to the function.\n\n"
                            + "Notes:\n"
                            + "\tIn Microsoft BASIC, spaces between FN and name are optional.\n"
                            + "\tUnlike Microsoft BASIC, in SE Basic IV, functions can be called recursively, albeit without tail call optimization.\n\n"
                            + "Errors:\n"
                            + "\tNo function named 'name' is defined: Undefined user function.\n"
                            + "\tThe number of parameters differs from the function definition: Syntax error.\n"
                            + "\tThe type of one or more parameters differs from the function definition: Type mismatch.\n"
                            + "\tThe return type is incompatible with the function name's sigil: Type mismatch.";
                    case "FRE":
                        return
                            "TODO: To return the number of available bytes in allocated string memory.\n\nSyntax:\nFRE(x$)\nFRE(x)";
                    case "INKEY$":
                    case "INK.":
                    case "INKEY":
                        return
                            "Syntax:\n"
                            + "\tkey = INKEY$ [ #file_num]\n\n"
                            + "Returns one character from the stream 'file_num'. If no stream is specified, returns one key-press from the keyboard buffer.\n"
                            + "If the keyboard buffer is empty, returns the empty string. Otherwise, the return value is a one-character string holding the\n"
                            + "ASCII code of the pressed key.\n\n"
                            + "Notes:\n"
                            + "\tWhen a function key F1 to F15 is pressed, INKEY$ will return the letters of the associated macro unless it's been set to empty\n"
                            + "\twith the KEY statement, in which case it returns the e-ASCII code for the function key.";
                    case "INP":
                        return
                            "Syntax:\n"
                            + "\tcode = INP(port)\n\n"
                            + "Returns the value of a machine port.\n\n"
                            + "Parameters:\n"
                            + "\t'port' is a numeric expression in [0 to 65535].";
                    case "INPUT#":
                    case "INPUT$":
                    case "INPUT":
                        return
                            "Syntax:\n"
                            + "\t1) INPUT [;] [prompt {;|,}] var_0 [, var_1] ...\n"
                            + "\t2) TODO: INPUT$ \n"
                            + "\t3) TODO: INPUT# \n\n"
                            + "1) Prints prompt to the screen and waits for the user to input values for the specified variables.\n"
                            + "The semicolon before the prompt, if present, stops a newline from being printed after the values have been entered.\n"
                            + "If the prompt is followed by a semicolon, it is printed with a trailing '?'.\n"
                            + "\tIf the prompt is followed by a comma, no question mark is added.\n\n"
                            + "Parameters:\n"
                            + "\t'prompt' is a string literal.\n"
                            + "\t'var_0', 'var_1', ... are variable names or fully indexed array elements.\n\n"
                            + "Notes:\n"
                            + "\tValues entered must be separated by commas. Leading and trailing whitespace is discarded.\n"
                            + "\tString values can be entered with or without double quotes (\").\n"
                            + "\tIf a string with a comma, leading or trailing whitespace is needed, quotes are the only way to enter it.\n"
                            + "\tBetween a closing quote and the comma at the end of the entry, only white-space is allowed.\n"
                            + "\tIf quotes are needed in the string itself, the first character must be neither a quote nor whitespace.\n"
                            + "\t\tIt is not possible to enter a string that starts with a quote through INPUT.\n"
                            + "\tIf a given 'var_n' is a numeric variable, the value entered must be number literal.\n"
                            + "\tCharacters beyond the 255th character of the screen line are discarded.\n"
                            + "\tIf user input is interrupted by Ctrl+Break, CONT will re-execute the INPUT statement.\n\n"
                            + "Errors:\n"
                            + "\tIf the value entered for a numeric variable is not a valid numeric literal,\n"
                            + "\t\tor the number of values entered does not match the number of variables in the statement,\n"
                            + "\t\t?Redo from start is printed and all values must be entered again.\n"
                            + "\tA Syntax error that is caused after the prompt is printed is only raised after the values have been entered.\n"
                            + "\t\tNo values are stored.\n\n"
                            + "2) TODO: INP(n)\n\nSyntax:\nINPUT$(x[,[#]file number)]\n\n"
                            + "3) TODO: To read data items from a sequential file and assign them to program variables.\n\nSyntax:\nINPUT# file number, variable list.";
                    case "INSTR":
                        return
                            "Syntax:\n"
                            + "\tposition = INSTR([start,] parent, child)\n\n"
                            + "Returns the location of the first occurrence of the substring 'child' in 'parent'.\n\n"
                            + "Parameters:\n"
                            + "\t'parent' and 'child' are string expressions.\n"
                            + "\t'start' is a numeric expression in [1 to 255], specifying the starting position from where to look;\n"
                            + "\t\tif not specified, the search starts at character 1.\n\n"
                            + "Notes:\n"
                            + "\tIf 'child' is not a substring of 'parent' occurring at or before 'start', INSTR returns 0.\n"
                            + "\tIf the 'start' index is 0 (instead of 1), it still searches from the beginning.\n"
                            + "\tIf 'child' is empty, it is found right away at the 'start' index.\n\n"
                            + "Errors:\n"
                            + "\t'start' has a string value or 'parent' or 'child' have numeric values: Type mismatch.\n"
                            + "\t'start' is not in [-32768 to 32767]: Overflow.\n"
                            + "\t'start' is not in [1 to 255]: Illegal function call.";
                    case "INT":
                        return
                            "Syntax:\n"
                            + "\twhole = INT(number)\n\n"
                            + "Returns 'number' truncated towards negative infinity.\n\n"
                            + "Parameters:\n"
                            + "\t'number' is a numeric expression.\n\n"
                            + "Notes:\n"
                            + "\tFIX truncates towards zero: it removes the fractional part. By contrast, INT truncates towards negative infinity.\n\n"
                            + "Errors:\n"
                            + "\t'number' is a string expression, Type mismatch.";
                    case "LEFT$":
                    case "LEFT":
                        return
                            "Syntax:\n"
                            + "\tchild = LEFT$(parent, num_chars)"
                            + "Returns the left most number of characters 'num_chars' from the string 'parent'.\n\n"
                            + "Parameters:\n"
                            + "\t'parent' is a string expression.\n"
                            + "\t'num_chars' is a numeric expression.";
                    case "LEN":
                        return
                            "Syntax:\n"
                            + "\tlength = LEN(string)\n\n"
                            + "Returns the number of characters in 'string'.\n\n"
                            + "Parameters:\n"
                            + "\t'string' is a string expression.\n\n"
                            + "Errors:\n"
                            + "\t'string' has a number value: Type mismatch.";
                    case "LOC":
                        return
                            "TODO: To return the current position in the file.\n\nSyntax:\nLOC(file number)";
                    case "LOF":
                        return
                            "TODO: To return the length (number of bytes) allocated to the file.\n\nSyntax:\nLOF(file number)";
                    case "LOG":
                        return
                            "Syntax:\n"
                            + "\ty = LOG(x)\n\n"
                            + "Returns the natural logarithm of 'x'.\n\n"
                            + "Parameters:\n"
                            + "\t'x' is a numeric expression greater than zero.\n\n"
                            + "Errors:\n"
                            + "\t'x' has a string value: Type mismatch.\n"
                            + "\t'x' is zero or negative: Illegal function call.";
                    case "MID$":
                    case "MID":
                        return
                            "Syntax:\n"
                            + "\tsubstring = MID$(string, position [, length])"
                            + "Returns the middle 'length' characters starting at 'position' from the string 'parent'.\n\n"
                            + "Parameters:\n"
                            + "\t'string' is a string expression.\n"
                            + "\t'position' is a numeric expression.\n"
                            + "\t'length' is a numeric expression.";
                    case "MOUSE":
                        return
                            "TODO: MOUSE function";
                    case "NMI":
                        return
                            "TODO: NMI function";
                    case "PEEK":
                        return
                            "Syntax:\n"
                            + "\tvalue = PEEK(address)\n\n"
                            + "Returns the value of the memory at 'segment' * 16 + 'address' where 'segment' is the current segment set with DEF SEG.\n\n"
                            + "Parameters:\n"
                            + "\t'address' is a numeric expression in [-32768 to 65535]. Negative values are interpreted as their two's complement.\n\n"
                            + "Notes:\n"
                            + "\tCurrently PEEK only accepts values in the range [0 to 65535] and ignores SEG, returning values from the 64K address space.\n\n"
                            + "Errors:\n"
                            + "\t'address' has a string value: Type mismatch.\n"
                            + "\t'address' is not in [-32768 to 65535]: Overflow.";
                    case "PI":
                        return
                            "TODO: PI function";
                    case "PMAP":
                        return
                            "TODO: To map expressions to logical or physical coordinates.\n\nSyntax:\nPMAP (exp,function)";
                    case "POINT":
                        return
                            "TODO: To read the color or attribute value of a pixel from the screen.\n\nSyntax:\nPOINT(x,y)\nPOINT(function)";
                    case "POS":
                        return
                            "TODO: To return the current cursor position.\n\nSyntax:\nPOS(c)";
                    case "RIGHT$":
                    case "RIGHT":
                        return
                            "Syntax:\n"
                            + "\tchild = RIGHT$(parent, num_chars)"
                            + "Returns the right most number of characters 'num_chars' from the string 'parent'.\n\n"
                            + "Parameters:\n"
                            + "\t'parent' is a string expression.\n"
                            + "\t'num_chars' is a numeric expression.";
                    case "RND":
                        return
                            "Syntax:\n"
                            + "\trandom = RND[(x)]\n\n"
                            + "Returns a pseudorandom number in the interval 0 to 1.\n\n"
                            + "Parameters:\n"
                            + "\t'x' is a numeric expression.\n\n"
                            + "Notes:\n"
                            + "\tIf 'x' is zero, RND repeats the last pseudo-random number.\n"
                            + "\tIf 'x' is greater than zero, a new pseudorandom number is returned.\n"
                            + "\tIf 'x' is negative, 'x' is converted to a single-precision floating-point value and the random number seed is set to the\n"
                            + "\t\tabsolute value of its mantissa. The function then generates a new pseudorandom numer with this seed. Since only the mantissa of 'x' is used,\n"
                            + "\t\tany two values whose ratio is a power of 2 will produce the same seed.\n"
                            + "\tNote that this procedure for generating a new seed differs from that used by RANDOMIZE.\n"
                            + "\tSE Basic IV's RND function produces different random numbers from Microsoft BASIC.\n"
                            + "\tIt is a very poor random number generator. RND should not be used for cryptography, scientific simulations or anything else remotely serious.\n\n"
                            + "Errors:\n"
                            + "\t'x' has a string value: Type mismatch.";
                    case "SGN":
                        return
                            "Syntax:\n"
                            + "\tsign = SGN(number)\n\n"
                            + "Returns the sign of number: 1 for positive, 0 for zero and -1 for negative.\n\n"
                            + "Parameters:\n"
                            + "\t'number' is a numeric expression.\n\n"
                            + "Errors:\n"
                            + "\t'number' has a string value: Type mismatch.";
                    case "SIN":
                        return
                            "Syntax:\n"
                            + "\tsine = SIN(angle)\n\n"
                            + "Returns the sine of 'angle'.\n\n"
                            + "Parameters:\n"
                            + "\t'angle' is a numeric expression giving the angle in radians.\n\n"
                            + "Errors:\n"
                            + "\t'angle' has a string value: Type mismatch.";
                    case "SPACE$":
                    case "SPACE":
                        return
                            "TODO: To return a string of x spaces.\n\nSyntax:\nSPACE$(x)";
                    case "SPC":
                        return
                            "TODO: To skip a specified number of spaces in a PRINT or an LPRINT statement.\n\nSyntax:\nSPC(n)";
                    case "SQR":
                        return
                            "Syntax:\n"
                            + "\troot = SQR(number)\n\n"
                            + "Returns the square root of 'number'.\n\n"
                            + "Parameters:\n"
                            + "\t'number' is a numeric expression.\n\n"
                            + "Errors:\n"
                            + "\t'number' has a string value: Type mismatch.";
                    case "STICK":
                        return
                            "TODO: To return the x and y coordinates of two joysticks.\n\nSyntax:\nSTICK(n)";
                    case "STRING$":
                    case "STRING":
                        return
                            "Syntax:\n"
                            + "\tstring = STRING$(length, char)\n\n"
                            + "Returns a string of 'length' times the character 'char'.\n\n"
                            + "Parameters:\n"
                            + "\tIf 'char' is a numeric expression, it must be in [0 to 255] and is interpreted as the code point of the character.\n"
                            + "\tIf 'char' is a string expression, its first character is used.\n\n"
                            + "Errors:\n"
                            + "\t'length' has a string value: Type mismatch.\n"
                            + "\t'char' is the empty string: Illegal function call.\n"
                            + "\t'char' or 'length' is not in [-32768 to 32767]: Overflow.\n"
                            + "\t'char' or 'length' is not in [0 to 255]: Illegal function call.";
                    case "STR$":
                    case "STR":
                        return
                            "Syntax:\n"
                            + "\trepr = STR$(number)\n\n"
                            + "Returns the string representation of 'number'.\n\n"
                            + "Parameters:\n"
                            + "\t'number' is a numeric expression.\n\n"
                            + "Errors:\n"
                            + "\t'number' has a string value: Type mismatch.";
                    case "TAB":
                        return
                            "TODO: Spaces to position n on the screen.\n\nSyntax:\nTAB(n)";
                    case "TAN":
                        return
                            "Syntax:\n"
                            + "\ttangent = TAN(angle)\n\n"
                            + "Returns the tangent of 'angle'.\n\n"
                            + "Parameters:\n"
                            + "\t'angle' is a numeric expression giving the angle in radians.\n\n"
                            + "Errors:\n"
                            + "\t'angle' has a string value: Type mismatch.";
                    case "TIMER":
                        return
                            "TODO: To return single-precision floating-point numbers representing the elapsed number of seconds since midnight or system reset.\n\nSyntax:\nTIMER";
                    case "USR":
                        return
                            "Syntax:\n"
                            + "\tvalue = USR[n](expr)\n\n"
                            + "Calls a machine-code function and returns its return value.\n\n"
                            + "Parameters:\n"
                            + "\t'n' is a digit (0 to 9).\n"
                            + "\t'expr' is an expression.\n\n"
                            + "Errors:\n"
                            + "\t'n' is not a digit [0 to 9]: Syntax error.";
                    case "VAL$":
                    case "V.":
                    case "VAL":
                        return
                            "Syntax:\n"
                            + "\t1) value = VAL(string)\n"
                            + "\t2) value = VAL$(string)\n\n"
                            + "1) Returns the numeric value of the string expression 'string'.\n\n"
                            + "Notes:\n"
                            + "\tSpaces before a number are ignored: VAL(\" 10\") returns 10. But unlike Microsoft BASIC, spaces inside a number are not ignored.\n"
                            + "\tUnlike Microsoft BASIC, expressions inside the string expression are also evaluated.\n"
                            + "\t\tFor example, VAL \"5 + 5\" returns 10 and VAL \"foo\" returns the value of variable foo.\n"
                            + "\tExpressions between curly braces { and } are not evaluated, but their syntax is checked upon entering.\n"
                            + "\t\tThey are interpreted as strings that can be passed to VAL for actual evaluation.\n\n"
                            + "Errors:\n"
                            + "\t'string' has a number value: Type mismatch.\n\n"
                            + "2) Evaluates a 'string' as a string expression. For example:\n"
                            + "\t10 INPUT a$, x$\n"
                            + "\t20 PRINT VAL$ a$\n\n"
                            + "The string value assigned to 'a$' should be an expression using 'x$'.\n"
                            + "For example, \"x$+x$\". A string value is then assigned to 'x$', for example \"yo\".\n"
                            + "VAL$ strips the quotes of the value of 'a$' to get 'x$+x$' and evaluates it using the value assigned to 'x$' displaying the result \"yoyo\".\n\n"
                            + "Notes:\n"
                            + "\tThis function is not present in Microsoft BASIC. It is very useful for creating recursive functions,\n"
                            + "\t\tif used together with AND applied to string arguments, allowing for selective evaluation.\n"
                            + "\tExpressions between curly braces { and } are not evaluated, but their syntax is checked upon entering.\n"
                            + "\t\tThey are interpreted as strings that can be passed to VAL$ for actual evaluation.";
                    case "VARPTR$":
                    case "VARPTR":
                        return
                            "Syntax:\n"
                            + "TODO: To return the address in memory of the variable or file control block (FCB).\n\nSyntax:\nVARPTR(variable name)\nVARPTR(#file number)"
                            + "TODO: To return a character form of the offset of a variable in memory.\n\nSyntax:\nVARPTR$(variable)";
                    case "AUTO":
                        return
                            "TODO: To generate and increment line numbers automatically each time you press the RETURN key.\n\nSyntax:\nAUTO [line number][,[increment]]\nAUTO .[,[increment]]";
                    case "BLOAD":
                        return
                            "Syntax:\n"
                            + "\tBLOAD file_spec , offset\n\n"
                            + "Loads a memory image file into memory.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expression 'file_spec' is a valid file specification indicating the file to read the memory image from.\n"
                            + "\t'offset' is a numeric expression in the range [-32768 to 65535]. It indicates an offset in the current DEF SEG segment where the file is to be stored.\n"
                            + "\t\tIf not specified, the 'offset' stored in the BSAVE file will be used. If negative, its two's complement will be used.\n\n"
                            + "Errors:\n"
                            + "\tThe loaded file is not in BSAVE format: Bad file mode.\n"
                            + "\t'file_spec' contains disallowed characters: Bad file number (on CAS1:); Bad file name (on disk devices).\n"
                            + "\t'file_spec' has a numeric value: Type mismatch.\n"
                            + "\t'offset' is not in the range [-32768 to 65535]: Overflow.";
                    case "CHDIR":
                        return
                            "Syntax:\n"
                            + "\tCHDIR dir_spec\n\n"
                            + "Change the current folder on a disk device to 'dir_spec'. Each disk device has its own current folder.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expression 'dir_spec' is a valid file specification indicating an existing folder on a disk device.\n\n"
                            + "Errors:\n"
                            + "\tNo matching path is found: Path not found.\n"
                            + "\t'dir_spec' has a numeric value: Type mismatch.\n"
                            + "\t'dir_spec' is empty: Bad file name.";
                    case "CLEAR":
                        return
                            "Syntax:\n"
                            + "\tCLEAR [mem_limit]\n\n"
                            + "Clears all variables, arrays, DEF FN user functions. Closes all files. Turns off all sound. Clears all ON ERROR traps. Clears the loop stack.\n\n"
                            + "Parameters:\n"
                            + "\t'mem_limit' specifies the upper limit of usable memory. Default is previous memory size. Default memory size is 65535.\n\n"
                            + "Notes:\n"
                            + "\tIf called inside a FOR to NEXT or WHILE to WEND loop, an error will be raised at the NEXT or WEND statement, since the loop stacks have been cleared.\n\n"
                            + "Errors:\n"
                            + "\tAny of the arguments has a string value: Type mismatch.\n"
                            + "\t'mem_limit' is not in [0 to 65535]: Overflow.\n"
                            + "\t'mem_limit' is too low: Address out of range.";
                    case "CONT":
                        return
                            "Syntax:\n"
                            + "\tCONT\n\n"
                            + "Resumes execution of a program that has been halted by STOP, END or Esc.\n\n"
                            + "Notes:\n"
                            + "\tAnything after the CONT keyword is ignored.\n"
                            + "\tThis statement can only be used in direct mode.\n"
                            + "\tIf a break is encountered in GOSUB routine called from a continuing direct line (for example, GOSUB 100:PRINT A$),\n"
                            + "\t\tCONT will overwrite the running direct line. As the subroutine RETURNs to the position after the GOSUB in the old direct line,\n"
                            + "\t\tstrange things may happen if commands are given after CONT. In Microsoft BASIC, this can lead to strange errors in non-existing program lines\n"
                            + "\t\tas the parser executes bytes that are not part of a program line. In SE Basic IV, if the new direct line is shorter, execution stops after RETURN;\n"
                            + "\t\tbut if the direct line is extended beyond the old return position, the parser tries to resume at that return position, with strange effects.\n\n"
                            + "Errors:\n"
                            + "\tNo program is loaded, a program has not been run, after a program line has been modified or after CLEAR: Can't continue.\n"
                            + "\tThe break occurred in a direct line: Can't continue.\n"
                            + "\tCONT is used in a program: Can't continue.";
                    case "DELETE":
                        return
                            "Syntax:\n"
                            + "\tDELETE [line_number_0|.] [-[line_number_1|.]]\n\n"
                            + "Deletes a range of lines from the program. Also stops program execution and returns control to the user.\n\n"
                            + "Parameters:\n"
                            + "\t'line_number_0' and 'line_number_1' are line numbers in the range [0 to 65529], specifying the inclusive range of line numbers to delete.\n"
                            + "\tA '.' indicates the last line edited.\n\n"
                            + "Notes:\n"
                            + "\tIf the start point is omitted, the range will start at the start of the program.\n"
                            + "\tIf the end point is omitted, the range will end at the end of the program.\n"
                            + "\tIf no range is specified, the whole program will be deleted.\n\n"
                            + "Errors:\n"
                            + "\t'line_number_0' or 'line_number_1' is greater than 65529: Syntax error.\n"
                            + "\tThe range specified does not include any program lines stored: Illegal function call.";
                    case "FILES":
                        return
                            "Syntax:\n"
                            + "\tFILES [filter_spec]\n\n"
                            + "Displays the files fitting the specified filter in the specified folder on a disk device.\n"
                            + "If 'filter_spec' is not specified, displays all files in the current working folder.\n\n"
                            + "Parameters:\n"
                            + "\t'filter_spec' is a string expression that is much like a file specification, but optionally allows the file name part to contain wildcards.\n\n"
                            + "Notes:\n"
                            + "\tWildcards are not currently supported.\n\n"
                            + "Errors:\n"
                            + "\t'filter_spec' has a numeric value: Type mismatch.\n"
                            + "\t'filter_spec' is the empty string: Bad file name.\n"
                            + "\tThe specified filter does not match any files: File not found.";
                    case "KILL":
                        return
                            "Syntax:\n"
                            + "\tKILL file_spec\n\n"
                            + "Deletes a file on a disk device.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expression 'file_spec' is a valid file specification indicating the file to delete. It must point to an existing file on a disk device.\n\n"
                            + "Errors:\n"
                            + "\t'file_spec' has a number value: Type mismatch.\n"
                            + "\tThe file 'file_spec' is open: File already open\n"
                            + "\tThe file or path 'file_spec' does not exist: File not found\n"
                            + "\tThe user has no write permission: Permission denied\n"
                            + "\tIf a syntax error occurs after the closing quote, the file is removed anyway.";
                    case "LIST":
                        return
                            "Syntax:\n"
                            + "\tLIST [# file_num;] [line_number_0][, ][line_number_1]\n\n"
                            + "Prints the program to the screen or a file, starting with 'line_number_0' up to and including 'line_number_1'.\n"
                            + "Also stops program execution and returns control to the user.\n"
                            + "In all cases, any further statements in a compound after LIST will be ignored, both in a program and in direct mode.\n"
                            + "When listing to the screen, the same control characters are recognised as in the PRINT statement.\n\n"
                            + "Notes:\n"
                            + "\tIn Microsoft BASIC, LIST will not show line numbers 65531 to 65535 inclusive.\n"
                            + "\tSE Basic IV's line range is currently [0 to 16383].\n"
                            + "\tThere is no LLIST command. Instead, LIST can be directed to the printer stream using LIST #.\n\n"
                            + "Parameters:\n"
                            + "\t'line_number_0' and 'line_number_1' are line numbers in the range [0 to 65529] or a '.' to indicate the last line edited.\n"
                            + "\t\tThe line numbers do not need to exist; they specify a range. If the range is empty, nothing is printed.\n"
                            + "\tThe string expression 'file_num' is a valid stream indicating the file to list to.\n\n"
                            + "Errors:\n"
                            + "\tA line number is greater than 65529: Syntax error.\n"
                            + "\t'file_num' has a string value: Type mismatch.";
                    case "LIST#":
                        return
                            "Syntax:\n"
                            + "\tLIST [# file_num;] [line_number_0][, ][line_number_1]\n\n"
                            + "Prints the program to the screen or a file, starting with 'line_number_0' up to and including 'line_number_1'.\n"
                            + "Also stops program execution and returns control to the user.\n"
                            + "In all cases, any further statements in a compound after LIST will be ignored, both in a program and in direct mode.\n"
                            + "When listing to the screen, the same control characters are recognised as in the PRINT statement.\n\n"
                            + "Notes:\n"
                            + "\tIn Microsoft BASIC, LIST will not show line numbers 65531 to 65535 inclusive.\n"
                            + "\tSE Basic IV's line range is currently [0 to 16383].\n"
                            + "\tThere is no LLIST command. Instead, LIST can be directed to the printer stream using LIST #.\n\n"
                            + "Parameters:\n"
                            + "\t'line_number_0' and 'line_number_1' are line numbers in the range [0 to 65529] or a '.' to indicate the last line edited.\n"
                            + "\t\tThe line numbers do not need to exist; they specify a range. If the range is empty, nothing is printed.\n"
                            + "\tThe string expression 'file_num' is a valid stream indicating the file to list to.\n\n"
                            + "Errors:\n"
                            + "\tA line number is greater than 65529: Syntax error.\n"
                            + "\t'file_num' has a string value: Type mismatch.";
                    case "LOAD":
                        return
                            "Syntax:\n"
                            + "\tLOAD file_spec [,{\"R\"|\"T\"}]\n\n"
                            + "Loads the program stored in a file into memory. Existing variables will be cleared and any program in memory will be erased.\n"
                            + "LOAD implies a [CLEAR](#CLEAR).\n"
                            + "If \"R\" is specified, keeps all data files open and runs the specified file.\n"
                            + "If \"T\" is specified, loads a tokenized program.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expression 'file_spec' is a valid file specification indicating the file to read the program from.\n\n"
                            + "Notes:\n"
                            + "\tUnlike Microsoft BASIC, SE Basic IV always expects BASIC programs to be in plain text format.\n"
                            + "\tUnlike Microsoft BASIC, the \"R\" directive must be in quotes. Otherwise SE Basic IV would treat it as a variable.\n\n"
                            + "Errors:\n"
                            + "\t'file_spec' has a numeric value: Type mismatch.\n"
                            + "\t'file_spec' contains disallowed characters: Bad file number (on CAS1:); Bad file name (on disk devices).\n"
                            + "\tThe file specified in 'file_spec' cannot be found: File not found.\n"
                            + "\tA loaded text file contains lines without line numbers: Direct statement in file.\n"
                            + "\tA loaded text file contains lines longer than 255 characters: Line buffer overflow.\n"
                            + "\t\tAttempting to load a text file that has LF rather than CR LF line endings may cause this error.";
                    case "LOCK":
                        return
                            "TODO: \n\nSyntax:\nLOCK ???";
                    case "MERGE":
                        return
                            "Syntax:\n"
                            + "\tMERGE file_spec\n\n"
                            + "Merges the program stored in a file into memory.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expression 'file_spec' is a valid file specification indicating the file to read the program from.\n\n"
                            + "Notes:\n"
                            + "\tUnlike Microsoft BASIC, SE Basic IV always expects BASIC programs to be in plain text format.\n\n"
                            + "Errors:\n"
                            + "\t'file_spec' has a numeric value: Type mismatch.\n"
                            + "\t'file_spec' contains disallowed characters: Bad file number (on CAS1:); Bad file name (on disk devices)."
                            + "\tThe file specified in 'file_spec' cannot be found: File not found.\n"
                            + "\tA loaded text file contains lines without line numbers: Direct statement in file.\n"
                            + "\tA loaded text file contains lines longer than 255 characters: Line buffer overflow.\n"
                            + "\t\tAttempting to load a text file that has LF rather than CR LF line endings may cause this error.";
                    case "MKDIR":
                        return
                            "Syntax:\n"
                            + "\tMKDIR dir_spec\n\n"
                            + "Creates a new folder on a disk device.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expression 'dir_spec' is a valid file specification that specifies the path of the new folder on a disk device.\n\n"
                            + "Errors:\n"
                            + "\t'dir_spec' is not a string: Type mismatch.\n"
                            + "\tThe parent folder does not exist: Path not found.\n"
                            + "\tThe folder name already exists on that path: Path/File access error.\n"
                            + "\tThe user has no write permission: Permission denied.";
                    case "NAME":
                        return
                            "Syntax:\n"
                            + "\tNAME old_name TO new_name\n\n"
                            + "Renames the disk file 'old_name' into 'new_name'.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expressions 'old_name' and 'new_name' are valid file specifications\n"
                            + "\t\tgiving the path on a disk device to the old and new filenames, respectively.\n\n"
                            + "Notes:\n"
                            + "\t'new_name' will be modified into all-uppercase 8.3 format.\n\n"
                            + "Errors:\n"
                            + "\t'old_name' or 'new_name' have number values: Type mismatch.\n"
                            + "\t'old_name' does not exist: File not found.\n"
                            + "\t'old_name' is open: File already open.\n"
                            + "\t'new_name' exists: File already exists.";
                    case "NEW":
                        return
                            "Sytnax:\n"
                            + "\tNEW\n\n"
                            + "Stops execution of a program, deletes the program in memory, executes CLEAR and RESTORE and returns control to the user.";
                    case "OLD":
                        return
                            "Syntax\n"
                            + "\tOLD\n\n"
                            + "Loads a backup from disk of the program that was in memory the last time\n"
                            + "a NEW command was issued and returns control to the user.";
                    case "PCOPY":
                        return
                            "TODO: To copy one screen page to another in all screen modes.\n\nSyntax:\nPCOPY sourcepage, destinationpage";
                    case "RENUM":
                        return
                            "Syntax:\n"
                            + "\tRENUM [new|.] [, [old|.] [, increment]]\n\n"
                            + "Replaces the line numbers in the program by a systematic enumeration starting from 'new' and increasing by 'increment'.\n"
                            + "If 'old' is specified, line numbers less than 'old' remain unchanged.\n"
                            + "'new', 'old' are line numbers; the dot '.' signifies the last line edited.\n"
                            + "'increment' is a line number but must not be a dot or zero.\n\n"
                            + "Notes:\n"
                            + "\tThe following keywords can reference line numbers, which will be renumbered by RENUM:\n"
                            + "\t\tGOSUB, GOTO, LIST, RESTORE, RUN.\n\n"
                            + "Errors:\n"
                            + "\tAny of the parameters is not in [0 to 65529]: Syntax error.\n"
                            + "\tAny of the newly generated line numbers is greater than 65529: Illegal function call.\n"
                            + "\t\tThe line numbers up to the error have not been changed.\n"
                            + "\t'increment' is empty or zero: Illegal function call.\n"
                            + "\t'old' is specified and 'new' is less than or equal to an existing line number less than 'old': Illegal function call.";
                    case "RESET":
                        return
                            "TODO: To close all disk files and write the directory information to a diskette before it is removed from a disk drive.\n\nSyntax:\nRESET";
                    case "RMDIR":
                        return
                            "Sytnax:\n"
                            + "\tRMDIR dir_spec\n\n"
                            + "Removes an empty folder on a disk device.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expression 'dir_spec' is a valid file specification that specifies the path and name of the folder.\n\n"
                            + "Errors:\n"
                            + "\t'dir_spec' has a numeric value: Type mismatch.\n"
                            + "\t'dir_spec' is an empty string: Bad file name.\n"
                            + "\tNo matching path is found: Path not found.";
                    case "RUN":
                        return
                            "Sytnax:\n"
                            + "\tRUN [line_number | app_name [, p0, p1, ...]]\n\n"
                            + "Executes a program. Existing variables will be cleared and any program in memory will be erased.\n"
                            + "RUN implies a CLEAR. If an 'app_name' is specified, opens the application.\n\n"
                            + "Parameters:\n"
                            + "\t'line_number' is a valid line number in the current program.\n"
                            + "\t\tIf specified, execution starts from this line number. The rest of the RUN statement is ignored in this case.\n"
                            + "\tThe string expression 'app_name', if specified, is a valid application name\n"
                            + "\t\t(case-insensitive, truncated to the first 11 characters).\n"
                            + "\t'p0', 'p1', ... are variables.\n\n"
                            + "Errors:\n"
                            + "\t'line_number' is not a line number in the current program: Undefined line number.\n"
                            + "\t'app_name' cannot be found: File not found.";
                    case "SAVE":
                        return
                            "Syntax:\n"
                            + "\tSAVE file_spec [,\"T\"]\n\n"
                            + "Stores the current program in a file.\n"
                            + "If \"T\" is specified, saves a tokenized program.\n\n"
                            + "Parameters:\n"
                            + "\tThe string expression 'file_spec' is a valid file specification indicating the file to store to.\n\n"
                            + "Notes:\n"
                            + "\tIn Microsoft BASIC you can append \"A\" to save the file in plain text or \"P\" to save a protected listing,\n"
                            + "\totherwise the file is saved in tokenized format. In SE Basic IV the file is saved in plain text format unless you append \"T\".\n\n"
                            + "Errors:\n"
                            + "\t'file_spec' has a number value: Type mismatch.\n"
                            + "\t'file_spec' is an empty string: Bad I/O device.\n"
                            + "\t'file_spec' is too long: Bad I/O device.";
                    case "STOP":
                        return
                            "Syntax:\n"
                            + "\tSTOP\n\n"
                            + "Breaks program execution, prints a Break message on the console and returns control to the user.\n"
                            + "Files are not closed. It is possible to resume program execution at the next statement using CONT.";
                    case "TERM":
                        return
                            "TODO";
                    case "TRACE":
                        return
                            "Sytnax:\n"
                            + "\tTRACE {ON|OFF}\n\n"
                            + "Turns line number tracing on or off. If line number tracing is on, BASIC prints\n"
                            + "a tag [100] to the console when program line 100 is executed, and so forth.\n\n"
                            + "Notes:\n"
                            + "\tTracing is turned off by the NEW and LOAD statements.";
                    case "UNLOCK":
                        return
                            "TODO: \n\nSyntax:\nLOCK ???";
                    case "AND":
                        return
                            "TODO: \n\nSyntax:\nAND";
                    case "CSRLIN":
                        return
                            "TODO: To return the current line (row) position of the cursor.\n\nSyntax:\nCSRLIN";
                    case "ERL":
                        return
                            "TODO: To return the error code.\n\nSyntax:\nERR";
                    case "ERR":
                        return
                            "TODO: To return the line number associated with an error.\n\nSyntax:\nERR";
                    case "OR":
                        return
                            "TODO: \n\nSyntax:\nOR";
                    case "XOR":
                        return
                            "TODO: \n\nSyntax:\nXOR";
                    default:
                        return "ToolTip Not Found";
                }
            } else
            {
                switch (inS.ToUpper())
                {
                    case "ADC":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:adc\n"
                            + "The sum of the two operands plus the carry flag (0 or 1) is calculated,\n"
                            + "and the result is written back into the first operand.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "adc a,op8\t;8 bit\n"
                            + "adc hl,op16\t;16 bit\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "adc a,a\n"
                            + "adc a,b\n"
                            + "adc a,c\n"
                            + "adc a,d\n"
                            + "adc a,e\n"
                            + "adc a,h\n"
                            + "adc a,l\n"
                            + "adc a,ixh\n"
                            + "adc a,ixl\n"
                            + "adc a,iyh\n"
                            + "adc a,iyl\n"
                            + "adc a,(hl)\n"
                            + "adc a,(ix+n)\n"
                            + "adc a,(iy+n)\n"
                            + "adc a,n\t\t;8-bit constant\n"
                            + "\n"
                            + "adc hl,bc\n"
                            + "adc hl,de\n"
                            + "adc hl,hl\n"
                            + "adc hl,sp\n"
                            + "\n"
                            + "Effects:\n"
                            + "The N flag is reset, P/V is interpreted as overflow.\n"
                            + "The rest of the flags are modified by definition.\n"
                            + "In the case of 16-bit addition the H flag is undefined.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "rr represents a two byte register pair: BC, DE, HL, SP\n"
                            + "\n"
                            + "a,r\t4\n"
                            + "a,X\t7\n"
                            + "a,(hl)\t7\n"
                            + "a,(ix+X)\t19\n"
                            + "a,(iy+X)\t19\n"
                            + "hl,rr\t15\n";
                    case "ADD":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:add\n"
                            + "The values of the two operands are added together, and\n"
                            + "the result is written back to the first one.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "add a,op8\t;8 bits\n"
                            + "add op16,op16\t;16 bits\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "add a,a\n"
                            + "add a,b\n"
                            + "add a,c\n"
                            + "add a,d\n"
                            + "add a,e\n"
                            + "add a,h\n"
                            + "add a,l\n"
                            + "add a,ixh\n"
                            + "add a,ixl\n"
                            + "add a,iyh\n"
                            + "add a,ixl\n"
                            + "add a,(hl)\n"
                            + "add a,(ix+n)\n"
                            + "add a,(iy+n)\n"
                            + "add a,n\t\t;8-bit constant\n"
                            + "\n"
                            + "add hl,bc\n"
                            + "add hl,de\n"
                            + "add hl,hl\n"
                            + "add hl,sp\n"
                            + "\n"
                            + "add ix,bc\n"
                            + "add ix,de\n"
                            + "add ix,ix\n"
                            + "add ix,sp\n"
                            + "\n"
                            + "add iy,bc\n"
                            + "add iy,de\n"
                            + "add iy,iy\n"
                            + "add iy,sp\n"
                            + "\n"
                            + "Effects:\n"
                            + "8-bit arithmetic\n"
                            + "N flag is reset, P/V is interpreted as overflow.\n"
                            + "The rest of the flags are modified by definition.\n"
                            + "\n"
                            + "16-bit arithmetic\n"
                            + "preserves the S, Z and P/V flags, and H is undefined.\n"
                            + "The rest of the flags are modified by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "rr represents a two byte register pair: BC, DE, HL, SP\n"
                            + "\n"
                            + "r\t4\n"
                            + "X\t7\n"
                            + "(hl)\t7\n"
                            + "(ix+X)\t19\n"
                            + "(iy+X)\t19\n"
                            + "hl, rr\t11\n"
                            + "ix, rr\t15\n"
                            + "iy, rr\t15\n";
                    case "AND":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:and\n"
                            + "AND is an instruction that takes an 8-bit input an compares it with the accumulator.\n"
                            + "It checks to see if both are set. If either one is reset, the resulting bit in the accumulator is zero.\n"
                            + "0 and 0 result: 0\n"
                            + "0 and 1 result: 0\n"
                            + "1 and 0 result: 0\n"
                            + "1 and 1 result: 1\n"
                            + "\n"
                            + "Syntax:\n"
                            + "and op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "and a\n"
                            + "and b\n"
                            + "and c\n"
                            + "and d\n"
                            + "and e\n"
                            + "and h\n"
                            + "and l\n"
                            + "and ixh\n"
                            + "and ixl\n"
                            + "and iyh\n"
                            + "and iyl\n"
                            + "and (hl)\n"
                            + "and (ix+n)\n"
                            + "and (iy+n)\n"
                            + "and n\t\t;8 bit constant\n"
                            + "\n"
                            + "Effects:\n"
                            + "C and N flags cleared, P/V is parity, rest are altered by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t4\n"
                            + "X\t7\n"
                            + "(hl)\t7\n"
                            + "(ix+X)\t19\n"
                            + "(iy+X)\t19\n";
                    case "BIT":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:bit\n"
                            + "Tests if the specified bit is set.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "bit n,op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "n can be any integer from [0,7]. It must be defined on compile time.\n"
                            + "\n"
                            + "bit n,a\n"
                            + "bit n,b\n"
                            + "bit n,c\n"
                            + "bit n,d\n"
                            + "bit n,e\n"
                            + "bit n,h\n"
                            + "bit n,l\n"
                            + "bit n,(hl)\n"
                            + "bit n,(ix+n)\n"
                            + "bit n,(iy+n)\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t8\n"
                            + "(hl)\t12\n"
                            + "(ix+X)\t20\n"
                            + "(iy+X)\t20\n";
                    case "CALL":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:call\n"
                            + "Pushes the address after the CALL instruction (PC+3) onto the stack and jumps to the label.\n"
                            + "Can also take conditions.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "call label\t\t;unconditional call\n"
                            + "call cond.,label\t;conditional call\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "call label\t\t;always calls\n"
                            + "\n"
                            + "call c,label\t;calls if C flag is set\n"
                            + "call nc,label\t;calls if C flag is reset\n"
                            + "\n"
                            + "call z,label\t;calls if Z flag is set\n"
                            + "call nz,label\t;calls if Z flag is reset\n"
                            + "\n"
                            + "call m,label\t;calls if S flag is set\n"
                            + "call p,label\t;calls if S flag is reset\n"
                            + "\n"
                            + "call pe,label\t;calls if P/V is set\n"
                            + "call po,label\t;calls if P/V is reset\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "cc is condition: NZ, Z, NC, C, PO, PE, P, M\n"
                            + "\n"
                            + "XX\t17\n"
                            + "\n"
                            + "\tcondition true\tcondition false\n"
                            + "cc,XX\t17\t\t\t10\n";
                    case "CCF":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:ccf\n"
                            + "Inverts the carry flag.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "ccf\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "ccf\n"
                            + "\n"
                            + "Effects:\n"
                            + "Carry flag inverted. Also inverts H and clears N. Rest of the flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "CP":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:cp\n"
                            + "CP is a subtraction from A that doesn't update A, only the flags it would have\n"
                            + " set/reset if it really was subtracted.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "CP op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "cp a\n"
                            + "cp b\n"
                            + "cp c\n"
                            + "cp d\n"
                            + "cp e\n"
                            + "cp h\n"
                            + "cp l\n"
                            + "cp ixh\n"
                            + "cp ixl\n"
                            + "cp iyh\n"
                            + "cp iyl\n"
                            + "cp (ix+n)\n"
                            + "cp (iy+n)\n"
                            + "cp n\t\t;8 bit constant\n"
                            + "\n"
                            + "Effects:\n"
                            + "C, S, and Z flags modified by definition\n"
                            + "P/V detects overflow\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t4\n"
                            + "X\t7\n"
                            + "(hl)\t7\n"
                            + "(ix+X)\t19\n"
                            + "(iy+X)\t19\n";
                    case "CPD":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:cpd\n"
                            + "Multiple instructions combined into one. CPD does these things in this order:\n"
                            + "\n"
                            + "CP (HL)\n"
                            + "DEC HL\n"
                            + "DEC BC\n"
                            + "\n"
                            + "Syntax:\n"
                            + "CPD\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "cpd\n"
                            + "\n"
                            + "Effects:\n"
                            + "The carry is preserved, N is set and all other flags are modified by definition.\n"
                            + "P/V denotes the overflowing of BC, while the Z flag is set if A=(HL) before HL is decreased.\n"
                            + "\n"
                            + "T-States:\n"
                            + "16 t-states\n";
                    case "CPDR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:cpdr\n"
                            + "Repeats CPD until either:\n"
                            + "\n"
                            + "BC=0; or\n"
                            + "A=HL\n"
                            + "\n"
                            + "Syntax:\n"
                            + "CPDR\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "cpdr\n"
                            + "\n"
                            + "Effects:\n"
                            + "The carry is preserved, N is set and all the other flags are modified by definition.\n"
                            + "P/V denotes the overflowing of BC, while the Z flag is set if A=(HL) before HL is decreased.\n"
                            + "\n"
                            + "T-States:\n"
                            + "BC ≠ 0 and A ≠ (HL)\t21\n"
                            + "BC = 0 or A = (HL)\t16\n";
                    case "CPI":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:cpi\n"
                            + "Multiple instructions combined into one. CPI does these things in this order:\n"
                            + "\n"
                            + "CP (HL)\n"
                            + "INC HL\n"
                            + "DEC BC\n"
                            + "\n"
                            + "Syntax:\n"
                            + "CPI\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "cpi\n"
                            + "\n"
                            + "Effects:\n"
                            + "The carry is preserved, N is set and all the other flags are modified by definition.\n"
                            + "P/V denotes the overflowing of BC, while the Z flag is set if A=(HL) before HL is increased.\n"
                            + "\n"
                            + "T-States:\n"
                            + "16 t-states\n";
                    case "CPIR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:cpir\n"
                            + "Repeats CPI until either:\n"
                            + "\n"
                            + "BC=0; or\n"
                            + "A=HL\n"
                            + "\n"
                            + "Syntax:\n"
                            + "CPIR\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "cpir\n"
                            + "\n"
                            + "Effects:\n"
                            + "The carry is preserved, N is set and all the other flags are modified by definition.\n"
                            + "P/V denotes the overflowing of BC, while the Z flag is set if A=(HL) before HL is decreased.\n"
                            + "\n"
                            + "T-States:\n"
                            + "BC != 0 and A != (HL)\t21\n"
                            + "BC = 0 or A = (HL)\t16\n";
                    case "CPL":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:cpl\n"
                            + "CPL inverts all bits of A.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "cpl\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "cpl\n"
                            + "\n"
                            + "Effects:\n"
                            + "Sets H and N, other flags are unmodified.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "DAA":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:daa\n"
                            + "When this instruction is executed, the A register is BCD corrected using the contents of the flags.\n"
                            + "The exact process is the following:\n"
                            + "\tif the least significant four bits of A contain a non-BCD digit (i. e. it is greater than 9)\n"
                            + "\tor the H flag is set, then $06 is added to the register.\n"
                            + "\tThen the four most significant bits are checked.\n"
                            + "\tIf this more significant digit also happens to be greater than 9 or the C flag is set, then $60 is added.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "daa\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "daa\n"
                            + "\n"
                            + "Effects:\n"
                            + "If the second addition was needed, the C flag is set after execution, otherwise it is reset.\n"
                            + "The N flag is preserved, P/V is parity and the others are altered by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "DEC":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:dec\n"
                            + "Decreases operand by one.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "dec op8\t;8 bits\n"
                            + "dec op16\t;16 bits\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "dec a\n"
                            + "dec b\n"
                            + "dec c\n"
                            + "dec d\n"
                            + "dec e\n"
                            + "dec h\n"
                            + "dec l\n"
                            + "dec ixh\n"
                            + "dec ixl\n"
                            + "dec iyh\n"
                            + "dec iyl\n"
                            + "dec (hl)\n"
                            + "dec (ix+n)\n"
                            + "dec (iy+n)\n"
                            + "\n"
                            + "dec bc\n"
                            + "dec de\n"
                            + "dec hl\n"
                            + "dec ix\n"
                            + "dec iy\n"
                            + "dec sp\n"
                            + "\n"
                            + "Effects:\n"
                            + "8 Bits\n"
                            + "C flag preserved, P/V detects overflow and rest modified by definition.\n"
                            + "16 Bits\n"
                            + "No flags altered.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "rr represents a two byte register pair: BC, DE, HL, SP\n"
                            + "\n"
                            + "r\t4\n"
                            + "(hl)\t11\n"
                            + "(ix+X)\t23\n"
                            + "(iy+X)\t23\n"
                            + "rr\t6\n"
                            + "ix\t10\n"
                            + "iy\t10\n";
                    case "DI":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:di\n"
                            + "The DI Disables the Interrupts (both mode 1 and mode 2).\n"
                            + "\n"
                            + "Syntax:\n"
                            + "di\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "di\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "DJNZ":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:djnz\n"
                            + "Decreases B and jumps to a label if not zero.\n"
                            + "Note that DJNZ does a relative jump, so it can only jump between 128 bytes back/ahead.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "djnz label\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "djnz\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "\tB != 0\tB = 0\n"
                            + "X\t13\t8\n";
                    case "EI":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:ei\n"
                            + "EI enables the interrupts.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "ei\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "ei\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "EX":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:ex\n"
                            + "Exchanges two 16-bit values.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "ex op16,op16\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "ex af,af'\n"
                            + "ex de,hl\n"
                            + "ex (sp),hl\n"
                            + "ex (sp),ix\n"
                            + "ex (sp),iy\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "de,hl\t4\n"
                            + "af,af'\t4\n"
                            + "(sp),hl\t19\n"
                            + "(sp),ix\t19\n"
                            + "(sp),iy\t19\n";
                    case "EXX":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:exx\n"
                            + "EXX exchanges BC, DE, and HL with shadow registers with BC', DE', and HL'.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "exx\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "exx\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "HALT":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:halt\n"
                            + "Suspends all actions until the next interrupt.\n"
                            + "Note: Since halt does wait for the next interrupt, if you disable interrupts halt will run forever,\n"
                            + "resulting in a crash. Make sure that you always either know the interrupts will be on,\n"
                            + "or turn it on right before you use the halt instruction.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "halt\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "halt\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "IM":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:im\n"
                            + "Sets the interrupt mode.\n"
                            + "\n"
                            + "Mode 0\n"
                            + "An external device plugged into a z80 device generates the interrupt.\n"
                            + "\n"
                            + "Mode 1\n"
                            + "Interrupts are generated by the internal circuitry of the processor.\n"
                            + "Every time an interrupt is encountered, the OS performs an RST $28.\n"
                            + "\n"
                            + "Mode 2\n"
                            + "Allows user to determine when an interrupt happens, and what the interrupt does.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "im 0\n"
                            + "im 1\n"
                            + "im 2\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "im n\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "8 t-states for each mode\n";
                    case "IN":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:in\n"
                            + "Reads a value from a hardware port.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "in op8,(op8)\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "in a,(n)\t;8-bit constant\n"
                            + "in a,(c)\n"
                            + "in b,(c)\n"
                            + "in c,(c)\n"
                            + "in d,(c)\n"
                            + "in e,(c)\n"
                            + "in h,(c)\n"
                            + "in l,(c)\n"
                            + "in (c)\t;undocumented command\n"
                            + "\n"
                            + "Effects:\n"
                            + "in A,(N)\n"
                            + "All flags are preserved.\n"
                            + "Others\n"
                            + "N flag reset, P/V represents parity, C flag preserved, all other flags affected by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "A,X\t11\n"
                            + "r,(C)\t12\n";
                    case "INC":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:inc\n"
                            + "Increases operand by 1.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "inc op8\t;8 bits\n"
                            + "inc op16\t;16 bits\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "inc a\n"
                            + "inc b\n"
                            + "inc c\n"
                            + "inc d\n"
                            + "inc e\n"
                            + "inc h\n"
                            + "inc l\n"
                            + "inc ixh\n"
                            + "inc ixl\n"
                            + "inc iyh\n"
                            + "inc iyl\n"
                            + "inc (hl)\n"
                            + "inc (ix+n)\n"
                            + "inc (iy+n)\n"
                            + "\n"
                            + "inc bc\n"
                            + "inc de\n"
                            + "inc hl\n"
                            + "inc ix\n"
                            + "inc iy\n"
                            + "inc sp\n"
                            + "\n"
                            + "Effects:\n"
                            + "8 Bits\n"
                            + "Preserves C flag, N flag is reset, P/V detects overflow and rest are modified by definition.\n"
                            + "\n"
                            + "16 Bits\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "rr represents a two byte register pair: BC, DE, HL, SP\n"
                            + "\n"
                            + "r\t4\n"
                            + "(hl)\t11\n"
                            + "(ix+X)\t23\n"
                            + "(iy+X)\t23\n"
                            + "rr\t6\n"
                            + "ix\t10\n"
                            + "iy\t10\n";
                    case "IND":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:ind\n"
                            + "Reads the (C) port and writes the result to (HL), then decrements HL and decrements B.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "ind\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "ind\n"
                            + "\n"
                            + "Effects:\n"
                            + "C is preserved, the N flag is set. S, H and P/V are undefined.\n"
                            + "Z is set if B becomes zero after decrementing, otherwise it is reset.\n"
                            + "\n"
                            + "T-States:\n"
                            + "16 t-states\n";
                    case "INDR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:indr\n"
                            + "Reads the (C) port and writes the result to (HL). HL and B are decremented. Repeats until B = 0.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "indr\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "indr\n"
                            + "\n"
                            + "Effects:\n"
                            + "Z is set, carry is preserved, N is set, S,H, and P/V are undefined.\n"
                            + "\n"
                            + "T-States:\n"
                            + "B = 0\tB != 0\n"
                            + "16\t21\n";
                    case "INI":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:ini\n"
                            + "Reads the (C) port and writes the result to (HL), then increments HL and decrements B.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "ini\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "ini\n"
                            + "\n"
                            + "Effects:\n"
                            + "C is preserved, the N flag is reset. S, H and P/V are undefined.\n"
                            + "Z is set if B becomes zero after decrementing, otherwise it is reset.\n"
                            + "\n"
                            + "T-States:\n"
                            + "16 t-states\n";
                    case "INIR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:inir\n"
                            + "Reads from the (C) port, then writes to (HL). HL is incremented and B is decremented.\n"
                            + "Repeats until B = 0.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "inir\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "inir\n"
                            + "\n"
                            + "Effects:\n"
                            + "Z is set, C is reset, N is reset, S, H, and P/V are undefined.\n"
                            + "\n"
                            + "T-States:\n"
                            + "B = 0\tB != 0\n"
                            + "16\t21\n";
                    case "JP":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:jp\n"
                            + "Absolute jumps to the address. Can be conditional or unconditional.\n"
                            + "JP takes one more byte than JR, but is also slightly faster,\n"
                            + "so decide whether speed or size is more important before choosing JP or JR.\n"
                            + "JP (HL), JP (IX), and JP (IY) are unconditional and are the fastest jumps,\n"
                            + "and do not take more bytes than other jumps.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "jp NN\t\t;unconditional jump\n"
                            + "jp cond.,NN\t;conditional jump\n"
                            + "jp (reg16)\t;HL, IX and IY only\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + ";Constants\n"
                            + "jp NN\t\t;no condition\n"
                            + "jp C,NN\t\t;jumps if C is set\n"
                            + "jp NC,NN\t;jumps if C is reset\n"
                            + "jp Z,NN\t\t;jumps if Z is set\n"
                            + "jp NZ,NN\t;jumps if Z is reset\n"
                            + "jp M,NN\t\t;jumps if S is set\n"
                            + "jp P,NN\t\t;jumps if S is reset\n"
                            + "jp PE,NN\t\t;jumps if P/V is set\n"
                            + "jp PO,NN\t;jumps if P/V is reset\n"
                            + "\n"
                            + ";HL points to address\n"
                            + "jp (HL)\n"
                            + "\n"
                            + ";IX points to address\n"
                            + "jp (IX)\n"
                            + "\n"
                            + ";IY points to address\n"
                            + "jp (IY)\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "cc is condition: NZ, Z, NC, C, PO, PE, P, M\n"
                            + "\n"
                            + "XX\t10\n"
                            + "cc,XX\t10\n"
                            + "(hl)\t4\n"
                            + "(ix)\t8\n"
                            + "(iy)\t8\n";
                    case "JR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:jr\n"
                            + "Relative jumps to the address. This means that it can only jump between 128 bytes ahead or behind.\n"
                            + "Can be conditional or unconditional. JR takes up one less byte than JP, but is also slower.\n"
                            + "Weigh the needs of the code at the time before choosing one over the other (speed vs. size).\n"
                            + "\n"
                            + "Syntax:\n"
                            + "jr NN\t\t;unconditional jump\n"
                            + "jr cond.,NN\t;conditional jump\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + ";Constants\n"
                            + "jr NN\t\t;no contition\n"
                            + "jr C,NN\t\t;jumps if C is set\n"
                            + "jr NC,NN\t;jumps if C is reset\n"
                            + "jr Z,NN\t\t;jumps if Z is set\n"
                            + "jr NZ,NN\t;jumps if Z is reset\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "cc is condition: NZ, Z, NC, C, PO, PE, P, M\n"
                            + "\n"
                            + "X\t12\n"
                            + "\tcondition met\tcondition not met\n"
                            + "cc,X\t12\t\t7\n";
                    case "LD":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:ld\n"
                            + "The LD instruction is used to put the value from one place into another place.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "ld N,M\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "ld a,X\t\tX = \ta\tb\tc\td\te\th\tl\ti\tr\tixh\tixl\tiyh\tiyl\t(bc)\t(de)\t(hl)\n"
                            + "\t\t\t(ix+n)\t(iy+n)\tn\t(nn)\n"
                            + "ld b,X\t\tX = \ta\tb\tc\td\te\th\tl\tixh\tixl\tiyh\tiyl\t(hl)\t(ix+n)\t(iy+n)\tn\n"
                            + "ld c,X\t\tX = \ta\tb\tc\td\te\th\tl\tixh\tixl\tiyh\tiyl\t(hl)\t(ix+n)\t(iy+n)\tn\n"
                            + "ld d,X\t\tX = \ta\tb\tc\td\te\th\tl\tixh\tixl\tiyh\tiyl\t(hl)\t(ix+n)\t(iy+n)\tn\n"
                            + "ld e,X\t\tX = \ta\tb\tc\td\te\th\tl\tixh\tixl\tiyh\tiyl\t(hl)\t(ix+n)\t(iy+n)\tn\n"
                            + "ld h,X\t\tX = \ta\tb\tc\td\te\th\tl\t(hl)\t(ix+n)\t(iy+n)\tn\n"
                            + "ld l,X\t\tX = \ta\tb\tc\td\te\th\tl\t(hl)\t(ix+n)\t(iy+n)\tn\n"
                            + "ld i,a\n"
                            + "ld r,a\n"
                            + "ld ixh,X\t\tX = \ta\tb\tc\td\te\tixh\tixl\tn\t(nn)\n"
                            + "ld ixl,X\t\tX = \ta\tb\tc\td\te\tixh\tixl\tn\t(nn)\n"
                            + "ld iyh,X\t\tX = \ta\tb\tc\td\te\tiyh\tiyl\tn\t(nn)\n"
                            + "ld iyl,X\t\tX = \ta\tb\tc\td\te\tiyh\tiyl\tn\t(nn)\n"
                            + "ld ixh,X\t\tX = \ta\tb\tc\td\te\tixh\tixl\tn\t(nn)\n"
                            + "ld bc,X\t\tX = \tnn\t(nn)\n"
                            + "ld de,X\t\tX = \tnn\t(nn)\n"
                            + "ld hl,X\t\tX = \tnn\t(nn)\n"
                            + "ld sp,X\t\tX = \tnn\t(nn)\n"
                            + "ld ix,X\t\tX = \tnn\t(nn)\n"
                            + "ld iy,X\t\tX = \tnn\t(nn)\n"
                            + "ld (bc),a\n"
                            + "ld (de),a\n"
                            + "ld (hl),X\t\tX = \ta\tb\tc\td\te\tixh\tixl\tn\t(nn)\n"
                            + "ld (ix+n),X\tX = \ta\tb\tc\td\te\tixh\tixl\tn\t(nn)\n"
                            + "ld (iy+n),X\tX = \ta\tb\tc\td\te\tixh\tixl\tn\t(nn)\n"
                            + "ld (nn),X\t\tX = \ta\tbc\tde\thl\tsp\tix\tiy\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved except in the cases of the I or R registers. In those cases, C is preserved, H and N are reset, and alters Z and S.\n"
                            + "P/V is set if interrupts are enabled, reset otherwise.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register | rr represents a two byte register pair: BC, DE, HL, SP\n"
                            + "\n"
                            + "r,r'\t4\tr,X\t7\tr,(hl)\t7\n"
                            + "r,(ix+X)\t19\tr,(iy+X)\t19\ta,(vc)\t7\n"
                            + "a,(de)\t7\ta,(XX)\t13\t(bc),a\t7\n"
                            + "(de),a\t7\t(XX),a\t13\ta,i\t9\n"
                            + "a,r\t9\ti,a\t9\tr,a\t9\n"
                            + "a,(BC)\t7\t(XX),a\t13\trr,XX\t10\n"
                            + "ix,XX\t14\tiy,XX\t14\thl,(XX)\t20\n"
                            + "ix,(XX)\t20\tiy,(XX)\t20\t(XX),hl\t20\n"
                            + "(XX),rr\t20\t(XX),ix\t20\t(XX),iy\t20\n"
                            + "sp,hl\t6\tsp,ix\t10\tsp,iy\t10\n";
                    case "LDD":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:ldd\n"
                            + "Does a sort of \"LD(DE),(HL)\", then decrements DE, HL, and BC.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "ldd\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "ldd\n"
                            + "\n"
                            + "Effects:\n"
                            + "P/V is reset in case of overflow (if BC=0 after calling LDD).\n"
                            + "\n"
                            + "T-States:\n"
                            + "16 t-states\n";
                    case "LDDR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:lddr\n"
                            + "Repeats the instruction LDD (Does a LD (DE),(HL) and decrements each of DE, HL, and BC) until BC=0.\n"
                            + "Note that if BC=0 before the start of the routine, it will loop around until BC=0 again.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "lddr\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "lddr\n"
                            + "\n"
                            + "Effects:\n"
                            + "P/V is reset.\n"
                            + "\n"
                            + "T-States:\n"
                            + "BC is not 0\t21\n"
                            + "BC equals 0\t16\n";
                    case "LDI":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:ldi\n"
                            + "Performs a \"LD(DE),(HL)\", then increments DE and HL, and decrements BC.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "ldi\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "ldi\n"
                            + "\n"
                            + "Effects:\n"
                            + "P/V is reset in case of overflow (if BC=0 after calling LDI).\n"
                            + "\n"
                            + "T-States:\n"
                            + "16 t-states\n";
                    case "LDIR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:ldir\n"
                            + "Repeats LDI (LD (DE),(HL), then increments DE, HL, and decrements BC) until BC=0.\n"
                            + "Note that if BC=0 before this instruction is called, it will loop around until BC=0 again.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "ldir\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "ldir\n"
                            + "\n"
                            + "Effects:\n"
                            + "P/V is reset.\n"
                            + "\n"
                            + "T-States:\n"
                            + "BC = 0\tBC != 0\n"
                            + "16\t21\n";
                    case "NEG":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:neg\n"
                            + "NEG negates the accumulator.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "neg\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "neg\n"
                            + "\n"
                            + "Effects:\n"
                            + "N flag is set, all other flags modified by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "8 t-states\n";
                    case "NOP":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:nop\n"
                            + "NOP does nothing for 4 clock cycles.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "nop\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "nop\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "OR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:or\n"
                            + "OR is an instruction that takes an 8-bit input an compare sit with the accumulator.\n"
                            + "It checks to see if anything is set, and if neither are set, it results in a zero.\n"
                            + "0 and 0 result: 0\n"
                            + "0 and 1 result: 1\n"
                            + "1 and 0 result: 1\n"
                            + "1 and 1 result: 1\n"
                            + "\n"
                            + "Syntax:\n"
                            + "or op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "or a\n"
                            + "or b\n"
                            + "or c\n"
                            + "or d\n"
                            + "or e\n"
                            + "or h\n"
                            + "or l\n"
                            + "or ixh\n"
                            + "or ixl\n"
                            + "or iyh\n"
                            + "or iyl\n"
                            + "or (hl)\n"
                            + "or (ix+n)\n"
                            + "or (iy+n)\n"
                            + "or n\t\t;8 bit constant\n"
                            + "\n"
                            + "Effects:\n"
                            + "C and N flags cleared, P/V detects parity, and rest are modified by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t4\n"
                            + "X\t7\n"
                            + "(hl)\t7\n"
                            + "(ix+X)\t19\n"
                            + "(iy+X)\t19\n";
                    case "OTDR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:otdr\n"
                            + "Reads from (HL) and writes to the (C) port. HL and B are then decremented.\n"
                            + "Repeats until B = 0.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "otdr\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "otdr\n"
                            + "\n"
                            + "Effects:\n"
                            + "C is preserved, Z is set, N is set, S, H, and P/V are undefined.\n"
                            + "\n"
                            + "T-States:\n"
                            + "B = 0\tB != 0\n"
                            + "16\t21\n";
                    case "OTIR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:otir\n"
                            + "Reads from (HL) and writes to the (C) port. HL is incremented and B is decremented.\n"
                            + "Repeats until B = 0.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "otir\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "otir\n"
                            + "\n"
                            + "Effects:\n"
                            + "Z is set, C is preserved, N is reset, H, S, and P/V are undefined.\n"
                            + "\n"
                            + "T-States:\n"
                            + "B = 0\tB != 0\n"
                            + "16\t21\n";
                    case "OUT":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:out\n"
                            + "Writes the value of the second operand into the port given by the first operand.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "out (imm8),a\n"
                            + "out (c),reg8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "out (imm8),a\n"
                            + "out (c),a\n"
                            + "out (c),b\n"
                            + "out (c),c\n"
                            + "out (c),d\n"
                            + "out (c),e\n"
                            + "out (c),h\n"
                            + "out (c),l\n"
                            + "\n"
                            + "out (c),0\t;Zero. Note: Undocumented\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "A,X\t11\n"
                            + "r,(C)\t12\n";
                    case "OUTD":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:outd\n"
                            + "Writes the value from (HL) to the (C) port, then decrements B and HL.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "outd\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "outd\n"
                            + "\n"
                            + "Effects:\n"
                            + "C is preserved, N is set, H, S, and P/V are undefined. \n"
                            + "Z is set only if B becomes zero after decrement, otherwise it is reset.\n"
                            + "\n"
                            + "T-States:\n"
                            + "16 t-states\n";
                    case "OUTI":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:outi\n"
                            + "Reads from (HL) and writes to the (C) port. HL is then incremented, and B is decremented.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "outi\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "outi\n"
                            + "\n"
                            + "Effects:\n"
                            + "C is preserved, N is reset, H, S, and P/V are undefined.\n"
                            + "Z is set only if B becomes zero after decrement, otherwise it's reset.\n"
                            + "\n"
                            + "T-States:\n"
                            + "16 t-states\n";
                    case "POP":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:pop\n"
                            + "Copies the two bytes from (SP) into the operand, then increases SP by 2.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "pop reg16\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "pop af\n"
                            + "pop bc\n"
                            + "pop de\n"
                            + "pop hl\n"
                            + "pop ix\n"
                            + "pop iy\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved except when popping AF.\n"
                            + "\n"
                            + "T-States:\n"
                            + "rr represents a two byte register pair: BC, DE, HL, SP\n"
                            + "\n"
                            + "rr\t10\n"
                            + "ix\t14\n"
                            + "iy\t14\n";
                    case "PUSH":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:push\n"
                            + "Copies the operand into (SP), then increments SP by 2.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "push reg16\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "push af\n"
                            + "push bc\n"
                            + "push de\n"
                            + "push hl\n"
                            + "push ix\n"
                            + "push iy\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "rr represents a two byte register pair: BC, DE, HL, SP\n"
                            + "\n"
                            + "rr\t11\n"
                            + "ix\t15\n"
                            + "iy\t15\n";
                    case "RES":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:res\n"
                            + "Resets the specified byte to zero.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "res n,op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "res n,a\n"
                            + "res n,b\n"
                            + "res n,c\n"
                            + "res n,d\n"
                            + "res n,e\n"
                            + "res n,h\n"
                            + "res n,l\n"
                            + "res n,(hl)\n"
                            + "res n,(ix+n)\n"
                            + "res n,(iy+n)\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t8\n"
                            + "(hl)\t15\n"
                            + "(ix+X)\t23\n"
                            + "(iy+X)\t23\n";
                    case "RET":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:ret\n"
                            + "Pops the top of the stack into the program counter.\n"
                            + "Note that RET can be either conditional or unconditional.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "ret\t\t;no conditions\n"
                            + "ret cond.\t;conditional\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "ret z\t\t; Z flag is set\n"
                            + "ret nz\t\t; Z flag is reset\n"
                            + "ret c\t\t; Carry flag is set\n"
                            + "ret nc\t\t; Carry flag is reset\n"
                            + "ret m\t\t; S flag is set\n"
                            + "ret p\t\t; S flag is reset\n"
                            + "ret pe\t\t; P/V is set\n"
                            + "ret po\t\t; P/V is reset\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "cc is condition: NZ, Z, NC, C, PO, PE, P, M\n"
                            + "\n"
                            + "ret\t10\n"
                            + "\tcondition true\tcondition false\n"
                            + "ret cc\t11\t\t5\n";
                    case "RETI":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:reti\n"
                            + "Returns from an interrupt routine. Note: RETI cannot use return conditions.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "reti\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "reti\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "14 t-states\n";
                    case "RETN":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:retn\n"
                            + "Returns from the non-maskable interrupt (NMI). Cannot take return conditions.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "retn\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "retn\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "14 t-states\n";
                    case "RL":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:rl\n"
                            + "9-bit rotation to the left. The register's bits are shifted left.\n"
                            + "The carry value is put into 0th bit of the register, and the leaving 7th bit is put into the carry.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "rl op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "rl A\n"
                            + "rl B\n"
                            + "rl C\n"
                            + "rl D\n"
                            + "rl H\n"
                            + "rl L\n"
                            + "rl (HL)\n"
                            + "rl (IX+n)\n"
                            + "rl (IY+n)\n"
                            + "\n"
                            + "Effects:\n"
                            + "C is changed to the leaving 7th bit, H and N are reset, P/V is parity,\n"
                            + "S and Z are modified by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t8\n"
                            + "(hl)\t15\n"
                            + "(ix+X)\t23\n"
                            + "(iy+X)\t23\n";
                    case "RLA":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:rla\n"
                            + "Performs an RL A, but is much faster and S, Z, and P/V flags are preserved.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "rla\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "rla\n"
                            + "\n"
                            + "Effects:\n"
                            + "C is changed to the leaving 7th bit, H and N are reset, P/V , S and Z are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "RLC":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:rlc\n"
                            + "8-bit rotation to the left.\n"
                            + "The bit leaving on the left is copied into the carry, and to bit 0.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "rlc op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "rlc A\n"
                            + "rlc B\n"
                            + "rlc C\n"
                            + "rlc D\n"
                            + "rlc E\n"
                            + "rlc H\n"
                            + "rlc L\n"
                            + "rlc (HL)\n"
                            + "rlc (IX+n)\n"
                            + "rlc (IY+n)\n"
                            + "\n"
                            + "Effects:\n"
                            + "H and N flags are reset, P/V is parity, S and Z are modified by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t8\n"
                            + "(hl)\t15\n"
                            + "(ix+X)\t23\n"
                            + "(iy+X)\t23\n";
                    case "RLCA":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:rlca\n"
                            + "Performs RLC A much quicker, and modifies the flags differently.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "rlca\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "rlca\n"
                            + "\n"
                            + "Effects:\n"
                            + "S,Z, and P/V are preserved, H and N flags are reset.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states";
                    case "RLD":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:rld\n"
                            + "Performs a 4-bit leftward rotation of the 12-bit number\n"
                            + "whose 4 most significant bits are the 4 least significant bits of A,\n"
                            + "and its 8 least significant bits are in (HL).\n"
                            + "\n"
                            + "Syntax:\n"
                            + "rld\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "rld\n"
                            + "\n"
                            + "Effects:\n"
                            + "The H and N flags are reset, P/V is parity, C is preserved,\n"
                            + "and S and Z are modified by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "18 t-states\n";
                    case "RR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:rr\n"
                            + "9-bit rotation to the right. The carry is copied into bit 7,\n"
                            + "and the bit leaving on the right is copied into the carry.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "rr op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "rr A\n"
                            + "rr B\n"
                            + "rr C\n"
                            + "rr D\n"
                            + "rr E\n"
                            + "rr H\n"
                            + "rr L\n"
                            + "rr (HL)\n"
                            + "rr (IX+n)\n"
                            + "rr (IY+n)\n"
                            + "\n"
                            + "Effects:\n"
                            + "Carry becomes the bit leaving on the right, H and N flags are reset,\n"
                            + "P/V is parity, S and Z are modified by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t8\n"
                            + "(hl)\t15\n"
                            + "(ix+X)\t23\n"
                            + "(iy+X)\t23\n";
                    case "RRA":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:rra\n"
                            + "Performs a RR A faster, and modifies the flags differently.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "rra\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "rra\n"
                            + "\n"
                            + "Effects:\n"
                            + "The Carry becomes the bit leaving on the right, H, N flags are reset,\n"
                            + "P/V , S, and Z are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "RRC":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:rrc\n"
                            + "8-bit rotation to the right. the bit leaving on the right\n"
                            + "is copied into the carry, and into bit 7.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "rrc op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "rrc A\n"
                            + "rrc B\n"
                            + "rrc C\n"
                            + "rrc D\n"
                            + "rrc E\n"
                            + "rrc H\n"
                            + "rrc L\n"
                            + "rrc (HL)\n"
                            + "rrc (IX+n)\n"
                            + "rrc (IY+n)\n"
                            + "\n"
                            + "Effects:\n"
                            + "The carry becomes the value leaving on the right, H and N are reset,\n"
                            + "P/V is parity, and S and Z are modified by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t8\n"
                            + "(hl)\t15\n"
                            + "(ix+X)\t23\n"
                            + "(iy+X)\t23\n";
                    case "RRCA":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:rrca\n"
                            + "Performs a RRC A faster and modifies the flags differently.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "rrca\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "rrca\n"
                            + "\n"
                            + "Effects:\n"
                            + "The carry becomes the value leaving on the right, H and N are reset,\n"
                            + "P/V, S, and Z are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "RRD":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:rrd\n"
                            + "Like rld, except rotation is rightward.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "rrd\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "rrd\n"
                            + "\n"
                            + "Effects:\n"
                            + "The H and N flags are reset, P/V is parity, C is preserved,\n"
                            + "and S and Z are modified by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "18 t-states\n";
                    case "RST":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:rst\n"
                            + "The current PC value plus three is pushed onto the stack.\n"
                            + "The MSB is loaded with $00 and the LSB is loaded with imm8.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "rst imm8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "rst $00\n"
                            + "rst $08 ;rOP1ToOP2\n"
                            + "rst $10 ;rFindSym\n"
                            + "rst $18 ;rPushRealO1\n"
                            + "rst $20 ;rMove9ToOP1\n"
                            + "rst $28 ;rBCALL\n"
                            + "rst $30 ;rFPAdd\n"
                            + "rst $38\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "11 t-states\n";
                    case "SBC":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:sbc\n"
                            + "Sum of second operand and carry flag is subtracted from the first operand.\n"
                            + "Results are written into the first operand.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "sbc a,op8\t;8 bits\n"
                            + "sbc hl,op16\t;16 bits\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "sbc a,a\n"
                            + "sbc a,b\n"
                            + "sbc a,c\n"
                            + "sbc a,d\n"
                            + "sbc a,e\n"
                            + "sbc a,h\n"
                            + "sbc a,l\n"
                            + "sbc a,ixh\n"
                            + "sbc a,ixl\n"
                            + "sbc a,iyh\n"
                            + "sbc a,iyl\n"
                            + "sbc a,(hl)\n"
                            + "sbc a,(ix+n)\n"
                            + "sbc a,(iy+n)\n"
                            + "sbc a,n\t\t;8 bits\n"
                            + "\n"
                            + "sbc hl,bc\n"
                            + "sbc hl,de\n"
                            + "sbc hl,hl\n"
                            + "sbc hl,sp\n"
                            + "\n"
                            + "Effects:\n"
                            + "N flag is set, P/V detects overflow, rest modified by definition.\n"
                            + "In the case of 16-bit registers, h flag is undefined.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "rr represents a two byte register pair: BC, DE, HL, SP\n"
                            + "\n"
                            + "r\t4\n"
                            + "X\t7\n"
                            + "(hl)\t7\n"
                            + "(ix+X)\t19\n"
                            + "(iy+X)\t19\n"
                            + "hl,rr\t15\n";
                    case "SCF":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:scf\n"
                            + "Set carry flag instruction.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "scf\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "scf\n"
                            + "\n"
                            + "Effects:\n"
                            + "Carry flag set, H and N cleared, rest are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "4 t-states\n";
                    case "SET":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:set\n"
                            + "Sets the specified bit.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "set n,op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "set n,a\n"
                            + "set n,b\n"
                            + "set n,c\n"
                            + "set n,d\n"
                            + "set n,e\n"
                            + "set n,h\n"
                            + "set n,l\n"
                            + "set n,(hl)\n"
                            + "set n,(ix+n)\n"
                            + "set n,(iy+n)\n"
                            + "\n"
                            + "Effects:\n"
                            + "All flags are preserved.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t8\n"
                            + "(hl)\t15\n"
                            + "(ix+X)\t23\n"
                            + "(iy+X)\t23\n";
                    case "SLA":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:sla\n"
                            + "Arithmetic shift left 1 bit, bit 7 goes to carry flag, bit 0 set to 0.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "sla op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "sla A\n"
                            + "sla B\n"
                            + "sla C\n"
                            + "sla D\n"
                            + "sla E\n"
                            + "sla H\n"
                            + "sla L\n"
                            + "sla (HL)\n"
                            + "sla (IX+n)\n"
                            + "sla (IY+n)\n"
                            + "\n"
                            + "Effects:\n"
                            + "S and Z by definition, H and N reset, C from bit 7, P/V set if result is even.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t8\n"
                            + "(hl)\t15\n"
                            + "(ix+X)\t23\n"
                            + "(iy+X)\t23\n";
                    case "SLL":
                    case "SL":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:sll\n"
                            + "Arithmetic shift left 1 bit, bit 7 goes to carry flag, bit 0 set to 1.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "sll op8\n"
                            + "sl1 op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "sll A\n"
                            + "sll B\n"
                            + "sll C\n"
                            + "sll D\n"
                            + "sll E\n"
                            + "sll H\n"
                            + "sll L\n"
                            + "sll (HL)\n"
                            + "sll (IX+n)\n"
                            + "sll (IY+n)\n"
                            + "sl1 A\n"
                            + "sl1 B\n"
                            + "sl1 C\n"
                            + "sl1 D\n"
                            + "sl1 E\n"
                            + "sl1 H\n"
                            + "sl1 L\n"
                            + "sl1 (HL)\n"
                            + "sl1 (IX+n)\n"
                            + "sl1 (IY+n)\n"
                            + "\n"
                            + "Effects:\n"
                            + "S and Z by definition, H and N reset, C from bit 7, P/V set if result is even.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register. IX and IY values assumed from SLA.\n"
                            + "\n"
                            + "r\t8\n"
                            + "(hl)\t15\n"
                            + "(ix+X)\t23?\n"
                            + "(iy+X)\t23?\n";
                    case "SRA":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:sra\n"
                            + "Arithmetic shift right 1 bit, bit 0 goes to carry flag, bit 7 remains unchanged.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "sra op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "sra A\n"
                            + "sra B\n"
                            + "sra C\n"
                            + "sra D\n"
                            + "sra E\n"
                            + "sra H\n"
                            + "sra L\n"
                            + "sra (HL)\n"
                            + "sra (IX+d)\n"
                            + "sra (IY+d)\n"
                            + "\n"
                            + "Effects:\n"
                            + "S and Z set according to definition, H and N reset, C from bit 0, P/V if parity is 0.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t8\n"
                            + "(hl)\t15\n"
                            + "(ix+X)\t23\n"
                            + "(iy+X)\t23\n";
                    case "SRL":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:srl\n"
                            + "Arithmetic shift right 1 bit, bit 0 goes to carry flag, bit 7 is set to 0.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "srl op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "srl A\n"
                            + "srl B\n"
                            + "srl C\n"
                            + "srl D\n"
                            + "srl E\n"
                            + "srl H\n"
                            + "srl L\n"
                            + "srl (HL)\n"
                            + "srl (IX+d)\n"
                            + "srl (IY+d)\n"
                            + "\n"
                            + "Effects:\n"
                            + "S, H, and N flags reset, Z if result is zero, P/V set if parity is even, C from bit 0.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t8\n"
                            + "(hl)\t15\n"
                            + "(ix+X)\t23\n"
                            + "(iy+X)\t23\n";
                    case "SUB":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:sub\n"
                            + "SUB stands for subtract but only takes one input.\n"
                            + "It subtracts the input from the accumulator and writes back to it.\n"
                            + "\n"
                            + "Syntax:\n"
                            + "sub op8\t\t;8 bit\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "sub a\n"
                            + "sub b\n"
                            + "sub c\n"
                            + "sub d\n"
                            + "sub e\n"
                            + "sub h\n"
                            + "sub l\n"
                            + "sub n\t\t;8 bit constant\n"
                            + "\n"
                            + "sub (hl)\n"
                            + "sub (ix+n)\n"
                            + "sub (iy+n)\n"
                            + "\n"
                            + "Effects:\n"
                            + "N flag set, P/V is overflow, rest modified by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t4\n"
                            + "X\t7\n"
                            + "(hl)\t7\n"
                            + "(ix+X)\t19\n"
                            + "(iy+X)\t19\n";
                    case "XOR":
                        return
                            "Source: http://z80-heaven.wikidot.com/instructions-set:xor\n"
                            + "XOR is an instruction that takes one 8-bit input and compares it with the accumulator.\n"
                            + "XOR is similar to OR, except for one thing:\n"
                            + "\tonly 1 of the 2 test bits can be set or else it will result in a zero.\n"
                            + "The final answer is stored to the accumulator.\n"
                            + "0 and 0 result: 0\n"
                            + "0 and 1 result: 1\n"
                            + "1 and 0 result: 1\n"
                            + "1 and 1 result: 0\n"
                            + "\n"
                            + "Syntax:\n"
                            + "xor op8\n"
                            + "\n"
                            + "Allowed Instructions:\n"
                            + "xor a\n"
                            + "xor b\n"
                            + "xor c\n"
                            + "xor d\n"
                            + "xor e\n"
                            + "xor h\n"
                            + "xor l\n"
                            + "xor ixh\n"
                            + "xor ixl\n"
                            + "xor iyh\n"
                            + "xor iyl\n"
                            + "xor (hl)\n"
                            + "xor (ix+n)\n"
                            + "xor (iy+n)\n"
                            + "xor n\t\t;8 bit constant\n"
                            + "\n"
                            + "Effects:\n"
                            + "C and N flags cleared. P/V is parity, and rest are modified by definition.\n"
                            + "\n"
                            + "T-States:\n"
                            + "r denotes 8-bit register.\n"
                            + "\n"
                            + "r\t4\n"
                            + "X\t7\n"
                            + "(hl)\t7\n"
                            + "(ix+X)\t19\n"
                            + "(iy+X)\t19\n";
                    default:
                        return "ToolTip Not Found";
                }
            }
        }

        private void miniTimer_LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            showAbout();
        }

        #region Save as
        private void toolStripButton6_MouseMove(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Save Document As";
        }

        private void toolStripButton6_MouseLeave(object sender, EventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "";
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "SE BASIC File (*.PRG)|*.PRG";
                sfd.FilterIndex = 2;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, synBox1.Text);
                    _path = sfd.FileName;
                }
            }
        }
        #endregion Save as

        private void synBox1_Load(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton7_Click_1(object sender, EventArgs e)
        {
            bLightTheme = !bLightTheme;

            if (!bLightTheme)
            {
                toolStripButton7.Text = "Swap to Light Theme";
            } else
            {
                toolStripButton7.Text = "Swap to Dark Theme";
            }

            highlightVisibleRange();
        }

        private void toolStripButton7_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (bLightTheme)
            {
                status.ForeColor = Color.FromArgb(255, 136, 136, 136);
                status.Text = "Swap to Dark Theme";
                toolStripButton7.ToolTipText = "Swap to Dark Theme";
            } else
            {
                status.ForeColor = Color.FromArgb(255, 136, 136, 136);
                status.Text = "Swap to Light Theme";
                toolStripButton7.ToolTipText = "Swap to Light Theme";
            }
        }

        private void toolStripButton7_MouseLeave_1(object sender, EventArgs e)
        {
            status.Text = "";
        }

        public readonly struct SyntaxError
        {
            public readonly IRecognizer Recognizer;
            public readonly IToken OffendingSymbol;
            public readonly int Line;
            public readonly int CharPositionInLine;
            public readonly string Message;
            public readonly RecognitionException Exception;

            public SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
                               int charPositionInLine, string message, RecognitionException exception)
            {
                Recognizer = recognizer;
                OffendingSymbol = offendingSymbol;
                Line = line;
                CharPositionInLine = charPositionInLine;
                Message = message;
                Exception = exception;
            }
        }

        public class SyntaxErrorListener : BaseErrorListener
        {
            public readonly List<SyntaxError> Errors = new List<SyntaxError>();
            public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
                RecognitionException e)
            {
                Errors.Add(new SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e));
            }
        }

        private void toolStripButton8_Click_1(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            ITokenStream tokens;
            IParseTree tree;

            parserErrorListener parserErrorListener = new parserErrorListener();
            lexerErrorListener lexerErrorListener = new lexerErrorListener();

            if (bBasic)
            {
                Lexer lexer = new SEBASICIVLexer(new AntlrInputStream(synBox1.Text));
                lexer.RemoveErrorListeners();
                lexer.AddErrorListener(lexerErrorListener);
                tokens = new CommonTokenStream(lexer);
                SEBASICIVParser parser = new SEBASICIVParser(tokens);
                parser.RemoveErrorListeners();
                parser.AddErrorListener(parserErrorListener);
                parser.BuildParseTree = true;

                try
                {
                    tree = parser.prog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                Lexer lexer = new asmZ80Lexer(new AntlrInputStream(synBox1.Text));
                lexer.RemoveErrorListeners();
                lexer.AddErrorListener(lexerErrorListener);
                tokens = new CommonTokenStream(lexer);
                asmZ80Parser parser = new asmZ80Parser(tokens);
                parser.RemoveErrorListeners();
                parser.AddErrorListener(parserErrorListener);
                parser.BuildParseTree = true;

                try
                {
                    tree = parser.prog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            List<string> pErrors = parserErrorListener.getSyntaxErrors();
            List<string> lErrors = lexerErrorListener.getSyntaxErrors();

            for (int i = 0; i < pErrors.Count; i++)
            {
                listBox1.Items.Add(pErrors[i]);
            }

            for (int i = 0; i < lErrors.Count; i++)
            {
                listBox1.Items.Add(lErrors[i]);
            }

            if (pErrors.Count == 0 && lErrors.Count == 0)
            {
                listBox1.Items.Add("No Lexer or Parser Errors Found!");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && listBox1.SelectedItem.ToString() != "No Lexer or Parser Errors Found!")
            {
                string selItem = listBox1.SelectedItem.ToString();
                int selLine = 0;
                int selPos = 0;

                int firstColon = selItem.IndexOf(":");
                int firstDash = selItem.IndexOf("-");
                string Line = selItem.Substring(firstColon + 2, firstDash - firstColon - 3);

                int middleColon = selItem.IndexOf(":", firstColon + 1);
                int middleDash = selItem.IndexOf("-", firstDash + 1);
                string Pos = selItem.Substring(middleColon + 2, middleDash - middleColon - 3);

                selLine = int.Parse(Line);
                selPos = int.Parse(Pos);

                synBox1.Focus();
                synBox1.Selection.Start = new Place(selPos, selLine - 1);
                synBox1.DoSelectionVisible();
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            listBox1_SelectedIndexChanged(sender, e);
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            listBox1_SelectedIndexChanged(sender, e);
        }

        private void toolStripButton8_MouseMove_1(object sender, MouseEventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "Check Syntax";
        }

        private void toolStripButton8_MouseLeave_1(object sender, EventArgs e)
        {
            status.ForeColor = Color.FromArgb(255, 136, 136, 136);
            status.Text = "";
        }
    }
}

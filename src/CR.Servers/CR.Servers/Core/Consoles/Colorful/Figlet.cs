using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CR.Servers.Core.Consoles.Colorful
{
    public class Figlet
    {
        private readonly FigletFont font;

        public Figlet()
        {
            font = FigletFont.Default;
        }

        public Figlet(FigletFont font)
        {
            if (font == null) throw new ArgumentNullException(nameof(font));

            this.font = font;
        }

        public StyledString ToAscii(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (Encoding.UTF8.GetByteCount(value) != value.Length)
                throw new ArgumentException("String contains non-ascii characters");

            var stringBuilder = new StringBuilder();

            var stringWidth = GetStringWidth(font, value);
            var characterGeometry = new char[font.Height + 1, stringWidth];
            var characterIndexGeometry = new int[font.Height + 1, stringWidth];
            var colorGeometry = new Color[font.Height + 1, stringWidth];

            for (var line = 1; line <= font.Height; line++)
            {
                var runningWidthTotal = 0;

                for (var c = 0; c < value.Length; c++)
                {
                    var character = value[c];
                    var fragment = GetCharacter(font, character, line);

                    stringBuilder.Append(fragment);
                    CalculateCharacterGeometries(fragment, c, runningWidthTotal, line, characterGeometry,
                        characterIndexGeometry);

                    runningWidthTotal += fragment.Length;
                }

                stringBuilder.AppendLine();
            }

            var styledString = new StyledString(value, stringBuilder.ToString());
            styledString.CharacterGeometry = characterGeometry;
            styledString.CharacterIndexGeometry = characterIndexGeometry;
            styledString.ColorGeometry = colorGeometry;

            return styledString;
        }

        private static void CalculateCharacterGeometries(string fragment, int characterIndex, int runningWidthTotal,
            int line, char[,] charGeometry, int[,] indexGeometry)
        {
            for (var i = runningWidthTotal; i < runningWidthTotal + fragment.Length; i++)
            {
                charGeometry[line, i] = fragment[i - runningWidthTotal];
                indexGeometry[line, i] = characterIndex;
            }
        }

        private static int GetStringWidth(FigletFont font, string value)
        {
            var charWidths = new List<int>();
            foreach (var character in value)
            {
                var charWidth = 0;
                for (var line = 1; line <= font.Height; line++)
                {
                    var figletCharacter = GetCharacter(font, character, line);

                    charWidth = figletCharacter.Length > charWidth ? figletCharacter.Length : charWidth;
                }

                charWidths.Add(charWidth);
            }

            return charWidths.Sum();
        }

        private static string GetCharacter(FigletFont font, char character, int line)
        {
            var start = font.CommentLines + (Convert.ToInt32(character) - 32) * font.Height;
            var result = font.Lines[start + line];
            var lineEnding = result[result.Length - 1];
            result = Regex.Replace(result, @"\" + lineEnding + "{1,2}$", string.Empty);

            if (font.Kerning > 0)
                result += new string(' ', font.Kerning);

            return result.Replace(font.HardBlank, " ");
        }
    }
}
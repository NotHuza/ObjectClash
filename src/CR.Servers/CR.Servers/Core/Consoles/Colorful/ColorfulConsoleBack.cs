using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace CR.Servers.Core.Consoles.Colorful
{
    /// <summary>
    ///     Wraps around the System.Console class, adding enhanced styling functionality.
    /// </summary>
    public static partial class Console
    {
        // Limitation of the Windows console window.
        private const int MAX_COLOR_CHANGES = 16;

        // Note that if you set ConsoleColor.Black to a different color, then the background of the
        // console will change as a side-effect!  The index of Black (in the ConsoleColor definition) is 0,
        // so avoid that index.
        private const int INITIAL_COLOR_CHANGE_COUNT_VALUE = 1;

        private static ColorStore colorStore;
        private static ColorManagerFactory colorManagerFactory;
        private static ColorManager colorManager;
        private static readonly Dictionary<string, COLORREF> defaultColorMap;

        private static readonly string WRITELINE_TRAILER = "\r\n";
        private static readonly string WRITE_TRAILER = "";

        private static readonly Color blackEquivalent = Color.FromArgb(0, 0, 0);
        private static readonly Color blueEquivalent = Color.FromArgb(0, 0, 255);
        private static readonly Color cyanEquivalent = Color.FromArgb(0, 255, 255);
        private static readonly Color darkBlueEquivalent = Color.FromArgb(0, 0, 128);
        private static readonly Color darkCyanEquivalent = Color.FromArgb(0, 128, 128);
        private static readonly Color darkGrayEquivalent = Color.FromArgb(128, 128, 128);
        private static readonly Color darkGreenEquivalent = Color.FromArgb(0, 128, 0);
        private static readonly Color darkMagentaEquivalent = Color.FromArgb(128, 0, 128);
        private static readonly Color darkRedEquivalent = Color.FromArgb(128, 0, 0);
        private static readonly Color darkYellowEquivalent = Color.FromArgb(128, 128, 0);
        private static readonly Color grayEquivalent = Color.FromArgb(192, 192, 192);
        private static readonly Color greenEquivalent = Color.FromArgb(0, 255, 0);
        private static readonly Color magentaEquivalent = Color.FromArgb(255, 0, 255);
        private static readonly Color redEquivalent = Color.FromArgb(255, 0, 0);
        private static readonly Color whiteEquivalent = Color.FromArgb(255, 255, 255);
        private static readonly Color yellowEquivalent = Color.FromArgb(255, 255, 0);

#if !NET40
        private static TaskQueue Queue { get; } = new TaskQueue();
#endif

        private static void MapToScreen(IEnumerable<KeyValuePair<string, Color>> styleMap, string trailer)
        {
#if !NET40
            Queue.Enqueue(() => Task.Factory.StartNew(() =>
            {
#endif
                var oldSystemColor = System.Console.ForegroundColor;
                var writeCount = 1;
                foreach (var textChunk in styleMap)
                {
                    System.Console.ForegroundColor = colorManager.GetConsoleColor(textChunk.Value);

                    if (writeCount == styleMap.Count())
                        System.Console.Write(textChunk.Key + trailer);
                    else
                        System.Console.Write(textChunk.Key);

                    writeCount++;
                }

                System.Console.ForegroundColor = oldSystemColor;
#if !NET40
            })).Wait();
#endif
        }

        private static void MapToScreen(StyledString styledString, string trailer)
        {
            var oldSystemColor = System.Console.ForegroundColor;
            var rowLength = styledString.CharacterGeometry.GetLength(0);
            var columnLength = styledString.CharacterGeometry.GetLength(1);
            for (var row = 0; row < rowLength; row++)
            for (var column = 0; column < columnLength; column++)
            {
                System.Console.ForegroundColor = colorManager.GetConsoleColor(styledString.ColorGeometry[row, column]);

                if (row == rowLength - 1 && column == columnLength - 1)
                    System.Console.Write(styledString.CharacterGeometry[row, column] + trailer);
                else if (column == columnLength - 1)
                    System.Console.Write(styledString.CharacterGeometry[row, column] + "\r\n");
                else
                    System.Console.Write(styledString.CharacterGeometry[row, column]);
            }

            System.Console.ForegroundColor = oldSystemColor;
        }

        private static void WriteInColor<T>(Action<T> action, T target, Color color)
        {
            var oldSystemColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = colorManager.GetConsoleColor(color);
            action.Invoke(target);
            System.Console.ForegroundColor = oldSystemColor;
        }

        private static void WriteChunkInColor(Action<string> action, char[] buffer, int index, int count, Color color)
        {
            var chunk = buffer.AsString().Substring(index, count);

            WriteInColor(action, chunk, color);
        }

        private static void WriteInColorAlternating<T>(Action<T> action, T target, ColorAlternator alternator)
        {
            var color = alternator.GetNextColor(target.AsString());

            var oldSystemColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = colorManager.GetConsoleColor(color);
            action.Invoke(target);
            System.Console.ForegroundColor = oldSystemColor;
        }

        private static void WriteChunkInColorAlternating(Action<string> action, char[] buffer, int index, int count,
            ColorAlternator alternator)
        {
            var chunk = buffer.AsString().Substring(index, count);

            WriteInColorAlternating(action, chunk, alternator);
        }

        private static void WriteInColorStyled<T>(string trailer, T target, StyleSheet styleSheet)
        {
            var annotator = new TextAnnotator(styleSheet);
            var annotationMap = annotator.GetAnnotationMap(target.AsString());

            MapToScreen(annotationMap, trailer);
        }

        private static void WriteAsciiInColorStyled(string trailer, StyledString target, StyleSheet styleSheet)
        {
            var annotator = new TextAnnotator(styleSheet);
            var annotationMap =
                annotator.GetAnnotationMap(target
                    .AbstractValue); // Should eventually be target.AsStyledString() everywhere...?

            PopulateColorGeometry(annotationMap, target);

            MapToScreen(target, trailer);
        }

        private static void PopulateColorGeometry(IEnumerable<KeyValuePair<string, Color>> annotationMap,
            StyledString target)
        {
            var abstractCharCount = 0;
            foreach (var fragment in annotationMap)
                for (var i = 0; i < fragment.Key.Length; i++)
                {
                    // This will run O(n^2) times...but with DP, could be O(n).
                    // Just need to keep a third array that keeps track of each abstract char's width, so you never iterate past that.
                    // This third array would be one-dimensional.

                    var rowLength = target.CharacterIndexGeometry.GetLength(0);
                    var columnLength = target.CharacterIndexGeometry.GetLength(1);
                    for (var row = 0; row < rowLength; row++)
                    for (var column = 0; column < columnLength; column++)
                        if (target.CharacterIndexGeometry[row, column] == abstractCharCount)
                            target.ColorGeometry[row, column] = fragment.Value;

                    abstractCharCount++;
                }
        }

        private static void WriteChunkInColorStyled(string trailer, char[] buffer, int index, int count,
            StyleSheet styleSheet)
        {
            var chunk = buffer.AsString().Substring(index, count);

            WriteInColorStyled(trailer, chunk, styleSheet);
        }

        private static void WriteInColor<T, U>(Action<T, U> action, T target0, U target1, Color color)
        {
            var oldSystemColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = colorManager.GetConsoleColor(color);
            action.Invoke(target0, target1);
            System.Console.ForegroundColor = oldSystemColor;
        }

        private static void WriteInColorAlternating<T, U>(Action<T, U> action, T target0, U target1,
            ColorAlternator alternator)
        {
            string formatted = string.Format(target0.ToString(), target1.Normalize());
            var color = alternator.GetNextColor(formatted);

            var oldSystemColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = colorManager.GetConsoleColor(color);
            action.Invoke(target0, target1);
            System.Console.ForegroundColor = oldSystemColor;
        }

        private static void WriteInColorStyled<T, U>(string trailer, T target0, U target1, StyleSheet styleSheet)
        {
            var annotator = new TextAnnotator(styleSheet);

            string formatted = string.Format(target0.ToString(), target1.Normalize());
            var annotationMap = annotator.GetAnnotationMap(formatted);

            MapToScreen(annotationMap, trailer);
        }

        private static void WriteInColorFormatted<T, U>(string trailer, T target0, U target1, Color styledColor,
            Color defaultColor)
        {
            var formatter = new TextFormatter(defaultColor);
            List<KeyValuePair<string, Color>> formatMap =
                formatter.GetFormatMap(target0.ToString(), target1.Normalize(), new[] {styledColor});

            MapToScreen(formatMap, trailer);
        }

        private static void WriteInColorFormatted<T>(string trailer, T target0, Formatter target1, Color defaultColor)
        {
            var formatter = new TextFormatter(defaultColor);
            var formatMap = formatter.GetFormatMap(target0.ToString(), new[] {target1.Target}, new[] {target1.Color});

            MapToScreen(formatMap, trailer);
        }

        private static void WriteInColor<T, U>(Action<T, U, U> action, T target0, U target1, U target2, Color color)
        {
            var oldSystemColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = colorManager.GetConsoleColor(color);
            action.Invoke(target0, target1, target2);
            System.Console.ForegroundColor = oldSystemColor;
        }

        private static void WriteInColorAlternating<T, U>(Action<T, U, U> action, T target0, U target1, U target2,
            ColorAlternator alternator)
        {
            var formatted = string.Format(target0.ToString(), target1, target2); // NOT FORMATTING
            var color = alternator.GetNextColor(formatted);

            var oldSystemColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = colorManager.GetConsoleColor(color);
            action.Invoke(target0, target1, target2);
            System.Console.ForegroundColor = oldSystemColor;
        }

        private static void WriteInColorStyled<T, U>(string trailer, T target0, U target1, U target2,
            StyleSheet styleSheet)
        {
            var annotator = new TextAnnotator(styleSheet);

            var formatted = string.Format(target0.ToString(), target1, target2);
            var annotationMap = annotator.GetAnnotationMap(formatted);

            MapToScreen(annotationMap, trailer);
        }

        private static void WriteInColorFormatted<T, U>(string trailer, T target0, U target1, U target2,
            Color styledColor, Color defaultColor)
        {
            var formatter = new TextFormatter(defaultColor);
            List<KeyValuePair<string, Color>> formatMap =
                formatter.GetFormatMap(target0.ToString(), new[] {target1, target2}.Normalize(), new[] {styledColor});

            MapToScreen(formatMap, trailer);
        }

        private static void WriteInColorFormatted<T>(string trailer, T target0, Formatter target1, Formatter target2,
            Color defaultColor)
        {
            var formatter = new TextFormatter(defaultColor);
            var formatMap = formatter.GetFormatMap(target0.ToString(), new[] {target1.Target, target2.Target},
                new[] {target1.Color, target2.Color});

            MapToScreen(formatMap, trailer);
        }

        private static void WriteInColor<T, U>(Action<T, U, U, U> action, T target0, U target1, U target2, U target3,
            Color color)
        {
            var oldSystemColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = colorManager.GetConsoleColor(color);
            action.Invoke(target0, target1, target2, target3);
            System.Console.ForegroundColor = oldSystemColor;
        }

        private static void WriteInColorAlternating<T, U>(Action<T, U, U, U> action, T target0, U target1, U target2,
            U target3, ColorAlternator alternator)
        {
            var formatted = string.Format(target0.ToString(), target1, target2, target3);
            var color = alternator.GetNextColor(formatted);

            var oldSystemColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = colorManager.GetConsoleColor(color);
            action.Invoke(target0, target1, target2, target3);
            System.Console.ForegroundColor = oldSystemColor;
        }

        private static void WriteInColorStyled<T, U>(string trailer, T target0, U target1, U target2, U target3,
            StyleSheet styleSheet)
        {
            var annotator = new TextAnnotator(styleSheet);

            var formatted = string.Format(target0.ToString(), target1, target2, target3);
            var annotationMap = annotator.GetAnnotationMap(formatted);

            MapToScreen(annotationMap, trailer);
        }

        private static void WriteInColorFormatted<T, U>(string trailer, T target0, U target1, U target2, U target3,
            Color styledColor, Color defaultColor)
        {
            var formatter = new TextFormatter(defaultColor);
            List<KeyValuePair<string, Color>> formatMap = formatter.GetFormatMap(target0.ToString(),
                new[] {target1, target2, target3}.Normalize(), new[] {styledColor});

            MapToScreen(formatMap, trailer);
        }

        private static void WriteInColorFormatted<T>(string trailer, T target0, Formatter target1, Formatter target2,
            Formatter target3, Color defaultColor)
        {
            var styler = new TextFormatter(defaultColor);
            var formatMap = styler.GetFormatMap(target0.ToString(),
                new[] {target1.Target, target2.Target, target3.Target},
                new[] {target1.Color, target2.Color, target3.Color});

            MapToScreen(formatMap, trailer);
        }

        private static void WriteInColorFormatted<T>(string trailer, T target0, Formatter[] targets, Color defaultColor)
        {
            var styler = new TextFormatter(defaultColor);
            var formatMap = styler.GetFormatMap(target0.ToString(),
                targets.Select(formatter => formatter.Target).ToArray(),
                targets.Select(formatter => formatter.Color).ToArray());

            MapToScreen(formatMap, trailer);
        }

        private static void DoWithGradient<T>(Action<object, Color> writeAction, IEnumerable<T> input, Color startColor,
            Color endColor, int maxColorsInGradient)
        {
            var generator = new GradientGenerator();
            var gradient = generator.GenerateGradient(input, startColor, endColor, maxColorsInGradient);

            foreach (var item in gradient)
                writeAction(item.Target, item.Color);
        }

        private static Figlet GetFiglet(FigletFont font = null)
        {
            if (font == null)
                return new Figlet();
            return new Figlet(font);
        }

        private static ColorStore GetColorStore()
        {
            var colorMap = new ConcurrentDictionary<Color, ConsoleColor>();
            var consoleColorMap = new ConcurrentDictionary<ConsoleColor, Color>();

            consoleColorMap.TryAdd(ConsoleColor.Black, blackEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.Blue, blueEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.Cyan, cyanEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.DarkBlue, darkBlueEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.DarkCyan, darkCyanEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.DarkGray, darkGrayEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.DarkGreen, darkGreenEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.DarkMagenta, darkMagentaEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.DarkRed, darkRedEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.DarkYellow, darkYellowEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.Gray, grayEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.Green, greenEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.Magenta, magentaEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.Red, redEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.White, whiteEquivalent);
            consoleColorMap.TryAdd(ConsoleColor.Yellow, yellowEquivalent);

            return new ColorStore(colorMap, consoleColorMap);
        }

        private static void ReplaceAllColorsWithDefaults(bool isInCompatibilityMode, bool isWindows)
        {
            colorStore = GetColorStore();
            colorManagerFactory = new ColorManagerFactory();
            colorManager = colorManagerFactory.GetManager(colorStore, MAX_COLOR_CHANGES,
                INITIAL_COLOR_CHANGE_COUNT_VALUE, isInCompatibilityMode);

            // There's no need to do this if in compatibility mode (or if not on Windows), as more than 16 colors won't be used, anyway.
            if (!colorManager.IsInCompatibilityMode && isWindows)
                new ColorMapper().SetBatchBufferColors(defaultColorMap);
        }
    }
}
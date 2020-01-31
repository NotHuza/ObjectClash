using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CR.Servers.Core.Consoles.Colorful
{
    /// <summary>
    ///     Exposes methods and properties used in batch styling of text.
    /// </summary>
    public sealed class TextAnnotator
    {
        private readonly Dictionary<StyleClass<TextPattern>, Styler.MatchFound> matchFoundHandlers =
            new Dictionary<StyleClass<TextPattern>, Styler.MatchFound>();
        // NOTE: I still feel that there's too much overlap between this class and the TextFormatter class.

        private readonly StyleSheet styleSheet;

        /// <summary>
        ///     Exposes methods and properties used in batch styling of text.
        /// </summary>
        /// <param name="styleSheet">The StyleSheet instance that defines the way in which text should be styled.</param>
        public TextAnnotator(StyleSheet styleSheet)
        {
            this.styleSheet = styleSheet;

            foreach (var styleClass in styleSheet.Styles)
                matchFoundHandlers.Add(styleClass, (styleClass as Styler).MatchFoundHandler);
        }

        /// <summary>
        ///     Partitions the input text into styled and unstyled pieces.
        /// </summary>
        /// <param name="input">The text to be styled.</param>
        /// <returns>Returns a map relating pieces of text to their corresponding styles.</returns>
        public List<KeyValuePair<string, Color>> GetAnnotationMap(string input)
        {
            IEnumerable<KeyValuePair<StyleClass<TextPattern>, MatchLocation>> targets = GetStyleTargets(input);

            return GenerateStyleMap(targets, input);
        }

        private List<KeyValuePair<StyleClass<TextPattern>, MatchLocation>> GetStyleTargets(string input)
        {
            var matches = new List<KeyValuePair<StyleClass<TextPattern>, MatchLocation>>();
            var locations = new List<MatchLocation>();

            foreach (var pattern in styleSheet.Styles)
            foreach (var location in pattern.Target.GetMatchLocations(input))
            {
                if (locations.Contains(location))
                {
                    var index = locations.IndexOf(location);

                    matches.RemoveAt(index);
                    locations.RemoveAt(index);
                }

                matches.Add(new KeyValuePair<StyleClass<TextPattern>, MatchLocation>(pattern, location));
                locations.Add(location);
            }

            matches = matches.OrderBy(match => match.Value).ToList();
            return matches;
        }

        private List<KeyValuePair<string, Color>> GenerateStyleMap(
            IEnumerable<KeyValuePair<StyleClass<TextPattern>, MatchLocation>> targets, string input)
        {
            var styleMap = new List<KeyValuePair<string, Color>>();

            var previousLocation = new MatchLocation(0, 0);
            var chocolateEnd = 0;
            foreach (var styledLocation in targets)
            {
                var currentLocation = styledLocation.Value;

                if (previousLocation.End > currentLocation.Beginning)
                    previousLocation = new MatchLocation(0, 0);

                var vanillaStart = previousLocation.End;
                var vanillaEnd = currentLocation.Beginning;
                var chocolateStart = vanillaEnd;
                chocolateEnd = currentLocation.End;

                var vanilla = input.Substring(vanillaStart, vanillaEnd - vanillaStart);
                var chocolate = input.Substring(chocolateStart, chocolateEnd - chocolateStart);

                chocolate = matchFoundHandlers[styledLocation.Key].Invoke(input, styledLocation.Value,
                    input.Substring(chocolateStart, chocolateEnd - chocolateStart));

                if (vanilla != "")
                    styleMap.Add(new KeyValuePair<string, Color>(vanilla, styleSheet.UnstyledColor));
                if (chocolate != "")
                    styleMap.Add(new KeyValuePair<string, Color>(chocolate, styledLocation.Key.Color));

                previousLocation = currentLocation.Prototype();
            }

            if (chocolateEnd < input.Length)
            {
                var vanilla = input.Substring(chocolateEnd, input.Length - chocolateEnd);
                styleMap.Add(new KeyValuePair<string, Color>(vanilla, styleSheet.UnstyledColor));
            }

            return styleMap;
        }
    }
}
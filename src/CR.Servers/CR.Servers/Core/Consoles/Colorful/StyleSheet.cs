using System.Collections.Generic;
using System.Drawing;

namespace CR.Servers.Core.Consoles.Colorful
{
    /// <summary>
    ///     Exposes a collection of style classifications which can be used to style text.
    /// </summary>
    public sealed class StyleSheet
    {
        /// <summary>
        ///     The color to be associated with unstyled text.
        /// </summary>
        public Color UnstyledColor;

        /// <summary>
        ///     Exposes a collection of style classifications which can be used to style text.
        /// </summary>
        /// <param name="defaultColor">The color to be associated with unstyled text.</param>
        public StyleSheet(Color defaultColor)
        {
            Styles = new List<StyleClass<TextPattern>>();
            UnstyledColor = defaultColor;
        }

        /// <summary>
        ///     The StyleSheet's collection of style classifications.
        /// </summary>
        public List<StyleClass<TextPattern>> Styles { get; }

        /// <summary>
        ///     Adds a style classification to the StyleSheet.
        /// </summary>
        /// <param name="target">The string to be styled.</param>
        /// <param name="color">The color to be applied to the target.</param>
        /// <param name="matchHandler">
        ///     A delegate instance which describes a transformation that
        ///     can be applied to the target.
        /// </param>
        public void AddStyle(string target, Color color, Styler.MatchFound matchHandler)
        {
            var styler = new Styler(target, color, matchHandler);

            Styles.Add(styler);
        }

        /// <summary>
        ///     Adds a style classification to the StyleSheet.
        /// </summary>
        /// <param name="target">The string to be styled.</param>
        /// <param name="color">The color to be applied to the target.</param>
        /// <param name="matchHandler">
        ///     A delegate instance which describes a simpler transformation that
        ///     can be applied to the target.
        /// </param>
        public void AddStyle(string target, Color color, Styler.MatchFoundLite matchHandler)
        {
            Styler.MatchFound wrapper = (s, l, m) => matchHandler.Invoke(m);
            var styler = new Styler(target, color, wrapper);

            Styles.Add(styler);
        }

        /// <summary>
        ///     Adds a style classification to the StyleSheet.
        /// </summary>
        /// <param name="target">The string to be styled.</param>
        /// <param name="color">The color to be applied to the target.</param>
        public void AddStyle(string target, Color color)
        {
            Styler.MatchFound handler = (s, l, m) => m;
            var styler = new Styler(target, color, handler);

            Styles.Add(styler);
        }
    }
}
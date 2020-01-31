using System.Drawing;

namespace CR.Servers.Core.Consoles.Colorful
{
    /// <summary>
    ///     Exposes methods and properties used for alternating over a set of colors.
    /// </summary>
    public abstract class ColorAlternator : IPrototypable<ColorAlternator>
    {
        protected int nextColorIndex = 0;

        /// <summary>
        ///     Exposes methods and properties used for alternating over a set of colors.
        /// </summary>
        protected ColorAlternator()
        {
            Colors = new Color[] { };
        }

        /// <summary>
        ///     Exposes methods and properties used for alternating over a set of colors.
        /// </summary>
        protected ColorAlternator(params Color[] colors)
        {
            Colors = colors;
        }

        /// <summary>
        ///     The set of colors over which to alternate.
        /// </summary>
        public Color[] Colors { get; set; }

        public ColorAlternator Prototype()
        {
            return PrototypeCore();
        }

        protected abstract ColorAlternator PrototypeCore();

        /// <summary>
        ///     Alternates colors based on the state of the ColorAlternator instance.
        /// </summary>
        /// <param name="input">The string to be styled.</param>
        /// <returns>The current color of the ColorAlternator.</returns>
        public abstract Color GetNextColor(string input);

        protected abstract void TryIncrementColorIndex();
    }
}
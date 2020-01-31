using System.Drawing;

namespace CR.Servers.Core.Consoles.Colorful
{
    /// <summary>
    ///     Exposes properties representing an object and its color.  This is a convenience wrapper around
    ///     the StyleClass type, so you don't have to provide the type argument each time.
    /// </summary>
    public sealed class Formatter
    {
        private readonly StyleClass<object> backingClass;

        /// <summary>
        ///     Exposes properties representing an object and its color.  This is a convenience wrapper around
        ///     the StyleClass type, so you don't have to provide the type argument each time.
        /// </summary>
        /// <param name="target">The object to be styled.</param>
        /// <param name="color">The color to be applied to the target.</param>
        public Formatter(object target, Color color)
        {
            backingClass = new StyleClass<object>(target, color);
        }

        /// <summary>
        ///     The object to be styled.
        /// </summary>
        public object Target => backingClass.Target;

        /// <summary>
        ///     The color to be applied to the target.
        /// </summary>
        public Color Color => backingClass.Color;
    }
}
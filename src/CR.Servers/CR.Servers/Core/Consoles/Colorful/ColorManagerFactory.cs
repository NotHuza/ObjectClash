using System;
using System.Collections.Concurrent;
using System.Drawing;

namespace CR.Servers.Core.Consoles.Colorful
{
    public sealed class ColorManagerFactory
    {
        public ColorManager GetManager(ColorStore colorStore, int maxColorChanges, int initialColorChangeCountValue,
            bool isInCompatibilityMode)
        {
            var colorMapper = GetColorMapperSafe(ColorManager.IsWindows());

            return new ColorManager(colorStore, colorMapper, maxColorChanges, initialColorChangeCountValue,
                isInCompatibilityMode);
        }

        public ColorManager GetManager(ConcurrentDictionary<Color, ConsoleColor> colorMap,
            ConcurrentDictionary<ConsoleColor, Color> consoleColorMap, int maxColorChanges,
            int initialColorChangeCountValue, bool isInCompatibilityMode)
        {
            var colorStore = new ColorStore(colorMap, consoleColorMap);
            var colorMapper = GetColorMapperSafe(ColorManager.IsWindows());

            return new ColorManager(colorStore, colorMapper, maxColorChanges, initialColorChangeCountValue,
                isInCompatibilityMode);
        }

        private ColorMapper GetColorMapperSafe(bool isWindows)
        {
            return isWindows ? new ColorMapper() : null;
        }
    }
}
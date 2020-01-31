using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CR.Servers.Core.Consoles.Colorful
{
    public sealed class GradientGenerator
    {
        public List<StyleClass<T>> GenerateGradient<T>(IEnumerable<T> input, Color startColor, Color endColor,
            int maxColorsInGradient)
        {
            var inputAsList = input.ToList();
            var numberOfGrades = inputAsList.Count / maxColorsInGradient;
            var numberOfGradesRemainder = inputAsList.Count % maxColorsInGradient;

            var gradients = new List<StyleClass<T>>();
            var previousColor = Color.Empty;
            var previousItem = default(T);
            Func<int, int>
                setProgressSymmetrically =
                    remainder => remainder > 1
                        ? -1
                        : 0; // An attempt to make the gradient symmetric in the event that maxColorsInGradient does not divide input.Count evenly.
            Func<int, int>
                resetProgressSymmetrically =
                    progress => progress == 0
                        ? -1
                        : 0; // An attempt to make the gradient symmetric in the event that maxColorsInGradient does not divide input.Count evenly.
            var colorChangeProgress = setProgressSymmetrically(numberOfGradesRemainder);
            var colorChangeCount = 0;

            Func<int, bool> isFirstRun = index => index == 0;
            Func<int, int, T, T, bool> shouldChangeColor =
                (index, progress, current, previous) => progress > numberOfGrades - 1 && !current.Equals(previous) ||
                                                        isFirstRun(index);
            Func<int, bool> canChangeColor = changeCount => changeCount < maxColorsInGradient;

            for (var i = 0; i < inputAsList.Count; i++)
            {
                var currentItem = inputAsList[i];
                colorChangeProgress++;

                if (shouldChangeColor(i, colorChangeProgress, currentItem, previousItem) &&
                    canChangeColor(colorChangeCount))
                {
                    previousColor = GetGradientColor(i, startColor, endColor, inputAsList.Count);
                    previousItem = currentItem;
                    colorChangeProgress = resetProgressSymmetrically(colorChangeProgress);
                    colorChangeCount++;
                }

                gradients.Add(new StyleClass<T>(currentItem, previousColor));
            }

            return gradients;
        }

        private Color GetGradientColor(int index, Color startColor, Color endColor, int numberOfGrades)
        {
            var numberOfGradesAdjusted = numberOfGrades - 1;

            var rDistance = startColor.R - endColor.R;
            var gDistance = startColor.G - endColor.G;
            var bDistance = startColor.B - endColor.B;

            var r = startColor.R + -rDistance * ((double) index / numberOfGradesAdjusted);
            var g = startColor.G + -gDistance * ((double) index / numberOfGradesAdjusted);
            var b = startColor.B + -bDistance * ((double) index / numberOfGradesAdjusted);

            var graded = Color.FromArgb((int) r, (int) g, (int) b);
            return graded;
        }
    }
}
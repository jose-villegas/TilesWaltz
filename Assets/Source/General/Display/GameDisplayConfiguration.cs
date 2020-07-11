using System;
using System.Collections.Generic;
using UnityEngine;

namespace TilesWalk.General.Display
{
    [Serializable]
    public class GameDisplayConfiguration
    {
        [SerializeField] private List<DisplayOption> _displayOptions;

        public float GetOrthogonalSize(int interop)
        {
            var ratio = (float) Screen.width / (float) Screen.height;
            ratio = (float) (Math.Round(ratio, 2));

            DisplayOption choosen = null;
            var closeness = 0f;

            foreach (var displayOption in _displayOptions)
            {
                var lowerPrecision = displayOption.Aspect;
                lowerPrecision = (float) (Math.Round(lowerPrecision, 2));

                if (choosen == null && ratio >= lowerPrecision)
                {
                    choosen = displayOption;
                    closeness = Math.Abs((lowerPrecision - ratio) / ratio);
                }

                // keep the highest matching
                if (choosen != null && ratio >= displayOption.Aspect)
                {
                    var newDifference = Math.Abs((lowerPrecision - ratio) / ratio);

                    if (newDifference < closeness)
                    {
                        choosen = displayOption;
                        closeness = newDifference;
                    }
                }
            }

            var normalized = interop / 5f;
            var result = 0f;

            if (choosen != null)
            {
                result = Mathf.LerpUnclamped(choosen.MaxOrthogonalSize - 5, choosen.MaxOrthogonalSize, normalized);
            }
            else
            {
                result = Mathf.Lerp(5f, 10f, normalized);
            }

            return result;
        }
    }
}
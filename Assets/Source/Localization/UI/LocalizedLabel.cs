using Mgl;
using TilesWalk.General.Patterns;
using TMPro;
using UnityEngine;

namespace TilesWalk.Localization.UI
{
    /// <summary>
    /// This component handles localization for text UI elements
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
    {
        /// <summary>
        /// Assign localized value
        /// </summary>
        private void Start()
        {
            if (Component.text == "Tap to continue...")
            {
                Debug.Log("Here");
            }

            Component.text = I18n.Instance.__(Component.text);

            if (Component.text == "Tap to continue...")
            {
                Debug.Log("Here");
            }
        }
    }
}

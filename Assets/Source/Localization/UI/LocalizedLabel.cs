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
        private I18n i18n = I18n.Instance;

        /// <summary>
        /// Assign localized value
        /// </summary>
        private void Start()
        {
            Component.text = i18n.__(Component.text);
        }
    }
}

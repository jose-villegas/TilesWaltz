using Mgl;
using UnityEngine;

namespace TilesWalk.Localization
{
    public class ConfigureLocale : MonoBehaviour
    {

        private void Awake()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    I18n.SetLocale("en-US");
                    break;
                case SystemLanguage.Spanish:
                    I18n.SetLocale("es-ES");
                    break;
                default:
                    I18n.SetLocale("en-US");
                    break;
            }

            I18n.SetLocale("es-ES");
        }
    }
}

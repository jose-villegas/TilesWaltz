using UniRx;
using UnityEngine;

namespace TilesWalk.Gameplay
{
    public class GameplaySetting
    {
        public BoolReactiveProperty ShowGuides { get; set; }
        public BoolReactiveProperty Music { get; set; }
        public BoolReactiveProperty Sounds { get; set; }

        public void SavePreferences()
        {
            PlayerPrefs.SetString("Settings.ShowGuides", ShowGuides.Value.ToString());
            PlayerPrefs.SetString("Settings.Music", Music.Value.ToString());
            PlayerPrefs.SetString("Settings.Sounds", Sounds.Value.ToString());
            PlayerPrefs.Save();
        }

        public void LoadPreferences()
        {
            ShowGuides = new BoolReactiveProperty(true);
            Music = new BoolReactiveProperty(true);
            Sounds = new BoolReactiveProperty(true);

            ShowGuides.Value = bool.Parse(PlayerPrefs.GetString("Settings.ShowGuides", "True"));
            Music.Value = bool.Parse(PlayerPrefs.GetString("Settings.Music", "True"));
            Sounds.Value = bool.Parse(PlayerPrefs.GetString("Settings.Sounds", "True"));
        }
    }
}
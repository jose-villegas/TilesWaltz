using UnityEngine;

namespace TilesWalk.Gameplay
{
    public class GameplaySetting
    {
        public bool ShowGuides { get; set; } = true;
        public bool Music { get; set; } = true;

        public bool Sounds { get; set; } = true;

        public void SavePreferences()
        {
            PlayerPrefs.SetString("Settings.ShowGuides", ShowGuides.ToString());
            PlayerPrefs.SetString("Settings.Music", Music.ToString());
            PlayerPrefs.SetString("Settings.Sounds", Sounds.ToString());
            PlayerPrefs.Save();
        }

        public void LoadPreferences()
        {
            ShowGuides = bool.Parse(PlayerPrefs.GetString("Settings.ShowGuides", "True"));
            Music = bool.Parse(PlayerPrefs.GetString("Settings.Music", "True"));
            Sounds = bool.Parse(PlayerPrefs.GetString("Settings.Sounds", "True"));
        }
    }
}
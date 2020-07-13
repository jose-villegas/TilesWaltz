using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Installer
{
    public class GameplaySettingInstaller : MonoInstaller
    {
        [SerializeField] private GameplaySetting _setting = new GameplaySetting();

        public override void InstallBindings()
        {
            _setting.LoadPreferences();
            Container.Bind<GameplaySetting>().FromInstance(_setting).AsSingle();
        }

		private void OnApplicationPause(bool _)
        {
            _setting.SavePreferences();
        }

        private void OnApplicationQuit()
        {
            _setting.SavePreferences();
        }
	}
}
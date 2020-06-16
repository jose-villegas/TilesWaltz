using TilesWalk.Building.LevelEditor.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.LevelEditor.Installer
{
	public class CustomLevelPlayerInstaller : MonoInstaller
	{
		[SerializeField] private CustomLevelPlayer _customLevelPlayer;

		public override void InstallBindings()
		{
			Container.Bind<CustomLevelPlayer>().FromInstance(_customLevelPlayer).AsSingle();
		}
	}
}
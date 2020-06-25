using TilesWalk.Building.LevelEditor.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.LevelEditor.Installer
{
	public class SaveLevelCanvasInstaller : MonoInstaller
	{
		[SerializeField] private SaveLevelCanvas _saveLevelCanvas;

		public override void InstallBindings()
		{
			Container.Bind<SaveLevelCanvas>().FromInstance(_saveLevelCanvas).AsSingle();
		}
	}
}
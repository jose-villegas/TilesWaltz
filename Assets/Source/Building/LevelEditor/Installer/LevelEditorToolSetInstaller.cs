using TilesWalk.Building.LevelEditor.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.LevelEditor.Installer
{
	public class LevelEditorToolSetInstaller : MonoInstaller
	{
		[SerializeField] private LevelEditorToolSet _levelEditorToolSet;

		public override void InstallBindings()
		{
			Container.Bind<LevelEditorToolSet>().FromInstance(_levelEditorToolSet).AsSingle();
		}
	}
}
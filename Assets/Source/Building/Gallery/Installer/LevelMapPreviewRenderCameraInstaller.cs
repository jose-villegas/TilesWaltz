using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Gallery.Installer
{
	public class LevelMapPreviewRenderCameraInstaller : MonoInstaller
	{
		[SerializeField] private LevelMapPreviewRenderCamera _previewCamera;

		public override void InstallBindings()
		{
			Container.Bind<LevelMapPreviewRenderCamera>().FromInstance(_previewCamera).AsSingle();
		}
	}
}
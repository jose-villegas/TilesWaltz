using UnityEngine;
using Zenject;

namespace TilesWalk.Audio.Installer
{
	[CreateAssetMenu(fileName = "GameAudioCollectionInstaller", menuName = "Installers/GameAudioCollectionInstaller")]
	public class GameAudioCollectionInstaller : ScriptableObjectInstaller<GameAudioCollectionInstaller>
	{
		[SerializeField] private GameAudioCollection _audioCollection;

		public override void InstallBindings()
		{
			Container.Bind<GameAudioCollection>().FromInstance(_audioCollection).AsSingle();
		}
	}
}
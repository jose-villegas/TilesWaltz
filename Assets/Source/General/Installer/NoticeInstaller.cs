using TilesWalk.General.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.General.Installer
{
	public class NoticeInstaller : MonoInstaller
	{
		[SerializeField] private Notice _notice;

		public override void InstallBindings()
		{
			Container.Bind<Notice>().FromInstance(_notice).AsSingle();
		}
	}
}

using TilesWalk.General.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.General.Installer
{
	public class ConfirmationInstaller : MonoInstaller
	{
		[SerializeField] private Confirmation _confirmation;

		public override void InstallBindings()
		{
			Container.Bind<Confirmation>().FromInstance(_confirmation).AsSingle();
		}
	}
}
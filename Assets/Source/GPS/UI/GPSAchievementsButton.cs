using GooglePlayGames;
using TilesWalk.General.Patterns;
using TilesWalk.General.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.GPS.UI
{
	[RequireComponent(typeof(Button))]
	public class GPSAchievementsButton : ObligatoryComponentBehaviour<Button>
	{
		[Inject] private Notice _notice;

		private void Start()
		{
			Component.onClick.AsObservable().Subscribe(ShowAchievements).AddTo(this);
		}

		private void ShowAchievements(Unit u)
		{
			if (PlayGamesPlatform.Instance.localUser.authenticated)
			{
				PlayGamesPlatform.Instance.ShowAchievementsUI();
			}
			else
			{
				_notice.Configure("Cannot show Achievements, not logged in").Show(2f);
			}
		}
	}
}
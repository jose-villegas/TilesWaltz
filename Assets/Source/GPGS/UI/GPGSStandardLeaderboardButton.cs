using GooglePlayGames;
using TilesWalk.General.Patterns;
using TilesWalk.General.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.GPGS.UI
{
	[RequireComponent(typeof(Button))]
	public class GPGSStandardLeaderboardButton : ObligatoryComponentBehaviour<Button>
	{
		[Inject] private Notice _notice;

		private void Start()
		{
			Component.onClick.AsObservable().Subscribe(ShowLeaderboard).AddTo(this);
		}

		private void ShowLeaderboard(Unit u)
		{
			if (PlayGamesPlatform.Instance.localUser.authenticated)
			{
				PlayGamesPlatform.Instance.ShowLeaderboardUI();
			}
			else
			{
				_notice.Configure("Cannot show leaderboard, not logged in").Show(2f);
			}
		}
	}
}
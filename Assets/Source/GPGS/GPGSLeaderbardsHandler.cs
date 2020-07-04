using GooglePlayGames;
using UnityEngine;

namespace TilesWalk.GPGS
{
	public class GPGSLeaderbardsHandler : MonoBehaviour
	{
		public void ReportGameLevelsScore(int totalScore)
		{
			// Submit leaderboard scores, if authenticated
			if (!PlayGamesPlatform.Instance.localUser.authenticated) return;

			PlayGamesPlatform.Instance.ReportScore(totalScore,
				GPGSIds.leaderboard_levels_score,
				(success) => { Debug.Log("(leaderboard_levels_score) update: " + success); });
		}
	}
}
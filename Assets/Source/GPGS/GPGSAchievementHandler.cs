using GooglePlayGames;
using UnityEngine;

namespace TilesWalk.GPGS
{
	public class GPGSAchievementHandler : MonoBehaviour
	{
		public void GameLevelCompletionAchievements(int levelsCompleted, bool show = true)
		{
			// no achievements for users that aren't logged in
			if (!PlayGamesPlatform.Instance.localUser.authenticated) return;

			if (levelsCompleted == 1)
			{
				PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement_high_score,
					b => { Debug.Log("(achievement_high_score) unlock: " + b); });
			}

			if (levelsCompleted == 5)
			{
				PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement_tile_apprentice,
					b => { Debug.Log("(achievement_tile_apprentice) unlock: " + b); });
			}

			if (levelsCompleted == 10)
			{
				PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement_tile_learner,
					b => { Debug.Log("(achievement_tile_learner) unlock: " + b); });
			}

			if (levelsCompleted == 20)
			{
				PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement_tile_juggler,
					b => { Debug.Log("(achievement_tile_juggler) unlock: " + b); });
			}
		}
	}
}
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace TilesWalk.GPGS
{
	public class GPGSInitializer : MonoBehaviour
	{
		private void Start()
		{
			// Create client configuration with saved games enabled
			PlayGamesClientConfiguration config = new
					PlayGamesClientConfiguration.Builder()
				.EnableSavedGames()
				.Build();

			// Enable debugging output (recommended)
			PlayGamesPlatform.DebugLogEnabled = true;

			// Initialize and activate the platform
			PlayGamesPlatform.InitializeInstance(config);
			PlayGamesPlatform.Activate();
		}
	}
}
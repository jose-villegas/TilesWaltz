using System.Collections.Generic;
using BayatGames.SaveGameFree;
using Zenject;

namespace TilesWalk.Gameplay.Score.Installer
{
	public class LevelScoreRecordsInstaller : MonoInstaller
	{
		private Dictionary<string, LevelScore> _scoreRecords = new Dictionary<string, LevelScore>();

		public override void InstallBindings()
		{
			if (SaveGame.Exists("Scores"))
			{
				_scoreRecords = SaveGame.Load("Scores", _scoreRecords);
			}
			else
			{
				_scoreRecords = new Dictionary<string, LevelScore>();
			}

			Container.Bind<Dictionary<string, LevelScore>>().FromInstance(_scoreRecords).AsSingle();
		}

		private void OnDestroy()
		{
			SaveGame.Save("Scores", _scoreRecords);
		}
	}
}
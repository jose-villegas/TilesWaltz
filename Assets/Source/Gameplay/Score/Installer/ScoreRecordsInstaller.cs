using System.Collections.Generic;
using BayatGames.SaveGameFree;
using Zenject;

namespace TilesWalk.Gameplay.Score.Installer
{
	public class ScoreRecordsInstaller : MonoInstaller
	{
		private Dictionary<string, Score> _scoreRecords = new Dictionary<string, Score>();

		public override void InstallBindings()
		{
			_scoreRecords = SaveGame.Load("Scores", _scoreRecords);
			Container.Bind<Dictionary<string, Score>>().FromInstance(_scoreRecords).AsSingle();
		}

		private void OnDestroy()
		{
			SaveGame.Save("Scores", _scoreRecords);
		}
	}
}
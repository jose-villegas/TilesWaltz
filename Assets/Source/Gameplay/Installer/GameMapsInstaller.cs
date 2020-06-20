using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Newtonsoft.Json;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Persistence;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Installer
{
	[CreateAssetMenu(fileName = "GameMapsInstaller", menuName = "Installers/GameMapsInstaller")]
	public class GameMapsInstaller : ScriptableObjectInstaller<GameMapsInstaller>
	{
		[Header("Insert Setup")] [SerializeField]
		private string _name;

		[SerializeField, TextArea] private string _instructions;
		[SerializeField] private FinishCondition _condition;

		[SerializeField, ShowIf("IsTimeCondition"), Min(1)]
		private float _seconds;

		[SerializeField, ShowIf("IsMovesCondition"), Min(1)]
		private int _moves;

		[SerializeField, Range(0, 5)] private int _mapSize;

		[Header("Entries")] [SerializeField] private GameMapCollection _gameMaps = new GameMapCollection();

		[Header("Game Levels Map")] [SerializeField]
		private GameLevelsMap _gameLevelsMap;

		private bool IsTimeCondition => _condition == FinishCondition.TimeLimit;

		private bool IsMovesCondition => _condition == FinishCondition.MovesLimit;

		public GameLevelsMap LevelsMap
		{
			get => _gameLevelsMap;
			set => _gameLevelsMap = value;
		}

		public override void InstallBindings()
		{
			Container.Bind<GameMapCollection>().WithId("GameMaps").FromInstance(_gameMaps);
		}

		[Button]
		public void Insert()
		{
			var map = JsonConvert.DeserializeObject<LevelMap>(_instructions);
			map.Id = _name;
			map.MapSize = _mapSize;
			map.FinishCondition = _condition;

			switch (_condition)
			{
				case FinishCondition.TimeLimit:
					var tCond = new TimeFinishCondition(map.Id, _seconds);
					_gameMaps.Insert(map, tCond);
					break;
				case FinishCondition.MovesLimit:
					var mCond = new MovesFinishCondition(map.Id, _moves);
					_gameMaps.Insert(map, mCond);
					break;
			}

		}
	}
}
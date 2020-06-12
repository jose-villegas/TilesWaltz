using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Newtonsoft.Json;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
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

		[Header("Entries")] [SerializeField] private List<LevelMap> _availableMaps;
		[SerializeField] private List<MovesFinishCondition> _movesFinishConditions;
		[SerializeField] private List<TimeFinishCondition> _timeFinishConditions;

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
			Container.Bind<List<LevelMap>>().FromInstance(_availableMaps).AsSingle();
			Container.Bind<List<MovesFinishCondition>>().FromInstance(_movesFinishConditions).AsSingle();
			Container.Bind<List<TimeFinishCondition>>().FromInstance(_timeFinishConditions).AsSingle();
		}

		[Button]
		public void Insert()
		{
			var map = JsonConvert.DeserializeObject<LevelMap>(_instructions);
			map.Id = _name;
			map.MapSize = _mapSize;
			map.FinishCondition = _condition;
			_availableMaps.Add(map);

			switch (_condition)
			{
				case FinishCondition.TimeLimit:
					var tCond = new TimeFinishCondition(map.Id, _seconds);
					_timeFinishConditions.Add(tCond);
					break;
				case FinishCondition.MovesLimit:
					var mCond = new MovesFinishCondition(map.Id, _moves);
					_movesFinishConditions.Add(mCond);
					break;
			}
		}
	}
}
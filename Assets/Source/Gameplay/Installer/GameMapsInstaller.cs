using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Newtonsoft.Json;
using TilesWalk.Building.Map;
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

		[Header("Entries")] [SerializeField] private List<TileMap> _availableMaps;
		[SerializeField] private List<MovesFinishCondition> _movesFinishConditions;
		[SerializeField] private List<TimeFinishCondition> _timeFinishConditions;

		public bool IsTimeCondition =>
			_condition == FinishCondition.TimeLimit || _condition == FinishCondition.TimeAndMoveLimit;

		public bool IsMovesCondition =>
			_condition == FinishCondition.MovesLimit || _condition == FinishCondition.TimeAndMoveLimit;

		public override void InstallBindings()
		{
			Container.Bind<List<TileMap>>().FromInstance(_availableMaps).AsSingle();
			Container.Bind<List<MovesFinishCondition>>().FromInstance(_movesFinishConditions).AsSingle();
			Container.Bind<List<TimeFinishCondition>>().FromInstance(_timeFinishConditions).AsSingle();
		}

		[Button]
		public void Insert()
		{
			var map = JsonConvert.DeserializeObject<TileMap>(_instructions);
			map.Id = _name;
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
				case FinishCondition.TimeAndMoveLimit:
					var tCon = new TimeFinishCondition(map.Id, _seconds);
					var mCon = new MovesFinishCondition(map.Id, _moves);
					_timeFinishConditions.Add(tCon);
					_movesFinishConditions.Add(mCon);
					break;
			}
		}
	}
}
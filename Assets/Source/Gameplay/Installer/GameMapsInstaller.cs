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
		[ShowIf("IsTimeCondition"), Min(1)] public float _seconds;
		[ShowIf("IsMovesCondition"), Min(1)] public int _moves;

		[Header("Entries")] [SerializeField] private List<TileMap> _availableMaps;
		[SerializeField] private List<MovesFinishCondition> _movesFinishConditions;
		[SerializeField] private List<TimeFinishCondition> _timeFinishConditions;

		public bool IsTimeCondition => _condition == FinishCondition.TimeLimit;
		public bool IsMovesCondition => _condition == FinishCondition.MovesLimit;

		public override void InstallBindings()
		{
			Container.Bind<List<TileMap>>().FromInstance(_availableMaps).AsSingle();
		}

		[Button()]
		public void Insert()
		{
			var map = JsonConvert.DeserializeObject<TileMap>(_instructions);

			switch (_condition)
			{
				case FinishCondition.TimeLimit:
					_movesFinishConditions.Add(new MovesFinishCondition(map.Id, _moves));
					break;
				case FinishCondition.MovesLimit:
					_timeFinishConditions.Add(new TimeFinishCondition(map.Id, TimeSpan.FromSeconds(_seconds)));
					break;
			}
		}
	}
}
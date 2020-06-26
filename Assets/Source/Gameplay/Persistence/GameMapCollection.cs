﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using UniRx;
using UnityEngine;

namespace TilesWalk.Gameplay.Persistence
{
	[Serializable]
	public class GameMapCollection
	{
		[JsonProperty] [SerializeField] private List<LevelMap> _availableMaps;
		[JsonProperty] [SerializeField] private List<MovesFinishCondition> _movesFinishConditions;
		[JsonProperty] [SerializeField] private List<TimeFinishCondition> _timeFinishConditions;

		[JsonIgnore] public List<LevelMap> AvailableMaps => _availableMaps;
		[JsonIgnore] public List<MovesFinishCondition> MovesFinishConditions => _movesFinishConditions;
		[JsonIgnore] public List<TimeFinishCondition> TimeFinishConditions => _timeFinishConditions;

		private Subject<LevelMap> _onNewLevelInsert;
		private Subject<LevelMap> _onLevelRemoved;

		public GameMapCollection(List<LevelMap> maps, List<MovesFinishCondition> moves, List<TimeFinishCondition> times)
		{
			_availableMaps = new List<LevelMap>(_availableMaps);
			_movesFinishConditions = new List<MovesFinishCondition>(moves);
			_timeFinishConditions = new List<TimeFinishCondition>(times);
		}

		public GameMapCollection()
		{
		}

		~GameMapCollection()
		{
			_onNewLevelInsert?.OnCompleted();
			_onLevelRemoved?.OnCompleted();
		}

		public IObservable<LevelMap> OnNewLevelInsertAsObservable()
		{
			return _onNewLevelInsert == null ? _onNewLevelInsert = new Subject<LevelMap>() : _onNewLevelInsert;
		}

		public IObservable<LevelMap> OnLevelRemovedAsObservable()
		{
			return _onLevelRemoved == null ? _onLevelRemoved = new Subject<LevelMap>() : _onLevelRemoved;
		}

		public bool Exist(string id)
		{
			if (AvailableMaps == null || AvailableMaps.Count == 0) return false;

			return AvailableMaps.Exists(x => x.Id == id);
		}

		public bool Remove(string id)
		{
			var indexOf = _availableMaps.FindIndex(x => x.Id == id);

			if (indexOf >= 0)
			{
				var map = _availableMaps[indexOf];
				var condition = map.FinishCondition;
				_availableMaps.RemoveAt(indexOf);

				switch (condition)
				{
					case FinishCondition.TimeLimit:
						indexOf = _timeFinishConditions.FindIndex(x => x.Id == id);

						if (indexOf >= 0)
						{
							_timeFinishConditions.RemoveAt(indexOf);
						}

						break;
					case FinishCondition.MovesLimit:
						indexOf = _movesFinishConditions.FindIndex(x => x.Id == id);

						if (indexOf >= 0)
						{
							_movesFinishConditions.RemoveAt(indexOf);
						}

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				_onLevelRemoved?.OnNext(map);
				return true;
			}

			return false;
		}

		public void Insert<T>(LevelMap map, T condition) where T : MapFinishCondition
		{
			if (AvailableMaps == null) _availableMaps = new List<LevelMap>();
			if (MovesFinishConditions == null) _movesFinishConditions = new List<MovesFinishCondition>();
			if (TimeFinishConditions == null) _timeFinishConditions = new List<TimeFinishCondition>();

			var indexOf = _availableMaps.FindIndex(x => x.Id == map.Id);

			// this means the level is already save so we are going to replace it instead
			if (indexOf >= 0)
			{
				_availableMaps[indexOf] = map;
			}
			else
			{
				_availableMaps.Add(map);
			}

			// look for condition
			if (map.FinishCondition == FinishCondition.MovesLimit)
			{
				indexOf = _movesFinishConditions.FindIndex(x => x.Id == map.Id);

				if (indexOf >= 0)
				{
					_movesFinishConditions[indexOf] = condition as MovesFinishCondition;
				}
				else
				{
					// check if condition exist on other structure
					indexOf = _timeFinishConditions.FindIndex(x => x.Id == map.Id);

					if (indexOf >= 0) _timeFinishConditions.RemoveAt(indexOf);

					_movesFinishConditions.Add(condition as MovesFinishCondition);
				}
			}

			if (map.FinishCondition == FinishCondition.TimeLimit)
			{
				indexOf = _timeFinishConditions.FindIndex(x => x.Id == map.Id);

				if (indexOf >= 0)
				{
					_timeFinishConditions[indexOf] = condition as TimeFinishCondition;
				}
				else
				{
					// check if condition exist on other structure
					indexOf = _movesFinishConditions.FindIndex(x => x.Id == map.Id);

					if (indexOf >= 0) _movesFinishConditions.RemoveAt(indexOf);

					_timeFinishConditions.Add(condition as TimeFinishCondition);
				}
			}

			_onNewLevelInsert?.OnNext(map);
		}
	}
}
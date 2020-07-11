using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using UniRx;
using UnityEngine;

namespace TilesWalk.Gameplay.Persistence
{
	/// <summary>
	/// This serves as a collection of <see cref="LevelMap"/> and <see cref="MapFinishCondition"/>
	/// </summary>
	[Serializable]
	public class GameMapCollection
	{
		[JsonProperty] [SerializeField] private List<LevelMap> _availableMaps;
		[JsonProperty] [SerializeField] private List<MovesFinishCondition> _movesFinishConditions;
		[JsonProperty] [SerializeField] private List<TimeFinishCondition> _timeFinishConditions;

		/// <summary>
		/// The maps within this collection
		/// </summary>
		[JsonIgnore] public List<LevelMap> AvailableMaps => _availableMaps;
		/// <summary>
		/// The finishing conditions for the <see cref="AvailableMaps"/> that have a
		/// <see cref="FinishCondition.MovesLimit"/> condition
		/// </summary>
		[JsonIgnore] public List<MovesFinishCondition> MovesFinishConditions => _movesFinishConditions;
		/// <summary>
		/// The finishing conditions for the <see cref="AvailableMaps"/> that have a
		/// <see cref="FinishCondition.TimeLimit"/> condition
		/// </summary>
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

		/// <summary>
		/// Triggered when a new level is inserted in the collection
		/// </summary>
		/// <returns></returns>
		public IObservable<LevelMap> OnNewLevelInsertAsObservable()
		{
			return _onNewLevelInsert == null ? _onNewLevelInsert = new Subject<LevelMap>() : _onNewLevelInsert;
		}

		/// <summary>
		/// Triggered when a level is removed from the collection
		/// </summary>
		/// <returns></returns>
		public IObservable<LevelMap> OnLevelRemovedAsObservable()
		{
			return _onLevelRemoved == null ? _onLevelRemoved = new Subject<LevelMap>() : _onLevelRemoved;
		}

		/// <summary>
		/// Checks if a level with the given id exists within the <see cref="AvailableMaps"/> structure
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool Exist(string id)
		{
			if (AvailableMaps == null || AvailableMaps.Count == 0) return false;

			return AvailableMaps.Exists(x => x.Id == id);
		}

		/// <summary>
		/// Removes a level from <see cref="AvailableMaps"/>, the method will also remove
		/// its matching <see cref="MapFinishCondition"/> stored within <see cref="MovesFinishConditions"/>
		/// or <see cref="TimeFinishConditions"/>
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Inserts a new map to the <see cref="AvailableMaps"/> and adds its matching
		/// <see cref="MapFinishCondition"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="map"></param>
		/// <param name="condition"></param>
		public void Insert<T>(LevelMap map, T condition) where T : MapFinishCondition
		{
			if (AvailableMaps == null) _availableMaps = new List<LevelMap>();
			if (MovesFinishConditions == null) _movesFinishConditions = new List<MovesFinishCondition>();
			if (TimeFinishConditions == null) _timeFinishConditions = new List<TimeFinishCondition>();

			// remove references to null
			PruneNullValues();

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

		private void PruneNullValues()
		{
			// prune null values
			_movesFinishConditions.RemoveAll(x => x == null);
			_timeFinishConditions.RemoveAll(x => x == null);
			_availableMaps.RemoveAll(x => x == null);
		}
	}
}
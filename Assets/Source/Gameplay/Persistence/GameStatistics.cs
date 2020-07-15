using System;
using Boo.Lang;
using Newtonsoft.Json;
using TilesWalk.Map.General;
using UnityEngine;

namespace TilesWalk.Gameplay.Persistence
{
    /// <summary>
    /// Statistics data from the user
    /// </summary>
    [Serializable]
    public class GameStatistics
    {
        [Header("Launch Info")] [JsonProperty] [SerializeField]
        private bool _isFirstTimeLaunch = true;

        [JsonProperty] [SerializeField] private DateTime _firstLaunchTimestamp;
        [JsonProperty] [SerializeField] private DateTime _lastSaveTimestamp;

        [Header("Levels Statistics - Game")] [JsonProperty] [SerializeField]
        private int _gameLevelsPlayed;

        [JsonProperty] [SerializeField] private long _gameLevelsFailed;
        [JsonProperty] [SerializeField] private long _gameLevelsCompletion;

        [Header("Levels Statistics - User")] [JsonProperty] [SerializeField]
        private int _userLevelsPlayed;

        [JsonProperty] [SerializeField] private long _userLevelsFailed;
        [JsonProperty] [SerializeField] private long _userLevelsCompletion;

        [Header("Levels Statistics - Imported")] [JsonProperty] [SerializeField]
        private int _importedLevelsPlayed;

        [JsonProperty] [SerializeField] private long _importedLevelsFailed;
        [JsonProperty] [SerializeField] private long _importedLevelsCompletion;

        [Header("Gameplay")] [JsonProperty] [SerializeField]
        private long _pointsCollected;

        [JsonProperty] [SerializeField] private long _combosDone;
        [JsonProperty] [SerializeField] private long _powerUpsUsed;
        [JsonProperty] [SerializeField] private long _pointsFromCombos;
        [JsonProperty] [SerializeField] private long _pointsFromPowerUps;

        [Header("Session")] [JsonProperty] [SerializeField]
        private long _numberOfSessions;

        [JsonProperty] [SerializeField] private TimeSpan _sessionAverage;
        [JsonProperty] [SerializeField] private DateTime _lastSessionBegin;
        [JsonProperty] [SerializeField] private DateTime _lastSessionEnd;

        [JsonProperty] [SerializeField] private List<string> _tutorialSequencesCompleted;

        public DateTime LastSaveTimestamp => _lastSaveTimestamp;

        public bool IsFirstTimeLaunch => _isFirstTimeLaunch;

        public void CompletedTutorial(string sequenceId)
        {
            if (_tutorialSequencesCompleted == null) _tutorialSequencesCompleted = new List<string>();

            _tutorialSequencesCompleted.Add(sequenceId);
        }

        public bool IsTutorialCompleted(string sequenceId)
        {
            return _tutorialSequencesCompleted != null && _tutorialSequencesCompleted.Contains(sequenceId);
        }

        public void TimestampSave()
        {
            _lastSaveTimestamp = DateTime.Now;
        }

        public void GameMapCompleted()
        {
            _gameLevelsCompletion += 1;
        }

        public void MapFailed(Provider source)
        {
            switch (source)
            {
                case Provider.UserMaps:
                    UserMapFailed();
                    break;
                case Provider.ImportedMaps:
                    ImportedMapFailed();
                    break;
                case Provider.GameMaps:
                    GameMapFailed();
                    break;
            }
        }

        public void MapCompleted(Provider source)
        {
            switch (source)
            {
                case Provider.UserMaps:
                    UserMapCompleted();
                    break;
                case Provider.ImportedMaps:
                    ImportedMapCompleted();
                    break;
                case Provider.GameMaps:
                    GameMapCompleted();
                    break;
            }
        }

        public void GameMapFailed()
        {
            _gameLevelsFailed += 1;
        }

        public void UserMapCompleted()
        {
            _userLevelsCompletion += 1;
        }

        public void UserMapFailed()
        {
            _userLevelsFailed += 1;
        }

        public void ImportedMapCompleted()
        {
            _importedLevelsCompletion += 1;
        }

        public void ImportedMapFailed()
        {
            _importedLevelsFailed += 1;
        }

        public void Points(int points)
        {
            _pointsCollected += points;
        }

        public void PointsFromCombo(int points)
        {
            _pointsFromCombos += points;
        }

        public void PointsFromPowerUp(int points)
        {
            _pointsFromPowerUps += points;
        }

        public void ComboDone()
        {
            _combosDone += 1;
        }

        public void PowerUpUsed()
        {
            _powerUpsUsed += 1;
        }

        private void IsFirstLaunch()
        {
            _isFirstTimeLaunch = true;
            _firstLaunchTimestamp = DateTime.Now;
        }

        public void BeginSession()
        {
            if (_numberOfSessions >= 1)
            {
                _isFirstTimeLaunch = false;
                var lastSession = (_lastSessionEnd - _lastSessionBegin).TotalSeconds;
                var normalize = _sessionAverage.TotalSeconds - _sessionAverage.TotalSeconds / _numberOfSessions;
                var newAverage = normalize + lastSession / _numberOfSessions;
                // rolling average approximation
                _sessionAverage = TimeSpan.FromSeconds(newAverage);
            }

            if (_numberOfSessions == 0)
            {
                IsFirstLaunch();
                _sessionAverage = TimeSpan.FromSeconds(0);
            }

            _numberOfSessions += 1;
            _lastSessionBegin = DateTime.Now;
        }

        public void EndSession()
        {
            _lastSessionEnd = DateTime.Now;
        }
    }
}
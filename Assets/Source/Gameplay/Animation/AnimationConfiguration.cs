using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Animation
{
    [Serializable]
    public class AnimationConfiguration
    {
        [Header("Tiles")] [SerializeField] private AnimationCurve _shuffleCurve;
        [SerializeField] [Min(0f)] private float _shuffleMoveTime;
        [SerializeField] private AnimationCurve _scalePopInCurve;
        [SerializeField] [Min(0f)] private float _scalePopInTime;
        [SerializeField] private float _powerUpPopInSequenceFactor;

        [Header("Level")] [SerializeField] private float _levelMapBuildingTime = 1.5f;

        [Header("Map Camera")] [SerializeField, Min(0f)]
        private float _targetTileTime;

        [Header("Level Editor")] [SerializeField, Min(0f)]
        private float _gridAnimationTime = 0.35f;

        [SerializeField] private float _pathGuideAnimationTime;

        [Header("Tutorial")] [SerializeField] private float _wordsPerSecond;
        [SerializeField] private float characterMovementTime;

        public float ShuffleMoveTime => _shuffleMoveTime;

        public float ScalePopInTime => _scalePopInTime;

        public float TargetTileTime => _targetTileTime;

        public AnimationCurve ShuffleCurve => _shuffleCurve;

        public AnimationCurve ScalePopInCurve => _scalePopInCurve;

        public float PowerUpPopInSequenceFactor => _powerUpPopInSequenceFactor;

        public float GridAnimationTime => _gridAnimationTime;
        public float PathGuideAnimationTime => _pathGuideAnimationTime;

        public float WordsPerSecond => _wordsPerSecond;

        public float CharacterMovementTime => characterMovementTime;

        public float LevelMapBuildingTime => _levelMapBuildingTime;
    }
}
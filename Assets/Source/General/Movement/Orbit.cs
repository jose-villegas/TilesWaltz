using UnityEditor;
using UnityEngine;

namespace TilesWalk.General.Movement
{
	public class Orbit : MonoBehaviour
	{
		[SerializeField] private Transform _center;
		[SerializeField] private float _radius;
		[SerializeField] private float _loopTime;

		private void Start()
		{
			transform.position = transform.right * _radius + _center.position;
		}

		private void Update()
		{
			transform.RotateAround(_center.position, transform.up, Time.deltaTime * _loopTime * 360f);
			transform.position = transform.right * _radius + _center.position;
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TilesWalk.General.FX
{
	public class ParticleSystemsCollector : MonoBehaviour
	{
		private Dictionary<string, ParticleSystem> _particleFx = new Dictionary<string, ParticleSystem>();

		public ParticleSystem this[string name] => ParticleFX[name];

		public Dictionary<string, ParticleSystem> ParticleFX
		{
			get => _particleFx;
		}

		public void StopAll()
		{
			_particleFx.Values.ToList().ForEach(ps => ps.Stop());
		}

		private void Awake()
		{
			if (_particleFx == null) _particleFx = new Dictionary<string, ParticleSystem>();

			var particles = GetComponentsInChildren<ParticleSystem>(true);

			foreach (var particle in particles)
			{
				_particleFx[particle.name] = particle;
			}
		}
	}
}
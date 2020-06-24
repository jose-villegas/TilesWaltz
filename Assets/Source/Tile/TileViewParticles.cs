using System.Collections.Generic;
using UnityEngine;

namespace TilesWalk.Tile
{
	public partial class TileView
	{
		private Dictionary<string, ParticleSystem> _particleFx;

		private void FetchParticleSystems()
		{
			_particleFx = new Dictionary<string, ParticleSystem>();
			var particles = GetComponentsInChildren<ParticleSystem>();
			
			foreach (var particle in particles)
			{
				_particleFx[particle.name] = particle;
			}
		}
	}
}
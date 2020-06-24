using UnityEngine;

namespace TilesWalk.FX
{
	public class ScrollingTexture : MonoBehaviour
	{
		[SerializeField] private float horizontalScrollSpeed = 0.25f;
		[SerializeField] private float verticalScrollSpeed = 0.25f;

		private bool _scroll = true;
		private Renderer _renderer;

		public void FixedUpdate()
		{
			if (_scroll)
			{
				if (_renderer == null) _renderer = GetComponent<Renderer>();

				float verticalOffset = Time.time * verticalScrollSpeed;
				float horizontalOffset = Time.time * horizontalScrollSpeed;
				_renderer.material.mainTextureOffset = new Vector2(horizontalOffset, verticalOffset);
			}
		}

		public void OnEnable()
		{
			_scroll = true;
		}

		public void OnDisable()
		{
			_scroll = false;
		}
	}
}
using UnityEngine;

namespace TilesWalk.Navigation.Map
{
	public class LevelNameRequestHandler : MonoBehaviour
	{
		[SerializeField] private string _levelName;

		public string LevelName
		{
			get => _levelName;
			set => _levelName = value;
		}

		private void Start()
		{
			var children = transform.GetComponentsInChildren<ILevelNameRequire>();

			foreach (var child in children)
			{
				child.LevelName.Value = _levelName;
			}
		}
	}
}

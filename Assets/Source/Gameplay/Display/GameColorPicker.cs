using NaughtyAttributes;
using TilesWalk.Gameplay.Installer;
using TilesWalk.General.Patterns;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Gameplay.Display
{
	[ExecuteInEditMode, RequireComponent(typeof(Graphic))]
	public class GameColorPicker : ObligatoryComponentBehaviour<Graphic>
	{
		[Inject(Optional = true)] private GameTileColorsConfiguration _colorsConfiguration;

		[SerializeField] private GameColor _name;
		[SerializeField, ReadOnly] private Color _color;

		private void OnValidate()
		{
			// solve this through Resource Load
			if (_colorsConfiguration == null)
			{
				_colorsConfiguration = Resources.Load<GameSettingsInstaller>("GameSettingsInstaller").GamePalette;
			}

			_color = _colorsConfiguration[_name];
			Component.color = _color;
		}
	}
}
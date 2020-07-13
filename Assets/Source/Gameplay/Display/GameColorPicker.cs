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
		[Inject] private GameTileColorsConfiguration _colorsConfiguration;

		[SerializeField] private GameColor _name;
		[SerializeField, ReadOnly] private Color _color;

        public GameColor Name
        {
            get => _name;
            set => _name = value;
        }

        private void OnValidate()
		{
			// solve this through Resource Load
			if (_colorsConfiguration == null)
			{
				_colorsConfiguration = Resources.Load<GameSettingsInstaller>("GameSettingsInstaller").GamePalette;
			}

			_color = _colorsConfiguration[Name];
			Component.color = _color;
		}
	}
}
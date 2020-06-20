using TilesWalk.Map.Scaffolding;
using UnityEngine;

namespace TilesWalk.Building.LevelEditor.UI.Gallery
{
    public class CustomLevelEntryCanvas : MonoBehaviour
    {
	    [SerializeField] private LevelNameRequestHandler _levelRequest;

	    public LevelNameRequestHandler LevelRequest => _levelRequest;
    }
}
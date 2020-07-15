using System.Collections.Generic;
using UnityEngine;

namespace TilesWalk.General.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIElementIdentifier : MonoBehaviour
    {
        [SerializeField] private int _identifier;

        public static Dictionary<int, UIElementIdentifier> Registered;

        public int Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        private void Awake()
        {
            // register element
            if (Registered == null) Registered = new Dictionary<int, UIElementIdentifier>();

            if (!Registered.TryGetValue(_identifier, out var identifier))
            {
                Registered.Add(_identifier, this);
            }
        }

        private void OnValidate()
        {
            if (_identifier == 0) _identifier = GetHashCode();
        }
    }
}
using System;
using TilesWalk.General.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Gameplay.Tutorial.UI
{
    /// <summary>
    /// Handles tutorial UI movements
    /// </summary>
    public class TutorialCanvas : CanvasGroupBehaviour
    {
        [Inject] private TutorialSequenceHandler _handler;

        [SerializeField] private Image _background;
        /// <summary>
        /// The dialog box for the tutorial
        /// </summary>
        [Header("Dialog Box"), SerializeField] private RectTransform _dialogBox;

        /// <summary>
        /// The actual dialog text
        /// </summary>
        [SerializeField] private TextMeshProUGUI _dialogContent;

        /// <summary>
        /// The little pointing sprite in the dialog box indicating where
        /// the text is coming from
        /// </summary>
        [SerializeField] private RectTransform _dialogTail;

        /// <summary>
        /// The area in which we are able to move the dialog box around.
        /// </summary>
        [SerializeField] private RectTransform _dialogArea;

        /// <summary>
        /// The area in which we are able to move the dialog tail around.
        /// </summary>
        [SerializeField] private RectTransform _tailArea;

        private float _tailHeight;
        private float _dialogHeight;

        /// <summary>
        /// The actual dialog text
        /// </summary>
        public TextMeshProUGUI DialogContent => _dialogContent;

        public Image Background => _background;

        private void Awake()
        {
            _tailHeight = _dialogTail.localPosition.y;
            _dialogHeight = _dialogBox.localPosition.y;
            Hide();
        }

        private void Start()
        {
            _handler.TileCharacter.transform.parent.ObserveEveryValueChanged(x => x.localPosition).Subscribe(OnCharacterMoved)
                .AddTo(this);
        }

        public void OnCharacterMoved(Vector3 position)
        {
            // first move the dialog box
            _dialogBox.localPosition += -_dialogBox.localPosition.x * Vector3.right + Vector3.right * position.x;
            _dialogBox.localPosition +=
                -_dialogBox.localPosition.y * Vector3.up + Vector3.up * (position.y + _dialogHeight);
            ClampToArea(_dialogArea, _dialogBox);
            // then move the dialog tail
            _dialogTail.localPosition += -_dialogTail.localPosition.x * Vector3.right + Vector3.right * position.x;
            _dialogTail.localPosition +=
                -_dialogTail.localPosition.y * Vector3.up + Vector3.up * (position.y + _tailHeight);
            ClampToArea(_tailArea, _dialogTail);
        }

        private void ClampToArea(RectTransform area, RectTransform element)
        {
            Vector3 pos = element.localPosition;

            Vector3[] corners = new Vector3[4];
            area.GetLocalCorners(corners);

            Vector3 minPosition = Vector2.right * corners[0].x + Vector2.up * corners[0].y - element.rect.min;
            Vector3 maxPosition = Vector2.right * corners[2].x + Vector2.up * corners[2].y - element.rect.max;

            pos.x = Mathf.Clamp(element.localPosition.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(element.localPosition.y, minPosition.y, maxPosition.y);

            element.localPosition = pos;
        }
    }
}
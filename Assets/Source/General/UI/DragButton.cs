using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TilesWalk.General.UI
{
	public class DragButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[SerializeField] private RectTransform _dragContainer;

		private Vector2 halfSize;
		private IEnumerator _moveToPosCoroutine;

		void Start()
		{
			halfSize = _dragContainer.sizeDelta * 0.5f * _dragContainer.root.localScale.x;
		}

		public void OnBeginDrag(PointerEventData data)
		{
			// If a smooth movement animation is in progress, cancel it
			if (_moveToPosCoroutine != null)
			{
				StopCoroutine(_moveToPosCoroutine);
				_moveToPosCoroutine = null;
			}
		}

		public void OnDrag(PointerEventData data)
		{
			_dragContainer.position = data.position;
		}

		public void OnEndDrag(PointerEventData data)
		{
			var screenWidth = Screen.width;
			var screenHeight = Screen.height;

			Vector3 pos = _dragContainer.position;

			// Find distances to all four edges
			var distToLeft = pos.x;
			var distToRight = Mathf.Abs(pos.x - screenWidth);

			var distToBottom = Mathf.Abs(pos.y);
			var distToTop = Mathf.Abs(pos.y - screenHeight);

			var horDistance = Mathf.Min(distToLeft, distToRight);
			var vertDistance = Mathf.Min(distToBottom, distToTop);

			// Find the nearest edge's coordinates
			if (horDistance < vertDistance)
			{
				if (distToLeft < distToRight)
				{
					pos = new Vector3(halfSize.x, pos.y, 0f);
				}
				else
				{
					pos = new Vector3(screenWidth - halfSize.x, pos.y, 0f);
				}

				pos.y = Mathf.Clamp(pos.y, halfSize.y, screenHeight - halfSize.y);
			}
			else
			{
				if (distToBottom < distToTop)
				{
					pos = new Vector3(pos.x, halfSize.y, 0f);
				}
				else
				{
					pos = new Vector3(pos.x, screenHeight - halfSize.y, 0f);
				}

				pos.x = Mathf.Clamp(pos.x, halfSize.x, screenWidth - halfSize.x);
			}

			// If another smooth movement animation is in progress, cancel it
			if (_moveToPosCoroutine != null)
			{
				StopCoroutine(_moveToPosCoroutine);
			}

			// Smoothly translate the popup to the specified position
			_moveToPosCoroutine = MoveToPosAnimation(pos);
			StartCoroutine(_moveToPosCoroutine);
		}

		private IEnumerator MoveToPosAnimation(Vector3 targetPos)
		{
			var modifier = 0f;
			var initialPos = _dragContainer.position;

			while (modifier < 1f)
			{
				modifier += 4f * Time.unscaledDeltaTime;
				_dragContainer.position = Vector3.Lerp(initialPos, targetPos, modifier);

				yield return null;
			}
		}
	}
}

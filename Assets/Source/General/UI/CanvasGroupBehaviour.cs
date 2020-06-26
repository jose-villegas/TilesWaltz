using System;
using TilesWalk.General.Patterns;
using TilesWalk.Navigation.UI;
using UniRx;
using UnityEngine;

namespace TilesWalk.General.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasGroupBehaviour : ObligatoryComponentBehaviour<CanvasGroup>
	{
		public bool IsVisible => Component.alpha > 0;

		private Subject<Unit> _onHide;
		private Subject<Unit> _onShow;

		public virtual void Hide()
		{
			Component.alpha = 0;
			Component.interactable = false;
			Component.blocksRaycasts = false;
			_onHide?.OnNext(new Unit());
		}

		public virtual void Show()
		{
			Component.alpha = 1;
			Component.interactable = true;
			Component.blocksRaycasts = true;
			_onShow?.OnNext(new Unit());
		}

		public IObservable<Unit> OnHideAsObservable()
		{
			return _onHide = _onHide ?? (_onHide = new Subject<Unit>());
		}

		public IObservable<Unit> OnShowAsObservable()
		{
			return _onShow = _onShow ?? (_onShow = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onHide?.OnCompleted();
			_onShow?.OnCompleted();
		}
	}
}
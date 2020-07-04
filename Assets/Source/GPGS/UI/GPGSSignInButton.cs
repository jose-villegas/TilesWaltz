using GooglePlayGames;
using TilesWalk.General.Patterns;
using TilesWalk.General.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.GPGS.UI
{
	[RequireComponent(typeof(Button))]
	public class GPGSSignInButton : ObligatoryComponentBehaviour<Button>
	{
		[Inject] private Confirmation _confirmation;


		[SerializeField] private TextMeshProUGUI _signOutLabel;
		[SerializeField] private TextMeshProUGUI _signInLabel;
		[SerializeField] private Image _signOutSprite;
		[SerializeField] private Image _signInSprite;

		private void Start()
		{
			// Try silent sign-in (second parameter is isSilent)
			PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);

			Component.onClick.AsObservable().Subscribe(SignIn).AddTo(this);
			RefreshButton(PlayGamesPlatform.Instance.localUser.authenticated);
		}

		private void RefreshButton(bool auth)
		{
			_signOutLabel.gameObject.SetActive(auth);
			_signOutSprite.gameObject.SetActive(auth);
			_signInLabel.gameObject.SetActive(!auth);
			_signInSprite.gameObject.SetActive(!auth);
		}

		private void SignIn(Unit u)
		{
			if (!PlayGamesPlatform.Instance.localUser.authenticated)
			{
				// Sign in with Play Game Services, showing the consent dialog
				// by setting the second parameter to isSilent=false.
				PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
			}
			else
			{
				_confirmation.Configure("Cloud saving won't be available if you log out, continue?", () =>
				{
					// Sign out of play games
					PlayGamesPlatform.Instance.SignOut();
					RefreshButton(false);
				}).Show();
			}
		}

		private void SignInCallback(bool signed)
		{
			RefreshButton(signed);
		}
	}
}
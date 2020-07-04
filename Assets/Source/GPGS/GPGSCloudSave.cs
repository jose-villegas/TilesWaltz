using System;
using System.Collections;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace TilesWalk.GPGS
{
	public class GPGSCloudSave
	{
		private bool ProcessCloudData(byte[] cloudData, out string processedData)
		{
			processedData = string.Empty;

			if (cloudData == null)
			{
				Debug.Log("No Data saved to the cloud");
				return false;
			}

			processedData = Encoding.UTF8.GetString(cloudData);
			return true;
		}

		public void SaveToCloud(string fileName, string data, Action onComplete = null, Action onFailure = null)
		{
			if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;

			PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution
			(
				fileName,
				DataSource.ReadCacheOrNetwork,
				ConflictResolutionStrategy.UseLongestPlaytime,
				((status, metadata) =>
				{
					if (status == SavedGameRequestStatus.Success)
					{
						byte[] raw = Encoding.UTF8.GetBytes(data);

						SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
						SavedGameMetadataUpdate updatedMetadata = builder.Build();

						PlayGamesPlatform.Instance.SavedGame.CommitUpdate(metadata, updatedMetadata, raw,
							(requestStatus, gameMetadata) =>
							{
								if (status != SavedGameRequestStatus.Success)
								{
									Debug.LogWarning("Error Saving" + status);
									onFailure?.Invoke();
								}
								else
								{
									onComplete?.Invoke();
								}
							});
					}
					else
					{
						Debug.LogWarning("Error opening Saved Game" + status);
						onFailure?.Invoke();
					}
				})
			);
		}

		public void LoadFromCloud(string fileName, Action<string> result, Action onFailure = null)
		{
			if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;

			PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution
			(
				fileName,
				DataSource.ReadCacheOrNetwork,
				ConflictResolutionStrategy.UseLongestPlaytime,
				((status, metadata) =>
				{
					if (status == SavedGameRequestStatus.Success)
					{
						PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(metadata, (requestStatus, bytes) =>
						{
							if (status != SavedGameRequestStatus.Success)
							{
								Debug.LogWarning("Error Saving" + status);
								onFailure?.Invoke();
							}
							else
							{
								ProcessCloudData(bytes, out var resultProcessedData);
								result?.Invoke(resultProcessedData);
							}
						});
					}
					else
					{
						Debug.LogWarning("Error opening Saved Game" + status);
						onFailure?.Invoke();
					}
				})
			);
		}
	}
}
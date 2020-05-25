using System;
using System.ComponentModel;
using TilesWalk.BaseInterfaces;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;

namespace TilesWalk.General
{
	[Serializable]
	public class SceneModel : IModel, INotifyPropertyChanged
	{
		[SerializeField] protected Matrix4x4 _model;

		/// <summary>
		/// This is used to notify the model-view to be updated in the game frame, 
		/// its helpful to avoid checking on <see cref="Update"/>
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		public Vector3 Position
		{
			get => _model.GetColumn(3);
			set
			{
				_model.SetColumn(3, value);
				// notify others
				NotifyChange(this, new PropertyChangedEventArgs("Position"));
			}
		}

		public Quaternion Rotation
		{
			get => Quaternion.LookRotation(Forward, Up);
			set
			{
				var rotation = Quaternion.LookRotation(Forward, Up);
				var diff = rotation * Quaternion.Inverse(value);
				_model *= Matrix4x4.Rotate(diff);
				// notify others
				NotifyChange(this, new PropertyChangedEventArgs("Rotation"));
			}
		}

		public Vector3 Scale
		{
			get
			{
				var scale = new Vector3(_model.GetColumn(0).magnitude,
					_model.GetColumn(1).magnitude,
					_model.GetColumn(2).magnitude);
				return scale;
			}
		}

		public Vector3 Forward => _model.GetColumn(2);
		public Vector3 Up => _model.GetColumn(1);
		public Vector3 Right => _model.GetColumn(0);

		public Matrix4x4 Model
		{
			get => _model;
			set => _model = value;
		}

		protected void NotifyChange(object sender, PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(sender, e);
		}

		public SceneModel()
		{
			_model = Matrix4x4.identity;
		}
	}
}
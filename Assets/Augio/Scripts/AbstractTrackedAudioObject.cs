using System;
using Vuforia;
using UnityEngine;

namespace Augio
{
	public abstract class AbstractTrackedAudioObject : MonoBehaviour
	{
		public TrackedObjectFramework TargetFramework;

		public string TrackKey;

		public delegate void TrackableStatusChangedEventHandler (TrackableStatus newStatus);

		public event TrackableStatusChangedEventHandler TrackableStatusChanged;

		protected virtual void OnTrackableStatusChanged(TrackableStatus newStatus)
		{
			if (TrackableStatusChanged != null)
				TrackableStatusChanged(newStatus);
		}

		public delegate void RotationChangedEventHandler (Quaternion currentRotation, Quaternion lastRotation);

		public event RotationChangedEventHandler RotationChanged;

		protected virtual void OnRotationChanged(Quaternion currentRotation, Quaternion lastRotation)
		{
			if (RotationChanged != null)
				RotationChanged(currentRotation, lastRotation);
		}

		public abstract void Init ();


	}
}

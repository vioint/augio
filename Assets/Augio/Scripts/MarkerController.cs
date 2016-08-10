using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Augio
{
	public class MarkerController : MonoBehaviour
	{

		public AudioModifierKind audioModCtrlKind = AudioModifierKind.Volume;
		private AudioModifierController audioModCtrl = null;

		public MarkerOptionsMenu optionsMenu = null;
		public MarkerLevelUI levelUI = null;


		public AbstractTrackedAudioObject trackedObject = null;
		public float levelSensivity = 2;
		public float volumeLevelMinimumValue = 0;
		public float volumeLevelMaximumValue = 1;
		public int bpmLevelMinimumValue = 1;
		public int bpmLevelMaximumValue = 200;
		public float fxLevelMinimumValue = 0;
		public float fxLevelMaximumValue = 1;
		public bool isDebugging = false;

		private float _lastLevel = float.MinValue;
		public float currentLevel = 0;

		void Start ()
		{
			if (trackedObject == null) {
				Debug.LogErrorFormat ("{0}: This object is missing a tracked object reference, please assign one.", transform.root.name);
			} else {
				trackedObject.RotationChanged += TrackedObject_RotationChanged;
				trackedObject.TrackableStatusChanged += TrackedObject_OnTrackableStatusChanged;
				if (!string.IsNullOrEmpty(trackedObject.TrackKey))
					Debug.LogFormat ("{0}: Registered with tracked object {1}", transform.root.name, trackedObject.TrackKey);
			}

			if (audioModCtrl == null) {
				audioModCtrl = GetComponentInChildren<AudioModifierController> () ?? gameObject.AddComponent<AudioModifierController> ();
			}
			if (audioModCtrl) {
				Debug.LogFormat ("{0}: Registered audio modifier {1} of kind {2} with tracked object {3}", 
					transform.root.name, 
					audioModCtrl.name,
					audioModCtrlKind,
					trackedObject.TrackKey);
			} else {
				Debug.LogErrorFormat ("{0}: Cannot register audio modifier of kind {2} with tracked object {3}", 
					transform.root.name, 
					audioModCtrlKind,
					trackedObject.TrackKey);
			}


			if (!optionsMenu) {
				Debug.LogWarningFormat ("{0}: An options menu was not assigned", transform.root.name);
			} else {
				optionsMenu.RegisterController (this);
			}

			if (!levelUI) {
				Debug.LogWarningFormat ("{0}: A level UI was not assigned", transform.root.name);
			}
		}


		private void OnTrackedObjectFound ()
		{
			Debug.Log ("Trackable " + trackedObject.TrackKey + " found");

			Renderer[] rendererComponents = GetComponentsInChildren<Renderer> (true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider> (true);
			Canvas[] canvasComponents = GetComponentsInChildren<Canvas> (true);

			// Enable rendering:
			foreach (Renderer component in rendererComponents)
				component.enabled = true;

			// Enable colliders:
			foreach (Collider component in colliderComponents)
				component.enabled = true;

			// Enable canvas objects
			foreach (Canvas component in canvasComponents)
				component.enabled = true;

			VisualSequencerController.Instance.ActivateTrack (trackedObject.TrackKey);
		}

		private void OnTrackedObjectLost ()
		{
			Debug.Log ("Trackable " + trackedObject.TrackKey + " lost");

			Renderer[] rendererComponents = GetComponentsInChildren<Renderer> (true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider> (true);
			Canvas[] canvasComponents = GetComponentsInChildren<Canvas> (true);

			// Disable rendering:
			foreach (Renderer component in rendererComponents)
				component.enabled = false;

			// Disable colliders:
			foreach (Collider component in colliderComponents)
				component.enabled = false;

			// Disable canvas objects
			foreach (Canvas component in canvasComponents)
				component.enabled = false;

			VisualSequencerController.Instance.DeactivateTrack (trackedObject.TrackKey);
		}

		private void TrackedObject_OnTrackableStatusChanged (TrackableStatus newStatus)
		{
			if (newStatus == TrackableStatus.Detected)
				OnTrackedObjectFound ();
			else
				OnTrackedObjectLost ();
		}

		void TrackedObject_RotationChanged (Quaternion currentRotation, Quaternion lastRotation)
		{
			string currentLevelText = "";
			float normalizedLevel = 0f;
			float currentAngle = Mathf.LerpAngle (
				                     (float)Math.Round (currentRotation.y, 2), 
				                     (float)Math.Round (lastRotation.y, 2), 
				                     Time.deltaTime);

			switch (audioModCtrlKind) {
			case AudioModifierKind.BPM:
//			Debug.LogFormat("RotY:{0},RotAxis:{1},RotAngle:{2}", currentRotation.y, rotAxis, rotAngle);
				currentLevel = Mathf.Clamp (
					((float)Math.Round (currentAngle, 2))
					.Remap (0, 1, bpmLevelMinimumValue, bpmLevelMaximumValue),
					bpmLevelMinimumValue, bpmLevelMaximumValue);
				normalizedLevel = currentLevel.Remap (bpmLevelMinimumValue, bpmLevelMaximumValue, 0, 1);
				currentLevelText = currentLevel.ToString ();
				break;

			case AudioModifierKind.FXDryToWetRatio:
				currentLevel = Mathf.Clamp (
					(float)Math.Round (currentAngle, 2) * levelSensivity, 
					fxLevelMinimumValue, 
					fxLevelMaximumValue);
				normalizedLevel = currentLevel.Remap (fxLevelMinimumValue, fxLevelMaximumValue, 0, 1);
				currentLevelText = currentLevel.ToString ("P0");
				break;

			case AudioModifierKind.Volume:
			default:
				currentLevel = Mathf.Clamp ((float)Math.Round (currentAngle, 2) * levelSensivity, 
					volumeLevelMinimumValue, volumeLevelMaximumValue);
				normalizedLevel = currentLevel.Remap (volumeLevelMinimumValue, volumeLevelMaximumValue, 0, 1);
				currentLevelText = currentLevel.ToString ("P0");
				break;
			}

			if (isDebugging)
				Debug.Log ("Updated level to " + currentLevelText);
		
			if (currentLevel != _lastLevel) {
				_lastLevel = currentLevel;

				levelUI.UpdateLevel (currentLevel, normalizedLevel, currentLevelText);
				levelUI.UpdateOrientationText (currentRotation);

				UpdateAudioModifier (currentLevel);
			}
		}

		public void UpdateAudioModifier (object level)
		{
			audioModCtrl.ModifyAudio (trackedObject.TrackKey, audioModCtrlKind, level);
		}


		void OnDestroy()
		{
			if (trackedObject != null) {
				trackedObject.RotationChanged -= TrackedObject_RotationChanged;
				trackedObject.TrackableStatusChanged -= TrackedObject_OnTrackableStatusChanged;
			}
		}
	}
}
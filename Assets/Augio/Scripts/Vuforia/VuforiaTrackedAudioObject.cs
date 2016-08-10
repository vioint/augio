using UnityEngine;
using System.Collections;
using Vuforia;
using TrackableBehaviourStatus = Vuforia.TrackableBehaviour.Status;
using Augio;

public class VuforiaTrackedAudioObject : AbstractTrackedAudioObject, ITrackableEventHandler
{
	// Vuforia specific tracker behaviour
	private TrackableBehaviour TrackableBehaviour;

	private TrackableBehaviourStatus currentTrackerStatus = TrackableBehaviourStatus.UNDEFINED;

	private Quaternion _lastRotation = Quaternion.identity;
	private Quaternion _currentRotation = Quaternion.identity;

	void Awake () {
		Init();
	}

	public override void Init() {
		// use sequencer only after it's ready
		if (!VisualSequencerController.Instance.IsReady)
			VisualSequencerController.Instance.OnReady += RegisterObject;
		else
			RegisterObject();
	}

	private void RegisterObject() {
		_lastRotation = transform.root.rotation;
		TrackableBehaviour = GetComponent<TrackableBehaviour>();

		if (!TrackableBehaviour) {
			Debug.LogErrorFormat("{0}.{1}: Trackable behaviour is missing", transform.root.name, GetType().Name);
		} else {
			TrackableBehaviour.RegisterTrackableEventHandler(this);
			Debug.LogFormat("{0}.{1}: Registered trackable with key '{2}'", transform.root.name, GetType().Name, TrackKey);
		}

	}
	
	void Update () {
		if (currentTrackerStatus == TrackableBehaviourStatus.DETECTED ||
			currentTrackerStatus == TrackableBehaviourStatus.TRACKED ||
			currentTrackerStatus == TrackableBehaviourStatus.EXTENDED_TRACKED) {
			_currentRotation = transform.rotation;
			if (_lastRotation != _currentRotation) {
				OnRotationChanged(_currentRotation, _lastRotation);
				_lastRotation = _currentRotation;
			}
		}
	}

	void OnDestroy(){
		if (TrackableBehaviour)
		{
			TrackableBehaviour.UnregisterTrackableEventHandler(this);
			Debug.LogFormat("{0}: Unregistered trackable with key '" + TrackKey + "'", transform.root.name);
		}
	}

	#region ITrackableEventHandler implementation

	public void OnTrackableStateChanged (TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			currentTrackerStatus = newStatus;
			OnTrackableStatusChanged(TrackableStatus.Detected);
		}
		else
		{
			currentTrackerStatus = newStatus;
			OnTrackableStatusChanged(TrackableStatus.NotFound);
		}
	}

	#endregion






}

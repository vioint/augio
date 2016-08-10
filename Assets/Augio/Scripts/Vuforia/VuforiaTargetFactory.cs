using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vuforia;
using Augio;
using System;

public class VuforiaTargetFactory : AbstractVisualTargetFactory, IUserDefinedTargetEventHandler
{
	#region PUBLIC_MEMBERS

	/// <summary>
	/// Must be set in the Unity inspector to reference a ImageTargetBehaviour 
	/// that is instanciated for augmentations of new user defined targets.
	/// </summary>
	public ImageTargetBehaviour ImageTargetTemplate;

	// Currently observed frame quality
	public ImageTargetBuilder.FrameQuality frameQuality = ImageTargetBuilder.FrameQuality.FRAME_QUALITY_NONE;

	public bool isExtendedTrackingEnabled = false;

	#endregion PUBLIC_MEMBERS


	#region PRIVATE_MEMBERS

	private const int MAX_TARGETS = 5;
	private UserDefinedTargetBuildingBehaviour mTargetBuildingBehaviour;
	private ObjectTracker mObjectTracker;

	// DataSet that newly defined targets are added to
	private DataSet mBuiltDataSet;

	private string lastTargetName = "NewUDT";


	#endregion //PRIVATE_MEMBERS


	#region MONOBEHAVIOUR_METHODS

	public void Start ()
	{
		mTargetBuildingBehaviour = gameObject.GetComponent<UserDefinedTargetBuildingBehaviour> () ?? gameObject.AddComponent<UserDefinedTargetBuildingBehaviour> ();
		if (mTargetBuildingBehaviour) {
			mTargetBuildingBehaviour.RegisterEventHandler (this);
			Debug.Log ("Registering User Defined Target event handler.");
		}

		// scanning must be started in order for the camera quality settings to be updated
		//		var udtBuilder = this.gameObject.GetComponent<Vuforia.UserDefinedTargetBuildingBehaviour>();
		//		udtBuilder.StartScanningAutomatically = true;
		//		udtBuilder.StopTrackerWhileScanning = false;
		//		udtBuilder.StopScanningWhenFinshedBuilding = true;
		//		Debug.Log("*** UDT builder setup ***");
	}

	#endregion //MONOBEHAVIOUR_METHODS

	#region IUserDefinedTargetEventHandler implementation

	/// <summary>
	/// Called when UserDefinedTargetBuildingBehaviour has been initialized successfully
	/// </summary>
	public void OnInitialized ()
	{
		mObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker> ();
		if (mObjectTracker != null) {
			// Create a new dataset
			mBuiltDataSet = mObjectTracker.CreateDataSet ();
			mObjectTracker.ActivateDataSet (mBuiltDataSet);
			Debug.Log ("Active Vuforia DB is " + mBuiltDataSet);
		}
	}

	/// <summary>
	/// Updates the current frame quality
	/// </summary>
	public void OnFrameQualityChanged (ImageTargetBuilder.FrameQuality currentFrameQuality)
	{
		if (frameQuality != currentFrameQuality) {
			frameQuality = currentFrameQuality;
			Debug.LogFormat ("{0}: Current camera image quality updated to {1}", GetType ().Name, frameQuality);
		}
	}

	/// <summary>
	/// Takes a new trackable source and adds it to the dataset
	/// This gets called automatically as soon as you 'BuildNewTarget with UserDefinedTargetBuildingBehaviour
	/// </summary>
	public void OnNewTrackableSource (TrackableSource trackableSource)
	{
		// Deactivates the dataset first
		mObjectTracker.DeactivateDataSet (mBuiltDataSet);

		// Destroy the oldest target if the dataset is full or the dataset 
		// already contains five user-defined targets.
		if (mBuiltDataSet.HasReachedTrackableLimit () || mBuiltDataSet.GetTrackables ().Count () >= MAX_TARGETS) {
			IEnumerable<Trackable> trackables = mBuiltDataSet.GetTrackables ();
			Trackable oldest = null;
			foreach (Trackable trackable in trackables) {
				if (oldest == null || trackable.ID < oldest.ID)
					oldest = trackable;
			}

			if (oldest != null) {
				Debug.Log ("Destroying oldest trackable in UDT dataset: " + oldest.Name);
				mBuiltDataSet.Destroy (oldest, true);
			}
		}

		// Get predefined trackable and instantiate it
		ImageTargetBehaviour imageTargetCopy = (ImageTargetBehaviour)Instantiate (ImageTargetTemplate);
		imageTargetCopy.gameObject.name = lastTargetName;

		// Add the duplicated trackable to the data set and activate it
		mBuiltDataSet.CreateTrackable (trackableSource, imageTargetCopy.gameObject);

		// Activate the dataset again
		mObjectTracker.ActivateDataSet (mBuiltDataSet);

		// Extended Tracking with user defined targets only works with the most recently defined target.
		// If tracking is enabled on previous target, it will not work on newly defined target.
		// Don't need to call this if you don't care about extended tracking.
		StopExtendedTracking ();
		mObjectTracker.Stop ();
		mObjectTracker.ResetExtendedTracking ();
		mObjectTracker.Start ();

		OnNewTargetCreated (imageTargetCopy.gameObject);
	}

	#endregion IUserDefinedTargetEventHandler implementation

	#region AbstractVisualTargetFactory implementation

	public override void CreateTarget (string targetName, TrackerTargetType targetType)
	{
		if (frameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_MEDIUM ||
		    frameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_HIGH) {

			switch (targetType) {
			case TrackerTargetType.RealtimeCaptured:
			default:
				Debug.LogFormat ("{0}: Building user defined target with size if {1}", GetType ().Name, ImageTargetTemplate.GetSize ());
				mTargetBuildingBehaviour.BuildNewTarget (targetName, ImageTargetTemplate.GetSize ().x);
				break;
			}

			lastTargetName = targetName;
		} else {
			Debug.LogWarningFormat ("{0}: Cannot build new target, due to poor camera image quality. [current Q: {1}]", GetType ().Name, frameQuality);
		}
	}

	public override void DestroyTarget (string targetName)
	{
		var trackable = GameObject.Find (targetName).GetComponent<ImageTargetBehaviour> ().Trackable;
		var dataSets = mObjectTracker.GetActiveDataSets ();
		foreach (var ds in dataSets) {
			if (ds.Contains (trackable)) {
				mObjectTracker.DeactivateDataSet (ds);
				ds.Destroy (trackable, true);
				mObjectTracker.ActivateDataSet (ds);
				break;
			}
		}

	}

	#endregion //AbstractVisualTargetFactory implementation

	#region PUBLIC_METHODS

	// scanning must be started in order for the camera quality settings to be updated
	public void StartScanning ()
	{
		mTargetBuildingBehaviour.StartScanning ();
	}

	public void StopScanning ()
	{
		mTargetBuildingBehaviour.StopScanning ();
	}

	#endregion //PUBLIC_METHODS

	#region PRIVATE_METHODS

	/// <summary>
	/// This method only demonstrates how to handle extended tracking feature when you have multiple targets in the scene
	/// So, this method could be removed otherwise
	/// </summary>
	private void StopExtendedTracking ()
	{
		// If Extended Tracking is enabled, we first disable it for all the trackables
		// and then enable it only for the newly created target
		if (isExtendedTrackingEnabled) {
			StateManager stateManager = TrackerManager.Instance.GetStateManager ();

			List<ImageTargetBehaviour> trackableList = stateManager.GetTrackableBehaviours()
				.Cast<ImageTargetBehaviour>().ToList();

			// 1. Stop extended tracking on all the trackables
			foreach (var itb in trackableList) {
				itb.ImageTarget.StopExtendedTracking ();
			}

			// 2. Start Extended Tracking on the most recently added target
			var lastItb = trackableList.FirstOrDefault (t => t.transform.root.name == lastTargetName);
			if (lastItb != null) {
				if (lastItb.ImageTarget.StartExtendedTracking ())
					Debug.Log ("Extended Tracking successfully enabled for target named " + lastItb.name);
			}
		}
	}

	#endregion //PRIVATE_METHODS
}




using UnityEngine;
using System.Collections;
using System;

namespace Augio
{
	
	public class VisualSequencerFactory : Singleton<VisualSequencerFactory>
	{

		public static int lastTargetIndex = 1;

		public AbstractVisualTargetFactory targetFactory = null;

		/// <summary>
		/// Occurs when a new target was created.
		/// <param name="newTarget">A GameObject instance of the new target</param>
		/// </summary>
		public event TargetCreated OnTargetCreated;

		public delegate void TargetCreated (GameObject newTarget);

		void Start ()
		{
			if (targetFactory == null) {
				Debug.LogErrorFormat ("{0}: A visual target factory has not been assigned, please assign one", GetType ().Name);
			}

			targetFactory.NewTargetCreated += UdtEventHandler_OnNewTargetCreated;
		}

		void UdtEventHandler_OnNewTargetCreated (GameObject go)
		{
			if (OnTargetCreated != null)
				OnTargetCreated (go);
		}

		private static string GetNewTargetName ()
		{
			return string.Format ("Target_{0}", ++lastTargetIndex);
		}

		private static GameObject CreateTargetGameObject ()
		{
			return new GameObject (GetNewTargetName ());
		}

		public static MarkerController CreateMarkerController (GameObject go, Sequencer seqChannel, 
		                                                       AudioModifierKind kind, AbstractTrackedAudioObject tao, 
		                                                       MarkerOptionsMenu optionsMenu = null, MarkerLevelUI mlui = null)
		{
			var mc = go.GetComponent<MarkerController> () ?? go.AddComponent<MarkerController> ();
			mc.audioModCtrlKind = kind;
			mc.trackedObject = tao;
			mc.optionsMenu = optionsMenu;
			mc.levelUI = mlui;
			return mc;
		}

		public static void CreateTrackedAudioObject (Sequencer seq, AudioModifierKind kind, TrackerTargetType targetType, string dbTargetId = null)
		{
			// since the target creation is not immediate we use an inline callback to handle the object
			// after it was created and setup the remaining components
			AbstractVisualTargetFactory.NewTargetCreatedHandler taoSetter = null;
			taoSetter = (GameObject go) => {
				AbstractTrackedAudioObject tao = go.GetComponent<AbstractTrackedAudioObject> () ?? go.AddComponent<AbstractTrackedAudioObject> ();
				tao.TrackKey = seq.name;
				tao.Init ();
				MarkerOptionsMenu mom = go.GetComponentInChildren<MarkerOptionsMenu> ();
				MarkerLevelUI mlui = go.GetComponentInChildren<MarkerLevelUI> ();
				MarkerController mc = CreateMarkerController (tao.gameObject, seq, kind, tao, mom, mlui);
				Debug.LogFormat ("{0}: Created new target '{1}' for track '{2}' of kind '{3}'", Instance.GetType ().Name, mc.gameObject.name, tao.TrackKey, mc.audioModCtrlKind);
				Instance.targetFactory.NewTargetCreated -= taoSetter;
			};

			string newTargetName = GetNewTargetName ();

			Instance.targetFactory.NewTargetCreated += taoSetter;
			Instance.targetFactory.CreateTarget (newTargetName, targetType);
		}

		public static void CreateTarget (Sequencer sequencer, AudioModifierKind kind, TrackerTargetType targetType)
		{
			CreateTrackedAudioObject (sequencer, kind, targetType);
		}

		public static void DestroyTarget (string targetName)
		{		
			Instance.targetFactory.DestroyTarget (targetName);
		}
	}

}
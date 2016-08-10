using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Augio
{
	public class VisualSequencerController : Singleton<VisualSequencerController>
	{

		public bool isDebug = true;

		public List<Sequencer> seqGroups = new List<Sequencer> ();

		private SequencerDriver sequencerDriver = null;

		public delegate void OnReadyEvent ();

		private event OnReadyEvent ReadyEvent;

		public event OnReadyEvent OnReady
		{
			add {
				if (_isReady)
					value();
				ReadyEvent += value;
			}
			remove {
				ReadyEvent -= value;
			}
		}


		bool _isReady = false;

		public bool IsReady {
			get {
				return _isReady;
			}
			private set {
				_isReady = value;
			}
		}

		// Use this for initialization
		void Awake ()
		{
			sequencerDriver = GameObject.FindObjectOfType<SequencerDriver> ();
			if (!sequencerDriver) {
				PrintWarning ("No sequencer driver found!!!");
			} else {
				if (!sequencerDriver.IsReady)
					sequencerDriver.OnReadyEvent = this.OnSequencerDriverReady;
				else
					this.OnSequencerDriverReady ();
				PrintDebug ("Initialized and registered 'OnSequencerDriverReady'");
			}
		}


		void OnSequencerDriverReady ()
		{
//		sequencerDriver.Mute(true);
//		sequencerDriver.Stop();
			PrintDebug ("OnSequencerDriverReady was called on " + sequencerDriver.sequencers.Length + " sequencer groups");
			foreach (var item in sequencerDriver.sequencers) {
				if (item.isMuted) {
					PrintDebug ("Item is muted: '" + item.name + "'");
				} else {
					item.isMuted = true;
					PrintDebug ("Item has been muted: '" + item.name + "'");

				}
				if (item.IsPlaying) {
					PrintWarning ("Item is playing: '" + item.name + "'");
					item.Stop ();
				}
			}
			IsReady = true;
			if (ReadyEvent != null) {
				ReadyEvent ();
			}
		}

		private void PrintWarning (string message)
		{
			if (isDebug)
				Debug.logger.LogWarning (GetType ().Name, message);
		}

		private void PrintDebug (string message)
		{
			if (isDebug)
				Debug.logger.Log (GetType ().Name, message);
		}

		public List<Sequencer> Sequencers {
			get {
				return sequencerDriver.gameObject.GetComponentsInChildren<Sequencer> ().ToList ();
			}
		}

		private bool _isPlaying = false;
		public bool IsPlaying
		{
			get {
				return _isReady && _isPlaying;
			}
			private set {
				_isPlaying = value;
			}
		}

		private bool TryGetSequencerGroup (string trackName, out SequencerGroup track)
		{
			track = sequencerDriver.gameObject.GetComponentsInChildren<SequencerGroup> ().FirstOrDefault (g => g.name == trackName);
			return track != null;
		}

		public bool TryGetSequencer (string trackName, out Sequencer track)
		{
			track = sequencerDriver.gameObject.GetComponentsInChildren<Sequencer> ().FirstOrDefault (g => g.name == trackName);
			return track != null;
		}

		public void ActivateTrack (string trackName)
		{
			Sequencer seq;
			if (TryGetSequencer (trackName, out seq)) {
				seq.Play ();
				seq.Mute (false);
				IsPlaying = true;
				PrintDebug ("Activated track " + seq.name);
			} else {
				PrintDebug ("Could not locate track named " + trackName);
			}
		}

		public void DeactivateTrack (string trackName)
		{
			Sequencer seq;
			if (TryGetSequencer (trackName, out seq)) {
				if (seq.IsPlaying) {
					seq.Mute (true);
					PrintDebug ("Deactivated track " + seq.name);
				}
				if (Sequencers.TrueForAll(s => !s.IsPlaying))
					IsPlaying = false;
			}
		}

		public void SetLevel (string sequencerName, AudioModifierKind kind, object level)
		{
			Sequencer seq = null;
			if (TryGetSequencer (sequencerName, out seq)) {
				switch (kind) {
				case AudioModifierKind.BPM:
					seq.bpm = (int)level;
					break;
				case AudioModifierKind.FXDryToWetRatio:
					throw new NotImplementedException ("Wet to dry ratio not implemented yet");

				case AudioModifierKind.Volume:
				default:
					seq.Volume = (float)level;
					break;
				}
			} else {
				PrintWarning ("Could not locate sequencer with name " + sequencerName + "!");
			}
		}

		public void SetVolume (string sequencerName, float level)
		{
			Sequencer seq = null;
			if (TryGetSequencer (sequencerName, out seq)) {
				seq.Volume = level;
			} else {
				PrintWarning ("Could not locate sequencer with name " + sequencerName + "!");
			}
		}

		public void SetBPM (string sequencerName, int bpmValue)
		{
			SetLevel (sequencerName, AudioModifierKind.BPM, bpmValue);
		}

		public void SetEffectDryWetRatio (string sequencerName, float fxValue)
		{
			SetLevel (sequencerName, AudioModifierKind.FXDryToWetRatio, fxValue);
		}

		public float GetLevel (string sequencerName)
		{
			Sequencer seq = null;
			if (TryGetSequencer (sequencerName, out seq)) {
				return seq.Volume;
			} else {
				PrintWarning ("Could not locate sequencer with name " + sequencerName + "!");
				return 0;
			}
		}
	}

}
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Augio
{
	public class AudioMixerController : MonoBehaviour
	{

		public bool isDebug = true;

		public AudioMixer audioMixer = null;
		public List<AudioTrack> audioTracks = new List<AudioTrack> ();
		private float currentPlayTime = 0f;

		void Start ()
		{
			VisualSequencerController.Instance.OnReady += Init;
		}

		void Init()
		{
			if (!audioMixer)
				audioMixer = FindObjectOfType<AudioMixer>();

			audioTracks = VisualSequencerController.Instance.Sequencers.Select(seq => new AudioTrack
				{
					Active = seq.isActiveAndEnabled,
					Clip = seq.clip,
					Name = seq.name,
					Source = seq.GetComponent<AudioSource>()
				})
				.ToList();
			
		}

		void Update ()
		{
			if (VisualSequencerController.Instance.IsPlaying)
			{
				currentPlayTime += Time.deltaTime;
			}
		}

		private void PrintDebug (string message)
		{
			if (isDebug)
				Debug.Log (message);
		}

		private bool TryGetAudioTrack (string trackName, out AudioTrack track)
		{
			track = audioTracks.FirstOrDefault (c => c.Name == trackName);
			return track != null;
		}

		public void ActivateTrack (string trackName)
		{
			AudioTrack track;
			if (TryGetAudioTrack (trackName, out track)) {
				if (!track.Active) {
					if (currentPlayTime > 0) {
						track.Source.PlayScheduled (-currentPlayTime);
					}
					track.Active = true;
					PrintDebug ("Activated track " + track.Name);
				}
			}
		}

		public void DeactivateTrack (string trackName)
		{
			AudioTrack track;
			if (TryGetAudioTrack (trackName, out track)) {
				if (track.Active) {
					track.Source.Stop ();
					track.Active = false;
					PrintDebug ("Deactivated track " + track.Name);
				}
			}
		}
	}

}
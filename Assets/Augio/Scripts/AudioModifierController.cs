using System;
using UnityEngine;

namespace Augio
{
	public class AudioModifierController : MonoBehaviour
	{

		void Awake ()
		{

		}

		public void ModifyAudio (string trackName, AudioModifierKind kind, object newValue)
		{
			switch (kind) {

			case AudioModifierKind.BPM:
				VisualSequencerController.Instance.SetBPM (trackName, Mathf.FloorToInt ((float)newValue));
				break;

			case AudioModifierKind.FXDryToWetRatio:
				VisualSequencerController.Instance.SetEffectDryWetRatio (trackName, (float)newValue);
				break;

			case AudioModifierKind.Volume:
			default:
				VisualSequencerController.Instance.SetLevel (trackName, AudioModifierKind.Volume, (float)newValue);
				break;

			}
		}
	}

}
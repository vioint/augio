using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.ComponentModel;
using System;
using Augio;

public class VisualSequencerUI : MonoBehaviour
{

	public Dropdown ddAvailableChannels = null;
	public Dropdown ddTargetType = null;
	public Dropdown ddAudioModifierKind = null;
	public Button btnCreateTarget = null;

	public TrackerTargetType selectedTargetType = TrackerTargetType.PredefinedImage;
	public AudioModifierKind selectedAudioModifierKind = AudioModifierKind.BPM;
	public string selectedAvailableChannel = "";

	private void LoadOptions ()
	{
		ddAvailableChannels.options = VisualSequencerController.Instance.Sequencers
			.ConvertAll ((s) => new Dropdown.OptionData (s.name));
		ddAvailableChannels.value = -1;

		ddAudioModifierKind.options = typeof(AudioModifierKind)
			.ToList<AudioModifierKind> ()
			.ConvertAll<Dropdown.OptionData> ((e) => new Dropdown.OptionData (e.GetDescription ()));
		ddAudioModifierKind.value = -1;

		ddTargetType.options = typeof(TrackerTargetType)
			.ToList<TrackerTargetType> ()
			.ConvertAll<Dropdown.OptionData> ((e) => new Dropdown.OptionData (e.GetDescription ()));
		ddTargetType.value = -1;
	}

	// Use this for initialization
	void Start ()
	{
		VisualSequencerController.Instance.OnReady += Init;
	}

	void AddListeners ()
	{
		btnCreateTarget.onClick.AddListener (OnCreateTargetClicked);
		ddTargetType.onValueChanged.AddListener (idx => {
			selectedTargetType = ddTargetType.options [idx].text.DescriptionToEnum<TrackerTargetType> ();
		});
		ddAvailableChannels.onValueChanged.AddListener (idx => {
			selectedAvailableChannel = ddAvailableChannels.options [idx].text;
		});
		ddAudioModifierKind.onValueChanged.AddListener (idx => {
			selectedAudioModifierKind = ddAudioModifierKind.options [idx].text.DescriptionToEnum<AudioModifierKind> ();
		});
	}

	void RemoveListeners ()
	{
		btnCreateTarget.onClick.RemoveAllListeners ();
		ddTargetType.onValueChanged.RemoveAllListeners ();
		ddAvailableChannels.onValueChanged.RemoveAllListeners ();
		ddAudioModifierKind.onValueChanged.RemoveAllListeners ();
	}
	
	// Update is called once per frame
	void Init ()
	{
		AddListeners ();
		LoadOptions ();
	}

	void OnDestroy ()
	{
		RemoveListeners ();
	}

	void OnCreateTargetClicked ()
	{
		Sequencer seq = null;
		if (VisualSequencerController.Instance.TryGetSequencer (selectedAvailableChannel, out seq)) {
			Augio.VisualSequencerFactory.CreateTarget (seq, selectedAudioModifierKind, selectedTargetType);
		} else {
			Debug.LogErrorFormat ("{0}: Could not locate sequencer named {1}", this.GetType ().Name, selectedAvailableChannel);
		}
	}
		
}

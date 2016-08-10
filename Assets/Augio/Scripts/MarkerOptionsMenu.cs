using System;
using UnityEngine;
using UnityEngine.UI;
using Augio;

public class MarkerOptionsMenu : MonoBehaviour
{
	MarkerController _controller = null;

	public Toggle menuVisibilityToggle = null;
	public GameObject menuContainer = null;

	public ToggleGroup modifierKindToggleGroup = null;

	public void RegisterController (MarkerController controller)
	{
		this._controller = controller;

		menuVisibilityToggle.onValueChanged.AddListener (menuContainer.SetActive);

		foreach (var t in modifierKindToggleGroup.GetComponentsInChildren<Toggle>()) {
			t.onValueChanged.AddListener (OnOptionsMenuToggleChanges);
			var toggleType = t.GetComponent<AudioModifierKindValue> ();
			t.isOn = toggleType.Kind == _controller.audioModCtrlKind;
			if (t.isOn)
				Debug.LogFormat ("+++ {0}: Set {1} as the selected audmod kind of {2}", GetType ().Name, toggleType.Kind, transform.root.name);
		}
		// turn off menu
		menuVisibilityToggle.isOn = false;
		menuContainer.SetActive (false);
	}

	private void OnOptionsMenuToggleChanges (bool state)
	{
		foreach (Toggle t in modifierKindToggleGroup.ActiveToggles()) {
			var toggleType = t.GetComponent<AudioModifierKindValue> ();
			_controller.audioModCtrlKind = toggleType.Kind;
//			VisualSequencerController.Instance.SetLevel(_controller.trackedObject.TrackKey, toggleType.Kind, _controller.currentLevel);
		}
	}
}



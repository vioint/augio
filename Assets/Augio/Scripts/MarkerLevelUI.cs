using System;
using UnityEngine;
using UnityEngine.UI;

public class MarkerLevelUI : MonoBehaviour
{
	public Text levelText = null;
	public Text orientationText = null;
	public Image levelCircularMeter = null;
	public string textLevelFormat = "Volume {0}";
	public string textOrientationFormat = "Orientation {0}";

	public float currentLevelValue = 0;
	public string currentLevelText = "0";

	public bool showOrientation = false;

	void Start()
	{
		SetupUI();
	}


	private void UpdateLevelText(string levelTextStr)
	{
		levelText.text = string.Format(textLevelFormat, levelTextStr);
	}

	public void UpdateOrientationText(Quaternion orientation)
	{
		orientationText.text = string.Format(textOrientationFormat, orientation);
	}

	private void UpdateLevelMeter(float level)
	{
		levelCircularMeter.fillAmount = level;
	}

	public void UpdateLevel(float levelValue, float normalizedLevelValue, string levelText)
	{
		currentLevelValue = levelValue;
		currentLevelText = levelText;
		UpdateLevelMeter(normalizedLevelValue);
		UpdateLevelText(currentLevelText);
	}

	private void SetupUI()
	{
		
		if (!levelCircularMeter) {
			Debug.LogWarningFormat("{0}: No circular level meter found.", transform.root.name);
		} else {
			levelCircularMeter.type = Image.Type.Filled;
			levelCircularMeter.fillMethod = Image.FillMethod.Radial360;
			levelCircularMeter.fillClockwise = true;
			levelCircularMeter.fillOrigin = (int)Image.Origin360.Bottom;
			levelCircularMeter.fillCenter = false;
			levelCircularMeter.fillAmount = 0f;
			levelCircularMeter.preserveAspect = true;
		}

		if (orientationText) {
			orientationText.enabled = showOrientation;
		}
		
		Debug.LogFormat("{0}: Initialized {1}", transform.root.name, GetType().Name);
	}

}



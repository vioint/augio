using System.ComponentModel;

namespace Augio
{
	public enum TrackerTargetType
	{
		[Description ("Realtime captured")]
		RealtimeCaptured,
		[Description ("Predefined image")]
		PredefinedImage
	}
}
using System;
using System.ComponentModel;

namespace Augio
{
	public enum AudioModifierKind
	{
		[Description ("Volume")]
		Volume,
		[Description ("BPM")]
		BPM,
		[Description ("FX Dry to Wet ratio")]
		FXDryToWetRatio
	}
}
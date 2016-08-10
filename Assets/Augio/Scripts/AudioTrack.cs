using System;
using UnityEngine;

namespace Augio
{
	[Serializable]
	public class AudioTrack
	{
		public string Name = "";

		public AudioClip Clip = null;

		[HideInInspector]
		public bool Active = false;

		[HideInInspector]
		public AudioSource Source = null;

		public AudioTrack ()
		{
		}


	}
}


using System;
using UnityEngine;

namespace Augio
{

	public abstract class AbstractVisualTargetFactory : UnityEngine.MonoBehaviour
	{
		public delegate void NewTargetCreatedHandler(GameObject newTarget);

		public event NewTargetCreatedHandler NewTargetCreated;

		protected virtual void OnNewTargetCreated(GameObject newTarget)
		{
			if (NewTargetCreated != null)
				NewTargetCreated(newTarget);
		}

		public abstract void CreateTarget(string targetName, TrackerTargetType targetType);

		public abstract void DestroyTarget(string targetName);
	}
}
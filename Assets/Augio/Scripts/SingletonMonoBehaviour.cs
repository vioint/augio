using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private static T _instance = default(T);

	public static T Instance
	{
		get {
			return _instance ?? (_instance = MonoBehaviour.FindObjectOfType<T>() ?? (new GameObject { name = typeof(T).Name }).AddComponent<T>());
		}
	}
}

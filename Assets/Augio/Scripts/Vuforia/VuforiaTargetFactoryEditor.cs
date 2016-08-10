/*==============================================================================
Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
==============================================================================*/

using UnityEditor;
using Vuforia;

/// <summary>
/// This editor class renders the custom inspector for the UDTEventHandler MonoBehaviour
/// </summary>
[CustomEditor(typeof(VuforiaTargetFactory))]
public class VuforiaTargetFactoryEditor : Editor
{
	#region UNITY_EDITOR_METHODS

	// Draws a custom UI for the user defined target event handler inspector
	public override void OnInspectorGUI()
	{
		VuforiaTargetFactory vtf = (VuforiaTargetFactory)target;

		EditorGUILayout.HelpBox("Here you can set the ImageTargetBehaviour from the scene that will be used to augment user created targets.", MessageType.Info);
		bool allowSceneObjects = !EditorUtility.IsPersistent(target);
		vtf.ImageTargetTemplate = (ImageTargetBehaviour)EditorGUILayout.ObjectField("Image Target Template",
		vtf.ImageTargetTemplate, typeof(ImageTargetBehaviour), allowSceneObjects);

		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.EnumPopup("Frame quality", vtf.frameQuality);
		EditorGUI.EndDisabledGroup();
	}

	#endregion // UNITY_EDITOR_METHODS
}
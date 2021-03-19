/* StatusFillFollowerEditor.cs */
/* Written by Kaz Crowe */
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[CustomEditor( typeof( StatusFillFollower ) )]
public class StatusFillFollowerEditor : Editor
{
	StatusFillFollower targ;
	List<string> statusOptions;
	SerializedProperty ultimateStatusBar, statusIndex;
	SerializedProperty imageSize, imageAspectRatio, targetImage;
	SerializedProperty xRatio, yRatio;
	SerializedProperty minimumPosition, maximumPosition;
	float testValue = 100.0f;
	bool hasImageComponent = false;
	

	void OnEnable ()
	{
		// Store the references to all variables.
		StoreReferences();

		// Register the UndoRedoCallback function to be called when an undo/redo is performed.
		Undo.undoRedoPerformed += UndoRedoCallback;
	}

	void OnDisable ()
	{
		// Remove the UndoRedoCallback from the Undo event.
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}
	
	void UndoRedoCallback ()
	{
		// Re-reference all variables on undo/redo.
		StoreReferences();
	}

	void StoreReferences ()
	{
		targ = ( StatusFillFollower )target;

		if( targ == null )
			return;

		ultimateStatusBar = serializedObject.FindProperty( "ultimateStatusBar" );
		statusIndex = serializedObject.FindProperty( "statusIndex" );
		imageSize = serializedObject.FindProperty( "imageSize" );
		minimumPosition = serializedObject.FindProperty( "minimumPosition" );
		maximumPosition = serializedObject.FindProperty( "maximumPosition" );

		imageAspectRatio = serializedObject.FindProperty( "imageAspectRatio" );
		targetImage = serializedObject.FindProperty( "targetImage" );
		xRatio = serializedObject.FindProperty( "xRatio" );
		yRatio = serializedObject.FindProperty( "yRatio" );

		if( targ.ultimateStatusBar != null && targ.ultimateStatusBar.UltimateStatusList.Count >= targ.statusIndex )
		{
			statusIndex.intValue = 0;
			serializedObject.ApplyModifiedProperties();
		}

		if( targ.ultimateStatusBar != null && targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].statusImage != null )
		{
			if( targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].fillConstraint )
				testValue = ( targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].statusImage.fillAmount - targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].fillConstraintMin ) / ( targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].fillConstraintMax - targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].fillConstraintMin ) * 100;
			else
				testValue = targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].statusImage.fillAmount * 100;
		}

		hasImageComponent = targ.GetComponent<Image>();

		statusOptions = new List<string>();

		if( targ.ultimateStatusBar != null )
		{
			for( int i = 0; i < targ.ultimateStatusBar.UltimateStatusList.Count; i++ )
			{
				if( targ.ultimateStatusBar.UltimateStatusList[ i ].statusName != string.Empty )
					statusOptions.Add( targ.ultimateStatusBar.UltimateStatusList[ i ].statusName );
			}

			if( statusIndex.intValue > statusOptions.Count - 1 )
			{
				statusIndex.intValue = 0;
				serializedObject.ApplyModifiedProperties();
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		
		EditorGUILayout.Space();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( ultimateStatusBar );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();

			StoreReferences();
		}

		if( targ.ultimateStatusBar == null )
		{
			EditorGUILayout.BeginVertical( "Box" );
			EditorGUILayout.HelpBox( "Please assign the targeted Ultimate Status Bar before continuing.", MessageType.Warning );
			if( GUILayout.Button( "Find Status Bar" ) )
			{
				ultimateStatusBar.objectReferenceValue = targ.gameObject.GetComponentInParent<UltimateStatusBar>();
				serializedObject.ApplyModifiedProperties();

				if( targ.ultimateStatusBar == null )
					Debug.LogWarning( "Status Fill Follower - Could not find an Ultimate Status Bar component in any parent GameObjects." );

				StoreReferences();
			}
			EditorGUILayout.EndVertical();
		}

		if( targ.ultimateStatusBar != null && targ.ultimateStatusBar.positioning != UltimateStatusBar.Positioning.Enabled )
			EditorGUILayout.HelpBox( "The Ultimate Status Bar associated with this component is not set to Screen Space for Positioning.", MessageType.Error );

		if( targ.ultimateStatusBar != null && targ.ultimateStatusBar.positioning == UltimateStatusBar.Positioning.Enabled )
		{
			if( targ.ultimateStatusBar != null && statusOptions.Count > 1 )
			{
				EditorGUI.BeginChangeCheck();
				statusIndex.intValue = EditorGUILayout.Popup( "Status Name", statusIndex.intValue, statusOptions.ToArray() );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Slider( imageSize, 0.0f, 1.0f, new GUIContent( "RectTransform Size", "Determines the overall size of the rect transform." ) );
			if( hasImageComponent )
				EditorGUILayout.PropertyField( imageAspectRatio, new GUIContent( "Image Aspect Ratio", "Determines if the aspect ratio should be calculated or manually set." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			if( !hasImageComponent )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( xRatio, 0.0f, 1.0f, new GUIContent( "X Ratio", "The desired width of the image." ) );
				EditorGUILayout.Slider( yRatio, 0.0f, 1.0f, new GUIContent( "Y Ratio", "The desired height of the image." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
			}
			else
			{
				if( targ.imageAspectRatio == StatusFillFollower.ImageAspectRatio.Preserve )
				{
					EditorGUI.indentLevel = 1;
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( targetImage, new GUIContent( "Target Image", "The targeted image to preserve the aspect ratio of." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
					EditorGUI.indentLevel = 0;

					if( targ.targetImage == null )
					{
						EditorGUILayout.BeginVertical( "Box" );
						EditorGUILayout.HelpBox( "The Target Image component needs to be assigned in order to preserve the aspect of the image.", MessageType.Error );
						if( GUILayout.Button( "Find", EditorStyles.miniButton ) )
						{
							targetImage.objectReferenceValue = targ.GetComponent<Image>();
							serializedObject.ApplyModifiedProperties();
						}
						EditorGUILayout.EndVertical();
					}
					
					if( targ.targetImage != null && targ.targetImage.sprite == null )
						EditorGUILayout.HelpBox( "The Target Image does not have a Source Image assigned to it.", MessageType.Error );
				}
				else if( targ.imageAspectRatio == StatusFillFollower.ImageAspectRatio.Custom )
				{
					EditorGUI.indentLevel = 1;
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.Slider( xRatio, 0.0f, 1.0f, new GUIContent( "X Ratio", "The desired width of the image." ) );
					EditorGUILayout.Slider( yRatio, 0.0f, 1.0f, new GUIContent( "Y Ratio", "The desired height of the image." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
					EditorGUI.indentLevel = 0;
				}
			}

			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical( "Box" );
			GUIStyle centeredLabel = new GUIStyle( EditorStyles.label ) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
			EditorGUILayout.LabelField( "Position Anchors", centeredLabel );

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( "Minimum Position: " + ( minimumPosition.vector2Value != Vector2.zero ? minimumPosition.vector2Value.ToString() : "NaN" ) );
			if( GUILayout.Button( "Set", EditorStyles.miniButton ) )
			{
				if( minimumPosition.vector2Value != Vector2.zero )
				{
					if( EditorUtility.DisplayDialog( "Status Fill Follower", "Warning! You are about to overwrite a previously registered position.\n\nContinue?", "Yes", "No" ) )
					{
						minimumPosition.vector2Value = ConfigureMinimumPosition( targ.GetComponent<RectTransform>().localPosition );
						serializedObject.ApplyModifiedProperties();
					}
				}
				else
				{
					minimumPosition.vector2Value = ConfigureMinimumPosition( targ.GetComponent<RectTransform>().localPosition );
					serializedObject.ApplyModifiedProperties();
				}
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( "Maximum Position: " + ( maximumPosition.vector2Value != Vector2.zero ? maximumPosition.vector2Value.ToString() : "NaN" ) );
			if( GUILayout.Button( "Set", EditorStyles.miniButton ) )
			{
				if( maximumPosition.vector2Value != Vector2.zero )
				{
					if( EditorUtility.DisplayDialog( "Status Fill Follower", "Warning! You are about to overwrite a previously registered position.\n\nContinue?", "Yes", "No" ) )
					{
						maximumPosition.vector2Value = ConfigureMaximumPosition( targ.GetComponent<RectTransform>().localPosition );
						serializedObject.ApplyModifiedProperties();
					}
				}
				else
				{
					maximumPosition.vector2Value = ConfigureMaximumPosition( targ.GetComponent<RectTransform>().localPosition );
					serializedObject.ApplyModifiedProperties();
				}
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space( 1 );
			EditorGUILayout.EndVertical();

			// TEST VALUE //
			EditorGUI.BeginChangeCheck();
			testValue = EditorGUILayout.Slider( new GUIContent( "Test Value" ), testValue, 0.0f, 100.0f );
			if( EditorGUI.EndChangeCheck() )
			{
				if( targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].statusImage != null )
				{
					Undo.RecordObject( targ.GetComponent<RectTransform>(), "Status Bar Test Value" );

					Undo.RecordObject( targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].statusImage, "Status Bar Test Value" );

					if( targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].displayText != UltimateStatusBar.UltimateStatus.DisplayText.Disabled && targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].statusText != null )
						Undo.RecordObject( targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].statusText, "Status Bar Test Value" );

					targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].statusImage.enabled = false;
					targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].UpdateStatus( testValue, 100.0f );
					targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].statusImage.enabled = true;

					targ.OnStatusUpdated( targ.ultimateStatusBar.UltimateStatusList[ targ.statusIndex ].CalculatedPercentage );
				}
			}
		}

		EditorGUILayout.Space();

		Repaint();
	}
	
	Vector3 ConfigureMinimumPosition ( Vector3 pos )
	{
		if( targ.ultimateStatusBar == null )
			return Vector2.zero;
		
		Vector3 tempVector = pos;
		tempVector.x = -tempVector.x / targ.ultimateStatusBar.BaseTransform.sizeDelta.x;
		tempVector.y = -tempVector.y / targ.ultimateStatusBar.BaseTransform.sizeDelta.y;
		return tempVector;
	}

	Vector3 ConfigureMaximumPosition ( Vector3 pos )
	{
		if( targ.ultimateStatusBar == null )
			return Vector2.zero;
		
		Vector3 tempVector = pos;
		tempVector.x = -tempVector.x / targ.ultimateStatusBar.BaseTransform.sizeDelta.x;
		tempVector.y = -tempVector.y / targ.ultimateStatusBar.BaseTransform.sizeDelta.y;
		return tempVector;
	}
}
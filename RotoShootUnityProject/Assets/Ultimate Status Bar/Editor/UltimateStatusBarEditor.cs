/* UltimateStatusBarEditor.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor( typeof( UltimateStatusBar ) )]
public class UltimateStatusBarEditor : Editor
{
	UltimateStatusBar targ;

	// STATUS BAR POSITIONING //
	SerializedProperty positioning, scalingAxis, statusBarSize, imageAspectRatio, targetImage;
	SerializedProperty xRatio, yRatio, horizontalPosition, verticalPosition;
	// Screen Space Camera Options //
	SerializedProperty depthPosition;
	float xPivot = 0.5f, yPivot = 0.5f;
	// World Space Canvas Options //
	Canvas parentCanvas;
	RectTransform parentCanvasRectTrans;
	float parentCanvasScale = 1.0f;
	Vector3 parentCanvasPosition = Vector3.zero;
	Vector3 parentCanvasRotation = Vector3.zero;
	Vector2 parentCanvasSizeDelta = Vector2.zero;

	// STATUS BAR OPTIONS //
	bool useStatusBarIcon = false, useStatusBarText = false;
	SerializedProperty statusBarIcon, statusBarText;
	Color statusBarIconColor = Color.white, statusBarTextColor = Color.white;
	string tempStatusString = "";
	bool useStatusBarTextOutline = false;
	Font statusBarTextFont;
	SerializedProperty updateVisibility, idleSeconds, statusBarAnimator, initialState;
	SerializedProperty enableDuration, disableDuration, enabledAlpha, disabledAlpha, onStatusUpdated;
	
	// STATUS INFORMATION //
	List<float> testValue;
	List<SerializedProperty> statusName, statusImage;
	// Image Options //
	List<Sprite> statusImageSprite;
	List<Image.FillMethod> fillMethod;
	List<Image.OriginHorizontal> fillMethodHorizontal;
	List<Image.OriginVertical> fillMethodVertical;
	List<Image.Origin90> fillMethod90;
	List<Image.Origin180> fillMethod180;
	List<Image.Origin360> fillMethod360;
	// Color Mode //
	List<SerializedProperty> colorMode, statusGradient;
	List<Color> statusColor;
	// Text Options //
	List<Color> statusTextColor;
	List<bool> useStatusTextOutline;
	List<Color> statusTextColorOutline;
	List<SerializedProperty> statusText, displayText, additionalText;
	List<Font> statusTextFont;
	// Smooth Fill //
	List<SerializedProperty> smoothFill, smoothFillSpeed;
	// Fill Constraint //
	List<SerializedProperty> fillConstraint, fillConstraintMin, fillConstraintMax, fillConstraintTicks;
	// Dramatic Fill //
	List<SerializedProperty> useDramaticFill, dramaticImage, dramaticStyle, secondsDelay, resetSensitivity, fillSpeed;
	List<Color> dramaticImageColor;
	// Visibility Options //
	List<SerializedProperty> keepVisible, triggerValue;

	// SCRIPT REFERENCE //
	bool UltimateStatusBarNameDuplicate = false;
	SerializedProperty statusBarName;
	List<string> statusNameList;
	int statusNameListIndex;
	
	class ExampleCode
	{
		public string optionName = "";
		public string optionDescription = "";
		public string basicCode = "";
	}
	ExampleCode[] PublicExampleCodes = new ExampleCode[]
	{
		new ExampleCode()
		{
			optionName = "Update Status",
			optionDescription = "Updates the default status on the Ultimate Status Bar.",
			basicCode = "statusBar.UpdateStatus( currentValue, maxValue );"
		},
		new ExampleCode()
		{
			optionName = "Update Status",
			optionDescription = "Updates the targeted Ultimate Status on the Ultimate Status Bar.",
			basicCode = "statusBar.UpdateStatus( current, max, \"{0}\" );",
		},
		new ExampleCode()
		{
			optionName = "Enable Status Bar",
			optionDescription = "Enables the status bar visually.",
			basicCode = "statusBar.EnableStatusBar();"
		},
		new ExampleCode()
		{
			optionName = "Disable Status Bar",
			optionDescription = "Disables the status bar visually.",
			basicCode = "statusBar.DisableStatusBar();"
		},
	};
	ExampleCode[] StaticExampleCodes = new ExampleCode[]
	{
		new ExampleCode()
		{
			optionName = "Update Status",
			optionDescription = "Updates the default status on the targeted Ultimate Status Bar.",
			basicCode = "UltimateStatusBar.UpdateStatus( \"{0}\", currentValue, maxValue );"
		},
		new ExampleCode()
		{
			optionName = "Update Status",
			optionDescription = "Updates the specific status on the targeted Ultimate Status Bar.",
			basicCode = "UltimateStatusBar.UpdateStatus( \"{0}\", currentValue, maxValue, \"{1}\" );",
		},
		new ExampleCode()
		{
			optionName = "Enable Status Bar",
			optionDescription = "Enables the targeted Ultimate Status Bar visually.",
			basicCode = "UltimateStatusBar.EnableStatusBar( \"{0}\" );"
		},
		new ExampleCode()
		{
			optionName = "Disable Status Bar",
			optionDescription = "Disables the targeted Ultimate Status Bar visually.",
			basicCode = "UltimateStatusBar.DisableStatusBar( \"{0}\" );"
		},
	};
	List<ExampleCode> PublicExampleCodeList = new List<ExampleCode>();
	List<ExampleCode> StaticExampleCodeList = new List<ExampleCode>();
	List<string> publicExampleCodeOptions = new List<string>();
	List<string> staticExampleCodeOptions = new List<string>();
	int exampleCodeIndex = 0;

	// DEVELOPMENT MODE //
	bool showDefaultInspector = false;

	// EDITOR STYLES //
	GUIStyle collapsableSectionStyle = new GUIStyle();
	GUIStyle headerDropdownStyle = new GUIStyle();
	GUIStyle statusInformationStyle = new GUIStyle();

	void OnEnable ()
	{
		Undo.undoRedoPerformed += UndoRedoCallback;

		StoreReferences();
		
		if( !targ.GetComponent<CanvasGroup>() )
			Undo.AddComponent( targ.gameObject, typeof( CanvasGroup ) );
	}

	void OnDisable ()
	{
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}

	void UndoRedoCallback ()
	{
		StoreReferences();
	}

	void StoreReferences ()
	{
		targ = ( UltimateStatusBar )target;

		if( targ == null )
			return;

		CheckForParentCanvas();
		
		// STATUS BAR POSITIONING //
		positioning = serializedObject.FindProperty( "positioning" );
		scalingAxis = serializedObject.FindProperty( "scalingAxis" );
		statusBarSize = serializedObject.FindProperty( "statusBarSize" );
		imageAspectRatio = serializedObject.FindProperty( "imageAspectRatio" );
		targetImage = serializedObject.FindProperty( "targetImage" );
		xRatio = serializedObject.FindProperty( "xRatio" );
		yRatio = serializedObject.FindProperty( "yRatio" );
		horizontalPosition = serializedObject.FindProperty( "horizontalPosition" );
		verticalPosition = serializedObject.FindProperty( "verticalPosition" );
		depthPosition = serializedObject.FindProperty( "depthPosition" );
		if( parentCanvas != null )
		{
			parentCanvasRectTrans = parentCanvas.GetComponent<RectTransform>();
			parentCanvasScale = parentCanvasRectTrans.localScale.x;
			parentCanvasPosition = parentCanvasRectTrans.position;
			parentCanvasRotation = parentCanvasRectTrans.eulerAngles;
			parentCanvasSizeDelta = parentCanvasRectTrans.sizeDelta;
		}
		
		// STATUS INFORMATION //
		if( targ.UltimateStatusList.Count == 0 )
		{
			serializedObject.FindProperty( "UltimateStatusList" ).arraySize++;
			serializedObject.ApplyModifiedProperties();
			targ.UltimateStatusList[ 0 ] = new UltimateStatusBar.UltimateStatus();
		}

		testValue = new List<float>();
		statusName = new List<SerializedProperty>();
		statusImage = new List<SerializedProperty>();
		statusImageSprite = new List<Sprite>();
		fillMethod = new List<Image.FillMethod>();
		fillMethodHorizontal = new List<Image.OriginHorizontal>();
		fillMethodVertical = new List<Image.OriginVertical>();
		fillMethod90 = new List<Image.Origin90>();
		fillMethod180 = new List<Image.Origin180>();
		fillMethod360 = new List<Image.Origin360>();
		colorMode = new List<SerializedProperty>();
		statusColor = new List<Color>();
		statusGradient = new List<SerializedProperty>();

		displayText = new List<SerializedProperty>();
		statusText = new List<SerializedProperty>();
		additionalText = new List<SerializedProperty>();
		statusTextColor = new List<Color>();
		useStatusTextOutline = new List<bool>();
		statusTextColorOutline = new List<Color>();
		statusTextFont = new List<Font>();

		smoothFill = new List<SerializedProperty>();
		smoothFillSpeed = new List<SerializedProperty>();

		fillConstraint = new List<SerializedProperty>();
		fillConstraintMin = new List<SerializedProperty>();
		fillConstraintMax = new List<SerializedProperty>();
		fillConstraintTicks = new List<SerializedProperty>();

		useDramaticFill = new List<SerializedProperty>();
		dramaticImage = new List<SerializedProperty>();
		dramaticStyle = new List<SerializedProperty>();
		secondsDelay = new List<SerializedProperty>();
		resetSensitivity = new List<SerializedProperty>();
		fillSpeed = new List<SerializedProperty>();
		dramaticImageColor = new List<Color>();

		keepVisible = new List<SerializedProperty>();
		triggerValue = new List<SerializedProperty>();
		
		for( int i = 0; i < targ.UltimateStatusList.Count; i++ )
		{
			testValue.Add( 100.0f );
			if( targ.UltimateStatusList[ i ].statusImage != null )
			{
				if( targ.UltimateStatusList[ i ].fillConstraint )
					testValue[ i ] = ( targ.UltimateStatusList[ i ].statusImage.fillAmount - targ.UltimateStatusList[ i ].fillConstraintMin ) / ( targ.UltimateStatusList[ i ].fillConstraintMax - targ.UltimateStatusList[ i ].fillConstraintMin ) * 100;
				else
					testValue[ i ] = targ.UltimateStatusList[ i ].statusImage.fillAmount * 100;
			}

			statusName.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].statusName", i ) ) );
			statusImage.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].statusImage", i ) ) );
			
			if( targ.UltimateStatusList[ i ].statusImage != null && targ.UltimateStatusList[ i ].statusImage.type != Image.Type.Filled )
				targ.UltimateStatusList[ i ].statusImage.type = Image.Type.Filled;

			statusImageSprite.Add( targ.UltimateStatusList[ i ].statusImage == null ? null : targ.UltimateStatusList[ i ].statusImage.sprite );
			fillMethod.Add( targ.UltimateStatusList[ i ].statusImage == null ? Image.FillMethod.Horizontal : targ.UltimateStatusList[ i ].statusImage.fillMethod );
			fillMethodHorizontal.Add( targ.UltimateStatusList[ i ].statusImage == null ? Image.OriginHorizontal.Left : ( Image.OriginHorizontal )targ.UltimateStatusList[ i ].statusImage.fillOrigin );
			fillMethodVertical.Add( targ.UltimateStatusList[ i ].statusImage == null ? Image.OriginVertical.Bottom : ( Image.OriginVertical )targ.UltimateStatusList[ i ].statusImage.fillOrigin );
			fillMethod90.Add( targ.UltimateStatusList[ i ].statusImage == null ? Image.Origin90.BottomLeft : ( Image.Origin90 )targ.UltimateStatusList[ i ].statusImage.fillOrigin );
			fillMethod180.Add( targ.UltimateStatusList[ i ].statusImage == null ? Image.Origin180.Bottom : ( Image.Origin180 )targ.UltimateStatusList[ i ].statusImage.fillOrigin );
			fillMethod360.Add( targ.UltimateStatusList[ i ].statusImage == null ? Image.Origin360.Bottom : ( Image.Origin360 )targ.UltimateStatusList[ i ].statusImage.fillOrigin );
			colorMode.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].colorMode", i ) ) );
			statusColor.Add( Color.white );
			statusGradient.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].statusGradient", i ) ) );

			displayText.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].displayText", i ) ) );
			statusText.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].statusText", i ) ) );
			additionalText.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].additionalText", i ) ) );
			statusTextColor.Add( Color.white );
			useStatusTextOutline.Add( targ.UltimateStatusList[ i ].statusText != null && targ.UltimateStatusList[ i ].statusText.GetComponent<Outline>() && targ.UltimateStatusList[ i ].statusText.GetComponent<Outline>().enabled );
			statusTextColorOutline.Add( Color.white );
			statusTextFont.Add( targ.UltimateStatusList[ i ].statusText != null ? targ.UltimateStatusList[ i ].statusText.font : Resources.GetBuiltinResource<Font>( "Arial.ttf" ) );

			smoothFill.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].smoothFill", i ) ) );
			smoothFillSpeed.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].smoothFillSpeed", i ) ) );

			fillConstraint.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].fillConstraint", i ) ) );
			fillConstraintMin.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].fillConstraintMin", i ) ) );
			fillConstraintMax.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].fillConstraintMax", i ) ) );
			fillConstraintTicks.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].fillConstraintTicks", i ) ) );
			
			useDramaticFill.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].useDramaticFill", i ) ) );
			dramaticImage.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].dramaticImage", i ) ) );
			dramaticStyle.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].dramaticStyle", i ) ) );
			secondsDelay.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].secondsDelay", i ) ) );
			resetSensitivity.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].resetSensitivity", i ) ) );
			fillSpeed.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].fillSpeed", i ) ) );
			dramaticImageColor.Add( Color.white );
			if( targ.UltimateStatusList[ i ].useDramaticFill && targ.UltimateStatusList[ i ].dramaticImage != null && targ.UltimateStatusList[ i ].dramaticImage.type != Image.Type.Filled )
				targ.UltimateStatusList[ i ].dramaticImage.type = Image.Type.Filled;

			keepVisible.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].keepVisible", i ) ) );
			triggerValue.Add( serializedObject.FindProperty( string.Format( "UltimateStatusList.Array.data[{0}].triggerValue", i ) ) );
			
			if( !Application.isPlaying )
				targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100 );
		}

		// STATUS BAR OPTIONS //
		useStatusBarIcon = targ.statusBarIcon != null && targ.statusBarIcon.gameObject.activeInHierarchy;
		statusBarIcon = serializedObject.FindProperty( "statusBarIcon" );
		if( targ.statusBarIcon != null )
			statusBarIconColor = targ.statusBarIcon.color;

		useStatusBarText = targ.statusBarText != null && targ.statusBarText.gameObject.activeInHierarchy;
		statusBarText = serializedObject.FindProperty( "statusBarText" );
		if( targ.statusBarText != null )
		{
			statusBarTextColor = targ.statusBarText.color;
			tempStatusString = targ.statusBarText.text;
			useStatusBarTextOutline = targ.statusBarText.GetComponent<Outline>() && targ.statusBarText.GetComponent<Outline>().enabled;
		}

		statusBarTextFont = targ.statusBarText != null ? targ.statusBarText.font : Resources.GetBuiltinResource<Font>( "Arial.ttf" );

		updateVisibility = serializedObject.FindProperty( "updateVisibility" );
		idleSeconds = serializedObject.FindProperty( "idleSeconds" );
		enableDuration = serializedObject.FindProperty( "enableDuration" );
		disableDuration = serializedObject.FindProperty( "disableDuration" );
		enabledAlpha = serializedObject.FindProperty( "enabledAlpha" );
		disabledAlpha = serializedObject.FindProperty( "disabledAlpha" );
		statusBarAnimator = serializedObject.FindProperty( "statusBarAnimator" );
		initialState = serializedObject.FindProperty( "initialState" );
		onStatusUpdated = serializedObject.FindProperty( "onStatusUpdated" );

		// SCRIPT REFERENCE //
		UltimateStatusBarNameDuplicate = DuplicateStatusBarName();
		statusBarName = serializedObject.FindProperty( "statusBarName" );
		StoreExampleCode();
		StoreNameList();
	}

	bool DisplayHeaderDropdown ( string headerName, string editorPref )
	{
		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
		GUILayout.Space( -10 );

		EditorGUI.BeginChangeCheck();
		GUILayout.Toggle( EditorPrefs.GetBool( editorPref ), ( EditorPrefs.GetBool( editorPref ) ? "▼" : "►" ) + headerName, headerDropdownStyle );
		if( EditorGUI.EndChangeCheck() )
			EditorPrefs.SetBool( editorPref, !EditorPrefs.GetBool( editorPref ) );

		GUILayout.EndHorizontal();

		if( EditorPrefs.GetBool( editorPref ) )
		{
			EditorGUILayout.Space();
			return true;
		}
		return false;
	}

	bool DisplayCollapsibleBoxSection ( string sectionTitle, string editorPref )
	{
		if( EditorPrefs.GetBool( editorPref ) )
			collapsableSectionStyle.fontStyle = FontStyle.Bold;

		if( GUILayout.Button( sectionTitle, collapsableSectionStyle ) )
			EditorPrefs.SetBool( editorPref, !EditorPrefs.GetBool( editorPref ) );

		if( EditorPrefs.GetBool( editorPref ) )
			collapsableSectionStyle.fontStyle = FontStyle.Normal;

		return EditorPrefs.GetBool( editorPref );
	}

	bool DisplayCollapsibleBoxSection ( string sectionTitle, string editorPref, ref bool enabledProp, ref bool valueChanged )
	{
		if( EditorPrefs.GetBool( editorPref ) && enabledProp )
			collapsableSectionStyle.fontStyle = FontStyle.Bold;

		EditorGUILayout.BeginHorizontal();

		EditorGUI.BeginChangeCheck();
		enabledProp = EditorGUILayout.Toggle( enabledProp, GUILayout.Width( 25 ) );
		if( EditorGUI.EndChangeCheck() )
		{
			if( enabledProp )
				EditorPrefs.SetBool( editorPref, true );
			else
				EditorPrefs.SetBool( editorPref, false );

			valueChanged = true;
		}

		GUILayout.Space( -25 );

		EditorGUI.BeginDisabledGroup( !enabledProp );
		if( GUILayout.Button( sectionTitle, collapsableSectionStyle ) )
			EditorPrefs.SetBool( editorPref, !EditorPrefs.GetBool( editorPref ) );
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.EndHorizontal();

		if( EditorPrefs.GetBool( editorPref ) )
			collapsableSectionStyle.fontStyle = FontStyle.Normal;

		return EditorPrefs.GetBool( editorPref ) && enabledProp;
	}

	void SpaceAfterIndent ()
	{
		GUILayout.Space( 2 );
	}

	void SubsectionEnd ()
	{
		EditorGUILayout.LabelField( "└────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────" );
	}

	void ClearFocusControl ()
	{
		GUI.FocusControl( "" );
	}

	void DisplayPropertyMinimizeButton ( string editorPref )
	{
		EditorGUI.BeginChangeCheck();
		GUILayout.Toggle( EditorPrefs.GetBool( editorPref ), EditorPrefs.GetBool( editorPref ) ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) );
		if( EditorGUI.EndChangeCheck() )
			EditorPrefs.SetBool( editorPref, !EditorPrefs.GetBool( editorPref ) );
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		collapsableSectionStyle = new GUIStyle( EditorStyles.label ) { alignment = TextAnchor.MiddleCenter };
		collapsableSectionStyle.onActive.textColor = collapsableSectionStyle.normal.textColor;
		collapsableSectionStyle.active.textColor = collapsableSectionStyle.normal.textColor;

		headerDropdownStyle = new GUIStyle( EditorStyles.toolbarButton ) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 11 };

		statusInformationStyle = new GUIStyle( EditorStyles.label ) { richText = true, alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold };

		bool valueChanged = false;

		if( DisplayHeaderDropdown( "Status Bar Positioning", "USB_StatusBarPositioning" ) )
		{
			// POSITIONING OPTION //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( positioning, new GUIContent( "Positioning", "Determines how the Ultimate Status Bar should position itself." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			// POSITIONING SCREEN SPACE //
			if( targ.positioning == UltimateStatusBar.Positioning.Enabled )
			{
				// AXIS AND SIZE //
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( scalingAxis, new GUIContent( "Scaling Axis", "Determines whether the Ultimate Status Bar is sized according to Screen Height or Screen Width." ) );
				EditorGUILayout.Slider( statusBarSize, 0.0f, 10.0f, new GUIContent( "Status Bar Size", "Determines the overall size of the status bar." ) );
				EditorGUILayout.Slider( horizontalPosition, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position of the image." ) );
				EditorGUILayout.Slider( verticalPosition, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position of the image." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				// IMAGE ASPECT RATIO //
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( imageAspectRatio, new GUIContent( "Image Aspect Ratio", "Determines if the aspect ratio should be calculated or manually set." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				// PRESERVE ASPECT RATIO //
				if( targ.imageAspectRatio == UltimateStatusBar.ImageAspectRatio.Preserve )
				{
					EditorGUI.indentLevel = 1;
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( targetImage, new GUIContent( "Target Image", "The targeted image to preserve the aspect ratio of." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
					EditorGUI.indentLevel = 0;

					if( targ.targetImage == null )
					{
						if( targ.GetComponent<Image>() && targ.GetComponent<Image>().sprite != null )
						{
							targetImage.objectReferenceValue = targ.GetComponent<Image>();
							serializedObject.ApplyModifiedProperties();
						}
						else
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
					}

					if( targ.targetImage != null && targ.targetImage.sprite == null )
						EditorGUILayout.HelpBox( "The Target Image does not have a Source Image assigned to it.", MessageType.Error );
				}

				// CUSTOM IMAGE RATIO //
				if( targ.imageAspectRatio == UltimateStatusBar.ImageAspectRatio.Custom )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUI.indentLevel = 1;
					EditorGUILayout.Slider( xRatio, 0.0f, 1.0f, new GUIContent( "X Ratio", "The desired width of the image." ) );
					EditorGUILayout.Slider( yRatio, 0.0f, 1.0f, new GUIContent( "Y Ratio", "The desired height of the image." ) );
					EditorGUI.indentLevel = 0;
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				
				if( parentCanvas != null && parentCanvas.renderMode == RenderMode.WorldSpace )
				{
					EditorGUILayout.BeginVertical( "Box" );
					if( DisplayCollapsibleBoxSection( "Canvas Options", "USB_CanvasOptions" ) )
					{
						EditorGUI.BeginChangeCheck();
						parentCanvasScale = EditorGUILayout.Slider( new GUIContent( "Canvas Scale", "The scale of the canvas rect transform." ), parentCanvasScale, 0.0f, 1.0f );
						if( EditorGUI.EndChangeCheck() )
						{
							Undo.RecordObject( parentCanvas.GetComponent<RectTransform>(), "Change Canvas Scale" );
							parentCanvas.GetComponent<RectTransform>().localScale = Vector3.one * parentCanvasScale;
						}

						EditorGUI.BeginChangeCheck();
						parentCanvasPosition = EditorGUILayout.Vector3Field( new GUIContent( "Canvas Position", "The position of the canvas rect transform." ), parentCanvasPosition );
						if( EditorGUI.EndChangeCheck() )
						{
							Undo.RecordObject( parentCanvas.GetComponent<RectTransform>(), "Change Canvas Position" );
							parentCanvas.GetComponent<RectTransform>().position = parentCanvasPosition;
						}

						EditorGUI.BeginChangeCheck();
						parentCanvasSizeDelta = EditorGUILayout.Vector2Field( new GUIContent( "Canvas Size Delta", "The size delta of the canvas rect transform." ), parentCanvasSizeDelta );
						if( EditorGUI.EndChangeCheck() )
						{
							Undo.RecordObject( parentCanvas.GetComponent<RectTransform>(), "Change Canvas Size Delta" );
							parentCanvas.GetComponent<RectTransform>().sizeDelta = parentCanvasSizeDelta;
						}

						EditorGUI.BeginChangeCheck();
						parentCanvasRotation = EditorGUILayout.Vector3Field( new GUIContent( "Canvas Rotation", "The rotation of the canvas rect transform." ), parentCanvasRotation );
						if( EditorGUI.EndChangeCheck() )
						{
							Undo.RecordObject( parentCanvas.GetComponent<RectTransform>(), "Change Canvas Rotation" );
							parentCanvas.GetComponent<RectTransform>().rotation = Quaternion.Euler( parentCanvasRotation );
						}
					}
					GUILayout.Space( 1 );
					EditorGUILayout.EndVertical();
				}

				if( parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera )
				{
					EditorGUILayout.BeginVertical( "Box" );
					if( DisplayCollapsibleBoxSection( "Screen Space Camera Options", "USB_ScreenSpaceCameraOptions" ) )
					{
						EditorGUI.BeginChangeCheck();
						if( targ.BaseTransform != null )
							xPivot = targ.BaseTransform.pivot.x;
						xPivot = EditorGUILayout.Slider( new GUIContent( "Image Pivot X", "The X Pivot of the RectTransform." ), xPivot, 0.0f, 1.0f );
						if( EditorGUI.EndChangeCheck() )
						{
							Undo.RecordObject( targ.BaseTransform, "Update RectTransform Pivot" );
							targ.BaseTransform.pivot = new Vector2( xPivot, targ.BaseTransform.pivot.y );
						}

						EditorGUI.BeginChangeCheck();
						if( targ.BaseTransform != null )
							yPivot = targ.BaseTransform.pivot.y;
						yPivot = EditorGUILayout.Slider( new GUIContent( "Image Pivot Y", "The Y Pivot of the RectTransform." ), yPivot, 0.0f, 1.0f );
						if( EditorGUI.EndChangeCheck() )
						{
							Undo.RecordObject( targ.BaseTransform, "Update RectTransform Pivot" );
							targ.BaseTransform.pivot = new Vector2( targ.BaseTransform.pivot.x, yPivot );
						}

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.Slider( depthPosition, -5.0f, 5.0f, new GUIContent( "Depth Position", "The depth position of the status bar in relation to the canvas." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
					}
					GUILayout.Space( 1 );
					EditorGUILayout.EndVertical();
				}
			}
		}
		
		if( DisplayHeaderDropdown( "Status Information", "USB_StatusInformation" ) )
		{
			if( targ.UltimateStatusList.Count > 0 )
			{
				for( int i = 0; i < targ.UltimateStatusList.Count; i++ )
				{
					EditorGUILayout.BeginVertical( "Box" );
					GUILayout.Space( 1 );

					// STATUS NAME //
					if( targ.UltimateStatusList.Count > 1 )
					{
						statusInformationStyle.alignment = TextAnchor.MiddleLeft;
						string statusColorHex = ColorUtility.ToHtmlStringRGBA( targ.UltimateStatusList[ i ].colorMode == UltimateStatusBar.UltimateStatus.ColorMode.Gradient ? targ.UltimateStatusList[ i ].statusGradient.Evaluate( testValue[ i ] / 100 ) : targ.UltimateStatusList[ i ].color );
						EditorGUILayout.BeginHorizontal();
						if( GUILayout.Button( EditorPrefs.GetBool( "USB_StatusAdvanced_" + i.ToString( "00" ) ) ? "▼" : "►", statusInformationStyle, GUILayout.Width( 32 ) ) )
						{
							EditorPrefs.SetBool( "USB_StatusAdvanced_" + i.ToString( "00" ), !EditorPrefs.GetBool( "USB_StatusAdvanced_" + i.ToString( "00" ) ) );
							ClearFocusControl();
						}
						GUILayout.Space( -20 );
						EditorGUILayout.LabelField( "<color=#" + statusColorHex + ">█</color> ", statusInformationStyle, GUILayout.Width( 12 ) );
						if( statusName[ i ].stringValue == string.Empty && Event.current.type == EventType.Repaint )
						{
							GUIStyle style = new GUIStyle( GUI.skin.textField );
							style.normal.textColor = new Color( 0.5f, 0.5f, 0.5f, 0.75f );
							EditorGUILayout.TextField( GUIContent.none, "Status Name", style );
						}
						else
						{
							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( statusName[ i ], GUIContent.none );
							if( EditorGUI.EndChangeCheck() )
							{
								serializedObject.ApplyModifiedProperties();
								StoreNameList();
							}
						}
						statusInformationStyle.alignment = TextAnchor.MiddleRight;
						if( GUILayout.Button( new GUIContent( "X", "Delete this status information." ), statusInformationStyle, GUILayout.Width( 12 ) ) )
						{
							if( EditorUtility.DisplayDialog( "Ultimate Status Bar", "Warning!\n\nAre you sure that you want to delete " + ( statusName[ i ].stringValue != string.Empty ? "the " + statusName[ i ].stringValue : "this" ) + " status?", "Yes", "No" ) )
							{
								if( targ.UltimateStatusList[ i ].statusImage != null || targ.UltimateStatusList[ i ].statusText != null || targ.UltimateStatusList[ i ].dramaticImage != null )
								{
									if( EditorUtility.DisplayDialog( "Ultimate Status Bar", "Would you like to delete the associated image " + ( targ.UltimateStatusList[ i ].statusText != null ? "and text " : "" ) + "objects that are assigned for this status?", "Yes", "No" ) )
									{
										if( targ.UltimateStatusList[ i ].statusImage != null )
											Undo.DestroyObjectImmediate( targ.UltimateStatusList[ i ].statusImage.gameObject );

										if( targ.UltimateStatusList[ i ].statusText != null )
											Undo.DestroyObjectImmediate( targ.UltimateStatusList[ i ].statusText.gameObject );

										if( targ.UltimateStatusList[ i ].dramaticImage != null )
											Undo.DestroyObjectImmediate( targ.UltimateStatusList[ i ].dramaticImage.gameObject );
									}
								}
								RemoveStatus( i );
								continue;
							}
						}
						EditorGUILayout.EndHorizontal();
						
						if( statusName[ i ].stringValue == string.Empty && targ.UltimateStatusList.Count > 1 )
						{
							GUIStyle noteStyle = new GUIStyle( EditorStyles.miniLabel ) { alignment = TextAnchor.MiddleLeft, richText = true, wordWrap = true };
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField( "            <color=red>▲</color> Status Name must be assigned to reference this particular status.", noteStyle );
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.Space();
						}
						else if( DuplicateStatusName( i ) )
						{
							GUIStyle noteStyle = new GUIStyle( EditorStyles.miniLabel ) { alignment = TextAnchor.MiddleLeft, richText = true, wordWrap = true };
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField( "            <color=red>▲</color> Status Name is already in use.", noteStyle );
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.Space();
						}
					}

					// TEST VALUE //
					EditorGUI.BeginChangeCheck();
					testValue[ i ] = EditorGUILayout.Slider( new GUIContent( "Test Value" ), testValue[ i ], 0.0f, 100.0f );
					if( EditorGUI.EndChangeCheck() )
					{
						if( targ.UltimateStatusList[ i ].statusImage != null )
						{
							Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Status Bar Test Value" );

							if( targ.UltimateStatusList[ i ].displayText != UltimateStatusBar.UltimateStatus.DisplayText.Disabled && targ.UltimateStatusList[ i ].statusText != null )
								Undo.RecordObject( targ.UltimateStatusList[ i ].statusText, "Status Bar Test Value" );

							targ.UltimateStatusList[ i ].statusImage.enabled = false;
							targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
							targ.UltimateStatusList[ i ].statusImage.enabled = true;
						}
					}

					if( targ.UltimateStatusList.Count == 1 || EditorPrefs.GetBool( "USB_StatusAdvanced_" + i.ToString( "00" ) ) )
					{
						// STATUS IMAGE //
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( statusImage[ i ], new GUIContent( "Status Image", "The image component to be used for this status." ) );
						if( EditorGUI.EndChangeCheck() )
						{
							serializedObject.ApplyModifiedProperties();

							if( targ.UltimateStatusList[ i ].statusImage != null )
							{
								Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Assign Status Image" );

								if( targ.UltimateStatusList[ i ].statusImage != null && targ.UltimateStatusList[ i ].statusImage.type != Image.Type.Filled )
								{
									targ.UltimateStatusList[ i ].statusImage.type = Image.Type.Filled;
									targ.UltimateStatusList[ i ].statusImage.fillMethod = Image.FillMethod.Horizontal;
								}

								targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
							}
						}

						if( targ.UltimateStatusList[ i ].statusImage == null )
						{
							EditorGUILayout.BeginVertical( "Box" );
							EditorGUILayout.HelpBox( "Status Image is unassigned.", MessageType.Warning );

							if( GUILayout.Button( "Generate Image", EditorStyles.miniButton ) )
							{
								GameObject newGameObject = new GameObject();
								RectTransform trans = newGameObject.AddComponent<RectTransform>();
								newGameObject.AddComponent<CanvasRenderer>();
								Image img = newGameObject.AddComponent<Image>();

								newGameObject.name = "Status_" + i.ToString( "00" );

								newGameObject.transform.SetParent( targ.transform );

								if( i == 0 )
									newGameObject.transform.SetAsFirstSibling();
								else if( targ.UltimateStatusList[ i - 1 ].statusImage != null )
									newGameObject.transform.SetSiblingIndex( targ.UltimateStatusList[ i - 1 ].statusImage.transform.GetSiblingIndex() + 1 );
								else
									newGameObject.transform.SetAsLastSibling();

								trans.anchorMin = Vector2.zero;
								trans.anchorMax = Vector2.one;
								trans.pivot = new Vector2( 0.5f, 0.5f );
								trans.localScale = Vector3.one;
								trans.localRotation = Quaternion.identity;
								trans.anchoredPosition3D = Vector3.zero;
								trans.offsetMax = Vector2.zero;
								trans.offsetMin = Vector2.zero;

								img.type = Image.Type.Filled;
								img.fillMethod = Image.FillMethod.Horizontal;
								img.color = Color.white;

								statusImage[ i ].objectReferenceValue = img;
								serializedObject.ApplyModifiedProperties();

								Undo.RegisterCreatedObjectUndo( newGameObject, "Create Status Image Object" );

								EditorPrefs.SetBool( "USB_ImageOptions_" + i.ToString( "00" ), true );
							}
							EditorGUILayout.EndVertical();
						}
						else
						{
							// IMAGE OPTIONS //
							EditorGUI.BeginChangeCheck();
							GUILayout.Toggle( EditorPrefs.GetBool( "USB_ImageOptions_" + i.ToString( "00" ) ), "Image Options", EditorStyles.foldout );
							if( EditorGUI.EndChangeCheck() )
								EditorPrefs.SetBool( "USB_ImageOptions_" + i.ToString( "00" ), !EditorPrefs.GetBool( "USB_ImageOptions_" + i.ToString( "00" ) ) );

							if( EditorPrefs.GetBool( "USB_ImageOptions_" + i.ToString( "00" ) ) )
							{
								EditorGUI.indentLevel++;

								statusImageSprite[ i ] = targ.UltimateStatusList[ i ].statusImage.sprite;
								EditorGUI.BeginChangeCheck();
								statusImageSprite[ i ] = ( Sprite )EditorGUILayout.ObjectField( "Status Sprite", statusImageSprite[ i ], typeof( Sprite ), false, GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
								if( EditorGUI.EndChangeCheck() )
								{
									Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Update Status Sprite" );
									targ.UltimateStatusList[ i ].statusImage.sprite = statusImageSprite[ i ];

									if( targ.UltimateStatusList[ i ].dramaticImage != null )
									{
										Undo.RecordObject( targ.UltimateStatusList[ i ].dramaticImage, "Update Status Sprite" );
										targ.UltimateStatusList[ i ].dramaticImage.sprite = statusImageSprite[ i ];
									}
								}

								fillMethod[ i ] = targ.UltimateStatusList[ i ].statusImage.fillMethod;
								EditorGUI.BeginChangeCheck();
								fillMethod[ i ] = ( Image.FillMethod )EditorGUILayout.EnumPopup( "Fill Method", fillMethod[ i ] );
								if( EditorGUI.EndChangeCheck() )
								{
									Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Update Fill Method" );

									if( targ.UltimateStatusList[ i ].statusImage != null )
									{
										targ.UltimateStatusList[ i ].statusImage.fillMethod = fillMethod[ i ];

										if( targ.UltimateStatusList[ i ].dramaticImage != null )
										{
											Undo.RecordObject( targ.UltimateStatusList[ i ].dramaticImage, "Update Fill Method" );
											targ.UltimateStatusList[ i ].dramaticImage.fillMethod = fillMethod[ i ];
										}
									}

									targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
								}

								switch( targ.UltimateStatusList[ i ].statusImage.fillMethod )
								{
									default:
									case Image.FillMethod.Horizontal:
									{
										fillMethodHorizontal[ i ] = ( Image.OriginHorizontal )targ.UltimateStatusList[ i ].statusImage.fillOrigin;
										EditorGUI.BeginChangeCheck();
										fillMethodHorizontal[ i ] = ( Image.OriginHorizontal )EditorGUILayout.EnumPopup( "Fill Origin", fillMethodHorizontal[ i ] );
										if( EditorGUI.EndChangeCheck() )
										{
											serializedObject.ApplyModifiedProperties();
											if( targ.UltimateStatusList[ i ].statusImage != null )
											{
												Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Update Fill Origin" );
												targ.UltimateStatusList[ i ].statusImage.fillOrigin = ( int )fillMethodHorizontal[ i ];

												if( targ.UltimateStatusList[ i ].dramaticImage != null )
												{
													Undo.RecordObject( targ.UltimateStatusList[ i ].dramaticImage, "Update Fill Origin" );
													targ.UltimateStatusList[ i ].dramaticImage.fillOrigin = ( int )fillMethodHorizontal[ i ];
												}
											}
											targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
										}
									}
									break;
									case Image.FillMethod.Vertical:
									{
										fillMethodVertical[ i ] = ( Image.OriginVertical )targ.UltimateStatusList[ i ].statusImage.fillOrigin;
										EditorGUI.BeginChangeCheck();
										fillMethodVertical[ i ] = ( Image.OriginVertical )EditorGUILayout.EnumPopup( "Fill Origin", fillMethodVertical[ i ] );
										if( EditorGUI.EndChangeCheck() )
										{
											serializedObject.ApplyModifiedProperties();
											if( targ.UltimateStatusList[ i ].statusImage != null )
											{
												Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Update Fill Origin" );
												targ.UltimateStatusList[ i ].statusImage.fillOrigin = ( int )fillMethodVertical[ i ];

												if( targ.UltimateStatusList[ i ].dramaticImage != null )
												{
													Undo.RecordObject( targ.UltimateStatusList[ i ].dramaticImage, "Update Fill Origin" );
													targ.UltimateStatusList[ i ].dramaticImage.fillOrigin = ( int )fillMethodVertical[ i ];
												}
											}
											targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
										}
									}
									break;
									case Image.FillMethod.Radial90:
									{
										fillMethod90[ i ] = ( Image.Origin90 )targ.UltimateStatusList[ i ].statusImage.fillOrigin;
										EditorGUI.BeginChangeCheck();
										fillMethod90[ i ] = ( Image.Origin90 )EditorGUILayout.EnumPopup( "Fill Origin", fillMethod90[ i ] );
										if( EditorGUI.EndChangeCheck() )
										{
											serializedObject.ApplyModifiedProperties();
											if( targ.UltimateStatusList[ i ].statusImage != null )
											{
												Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Update Fill Origin" );
												targ.UltimateStatusList[ i ].statusImage.fillOrigin = ( int )fillMethod90[ i ];

												if( targ.UltimateStatusList[ i ].dramaticImage != null )
												{
													Undo.RecordObject( targ.UltimateStatusList[ i ].dramaticImage, "Update Fill Origin" );
													targ.UltimateStatusList[ i ].dramaticImage.fillOrigin = ( int )fillMethod90[ i ];
												}
											}
											targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
										}
									}
									break;
									case Image.FillMethod.Radial180:
									{
										fillMethod180[ i ] = ( Image.Origin180 )targ.UltimateStatusList[ i ].statusImage.fillOrigin;
										EditorGUI.BeginChangeCheck();
										fillMethod180[ i ] = ( Image.Origin180 )EditorGUILayout.EnumPopup( "Fill Origin", fillMethod180[ i ] );
										if( EditorGUI.EndChangeCheck() )
										{
											serializedObject.ApplyModifiedProperties();
											if( targ.UltimateStatusList[ i ].statusImage != null )
											{
												Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Update Fill Origin" );
												targ.UltimateStatusList[ i ].statusImage.fillOrigin = ( int )fillMethod180[ i ];

												if( targ.UltimateStatusList[ i ].dramaticImage != null )
												{
													Undo.RecordObject( targ.UltimateStatusList[ i ].dramaticImage, "Update Fill Origin" );
													targ.UltimateStatusList[ i ].dramaticImage.fillOrigin = ( int )fillMethod180[ i ];
												}
											}
											targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
										}
									}
									break;
									case Image.FillMethod.Radial360:
									{
										fillMethod360[ i ] = ( Image.Origin360 )targ.UltimateStatusList[ i ].statusImage.fillOrigin;
										EditorGUI.BeginChangeCheck();
										fillMethod360[ i ] = ( Image.Origin360 )EditorGUILayout.EnumPopup( "Fill Origin", fillMethod360[ i ] );
										if( EditorGUI.EndChangeCheck() )
										{
											serializedObject.ApplyModifiedProperties();
											if( targ.UltimateStatusList[ i ].statusImage != null )
											{
												Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Update Fill Origin" );
												targ.UltimateStatusList[ i ].statusImage.fillOrigin = ( int )fillMethod360[ i ];

												if( targ.UltimateStatusList[ i ].dramaticImage != null )
												{
													Undo.RecordObject( targ.UltimateStatusList[ i ].dramaticImage, "Update Fill Origin" );
													targ.UltimateStatusList[ i ].dramaticImage.fillOrigin = ( int )fillMethod360[ i ];
												}
											}
											targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
										}
									}
									break;
								}

								EditorGUI.indentLevel--;
								SubsectionEnd();
							}

							// STATUS COLORS //
							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( colorMode[ i ], new GUIContent( "Color Mode", "The mode in which to display the color of the status to the image component." ) );
							if( EditorGUI.EndChangeCheck() )
							{
								serializedObject.ApplyModifiedProperties();
								UpdateStatusColor( i );
							}

							EditorGUI.indentLevel = 1;
							if( targ.UltimateStatusList[ i ].colorMode == UltimateStatusBar.UltimateStatus.ColorMode.Single )
							{
								statusColor[ i ] = targ.UltimateStatusList[ i ].statusImage.color;
								EditorGUI.BeginChangeCheck();
								statusColor[ i ] = EditorGUILayout.ColorField( new GUIContent( "Status Color", "The color of this status image." ), statusColor[ i ] );
								if( EditorGUI.EndChangeCheck() )
									UpdateStatusColor( i );
							}
							else
							{
								
								EditorGUI.BeginChangeCheck();
								EditorGUILayout.PropertyField( statusGradient[ i ], new GUIContent( "Status Gradient", "The color gradient of this status image." ) );
								if( EditorGUI.EndChangeCheck() )
								{
									serializedObject.ApplyModifiedProperties();
									UpdateStatusColor( i );
								}
							}
							EditorGUI.indentLevel = 0;
							SpaceAfterIndent();

							// FILL CONSTRAINT //
							EditorGUILayout.BeginHorizontal();
							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( fillConstraint[ i ], new GUIContent( "Fill Constraint", "Determines whether or not the image fill should be constrained." ) );
							if( EditorGUI.EndChangeCheck() )
							{
								serializedObject.ApplyModifiedProperties();

								if( targ.UltimateStatusList[ i ].fillConstraint )
									EditorPrefs.SetBool( "USB_FillConstraint_" + i.ToString( "00" ), true );

								if( targ.UltimateStatusList[ i ].statusImage != null )
								{
									Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Enable Fill Constraint" );
									targ.UltimateStatusList[ i ].statusImage.enabled = false;
									targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
									targ.UltimateStatusList[ i ].statusImage.enabled = true;
								}
							}

							if( targ.UltimateStatusList[ i ].fillConstraint )
								DisplayPropertyMinimizeButton( "USB_FillConstraint_" + i.ToString( "00" ) );

							EditorGUILayout.EndHorizontal();

							if( targ.UltimateStatusList[ i ].fillConstraint && EditorPrefs.GetBool( "USB_FillConstraint_" + i.ToString( "00" ) ) )
							{
								EditorGUI.indentLevel = 1;

								EditorGUI.BeginChangeCheck();
								EditorGUILayout.Slider( fillConstraintMin[ i ], 0.0f, targ.UltimateStatusList[ i ].fillConstraintMax - 0.01f, new GUIContent( "Fill Minimum", "The minimum fill amount." ) );
								EditorGUILayout.Slider( fillConstraintMax[ i ], targ.UltimateStatusList[ i ].fillConstraintMin + 0.01f, 1.0f, new GUIContent( "Fill Maximum", "The maximum fill amount." ) );
								if( EditorGUI.EndChangeCheck() )
								{
									serializedObject.ApplyModifiedProperties();
									if( targ.UltimateStatusList[ i ].statusImage != null )
									{
										Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Update Fill Constraint" );
										targ.UltimateStatusList[ i ].statusImage.enabled = false;
										targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
										targ.UltimateStatusList[ i ].statusImage.enabled = true;
									}
								}

								EditorGUI.BeginChangeCheck();
								EditorGUILayout.PropertyField( fillConstraintTicks[ i ], new GUIContent( "Fill Ticks", "The amount of ticks in the status image." ) );
								if( EditorGUI.EndChangeCheck() )
								{
									if( fillConstraintTicks[ i ].intValue < 0 )
										fillConstraintTicks[ i ].intValue = 0;

									serializedObject.ApplyModifiedProperties();

									if( targ.UltimateStatusList[ i ].statusImage != null )
									{
										Undo.RecordObject( targ.UltimateStatusList[ i ].statusImage, "Update Fill Constraint" );
										if( targ.UltimateStatusList[ i ].displayText != UltimateStatusBar.UltimateStatus.DisplayText.Disabled && targ.UltimateStatusList[ i ].statusText != null )
											Undo.RecordObject( targ.UltimateStatusList[ i ].statusText, "Update Fill Constraint" );

										targ.UltimateStatusList[ i ].statusImage.enabled = false;
										targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
										targ.UltimateStatusList[ i ].statusImage.enabled = true;
									}
								}

								EditorGUI.indentLevel = 0;
								EditorGUILayout.Space();
							}

							// TEXT OPTIONS //
							EditorGUILayout.BeginHorizontal();
							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( displayText[ i ], new GUIContent( "Display Text", "Determines how this status will display text to the user." ) );
							if( EditorGUI.EndChangeCheck() )
							{
								if( targ.UltimateStatusList[ i ].displayText == UltimateStatusBar.UltimateStatus.DisplayText.Disabled && displayText[ i ].enumValueIndex > 0 )
									EditorPrefs.SetBool( "USB_DisplayText_" + i.ToString( "00" ), true );
								
								serializedObject.ApplyModifiedProperties();

								if( targ.UltimateStatusList[ i ].displayText == UltimateStatusBar.UltimateStatus.DisplayText.Disabled )
								{
									if( targ.UltimateStatusList[ i ].statusText != null )
									{
										Undo.RecordObject( targ.UltimateStatusList[ i ].statusText.gameObject, "Disable Status Text" );
										targ.UltimateStatusList[ i ].statusText.gameObject.SetActive( false );
									}
								}
								else
								{
									if( targ.UltimateStatusList[ i ].statusText != null && !targ.UltimateStatusList[ i ].statusText.gameObject.activeInHierarchy )
									{
										Undo.RecordObject( targ.UltimateStatusList[ i ].statusText.gameObject, "Enable Status Text" );
										targ.UltimateStatusList[ i ].statusText.gameObject.SetActive( true );
									}
								}
								targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
							}

							if( targ.UltimateStatusList[ i ].displayText != UltimateStatusBar.UltimateStatus.DisplayText.Disabled && targ.UltimateStatusList[ i ].statusText != null )
								DisplayPropertyMinimizeButton( "USB_DisplayText_" + i.ToString( "00" ) );

							EditorGUILayout.EndHorizontal();

							if( targ.UltimateStatusList[ i ].displayText != UltimateStatusBar.UltimateStatus.DisplayText.Disabled && EditorPrefs.GetBool( "USB_DisplayText_" + i.ToString( "00" ) ) )
							{
								EditorGUI.indentLevel++;

								EditorGUI.BeginChangeCheck();
								EditorGUILayout.PropertyField( statusText[ i ], new GUIContent( "Status Text", "The Text component to be used for the status text." ) );
								if( EditorGUI.EndChangeCheck() )
								{
									serializedObject.ApplyModifiedProperties();

									if( targ.UltimateStatusList[ i ].statusText != null )
									{
										Undo.RecordObject( targ.UltimateStatusList[ i ].statusText, "Assign Status Text" );
										targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
									}
								}

								EditorGUI.indentLevel--;

								if( targ.UltimateStatusList[ i ].statusText == null )
								{
									EditorGUILayout.BeginVertical( "Box" );
									EditorGUILayout.HelpBox( "Status Text is unassigned.", MessageType.Warning );

									if( GUILayout.Button( "Generate Text", EditorStyles.miniButton ) )
									{
										GameObject newGameObject = new GameObject();
										RectTransform trans = newGameObject.AddComponent<RectTransform>();
										newGameObject.AddComponent<CanvasRenderer>();
										Text txt = newGameObject.AddComponent<Text>();

										newGameObject.name = "Status_" + i.ToString( "00" ) + "_Text";

										newGameObject.transform.SetParent( targ.UltimateStatusList[ i ].statusImage.transform );
										
										trans.anchorMin = Vector2.zero;
										trans.anchorMax = Vector2.one;
										trans.pivot = new Vector2( 0.5f, 0.5f );
										trans.localScale = Vector3.one;
										trans.localRotation = Quaternion.identity;
										trans.anchoredPosition3D = Vector3.zero;
										trans.offsetMax = Vector2.zero;
										trans.offsetMin = Vector2.zero;

										txt.color = Color.white;
										txt.resizeTextForBestFit = true;
										txt.resizeTextMinSize = 0;
										txt.resizeTextMaxSize = 300;
										txt.alignment = TextAnchor.MiddleCenter;
										txt.raycastTarget = false;

										statusText[ i ].objectReferenceValue = txt;
										serializedObject.ApplyModifiedProperties();

										Undo.RegisterCreatedObjectUndo( newGameObject, "Create Status Text Object" );

										targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100 );

										EditorPrefs.SetBool( "USB_TextSettings_" + i.ToString( "00" ), true );
									}
									EditorGUILayout.EndVertical();
								}
								else
								{
									EditorGUI.indentLevel++;

									EditorGUILayout.BeginHorizontal();
									GUILayout.Space( 16 );
									EditorGUI.BeginChangeCheck();
									GUILayout.Toggle( EditorPrefs.GetBool( "USB_TextSettings_" + i.ToString( "00" ) ), "Text Settings", EditorStyles.foldout );
									if( EditorGUI.EndChangeCheck() )
										EditorPrefs.SetBool( "USB_TextSettings_" + i.ToString( "00" ), !EditorPrefs.GetBool( "USB_TextSettings_" + i.ToString( "00" ) ) );
									EditorGUILayout.EndHorizontal();

									if( EditorPrefs.GetBool( "USB_TextSettings_" + i.ToString( "00" ) ) )
									{
										EditorGUI.indentLevel++;

										Vector2 anchor = Vector2.zero;

										anchor = targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMin;
										EditorGUI.BeginChangeCheck();
										anchor = EditorGUILayout.Vector2Field( "Anchor Min", anchor );
										if( EditorGUI.EndChangeCheck() )
										{
											if( anchor.x < 0 )
												anchor.x = 0;
											else if( anchor.x > targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMax.x )
												anchor.x = targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMax.x;
											else if( anchor.x > 1 )
												anchor.x = 1;

											if( anchor.y < 0 )
												anchor.y = 0;
											else if( anchor.y > targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMax.y )
												anchor.y = targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMax.y;
											else if( anchor.y > 1 )
												anchor.y = 1;

											Undo.RecordObject( targ.UltimateStatusList[ i ].statusText.rectTransform, "Update Text Anchor" );
											targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMin = anchor;
										}

										anchor = targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMax;
										EditorGUI.BeginChangeCheck();
										anchor = EditorGUILayout.Vector2Field( "Anchor Max", anchor );
										if( EditorGUI.EndChangeCheck() )
										{
											if( anchor.x < 0 )
												anchor.x = 0;
											else if( anchor.x < targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMin.x )
												anchor.x = targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMin.x;
											else if( anchor.x > 1 )
												anchor.x = 1;

											if( anchor.y < 0 )
												anchor.y = 0;
											else if( anchor.y < targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMin.y )
												anchor.y = targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMin.y;
											else if( anchor.y > 1 )
												anchor.y = 1;

											Undo.RecordObject( targ.UltimateStatusList[ i ].statusText.rectTransform, "Update Text Anchor" );
											targ.UltimateStatusList[ i ].statusText.rectTransform.anchorMax = anchor;
										}

										TextAnchor textAnchor = targ.UltimateStatusList[ i ].statusText.alignment;
										EditorGUI.BeginChangeCheck();
										textAnchor = ( TextAnchor )EditorGUILayout.EnumPopup( "Text Anchor", textAnchor );
										if( EditorGUI.EndChangeCheck() )
										{
											Undo.RecordObject( targ.UltimateStatusList[ i ].statusText, "Update Text Anchor" );
											targ.UltimateStatusList[ i ].statusText.alignment = textAnchor;
										}

										EditorGUI.BeginChangeCheck();
										useStatusTextOutline[ i ] = EditorGUILayout.Toggle( "Text Outline", useStatusTextOutline[ i ] );
										if( EditorGUI.EndChangeCheck() )
										{
											if( useStatusTextOutline[ i ] )
											{
												if( targ.UltimateStatusList[ i ].statusText.GetComponent<Outline>() )
												{
													Undo.RecordObject( targ.UltimateStatusList[ i ].statusText.GetComponent<Outline>(), "Enable Status Text Outline" );
													targ.UltimateStatusList[ i ].statusText.GetComponent<Outline>().enabled = true;
												}
												else
													Undo.AddComponent( targ.UltimateStatusList[ i ].statusText.gameObject, typeof( Outline ) );
											}
											else
											{
												if( targ.UltimateStatusList[ i ].statusText.GetComponent<Outline>() )
												{
													Undo.RecordObject( targ.UltimateStatusList[ i ].statusText.GetComponent<Outline>(), "Disable Status Text Outline" );
													targ.UltimateStatusList[ i ].statusText.GetComponent<Outline>().enabled = false;
												}
											}
										}

										if( useStatusTextOutline[ i ] )
										{
											EditorGUI.indentLevel++;
											statusTextColorOutline[ i ] = targ.UltimateStatusList[ i ].statusText.GetComponent<Outline>().effectColor;
											EditorGUI.BeginChangeCheck();
											statusTextColorOutline[ i ] = EditorGUILayout.ColorField( new GUIContent( "Outline Color", "The color of the Outline component." ), statusTextColorOutline[ i ] );
											if( EditorGUI.EndChangeCheck() )
											{
												Undo.RecordObject( targ.UltimateStatusList[ i ].statusText.GetComponent<Outline>(), "Update Status Text Outline Color" );
												targ.UltimateStatusList[ i ].statusText.GetComponent<Outline>().effectColor = statusTextColorOutline[ i ];
											}
											EditorGUI.indentLevel--;
										}

										EditorGUI.BeginChangeCheck();
										statusTextFont[ i ] = ( Font )EditorGUILayout.ObjectField( "Text Font", statusTextFont[ i ], typeof( Font ), false );
										if( EditorGUI.EndChangeCheck() )
										{
											if( targ.UltimateStatusList[ i ].statusText != null )
											{
												Undo.RecordObject( targ.UltimateStatusList[ i ].statusText, "Update Status Text Font" );
												targ.UltimateStatusList[ i ].statusText.font = statusTextFont[ i ];
											}
										}

										EditorGUI.indentLevel--;

										SubsectionEnd();
									}

									statusTextColor[ i ] = targ.UltimateStatusList[ i ].statusText == null ? Color.white : targ.UltimateStatusList[ i ].statusText.color;
									EditorGUI.BeginChangeCheck();
									statusTextColor[ i ] = EditorGUILayout.ColorField( new GUIContent( "Text Color", "The color of the Text component." ), statusTextColor[ i ] );
									if( EditorGUI.EndChangeCheck() )
									{
										Undo.RecordObject( targ.UltimateStatusList[ i ].statusText, "Update Status Text Color" );
										targ.UltimateStatusList[ i ].statusText.color = statusTextColor[ i ];
									}

									EditorGUI.BeginChangeCheck();
									EditorGUILayout.PropertyField( additionalText[ i ], new GUIContent( "Additional Text", "Additional text to be displayed before the current status information." ) );
									if( EditorGUI.EndChangeCheck() )
									{
										serializedObject.ApplyModifiedProperties();
										
										Undo.RecordObject( targ.UltimateStatusList[ i ].statusText, "Update Additional Text" );
										targ.UltimateStatusList[ i ].UpdateStatus( testValue[ i ], 100.0f );
									}

									EditorGUI.indentLevel--;
								}

								EditorGUILayout.Space();
							}

							// SMOOTH FILL //
							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( smoothFill[ i ], new GUIContent( "Smooth Fill", "Determines if the status should smoothly transition from it's current value." ) );
							if( EditorGUI.EndChangeCheck() )
								serializedObject.ApplyModifiedProperties();

							if( targ.UltimateStatusList[ i ].smoothFill )
							{
								EditorGUI.indentLevel = 1;

								EditorGUI.BeginChangeCheck();
								EditorGUILayout.PropertyField( smoothFillSpeed[ i ], new GUIContent( "Fill Speed", "The speed to reach the target fill amount." ) );
								if( EditorGUI.EndChangeCheck() )
								{
									if( smoothFillSpeed[ i ].floatValue < 0 )
										smoothFillSpeed[ i ].floatValue = 0;

									serializedObject.ApplyModifiedProperties();
								}

								EditorGUI.indentLevel = 0;
								EditorGUILayout.Space();
							}
							
							// DRAMATIC FILL //
							EditorGUILayout.BeginHorizontal();

							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( useDramaticFill[ i ], new GUIContent( "Dramatic Fill", "Determines the use of a separate image to show behind the status to display how much the value has changed." ) );
							if( EditorGUI.EndChangeCheck() )
							{
								serializedObject.ApplyModifiedProperties();

								if( targ.UltimateStatusList[ i ].useDramaticFill )
								{
									EditorPrefs.SetBool( "USB_DramaticFill_" + i.ToString( "00" ), true );

									if( targ.UltimateStatusList[ i ].dramaticImage != null && !targ.UltimateStatusList[ i ].dramaticImage.gameObject.activeInHierarchy )
									{
										Undo.RecordObject( targ.UltimateStatusList[ i ].dramaticImage.gameObject, "Enable Dramatic Fill" );
										targ.UltimateStatusList[ i ].dramaticImage.gameObject.SetActive( true );
									}
								}
								else
								{
									if( targ.UltimateStatusList[ i ].dramaticImage != null && targ.UltimateStatusList[ i ].dramaticImage.gameObject.activeInHierarchy )
									{
										Undo.RecordObject( targ.UltimateStatusList[ i ].dramaticImage.gameObject, "Disable Dramatic Fill" );
										targ.UltimateStatusList[ i ].dramaticImage.gameObject.SetActive( false );
									}
								}
							}

							if( targ.UltimateStatusList[ i ].useDramaticFill && targ.UltimateStatusList[ i ].dramaticImage != null )
								DisplayPropertyMinimizeButton( "USB_DramaticFill_" + i.ToString( "00" ) );

							EditorGUILayout.EndHorizontal();

							if( targ.UltimateStatusList[ i ].useDramaticFill && ( EditorPrefs.GetBool( "USB_DramaticFill_" + i.ToString( "00" ) ) || targ.UltimateStatusList[ i ].dramaticImage == null ) )
							{
								EditorGUI.indentLevel++;
								EditorGUI.BeginChangeCheck();
								EditorGUILayout.PropertyField( dramaticImage[ i ], new GUIContent( "Dramatic Image", "The image component to use as the dramatic fill for this status." ) );
								if( EditorGUI.EndChangeCheck() )
								{
									serializedObject.ApplyModifiedProperties();

									if( targ.UltimateStatusList[ i ].dramaticImage != null )
									{
										Undo.RecordObject( targ.UltimateStatusList[ i ].dramaticImage, "Assign Dramatic Image" );
										targ.UltimateStatusList[ i ].dramaticImage.type = Image.Type.Filled;
										targ.UltimateStatusList[ i ].dramaticImage.fillMethod = targ.UltimateStatusList[ i ].statusImage.fillMethod;
									}
								}
								EditorGUI.indentLevel--;

								if( targ.UltimateStatusList[ i ].dramaticImage == null )
								{
									EditorGUILayout.BeginVertical( "Box" );
									EditorGUILayout.HelpBox( "Dramatic Image is unassigned.", MessageType.Warning );
									
									if( GUILayout.Button( "Generate Image", EditorStyles.miniButton ) )
									{
										GameObject newGameObject = new GameObject();
										RectTransform trans = newGameObject.AddComponent<RectTransform>();
										newGameObject.AddComponent<CanvasRenderer>();
										Image img = newGameObject.AddComponent<Image>();

										newGameObject.name = "Status_" + i.ToString( "00" ) + "_Dramatic";

										newGameObject.transform.SetParent( targ.transform );
										newGameObject.transform.SetSiblingIndex( targ.UltimateStatusList[ i ].statusImage.transform.GetSiblingIndex() );

										trans.anchorMin = Vector2.zero;
										trans.anchorMax = Vector2.one;
										trans.pivot = new Vector2( 0.5f, 0.5f );
										trans.localScale = targ.UltimateStatusList[ i ].statusImage.transform.localScale;
										trans.localRotation = Quaternion.identity;
										trans.anchoredPosition3D = Vector3.zero;
										trans.offsetMax = Vector2.zero;
										trans.offsetMin = Vector2.zero;

										if( targ.UltimateStatusList[ i ].statusImage.sprite != null )
											img.sprite = targ.UltimateStatusList[ i ].statusImage.sprite;
										
										img.type = Image.Type.Filled;
										img.fillMethod = targ.UltimateStatusList[ i ].statusImage.fillMethod;
										img.fillOrigin = targ.UltimateStatusList[ i ].statusImage.fillOrigin;
										img.color = Color.white;

										dramaticImage[ i ].objectReferenceValue = img;
										serializedObject.ApplyModifiedProperties();

										Undo.RegisterCreatedObjectUndo( newGameObject, "Create Dramatic Fill Object" );
									}
									EditorGUILayout.EndVertical();
								}
								else
								{
									EditorGUI.indentLevel++;

									dramaticImageColor[ i ] = targ.UltimateStatusList[ i ].dramaticImage.color;
									EditorGUI.BeginChangeCheck();
									dramaticImageColor[ i ] = EditorGUILayout.ColorField( new GUIContent( "Image Color", "The color of this dramatic image." ), dramaticImageColor[ i ] );
									if( EditorGUI.EndChangeCheck() )
									{
										Undo.RecordObject( targ.UltimateStatusList[ i ].dramaticImage, "Update Dramatic Color" );
										targ.UltimateStatusList[ i ].dramaticImage.color = dramaticImageColor[ i ];
									}

									EditorGUI.BeginChangeCheck();
									EditorGUILayout.PropertyField( dramaticStyle[ i ], new GUIContent( "Style", "Determines when the dramatic image will be displayed to bring attention to the change in value." ) );
									if( EditorGUI.EndChangeCheck() )
										serializedObject.ApplyModifiedProperties();

									if( targ.UltimateStatusList[ i ].dramaticStyle == UltimateStatusBar.UltimateStatus.DramaticStyle.Decrease )
									{
										EditorGUI.BeginChangeCheck();
										EditorGUILayout.PropertyField( fillSpeed[ i ], new GUIContent( "Fill Speed", "The speed in which the dramatic fill will move towards it's target." ) );
										if( EditorGUI.EndChangeCheck() )
										{
											if( fillSpeed[ i ].floatValue < 0 )
												fillSpeed[ i ].floatValue = 0;
											
											serializedObject.ApplyModifiedProperties();
										}

										EditorGUI.BeginChangeCheck();
										EditorGUILayout.PropertyField( secondsDelay[ i ], new GUIContent( "Seconds Delay", "The time seconds to delay before the dramatic fill image will start decreasing." ) );
										if( EditorGUI.EndChangeCheck() )
										{
											if( secondsDelay[ i ].floatValue < 0 )
												secondsDelay[ i ].floatValue = 0;
											
											serializedObject.ApplyModifiedProperties();
										}

										if( targ.UltimateStatusList[ i ].secondsDelay > 0 )
										{
											EditorGUI.BeginChangeCheck();
											EditorGUILayout.PropertyField( resetSensitivity[ i ], new GUIContent( "Reset Sensitivity", "If the difference in fill value of the status and dramatic images is within this sensitivity value, then the Seconds Delay will be applied again." ) );
											if( EditorGUI.EndChangeCheck() )
											{
												if( resetSensitivity[ i ].floatValue < 0 )
													resetSensitivity[ i ].floatValue = 0;

												serializedObject.ApplyModifiedProperties();
											}
										}
									}
									
									EditorGUI.indentLevel--;
								}

								//if( targ.onStatusUpdated )
								//	EditorGUILayout.Space();
							}
						}
					}

					GUILayout.Space( 1 );
					EditorGUILayout.EndVertical();
				}

				EditorGUI.BeginDisabledGroup( Application.isPlaying );
				if( GUILayout.Button( "Create New Status", EditorStyles.miniButton ) )
				{
					if( targ.UltimateStatusList.Count == 1 )
						EditorPrefs.SetBool( "USB_StatusAdvanced_00", true );

					AddNewStatus( targ.UltimateStatusList.Count );
				}
				EditorGUI.EndDisabledGroup();
			}
		}

		if( DisplayHeaderDropdown( "Status Bar Options", "USB_StatusBarOptions" ) )
		{
			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Status Bar Icon", "USB_StatusBarIcon", ref useStatusBarIcon, ref valueChanged ) )
			{
				// STATUS BAR ICON //
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( statusBarIcon, new GUIContent( "Status Bar Icon", "The icon image associated with this status bar." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				if( targ.statusBarIcon == null )
				{
					EditorGUILayout.BeginVertical( "Box" );
					EditorGUILayout.HelpBox( "Status Bar Icon is unassigned.", MessageType.Warning );

					if( GUILayout.Button( "Generate Image", EditorStyles.miniButton ) )
					{
						GameObject newGameObject = new GameObject();
						RectTransform trans = newGameObject.AddComponent<RectTransform>();
						newGameObject.AddComponent<CanvasRenderer>();
						Image img = newGameObject.AddComponent<Image>();

						newGameObject.name = "Status Bar Icon";

						newGameObject.transform.SetParent( targ.transform );

						newGameObject.transform.SetAsFirstSibling();

						trans.anchorMin = Vector2.zero;
						trans.anchorMax = Vector2.one;
						trans.pivot = new Vector2( 0.5f, 0.5f );
						trans.localScale = Vector3.one;
						trans.localRotation = Quaternion.identity;
						trans.anchoredPosition3D = Vector3.zero;
						trans.offsetMax = Vector2.zero;
						trans.offsetMin = Vector2.zero;

						img.color = Color.white;

						statusBarIcon.objectReferenceValue = img;
						serializedObject.ApplyModifiedProperties();

						Undo.RegisterCreatedObjectUndo( newGameObject, "Create Status Bar Icon Object" );
					}
					EditorGUILayout.EndVertical();
				}
				else
				{
					Sprite statusBarIconSprite = targ.statusBarIcon.sprite;
					EditorGUI.BeginChangeCheck();
					statusBarIconSprite = ( Sprite )EditorGUILayout.ObjectField( "Icon Sprite", statusBarIconSprite, typeof( Sprite ), false, GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
					if( EditorGUI.EndChangeCheck() )
					{
						Undo.RecordObject( targ.statusBarIcon, "Update Icon Sprite" );
						targ.statusBarIcon.sprite = statusBarIconSprite;
					}

					EditorGUI.BeginChangeCheck();
					statusBarIconColor = EditorGUILayout.ColorField( "Icon Color", statusBarIconColor );
					if( EditorGUI.EndChangeCheck() )
					{
						Undo.RecordObject( targ.statusBarIcon, "Update Status Bar Icon Color" );
						targ.statusBarIcon.enabled = false;
						targ.statusBarIcon.color = statusBarIconColor;
						targ.statusBarIcon.enabled = true;
					}

					Vector2 anchor = Vector2.zero;

					anchor = targ.statusBarIcon.rectTransform.anchorMin;
					EditorGUI.BeginChangeCheck();
					anchor = EditorGUILayout.Vector2Field( "Anchor Min", anchor );
					if( EditorGUI.EndChangeCheck() )
					{
						if( anchor.x < 0 )
							anchor.x = 0;
						else if( anchor.x > targ.statusBarIcon.rectTransform.anchorMax.x )
							anchor.x = targ.statusBarIcon.rectTransform.anchorMax.x;
						else if( anchor.x > 1 )
							anchor.x = 1;

						if( anchor.y < 0 )
							anchor.y = 0;
						else if( anchor.y > targ.statusBarIcon.rectTransform.anchorMax.y )
							anchor.y = targ.statusBarIcon.rectTransform.anchorMax.y;
						else if( anchor.y > 1 )
							anchor.y = 1;

						Undo.RecordObject( targ.statusBarIcon.rectTransform, "Update Icon Anchor" );
						targ.statusBarIcon.rectTransform.anchorMin = anchor;
					}

					anchor = targ.statusBarIcon.rectTransform.anchorMax;
					EditorGUI.BeginChangeCheck();
					anchor = EditorGUILayout.Vector2Field( "Anchor Max", anchor );
					if( EditorGUI.EndChangeCheck() )
					{
						if( anchor.x < 0 )
							anchor.x = 0;
						else if( anchor.x < targ.statusBarIcon.rectTransform.anchorMin.x )
							anchor.x = targ.statusBarIcon.rectTransform.anchorMin.x;
						else if( anchor.x > 1 )
							anchor.x = 1;

						if( anchor.y < 0 )
							anchor.y = 0;
						else if( anchor.y < targ.statusBarIcon.rectTransform.anchorMin.y )
							anchor.y = targ.statusBarIcon.rectTransform.anchorMin.y;
						else if( anchor.y > 1 )
							anchor.y = 1;

						Undo.RecordObject( targ.statusBarIcon.rectTransform, "Update Icon Anchor" );
						targ.statusBarIcon.rectTransform.anchorMax = anchor;
					}
				}
				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();
			if( valueChanged )
			{
				if( useStatusBarIcon )
				{
					if( targ.statusBarIcon != null && !targ.statusBarIcon.gameObject.activeInHierarchy )
					{
						Undo.RecordObject( targ.statusBarIcon.gameObject, "Enable Status Bar Icon" );
						targ.statusBarIcon.gameObject.SetActive( true );
					}
				}
				else
				{
					if( targ.statusBarIcon != null && targ.statusBarIcon.gameObject.activeInHierarchy )
					{
						Undo.RecordObject( targ.statusBarIcon.gameObject, "Disable Status Bar Icon" );
						targ.statusBarIcon.gameObject.SetActive( false );
					}
				}
			}

			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Status Bar Text", "USB_StatusBarText", ref useStatusBarText, ref valueChanged ) )
			{
				// STATUS BAR TEXT //
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( statusBarText, new GUIContent( "Status Bar Text", "The text component associated with this status bar." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					tempStatusString = targ.statusBarText != null && targ.statusBarText.text != "New Text" ? targ.statusBarText.text : "";
					statusBarTextColor = targ.statusBarText != null ? targ.statusBarText.color : statusBarTextColor;
					useStatusBarTextOutline = targ.statusBarText.GetComponent<Outline>() && targ.statusBarText.GetComponent<Outline>().enabled;
					statusBarTextFont = targ.statusBarText != null ? targ.statusBarText.font : Resources.GetBuiltinResource<Font>( "Arial.ttf" );
				}

				if( targ.statusBarText == null )
				{
					EditorGUILayout.BeginVertical( "Box" );
					EditorGUILayout.HelpBox( "Status Text is unassigned.", MessageType.Warning );

					if( GUILayout.Button( "Generate Text", EditorStyles.miniButton ) )
					{
						GameObject newGameObject = new GameObject();
						RectTransform trans = newGameObject.AddComponent<RectTransform>();
						newGameObject.AddComponent<CanvasRenderer>();
						Text txt = newGameObject.AddComponent<Text>();

						newGameObject.name = "Status Bar Text";

						newGameObject.transform.SetParent( targ.transform );

						newGameObject.transform.SetAsFirstSibling();

						trans.anchorMin = Vector2.zero;
						trans.anchorMax = Vector2.one;
						trans.pivot = new Vector2( 0.5f, 0.5f );
						trans.localScale = Vector3.one;
						trans.localRotation = Quaternion.identity;
						trans.anchoredPosition3D = Vector3.zero;
						trans.offsetMax = Vector2.zero;
						trans.offsetMin = Vector2.zero;

						txt.color = Color.white;
						txt.resizeTextForBestFit = true;
						txt.resizeTextMinSize = 0;
						txt.resizeTextMaxSize = 300;
						txt.alignment = TextAnchor.MiddleCenter;
						txt.raycastTarget = false;
						txt.text = "New Text";

						statusBarText.objectReferenceValue = txt;
						serializedObject.ApplyModifiedProperties();

						Undo.RegisterCreatedObjectUndo( newGameObject, "Create Status Text Object" );
					}
					EditorGUILayout.EndVertical();
				}
				else
				{
					tempStatusString = targ.statusBarText.text;
					EditorGUI.BeginChangeCheck();
					tempStatusString = EditorGUILayout.TextField( new GUIContent( "Text Value" ), tempStatusString );
					if( EditorGUI.EndChangeCheck() )
					{
						if( tempStatusString != string.Empty )
						{
							Undo.RecordObject( targ.statusBarText, "Update Status Bar Text" );
							targ.statusBarText.enabled = false;
							targ.statusBarText.text = tempStatusString;
							targ.statusBarText.enabled = true;
						}
					}

					EditorGUI.BeginChangeCheck();
					statusBarTextColor = EditorGUILayout.ColorField( "Text Color", statusBarTextColor );
					if( EditorGUI.EndChangeCheck() )
					{
						Undo.RecordObject( targ.statusBarText, "Update Status Bar Text Color" );
						targ.statusBarText.enabled = false;
						targ.statusBarText.color = statusBarTextColor;
						targ.statusBarText.enabled = true;
					}

					Vector2 anchor = Vector2.zero;

					anchor = targ.statusBarText.rectTransform.anchorMin;
					EditorGUI.BeginChangeCheck();
					anchor = EditorGUILayout.Vector2Field( "Anchor Min", anchor );
					if( EditorGUI.EndChangeCheck() )
					{
						if( anchor.x < 0 )
							anchor.x = 0;
						else if( anchor.x > targ.statusBarText.rectTransform.anchorMax.x )
							anchor.x = targ.statusBarText.rectTransform.anchorMax.x;
						else if( anchor.x > 1 )
							anchor.x = 1;

						if( anchor.y < 0 )
							anchor.y = 0;
						else if( anchor.y > targ.statusBarText.rectTransform.anchorMax.y )
							anchor.y = targ.statusBarText.rectTransform.anchorMax.y;
						else if( anchor.y > 1 )
							anchor.y = 1;

						Undo.RecordObject( targ.statusBarText.rectTransform, "Update Text Anchor" );
						targ.statusBarText.rectTransform.anchorMin = anchor;
					}

					anchor = targ.statusBarText.rectTransform.anchorMax;
					EditorGUI.BeginChangeCheck();
					anchor = EditorGUILayout.Vector2Field( "Anchor Max", anchor );
					if( EditorGUI.EndChangeCheck() )
					{
						if( anchor.x < 0 )
							anchor.x = 0;
						else if( anchor.x < targ.statusBarText.rectTransform.anchorMin.x )
							anchor.x = targ.statusBarText.rectTransform.anchorMin.x;
						else if( anchor.x > 1 )
							anchor.x = 1;

						if( anchor.y < 0 )
							anchor.y = 0;
						else if( anchor.y < targ.statusBarText.rectTransform.anchorMin.y )
							anchor.y = targ.statusBarText.rectTransform.anchorMin.y;
						else if( anchor.y > 1 )
							anchor.y = 1;

						Undo.RecordObject( targ.statusBarText.rectTransform, "Update Text Anchor" );
						targ.statusBarText.rectTransform.anchorMax = anchor;
					}

					TextAnchor textAnchor = targ.statusBarText.alignment;
					EditorGUI.BeginChangeCheck();
					textAnchor = ( TextAnchor )EditorGUILayout.EnumPopup( "Text Anchor", textAnchor );
					if( EditorGUI.EndChangeCheck() )
					{
						Undo.RecordObject( targ.statusBarText, "Update Text Anchor" );
						targ.statusBarText.alignment = textAnchor;
					}

					EditorGUI.BeginChangeCheck();
					statusBarTextFont = ( Font )EditorGUILayout.ObjectField( "Text Font", statusBarTextFont, typeof( Font ), false );
					if( EditorGUI.EndChangeCheck() )
					{
						if( targ.statusBarText != null )
						{
							Undo.RecordObject( targ.statusBarText, "Update Status Text Font" );
							targ.statusBarText.font = statusBarTextFont;
						}
					}

					EditorGUI.BeginChangeCheck();
					useStatusBarTextOutline = EditorGUILayout.Toggle( "Text Outline", useStatusBarTextOutline );
					if( EditorGUI.EndChangeCheck() )
					{
						if( useStatusBarTextOutline )
						{
							if( targ.statusBarText.GetComponent<Outline>() )
							{
								Undo.RecordObject( targ.statusBarText.GetComponent<Outline>(), "Enable Status Text Outline" );
								targ.statusBarText.GetComponent<Outline>().enabled = true;
							}
							else
								Undo.AddComponent( targ.statusBarText.gameObject, typeof( Outline ) );
						}
						else
						{
							if( targ.statusBarText.GetComponent<Outline>() )
							{
								Undo.RecordObject( targ.statusBarText.GetComponent<Outline>(), "Disable Status Text Outline" );
								targ.statusBarText.GetComponent<Outline>().enabled = false;
							}
						}
					}

					if( useStatusBarTextOutline )
					{
						EditorGUI.indentLevel++;
						Color statusBarTextColorOutline = targ.statusBarText.GetComponent<Outline>().effectColor;
						EditorGUI.BeginChangeCheck();
						statusBarTextColorOutline = EditorGUILayout.ColorField( new GUIContent( "Outline Color", "The color of the Outline component." ), statusBarTextColorOutline );
						if( EditorGUI.EndChangeCheck() )
						{
							Undo.RecordObject( targ.statusBarText.GetComponent<Outline>(), "Update Status Text Outline Color" );
							targ.statusBarText.GetComponent<Outline>().effectColor = statusBarTextColorOutline;
						}
						EditorGUI.indentLevel--;
					}
				}
			}
			EditorGUILayout.EndVertical();
			if( valueChanged )
			{
				if( useStatusBarText )
				{
					if( targ.statusBarText != null && !targ.statusBarText.gameObject.activeInHierarchy )
					{
						Undo.RecordObject( targ.statusBarText.gameObject, "Enable Status Bar Text" );
						targ.statusBarText.gameObject.SetActive( true );
					}
				}
				else
				{
					if( targ.statusBarText != null && targ.statusBarText.gameObject.activeInHierarchy )
					{
						Undo.RecordObject( targ.statusBarText.gameObject, "Disable Status Bar Text" );
						targ.statusBarText.gameObject.SetActive( false );
					}
				}
			}

			// ----- < STATUS BAR VISIBILITY > ----- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( updateVisibility, new GUIContent( "Update Visibility", "Determines how the status bar should be enabled/disabled in the scene if necessary." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			EditorGUI.indentLevel++;

			// OnStatusUpdated //
			EditorGUILayout.BeginHorizontal();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( onStatusUpdated, new GUIContent( "On Status Updated", "Should this status bar's visibility be updated when a status been updated?" ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				
				if( targ.onStatusUpdated )
					EditorPrefs.SetBool( "USB_OnStatusUpdated", true );
			}

			if( targ.onStatusUpdated )
				DisplayPropertyMinimizeButton( "USB_OnStatusUpdated" );

			EditorGUILayout.EndHorizontal();

			if( targ.onStatusUpdated && EditorPrefs.GetBool( "USB_OnStatusUpdated" ) )
			{
				EditorGUI.indentLevel--;
				EditorGUILayout.BeginVertical( "Box" );

				EditorGUILayout.LabelField( "On Status Updated Options", EditorStyles.boldLabel );
				EditorGUILayout.LabelField( "Keep Status Bar Visible" );

				int indentLevel = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;

				for( int i = 0; i < targ.UltimateStatusList.Count; i++ )
				{
					EditorGUI.BeginChangeCheck();
					keepVisible[ i ].boolValue = EditorGUILayout.ToggleLeft( new GUIContent( targ.UltimateStatusList.Count <= 1 || targ.UltimateStatusList[ i ].statusName == string.Empty ? "Status " + i.ToString( "00" ) : "Status Name: " + targ.UltimateStatusList[ i ].statusName, "Determines if this status will force the Ultimate Status Bar to stay visible if under the trigger value." ), keepVisible[ i ].boolValue );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();

					if( targ.UltimateStatusList[ i ].keepVisible )
					{
						EditorGUI.indentLevel++;
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.Slider( triggerValue[ i ], 0.0f, 1.0f, new GUIContent( "Trigger Value", "The percentage value to keep the Ultimate Status Bar visible." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
						EditorGUI.indentLevel--;
						GUILayout.Space( 2 );
					}
				}
				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel = indentLevel;
				EditorGUI.indentLevel++;
			}

			if( targ.onStatusUpdated )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( idleSeconds, new GUIContent( "Idle Seconds", "Time in seconds before the visibility should be updated." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					if( idleSeconds.floatValue < 0 )
						idleSeconds.floatValue = 0;

					serializedObject.ApplyModifiedProperties();
				}
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( initialState, new GUIContent( "Initial State", "The initial state that the Ultimate Status Bar visibility should be." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			if( targ.updateVisibility == UltimateStatusBar.UpdateVisibility.Animation )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( statusBarAnimator, new GUIContent( "Animator", "The animator component to be used." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
			}
			else
			{
				if( targ.updateVisibility == UltimateStatusBar.UpdateVisibility.Fade )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( enableDuration, new GUIContent( "Fade In Duration", "Time in seconds for the visibility to fade in." ) );
					EditorGUILayout.PropertyField( disableDuration, new GUIContent( "Fade Out Duration", "Time in seconds for the visibility to fade out." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						if( enableDuration.floatValue < 0 )
							enableDuration.floatValue = 0;
						if( disableDuration.floatValue < 0 )
							disableDuration.floatValue = 0;

						serializedObject.ApplyModifiedProperties();
					}
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( enabledAlpha, targ.disabledAlpha, 1.0f, new GUIContent( "Enabled Alpha", "The desired alpha value for the enabled state." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					
					Undo.RecordObject( targ.GetComponent<CanvasGroup>(), "Update Enabled Alpha" );
					targ.GetComponent<CanvasGroup>().alpha = targ.enabledAlpha;
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( disabledAlpha, 0.0f, targ.enabledAlpha, new GUIContent( "Disabled Alpha", "The desired alpha value for the disabled state." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

			}
			
			EditorGUI.indentLevel = 0;

			if( Application.isPlaying )
			{
				GUIStyle centeredLabel = new GUIStyle( EditorStyles.label ) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };

				EditorGUILayout.BeginVertical( "Box" );
				GUILayout.Label( "Status Bar State", centeredLabel );
				EditorGUILayout.BeginHorizontal();

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle( targ.IsEnabled, "Enable", EditorStyles.miniButtonLeft );
				if( EditorGUI.EndChangeCheck() )
					targ.EnableStatusBar();

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle( !targ.IsEnabled, "Disable", EditorStyles.miniButtonRight );
				if( EditorGUI.EndChangeCheck() )
					targ.DisableStatusBar();

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
		}

		if( DisplayHeaderDropdown( "Script Reference", "USB_ScriptReference" ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( statusBarName, new GUIContent( "Status Bar Name", "The name to be used for reference from scripts." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				UltimateStatusBarNameDuplicate = DuplicateStatusBarName();
			}

			if( targ.statusBarName == string.Empty )
			{
				EditorGUILayout.HelpBox( "Please make sure to assign a name so that this status bar can be referenced from your scripts.", MessageType.Warning );

				EditorGUILayout.BeginVertical( "Box" );
				GUILayout.Space( 1 );
				EditorGUILayout.LabelField( "Example Code Generator", EditorStyles.boldLabel );

				EditorGUILayout.LabelField( "Copy and paste this variable into your script:", EditorStyles.wordWrappedLabel );
				EditorGUILayout.TextArea( "public UltimateStatusBar statusBar;", EditorStyles.textArea );

				exampleCodeIndex = EditorGUILayout.Popup( "Function", exampleCodeIndex, publicExampleCodeOptions.ToArray() );
				if( statusNameList.Count > 0 && exampleCodeIndex == 0 )
				{
					statusNameListIndex = EditorGUILayout.Popup( "Status Name", statusNameListIndex, statusNameList.ToArray() );
					EditorGUILayout.TextField( GenerateExampleCode( PublicExampleCodeList[ exampleCodeIndex ].basicCode, statusNameList[ statusNameListIndex ] ) );
				}
				else
					EditorGUILayout.TextField( GenerateExampleCode( PublicExampleCodeList[ exampleCodeIndex ].basicCode, "" ) );

				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();
			}

			if( UltimateStatusBarNameDuplicate )
				EditorGUILayout.HelpBox( "This name has already been used in your scene. Please make sure to make the Status Bar Name unique.", MessageType.Error );

			if( !UltimateStatusBarNameDuplicate && targ.statusBarName != string.Empty )
			{
				EditorGUILayout.BeginVertical( "Box" );
				GUILayout.Space( 1 );
				EditorGUILayout.LabelField( "Example Code Generator", EditorStyles.boldLabel );
				exampleCodeIndex = EditorGUILayout.Popup( "Function", exampleCodeIndex, staticExampleCodeOptions.ToArray() );

				if( statusNameList.Count > 0 && exampleCodeIndex == 0 )
				{
					statusNameListIndex = EditorGUILayout.Popup( "Status Name", statusNameListIndex, statusNameList.ToArray() );
					EditorGUILayout.TextField( GenerateExampleCode( StaticExampleCodeList[ exampleCodeIndex ].basicCode, targ.statusBarName, statusNameList[ statusNameListIndex ] ) );
				}
				else
					EditorGUILayout.TextField( GenerateExampleCode( StaticExampleCodeList[ exampleCodeIndex ].basicCode, targ.statusBarName ) );

				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();
			}

			if( GUILayout.Button( "Open Documentation" ) )
				UltimateStatusBarReadmeEditor.OpenReadmeDocumentation();
		}

		if( EditorPrefs.GetBool( "UUI_DevelopmentMode" ) )
		{
			EditorGUILayout.Space();
			GUIStyle developmentHeaderStyle = new GUIStyle( headerDropdownStyle ) { richText = true };
			GUILayout.BeginHorizontal();
			GUILayout.Space( -10 );
			showDefaultInspector = GUILayout.Toggle( showDefaultInspector, ( showDefaultInspector ? "▼" : "►" ) + "<color=#ff0000ff>Development Inspector</color>", developmentHeaderStyle );
			GUILayout.EndHorizontal();
			if( showDefaultInspector )
			{
				EditorGUILayout.Space();

				base.OnInspectorGUI();
			}
		}
		
		EditorGUILayout.Space();

		Repaint();
	}

	void StoreExampleCode ()
	{
		PublicExampleCodeList = new List<ExampleCode>();
		publicExampleCodeOptions = new List<string>();
		for( int i = 0; i < PublicExampleCodes.Length; i++ )
		{
			if( i == 0 && targ.UltimateStatusList.Count > 1 )
				continue;

			if( i == 1 && targ.UltimateStatusList.Count <= 1 )
				continue;

			PublicExampleCodeList.Add( PublicExampleCodes[ i ] );
			publicExampleCodeOptions.Add( PublicExampleCodes[ i ].optionName );
		}

		StaticExampleCodeList = new List<ExampleCode>();
		staticExampleCodeOptions = new List<string>();
		for( int i = 0; i < StaticExampleCodes.Length; i++ )
		{
			if( i == 0 && targ.UltimateStatusList.Count > 1 )
				continue;

			if( i == 1 && targ.UltimateStatusList.Count <= 1 )
				continue;

			StaticExampleCodeList.Add( StaticExampleCodes[ i ] );
			staticExampleCodeOptions.Add( StaticExampleCodes[ i ].optionName );
		}
	}

	void StoreNameList ()
	{
		statusNameList = new List<string>();

		if( Selection.gameObjects.Length > 1 )
			return;

		if( targ.UltimateStatusList.Count == 1 )
			return;

		for( int i = 0; i < statusName.Count; i++ )
		{
			if( statusName[ i ].stringValue != string.Empty )
				statusNameList.Add( statusName[ i ].stringValue );
		}
	}

	void AddNewStatus ( int index )
	{
		serializedObject.FindProperty( "UltimateStatusList" ).InsertArrayElementAtIndex( index );
		serializedObject.ApplyModifiedProperties();

		targ.UltimateStatusList[ index ] = new UltimateStatusBar.UltimateStatus();

		EditorPrefs.SetBool( "USB_StatusAdvanced_" + index.ToString( "00" ), true );

		StoreReferences();
	}

	void RemoveStatus ( int index )
	{
		serializedObject.FindProperty( "UltimateStatusList" ).DeleteArrayElementAtIndex( index );
		serializedObject.ApplyModifiedProperties();
		
		StoreReferences();
	}
	
	bool DuplicateStatusBarName ()
	{
		UltimateStatusBar[] ultimateStatusBars = FindObjectsOfType<UltimateStatusBar>();

		for( int i = 0; i < ultimateStatusBars.Length; i++ )
		{
			if( ultimateStatusBars[ i ].statusBarName == string.Empty )
				continue;

			if( ultimateStatusBars[ i ] != targ && ultimateStatusBars[ i ].statusBarName == targ.statusBarName )
				return true;
		}

		return false;
	}

	bool DuplicateStatusName ( int index )
	{
		if( statusName[ index ].stringValue == string.Empty )
			return false;

		for( int i = 0; i < statusName.Count; i++ )
		{
			if( i == index )
				continue;

			if( statusName[ i ].stringValue == statusName[ index ].stringValue && i < index )
				return true;
		}
		return false;
	}

	string GenerateExampleCode ( string basicCode, string name, string name2 = "" )
	{
		if( name2 != string.Empty )
			return string.Format( basicCode, name, name2 );

		return string.Format( basicCode, name );
	}
	
	void UpdateStatusColor ( int index )
	{
		if( targ.UltimateStatusList[ index ].statusImage == null )
			return;

		Undo.RecordObject( targ.UltimateStatusList[ index ].statusImage, "Update Status Color" );
		if( targ.UltimateStatusList[ index ].colorMode == UltimateStatusBar.UltimateStatus.ColorMode.Gradient )
			targ.UltimateStatusList[ index ].statusImage.color = targ.UltimateStatusList[ index ].statusGradient.Evaluate( targ.UltimateStatusList[ index ].CalculatedPercentage );
		else
			targ.UltimateStatusList[ index ].statusImage.color = statusColor[ index ];
	}
	
	void OnSceneGUI ()
	{
		if( Selection.activeGameObject == null || Selection.objects.Length > 1 || parentCanvas == null || targ.BaseTransform == null )
			return;
		
		if( targ.IsFollowingTransform )
		{
			Handles.color = Color.red;

			if( targ.IsEnabled )
				Handles.color = Color.green;

			Rect inBounds = new Rect( ( -targ.BaseTransform.sizeDelta / 2 ) * parentCanvas.GetComponent<RectTransform>().localScale, new Vector2( parentCanvasRectTrans.sizeDelta.x + targ.BaseTransform.sizeDelta.x, parentCanvasRectTrans.sizeDelta.y + targ.BaseTransform.sizeDelta.y ) * parentCanvas.GetComponent<RectTransform>().localScale );
			Handles.DrawWireCube( inBounds.center, inBounds.size );
		}

		if( Application.isPlaying )
			return;

		Handles.color = Color.white;
		
		RectTransform statusBarTrans = targ.GetComponent<RectTransform>();

		if( useStatusBarIcon && targ.statusBarIcon != null && EditorPrefs.GetBool( "USB_StatusBarOptions" ) && EditorPrefs.GetBool( "USB_StatusBarIcon" ) )
		{
			Bounds statusIconBounds = RectTransformUtility.CalculateRelativeRectTransformBounds( statusBarTrans, targ.statusBarIcon.rectTransform );

			DisplayWireBox( targ.statusBarIcon.rectTransform.InverseTransformPoint( targ.statusBarIcon.rectTransform.position ), statusIconBounds.size, targ.statusBarIcon.rectTransform );
		}
		
		if( useStatusBarText && targ.statusBarText != null && EditorPrefs.GetBool( "USB_StatusBarOptions" ) && EditorPrefs.GetBool( "USB_StatusBarText" ) )
		{
			Bounds statusBounds = RectTransformUtility.CalculateRelativeRectTransformBounds( statusBarTrans, targ.statusBarText.rectTransform );

			DisplayWireBox( targ.statusBarText.rectTransform.InverseTransformPoint( targ.statusBarText.rectTransform.position ), statusBounds.size, targ.statusBarText.rectTransform );
		}

		for( int i = 0; i < targ.UltimateStatusList.Count; i++ )
		{
			if( ( targ.UltimateStatusList.Count == 1 || EditorPrefs.GetBool( "USB_StatusAdvanced_" + i.ToString( "00" ) ) ) && EditorPrefs.GetBool( "USB_DisplayText_" + i.ToString( "00" ) ) && EditorPrefs.GetBool( "USB_TextSettings_" + i.ToString( "00" ) ) )
			{
				if( targ.UltimateStatusList[ i ].statusText == null )
					continue;
				
				Bounds statusBounds = RectTransformUtility.CalculateRelativeRectTransformBounds( statusBarTrans, targ.UltimateStatusList[ i ].statusText.rectTransform );

				DisplayWireBox( targ.UltimateStatusList[ i ].statusText.rectTransform.InverseTransformPoint( targ.UltimateStatusList[ i ].statusText.rectTransform.position ), statusBounds.size, targ.UltimateStatusList[ i ].statusText.rectTransform );
			}
		}
	}

	void DisplayWireBox ( Vector3 center, Vector2 sizeDelta, Transform trans )
	{
		float halfHeight = sizeDelta.y / 2;
		float halfWidth = sizeDelta.x / 2;

		Vector3 topLeft = center + new Vector3( -halfWidth, halfHeight, 0 );
		Vector3 topRight = center + new Vector3( halfWidth, halfHeight, 0 );
		Vector3 bottomRight = center + new Vector3( halfWidth, -halfHeight, 0 );
		Vector3 bottomLeft = center + new Vector3( -halfWidth, -halfHeight, 0 );

		topLeft = trans.TransformPoint( topLeft );
		topRight = trans.TransformPoint( topRight );
		bottomRight = trans.TransformPoint( bottomRight );
		bottomLeft = trans.TransformPoint( bottomLeft );

		Handles.DrawLine( topLeft, topRight );
		Handles.DrawLine( topRight, bottomRight );
		Handles.DrawLine( bottomRight, bottomLeft );
		Handles.DrawLine( bottomLeft, topLeft );
	}

	void CheckForParentCanvas ()
	{
		if( Selection.activeGameObject == null )
			return;

		// Store the current parent.
		Transform parent = Selection.activeGameObject.transform.parent;

		// Loop through parents as long as there is one.
		while( parent != null )
		{
			// If there is a Canvas component, return that gameObject.
			if( parent.transform.GetComponent<Canvas>() && parent.transform.GetComponent<Canvas>().enabled == true )
			{
				parentCanvas = parent.transform.GetComponent<Canvas>();
				return;
			}

			// Else, shift to the next parent.
			parent = parent.transform.parent;
		}
		if( parent == null && !AssetDatabase.Contains( Selection.activeGameObject ) )
		{
			if( EditorUtility.DisplayDialog( "Ultimate Status Bar", "Where are you wanting to use this Ultimate Status Bar?", "World Space", "Screen Space" ) )
				RequestCanvas( Selection.activeGameObject, false );
			else
				RequestCanvas( Selection.activeGameObject );
		}
	}

	void CheckEventSystem ()
	{
		Object currentEventSystem = FindObjectOfType<EventSystem>();
		if( currentEventSystem == null )
		{
			GameObject eventSystem = new GameObject( "EventSystem" );
			currentEventSystem = eventSystem.AddComponent<EventSystem>();
			eventSystem.AddComponent<StandaloneInputModule>();

			Undo.RegisterCreatedObjectUndo( eventSystem, "Create " + eventSystem.name );
		}
	}

	void RequestCanvas ( GameObject child, bool screenSpaceOverlay = true )
	{
		// Store all canvas objects to check the render mode options.
		Canvas[] allCanvas = Object.FindObjectsOfType( typeof( Canvas ) ) as Canvas[];

		// Loop through each canvas.
		for( int i = 0; i < allCanvas.Length; i++ )
		{
			// If the user wants a screen space canvas...
			if( screenSpaceOverlay )
			{
				// Check to see if this canvas is set to Screen Space and it is enabled. Then set the parent and check for an event system.
				if( allCanvas[ i ].renderMode == RenderMode.ScreenSpaceOverlay && allCanvas[ i ].enabled == true )
				{
					Undo.SetTransformParent( child.transform, allCanvas[ i ].transform, "Update Radial Menu Parent" );
					CheckEventSystem();
					return;
				}
			}
			// Else the user wants a world space canvas...
			else
			{
				// Check to see if this canvas is set to World Space and see if it is enabled. Then set the parent and check for an event system.
				if( allCanvas[ i ].renderMode == RenderMode.WorldSpace && allCanvas[ i ].enabled == true )
				{
					Undo.SetTransformParent( child.transform, allCanvas[ i ].transform, "Update Radial Menu Parent" );
					CheckEventSystem();
					return;
				}
			}
		}

		// If there have been no canvas objects found for this child, then create a new one.
		GameObject newCanvasObject = new GameObject( "Canvas" );
		newCanvasObject.layer = LayerMask.NameToLayer( "UI" );
		Canvas canvasComponent = newCanvasObject.AddComponent<Canvas>();
		newCanvasObject.AddComponent<GraphicRaycaster>();

		if( screenSpaceOverlay )
			canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
		else
			canvasComponent.renderMode = RenderMode.WorldSpace;

		Undo.RegisterCreatedObjectUndo( newCanvasObject, "Create New Canvas" );
		Undo.SetTransformParent( child.transform, newCanvasObject.transform, "Create New Canvas" );
		CheckEventSystem();
	}
}
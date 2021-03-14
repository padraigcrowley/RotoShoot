/* UltimateStatusBarReadmeEditor.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
[CustomEditor( typeof( UltimateStatusBarReadme ) )]
public class UltimateStatusBarReadmeEditor : Editor
{
	static UltimateStatusBarReadme readme;

	// LAYOUT STYLES //
	string Indent
	{
		get
		{
			return "    ";
		}
	}
	int sectionSpace = 20;
	int itemHeaderSpace = 10;
	int paragraphSpace = 5;
	GUIStyle titleStyle = new GUIStyle();
	GUIStyle sectionHeaderStyle = new GUIStyle();
	GUIStyle itemHeaderStyle = new GUIStyle();
	GUIStyle paragraphStyle = new GUIStyle();
	GUIStyle versionStyle = new GUIStyle();

	class PageInformation
	{
		public string pageName = "";
		public delegate void TargetMethod ();
		public TargetMethod targetMethod;
	}
	static List<PageInformation> pageHistory = new List<PageInformation>();
	static PageInformation[] AllPages = new PageInformation[]
	{
		// MAIN MENU - 0 //
		new PageInformation()
		{
			pageName = "Product Manual"
		},
		// Getting Started - 1 //
		new PageInformation()
		{
			pageName = "Getting Started"
		},
		// Overview - 2 //
		new PageInformation()
		{
			pageName = "Overview"
		},
		// Overview ASH - 3 //
		new PageInformation()
		{
			pageName = "Overview"
		},
		// Overview: SFF - 4 //
		new PageInformation()
		{
			pageName = "Overview"
		},
		// Documentation - 5 //
		new PageInformation()
		{
			pageName = "Documentation"
		},
		// Documentation - USB - 6 //
		new PageInformation()
		{
			pageName = "Documentation"
		},
		// Documentation - US - 7 //
		new PageInformation()
		{
			pageName = "Documentation"
		},
		// Documentation - ASH - 8 //
		new PageInformation()
		{
			pageName = "Documentation"
		},
		// Version History - 9 //
		new PageInformation()
		{
			pageName = "Version History"
		},
		// Important Change - 10 //
		new PageInformation()
		{
			pageName = "Important Change"
		},
		// Thank You! - 11 //
		new PageInformation()
		{
			pageName = "Thank You!"
		},
		// Settings - 12 //
		new PageInformation()
		{
			pageName = "Settings"
		},
	};

	class EndPageComment
	{
		public string comment = "";
		public string url = "";
	}
	EndPageComment[] endPageComments = new EndPageComment[]
	{
		new EndPageComment()
		{
			comment = "Enjoying the Ultimate Status Bar? Leave us a review on the <b><color=blue>Unity Asset Store</color></b>!",
			url = "https://assetstore.unity.com/packages/slug/48320"
		},
		new EndPageComment()
		{
			comment = "Looking for a mobile joystick for your game? Check out the <b><color=blue>Ultimate Joystick</color></b>!",
			url = "https://www.tankandhealerstudio.com/ultimate-joystick.html"
		},
		new EndPageComment()
		{
			comment = "Do you need a radial menu for your game? Check out the <b><color=blue>Ultimate Radial Menu</color></b>!",
			url = "https://www.tankandhealerstudio.com/ultimate-radial-menu.html"
		},
		new EndPageComment()
		{
			comment = "Check out our <b><color=blue>other products</color></b>!",
			url = "https://www.tankandhealerstudio.com/assets.html"
		},
	};
	int randomComment = 0;

	class DocumentationInfo
	{
		public string functionName = "";
		public bool showMore = false;
		public string[] parameter;
		public string returnType = "";
		public string description = "";
		public string codeExample = "";
	}
	// ULTIMATE STATUS BAR DOCUMENTATION //
	DocumentationInfo[] UltimateStatusBar_PublicFunctions = new DocumentationInfo[]
	{
		// GetUltimateStatus
		new DocumentationInfo()
		{
			functionName = "GetUltimateStatus()",
			parameter = new string[]
			{
				"string statusName - The name of the targeted Ultimate Status.",
			},
			returnType = "UltimateStatus",
			description = "This function returns the Ultimate Status class that has been registered with the <i>statusName</i> parameter.",
			codeExample = "UltimateStatusBar.UltimateStatus healthStatus = statusBar.GetUltimateStatus( \"Health\" );"
		},
		// UpdatePositioning
		new DocumentationInfo()
		{
			functionName = "UpdatePositioning()",
			description = "This function updates the size and positioning of the Ultimate Status Bar on the screen.",
			codeExample = "statusBar.screenSpaceOptions.statusBarSize = 5.0f;\nstatusBar.UpdatePositioning();"
		},
		// UpdateStatus
		new DocumentationInfo()
		{
			functionName = "UpdateStatus()",
			parameter = new string[]
			{
				"float currentValue - The current value of the status.",
				"float maxValue - The maximum value of the status."
			},
			description = "This function will call the default status on the targeted Ultimate Status Bar. It updates the values of the status in order to display them to the user. This function has two parameters that need to be passed into it. The <i>currentValue</i> should be the current amount of the targeted status, whereas the <i>maxValue</i> should be the maximum amount that the status can be. These values must be passed into the function in order to correctly display them to the user.",
			codeExample = "statusBar.UpdateStatus( currentValue, maxValue );"
		},
		// UpdateStatus Specific
		new DocumentationInfo()
		{
			functionName = "UpdateStatus() *Specific Status",
			parameter = new string[]
			{
				"float currentValue - The current value of the status.",
				"float maxValue - The maximum value of the status.",
				"string statusName - The name of the targeted Ultimate Status.",
			},
			description = "This function will call the targeted Ultimate Status that has been registered with the <i>statusName</i> parameter. It updates the values of the status in order to display them to the user. The <i>currentValue</i> should be the current amount of the targeted status, whereas the <i>maxValue</i> should be the maximum amount that the status can be. These values must be passed into the function in order to correctly display them to the user.",
			codeExample = "statusBar.UpdateStatus( currentValue, maxValue, \"Health\" );"
		},
		// EnableStatusBar
		new DocumentationInfo()
		{
			functionName = "EnableStatusBar()",
			description = "Enables the status bar visually so that the player can see it.",
			codeExample = "statusBar.EnableStatusBar();"
		},
		// DisableStatusBar
		new DocumentationInfo()
		{
			functionName = "DisableStatusBar()",
			description = "Disables the status bar visually so that the player can no longer see it",
			codeExample = "statusBar.DisableStatusBar();"
		},
		// StartFollowTransform
		new DocumentationInfo()
		{
			functionName = "StartFollowTransform()",
			parameter = new string[]
			{
				"Transform transformToFollow - The Transform component of the object that the status bar should follow.",
				"Camera camera - The camera component that should be used for calculations of the world objects position on the screen.",
				"Vector3 positionModifier - This parameter allows you to specify the position in relation to the target transform for the status bar to follow.",
			},
			description = "This function starts a coroutine that allows the status bar to follow a world objects position on the screen.",
			codeExample = "statusBar.StartFollowTransform( enemyToFollow, playerCamera, 2 );"
		},
		// StopFollowTransform
		new DocumentationInfo()
		{
			functionName = "StopFollowTransform()",
			description = "This function stops the FollowTransform coroutine if it is running.",
			codeExample = "statusBar.StopFollowTransform();"
		},
		// StartLookAtCamera
		new DocumentationInfo()
		{
			functionName = "StartLookAtCamera()",
			parameter = new string[]
			{
				"Camera camera - The camera component that the status bar canvas should look at.",
			},
			description = "This function starts a coroutine that forces the canvas of the status bar to look at the camera.",
			codeExample = "statusBar.StartLookAtCamera( playerCamera );"
		},
		// StopLookAtCamera
		new DocumentationInfo()
		{
			functionName = "StopLookAtCamera()",
			description = "This function stops the LookAtCamera coroutine if it is running.",
			codeExample = "statusBar.StopLookAtCamera();"
		},
	};
	DocumentationInfo[] UltimateStatusBar_StaticFunctions = new DocumentationInfo[]
	{
		// GetUltimateStatusBar
		new DocumentationInfo()
		{
			functionName = "GetUltimateStatusBar()",
			returnType = "UltimateStatusBar",
			parameter = new string[]
			{
				"string statusBarName - The name of the targeted Ultimate Status Bar.",
			},
			description = "This function will return the Ultimate Status Bar component that has been registered with the <i>statusBarName</i> parameter.",
			codeExample = "UltimateStatusBar playerStatusBar = UltimateStatusBar.GetUltimateStatusBar( \"Player\" );"
		},
		// UpdatePositioning
		new DocumentationInfo()
		{
			functionName = "UpdatePositioning()",
			parameter = new string[]
			{
				"string statusBarName - The name of the targeted Ultimate Status Bar.",
			},
			description = "This function will update the positioning of the Ultimate Status Bar on the canvas. This function is useful for if you have changed some of the position variables at runtime and want to apply them.",
			codeExample = "UltimateStatusBar.UpdatePositioning( \"Player\" );"
		},
		// UpdateStatus
		new DocumentationInfo()
		{
			functionName = "UpdateStatus()",
			parameter = new string[]
			{
				"string statusBarName - The name of the targeted Ultimate Status Bar.",
				"float currentValue - The current value of the status.",
				"float maxValue - The maximum value of the status.",
			},
			description = "This function will update the default status on the Ultimate Status Bar component with the current and max values that you provided to it.",
			codeExample = "UltimateStatusBar.UpdateStatus( \"Player\", currentValue, maxValue );"
		},
		// UpdateStatus Specific
		new DocumentationInfo()
		{
			functionName = "UpdateStatus() *Specific Status",
			parameter = new string[]
			{
				"string statusBarName - The name of the targeted Ultimate Status Bar.",
				"float currentValue - The current value of the status.",
				"float maxValue - The maximum value of the status.",
				"string statusName - The name of the targeted Ultimate Status.",
			},
			description = "This function will update the targeted Ultimate Status that has been registered on the Ultimate Status Bar component. See the UpdateStatus() function inside the Ultimate Status documentation for more details.",
			codeExample = "UltimateStatusBar.UpdateStatus( \"Player\", currentValue, maxValue, \"Health\" );"
		},
		// EnableStatusBar
		new DocumentationInfo()
		{
			functionName = "EnableStatusBar()",
			parameter = new string[]
			{
				"string statusBarName - The name of the targeted Ultimate Status Bar.",
			},
			description = "Enables the targeted Ultimate Status Bar so that it is visible to the player.",
			codeExample = "UltimateStatusBar.EnableStatusBar( \"Player\" );"
		},
		// DisableStatusBar
		new DocumentationInfo()
		{
			functionName = "DisableStatusBar()",
			parameter = new string[]
			{
				"string statusBarName - The name of the targeted Ultimate Status Bar.",
			},
			description = "Disables the targeted Ultimate Status Bar so that it is no longer visible to the player.",
			codeExample = "UltimateStatusBar.DisableStatusBar( \"Player\" );"
		},
	};
	DocumentationInfo[] UltimateStatusBar_PublicAccessors = new DocumentationInfo[]
	{
		// ParentCanvas
		new DocumentationInfo()
		{
			functionName = "ParentCanvas",
			description = "The Canvas component that this status bar is a child of.",
		},
		// BaseTransform
		new DocumentationInfo()
		{
			functionName = "BaseTransform",
			description = "The base RectTransform component of this status bar.",
		},
		// IsEnabled
		new DocumentationInfo()
		{
			functionName = "IsEnabled",
			returnType = "bool",
			description = "Returns the current visual state of the status bar.",
		},
		// IsFollowingTransform
		new DocumentationInfo()
		{
			functionName = "IsFollowingTransform",
			returnType = "bool",
			description = "Returns if the status bar is following the position of a transform in the world.",
		},
		// IsFollowingCamera
		new DocumentationInfo()
		{
			functionName = "IsFollowingCamera",
			returnType = "bool",
			description = "Returns if the status bar is following the camera rotation.",
		},
		// icon
		new DocumentationInfo()
		{
			functionName = "icon",
			description = "This accessor returns the current sprite that is used by the status bar icon, and you can set a new sprite by assigning a Sprite to this accessor.",
		},
		// text
		new DocumentationInfo()
		{
			functionName = "text",
			description = "This accessor returns the current string value of the text component for the status bar, and you can set a new string text by assigning a new string to this accessor.",
		},
	};
	DocumentationInfo[] UltimateStatusBar_Callbacks = new DocumentationInfo[]
	{
		// OnUpdatePositioning
		new DocumentationInfo()
		{
			functionName = "OnUpdatePositioning",
			description = "This callback is invoked when the UpdatePositioning function has been called.",
		},
		// OnEnableStatusBar
		new DocumentationInfo()
		{
			functionName = "OnEnableStatusBar",
			description = "This callback is invoked when the status bar is enabled visually.",
		},
		// OnDisableStatusBar
		new DocumentationInfo()
		{
			functionName = "OnDisableStatusBar",
			description = "This callback is invoked when the status bar is disabled visually.",
		},
	};
	DocumentationInfo[] UltimateStatus_PublicFunctions = new DocumentationInfo[]
	{
		new DocumentationInfo()
		{
			functionName = "UpdateStatus()",
			parameter = new string[]
			{
				"float currentValue - The current value of the status.",
				"float maxValue - The maximum value of the status."
			},
			description = "This function will update the values of the status in order to display them to the user. This function has two parameters that need to be passed into it. The <i>currentValue</i> should be the current amount of the targeted status, whereas the <i>maxValue</i> should be the maximum amount that the status can be. These values must be passed into the function in order to correctly display them to the user. Using these values, the Ultimate Status will calculate out the percentage values and then display this information to the user according to the options set in the inspector.",
			codeExample = "status.UpdateStatus( current, max );"
		},
	};
	DocumentationInfo[] UltimateStatus_PublicAccessors = new DocumentationInfo[]
	{
		// ForceVisible
		new DocumentationInfo()
		{
			functionName = "ForceVisible",
			returnType = "bool",
			description = "Returns the state of this status forcing the status bar to be visible determined by the users options.",
		},
		// CalculatedPercentage
		new DocumentationInfo()
		{
			functionName = "CalculatedPercentage",
			returnType = "float",
			description = "The calculated percentage of the status bar percentage at this frame.",
		},
		// textColor
		new DocumentationInfo()
		{
			functionName = "textColor",
			returnType = "Color",
			description = "Sets the text color to the provided value.",
		},
		// color
		new DocumentationInfo()
		{
			functionName = "color",
			returnType = "bool",
			description = "Sets the status image color to the provided value.",
		},
	};
	DocumentationInfo[] UltimateStatus_Callbacks = new DocumentationInfo[]
	{
		// OnStatusUpdated
		new DocumentationInfo()
		{
			functionName = "OnStatusUpdated<float>",
			description = "This callback is invoked whenever this status is updated and sends the current calculated percentage of the status as the parameter.",
		},
	};

	// ALTERNATE STATE HANDLER DOCUMENTATION //
	DocumentationInfo[] AlternateStateHandler_PublicFunctions = new DocumentationInfo[]
	{
		new DocumentationInfo()
		{
			functionName = "SwitchState()",
			parameter = new string[]
			{
				"string stateName - The name of the state.",
				"bool state - The targeted state."
			},
			description = "This function switches the targeted Alternate State to the desired state. The <i>stateName</i> parameter will allow the script to find that specific state in order to switch it.",
			codeExample = "altStateHandler.SwitchState( \"HealthCritical\", true );"
		},
		new DocumentationInfo()
		{
			functionName = "GetAlternateState()",
			parameter = new string[]
			{
				"string stateName - The name of the state.",
			},
			description = "This function will return the AlternateState class that has been registered with the <i>stateName</i> parameter.",
			codeExample = "AlternateStateHandler.AlternateState alternateState = altStateHandler.GetAlternateState( \"HealthCritical\" );"
		},
	};
	DocumentationInfo[] AlternateStateHandler_StaticFunctions = new DocumentationInfo[]
	{
		new DocumentationInfo()
		{
			functionName = "SwitchState()",
			parameter = new string[]
			{
				"string statusBarName - The name of the Ultimate Status Bar associated with the Alternate State Handler.",
				"string stateName - The name the desired state to update.",
				"bool state - The targeted state."
			},
			description = "This function switches the targeted Alternate State to the desired state. The <i>statusBarName</i> parameter will allow the script to find the specific Alternate State Handler that has been registered with the name of the Ultimate Status Bar that it is associated with. The <i>stateName</i> parameter will allow the script to find that specific state in order to switch it.",
			codeExample = "AlternateStateHandler.SwitchState( \"Player\", \"HealthCritical\", true );"
		},
		new DocumentationInfo()
		{
			functionName = "GetAlternateStateHandler()",
			parameter = new string[]
			{
				"string statusBarName - The name of the Ultimate Status Bar associated with the Alternate State Handler.",
			},
			description = "This function returns the AlternateStateHandler class that has been registered with the <i>statusBarName</i> parameter. It's worth noting that the Alternate State Handler will be registered with the name of the Ultimate Status Bar that it is associated with.",
			codeExample = "AlternateStateHandler altStateHandler = AlternateStateHandler.GetAlternateStateHandler( \"Player\" );"
		},
	};

	bool showStatusBarPositioning = false, showStatusBarOptions = false;
	bool showStatusInformation = false, showScriptReference = false;
	bool showAlternateStates = false, altShowScriptRefernce = false;

	enum NumberOfStatus
	{
		Single,
		Multiple
	}
	NumberOfStatus numberOfStatus = NumberOfStatus.Single;
	

	static UltimateStatusBarReadmeEditor ()
	{
		EditorApplication.update += WaitForCompile;
	}

	static void WaitForCompile ()
	{
		if( EditorApplication.isCompiling )
			return;

		EditorApplication.update -= WaitForCompile;
		
		// If this is the first time that the user has downloaded this asset...
		if( !EditorPrefs.HasKey( "UltimateStatusBarVersion" ) )
		{
			NavigateForward( 11 );
			EditorPrefs.SetInt( "UltimateStatusBarVersion", UltimateStatusBarReadme.ImportantChange );
			var ids = AssetDatabase.FindAssets( "README t:UltimateStatusBarReadme" );
			if( ids.Length == 1 )
			{
				var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );
				Selection.objects = new Object[] { readmeObject };
			}
		}
		// Else if the version has been updated and there are important changes to display to the user...
		else if( EditorPrefs.GetInt( "UltimateStatusBarVersion" ) < UltimateStatusBarReadme.ImportantChange )
		{
			NavigateForward( 10 );
			EditorPrefs.SetInt( "UltimateStatusBarVersion", UltimateStatusBarReadme.ImportantChange );
			var ids = AssetDatabase.FindAssets( "README t:UltimateStatusBarReadme" );
			if( ids.Length == 1 )
			{
				var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );
				Selection.objects = new Object[] { readmeObject };
			}
		}
	}

	void OnEnable ()
	{
		readme = ( UltimateStatusBarReadme )target;

		//if( !pageHistory.Contains( mainMenu ) )
		//	pageHistory.Insert( 0, mainMenu );

		//mainMenu.targetMethod = MainPage;
		//gettingStarted.targetMethod = GettingStarted;
		//overview.targetMethod = Overview;
		//overview_ASH.targetMethod = Overview_AlternateStateHandler;
		//overview_SFF.targetMethod = Overview_StatusFillFollower;
		//documentation.targetMethod = Documentation;
		//documentation_US.targetMethod = Documentation_UltimateStatus;
		//documentation_USB.targetMethod = Documentation_UltimateStatusBar;
		//documentation_ASH.targetMethod = Documentation_AlternateStateHandler;
		//versionHistory.targetMethod = VersionHistory;
		//importantChange.targetMethod = ImportantChange;
		//thankYou.targetMethod = ThankYou;
		//settings.targetMethod = Settings;

		AllPages[ 0 ].targetMethod = MainPage;
		AllPages[ 1 ].targetMethod = GettingStarted;
		AllPages[ 2 ].targetMethod = Overview;
		AllPages[ 3 ].targetMethod = Overview_AlternateStateHandler;
		AllPages[ 4 ].targetMethod = Overview_StatusFillFollower;
		AllPages[ 5 ].targetMethod = Documentation;
		AllPages[ 6 ].targetMethod = Documentation_UltimateStatusBar;
		AllPages[ 7 ].targetMethod = Documentation_UltimateStatus;
		AllPages[ 8 ].targetMethod = Documentation_AlternateStateHandler;
		AllPages[ 9 ].targetMethod = VersionHistory;
		AllPages[ 10 ].targetMethod = ImportantChange;
		AllPages[ 11 ].targetMethod = ThankYou;
		AllPages[ 12 ].targetMethod = Settings;

		pageHistory = new List<PageInformation>();
		for( int i = 0; i < readme.pageHistory.Count; i++ )
			pageHistory.Add( AllPages[ readme.pageHistory[ i ] ] );

		if( !pageHistory.Contains( AllPages[ 0 ] ) )
		{
			pageHistory.Insert( 0, AllPages[ 0 ] );
			readme.pageHistory.Insert( 0, 0 );
		}

		//if( pageHistory.Count == 1 )
		//	currentPage = mainMenu;

		randomComment = Random.Range( 0, endPageComments.Length );

	}

	protected override void OnHeaderGUI ()
	{
		UltimateStatusBarReadme readme = ( UltimateStatusBarReadme )target;

		float iconWidth = Mathf.Min( EditorGUIUtility.currentViewWidth, 350f );

		Vector2 ratio = new Vector2( readme.icon.width, readme.icon.height ) / ( readme.icon.width > readme.icon.height ? readme.icon.width : readme.icon.height );

		GUILayout.BeginHorizontal( "In BigTitle" );
		{
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.Label( readme.icon, GUILayout.Width( iconWidth * ratio.x ), GUILayout.Height( iconWidth * ratio.y ) );
			GUILayout.Space( -20 );
			if( GUILayout.Button( readme.versionHistory[ 0 ].versionNumber, versionStyle ) && !pageHistory.Contains( AllPages[ 9 ] ) )
				NavigateForward( 9 );
			var rect = GUILayoutUtility.GetLastRect();
			if( pageHistory[ pageHistory.Count - 1 ] != AllPages[ 9 ] )
				EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		paragraphStyle = new GUIStyle( EditorStyles.label ) { wordWrap = true, richText = true, fontSize = 12 };
		itemHeaderStyle = new GUIStyle( paragraphStyle ) { fontSize = 12, fontStyle = FontStyle.Bold };
		sectionHeaderStyle = new GUIStyle( paragraphStyle ) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
		titleStyle = new GUIStyle( paragraphStyle ) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
		versionStyle = new GUIStyle( paragraphStyle ) { alignment = TextAnchor.MiddleCenter, fontSize = 10 };

		paragraphStyle.active.textColor = paragraphStyle.normal.textColor;

		// SETTINGS BUTTON //
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( readme.settings, versionStyle, GUILayout.Width( 24 ), GUILayout.Height( 24 ) ) && !pageHistory.Contains( AllPages[ 12 ] ) )
			NavigateForward( 12 );
		var rect = GUILayoutUtility.GetLastRect();
		if( pageHistory[ pageHistory.Count - 1 ] != AllPages[ 12 ] )
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		GUILayout.EndHorizontal();
		GUILayout.Space( -24 );
		GUILayout.EndVertical();

		// BACK BUTTON //
		EditorGUILayout.BeginHorizontal();
		EditorGUI.BeginDisabledGroup( pageHistory.Count <= 1 );
		if( GUILayout.Button( "◄", titleStyle, GUILayout.Width( 24 ) ) )
			NavigateBack();
		if( pageHistory.Count > 1 )
		{
			rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		}
		EditorGUI.EndDisabledGroup();
		GUILayout.Space( -24 );

		// PAGE TITLE //
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( pageHistory[ pageHistory.Count - 1 ].pageName, titleStyle );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		// DISPLAY PAGE //
		if( pageHistory[ pageHistory.Count - 1 ].targetMethod != null )
			pageHistory[ pageHistory.Count - 1 ].targetMethod();

		Repaint();
	}

	void StartPage ()
	{
		readme.scrollValue = EditorGUILayout.BeginScrollView( readme.scrollValue, false, false );
		GUILayout.Space( 15 );
	}

	void EndPage ()
	{
		EditorGUILayout.EndScrollView();
	}

	static void ClearFocusControl ()
	{
		GUI.FocusControl( "" );
	}

	static void NavigateBack ()
	{
		readme.pageHistory.RemoveAt( readme.pageHistory.Count - 1 );
		pageHistory.RemoveAt( pageHistory.Count - 1 );
		GUI.FocusControl( "" );

		readme.scrollValue = Vector2.zero;
	}

	static void NavigateForward ( int menuIndex )
	{
		pageHistory.Add( AllPages[ menuIndex ] );
		GUI.FocusControl( "" );

		readme.pageHistory.Add( menuIndex );
		readme.scrollValue = Vector2.zero;
	}

	void MainPage ()
	{
		StartPage();

		EditorGUILayout.LabelField( "We hope that you are enjoying using the Ultimate Status Bar in your project! Here is a list of helpful resources for this asset:", paragraphStyle );

		EditorGUILayout.Space();

		if( GUILayout.Button( "  • Read the <b><color=blue>Getting Started</color></b> section of this README!", paragraphStyle ) )
			NavigateForward( 1 );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EditorGUILayout.Space();

		if( GUILayout.Button( "  • To learn more about the sections of the inspector, read the <b><color=blue>Overview</color></b> section!", paragraphStyle ) )
			NavigateForward( 2 );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EditorGUILayout.Space();

		if( GUILayout.Button( "  • Check out the <b><color=blue>Documentation</color></b> section!", paragraphStyle ) )
			NavigateForward( 5 );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EditorGUILayout.Space();

		if( GUILayout.Button( "  • Watch our <b><color=blue>Video Tutorials</color></b> on the Ultimate Status Bar!", paragraphStyle ) )
		{
			Debug.Log( "Ultimate Status Bar\nOpening YouTube Tutorials" );
			Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9Tl0VRLpo3VoU2U-SbLgwB3-" );
		}
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EditorGUILayout.Space();

		if( GUILayout.Button( "  • Join our <b><color=blue>Discord Server</color></b> so that you can get live help from us and other community members.", paragraphStyle ) )
		{
			Debug.Log( "Ultimate Status Bar\nOpening Tank & Healer Studio Discord Server" );
			Application.OpenURL( "https://discord.gg/RqwTFVs" );
		}
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EditorGUILayout.Space();

		if( GUILayout.Button( "  • <b><color=blue>Contact Us</color></b> directly with your issue! We'll try to help you out as much as we can.", paragraphStyle ) )
		{
			Debug.Log( "Ultimate Status Bar\nOpening Online Contact Form" );
			Application.OpenURL( "https://www.tankandhealerstudio.com/contact-us.html" );
		}
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Now you have the tools you need to get the Ultimate Status Bar working in your project. Now get out there and make your awesome game!", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Happy Game Making,\n" + Indent + "Tank & Healer Studio", paragraphStyle );

		GUILayout.Space( 20 );

		GUILayout.FlexibleSpace();

		if( GUILayout.Button( endPageComments[ randomComment ].comment, paragraphStyle ) )
			Application.OpenURL( endPageComments[ randomComment ].url );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EndPage();
	}

	void GettingStarted ()// EDIT: I have edited up to the first EDIT: in this method.
	{
		StartPage();

		EditorGUILayout.LabelField( "Video Introduction", sectionHeaderStyle );

		if( GUILayout.Button( Indent + "To begin, please watch the <b><color=blue>Introduction Video</color></b> from our website for the Ultimate Status Bar. This video will explain how to get started using the Ultimate Status Bar and help you understand how to implement it into your project.", paragraphStyle ) )
		{
			Debug.Log( "Ultimate Status Bar\nOpening Online Video Tutorials" );
			Application.OpenURL( "https://www.tankandhealerstudio.com/ultimate-status-bar_documentation_video-tutorials.html" );
		}
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Written Introduction", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "The Ultimate Status Bar has been built from the ground up with being easy to use and customize to make it work the way that you want.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "To begin we'll look at how to simply create and customize an Ultimate Status Bar in your scene. After that we will go over how to reference the Ultimate Status Bar in your custom scripts.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "To create an Ultimate Status Bar in your scene, simply find the Ultimate Status Bar prefab that you would like to add and drag the prefab into the scene. The Ultimate Status Bar prefab will automatically find or create a canvas in your scene for you.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Customize", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "There are many ways to use the Ultimate Status Bar within your projects. The main Ultimate Status Bar component is used to display each status within your scene, while there are two subcomponents that are all used to enhance the visual display of the Ultimate Status Bar.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "For more information about each of these scripts, please see the Overview and Documentation sections of this README. The Overview section explains how to get the components in your scene and what they do. The Documentation section explains each function available in these scripts.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Reference", sectionHeaderStyle );// EDIT: Make this a more simple example of just how to get a default status referenced. Then at the end show a quick example of another status that can be created.

		EditorGUILayout.LabelField( Indent + "The Ultimate Status Bar is very easy to get implemented into your scripts and can be implemented with one simple line of code. The one function that we are going to be using is the UpdateStatus() function, and there are several overloads of this function that make it easy to use more than one status on a single bar if need be. If you are looking for more information about the available functions in the Ultimate Status Bar class, please visit the Documentation section of this README.", paragraphStyle );

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel( "How many status?", paragraphStyle );
		numberOfStatus = ( NumberOfStatus )EditorGUILayout.EnumPopup( numberOfStatus );// EDIT: I could change this to button selection of Single and Multiple.
		EditorGUILayout.EndHorizontal();
		float imageWidth = Mathf.Min( EditorGUIUtility.currentViewWidth, 350f ) * 0.85f;

		GUILayout.Space( paragraphSpace );

		if( numberOfStatus == NumberOfStatus.Single )
		{
			EditorGUILayout.LabelField( "For this example let's use the Ultimate Status Bar to display the value of the player's health. After creating this status bar in the scene, all we need to do to get it implemented is to go to the Script Reference section of the Ultimate Status Bar inspector and assign a name to be referenced through code. In this example let's use the name: Health.", paragraphStyle );

			//float imageWidth = Mathf.Min( EditorGUIUtility.currentViewWidth, 350f ) * 0.85f;
			//Vector2 ratio = new Vector2( readme.statusInformation.width, readme.statusInformation.height ) / ( readme.statusInformation.width > readme.statusInformation.height ? readme.statusInformation.width : readme.statusInformation.height );
			//EditorGUILayout.BeginHorizontal();
			//GUILayout.FlexibleSpace();
			//GUILayout.Label( readme.statusInformation, GUILayout.Width( imageWidth * ratio.x ), GUILayout.Height( imageWidth * ratio.y ) );
			//GUILayout.FlexibleSpace();
			//EditorGUILayout.EndHorizontal();

			EditorGUILayout.LabelField( "After assigning the Status Bar Name you can copy the provided code into your script where you are modifying the players health, whether by damage or healing. Just make sure that you put the provided code after the health of the player has been modified. By doing this you will be sending in the most up to date value of the player's health. Of course, be sure to replace the currentValue and maxValue of the example code with your character's current and maximum health values. Whenever the character's health is updated, either by damage or healing done to the character, you will want to send the new information of the health's value to the Ultimate Status Bar.", paragraphStyle );

			GUILayout.Space( paragraphSpace );

			EditorGUILayout.LabelField( "", paragraphStyle );

			GUILayout.Space( paragraphSpace );

			EditorGUILayout.LabelField( "", paragraphStyle );

			GUILayout.Space( paragraphSpace );

			EditorGUILayout.LabelField( "", paragraphStyle );
		}
		else
		{
			EditorGUILayout.LabelField( Indent + "The Ultimate Status Bar is incredibly easy to get implemented into your custom scripts. There are a few ways that you can reference the Ultimate Status Bar through code, and it all depends on how many different status sections you have created on that particular Ultimate Status Bar. For more information on how to reference the Ultimate Status Bar, please see the Documentation section of this README, or the Script Reference section of the Ultimate Status Bar inspector.", paragraphStyle );

			GUILayout.Space( paragraphSpace );

			EditorGUILayout.LabelField( "For this example, we will create an Ultimate Status Bar for the Player of a simple game. Let's assume that the Player has several different status values that must be displayed. For this example, the Player will have a <i>Health</i> value, and a <i>Energy</i> value. These will need to be created inside the <b>Status Information</b> section in order to be referenced through code.", paragraphStyle );

			Vector2 ratio = new Vector2( readme.statusInformation.width, readme.statusInformation.height ) / ( readme.statusInformation.width > readme.statusInformation.height ? readme.statusInformation.width : readme.statusInformation.height );
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label( readme.statusInformation, GUILayout.Width( imageWidth * ratio.x ), GUILayout.Height( imageWidth * ratio.y ) );
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.LabelField( "After these have been created, we need to give the Ultimate Status Bar a unique name to be referenced through code. This is done in the <b>Script Reference</b> section located within the Inspector window. For this example, we are creating this status bar for the <i>Player</i>, so that's what we will name it.", paragraphStyle );

			ratio = new Vector2( readme.scriptReference.width, readme.scriptReference.height ) / ( readme.scriptReference.width > readme.scriptReference.height ? readme.scriptReference.width : readme.scriptReference.height );
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label( readme.scriptReference, GUILayout.Width( imageWidth * ratio.x ), GUILayout.Height( imageWidth * ratio.y ) );
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.LabelField( "Now that each status has been named, and the Ultimate Status Bar has a unique name that can be referenced, simply copy the code provided inside the <b>Script Reference</b> section for the desired status. Make sure that the Function option is set to: Update Status.", paragraphStyle );

			GUILayout.Space( paragraphSpace );

			EditorGUILayout.LabelField( "After copying the code that is provided, find the function in <i>your player's health script</i> where your player is receiving damage from and paste the example code into the function. Be sure to put it after the damage or healing has modified the health value. Of course, be sure to replace the currentValue and maxValue of the example code with your character's current and maximum health values. Whenever the character's health is updated, either by damage or healing done to the character, you will want to send the new information of the health's value.", paragraphStyle );

			GUILayout.Space( paragraphSpace );

			EditorGUILayout.LabelField( "This process can be used for any status that you need to be displayed to the user. For more information about the individual functions available for the Ultimate Status Bar and other components, please refer to the Documentation section of this window.", paragraphStyle );
		}
		
		EndPage();
	}

	void Overview ()
	{
		StartPage();

		EditorGUILayout.LabelField( "Sections", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The display below is mimicking the Ultimate Status Bar inspector. Expand each section to learn what each one is designed for.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// STATUS BAR POSITIONING //
		GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbarButton ) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 11 };

		showStatusBarPositioning = GUILayout.Toggle( showStatusBarPositioning, ( showStatusBarPositioning ? "▼" : "►" ) + "Status Bar Positioning", toolbarStyle );
		if( showStatusBarPositioning )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "This section handles the positioning of the Ultimate Status Bar on the canvas.", paragraphStyle );
		}
		
		EditorGUILayout.Space();

		// STATUS INFORMATION //
		showStatusInformation = GUILayout.Toggle( showStatusInformation, ( showStatusInformation ? "▼" : "►" ) + "Status Information", toolbarStyle );
		if( showStatusInformation )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "Here is where you can customize and edit each status individually. This is where you will want to create each status that needs to be a part of this status bar.", paragraphStyle );
		}

		EditorGUILayout.Space();

		// STATUS BAR OPTIONS //
		showStatusBarOptions = GUILayout.Toggle( showStatusBarOptions, ( showStatusBarOptions ? "▼" : "►" ) + "Status Bar Options", toolbarStyle );
		if( showStatusBarOptions )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "This section has options that affect the status bar as a whole. Settings for icon, text and visibility.", paragraphStyle );
		}

		EditorGUILayout.Space();

		// SCRIPT REFERENCE //
		showScriptReference = GUILayout.Toggle( showScriptReference, ( showScriptReference ? "▼" : "►" ) + "Script Reference", toolbarStyle );
		if( showScriptReference )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "In this section you will be able to setup the reference to this Ultimate Status Bar, and you will be provided with code examples to be able to copy and paste into your own scripts.", paragraphStyle );
		}

		GUILayout.Space( sectionSpace );

		// TOOLTIPS //
		EditorGUILayout.LabelField( "Tooltips", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To learn more about each option in these sections, please select the Ultimate Status Bar in your scene, and hover over each property to read the provided tooltip.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		// ADDITIONAL COMPONENTS //
		EditorGUILayout.LabelField( "Additional Components", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The Ultimate Status Bar has two extra components that can help add to the visual display of your status bars. To learn more about these components, please visit the sections below.", paragraphStyle );

		if( GUILayout.Button( "AlternateStateHandler.cs", itemHeaderStyle ) )
		{
			NavigateForward( 3 );
			ClearFocusControl();
		}
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.Space( paragraphSpace );

		if( GUILayout.Button( "StatusFillFollower.cs", itemHeaderStyle ) )
		{
			NavigateForward( 4 );
			ClearFocusControl();
		}
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EndPage();
	}

	void Overview_AlternateStateHandler ()
	{
		StartPage();

		EditorGUILayout.LabelField( "AlternateStateHandler.cs", sectionHeaderStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Summary", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The <i>Alternate State Handler</i> component allows you to display different states for each status in your scene. This can be done using images or text. An example for this would be making the Health bar of the player flash when it is at a critical amount. However, you can use this component in what ever way your project needs.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		/* HOW TO CREATE */
		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The Alternate State Handler is very easy to get started. Simply add the Alternate State Handler component to the Ultimate Status Bar GameObject. After selecting the Ultimate Status Bar GameObject you can add this component in the Inspector by selecting: <i>Add Component / UI / Ultimate Status Bar / Alternate State Handler.</i>", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Inspector", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The display below is mimicking the Alternate State Handler inspector. Expand each section to learn what each one is designed for.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbarButton ) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 11 };
		
		showAlternateStates = GUILayout.Toggle( showAlternateStates, ( showAlternateStates ? "▼" : "►" ) + "Alternate States", toolbarStyle );
		if( showAlternateStates )
		{
			GUILayout.Space( paragraphSpace );

			EditorGUILayout.LabelField( "Here is where you can customize each state that you want for the status bar.", paragraphStyle );
		}

		EditorGUILayout.Space();
		
		altShowScriptRefernce = GUILayout.Toggle( altShowScriptRefernce, ( altShowScriptRefernce ? "▼" : "►" ) + "Script Reference", toolbarStyle );
		if( altShowScriptRefernce )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "In this section you will be able to setup the reference to this Alternate State Handler, and you will be provided with code examples to be able to copy and paste into your own scripts.", paragraphStyle );
		}
		
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Tooltips", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To learn more about each option on this component, please select the Alternate State Handler in your scene and hover over each item to read the provided tooltip.", paragraphStyle );

		EndPage();
	}

	void Overview_StatusFillFollower ()
	{
		StartPage();

		EditorGUILayout.LabelField( "StatusFillFollower.cs", sectionHeaderStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Summary", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The <i>Status Fill Follower</i> component can be used to follow the current fill of an image to draw attention to the amount that it is at.", paragraphStyle );

		GUILayout.Space( sectionSpace );
		
		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To create a Status Fill Follower for your Ultimate Status Bar you will need to create a new image or text that is a child of the Ultimate Status Bar. To do this, you can right click on the Ultimate Status Bar GameObject and select: <i>UI / Image (Text)</i>. Then simply add the Status Fill Follower component to the new GameObject. This can be done by selecting: <i>Add Component / UI / Ultimate Status Bar / Status Fill Follower.</i>", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Inspector", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "Once you have created a Status Fill Follower within the Ultimate Status Bar game object, use the transform gizmo in the scene view to move the Status Fill Follower object to the minimum position that you want it to follow and click the 'Set' button in the Position Anchors box in the inspector. Then move the object to the maximum position and click the 'Set' button. Now you can adjust the Test Value slider and see the object moving as the status bar updates.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Inspector Tooltips", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To learn more about each option on this component, please select the Status Fill Follower in your scene and hover over each item to read the provided tooltip.", paragraphStyle );

		EndPage();
	}

	void Documentation ()
	{
		StartPage();

		EditorGUILayout.LabelField( "Introduction", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "Welcome to the Documentation section. This section will go over the various functions that you have available. Please click on the class to learn more about each function.", paragraphStyle );
		
		GUILayout.Space( sectionSpace );

		// UltimateStatusBar.cs
		EditorGUILayout.LabelField( "UltimateStatusBar.cs", itemHeaderStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			NavigateForward( 6 );
			ClearFocusControl();
		}
		
		// AlternateStateHandler.cs
		EditorGUILayout.LabelField( "AlternateStateHandler.cs", itemHeaderStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			NavigateForward( 8 );
			ClearFocusControl();
		}

		EndPage();
	}

	void Documentation_UltimateStatusBar ()
	{
		StartPage();
		
		// STATIC FUNCTIONS //
		EditorGUILayout.LabelField( "Static Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "The following functions can be referenced from your scripts without the need for an assigned local Ultimate Status Bar variable. However, each function must have the targeted Ultimate Status Bar name in order to find the correct Ultimate Status Bar in the scene. Each example code provided uses the name <i>Player</i> as the status bar name.", paragraphStyle );
		
		Vector2 ratio = new Vector2( readme.scriptReference.width, readme.scriptReference.height ) / ( readme.scriptReference.width > readme.scriptReference.height ? readme.scriptReference.width : readme.scriptReference.height );

		float imageWidth = readme.scriptReference.width > Screen.width - 50 ? Screen.width - 50 : readme.scriptReference.width;

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( readme.scriptReference, GUILayout.Width( imageWidth ), GUILayout.Height( imageWidth * ratio.y ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateStatusBar_StaticFunctions.Length; i++ )
			ShowDocumentation( UltimateStatusBar_StaticFunctions[ i ] );

		GUILayout.Space( sectionSpace );

		// PUBLIC FUNCTIONS //
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to the Ultimate Status Bar class. Each example provided relies on having a Ultimate Status Bar variable named 'statusBar' stored inside your script. When using any of the example code provided, make sure that you have a Ultimate Status Bar variable like the one below:", paragraphStyle );

		EditorGUILayout.TextArea( "// Place this in your public variables and assign it in the inspector. //\npublic UltimateStatusBar statusBar;", GUI.skin.textArea );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Please click on the function name to learn more.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateStatusBar_PublicFunctions.Length; i++ )
			ShowDocumentation( UltimateStatusBar_PublicFunctions[ i ] );

		GUILayout.Space( sectionSpace );

		// PUBLIC CLASSES //
		EditorGUILayout.LabelField( "Public Classes", sectionHeaderStyle );

		GUILayout.Space( itemHeaderSpace );
		
		EditorGUILayout.LabelField( "UltimateStatus", itemHeaderStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			NavigateForward( 7 );
			ClearFocusControl();
		}

		GUILayout.Space( sectionSpace );

		// PUBLIC ACCESSORS //
		EditorGUILayout.LabelField( "Public Accessors", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateStatusBar_PublicAccessors.Length; i++ )
			ShowDocumentation( UltimateStatusBar_PublicAccessors[ i ] );

		GUILayout.Space( sectionSpace );

		// CALLBACKS //
		EditorGUILayout.LabelField( "Callbacks", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateStatusBar_Callbacks.Length; i++ )
			ShowDocumentation( UltimateStatusBar_Callbacks[ i ] );

		EndPage();
	}

	void Documentation_UltimateStatus ()
	{
		StartPage();

		// PUBLIC FUNCTIONS //
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to an Ultimate Status class. Since this class is nested under the Ultimate Status Bar class, you will need to have a reference to an Ultimate Status Bar component first, like the code below:", paragraphStyle );

		EditorGUILayout.TextArea( "// Place these with your variables and assign the Ultimate Status Bar in the inspector. //\npublic UltimateStatusBar statusBar;\nUltimateStatusBar.UltimateStatus status;", GUI.skin.textArea );

		EditorGUILayout.LabelField( Indent + "After you have a reference to an Ultimate Status Bar you can get a reference to an Ultimate Status component. The examples provided rely on having an Ultimate Status variable named 'status' stored inside your script. When using any of the example code provided, make sure that you have a Ultimate Status variable like the one below:", paragraphStyle );

		EditorGUILayout.TextArea( "// Place this in the Start() function. //\nstatus = statusBar.GetUltimateStatus( \"Status Name\" );", GUI.skin.textArea );

		EditorGUILayout.LabelField( "Be sure to change the \"Status Name\" to the name of your status that you are trying to access.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Please click on the function name to learn more.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateStatus_PublicFunctions.Length; i++ )
			ShowDocumentation( UltimateStatus_PublicFunctions[ i ] );

		GUILayout.Space( sectionSpace );

		// PUBLIC ACCESSORS //
		EditorGUILayout.LabelField( "Public Accessors", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateStatus_PublicAccessors.Length; i++ )
			ShowDocumentation( UltimateStatus_PublicAccessors[ i ] );

		GUILayout.Space( sectionSpace );

		// CALLBACKS //
		EditorGUILayout.LabelField( "Callbacks", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateStatus_Callbacks.Length; i++ )
			ShowDocumentation( UltimateStatus_Callbacks[ i ] );

		EndPage();
	}

	void Documentation_AlternateStateHandler ()
	{
		StartPage();

		// STATIC FUNCTIONS //
		EditorGUILayout.LabelField( "Static Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All static functions require a string to be passed through the function first. Each Alternate State Handler is registered with the targeted Ultimate Status Bar's name. The <i>statusBarName</i> parameter is used to locate the targeted Alternate State Handler from a static list of Alternate State Handler classes that have been stored.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < AlternateStateHandler_StaticFunctions.Length; i++ )
			ShowDocumentation( AlternateStateHandler_StaticFunctions[ i ] );

		GUILayout.Space( sectionSpace );

		// PUBLIC FUNCTIONS //
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to the Alternate State Handler class. Each example provided relies on having a Alternate State Handler variable named 'altStateHandler' stored inside your script. When using any of the example code provided, make sure that you have a Alternate State Handler variable like the one below:", paragraphStyle );

		EditorGUILayout.TextArea( "// Place this in your public variables and assign it in the inspector. //\npublic AlternateStateHandler altStateHandler;", GUI.skin.textArea );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Please click on the function name to learn more.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < AlternateStateHandler_PublicFunctions.Length; i++ )
			ShowDocumentation( AlternateStateHandler_PublicFunctions[ i ] );
		
		EndPage();
	}

	void VersionHistory ()
	{
		StartPage();

		for( int i = 0; i < readme.versionHistory.Length; i++ )
		{
			EditorGUILayout.LabelField( "Version " + readme.versionHistory[ i ].versionNumber, itemHeaderStyle );

			for( int n = 0; n < readme.versionHistory[ i ].changes.Length; n++ )
				EditorGUILayout.LabelField( "  • " + readme.versionHistory[ i ].changes[ n ] + ".", paragraphStyle );

			if( i < ( readme.versionHistory.Length - 1 ) )
				GUILayout.Space( itemHeaderSpace );
		}

		EndPage();
	}

	void ImportantChange ()// EDIT: Just the video link now.
	{
		StartPage();

		EditorGUILayout.LabelField( Indent + "Thank you for downloading the most recent version of the Ultimate Status Bar. This update was very big and even though we tried our best to allow for existing projects to continue using the Ultimate Status Bar without any change, some of the previous functionality may have been removed which may cause issues with existing projects. Before you do anything, please take just a few moments to watch this video that we made to help you get the Ultimate Status Bar working in your project after the new update:", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		if( GUILayout.Button( "Ultimate Status Bar - 3.0.0 Important Changes", itemHeaderStyle ) )
		{
			Debug.Log( "Ultimate Status Bar\nOpening 3.0.0 Important Changes Video" );
			Application.OpenURL( "https://youtu.be/B6V6wgS2qpU" );// EDIT:
		}
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Depreciated Functions and Events", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The main function for referencing the Ultimate Status Bar has been depreciated. This means that it will still function as it did previously, but the new method should be implemented as soon as possible. The new method of referencing the Ultimate Status Bar is more straight forward and easier to use. Below is a list of the main functions that you may have been using, as well as a quick example of how to replace it with the new method.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "public static void UpdateStatus()", itemHeaderStyle );
		EditorGUILayout.LabelField( "This function allowed you to update a specific status of an Ultimate Status Bar. The only thing that changed about the new reference is the parameter order, which allows for all of the UpdateStatus functions to have a similar format which will hopefully make referencing easier to use. Here is an example:", paragraphStyle );
		EditorGUILayout.LabelField( "UpdateStatus( \"Player\", currentHealth, maxHealth, \"Health\" );", paragraphStyle );

		EditorGUILayout.LabelField( "Conclusion", sectionHeaderStyle );
		EditorGUILayout.LabelField( "For a full list of changes please click on the version number in the title at the top of this README. There are a few more depreciated functions that you may have been using. As always, if you run into any issues with this asset, please contact us at:", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( sectionSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Got it!", GUILayout.Width( Screen.width / 2 ) ) )
			NavigateBack();

		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}

	void ThankYou ()
	{
		StartPage();

		EditorGUILayout.LabelField( "The two of us at Tank & Healer Studio would like to thank you for purchasing the Ultimate Status Bar asset package from the Unity Asset Store.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "We hope that the Ultimate Status Bar will be a great help to you in the development of your game. After clicking the <i>Continue</i> button below, you will be presented with information to assist you in getting to know the Ultimate Status Bar and getting it implementing into your project.", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "You can access this information at any time by clicking on the <b>README</b> file inside the Ultimate Status Bar folder.", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Again, thank you for downloading the Ultimate Status Bar. We hope that your project is a success!", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Happy Game Making,\n" + Indent + "Tank & Healer Studio", paragraphStyle );

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Continue", GUILayout.Width( Screen.width / 2 ) ) )
			NavigateBack();

		var rect2 = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect2, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}

	void Settings ()
	{
		StartPage();
		
		if( EditorPrefs.GetBool( "UUI_DevelopmentMode" ) )
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField( "Development Mode", sectionHeaderStyle );
			base.OnInspectorGUI();
			EditorGUILayout.Space();
		}

		GUILayout.FlexibleSpace();

		GUILayout.Space( sectionSpace );

		EditorGUI.BeginChangeCheck();
		GUILayout.Toggle( EditorPrefs.GetBool( "UUI_DevelopmentMode" ), ( EditorPrefs.GetBool( "UUI_DevelopmentMode" ) ? "Disable" : "Enable" ) + " Development Mode", EditorStyles.radioButton );
		if( EditorGUI.EndChangeCheck() )
		{
			if( EditorPrefs.GetBool( "UUI_DevelopmentMode" ) == false )
			{
				if( EditorUtility.DisplayDialog( "Enable Development Mode", "Are you sure you want to enable development mode for Tank & Healer Studio assets? This mode will allow you to see the default inspector for this asset which is useful when adding variables to this script without having to edit the custom editor script.", "Enable", "Cancel" ) )
				{
					EditorPrefs.SetBool( "UUI_DevelopmentMode", !EditorPrefs.GetBool( "UUI_DevelopmentMode" ) );
				}
			}
			else
				EditorPrefs.SetBool( "UUI_DevelopmentMode", !EditorPrefs.GetBool( "UUI_DevelopmentMode" ) );
		}

		EndPage();
	}
	
	void ShowDocumentation ( DocumentationInfo info )
	{
		GUILayout.Space( paragraphSpace );

		if( GUILayout.Button( info.functionName, itemHeaderStyle ) )
		{
			info.showMore = !info.showMore;
			GUI.FocusControl( "" );
		}
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		if( info.showMore )
		{
			EditorGUILayout.LabelField( Indent + "<i>Description:</i> " + info.description, paragraphStyle );

			if( info.parameter != null )
			{
				for( int i = 0; i < info.parameter.Length; i++ )
					EditorGUILayout.LabelField( Indent + "<i>Parameter:</i> " + info.parameter[ i ], paragraphStyle );
			}
			if( info.returnType != string.Empty )
				EditorGUILayout.LabelField( Indent + "<i>Return type:</i> " + info.returnType, paragraphStyle );

			if( info.codeExample != string.Empty )
				EditorGUILayout.TextArea( info.codeExample, GUI.skin.textArea );

			GUILayout.Space( paragraphSpace );
		}
	}

	public static void OpenReadmeDocumentation ()
	{
		SelectReadmeFile();

		if( !pageHistory.Contains( AllPages[ 3 ] ) )
			NavigateForward( 5 );

		if( !pageHistory.Contains( AllPages[ 4 ] ) )
			NavigateForward( 6 );
	}

	[MenuItem( "Window/Tank and Healer Studio/Ultimate Status Bar", false, 10 )]
	public static void SelectReadmeFile ()
	{
		var ids = AssetDatabase.FindAssets( "README t:UltimateStatusBarReadme" );
		if( ids.Length == 1 )
		{
			var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );
			Selection.objects = new Object[] { readmeObject };
			readme = ( UltimateStatusBarReadme )readmeObject;
		}
		else
		{
			Debug.LogError( "There is no README object in the Ultimate Status Bar folder." );
		}
	}
}
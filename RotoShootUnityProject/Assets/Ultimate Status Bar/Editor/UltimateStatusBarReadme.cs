/* UltimateStatusBarReadme.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using System.Collections.Generic;

//[CreateAssetMenu( fileName = "README", menuName = "ScriptableObjects/ReadmeScriptableObject", order = 1 )]
public class UltimateStatusBarReadme : ScriptableObject
{
	public static int ImportantChange = 4;// UPDATE ON IMPORTANT CHANGES // 5 >= 3.0.0 / 4 >= 2.6.0 / 3 >= 2.5.0 / 2 >= 2.1.0 - 2.1.3 / 1 >= 2.0.0 - 2.0.3
	public Texture2D icon;
	public Texture2D scriptReference, statusInformation;
	public Texture2D settings;
	
	public class VersionHistory
	{
		public string versionNumber = "";
		public string[] changes;
	}
	public VersionHistory[] versionHistory = new VersionHistory[]
	{
		// VERSION 3.0.0
		new VersionHistory()
		{
			versionNumber = "3.0.0",
			changes = new string[]
			{
				// GENERAL //
				"Revamped the positioning options of the status bar on the screen. Additionally, improved functionality to support positioning on Screen Space Overlay, World Space, and Screen Space Camera canvas setups",
				"Replaced the GetStatusBarState accessor with IsEnabled",
				"Improved Smooth Fill functionality",
				"Added options for constraining the status fill to a certain number of ticks or notches",
				"Overall improvements to the Ultimate Status class functionality and performance",
				"Simplified reference to the Ultimate Status Bar class and simplified internal calculations for referencing each status bar in the scene",
				"Streamlined the reference for each status in the scene, regardless of how many status are on one bar. This makes the reference a lot easier and more straightforward",
				"Major improvements to the Ultimate Status Bar inspector to save time and improve overall workflow",
				"Updated existing prefabs and added new ones",
				"Added new action events for developers to easily expand functionality if desired",
				"Included new and improved status bar textures and included a new style: Dark",
				"Improved the README file to stay on the same page even after Unity compiles scripts",
				// REMOVED //
				"Removed 'World Space' options for following camera rotation from the status bar positioning and created a new function to handle following the camera rotation for any World Space canvas",
				"Removed the Dramatic Status Fill script and integrated that functionality directly into the Ultimate Status Bar script",
				"Removed various accessors from the Ultimate Status class that were likely unused from outside scripts (GetCurrentFraction, GetMaxValue, GetTargetFill, GetCurrentCalculatedFraction)",
				"Removed the two functions for updating the Color (UpdateStatusColor) and Text Color (UpdateStatusTextColor) from the Ultimate Status class. Replaced them with accessors (.color and .textColor)",
				"Removed function: UpdateStatus (string, float, float) because it conflicted with the new static reference for the default status of a targeted status bar. This function still exists but the parameters have been swapped so that it flows better with the other UpdateStatus functions. The new function is: UpdateStatus (float, float, string) with the string being the targeted name of the status",
				"Removed all the old status bar textures and replaced them with new revamped textures",
				// ADDED METHODS //
				"Added two new functions for the Ultimate Status Bar to follow a world object's position on the screen. These functions are: StartFollowTransform and StopFollowTransform",
				"Added two new functions for a world space Ultimate Status Bar to look at the camera. These functions are: StartLookAtCamera and StopLookAtCamera",
				"Added two new functions to enable and disable the status bar visually. These functions are: EnableStatusBar and DisableStatusBar",
				"Added new function: UpdateStatus ( float currentValue, float maxValue, float statusName )",
				"Added new static function: UpdatePositioning",
				"Added two new static functions to enable and disable the targeted status bar visually. These functions are: EnableStatusBar and DisableStatusBar",
				"Added new static function: UpdateStatus ( string barName, float currentValue, float maxValue, string statusName)",
				// DEPRECIATED METHODS //
				"Depreciated function: ShowStatusBar",
				"Depreciated function: HideStatusBar",
				"Depreciated function: UpdateStatusBarIcon",
				"Depreciated function: UpdateStatusBarText",
				"Depreciated current static function: UpdateStatus. This function has been replaced with a new UpdateStatus function with a slight change to the parameters. Doing this allows for an easier and more consistent reference of the status bar",
				"Depreciated static function: UpdateStatusBarIcon",
				"Depreciated static function: UpdateStatusBarText",
			},
		},
		// VERSION 2.6.0
		new VersionHistory ()
		{
			versionNumber = "2.6.0",
			changes = new string[]
			{
				"Simplified the editor script internally",
				"Removed AnimBool functionality from the inspector to avoid errors with Unity 2019+",
				"Added coding comments to the AlternateStateHandler.cs script since they were missing",
				"Added new script: UltimateStatusBarReadme.cs",
				"Added new script: UltimateStatusBarReadmeEditor.cs",
				"Added new file at the Ultimate Status Bar root folder: README. This file has all the documentation and how to information",
				"Removed the UltimateStatusBarWindow.cs file. All of that information and more is now located in the README file",
				"Removed the old README text file. All of that information and more is now located in the README file",
			},
		},
		// VERSION 2.5.0
		new VersionHistory ()
		{
			versionNumber = "2.5.0",
			changes = new string[]
			{
				"Restructured the folders to help conform with the Unity standard for Assets",
				"Made some pretty big changes and improvements to the Documentation Window",
				"Updated and fixed some minor things inside the Status Fill Follower script",
				"Removed the UltimateStatusBarUpdater class out of the UltimateStatusBar class as this would cause errors in some rare cases. A new script has been added to fix these errors. The script is named UltimateStatusBarScreenSizeUpdater",
			},
		},
	};

	[HideInInspector]
	public List<int> pageHistory = new List<int>();
	[HideInInspector]
	public Vector2 scrollValue = new Vector2();
}
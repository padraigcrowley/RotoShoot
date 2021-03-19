/* StatusFillFollower.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu( "UI/Ultimate Status Bar/Status Fill Follower" )]
public class StatusFillFollower : MonoBehaviour
{
	// ----- < ULTIMATE STATUS BAR > ----- //
	public UltimateStatusBar ultimateStatusBar;
	public int statusIndex = 0;
	
	// ----- < SCALING AND RATIO > ----- //
	RectTransform baseTransform;
	public enum ImageAspectRatio
	{
		Preserve,
		Custom
	}
	public ImageAspectRatio imageAspectRatio = ImageAspectRatio.Custom;
	public Image targetImage;
	public float xRatio = 1.0f, yRatio = 1.0f;
	public float imageSize = 1.0f;

	// ----- < POSITIONS > ----- //
	public Vector2 minimumPosition = Vector3.zero;
	public Vector2 maximumPosition = Vector3.zero;
	Vector2 _minimumPosition = Vector3.zero;
	Vector2 _maximumPosition = Vector3.zero;

	
	void Start ()
	{
		// If the application is not running, then return.
		if( !Application.isPlaying )
			return;

		// If the Ultimate Status Bar variable is unassigned, then warn the user and return.
		if( ultimateStatusBar == null )
		{
			Debug.LogError( "Status Fill Follower\nThe Ultimate Status Bar component has not been assigned. Disabling this component to avoid errors." );
			enabled = false;
			return;
		}

		// Subscribe to the OnStatusUpdated function for the targeted Ultimate Status.
		ultimateStatusBar.UltimateStatusList[ statusIndex ].OnStatusUpdated += OnStatusUpdated;
		
		// Subscribe to the positioning function of the Ultimate Status Bar.
		ultimateStatusBar.OnUpdatePositioning += UpdatePositioning;

		// Update the positioning.
		UpdatePositioning();
	}
	
	/// <summary>
	/// This function gets subscribed to the Ultimate Status OnStatusUpdated function.
	/// </summary>
	/// <param name="currVal">The current percentage to apply to the position of the transform.</param>
	public void OnStatusUpdated ( float currVal )
	{
		// If the application is only running in the editor, then check the fill constraints for the Ultimate Status. Since the user can be changing values, if the fill constraints are the same it will throw an error, so return.
		if( !Application.isPlaying && ultimateStatusBar.UltimateStatusList[ statusIndex ].fillConstraintMin == ultimateStatusBar.UltimateStatusList[ statusIndex ].fillConstraintMax )
			return;

		// Lerp the position from the minimum to the maximum by the current value.
		baseTransform.localPosition = Vector3.Lerp( _minimumPosition, _maximumPosition, currVal );
	}

	#if UNITY_EDITOR
	void Update ()
	{
		if( !Application.isPlaying )
			UpdatePositioning();
	}
	#endif

	/// <summary>
	/// Updates the size and positioning in relation to the Ultimate Status Bar.
	/// </summary>
	void UpdatePositioning ()
	{
		// If the Ultimate Status Bar variable is unassigned or the parent canvas is null, then return.
		if( ultimateStatusBar == null || ultimateStatusBar.ParentCanvas == null )
			return;

		// If the statusIndex is out of range, then return.
		if( ultimateStatusBar.UltimateStatusList.Count <= statusIndex )
			return;
		
		// If the baseTransform variable is unassigned...
		if( baseTransform == null )
		{
			// Assign the transform component.
			baseTransform = GetComponent<RectTransform>();

			// If the baseTransform variable is still null...
			if( baseTransform == null )
			{
				// If the application is running, then inform the user.
				if( Application.isPlaying )
					Debug.LogError( "Status Fill Follower\nThere is no RectTransform component attached to this gameObject." );

				return;
			}
		}

		// Apply a scale of one.
		baseTransform.localScale = Vector3.one;
		
		// If the user is wanting to preserve the aspect ratio of the selected image...
		if( imageAspectRatio == ImageAspectRatio.Preserve )
		{
			// If the targetImage variable has been left unassigned, then inform the user and return.
			if( targetImage == null )
			{
				// If the application is running, then inform the user of the error.
				if( Application.isPlaying )
					Debug.LogError( "Status Fill Follower\nThe Target Image has not been assigned." );

				return;
			}
			
			// If the target image does not have a sprite, then return.
			if( targetImage.sprite == null )
				return;

			// Store the raw values of the sprites ratio so that a smaller value can be configured.
			Vector2 rawRatio = new Vector2( targetImage.sprite.rect.width, targetImage.sprite.rect.height );

			// Temporary float to store the largest side of the sprite.
			float maxValue = rawRatio.x > rawRatio.y ? rawRatio.x : rawRatio.y;

			// Now configure the ratio based on the above information.
			xRatio = rawRatio.x / maxValue;
			yRatio = rawRatio.y / maxValue;
		}

		// Set the reference size according to the Scale Direction option.
		float referenceSize = ultimateStatusBar.BaseTransform.sizeDelta.y > ultimateStatusBar.BaseTransform.sizeDelta.x ? ultimateStatusBar.BaseTransform.sizeDelta.y : ultimateStatusBar.BaseTransform.sizeDelta.x;

		// Configure the size of the image.
		float textureSize = referenceSize * imageSize;

		// Apply the size to the image along with the ratio options.
		baseTransform.sizeDelta = new Vector2( textureSize * xRatio, textureSize * yRatio );

		// Configure the minimum position according to the size of the Ultimate Status Bar.
		Vector3 tempVec = Vector3.zero;
		tempVec.x -= ( minimumPosition.x * ultimateStatusBar.BaseTransform.sizeDelta.x );
		tempVec.y -= ( minimumPosition.y * ultimateStatusBar.BaseTransform.sizeDelta.y );
		_minimumPosition = tempVec;

		// Configure the maximum position according to the size of the Ultimate Status Bar.
		Vector3 tempVecFull = Vector3.zero;
		tempVecFull.x -= ( maximumPosition.x * ultimateStatusBar.BaseTransform.sizeDelta.x );
		tempVecFull.y -= ( maximumPosition.y * ultimateStatusBar.BaseTransform.sizeDelta.y );
		_maximumPosition = tempVecFull;

		// Configure the calculated percentage of the status bar.
		float calculatedPercentage = ultimateStatusBar.UltimateStatusList[ statusIndex ].fillConstraint ? ( ultimateStatusBar.UltimateStatusList[ statusIndex ].statusImage.fillAmount - ultimateStatusBar.UltimateStatusList[ statusIndex ].fillConstraintMin ) / ( ultimateStatusBar.UltimateStatusList[ statusIndex ].fillConstraintMax - ultimateStatusBar.UltimateStatusList[ statusIndex ].fillConstraintMin ) : ultimateStatusBar.UltimateStatusList[ statusIndex ].statusImage.fillAmount;
		
		// IF this is inside the Unity Editor, then only update the Fill Follower if this object is not currently selected. This will allow the position options to be edited without the Fill Follower updating.
		#if UNITY_EDITOR
		if( !Application.isPlaying && UnityEditor.Selection.activeGameObject != gameObject && ultimateStatusBar != null && ultimateStatusBar.UltimateStatusList[ statusIndex ].statusImage != null )
			OnStatusUpdated( calculatedPercentage );
		#endif

		// If the application is playing, then update this so that it displays correctly.
		if( Application.isPlaying )
			OnStatusUpdated( calculatedPercentage );
	}
}
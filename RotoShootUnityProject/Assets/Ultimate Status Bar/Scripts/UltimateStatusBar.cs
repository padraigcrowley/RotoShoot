/* UltimateStatusBar.cs */
/* Written by Kaz Crowe */
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//#pragma warning disable IDE1006// NOTE: This line is only to disable warnings about some accessor names which were named that way to mimic Unity's accessors.

//[ExecuteInEditMode]
[ExecuteAlways]
[RequireComponent( typeof( CanvasGroup ) )]
[AddComponentMenu( "UI/Ultimate Status Bar/Ultimate Status Bar" )]
public class UltimateStatusBar : MonoBehaviour
{
	// INTERNAL CALCULATIONS //
	/// <summary>
	/// The parent canvas component that this status bar is a child of.
	/// </summary>
	public Canvas ParentCanvas
	{
		get;
		private set;
	}
	RectTransform canvasRectTrans;
	/// <summary>
	/// The base RectTransform component of this status bar.
	/// </summary>
	public RectTransform BaseTransform
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the current visual state of the status bar.
	/// </summary>
	public bool IsEnabled
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns if the status bar is following the position of a transform in the world.
	/// </summary>
	public bool IsFollowingTransform
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns if the status bar is following the camera rotation.
	/// </summary>
	public bool IsFollowingCamera
	{
		get;
		private set;
	}

	// STATUS BAR POSITIONING //
	public enum Positioning
	{
		Enabled,
		Disabled
	}
	public Positioning positioning = Positioning.Enabled;
	public enum ScalingAxis
	{
		Height,
		Width
	}
	public ScalingAxis scalingAxis = ScalingAxis.Width;
	public float statusBarSize = 1.75f;
	public enum ImageAspectRatio
	{
		Preserve,
		Custom
	}
	public ImageAspectRatio imageAspectRatio = ImageAspectRatio.Preserve;
	public Image targetImage;
	[Range( 0.0f, 1.0f )]
	public float xRatio = 1.0f, yRatio = 1.0f;
	[Range( 0.0f, 100.0f )]
	public float horizontalPosition = 50.0f, verticalPosition = 50.0f;
	public float depthPosition = 0.0f;

	// STATUS BAR OPTIONS //
	public Image statusBarIcon;
	public Sprite icon
	{
		get
		{
			if( statusBarIcon == null )
			{
				Debug.LogWarning( "Ultimate Status Bar\nNo Image component has been assigned to be used as an icon." );
				return null;
			}

			return statusBarIcon.sprite;
		}
		set
		{
			if( statusBarIcon == null )
			{
				Debug.LogWarning( "Ultimate Status Bar\nNo Image component has been assigned to be used as an icon." );
				return;
			}

			statusBarIcon.sprite = value;
		}
	}
	public Text statusBarText;
	public string text
	{
		get
		{
			if( statusBarText == null )
			{
				Debug.LogWarning( "Ultimate Status Bar\nNo Text component has been assigned for the status bar text." );
				return "";
			}

			return statusBarText.text;
		}
		set
		{
			if( statusBarText == null )
			{
				Debug.LogWarning( "Ultimate Status Bar\nNo Text component has been assigned for the status bar text." );
				return;
			}

			statusBarText.text = value;
		}
	}
	public enum UpdateVisibility
	{
		Instant,
		Fade,
		Animation
	}
	public UpdateVisibility updateVisibility = UpdateVisibility.Instant;
	public float idleSeconds = 2.0f;
	public float enableDuration = 1.0f, disableDuration = 1.0f;
	public float enabledAlpha = 1.0f, disabledAlpha = 0.0f;
	float enabledSpeed = 1.0f, disabledSpeed = 1.0f;
	bool isFading = false, isCountingDown = false;
	float countdownTime = 0.0f;
	public enum InitialState
	{
		Enabled,
		Disabled
	}
	public InitialState initialState = InitialState.Enabled;
	bool forceVisible = false;
	public Animator statusBarAnimator;
	CanvasGroup statusBarGroup;
	bool canvasGroupInteractable = false;
	bool canvasGroupBlockRaycasts = false;
	bool canvasGroupIgnoreParentGroup = false;
	public bool onStatusUpdated = false;
	
	// STATUS INFORMATION //
	[Serializable]
	public class UltimateStatus
	{
		// ACCESSORS //
		/// <summary>
		/// Returns the state of this status forcing the status bar to be visible determined by the users options.
		/// </summary>
		public bool ForceVisible
		{
			get;
			private set;
		}
		/// <summary>
		/// The calculated percentage of the status bar percentage at this frame.
		/// </summary>
		public float CalculatedPercentage
		{
			get;
			private set;
		}
		/// <summary>
		/// Sets the text color to the provided value.
		/// </summary>
		public Color textColor
		{
			get
			{
				if( displayText == DisplayText.Disabled || statusText == null )
					return Color.white;

				return statusText.color;
			}
			set
			{
				if( displayText == DisplayText.Disabled || statusText == null )
					return;

				statusText.color = value;
			}
		}
		/// <summary>
		/// Sets the status image color to the provided value.
		/// </summary>
		public Color color
		{
			get
			{
				if( colorMode != ColorMode.Single || statusImage == null )
					return Color.white;

				return statusImage.color;
			}
			set
			{
				if( colorMode != ColorMode.Single || statusImage == null )
					return;

				statusImage.color = value;
			}
		}

		// INTERNAL CALCULATIONS //
		float currentPercentage;
		float maxValue;
		float targetFill;

		// REFERENCE //
		public UltimateStatusBar statusBar;
		public string statusName = "";

		// COLOR OPTIONS //
		public enum ColorMode
		{
			Single,
			Gradient
		}
		public ColorMode colorMode;
		public Image statusImage;
		public Gradient statusGradient = new Gradient();

		// FILL CONSTRAINT //
		public bool fillConstraint = false;
		public float fillConstraintMin = 0.0f;
		public float fillConstraintMax = 1.0f;
		public int fillConstraintTicks = 0;
		float fillConstraintRounding
		{
			get
			{
				return ( fillConstraintMax - fillConstraintMin ) / fillConstraintTicks;
			}
		}

		// TEXT OPTIONS //
		public enum DisplayText
		{
			Disabled,
			Percentage,
			CurrentValue,
			CurrentAndMaxValues
		}
		public DisplayText displayText;
		public Text statusText;
		public string additionalText = string.Empty;

		// SMOOTH FILL //
		public bool smoothFill = false;
		public float smoothFillSpeed = 1.0f;
		int fillDirection = 0;
		bool isSmoothing = false;
		
		// VISIBILITY CONFIGURATION //
		public bool keepVisible = false;
		public float triggerValue = 0.25f;

		// DRAMATIC STATUS FILL //
		public bool useDramaticFill = false;
		public Image dramaticImage;
		public enum DramaticStyle
		{
			Increase,
			Decrease
		}
		public DramaticStyle dramaticStyle = DramaticStyle.Decrease;
		bool isDramaticFilling = false;
		float _secondsDelay = 0.1f;
		public float secondsDelay = 0.1f;
		public float resetSensitivity = 0.1f;
		float previousFillAmt = 0.0f;
		public float fillSpeed = 0.5f;
		
		// CALLBACKS //
		public event Action<float> OnStatusUpdated;
		

		/// <summary>
		/// Updates the status bar with the current and max values.
		/// </summary>
		/// <param name="currentValue">The current value of the status.</param>
		/// <param name="maxValue">The maximum value of the status.</param>
		public void UpdateStatus ( float currentValue, float maxValue )
		{
			// If the status bar is left unassigned, then return.
			if( statusImage == null )
				return;
			
			// Fix the value to be a percentage.
			currentPercentage = currentValue / maxValue;

			// If the value is greater than 1 or less than 0, then fix the values to being min/max.
			if( currentPercentage < 0 || currentPercentage > 1 )
				currentPercentage = currentPercentage < 0 ? 0 : 1;

			// Store the target amount of fill according to the users options.
			targetFill = fillConstraint ? Mathf.Lerp( fillConstraintMin, fillConstraintMax, currentPercentage ) : currentPercentage;

			// If the user wants to constrain the fill to an amount of ticks, and the min and max constraints are not equal, then set the target fill according to the users options.
			if( fillConstraint && fillConstraintTicks > 0 && fillConstraintMax != fillConstraintMin )
				targetFill = fillConstraintMin + ( fillConstraintRounding * Mathf.RoundToInt( ( ( fillConstraintMax - fillConstraintMin ) * currentPercentage ) / fillConstraintRounding ) );

			// Store the values so that other functions used can reference the maxValue.
			this.maxValue = maxValue;

			// If the user is not using the Smooth Fill option, or this function is being called inside Edit Mode...
			if( !smoothFill || !Application.isPlaying )
			{
				// Then just apply the target fill amount.
				statusImage.fillAmount = targetFill;

				// Call the functions for the options.
				UpdateStatusOptions();
			}
			// Else the user is using Smooth Fill while in Play Mode.
			else
			{
				// Configure the fill direction.
				fillDirection = ( int )Mathf.Sign( targetFill - statusImage.fillAmount );
				
				// If the smoothing coroutine is not currently running, the start it.
				if( !isSmoothing )
					statusBar.StartCoroutine( SmoothFill() );
			}
		}
		
		/// <summary>
		/// Updates the status options.
		/// </summary>
		void UpdateStatusOptions ()
		{
			// If the user is using some form of fill constraint...
			if( fillConstraint )
			{
				// If the last calculated percentage is the same as this calculation for fill constraint, then just return since nothing needs to be updated.
				if( CalculatedPercentage == ( statusImage.fillAmount - fillConstraintMin ) / ( fillConstraintMax - fillConstraintMin ) && Application.isPlaying )
					return;

				// Calculate the current percentage based off of the users fill constraint options.
				CalculatedPercentage = ( statusImage.fillAmount - fillConstraintMin ) / ( fillConstraintMax - fillConstraintMin );
			}
			// Else the user is not wanting any fill constraints...
			else
			{
				// If the last calculated percentage is the same as the current fill amount, then return.
				if( CalculatedPercentage == statusImage.fillAmount && Application.isPlaying )
					return;

				// Set the calculated percentage to the fill amount of the image.
				CalculatedPercentage = statusImage.fillAmount;
			}
			
			// If the color mode is set to Gradient, then apply the current gradient color.
			if( colorMode == ColorMode.Gradient )
				statusImage.color = statusGradient.Evaluate( CalculatedPercentage );

			// If the user doesn't want the text disabled, and the text component is assigned...
			if( displayText != DisplayText.Disabled && statusText != null )
			{
				// If the user wants to display the percentage, do that here.
				if( displayText == DisplayText.Percentage )
					statusText.text = additionalText + ( CalculatedPercentage * 100 ).ToString( "F0" ) + "%";
				// Else if the user wants to display the current value, do that.
				else if( displayText == DisplayText.CurrentValue )
					statusText.text = additionalText + ( CalculatedPercentage * maxValue ).ToString( "F0" );
				// Else the user wants to display the current and max values, so display those.
				else if( displayText == DisplayText.CurrentAndMaxValues )
					statusText.text = additionalText + ( CalculatedPercentage * maxValue ).ToString( "F0" ) + " / " + maxValue.ToString();
			}

			// If the application is not playing, then no visibility options should be applied.
			if( !Application.isPlaying )
				return;

			// If the user wants a dramatic fill, and the image is assigned...
			if( useDramaticFill && dramaticImage != null )
			{
				// If the user wants to display the dramatic fill on decrease...
				if( dramaticStyle == DramaticStyle.Decrease )
				{
					// If the official status bar's fill amount is higher than this status...
					if( statusImage.fillAmount > dramaticImage.fillAmount )
					{
						// Apply the same fill amount so that it will keep up visually, then return.
						dramaticImage.fillAmount = statusImage.fillAmount;

						// If the dramatic image is filling, then stop it.
						if( isDramaticFilling )
							isDramaticFilling = false;
					}
					// Else the fill amount of the status is lower...
					else
					{
						// If the status is not currently updating, then start the update.
						if( !isDramaticFilling )
							statusBar.StartCoroutine( DramaticFill() );
						// Else if the status is currently updating and the difference between fills is less than the reset sensitivity, then reset the wait seconds.
						else if( isDramaticFilling && ( dramaticImage.fillAmount - previousFillAmt ) < resetSensitivity )
							_secondsDelay = secondsDelay;

						// Store the previous fill amount.
						previousFillAmt = statusImage.fillAmount;
					}
				}
				// Else if the user wants to display the dramatic image on increase...
				else if( dramaticStyle == DramaticStyle.Increase )
				{
					// If the status fill amount is less than the dramatic image, then just apply the fill to keep up.
					if( statusImage.fillAmount < dramaticImage.fillAmount && statusImage.fillAmount > targetFill )
						dramaticImage.fillAmount = statusImage.fillAmount;
					// Else if the dramatic fill is not the same as the target fill, then apply the target.
					else if( dramaticImage.fillAmount != targetFill )
						dramaticImage.fillAmount = targetFill;
				}
			}

			// If the user does not want this Ultimate Status to keep the Ultimate Status Bar visible...
			if( !keepVisible )
			{
				// Then just inform the Ultimate Status Bar that the status has simply been updated.
				statusBar.UpdateStatusBar();
			}
			// Else if the current fraction is less than the trigger, and forceVisible is false...
			else if( CalculatedPercentage <= triggerValue && !ForceVisible )
			{
				// Set forceVisible to true so the Ultimate Status Bar knows and then inform the Ultimate Status Bar that a change to the visibility may be needed.
				ForceVisible = true;
				statusBar.UpdateStatusBarVisibility();
			}
			// Else if the current fraction is greater than the trigger and forceVisible is currently true...
			else if( CalculatedPercentage > triggerValue && ForceVisible )
			{
				// Set forceVisible to false so the Ultimate Status Bar knows and then inform the Ultimate Status Bar that a change may be needed.
				ForceVisible = false;
				statusBar.UpdateStatusBarVisibility();
			}
			else
			{
				// If nothing else has triggered, then simply update the status bar.
				statusBar.UpdateStatusBar();
			}
			
			// Inform any subscribers that the status has been updated.
			if( OnStatusUpdated != null )
				OnStatusUpdated( CalculatedPercentage );
		}

		/// <summary>
		/// Coroutine to update the fill of the status image over time.
		/// </summary>
		IEnumerator SmoothFill ()
		{
			// Set isSmoothing to true for outside calculations.
			isSmoothing = true;
			
			// While the fill amount is still heading towards the target fill...
			while( fillDirection < 0 ? statusImage.fillAmount > targetFill : statusImage.fillAmount < targetFill )
			{
				// Increase the fill amount by the speed multiplied by the fill direction.
				statusImage.fillAmount += ( smoothFillSpeed * fillDirection ) * Time.deltaTime;

				// If the fill amount is more in the direction than the target, then apply the target.
				if( fillDirection < 0 && statusImage.fillAmount < targetFill )
					statusImage.fillAmount = targetFill;

				// If the fill amount is more in the direction than the target, then apply the target.
				if( fillDirection > 0 && statusImage.fillAmount > targetFill )
					statusImage.fillAmount = targetFill;

				// Call the UpdateStatusOptions each frame that this status is updated to update options.
				UpdateStatusOptions();
				
				yield return null;
			}

			// Finalize the fill amount at the end of this coroutine.
			statusImage.fillAmount = targetFill;

			// Update the options since the final amount has been applied.
			UpdateStatusOptions();
			
			// Set isSmoothing to false for outside calculations.
			isSmoothing = false;
		}

		/// <summary>
		/// Coroutine to update the dramatic fill image over time.
		/// </summary>
		IEnumerator DramaticFill ()
		{
			// Set isUpdating to true so that other functions can check if this is running.
			isDramaticFilling = true;

			// Apply the wait time.
			_secondsDelay = secondsDelay;

			// This loop will continue while the local fill amount is greater than the target fill amount.
			while( dramaticImage.fillAmount > statusImage.fillAmount && isDramaticFilling )
			{
				// If the wait seconds are greater than zero, then decrease the time.
				if( _secondsDelay > 0 )
					_secondsDelay -= Time.deltaTime;
				// Else, reduce the fill amount by the configured fill speed.
				else
					dramaticImage.fillAmount -= fillSpeed * Time.deltaTime;
				
				yield return null;
			}

			// If isUpdating is true, then this coroutine finished, so apply the final amount.
			if( isDramaticFilling )
				dramaticImage.fillAmount = statusImage.fillAmount;

			// Set isUpdating to false so that other function will know that this function is not running now.
			isDramaticFilling = false;
		}

		[Obsolete( "This is used in the DramaticStatusFill.cs from versions prior to 3.0. Please use the new Dramatic Fill option on the Ultimate Status Bar base script if possible." )]
		public float GetTargetFill
		{
			get
			{
				return targetFill;
			}
		}
	}
	public List<UltimateStatus> UltimateStatusList = new List<UltimateStatus>();
	Dictionary<string, UltimateStatus> UltimateStatusDict = new Dictionary<string, UltimateStatus>();

	// SCRIPT REFERENCE //
	static Dictionary<string, UltimateStatusBar> UltimateStatusBars = new Dictionary<string, UltimateStatusBar>();
	public string statusBarName = string.Empty;

	// CALLBACKS //
	public event Action OnUpdatePositioning;
	public event Action OnEnableStatusBar;
	public event Action OnDisableStatusBar;


	void Awake () // this was Awake() but needed to change coz the "Awake" function wan't being called or sumtin.....
	{
		print("I'm wide awake");
		
		// If the application is not playing, then return.
		if( !Application.isPlaying )
			return;
		
		// Update the ParentCanvas component.
		UpdateParentCanvas();

		// If the parent canvas is still null after trying to update it, then warn the user and return to avoid errors.
		if( ParentCanvas == null )
		{
			Debug.LogError( "Ultimate Status Bar\nThis component is not within a Canvas object. Disabling this component to avoid any errors." );
			enabled = false;
			return;
		}

		// Set IsEnabled to true by default.
		IsEnabled = true;

		// Store this object's RectTrans so that it can be positioned.
		BaseTransform = GetComponent<RectTransform>();

		// If the BaseTransform is still null, then inform the user, disable the component and return.
		if( BaseTransform == null )
		{
			Debug.LogError( "Ultimate Status Bar\nNo RectTransform could be found on this object. Please make sure that the Ultimate Status Bar component is attached to a UI Game Object." );
			enabled = false;
			return;
		}

		// If the statusBarName is assigned...
		if( statusBarName != string.Empty )
		{
			// Check to see if the UltimateStatusBars dictionary already contains this name, and if so, remove the current one.
			if( UltimateStatusBars.ContainsKey( statusBarName ) )
				UltimateStatusBars.Remove( statusBarName );

			// Register this UltimateStatusBar into the dictionary.
			UltimateStatusBars.Add( statusBarName, GetComponent<UltimateStatusBar>() );
		}

		// This loops through each of the Ultimate Status that is in this component.
		for( int i = 0; i < UltimateStatusList.Count; i++ )
		{
			// Assign the statusBar variable to this component.
			UltimateStatusList[ i ].statusBar = this;

			// If the user has a dramatic image to use, then set the fill amount as the same as the status image fill amount.
			if( UltimateStatusList[ i ].useDramaticFill && UltimateStatusList[ i ].dramaticImage != null && UltimateStatusList[ i ].statusImage != null )
				UltimateStatusList[ i ].dramaticImage.fillAmount = UltimateStatusList[ i ].statusImage.fillAmount;

			// If the statusName variable is assigned, then register it to the local dictionary.
			if ( UltimateStatusList[ i ].statusName != string.Empty )
				UltimateStatusDict.Add( UltimateStatusList[ i ].statusName, UltimateStatusList[ i ] );
		}

		// Get the CanvasGroup component
		statusBarGroup = GetComponent<CanvasGroup>();

		// If the Canvas Group is null, then add the component and assign the variable again.
		if( statusBarGroup == null )
			statusBarGroup = gameObject.AddComponent<CanvasGroup>();

		canvasGroupInteractable = statusBarGroup.interactable;
		canvasGroupBlockRaycasts = statusBarGroup.blocksRaycasts;
		canvasGroupIgnoreParentGroup = statusBarGroup.ignoreParentGroups;

		// If the user is wanting to update the visibility using animations...
		if( updateVisibility == UpdateVisibility.Animation )
		{
			// Show the status bar.
			EnableStatusBar();

			// If the user wants to update the visibility when the status has been updated, and the initial state is true, then start the countdown.
			if( onStatusUpdated && initialState == InitialState.Enabled )
				StartCoroutine( "EnableStatusBarCountdown" );
		}
		// Else the user is wanting to update the visibility of the Ultimate Status Bar using either instant or fade...
		else
		{
			// If the user does want to fade the status bar...
			if( updateVisibility == UpdateVisibility.Fade )
			{
				// Configure the different fade speeds if they are greater than zero.
				if( enableDuration > 0 )
					enabledSpeed = 1.0f / enableDuration;

				if( disableDuration > 0 )
					disabledSpeed = 1.0f / disableDuration;
			}

			// Set the current state to the initial state value.
			IsEnabled = initialState == InitialState.Enabled;

			// If the current state is false, then set the canvas group to the disabled alpha.
			if( !IsEnabled )
				statusBarGroup.alpha = disabledAlpha;
			// Else apply the enabled alpha.
			else
				statusBarGroup.alpha = enabledAlpha;
			
			// If the user wants to update the visibility when the status has been updated, and the initial state is true, then start the countdown.
			if( onStatusUpdated && initialState == InitialState.Enabled )
				StartCoroutine( "EnableStatusBarCountdown" );
		}
	}

	void Start ()
	{
		// If the game isn't running, then this is still within the editor, so return.
		if( !Application.isPlaying )
			return;
		
		// If the user wants to use the positioning of this script...
		if( positioning == Positioning.Enabled )
		{
			// If the parent canvas does not have an Updater component, then add one so that this will update when the screen size changes.
			if( !ParentCanvas.GetComponent<UltimateStatusBarScreenSizeUpdater>() )
				ParentCanvas.gameObject.AddComponent( typeof( UltimateStatusBarScreenSizeUpdater ) );
			
			// Call UpdatePositioning() to apply the users positioning options on Start().
			UpdatePositioning();
		}
	}

	void OnDestroy ()
	{
		// If the application is not playing, then just return.
		if( !Application.isPlaying )
			return;

		// Remove this status bar from the dictionary.
		UltimateStatusBars.Remove( statusBarName );
	}

	#if UNITY_EDITOR
	void Update ()
	{
		if( Application.isPlaying )
			return;

		if( positioning == Positioning.Enabled )
			UpdatePositioning();
	}
	#endif

	/// <summary>
	/// This function is called by Unity when the parent of this transform changes.
	/// </summary>
	void OnTransformParentChanged ()
	{
		UpdateParentCanvas();
	}

	/// <summary>
	/// Updates the parent canvas if it has changed.
	/// </summary>
	public void UpdateParentCanvas ()
	{
		// Store the parent of this object.
		Transform parent = transform.parent;

		// If the parent is null, then just return.
		if( parent == null )
			return;

		// While the parent is assigned...
		while( parent != null )
		{
			// If the parent object has a Canvas component, then assign the ParentCanvas and transform.
			if( parent.transform.GetComponent<Canvas>() )
			{
				ParentCanvas = parent.transform.GetComponent<Canvas>();
				canvasRectTrans = ParentCanvas.GetComponent<RectTransform>();
				return;
			}

			// If the parent does not have a canvas, then store it's parent to loop again.
			parent = parent.transform.parent;
		}
	}

	/// <summary>
	/// This function is called from the UltimateStatus class. This function will be called each time that a status is updated. Therefore, even if the certain status will not keep the status bar visible, it can still update the visibility when it has been modified.
	/// </summary>
	void UpdateStatusBar ()
	{
		// If the user is not using the OnStatusUpdated option, or the Ultimate Status Bar is already forcing visible, then return.
		if( !onStatusUpdated || forceVisible )
			return;
		
		// If the countdown is already running, then reset the countdown time.
		if( isCountingDown )
			countdownTime = idleSeconds;
		// Else start the countdown.
		else
			StartCoroutine( "EnableStatusBarCountdown" );

		// If the status bar is currently fading out...
		if( isFading && !IsEnabled )
		{
			// Then set isFading to false, and stop the coroutine.
			isFading = false;
			StopCoroutine( "FadeOutHandler" );
		}

		// Show the status bar.
		EnableStatusBar();
	}

	/// <summary>
	/// This function is called from the UltimateStatus class. This function is only called when the ultimate status has triggered it's keep visible value.
	/// </summary>
	void UpdateStatusBarVisibility ()
	{
		// If the user doesn't have the timeout option enabled, then return.
		if( !onStatusUpdated )
			return;

		// Loop through each Ultimate Status...
		for( int i = 0; i < UltimateStatusList.Count; i++ )
		{
			// If the current status is currently wanting to force the visibility...
			if( UltimateStatusList[ i ].ForceVisible )
			{
				// Stop counting down.
				isCountingDown = false;
				StopCoroutine( "EnableStatusBarCountdown" );

				// Show the status bar.
				EnableStatusBar();

				// Set forceVisible to true and return.
				forceVisible = true;
				return;
			}
		}

		// Set force visible to false.
		forceVisible = false;

		// If there is already a countdown coroutine running, then stop it.
		if( isCountingDown )
			StopCoroutine( "EnableStatusBarCountdown" );

		// Start the countdown and show the status bar.
		StartCoroutine( "EnableStatusBarCountdown" );
		EnableStatusBar();
	}

	/// <summary>
	/// This function handles the fading in of the Ultimate Status Bar.
	/// </summary>
	IEnumerator FadeInHandler ()
	{
		// Set isFading to true so that other functions will know that this coroutine is running.
		isFading = true;

		// Store the current value of the Canvas Group's alpha.
		float currentAlpha = statusBarGroup.alpha;

		// Loop for the duration of the enabled duration variable.
		for( float t = 0.0f; t < 1.0f && isFading && IsEnabled; t += Time.deltaTime * enabledSpeed )
		{
			// If the speed is NaN, then break the loop.
			if( float.IsInfinity( enabledSpeed ) )
				break;

			// Apply the alpha to the CanvasGroup.
			statusBarGroup.alpha = Mathf.Lerp( currentAlpha, enabledAlpha, t );
			yield return null;
		}

		// If the coroutine was not interrupted, then apply the final value.
		if( isFading && IsEnabled )
			statusBarGroup.alpha = enabledAlpha;

		// Set isFading to false so that other functions know that this coroutine is not running anymore.
		isFading = false;
	}
	
	/// <summary>
	/// For details on this coroutine, see the FadeInHandler() function above.
	/// </summary>
	IEnumerator FadeOutHandler ()
	{
		// Set isFading to true so other methods can know this coroutine is running.
		isFading = true;

		// Store the current alpha.
		float currentAlpha = statusBarGroup.alpha;

		// Loop for as long as the user wants.
		for( float t = 0.0f; t < 1.0f && isFading && !IsEnabled; t += Time.deltaTime * disabledSpeed )
		{
			// If the disabled speed is infinite, then break the loop.
			if( float.IsInfinity( disabledSpeed ) )
				break;

			// Lerp the alpha of the canvas group.
			statusBarGroup.alpha = Mathf.Lerp( currentAlpha, disabledAlpha, t );

			// Yield 1 frame.
			yield return null;
		}

		// If this has not been interrupted, then apply the final alpha.
		if( isFading && !IsEnabled )
			statusBarGroup.alpha = disabledAlpha;

		// Set isFading to false so that outside methods know.
		isFading = false;
	}

	/// <summary>
	/// This function is used as a local controlled Update function for count down the time that this status bar has been idle.
	/// </summary>
	IEnumerator EnableStatusBarCountdown ()
	{
		// Set isCountingDown to true for checks.
		isCountingDown = true;

		// Set the starting time.
		countdownTime = idleSeconds;

		// If the current state is false, then add the duration of the enable to make idle time correct.
		if( !IsEnabled && updateVisibility == UpdateVisibility.Fade )
			countdownTime += enableDuration;

		// While the countdownTime is greater than zero, continue counting down.
		while( countdownTime > 0 )
		{
			countdownTime -= Time.deltaTime;
			yield return null;
		}

		// Once the countdown is complete, set isCountingDown to false and hide the status bar.
		isCountingDown = false;
		DisableStatusBar();
	}

	/// <summary>
	/// This coroutine makes the status bar follow the screen position of the world object's transform.
	/// </summary>
	IEnumerator FollowTransform ( Transform trans, Camera camera, Vector3 positionModifier, LayerMask blockingLayerMask )
	{
		// Set the controller to true for reference.
		IsFollowingTransform = true;

		// Store the size of the canvas for calculations.
		Vector2 canvasSize = canvasRectTrans.sizeDelta * ParentCanvas.GetComponent<RectTransform>().localScale;
		
		// Configure the bounds of the rect.
		Rect inBounds = new Rect( -BaseTransform.sizeDelta / 2, new Vector2( canvasSize.x + BaseTransform.sizeDelta.x, canvasSize.y + BaseTransform.sizeDelta.y ) );

		// This variable is necessary to store the visible state of the transform while not overwriting the onStatusUpdated option if the user has that selected.
		bool isVisible = false;

		// Loop for as long as IsFollowingTransform is true.
		while( IsFollowingTransform )
		{
			// If the gameobject has been destroyed then break the loop.
			if( trans == null || trans.gameObject == null )
				break;

			// If the canvas size has changed...
			if( canvasSize != canvasRectTrans.sizeDelta * ParentCanvas.GetComponent<RectTransform>().localScale )
			{
				// Store the new canvas size.
				canvasSize = canvasRectTrans.sizeDelta * ParentCanvas.GetComponent<RectTransform>().localScale;

				// Recalculate the new bounds of the camera.
				inBounds = new Rect( -BaseTransform.sizeDelta / 2, new Vector2( canvasSize.x + BaseTransform.sizeDelta.x, canvasSize.y + BaseTransform.sizeDelta.y ) );
			}

			// Find the screen position of the world object.
			Vector3 screenPoint = camera.WorldToScreenPoint( trans.position + positionModifier );

			// Configure the relative position of the transform from the camera. This is important for checking if the transform is in front of or behind.
			Vector3 relativePosition = camera.transform.InverseTransformPoint( trans.position );
			
			// If the screen position of the transform is not within the bounds, or if the relative position of the transform is behind the camera...
			if( !inBounds.Contains( screenPoint ) || relativePosition.z <= 0 || !InLineOfSight( camera.transform.position, trans, blockingLayerMask ) )
			{
				// Set isVisible to false for calculations.
				isVisible = false;
				
				// If the status bar is enabled...
				if( IsEnabled )
				{
					// Set IsEnabled to false.
					IsEnabled = false;

					// Zero the alpha of the status bar group and make the status bar not interactable.
					statusBarGroup.alpha = 0.0f;
					UpdateCanvasGroupProperties();
				}
			}
			// Else if the bounds does contain the screen position of the transform, and the relative position is positive...
			else if( inBounds.Contains( screenPoint ) && relativePosition.z > 0 && InLineOfSight( camera.transform.position, trans, blockingLayerMask ) )
			{
				// If the status bar is currently disabled, and the last known state of visibility was false...
				if( !IsEnabled && !isVisible )
				{
					// If the user wants the visibility to be updated when a status is updated...
					if( onStatusUpdated )
					{
						// Store a temporary bool for setting visible.
						bool setVisible = false;

						// If the initial state option is set to Enabled, then set the visibility to true.
						if( initialState == InitialState.Enabled )
							setVisible = true;
						// Else the initial state is Disabled...
						else
						{
							// So loop through each status.
							for( int i = 0; i < UltimateStatusList.Count; i++ )
							{
								// If this status is wanting to force the status bar as visible...
								if( UltimateStatusList[ i ].ForceVisible )
								{
									// Then setVisible true so that the status bar will be enabled.
									setVisible = true;

									// Break this loop.
									break;
								}
							}
						}

						// If the status bar should be visible...
						if( setVisible )
						{
							// Set is enabled to true.
							IsEnabled = true;

							// Set the alpha of the group to the needed value depending on user settings.
							statusBarGroup.alpha = updateVisibility != UpdateVisibility.Animation ? enabledAlpha : 1.0f;

							// Set interactable to true since the status bar is enabled again.
							UpdateCanvasGroupProperties();

							// Update the status bar visibility through code for countdowns.
							UpdateStatusBarVisibility();
						}
					}
					else
					{
						// Set is enabled to true.
						IsEnabled = true;

						// Set the alpha of the group to the needed value depending on user settings.
						statusBarGroup.alpha = updateVisibility != UpdateVisibility.Animation ? enabledAlpha : 1.0f;

						// Set interactable to true since the status bar is enabled again.
						UpdateCanvasGroupProperties();
					}
				}

				// Set isVisible to true for calculations.
				isVisible = true;
			}

			// If the status bar is enabled and the base transform is assigned, then set the screen position.
			if( isVisible && BaseTransform != null )
				BaseTransform.position = screenPoint;
			
			yield return null;
		}

		// Set the controller to false for other methods to reference.
		IsFollowingTransform = false;
	}

	/// <summary>
	/// Returns the results of the target transform being in line of sight of the camera.
	/// </summary>
	/// <param name="origin">The origin point of the camera.</param>
	/// <param name="target">The transform component of the target transform.</param>
	bool InLineOfSight ( Vector3 origin, Transform target, LayerMask blockingLayerMask )
	{
		// Temp RaycastHit variable.
		RaycastHit hit;

		// If there is some sort of collider between the origin and the target transform, and the hit transform is not a relative of the target, then return false.
		if( Physics.Raycast( origin, ( target.position - origin ), out hit, Mathf.Infinity, blockingLayerMask ) && !IsTransformRelative( hit.transform, target ) )
			return false;
		
		// Otherwise there was no collider in the way, so return true.
		return true;
	}

	/// <summary>
	/// Returns the results of the hit transform being related to the target transform in hierarchy.
	/// </summary>
	/// <param name="hitTransform">The transform component of the hit object.</param>
	/// <param name="targetTransform">The transform component of the target transform.</param>
	bool IsTransformRelative ( Transform hitTransform, Transform targetTransform )
	{
		if( hitTransform == targetTransform )
			return true;

		// Loop while the hit transform parent is not null.
		while( hitTransform.parent != null )
		{
			// If the hit transform is the same as the target transform, then return true.
			if( hitTransform == targetTransform )
				return true;

			// Else assign the hit transforms parent.
			hitTransform = hitTransform.parent;
		}
		
		// Otherwise the transform is not related, so return false.
		return false;
	}

	/// <summary>
	/// This function acts as an Update function to follow the rotation of the targeted camera.
	/// </summary>
	IEnumerator LookAtCamera ( Camera camera )
	{
		// Set the controller to true so other functions know this coroutine is running.
		IsFollowingCamera = true;
		
		// This loop will continue until specified to stop.
		while( true )
		{
			// If the camera transform is null, then notify the user and break the loop.
			if( camera == null )
			{
				Debug.LogError( "Ultimate Status Bar\nThe Camera provided for the FollowCamera function is null." );
				break;
			}

			// Make the canvas look at the camera.
			ParentCanvas.transform.LookAt( ParentCanvas.transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up );

			yield return null;
		}

		// Set the controller to false so other functions know this coroutine is finished.
		IsFollowingCamera = false;
	}

	/// <summary>
	/// This function will modify the Canvas Group so that it effectively "disappears" when the status bar is disabled visually, and will return to the default user defined settings when enabled.
	/// </summary>
	void UpdateCanvasGroupProperties ()
	{
		// If the status bar is enabled the restore the users settings on the canvas group.
		if( IsEnabled )
		{
			statusBarGroup.interactable = canvasGroupInteractable;
			statusBarGroup.blocksRaycasts = canvasGroupBlockRaycasts;
			statusBarGroup.ignoreParentGroups = canvasGroupIgnoreParentGroup;
		}
		// Else disable all interaction and blocking of any raycast.
		else
		{
			statusBarGroup.interactable = false;
			statusBarGroup.blocksRaycasts = false;
			statusBarGroup.ignoreParentGroups = true;
		}
	}

	// --------------------------------------------- <<< PUBLIC FUNCTIONS FOR THE USER >>> --------------------------------------------- //
	/// <summary>
	/// Returns the Ultimate Status registered with the targeted status name.
	/// </summary>
	/// <param name="statusName">The name of the targeted Ultimate Status.</param>
	public UltimateStatus GetUltimateStatus ( string statusName )
	{
		// If an Ultimate Status has not been registered with the statusName parameter, then return null.
		if( !UltimateStatusRegistered( statusName ) )
			return null;

		// Return the Ultimate Status that has been registered with the statusName parameter.
		return UltimateStatusDict[ statusName ];
	}

	/// <summary>
	/// Updates the size and positioning of the Ultimate Status Bar on the screen.
	/// </summary>
	public void UpdatePositioning ()
	{
		// If the user is not using the Positioning, then return.
		if( positioning != Positioning.Enabled )
			return;

		// If the parent canvas is null, then try to get the parent canvas component.
		if( ParentCanvas == null )
			UpdateParentCanvas();

		// If it is still null, then log a error and return.
		if( ParentCanvas == null )
		{
			Debug.LogError( "Ultimate Status Bar\nThere is no parent canvas object. Please make sure that the Ultimate Status Bar is placed within a canvas." );
			return;
		}

		// Store this object's RectTrans so that it can be positioned.
		BaseTransform = GetComponent<RectTransform>();

		// Set the current reference size for scaling.
		float referenceSize = scalingAxis == ScalingAxis.Height ? canvasRectTrans.sizeDelta.y : canvasRectTrans.sizeDelta.x;

		// Configure the target size for the graphic.
		float textureSize = referenceSize * ( statusBarSize / 10 );
		
		// Apply the scale of 1 for calculations.
		BaseTransform.localScale = Vector3.one;

		// If the user wants to preserve the aspect ratio of their image...
		if( imageAspectRatio == ImageAspectRatio.Preserve )
		{
			// If the targetImage variable has been left unassigned...
			if( targetImage == null )
			{
				// If there is an image component on this object and the sprite is assigned, then assign the image.
				if( GetComponent<Image>() && GetComponent<Image>().sprite != null )
					targetImage = GetComponent<Image>();
				// Else there is no image (or sprite on the image), so if the application is playing, inform the user.
				else if( Application.isPlaying )
					Debug.LogError( "Ultimate Status Bar\nThe Target Image component has not been assigned." );
			}

			// If the target image does have a sprite, then assign the ratio information.
			if( targetImage != null && targetImage.sprite != null )
			{
				// Configure the x and y aspect ratio determined by the sprite of the image.
				xRatio = targetImage.sprite.rect.width / Mathf.Max( targetImage.sprite.rect.width, targetImage.sprite.rect.height );
				yRatio = targetImage.sprite.rect.height / Mathf.Max( targetImage.sprite.rect.width, targetImage.sprite.rect.height );
			}
		}

		// Apply the texture size to the baseTransform.
		BaseTransform.sizeDelta = new Vector2( textureSize * xRatio, textureSize * yRatio );

		// Configure the images pivot space so that the image will be in the correct position regardless of the pivot set by the user.
		Vector3 pivotSpacer = new Vector3( BaseTransform.sizeDelta.x * BaseTransform.pivot.x, BaseTransform.sizeDelta.y * BaseTransform.pivot.y, 0 ) - ( Vector3 )( BaseTransform.sizeDelta / 2 );
		
		// First, fix the positioning to be a value between -0.5f and 0.5f.
		Vector2 fixedPositioning = new Vector2( horizontalPosition - 50, verticalPosition - 50 ) / 100;

		// Then configure position spacers according to the screen's dimensions, the fixed spacing and texture size.
		float xPosition = canvasRectTrans.sizeDelta.x * fixedPositioning.x - ( BaseTransform.sizeDelta.x * fixedPositioning.x );
		float yPosition = canvasRectTrans.sizeDelta.y * fixedPositioning.y - ( BaseTransform.sizeDelta.y * fixedPositioning.y );
		
		// Apply the positioning to the baseTransform.
		BaseTransform.localPosition = new Vector3( xPosition, yPosition ) + pivotSpacer;

		// If the parent canvas is not rendering in screen space, then modify the position by the depth set by the user.
		if( ParentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
			BaseTransform.localPosition += new Vector3( 0, 0, referenceSize * ( depthPosition / 10 ) );
		// Else set the local rotation to zero.
		else
			BaseTransform.localRotation = Quaternion.identity;
		
		// Notify any subscribers that the positioning has been updated.
		if( OnUpdatePositioning != null )
			OnUpdatePositioning();
	}
	
	/// <summary>
	/// Updates the default status on the local Ultimate Status Bar.
	/// </summary>
	/// <param name="currentValue">The current value of the status.</param>
	/// <param name="maxValue">The max value of the status.</param>
	public void UpdateStatus ( float currentValue, float maxValue )
	{
		// Call the UpdateStatus function on the default UltimateStatus.
		UltimateStatusList[ 0 ].UpdateStatus( currentValue, maxValue );
	}

	/// <summary>
	/// Updates the Ultimate Status that is registered with the targeted status name.
	/// </summary>
	/// <param name="statusName">The name of the targeted Ultimate Status.</param>
	/// <param name="currentValue">The current value of the status.</param>
	/// <param name="maxValue">The max value of the status.</param>
	public void UpdateStatus ( float currentValue, float maxValue, string statusName )
	{
		// If an Ultimate Status has not been registered with the statusName parameter, then return.
		if( !UltimateStatusRegistered( statusName ) )
			return;

		// Update the Ultimate Status that has been registered with the statusName.
		UltimateStatusDict[ statusName ].UpdateStatus( currentValue, maxValue );
	}

	/// <summary>
	/// Enables the status bar.
	/// </summary>
	public void EnableStatusBar ()
	{
		// If the current state is already true, or the user does not want to update the visibility ever, then return.
		if( IsEnabled )
			return;

		// Set the current state to true.
		IsEnabled = true;

		// If the user is wanting to update the visibility using a Canvas Group fade...
		if( updateVisibility == UpdateVisibility.Fade )
		{
			// If there is no CanvasGroup, then return.
			if( statusBarGroup == null )
				return;

			// If the status bar is currently fading, then stop the FadeOutHandler.
			if( isFading )
				StopCoroutine( "FadeOutHandler" );

			// If the enable duration is zero, then the user wants the alpha to apply instantly so do that.
			if( enableDuration <= 0 )
				statusBarGroup.alpha = enabledAlpha;
			// Else start the Fade In routine.
			else
				StartCoroutine( "FadeInHandler" );
		}
		// Else if the user wants to use an animation, then update the animator.
		else if( updateVisibility == UpdateVisibility.Animation )
		{
			if( statusBarAnimator != null )
				statusBarAnimator.SetBool( "BarActive", true );
		}
		// Else apply the alpha instantly.
		else
		{
			// If there is no CanvasGroup, then return.
			if( statusBarGroup == null )
				return;

			statusBarGroup.alpha = enabledAlpha;
		}

		// Configure the canvas group for being enabled.
		UpdateCanvasGroupProperties();

		// Inform any subscribers that the status bar has been enabled.
		if( OnEnableStatusBar != null )
			OnEnableStatusBar();
	}

	/// <summary>
	/// Hides the status bar.
	/// </summary>
	public void DisableStatusBar ()
	{
		// If the current state is already false, or the user does not want to update the visibility ever, then return.
		if( !IsEnabled )
			return;

		// Set the current state to false.
		IsEnabled = false;

		// If the user is wanting to update the visibility using a Canvas Group fade...
		if( updateVisibility == UpdateVisibility.Fade )
		{
			// If the statusBarGroup isn't assigned, return.
			if( statusBarGroup == null )
				return;

			// If the status bar is currently fading, then stop the coroutine.
			if( isFading )
				StopCoroutine( "FadeInHandler" );

			if( disableDuration <= 0 )
				statusBarGroup.alpha = disabledAlpha;
			// Else start the Fade Out routine.
			else
				StartCoroutine( "FadeOutHandler" );
		}
		// Else, update the animator.
		else if( updateVisibility == UpdateVisibility.Animation )
		{
			if( statusBarAnimator != null )
				statusBarAnimator.SetBool( "BarActive", false );
		}
		// Else apply the alpha instantly.
		else
		{
			// If there is no CanvasGroup, then return.
			if( statusBarGroup == null )
				return;

			statusBarGroup.alpha = disabledAlpha;
		}

		// Configure the canvas group properties for being disabled now.
		UpdateCanvasGroupProperties();

		// Inform any subscribers that the status bar has been disabled.
		if( OnDisableStatusBar != null )
			OnDisableStatusBar();
	}

	/// <summary>
	/// Attempts to start the coroutine to follow a transform around in the scene with the information provided.
	/// </summary>
	/// <param name="transformToFollow">The transform component to follow the position of.</param>
	/// <param name="camera">The camera to calculate the screen position off of.</param>
	/// <param name="positionModifier">This determines the position that the status bar should follow in relation to the transforms actual position.</param>
	/// <param name="blockingLayerMask">The layer mask that is to be used to determine if the object is in line of sight of the camera or not.</param>
	public void StartFollowTransform ( Transform transformToFollow, Camera camera, Vector3 positionModifier, LayerMask blockingLayerMask )
	{
		// If this gameobject is inactive in the scene, then inform the user and return to avoid errors.
		if( !gameObject.activeInHierarchy )
		{
			Debug.LogError( "Ultimate Status Bar\nThe Ultimate Status Bar gameobject is disabled. Cannot start following a world transform." );
			return;
		}

		// If the canvas is not set to the right settings, then inform the user and return to avoid errors.
		if( ParentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
		{
			Debug.LogError( "Ultimate Status Bar\nThe parent canvas of this Ultimate Status Bar is not set to Screen Space Overlay. The canvas must be set to this in order to follow a world transforms position." );
			return;
		}
		
		// If this status bar is already following a transform, then stop the coroutine.
		if( IsFollowingTransform )
			StopCoroutine( "FollowTransform" );

		// Start the coroutine to follow the target transform.
		StartCoroutine( FollowTransform( transformToFollow, camera, positionModifier, blockingLayerMask ) );
	}

	/// <summary>
	/// Stops following the world transform if the status bar is currently following one.
	/// </summary>
	public void StopFollowTransform ()
	{
		// If this status bar is not following an object, then just return.
		if( !IsFollowingTransform )
			return;

		// Stop the coroutine and set isFollowingTransform to false.
		StopCoroutine( "FollowTransform" );
		IsFollowingTransform = false;

		// If the status bar is currently enabled...
		if( IsEnabled )
		{
			// Set IsEnabled to false for calculations.
			IsEnabled = false;

			// Set the alpha to zero and configure the properties of the canvas group.
			statusBarGroup.alpha = 0.0f;
			UpdateCanvasGroupProperties();
		}
	}

	/// <summary>
	/// Makes the Ultimate Status Bar look at the targeted camera. Only useful for World Space canvases.
	/// </summary>
	/// <param name="camera">The camera for this Ultimate Status Bar to look at.</param>
	public void StartLookAtCamera ( Camera camera )
	{
		// If the parent canvas is not world space, inform the user and return.
		if( ParentCanvas.renderMode != RenderMode.WorldSpace )
		{
			Debug.LogError( "Ultimate Status Bar\nThis Ultimate Status Bar is not being used in world space, and therefore cannot look at the camera." );
			return;
		}

		// If this status bar is already following a transform, then stop the coroutine.
		if( IsFollowingCamera )
			StopCoroutine( "LookAtCamera" );

		// Start the coroutine to follow the target transform.
		StartCoroutine( LookAtCamera( camera ) );
	}

	/// <summary>
	/// Stops the Ultimate Status Bar from looking at the camera.
	/// </summary>
	public void StopLookAtCamera ()
	{
		// If this status bar is not following a camera, then just return.
		if( !IsFollowingCamera )
			return;

		// Stop the coroutine and set isFollowingCamera to false.
		StopCoroutine( "FollowCamera" );
		IsFollowingCamera = false;
	}

	/// <summary>
	/// Returns the result of the Ultimate Status being registered to the local dictionary.
	/// </summary>
	bool UltimateStatusRegistered ( string statusName )
	{
		// If the Ultimate Status Dictionary contains the statusName, return true.
		if( UltimateStatusDict.ContainsKey( statusName ) )
			return true;

		// Debug a warning to the user so they know that the status they are trying to reference has not been registered, and then return false.
		Debug.LogWarning( "Ultimate Status Bar\nNo status has been registered with the name: " + statusName + ". Please make sure that the reference is correct and that this status has actually been created on the targeted Ultimate Status Bar." );
		return false;
	}
	// ------------------------------------------- <<< END PUBLIC FUNCTIONS FOR THE USER >>> ------------------------------------------- //

	// --------------------------------------------- <<< STATIC FUNCTIONS FOR THE USER >>> --------------------------------------------- //
	/// <summary>
	/// Returns the Ultimate Status Bar registered with the status name.
	/// </summary>
	/// <param name="statusBarName">The name of the targeted Ultimate Status Bar.</param>
	public static UltimateStatusBar GetUltimateStatusBar ( string statusBarName )
	{
		// If an Ultimate Status Bar has not been registered with the statusBarName parameter, then return null.
		if( !UltimateStatusBarRegistered( statusBarName ) )
			return null;

		// Return the Ultimate Status Bar that has been registered with the statusBarName parameter.
		return UltimateStatusBars[ statusBarName ];
	}

	/// <summary>
	/// Updates the positioning of the targeted Ultimate Status Bar on the screen.
	/// </summary>
	/// <param name="statusBarName">The string name that has been registered in the Script Reference section of the targeted Ultimate Status Bar.</param>
	public static void UpdatePositioning ( string statusBarName )
	{
		// If there is no status bar registered with the targeted name, then just return.
		if( !UltimateStatusBarRegistered( statusBarName ) )
			return;

		// Call the function on the targeted Ultimate Status Bar.
		UltimateStatusBars[ statusBarName ].UpdatePositioning();
	}

	/// <summary>
	/// Updates the default status on the targeted Ultimate Status Bar.
	/// </summary>
	/// <param name="statusBarName">The name of the targeted Ultimate Status Bar.</param>
	/// <param name="currentValue">The current value of the status.</param>
	/// <param name="maxValue">The maximum value of the status.</param>
	public static void UpdateStatus ( string statusBarName, float currentValue, float maxValue )
	{
		// If there is no status bar registered with the targeted name, then just return.
		if( !UltimateStatusBarRegistered( statusBarName ) )
			return;

		// Update the default Ultimate Status with the provided values.
		UltimateStatusBars[ statusBarName ].UpdateStatus( currentValue, maxValue );
	}

	/// <summary>
	/// Updates the targeted Ultimate Status registered on the targeted Ultimate Status Bar, with the current and max values.
	/// </summary>
	/// <param name="statusBarName">The name of the targeted Ultimate Status Bar.</param>
	/// <param name="currentValue">The current value of the status.</param>
	/// <param name="maxValue">The maximum value of the status.</param>
	/// <param name="statusName">The name of the targeted status.</param>
	public static void UpdateStatus ( string statusBarName, float currentValue, float maxValue, string statusName )
	{
		// If there is no status bar registered with the targeted name, then just return.
		if( !UltimateStatusBarRegistered( statusBarName ) )
			return;

		// If there is no status registered with the targeted name, then just return.
		if( !UltimateStatusBars[ statusBarName ].UltimateStatusRegistered( statusName ) )
			return;

		// Update the Ultimate Status with the current and max values.
		UltimateStatusBars[ statusBarName ].UltimateStatusDict[ statusName ].UpdateStatus( currentValue, maxValue );
	}

	/// <summary>
	/// Enables the targeted Ultimate Status Bar if it exists in the scene.
	/// </summary>
	/// <param name="statusBarName">The string name that has been registered in the Script Reference section of the targeted Ultimate Status Bar.</param>
	public static void EnableStatusBar ( string statusBarName )
	{
		// If there is no status bar registered with the targeted name, then just return.
		if( !UltimateStatusBarRegistered( statusBarName ) )
			return;

		// Call the function on the targeted Ultimate Status Bar.
		UltimateStatusBars[ statusBarName ].EnableStatusBar();
	}

	/// <summary>
	/// Disables the targeted Ultimate Status Bar if it exists in the current scene.
	/// </summary>
	/// <param name="statusBarName">The string name that has been registered in the Script Reference section of the targeted Ultimate Status Bar.</param>
	public static void DisableStatusBar ( string statusBarName )
	{
		// If there is no status bar registered with the targeted name, then just return.
		if( !UltimateStatusBarRegistered( statusBarName ) )
			return;

		// Call the function on the targeted Ultimate Status Bar.
		UltimateStatusBars[ statusBarName ].DisableStatusBar();
	}
	
	/// <summary>
	/// Returns the results of the Ultimate Status Bar being registered to the dictionary.
	/// </summary>
	static bool UltimateStatusBarRegistered ( string statusBarName )
	{
		// If the Ultimate Status Bar Dictionary contains the statusName, return true.
		if( UltimateStatusBars.ContainsKey( statusBarName ) )
			return true;

		// Debug a warning to the user so they know that the status bar they are trying to reference has not been registered, and then return false.
		Debug.LogWarning( "Ultimate Status Bar\nNo Ultimate Status Bar has been registered with the name: " + statusBarName + "." );
		return false;
	}
	// ------------------------------------------- <<< END STATIC FUNCTIONS FOR THE USER >>> ------------------------------------------- //
	
	#region DEPRECIATED
	[Obsolete( "Please use EnableStatusBar instead." )]
	public void ShowStatusBar ()
	{
		EnableStatusBar();
	}

	[Obsolete( "Please use DisableStatusBar instead." )]
	public void HideStatusBar ()
	{
		DisableStatusBar();
	}

	[Obsolete( "Please use the public accessor 'icon' instead." )]
	public void UpdateStatusBarIcon ( Sprite newIcon )
	{
		icon = newIcon;
	}

	[Obsolete( "Please use the public accessor 'text' instead." )]
	public void UpdateStatusBarText ( string newText )
	{
		text = newText;
	}

	[Obsolete( "Please use the new current UpdateStatus functions." )]
	public static void UpdateStatus ( string statusBarName, string statusName, float currentValue, float maxValue )
	{
		UpdateStatus( statusBarName, currentValue, maxValue, statusName );
	}

	[Obsolete( "Please use the public accessor 'icon' instead." )]
	public static void UpdateStatusBarIcon ( string statusBarName, Sprite newIcon )
	{
		if( !UltimateStatusBarRegistered( statusBarName ) )
			return;
		
		UltimateStatusBars[ statusBarName ].icon = newIcon;
	}

	[Obsolete( "Please use the public accessor 'text' instead." )]
	static public void UpdateStatusBarText ( string statusBarName, string newText )
	{
		if( !UltimateStatusBarRegistered( statusBarName ) )
			return;

		UltimateStatusBars[ statusBarName ].text = newText;
	}
	#endregion
}
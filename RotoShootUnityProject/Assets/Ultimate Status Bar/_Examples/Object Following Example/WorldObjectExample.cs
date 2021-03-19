/* Written by Kaz Crowe */
/* WorldObjectExample.cs */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WorldObjectExample : MonoBehaviour
{
	public Canvas canvas;

	public float rotationSpeed = 10;
	float yRotation = 0;
	float xRotationDefault = 0;
	public Text fpsText;
	float deltaTime = 0.0f;
	public int cubesToSpawn = 10;
	public GameObject blockingBox;

	[Header( "Screen Space Options" )]
	public Transform cubeTransform;
	public UltimateStatusBar statusBar;
	public LayerMask blockingLayerMask;

	[Header( "World Space Options" )]
	public bool useWorldSpace = false;
	public Transform cubeTransformWithCanvas;
	

	private void Start ()
	{
		xRotationDefault = transform.rotation.eulerAngles.x;

		int heightMod = 1;

		if( !useWorldSpace )
		{
			statusBar.gameObject.SetActive( true );
			cubeTransform.gameObject.SetActive( true );
		}
		else
			cubeTransformWithCanvas.gameObject.SetActive( true );

		for( int i = 0; i < cubesToSpawn; i++ )
		{
			Vector3 randomPosition = new Vector3( Random.Range( -50, 50 ), Random.Range( -5, 5 ), Random.Range( -50, 50 ) );
			if( !useWorldSpace )
			{
				GameObject cubeObj = Instantiate( cubeTransform.gameObject, randomPosition, Quaternion.identity, cubeTransform.parent );
				GameObject statusObj = Instantiate( statusBar.gameObject, canvas.transform );
				statusObj.GetComponent<UltimateStatusBar>().StartFollowTransform( cubeObj.transform, GetComponent<Camera>(), new Vector3( 0, heightMod, 0 ), blockingLayerMask );
			}
			else
			{
				GameObject cubeObj = Instantiate( cubeTransformWithCanvas.gameObject, randomPosition, Quaternion.identity, cubeTransform.parent );
				cubeObj.GetComponentInChildren<UltimateStatusBar>().StartLookAtCamera( GetComponent<Camera>() );
			}
		}

		if( !useWorldSpace )
			statusBar.StartFollowTransform( cubeTransform, GetComponent<Camera>(), new Vector3( 0, heightMod, 0 ), blockingLayerMask );
		else
			cubeTransformWithCanvas.GetComponentInChildren<UltimateStatusBar>().StartLookAtCamera( GetComponent<Camera>() );
	}

	private void Update ()
	{
		yRotation += rotationSpeed * Time.deltaTime;

		transform.rotation = Quaternion.Euler( xRotationDefault, yRotation, 0 );
		
		deltaTime += ( Time.unscaledDeltaTime - deltaTime ) * 0.1f;

		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		fpsText.text = string.Format( "{0:0.0} ms ({1:0.} fps)", msec, fps );
	}

	public void ToggleBlockingBox ()
	{
		blockingBox.SetActive( !blockingBox.activeInHierarchy );
	}
}
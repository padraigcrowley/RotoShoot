using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Adapted from:
// http://2sa-studio.blogspot.com/2015/01/simulating-touch-events-from-mouse.html
//
// See bottom of this file for the original code from above
//

/*
[System.Serializable] //https://docs.unity3d.com/ScriptReference/Events.UnityEvent_1.html
public class MyFloatEvent : UnityEvent<float>
{
}*/

public class InputManager : MonoBehaviour
{
  private Vector3 startTouchPos, endTouchPos;
  private float minSwipeDistanceThreshold = 0.75f; //1.0f;


  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      if (Helpers.IsOverUi())// todo - if this works here, remove it from all the other places below
      {
        //Debug.Log("TOUCHED UI OBJECT, RETURNING");
        return;
      }
      DetectTouch(); //detect and handle the screen touch/mouse clicks
      DetectKeys();
    }
  }

  private void DetectTouch()
  {
    // Handle native touch events
    foreach (Touch touch in Input.touches)
    {
      //check touch isn't tapping on a UI gameobject
      if (Input.touchCount > 0 && touch.phase == TouchPhase.Began)
      {
        if (Helpers.IsOverUi())
        {
          //Debug.Log("TOUCHED UI OBJECT, RETURNING");
          return;
        }
      }
      HandleTouch(touch.fingerId, Camera.main.ScreenToWorldPoint(touch.position), touch.phase);
    }

    // Simulate touch events from mouse events
    if (Input.touchCount == 0) // so you don't detect both touches AND mouse-clicks?
    {
      if (Input.GetMouseButtonDown(0))
      {
        if (Helpers.IsOverUi())
        {
          //Debug.Log(" 111 TOUCHED UI OBJECT, RETURNING");
          return;
        }
        else
          HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Began);
      }
      if (Input.GetMouseButton(0))
      {
        if (Helpers.IsOverUi())//check touch isn't tapping on a UI gameobject
        {
          //Debug.Log("222 TOUCHED UI OBJECT, RETURNING");
          return;
        }
        else
          HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Moved);
      }
      if (Input.GetMouseButtonUp(0))
      {
        if (Helpers.IsOverUi())//check touch isn't tapping on a UI gameobject
        {
          //Debug.Log("333 TOUCHED UI OBJECT, RETURNING");
          return;
        }
        else
          HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Ended);
      }
    }
  }

  private void DetectKeys()
  {
    if (Input.GetKeyDown("left") || Input.GetKeyDown(KeyCode.A))
    {
      GameplayManager.Instance.mouseClickQueue.Enqueue(GameplayManager.Instance.angleToRotatePlayerShip);
    }
    if (Input.GetKeyDown("right") || Input.GetKeyDown(KeyCode.D))
    {
      GameplayManager.Instance.mouseClickQueue.Enqueue(-GameplayManager.Instance.angleToRotatePlayerShip);
    }
    if ((Input.GetKeyDown("up")) || (Input.GetKeyDown("down")) || (Input.GetKeyDown("space")))
    {
      GameplayManager.Instance.mouseClickQueue.Enqueue(180f);
    }
  }

  private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
  {
    int swipeDirection = 0;

    switch (touchPhase)
    {
      case TouchPhase.Began:
        // TODO
        startTouchPos = touchPosition;
        //Debug.Log("startTouchPos: " + startTouchPos);
        //print("Objects LocalRot: " + objectToRotate.transform.localRotation + "Objects Rotation: " + objectToRotate.transform.rotation);
        //Debug.Log("Q: " + mouseClickQueue.ToString());
        break;
      case TouchPhase.Moved:
        // TODO
        break;
      case TouchPhase.Ended:
        endTouchPos = touchPosition;
        //Debug.Log("endTouchPos" + endTouchPos);

        if ((swipeDirection = DetectSwipe(startTouchPos, endTouchPos)) != 0) //swipe
        {
          //Debug.Log("Swiped!!");
          if (swipeDirection == 1)
          {
            GameplayManager.Instance.mouseClickQueue.Enqueue(-180f);
            /*StartCoroutine(rotateObject(playerShipObj, new Vector3(0, 0, 180), rotationDuration * 2)); // have to multiply rotationDuration by something as it's very fast to do a full 180 degrees otherwise. technically it should be multiplied by 4 (4 separate rotations to reach 180) but that feels too slow
            PlayerShipGreenRotArrowObj.transform.Rotate(Vector3.forward * 180);
            PlayerShipRedRotArrowObj.transform.Rotate(Vector3.forward * 180);*/
          }
          else
          {
            GameplayManager.Instance.mouseClickQueue.Enqueue(180f);
            //StartCoroutine(rotateObject(playerShipObj, new Vector3(0, 0, -180), rotationDuration * 2)); // have to multiply rotationDuration by something as it's very fast to do a full 180 degrees otherwise. technically it should be multiplied by 4 (4 separate rotations to reach 180) but that feels too slow
            //PlayerShipGreenRotArrowObj.transform.Rotate(Vector3.forward * -180);
            //PlayerShipRedRotArrowObj.transform.Rotate(Vector3.forward * -180);
          }
        }
        else // a tap, not a swipe
        {
          if (startTouchPos.x < 0)
          {
            GameplayManager.Instance.mouseClickQueue.Enqueue(180f);
          }
          else
          {
            GameplayManager.Instance.mouseClickQueue.Enqueue(-180f);
          }
        }
        break;
    }
  }

  // Given a start touch pos and an end touch pos, determine if this was a valid swipe.
  // since i'm only interested in a Left or Right swipe for now, return -1 for a left swipe, 1 for a right swipe, 0 for not a valid swipe
  private int DetectSwipe(Vector3 startTouchPos, Vector3 endTouchPos)
  {

    //print("System.Math.Abs(endTouchPos.x - startTouchPos.x)" + System.Math.Abs(endTouchPos.x - startTouchPos.x));
    //print("System.Math.Abs(endTouchPos.y - startTouchPos.y)" + System.Math.Abs(endTouchPos.y - startTouchPos.y));

    if ((System.Math.Abs(endTouchPos.x - startTouchPos.x) > minSwipeDistanceThreshold) && (endTouchPos.x >= startTouchPos.x))
    {
      return 1;
    }
    else if ((System.Math.Abs(endTouchPos.x - startTouchPos.x) > minSwipeDistanceThreshold) && (endTouchPos.x < startTouchPos.x))
    {
      return -1;
    }
    else if ((System.Math.Abs(endTouchPos.y - startTouchPos.y) > minSwipeDistanceThreshold) && (endTouchPos.y >= startTouchPos.y))
    {
      return 1;
    }
    else if ((System.Math.Abs(endTouchPos.y - startTouchPos.y) > minSwipeDistanceThreshold) && (endTouchPos.y < startTouchPos.y))
    {
      return -1;
    }
    else
      return 0;
  }


}

/*
When building a game targetting touch devices, your scripts obviously handle touch events.

The problem is that, when running the game in Unity editor, only mouse events are triggered, and I could not find any option that would allow to simulate touch events from the mouse interactions performed in the Game view.

So I finally came up with this simple pattern that somehow simulates touch events from the mouse events (language is C#), to have one single processing piece of code focused on touch events only:


void Update () {
  // Handle native touch events
  foreach (Touch touch in Input.touches)
  {
    HandleTouch(touch.fingerId, Camera.main.ScreenToWorldPoint(touch.position), touch.phase);
  }

  // Simulate touch events from mouse events
  if (Input.touchCount == 0)
  {
    if (Input.GetMouseButtonDown(0))
    {
      HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Began);
    }
    if (Input.GetMouseButton(0))
    {
      HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Moved);
    }
    if (Input.GetMouseButtonUp(0))
    {
      HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Ended);
    }
  }
}

private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
{
  switch (touchPhase)
  {
    case TouchPhase.Began:
      // TODO
      break;
    case TouchPhase.Moved:
      // TODO
      break;
    case TouchPhase.Ended:
      // TODO
      break;
  }
}
*/
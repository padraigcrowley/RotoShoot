/*
 * Copyright (c) 2015 Razeware LLC 
 */

using UnityEngine;
using System.Collections;

// This is a handy little script I adapted from one I found ages ago on Stack Overflow. 
// By assigning it to four "Boundary" GameObjects with BoxCollider2D components attached, you can create trigger areas to deal with objects that try to, or move off screen.
// Just assign each of your four barriers a Left, Top, Right or Bottom direction from the Inspector. 

public class Boundary : MonoBehaviour
{

  public enum BoundaryLocation
  {
    LEFT, TOP, RIGHT, BOTTOM
  };
  public BoundaryLocation direction;
  private BoxCollider2D barrier;
  public float boundaryWidth = 0.4f;
  public float overhang = 1.0f; // We add this to the length of the boundaries to ensure there are no gaps at the corners of the screen
                                // If we lose any object pooled bullets they will never be returned to the pool. 
  void Start()
  {

    // Get the the world coordinates of the corners of the camera viewport.

    Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, 0));
    Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0));
    Vector3 lowerLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
    Vector3 lowerRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, 0));

    // Get this game objects BoxCollider2D

    barrier = GetComponent<BoxCollider2D>();

    // Depending on the assigned 'direction' of the Boundary we adjust the size and position based on the camera viewport as obtained above

    if (direction == BoundaryLocation.TOP)
    {
      barrier.size = new Vector2(Mathf.Abs(topLeft.x) + Mathf.Abs(topRight.x) + overhang, boundaryWidth);
      barrier.offset = new Vector2(0, boundaryWidth / 2);
      transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight, 1));
    }
    if (direction == BoundaryLocation.BOTTOM)
    {
      barrier.size = new Vector2(Mathf.Abs(topLeft.x) + Mathf.Abs(topRight.x) + overhang, boundaryWidth);
      barrier.offset = new Vector2(0, -boundaryWidth / 2);
      transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, 0, 1));
    }
    if (direction == BoundaryLocation.LEFT)
    {
      barrier.size = new Vector2(boundaryWidth, Mathf.Abs(lowerLeft.y) + Mathf.Abs(lowerRight.y) + overhang);
      barrier.offset = new Vector2(-boundaryWidth / 2, 0);
      transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight / 2, 1));
    }
    if (direction == BoundaryLocation.RIGHT)
    {
      barrier.size = new Vector2(boundaryWidth, Mathf.Abs(lowerLeft.y) + Mathf.Abs(lowerRight.y) + overhang);
      barrier.offset = new Vector2(boundaryWidth / 2, 0);
      transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight / 2, 1));
    }
  }
}

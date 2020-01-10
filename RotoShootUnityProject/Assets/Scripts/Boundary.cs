using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script defines the size of the ‘Boundary’ depending on Viewport. When objects go beyond the ‘Boundary’, they are destroyed or deactivated.
/// </summary>
public class Boundary : MonoBehaviour
{

  BoxCollider2D boundareCollider;

  //receiving collider's component and changing boundary borders
  private void Start()
  {
    boundareCollider = GetComponent<BoxCollider2D>();
    ResizeCollider();
  }

  //changing the collider's size up to Viewport's size multiply 1.5
  void ResizeCollider()
  {
    Vector2 viewportSize = Camera.main.ViewportToWorldPoint(new Vector2(1, 1)) * 2;
    print($"Screen Boundary X: {viewportSize.x} Screen Boundary Y: {viewportSize.y}");

    GameplayManager.Instance.screenEdgeX = viewportSize.x;
    GameplayManager.Instance.screenEdgeY = viewportSize.y;
    viewportSize.x *= 1.5f;
    viewportSize.y *= 1.5f;
    GameplayManager.Instance.screenCollisionBoundaryX = viewportSize.x;
    GameplayManager.Instance.screenCollisionBoundaryY = viewportSize.y;
    boundareCollider.size = viewportSize;
  }

  //when another object leaves collider
  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.tag == "PlayerMissile")
    {
      Destroy(collision.gameObject);
    }
    else if (collision.tag == "Bonus")
      Destroy(collision.gameObject);
  }

}

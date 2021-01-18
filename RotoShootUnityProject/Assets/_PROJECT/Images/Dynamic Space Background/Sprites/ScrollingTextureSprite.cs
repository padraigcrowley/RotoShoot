using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTextureSprite : MonoBehaviour
{

  //public float  scrollSpeed = 0.065f;
  public float scrollSpeed = 0.01f;
  public float scrollSpeedMultiplier = 8.59f;

  Material _material;
  void Start()
  {
    _material = GetComponent<SpriteRenderer>().material;
    //_material = GetComponent<MeshRenderer>().material;
  }

  void Update()
  {
    /*currentscroll = speed * Time.deltaTime;
    Debug.Log("CurrentScroll" + currentscroll);
    _material.mainTextureOffset = new Vector2(0,-currentscroll);*/

  }

  void FixedUpdate()
  {

    //float offset = Time.time * scrollSpeed;
    //_material.mainTextureOffset = new Vector2(0, offset);
    //less jerky than the above?  https://www.reddit.com/r/Unity3D/comments/9tfpli/odd_jitter_behavior_when_changing_speed_of/
    _material.mainTextureOffset += new Vector2(0, 1) * scrollSpeed * Time.deltaTime;

  }


}

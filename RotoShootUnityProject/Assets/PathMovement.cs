using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mr1
{
  public class PathMovement : MonoBehaviour
  {
    // Start is called before the first frame update
    void Start()
    {
      transform.FollowPath("Path0001", 5f, FollowType.Loop).Log(true);

    }

    // Update is called once per frame
    void Update()
    {

    }
  }
}

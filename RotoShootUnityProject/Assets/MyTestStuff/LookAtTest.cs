using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTest : MonoBehaviour
{
  public GameObject enemy01;
  GameObject go;

  void Start()
  {
    go = Instantiate(enemy01, new Vector3(5, 5), Quaternion.identity) as GameObject;

    //enemy01.transform.up = transform.position - enemy01.transform.position;

    /*Vector3 dir = transform.position - go.transform.position;
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
    go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);*/
  }

  // Update is called once per frame
  void Update()
  {
    Vector3 dir = transform.position - go.transform.position;
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
    go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
  }
}

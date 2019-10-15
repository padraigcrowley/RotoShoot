/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchBarBehaviour : MonoBehaviour
{
  private bool paused = false;
  private bool touchActive = false;
  

  
  public GameObject playerShipObj;
  

  
  public float rotationDuration;
  

  

  

  [SerializeField]
  

  

  

  // Use this for initialization
  void Start()
  {
    //InvokeRepeating("FireBullet", 2.0f, .3f);
    //StartCoroutine("FireBullet");
    
    

    PlayerShipRedRotArrowObj.SetActive(false);
  }

  void Update()
  {

    //if (!paused)
      DetectTouch(); //detect and handle the screen touch/mouse clicks

    if (Input.GetKeyDown("left"))
    {
      mouseClickQueue.Enqueue(-1);
      PlayerShipGreenRotArrowObj.transform.Rotate(Vector3.forward * angleToRotate);
      PlayerShipRedRotArrowObj.transform.Rotate(Vector3.forward * angleToRotate);

    }
    if (Input.GetKeyDown("right"))
    {
      mouseClickQueue.Enqueue(1);
      PlayerShipGreenRotArrowObj.transform.Rotate(Vector3.forward * -angleToRotate);
      PlayerShipRedRotArrowObj.transform.Rotate(Vector3.forward * -angleToRotate);
    }

    
    
         
    

  }


  

  /*IEnumerator FireBullet()
  {
    while(true)
    {
      GameObject firedBullet = Instantiate(bullet, barrelTip.position, barrelTip.rotation);
      firedBullet.GetComponent<Rigidbody2D>().velocity = barrelTip.up * 10f;
      yield return new WaitForSeconds(.5f);
    }
  }*/


  

  /*
  IEnumerator rotateObject(GameObject gameObjectToMove, Vector3 eulerAngles, float duration)  //https://stackoverflow.com/questions/37586407/rotate-gameobject-over-time/37588536

  {
    if (MyGameplayManager.Instance.playerShipRotating)
    {
      yield break;
    }
    MyGameplayManager.Instance.playerShipRotating = true;
    PlayerShipRedRotArrowObj.SetActive(true);

    Vector3 newRot = gameObjectToMove.transform.eulerAngles + eulerAngles;

    Vector3 currentRot = gameObjectToMove.transform.eulerAngles;

    float counter = 0;
    while (counter < duration)
    {
      counter += Time.deltaTime;
      gameObjectToMove.transform.eulerAngles = Vector3.Lerp(currentRot, newRot, counter / duration);
      yield return null;
    }
    MyGameplayManager.Instance.playerShipRotating = false;
    PlayerShipRedRotArrowObj.SetActive(false);
  }


 

}*/
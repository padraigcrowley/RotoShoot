using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningMineBehaviour : MonoBehaviour
{

  enum EnemyState { WAITING_TO_SPAWN, SPINNING_IN_STARTED, SPINNING_IN_IN_PROGRESS, SPINNING_IN_COMPLETED, FIRING, NOT_FIRING, SPINNING_OUT_IN_PROGRESS, SPINNING_OUT_COMPLETED }
  EnemyState enemyState;
  private Renderer spriteMaterial, spriteRenderer;
  private float minX = -3.22f, maxX = 3.2f, minY = -2f, maxY = 8f; //(topleft: -3.22, 8.0) (bottomright: 3.2, -2.0)
  private float xDelta = 2f, yDelta = 2f;

  private void Awake()
  {
    spriteMaterial = GetComponent<Renderer>();
    spriteRenderer = GetComponent<SpriteRenderer>();
  }


  void Start()
  {
    spriteRenderer.material.color = new Color(1, 1, 1, 0);
    spriteMaterial.material.SetFloat("_TwistUvAmount", 1f);
    spriteMaterial.material.SetFloat("_BlurIntensity", 100f);
    enemyState = EnemyState.WAITING_TO_SPAWN;


  }
  IEnumerator AlphaFadeTo(float aValue, float aTime)
  {
    float alpha = spriteRenderer.material.color.a;
    for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
    {
      Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
      spriteRenderer.material.color = newColor;
      yield return null;
    }
  }
  IEnumerator SpinInEffect(float duration)
  {
    float elapsedTime = 0f;
    float currentTwistVal, currentBlurVal;
    while (elapsedTime <= duration)
    {
      currentTwistVal = Mathf.Lerp(1f, 3.14f, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_TwistUvAmount", currentTwistVal);
      currentBlurVal = Mathf.Lerp(100f, 0f, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_BlurIntensity", currentBlurVal);
      elapsedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }
    spriteMaterial.material.SetFloat("_TwistUvAmount", 3.14f);
    spriteMaterial.material.SetFloat("_BlurIntensity", 0f);
    enemyState = EnemyState.SPINNING_IN_COMPLETED;
  }



  // Update is called once per frame
  void Update()
  {
    switch(enemyState)
    {
      case EnemyState.WAITING_TO_SPAWN:
        if (Input.GetKeyDown(KeyCode.S))
        {
          transform.position = new Vector2((UnityEngine.Random.Range(-3.2f, 3.2f)), (UnityEngine.Random.Range(2f, 8f)));
          enemyState = EnemyState.SPINNING_IN_STARTED;
        }
        break;
      case EnemyState.SPINNING_IN_STARTED:
        StartCoroutine(AlphaFadeTo(1.0f, .5f));
        StartCoroutine(SpinInEffect(2f));
        enemyState = EnemyState.SPINNING_IN_IN_PROGRESS;
        break;
      case EnemyState.SPINNING_IN_IN_PROGRESS:
        break;
      case EnemyState.SPINNING_IN_COMPLETED:
        transform.Rotate(new Vector3(0, 0, 100 * Time.deltaTime));

        if ((transform.position.x >= maxX) || (transform.position.x <= minX))
          xDelta = -xDelta;
        if ((transform.position.y >= maxY) || (transform.position.y <= minY))
          yDelta = -yDelta;
        transform.position = new Vector2(transform.position.x + (xDelta * Time.deltaTime), transform.position.y + (yDelta * Time.deltaTime));
        break;
      
      default:
        break;
    }
  }
}


/* Define a set of screen valid to appear within (topleft: -3.22, 8.0) (bottomright: 3.2, -2.0)
 * set the 5 shooting points
 * set the collision on the min body of the mine
 * set collision on the 5 shooting ponits
 * choose a vfx type of missile it shoots
 * add a worldspace healthbar to it
 * add successful hitFX
 * 
 * set it up to be triggered by the levelmanager
 * 
 * while (alive)
 * {
 *  "randomly" appear within a set of screen bounds
 *   use shader to 'spin' shader into life
 *    when spin shader ends, start rotating and firing from the 5 shooting points - do this for x seconds
 *    use shader to 'spin' shader out of life
 *  }
 *        
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldBehaviour : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if(GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS)
		{
      Destroy(gameObject);
		}
  }

  private void OnTriggerEnter(Collider collision)
  {
    if ((collision.gameObject.tag.Equals("EnemyMissile")) || (collision.gameObject.tag.Equals("Enemy01")) || (collision.gameObject.tag.Equals("Asteroid")))
    {

    }

  }
}

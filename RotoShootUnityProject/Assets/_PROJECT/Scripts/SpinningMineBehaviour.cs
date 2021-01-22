using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningMineBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2((UnityEngine.Random.Range(-3.2f,3.2f)), (UnityEngine.Random.Range(2f, 8f)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


/* Define a set of screen valid to appear within (topleft: -3.22, 8.0) (bottomright: 3.2, 2.0)
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
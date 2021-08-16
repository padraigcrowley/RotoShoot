using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpriteTweenBehaviour : MonoBehaviour
{

  private Tween pulseTween;
  void Start()
  {
    pulseTween = transform.DOScale(1.5f, 0.3f).SetLoops(-1, LoopType.Yoyo);

  }

  void OnEnable()
  {
		bool isActive = pulseTween.IsActive();
    if (isActive)
    {

      bool isPlaying = pulseTween.IsPlaying();
      if (!isPlaying)
      {
        pulseTween.Play();
        //pulseTween = transform.DOScale(1.5f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        print("Tween wasn't active, kicking it off again.");
      }
      else
      {
        print("Tween was active, no need to kick off again...");
      }
    }
	}

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.X))
    {
      print("X pressed");
      bool isPlaying = pulseTween.IsPlaying();
      if (isPlaying)
      {
        //print("caling KILL!");
        pulseTween.Pause();
        SimplePool.Despawn(gameObject);
      }
    }
  }
}

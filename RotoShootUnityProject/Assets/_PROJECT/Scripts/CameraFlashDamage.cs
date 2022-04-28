///
/// https://answers.unity.com/questions/1286867/screen-flashes-red-on-damage.html
///

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFlashDamage : MonoBehaviour
{

  public KeyCode activationKey;
  public Color targetColor;
  public float duration;
  private GameObject flashView;
  private GameObject panel;

  void Start()
  {

    // Create Flash Panel
    flashView = new GameObject();
    flashView.name = "FlashCanvas";
    flashView.AddComponent<Canvas>();
    Canvas myCanvas = flashView.GetComponent<Canvas>();
    myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
    flashView.AddComponent<CanvasScaler>();
    flashView.AddComponent<GraphicRaycaster>();
    flashView.transform.SetParent(transform);
    panel = new GameObject("Panel");
    panel.AddComponent<CanvasRenderer>();
    panel.AddComponent<Image>();
    RectTransform rt = panel.GetComponent<RectTransform>();
    rt.sizeDelta = new Vector2(2000, 3000);
    panel.transform.SetParent(myCanvas.transform, false);
    flashView.SetActive(false);
  }

  IEnumerator PlayAnimation()
  {
    Debug.Log("Show Flash Damage Animation");
    
    if (!flashView.activeSelf)
    {
      flashView.SetActive(true);
    }
    Image img = flashView.GetComponentInChildren<Image>();
    print($"img attribs {img.color}");
    Tween myTween = img.DOColor(new Color(1, 1, 1, 0), .75f).OnComplete(() => flashView.SetActive(false));
       
    yield return myTween.WaitForCompletion();
    img.color = new Color(1, 1, 1, 1);
    
  }

  public void doFlashAnim()
  {
    StartCoroutine("PlayAnimation");
  }
  
  //void Update()
  //{

  //  if (Input.GetKeyUp(activationKey))
  //  {
  //    StartCoroutine("PlayAnimation");
  //  }
  //}
}
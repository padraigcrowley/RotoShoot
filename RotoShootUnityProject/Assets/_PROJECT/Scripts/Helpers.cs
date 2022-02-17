using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// from TARODEV https://youtu.be/JOABOQMurZo?t=128
/// </summary>

public static class Helpers
{


  private static PointerEventData _eventDataCurrentPosition;
  private static List <RaycastResult> _results;

  public static bool IsOverUi()
  {
    _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
    _results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
    return _results.Count > 0;
  }



}

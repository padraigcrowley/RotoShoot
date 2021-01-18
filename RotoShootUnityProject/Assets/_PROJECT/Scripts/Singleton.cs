/*
 Find attached the singleton class I use for Kawaii slide - so completely unrelated to EPS and so safe to use as you will.

Take a class e.g MyGameManager (for scores etc) and change to monbehaviour to this....

public class MyGameManager : Singleton<MyGameManager>

You can then create a gameobject in your main scene and lob MyGameManager component onto it.

Then from within you game you can access any data/functions on this using this sort of thing....

MyGameManager.Instance.ResetLevelPosition();
MyGameManager.Instance.IncrementScore(int _incrementScore);

Don't be tempted to have just one and fill it full of all the stuff you can think of - try and keep them neat and self contained.
So you could separate GameManger and GamePlayManager for example.
*/



using UnityEngine;
using System.Diagnostics;

[DebuggerStepThrough]
public class Singleton<T> : ExtendedBehaviour where T : ExtendedBehaviour
{
  private static T _instance = null;
  private static object _lock = new object();
  //private static bool hasInitialised = false;

  public static T Instance
  {
    get
    {
      lock (_lock)
      {
#if UNITY_EDITOR2
if (FindObjectsOfType(typeof(T)).Length > 1)
Debug.LogError("More than one instance of singleton " + typeof(T).ToString());
#endif
        //if (!hasInitialised)
        {
          if (_instance == null)
            _instance = FindObjectOfType(typeof(T)) as T;

          //hasInitialised = (_instance != null);
        }

        return _instance;
      }
    }
  }

  public static bool ValidInstance()
  {
    return (_instance != null) ? true : false;
  }

  public static void InstanceReset()
  {
    //hasInitialised = false;

  }
}

/*
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
private static T _instance = null;
private static bool _destroyed = false;
private static object _lock = new object();

public static T Instance
{
    get
    {
        lock (_lock)
        {
            if (_destroyed)
                return null;

            if (_instance == null)
            {
                GameObject singleton = new GameObject();

                if (singleton)
                {
                    singleton.name = "[Singleton] " + typeof(T).ToString();
                    DontDestroyOnLoad(singleton);

                    _instance = singleton.AddComponent<T>();
                }
            }

            return _instance;
        }
    }
}

protected virtual void OnDestroy()
{
    _destroyed = true;
    _instance = null;
}

public static bool ValidInstance()
{
    return (_instance != null) ? true : false;
}
}
*/

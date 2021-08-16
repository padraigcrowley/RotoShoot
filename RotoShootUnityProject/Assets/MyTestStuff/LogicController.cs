using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicModeBase
{
	public LogicModeBase (BeginDelegate begin, UpdateDelegate update, EndDelegate end, ChangeDelegate change)
    {
		AssignDelegates (begin, update, end, change, null, null);
	}

    public LogicControllerBase ownerController;
    
	public LogicModeBase() {}
	
	public delegate void BeginDelegate(int PrevMode);
	public delegate void UpdateDelegate();
	public delegate void LateUpdateDelegate();
	public delegate void EndDelegate(int NextMode);
	public delegate bool ChangeDelegate(int newMode);
	public delegate void ShutdownDelegate();
	
	private BeginDelegate 		delegateBegin  		= null;
	private UpdateDelegate		delegateUpdate 		= null;
	private EndDelegate			delegateEnd	   		= null;
	private ChangeDelegate		delegateChange		= null;
	private LateUpdateDelegate	delegateLate		= null;
	private ShutdownDelegate	delegateShutdown	= null;
	
	private int						modeID		   			= -1;
	private int						previousModeID		   	= -1;
	private int						nextModeID			   	= -1;
	private System.DateTime			modeStartTime	   		= new System.DateTime();
	
	public void AssignDelegates (BeginDelegate begin, UpdateDelegate update, EndDelegate end, ChangeDelegate change, LateUpdateDelegate late, ShutdownDelegate shut)
	{
		delegateBegin  		= begin;
		delegateUpdate 		= update;
		delegateEnd	  		= end;
		delegateChange		= change;
		delegateLate		= late;
		delegateShutdown	= shut;
	}
	
	public int ModeID
	{
		get {return modeID;}
		set {modeID = value;}
	}
	
	public int PreviousModeID
	{
		get {return previousModeID;}
	}
	
	public int NextModeID
	{
		get {return nextModeID;}
	}
	
	public float ModeTimeElapsed
	{
		get {return (float)System.DateTime.Now.Subtract (modeStartTime).TotalSeconds;}
	}

	public void CallBegin(int prevMode)
	{
		if (delegateBegin != null)
		{
			previousModeID 	= prevMode;
			modeStartTime	= System.DateTime.Now;
			
			delegateBegin(prevMode);
		}
	}
	
	public void CallEnd(int nextMode)
	{
		if (delegateEnd != null)
			delegateEnd(nextMode);
	}
	
	public void CallUpdate()
	{
		if (delegateUpdate != null)
			delegateUpdate();
	}
	
	public void CallLateUpdate()
	{
		if (delegateLate != null)
			delegateLate();
	}
	
	public void CallShutdown()
	{
		if (delegateShutdown != null)
			delegateShutdown();
	}
	
	public bool CallCanChange(int nextMode)
	{
		nextModeID = nextMode;
		
		if (delegateChange != null)
		{
			if (!delegateChange(nextMode))
			{
				return false;
			}
		}
		
		return true;
	}
	
	public virtual void OnRegister(){}
	public virtual void OnDeregister(){}
}

public class LogicMode : LogicModeBase
{
    public LogicMode()
    {
        base.AssignDelegates((prevMode) => { Begin(prevMode); },
            () => { Update(); },
            (nextMode) => { End(nextMode); },
            (nextMode) => { return Change(nextMode); },
            () => { LateUpdate(); },
            () => { Shutdown(); }
        );

    }

    public virtual void Begin(int prevMode)
    {
    }

    public virtual void Update()
    {
    }

    public virtual void End(int nextMode)
    {
    }

    public virtual void LateUpdate()
    {
    }

    public virtual bool Change(int nextMode)
    {
        return true;
    }

    public virtual void Shutdown()
    {
    }
}

public class LogicControllerBase
{
	private Dictionary<int, LogicModeBase>	logicModes		    = new Dictionary<int, LogicModeBase>();
	private int 							currentMode		    = -1;
	private int								previousMode	    = -1;
    private LogicModeBase                   currentLogicMode    = null;

	public override string ToString()
	{
		return GetType().ToString() + " : " + currentMode + " : " + (currentLogicMode == null ? "" : currentLogicMode.GetType().ToString());
	}

	public int CurrentMode
	{
		get {return currentMode;}
	}

	public int PreviousMode
	{
		get {return previousMode;}
	}
	
	public void UpdateController()
	{
        if (currentLogicMode != null)
            currentLogicMode.CallUpdate();
	}
	
	public void LateUpdateController()
	{
        if (currentLogicMode != null)
            currentLogicMode.CallLateUpdate();
	}
	
	public void Shutdown()
	{
		foreach (var mode in logicModes)
			mode.Value.CallShutdown();

        currentLogicMode = null;
	}
	
	//////////////////////////////////////////////////////////////
	// Methods for adding new logic modes using raw delegates 
	//////////////////////////////////////////////////////////////
		
	public bool RegisterLogicMode(int Mode, LogicMode.BeginDelegate Begin, LogicMode.UpdateDelegate Update, LogicMode.EndDelegate End, LogicMode.ChangeDelegate Change)
	{
		if (logicModes.ContainsKey(Mode)) 
			return false;
		
		LogicModeBase logicMode = new LogicModeBase(Begin, Update, End, Change);

		logicMode.OnRegister();
		logicMode.ModeID = Mode;
        logicMode.ownerController = this;
		logicModes.Add(Mode, logicMode);
		
		return true;		
	}

	public bool RegisterLogicMode (int modeID, LogicMode modeObject)
	{
		if (!logicModes.ContainsKey(modeID))
		{
			modeObject.OnRegister();
			modeObject.ModeID = modeID;
			logicModes.Add (modeID, modeObject);
			return true;
		}
		
		return false;
	}
	
	//////////////////////////////////////////////////////////////
		
	public int GetLogicMode()
	{
		return currentMode;
	}

	// Determines whether the current mode can be changed and if so calls the End method
	// of the current mode, followed by the begin method of the new mode
	public bool SetLogicMode(int newMode) 
	{
		if (newMode == currentMode) 
			return false;
		
		if (!logicModes.ContainsKey(newMode)) 
			return false;
		
        if (currentLogicMode != null)
        {
            if (!currentLogicMode.CallCanChange(newMode))
                return false;

            currentLogicMode.CallEnd(newMode);
        }

		previousMode = currentMode;
		currentMode  = newMode;
		
        currentLogicMode = logicModes[currentMode];
        currentLogicMode.CallBegin(previousMode);

		return true;
	}
	
	public void RestartLogicMode() 
	{
        currentLogicMode.CallBegin(currentMode);
	}

	public void ResetLogicMode() 
	{
        currentLogicMode.CallEnd(currentMode);
        currentLogicMode.CallBegin(currentMode);
	}

	public float LogicModeTime()
	{
        if (currentLogicMode != null)
            return currentLogicMode.ModeTimeElapsed;

        return 0.0f;
	}
	
	public bool LogicModeTimeElapsed(float Duration)
	{
		return (LogicModeTime() >= Duration)? true : false;
	}
}


public class LogicController : MonoBehaviour
{
    private LogicControllerBase logicController = new LogicControllerBase();

    protected virtual void Awake()
    {
    }
	
    protected virtual void Start()
    {
        
    }
	
    protected virtual void Update()
    {
        logicController.UpdateController();
    }
	
    public bool RegisterLogicMode(int Mode, LogicMode.BeginDelegate Begin, LogicMode.UpdateDelegate Update, LogicMode.EndDelegate End, LogicMode.ChangeDelegate Change)
    {
        return logicController.RegisterLogicMode(Mode, Begin, Update, End, Change);
    }

    public bool RegisterLogicMode(int modeID, LogicMode modeObject)
    {
        return logicController.RegisterLogicMode(modeID, modeObject);
    }

    public int GetLogicMode()
    {
        return logicController.GetLogicMode();
    }

    public bool SetLogicMode(int newMode) 
    {
        return logicController.SetLogicMode(newMode);
    }

    public void ResetLogicMode() 
    {
        logicController.ResetLogicMode();
    }

    public void RestartLogicMode() 
    {
        logicController.RestartLogicMode();
    }

    public float LogicModeTime()
    {
        return logicController.LogicModeTime();
    }

    public bool LogicModeTimeElapsed(float Duration)
    {
        return logicController.LogicModeTimeElapsed(Duration);
    }
	
}


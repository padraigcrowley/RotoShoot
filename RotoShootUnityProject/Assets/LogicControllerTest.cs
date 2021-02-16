/*------------------------------
Lob the LogicControllerTest onto a game object in a scene and press play.
You will see spam for what is going on with the logic modes.
It will start in an idle.
Hold ‘M’ to move – it will change the spam and value on the debug string in LogicControllerTest
Release to return to an idle.

Press ‘J’ to jump.
The update for jump has a 3 second timer after which it will land.

You will see from the setup of the logic modes that I allow….
Idle->Jump
Idle->Move
Jump->Idle
Move->Idle
Move->Jump

Each time a logic mode is changed you get the Enter for the new one and the Exit for the old one.

Hopefully you can see that this is good for managing multiple states and conditions for switches between them.

The base code for the logic controller may seem a bit weird because of the use of ‘delegates’. These are a bit like actions, events etc.
They just let you specify a function (or functions!) to execute.

https://learn.unity.com/tutorial/delegates
--------------------------------------------------------*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicControllerTest : MonoBehaviour
{
    private LogicController myController;

    public string debugString;

    private enum LogicIDs
    {
        Idle = 0,
        Move = 1,
        Jump = 2
    };

    // Start is called before the first frame update
    void Start()
    {
        myController = gameObject.AddComponent<LogicController>();

        myController.RegisterLogicMode((int)LogicIDs.Idle, Idle_Start, Idle_Update, Idle_End, null);
        myController.RegisterLogicMode((int)LogicIDs.Move, Move_Start, Move_Update, Move_End, null);
        myController.RegisterLogicMode((int)LogicIDs.Jump, Jump_Start, Jump_Update, Jump_End, null);

        myController.SetLogicMode((int)LogicIDs.Idle);
    }

    // Update is called once per frame
    void Update()
    {
    }

    bool CheckMove()
    {
        if (Input.GetKey(KeyCode.M))
        {
            myController.SetLogicMode((int)LogicIDs.Move);
            return true;
        }
        else
        {
            myController.SetLogicMode((int)LogicIDs.Idle);
            return false;
        }
    }

    bool CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            myController.SetLogicMode((int)LogicIDs.Jump);
            return true;
        }

        return false;
    }

    //******* LOGIC MODE STUFF **********
    
    //*********************************
    //IDLE
    //*********************************
    
    private void Idle_Start(int PrevMode)
    {
        Debug.Log("Begin Idle");
    }

    private void Idle_Update()
    {
        Debug.Log("Idling!");

        debugString = "Idling";

        if (CheckJump())
        {
            return;
        }

        CheckMove();

    }

    private void Idle_End(int NextMode)
    {
        Debug.Log("End Idle");
    }

    //***********************************
    //MOVE
    //***********************************
    
    private void Move_Start(int PrevMode)
    {
        Debug.Log("Go!Go!Go!");
    }

    private void Move_Update()
    {
        if (CheckJump())
        {
            myController.SetLogicMode((int)LogicIDs.Jump);
            return;
        }

        CheckMove();
        debugString = "Moving";
    }

    private void Move_End(int NextMode)
    {
        Debug.Log("STOP!");
    }

    
    //************************************
    //JUMP
    //************************************

    private float jumpDuration = 0.0f;

    private void Jump_Start(int PrevMode)
    {
        Debug.Log("BOING!");
        jumpDuration = 3.0f;
    }

    private void Jump_Update()
    {
        jumpDuration -= Time.deltaTime;

        if (jumpDuration <= 0.0f)
        {
            myController.SetLogicMode((int)LogicIDs.Idle);
        }

        debugString = "Jumping : " + jumpDuration;
    }

    private void Jump_End(int NextMode)
    {
        Debug.Log("LANDED!");
    }

}
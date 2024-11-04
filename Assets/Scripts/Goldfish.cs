using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Gold : MonoBehaviour
{
    private Rigidbody2D rb;
    private enum GoldfishState { Treasure, Grabbed }
    private GoldfishState _state = GoldfishState.Treasure;

    private GameObject grabber;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SwitchState(GoldfishState.Treasure);
    }

    private void Update()
    {
        switch(_state)
        { 
            //cas
        }
    }

    private void SwitchState(GoldfishState newState)
    {
        //Previous state
        switch (_state)
        {

        }

        //New state
        switch (newState)
        {

        }

        _state = newState;
    }

    public void Grab(GameObject grabberObject)
    {
        grabber = grabberObject;
        SwitchState(GoldfishState.Grabbed);
    }
}

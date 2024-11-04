using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Goldfish : MonoBehaviour
{
    public float grabFollowForce = 0.5f;

    private Rigidbody2D rb;
    private Floaty floaty;
    private enum GoldfishState { Treasure, Grabbed }
    private GoldfishState _state = GoldfishState.Treasure;

    private GameObject grabber;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        floaty = GetComponent<Floaty>();
        SwitchState(GoldfishState.Treasure);
    }

    void FixedUpdate()
    {
        switch(_state)
        {
            case GoldfishState.Grabbed:
                floaty.MoveTowards(grabber.transform.position, grabFollowForce);
                floaty.ApplyFriction();
                break;
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

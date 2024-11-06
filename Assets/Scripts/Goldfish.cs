using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Goldfish : MonoBehaviour
{
    public float grabFollowForce = 0.5f;

    public float speed = 0.1f;

    public int childrenMax = 5;
    public int childrenMin = 2;

    public LayerMask goldfishMask;
    public LayerMask fishFoodMask;

    private Rigidbody2D rb;
    private Floaty floaty;
    private enum GoldfishState { Treasure, Grabbed, Wandering, GoingToFood, Reproducing, RunAway}
    private GoldfishState _state = GoldfishState.Treasure;

    private GameObject grabber;

    private FishFood targetedFood;
    private Goldfish mate;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        floaty = GetComponent<Floaty>();
        SwitchState(GoldfishState.Wandering);
    }

    void FixedUpdate()
    {
        switch(_state)
        {
            case GoldfishState.Grabbed:
                floaty.SetTargetPosition(grabber.transform.position);
                floaty.MoveTowardsTargetPosition(grabFollowForce);
                floaty.ApplyFriction();
                break;

            case GoldfishState.Wandering:
                if (floaty.TargetPositionReached())
                {
                    floaty.SetRandomTargetPosition(floaty.nullVector3);
                }

                floaty.MoveTowardsTargetPosition(speed);
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
            case GoldfishState.Wandering:
                floaty.SetRandomTargetPosition(floaty.nullVector3);
                break;
        }

        _state = newState;
    }

    public void Grab(GameObject grabberObject)
    {
        grabber = grabberObject;
        floaty.SetTargetPosition(grabber.transform.position);
        SwitchState(GoldfishState.Grabbed);
    }

    public bool IsGrabbed()
    {
        if (_state == GoldfishState.Grabbed)
            return true;
        return false;
    }

    public bool IsTreasure()
    {
        if (_state == GoldfishState.Treasure)
            return true;
        return false;
    }
}

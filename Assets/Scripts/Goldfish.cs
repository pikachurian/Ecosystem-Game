using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Goldfish : MonoBehaviour
{
    public float grabFollowForce = 0.5f;
    public int childrenMax = 5;
    public int childrenMin = 2;

    //public float newPositionRadius = 4f;
    //public float targetReachedRadius = 1f;

    public LayerMask goldfishMask;
    public LayerMask fishFoodMask;

    private Rigidbody2D rb;
    private Floaty floaty;
    private enum GoldfishState { Treasure, Grabbed, Wandering, GoingToFood, GoingToMate, Mating, RunAway}
    private GoldfishState _state = GoldfishState.Treasure;

    //private Vector3 targetPosition = Vector3.zero;
    //private float nullFloat = 1000.1f;
    //private Vector3 nullVector3 = Vector3.zero;

    private GameObject grabber;

    private FishFood targetedFood;
    private Goldfish mate;

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
                floaty.SetTargetPosition(grabber.transform.position);
                floaty.MoveTowardsTargetPosition(grabFollowForce);
                floaty.ApplyFriction();
                break;

            case GoldfishState.Wandering:

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
        floaty.SetTargetPosition(grabber.transform.position);
        SwitchState(GoldfishState.Grabbed);
    }

    public bool IsGrabbed()
    {
        if (_state == GoldfishState.Grabbed)
            return true;
        return false;
    }
}

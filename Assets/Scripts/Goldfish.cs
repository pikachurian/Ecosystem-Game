using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Goldfish : MonoBehaviour
{
    public GoldfishState startingState;
    public AudioSource audio;
    public AudioClip chompSound;
    public SpriteRenderer spriteRenderer;
    public Sprite activeSprite;
    public Sprite treasureSprite;
    public float grabFollowForce = 0.5f;

    public float speed = 0.1f;

    public float foodSearchRadius = 4f;

    public int childrenMax = 3;
    public int childrenMin = 2;

    public LayerMask goldfishMask;
    public LayerMask fishFoodMask;

    private Rigidbody2D rb;
    private Floaty floaty;
    public enum GoldfishState { Treasure, Grabbed, Wandering, GoingToFood, Reproducing, RunAway}
    private GoldfishState _state = GoldfishState.Treasure;

    private GameObject grabber;

    private FishFood targetedFood;
    private Goldfish mate;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        floaty = GetComponent<Floaty>();
        SwitchState(startingState);
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
                    //Look for food.
                    #region
                    Collider2D[] foodNearMe = Physics2D.OverlapCircleAll(transform.position, foodSearchRadius, fishFoodMask);
                    if(foodNearMe.Length > 0)
                    {
                        GameObject closestFood = foodNearMe[0].gameObject;
                        for(int i = 0; i < foodNearMe.Length; i ++)
                        {
                            if (Vector3.Distance(transform.position, foodNearMe[i].transform.position) <
                                    Vector3.Distance(transform.position, closestFood.transform.position))
                            {
                                closestFood = foodNearMe[i].gameObject;
                            }
                        }

                        //Go to food
                        floaty.SetTargetPosition(closestFood.transform.position);
                        targetedFood = closestFood.GetComponent<FishFood>();
                        SwitchState(GoldfishState.GoingToFood);
                    }
                    #endregion

                    floaty.SetRandomTargetPosition(floaty.nullVector3);
                }

                floaty.MoveTowardsTargetPosition(speed);
                floaty.ApplyFriction();
                break;

            case GoldfishState.GoingToFood:
                if(GameObject.Find(targetedFood.name) == null)
                {
                    SwitchState(GoldfishState.Wandering);
                    targetedFood = null;
                }else if (floaty.TargetPositionReached())
                {

                }
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
                spriteRenderer.sprite = activeSprite;
                break;

            case GoldfishState.Treasure:
                spriteRenderer.sprite = treasureSprite;
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

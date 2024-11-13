using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Goldfish : MonoBehaviour
{
    public GoldfishState startingState;
    private AudioSource audio;
    public GameObject goldfishPrefab;
    public AudioClip chompSound;
    public AudioClip bornSound;
    public SpriteRenderer spriteRenderer;
    public Sprite activeSprite;
    public Sprite treasureSprite;
    public float grabFollowForce = 0.5f;

    public float speed = 0.1f;

    public float foodSearchRadius = 4f;

    public int childrenMax = 3;
    public int childrenMin = 2;

    public int foodNeededMax = 5;
    public int foodNeededMin = 3;

    public float treasureVelocityDampen = 0.2f;

    public LayerMask goldfishMask;
    public LayerMask fishFoodMask;

    private Rigidbody2D rb;
    private Floaty floaty;
    public enum GoldfishState { Treasure, Grabbed, Wandering, GoingToFood, Reproducing, RunAway}
    private GoldfishState _state = GoldfishState.Treasure;

    private GameObject grabber;

    private FishFood targetedFood;
    private Goldfish mate;

    private int childrenToSpawn = 1;

    private int foodEaten = 0;
    private int foodNeeded = 3;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        floaty = GetComponent<Floaty>();
        audio = GetComponent<AudioSource>();
        SwitchState(startingState);
        foodNeeded = Random.Range(foodNeededMin, foodNeededMax);
        childrenToSpawn = Random.Range(childrenMin, childrenMax);
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
                        List<Collider2D> foodList = new List<Collider2D>();

                        //Remove eaten food
                        for (int i = 0; i < foodNearMe.Length; i ++)
                        {
                            if (foodNearMe[i] != null && foodNearMe[i].GetComponent<FishFood>().isEaten == false)
                                foodList.Add(foodNearMe[i]);
                        }

                        if (foodList.Count > 0)
                        {
                            GameObject closestFood = foodList[0].gameObject;
                            for (int i = 0; i < foodList.Count; i++)
                            {
                                if (Vector3.Distance(transform.position, foodList[i].transform.position) <
                                        Vector3.Distance(transform.position, closestFood.transform.position))
                                {
                                    closestFood = foodList[i].gameObject;
                                }
                            }

                            //Go to food
                            floaty.SetTargetPosition(closestFood.transform.position);
                            targetedFood = closestFood.GetComponent<FishFood>();
                            SwitchState(GoldfishState.GoingToFood);
                        }
                    }
                    
                    if(_state != GoldfishState.GoingToFood)
                        floaty.SetRandomTargetPosition(floaty.nullVector3);
                    #endregion

                }

                floaty.MoveTowardsTargetPosition(speed);
                floaty.ApplyFriction();
                break;

            case GoldfishState.GoingToFood:
                if(targetedFood.isEaten)
                {
                    SwitchState(GoldfishState.Wandering);
                    targetedFood = null;
                }else if (floaty.TargetPositionReached())
                {
                    targetedFood.GetComponent<FishFood>().Eaten();
                    audio.PlayOneShot(chompSound);

                    foodEaten += 1;
                    if (foodEaten >= foodNeeded)
                    {
                        SwitchState(GoldfishState.Reproducing);
                    }
                }

                if (targetedFood != null)
                    floaty.SetTargetPosition(targetedFood.transform.position);

                floaty.MoveTowardsTargetPosition(speed);
                floaty.ApplyFriction();
                break;

            case GoldfishState.Reproducing:
                SwitchState(GoldfishState.Treasure);
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

            case GoldfishState.Reproducing:
                for (int i = 0; i < childrenToSpawn; i ++)
                    Instantiate(goldfishPrefab);
                audio.PlayOneShot(bornSound);
                rb.velocity *= treasureVelocityDampen;
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

    public void Eaten()
    {
        floaty.isEaten = true;
        spriteRenderer.enabled = false;
        this.enabled = false;
    }

    public void Ungrab()
    {
        grabber = null;
        rb.velocity *= treasureVelocityDampen;
        SwitchState(GoldfishState.Treasure);
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

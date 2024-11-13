using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Goldfish;

public class Shark : MonoBehaviour
{
    public SharkState startingState;
    public AudioSource audio;
    public AudioClip chompSound;
    public AudioClip sleepSound;
    public AudioClip chaseSound;
    public SpriteRenderer spriteRenderer;
    public Sprite activeSprite;
    public Sprite sleepSprite;
    public Sprite chaseSprite;

    public float speed = 0.1f;
    public float enterSpeed = 1f;

    public float chaseStartRadius = 2f;

    public float lifeTimeMax = 20f;
    public float lifeTimeMin = 15f;

    public float stateTimeMax = 10f;
    public float stateTimeMin = 5f;

    public float treasureVelocityDampen = 0.2f;

    public float leaveRightX = 1f;
    public float leaveLeftX = -1f;

    public LayerMask chaseMask;

    private Rigidbody2D rb;
    private Floaty floaty;
    public enum SharkState { Enter, Leave, Wandering, Sleep, Chase}
    private SharkState _state = SharkState.Enter;

    private Floaty chaseTarget;

    private float lifeTick = 1f;
    private float stateTick = 1f;

    [HideInInspector]
    public bool enteredRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        floaty = GetComponent<Floaty>();
        audio = GetComponent<AudioSource>();
        SwitchState(startingState);
        lifeTick = Random.Range(lifeTimeMin, lifeTimeMax);
    }

    void FixedUpdate()
    {
        switch(_state)
        {
            case SharkState.Enter:
                if (enteredRight)
                {
                    rb.velocity = new Vector3(-enterSpeed, 0f);
                }else
                {
                    rb.velocity = new Vector3(enterSpeed, 0f);
                }

                if (transform.position.x > floaty.minX + 4 && transform.position.x < floaty.maxX - 4)
                {
                    SwitchState(SharkState.Sleep);
                }
                break;

            case SharkState.Wandering:
                if (floaty.TargetPositionReached())
                {
                        floaty.SetRandomTargetPosition(floaty.nullVector3);
                }

                if (stateTick <= 0f)
                {
                    SwitchState(SharkState.Sleep);
                }
                else stateTick -= Time.deltaTime;

                CheckChase();

                floaty.MoveTowardsTargetPosition(speed);
                floaty.ApplyFriction();
                UpdateLifeTime();
                break;

            case SharkState.Chase:
                if(chaseTarget.isEaten)
                {
                    SwitchState(SharkState.Wandering);
                    chaseTarget = null;
                }else if (floaty.TargetPositionReached())
                {
                    //chaseTarget.isEaten = true;
                    if(chaseTarget.GetComponent<Diver>())
                    {
                        chaseTarget.GetComponent<Diver>().Eaten();
                    }else if (chaseTarget.GetComponent<Goldfish>())
                    {
                        chaseTarget.GetComponent<Goldfish>().Eaten();
                    }

                    audio.PlayOneShot(chompSound);
                    SwitchState(SharkState.Sleep);
                }

                if (chaseTarget != null)
                    floaty.SetTargetPosition(chaseTarget.transform.position);

                floaty.MoveTowardsTargetPosition(speed);
                floaty.ApplyFriction();
                UpdateLifeTime();
                break;

            case SharkState.Sleep:
                if (stateTick <= 0f)
                {
                    SwitchState(SharkState.Wandering);
                }
                else stateTick -= Time.deltaTime;
                floaty.ApplyFriction();
                UpdateLifeTime();
                break;

            case SharkState.Leave:
                if (enteredRight)
                {
                    rb.velocity = new Vector3(-enterSpeed, 0f);
                }
                else
                {
                    rb.velocity = new Vector3(enterSpeed, 0f);
                }

                if (transform.position.x < floaty.minX - 4 || transform.position.x > floaty.maxX + 4)
                {
                    Destroy(this.gameObject);
                }
                break;
        }
    }

    private bool CheckChase()
    {
        Collider2D[] objectsNearMe = Physics2D.OverlapCircleAll(transform.position, chaseStartRadius, chaseMask);
        if (objectsNearMe.Length > 0)
        {
            List<Collider2D> objectsList = new List<Collider2D>();

            //Remove eaten food
            for (int i = 0; i < objectsNearMe.Length; i++)
            {
                if (objectsNearMe[i] != null)
                {
                    Goldfish goldFish = objectsNearMe[i].GetComponent<Goldfish>();
                    if (goldFish != null && goldFish.IsTreasure() == false)
                        objectsList.Add(objectsNearMe[i]);
                    else if(goldFish == null)
                        objectsList.Add(objectsNearMe[i]);

                }
            }

            if (objectsList.Count > 0)
            {
                GameObject closestObject = objectsList[0].gameObject;
                for (int i = 0; i < objectsList.Count; i++)
                {
                    if (Vector3.Distance(transform.position, objectsList[i].transform.position) <
                            Vector3.Distance(transform.position, closestObject.transform.position))
                    {
                        closestObject = objectsList[i].gameObject;
                    }
                }

                //Go to food
                floaty.SetTargetPosition(closestObject.transform.position);
                chaseTarget = closestObject.GetComponent<Floaty>();
                SwitchState(SharkState.Chase);
                return true;
            }
        }

        return false;
    }

    private void SwitchState(SharkState newState)
    {
        //Previous state
        switch (_state)
        {
            case SharkState.Sleep:
                stateTick = Random.Range(stateTimeMin, stateTimeMax);
                break;
        }

        //New state
        switch (newState)
        {
            case SharkState.Enter:
            case SharkState.Leave:
                spriteRenderer.sprite = activeSprite;
                break;

            case SharkState.Wandering:
                floaty.SetRandomTargetPosition(floaty.nullVector3);
                spriteRenderer.sprite = activeSprite;
                break;

            case SharkState.Sleep:
                spriteRenderer.sprite = sleepSprite;
                stateTick = Random.Range(stateTimeMin, stateTimeMax);
                rb.velocity = Vector2.zero;
                audio.PlayOneShot(sleepSound);
                break;

            case SharkState.Chase:
                audio.PlayOneShot(chaseSound);
                spriteRenderer.sprite = chaseSprite;
                break;
        }

        _state = newState;
    }

    private void UpdateLifeTime()
    {
        if (lifeTick <= 0f)
        {
            SwitchState(SharkState.Leave);
        }
        else lifeTick -= Time.deltaTime;
    }
}

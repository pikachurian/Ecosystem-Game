using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Diver : MonoBehaviour
{
    public AudioSource audio;
    public AudioClip dive;
    public float surfaceY = 0f;
    public float speed = 1f;
    public float enterMapSpeed = 3f;
    public float enterMapDepth = 3f;
    public LayerMask treasureMask;

    public float startUpTimeMax = 2f;
    public float startUpTimeMin = 0.05f;

    public float diveRecoveryTime = 1f;
    

    private Rigidbody2D rb = null;
    private Floaty floaty = null;
    private enum DiverState { EnterMap, Searching, TreasureFound, Surface, RunAway, GoingToTreasure, StartUp, DiveRecovery}
    private DiverState _state = DiverState.EnterMap;

    private Goldfish targetedTreasure = null;

    private float startUpTick = 0f;
    private float startUpTime = 1f;

    private float diveRecoveryTick = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        floaty = GetComponent<Floaty>();
        SwitchState(DiverState.StartUp);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(_state)
        {
            case DiverState.StartUp:
                if (startUpTick >= startUpTime)
                {
                    SwitchState(DiverState.EnterMap);

                }
                else startUpTick += Time.deltaTime;
                
                break;

            case DiverState.DiveRecovery:
                if (diveRecoveryTick >= diveRecoveryTime)
                {
                    SwitchState(DiverState.Searching);

                }
                else diveRecoveryTick += Time.deltaTime;

                floaty.ApplyFriction(0.1f);
                break;

            case DiverState.EnterMap:
                //Exit state
                if(floaty.TargetPositionReached())
                {
                    SwitchState(DiverState.DiveRecovery);
                }
                break;

            case DiverState.Searching:
                if (floaty.TargetPositionReached())
                {
                    //Check if treasure is within the newPositionRadius
                    Collider2D[] treasuresNearMe = Physics2D.OverlapCircleAll(transform.position, floaty.newPositionRadius, treasureMask);
                    print(treasuresNearMe);
                    if (treasuresNearMe.Length > 0)
                    {
                        List<Collider2D> treasureList = new List<Collider2D>();

                        //Remove grabbed treasures from the array
                        for (int i = 0; i < treasuresNearMe.Length; i ++)
                        {
                            if (treasuresNearMe[i] != null && treasuresNearMe[i].GetComponent<Goldfish>().IsTreasure() && treasuresNearMe[i].gameObject.GetComponent<Goldfish>().IsGrabbed() == false)
                            {
                                treasureList.Add(treasuresNearMe[i]);
                            }
                        }
                        
                        //Check if there are available treasures
                        if (treasureList.Count > 0)
                        {
                            //Get closest tresure
                            GameObject closestTreasure = treasureList[0].gameObject;
                            for (int i = 0; i < treasureList.Count; i ++)
                            {
                                if (Vector3.Distance(transform.position, treasureList[i].transform.position) <
                                    Vector3.Distance(transform.position, closestTreasure.transform.position))
                                {
                                    closestTreasure = treasureList[i].gameObject;
                                }
                            }

                            //Set target position to treasure
                            print(name + " is moving to treasure");
                            floaty.SetTargetPosition(closestTreasure.transform.position);
                            targetedTreasure = closestTreasure.GetComponent<Goldfish>();
                            SwitchState(DiverState.GoingToTreasure);
                        }
                    }
                    else
                    {

                        floaty.SetRandomTargetPosition(floaty.nullVector3);
                        print(name + " reached target position");
                    }
                }

                floaty.MoveTowardsTargetPosition(speed);
                floaty.ApplyFriction();
                break;

            case DiverState.GoingToTreasure:
                if (targetedTreasure.IsGrabbed())
                {
                    SwitchState(DiverState.DiveRecovery);
                    targetedTreasure = null;
                }else if (floaty.TargetPositionReached())
                {
                    targetedTreasure.GetComponent<Goldfish>().Grab(gameObject);
                    SwitchState(DiverState.Surface);
                }

                floaty.MoveTowardsTargetPosition(speed);
                floaty.ApplyFriction();
                break;

            case DiverState.Surface:
                if (floaty.TargetPositionReached())
                {
                    Surfaced();
                }

                floaty.MoveTowardsTargetPosition(speed);
                floaty.ApplyFriction();
                break;
        }
    }

    private void SwitchState(DiverState newState)
    {
        //Previous state
        switch(_state)
        {
            case DiverState.StartUp:
                rb.simulated = true;
                break;

            case DiverState.EnterMap:
                if (rb == null)
                    rb = GetComponent<Rigidbody2D>();
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.1f);
                break;
        }

        //New state
        switch (newState)
        {
            case DiverState.StartUp:
                startUpTick = 0f;
                startUpTime = Random.Range(startUpTimeMin, startUpTimeMax);
                rb.simulated = false;
                break;

            case DiverState.DiveRecovery:
                diveRecoveryTick = 0f;
                break;

            case DiverState.EnterMap:
                floaty.SetRandomTargetPosition(floaty.nullVector3);
                floaty.SetTargetPosition(new Vector3(floaty.targetPosition.x, enterMapDepth, floaty.targetPosition.z));
                floaty.MoveTowardsTargetPosition(enterMapSpeed);
                break;

            case DiverState.Searching:
                floaty.SetRandomTargetPosition(floaty.nullVector3);
                break;

            case DiverState.Surface:
                floaty.SetTargetPosition(new Vector3(transform.position.x, surfaceY, transform.position.z));
                break;
        }

        _state = newState;
    }

    private void Surfaced()
    {
        EntityManager.reference.DeleteInstanceFromList(EntityManager.EntityListType.Diver, this.gameObject);
        print(name + " surfaced");
        Destroy(targetedTreasure.gameObject);
        Destroy(this.gameObject);
    }

    public void StartUp()
    {
        SwitchState(DiverState.StartUp);
    }
}

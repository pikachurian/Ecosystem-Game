using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Diver : MonoBehaviour
{
    //public float minX = 0f;
    //public float maxX = 100f;
    //public float minY = 0f;
    //public float maxY = 100f;
    public float surfaceY = 0f;
    //public float newPositionRadius = 4f;
    public float speed = 1f;
    public float enterMapSpeed = 3f;
    public float enterMapDepth = 3f;
    //public float targetReachedRadius = 1f;
    //public float waterFricLerp = 0.5f;
    public LayerMask treasureMask;

    public float startUpTimeMax = 2f;
    public float startUpTimeMin = 0.05f;

    public float diveRecoveryTime = 1f;
    

    private Rigidbody2D rb = null;
    private Floaty floaty = null;
    private enum DiverState { EnterMap, Searching, TreasureFound, Surface, RunAway, GoingToTreasure, StartUp, DiveRecovery}
    private DiverState _state = DiverState.EnterMap;

    //private Vector3 targetPosition = Vector3.zero;
    //private float nullFloat = 1000.1f;
    //private Vector3 nullVector3 = Vector3.zero;

    private Goldfish targetedTreasure = null;

    private float startUpTick = 0f;
    private float startUpTime = 1f;

    private float diveRecoveryTick = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //nullVector3 = new Vector3(nullFloat, nullFloat, nullFloat);
        rb = GetComponent<Rigidbody2D>();
        floaty = GetComponent<Floaty>();
        //SwitchState(DiverState.EnterMap);
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
                            if (treasuresNearMe[i] != null && treasuresNearMe[i].gameObject.GetComponent<Goldfish>().IsGrabbed() == false)
                            {
                                treasureList.Add(treasuresNearMe[i]);
                            }
                        }
                        
                        //Check if there are available treasures
                        if (treasureList.Count > 0)
                        {
                            //Get closest tresure
                            GameObject closestTreasure = treasureList[0].gameObject;
                            for (int i = 0; i < treasureList.Count; i++)
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
                //floaty.PushOffTowardsRandomTargetPosition(new Vector3(floaty.nullFloat, enterMapDepth, floaty.nullFloat), enterMapSpeed);
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

    /*private void PushOffTowardsRandomTargetPosition(Vector3 positionOverride, float force)
    {
        floaty.SetRandomTargetPosition(positionOverride);

        //Vector3 dir = targetPosition - transform.position;
        //rb.AddForce(dir * force, ForceMode2D.Impulse);
        floaty.MoveTowardsTargetPosition(force);
    }*/

    private void Surfaced()
    {
        EntityManager.reference.DeleteInstanceFromList(EntityManager.EntityListType.Diver, this.gameObject);
        print(name + " surfaced");
        Destroy(targetedTreasure.gameObject);
        Destroy(this.gameObject);
    }

    /*private void SetRandomTargetPosition(Vector3 positionOverride)
    {
        if (positionOverride != null && positionOverride.x != nullFloat)
            targetPosition.x = positionOverride.x;
        else
        {
            float rollX = Random.Range(-newPositionRadius, newPositionRadius);
            targetPosition.x = transform.position.x + rollX;
        }

        if (positionOverride != null && positionOverride.y !=nullFloat)
            targetPosition.y = positionOverride.y;
        else
        {
            float rollY = Random.Range(-newPositionRadius, newPositionRadius);
            targetPosition.y = transform.position.y + rollY;
        }

        //Clamp values inside window
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
    }*/

    /*private void MoveTowards(Vector3 target, float force)
    {
        Vector3 dir = target - transform.position;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
    }*/

    public void StartUp()
    {
        SwitchState(DiverState.StartUp);
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(targetPosition, targetReachedRadius);

        //Gizmos.DrawWireSphere(transform.position, newPositionRadius);
    }
}

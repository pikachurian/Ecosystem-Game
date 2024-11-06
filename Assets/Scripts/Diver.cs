using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Diver : MonoBehaviour
{
    public float minX = 0f;
    public float maxX = 100f;
    public float minY = 0f;
    public float maxY = 100f;
    public float surfaceY = 0f;
    public float newPositionRadius = 4f;
    public float speed = 1f;
    public float enterMapSpeed = 3f;
    public float enterMapDepth = 3f;
    public float targetReachedRadius = 1f;
    //public float waterFricLerp = 0.5f;
    public LayerMask treasureMask;

    public float startUpTimeMax = 2f;
    public float startUpTimeMin = 0.05f;

    public float diveRecoveryTime = 1f;
    

    private Rigidbody2D rb;
    private Floaty floaty;
    private enum DiverState { EnterMap, Searching, TreasureFound, Surface, RunAway, goingToTreasure, StartUp, DiveRecovery}
    private DiverState _state = DiverState.EnterMap;

    private Vector3 targetPosition = Vector3.zero;
    private float nullFloat = 1000.1f;
    private Vector3 nullVector3 = Vector3.zero;

    private GameObject targetedTreasure = null;

    private float startUpTick = 0f;
    private float startUpTime = 1f;

    private float diveRecoveryTick = 0f;

    // Start is called before the first frame update
    void Start()
    {
        nullVector3 = new Vector3(nullFloat, nullFloat, nullFloat);
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
                if(Vector3.Distance(transform.position, targetPosition) <= targetReachedRadius)
                {
                    SwitchState(DiverState.DiveRecovery);
                }
                break;

            case DiverState.Searching:
                if (Vector3.Distance(transform.position, targetPosition) <= targetReachedRadius)
                {
                    //Check if treasure is within the newPositionRadius
                    Collider2D[] treasuresNearMe = Physics2D.OverlapCircleAll(transform.position, newPositionRadius, treasureMask);
                    print(treasuresNearMe);
                    if (treasuresNearMe.Length > 0)
                    {
                        //Get closest tresure
                        GameObject closestTreasure = treasuresNearMe[0].gameObject;
                        for (int i = 0; i < treasuresNearMe.Length; i++)
                        {
                            if (Vector3.Distance(transform.position, treasuresNearMe[i].transform.position) <
                                Vector3.Distance(transform.position, closestTreasure.transform.position))
                            {
                                closestTreasure = treasuresNearMe[i].gameObject;
                            }
                        }

                        //Set target position to treasure
                        print(name + " is moving to treasure");
                        SetRandomTargetPosition(closestTreasure.transform.position);
                        targetedTreasure = closestTreasure;
                        SwitchState(DiverState.goingToTreasure);
                    }
                    else
                    {

                        SetRandomTargetPosition(nullVector3);
                        print(name + " reached target position");
                    }
                }

                floaty.MoveTowards(targetPosition, speed);
                floaty.ApplyFriction();
                break;

            case DiverState.goingToTreasure:
                if (Vector3.Distance(transform.position, targetPosition) <= targetReachedRadius)
                {
                    targetedTreasure.GetComponent<Goldfish>().Grab(gameObject);
                    SwitchState(DiverState.Surface);
                }

                floaty.MoveTowards(targetPosition, speed);
                floaty.ApplyFriction();
                break;

            case DiverState.Surface:
                if (Vector3.Distance(transform.position, targetPosition) <= targetReachedRadius)
                {
                    Surfaced();
                }

                floaty.MoveTowards(targetPosition, speed);
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
                PushOffTowardsRandomTargetPosition(new Vector3(nullFloat, enterMapDepth, nullFloat), enterMapSpeed);
                break;

            case DiverState.Searching:
                SetRandomTargetPosition(nullVector3);
                break;

            case DiverState.Surface:
                targetPosition = new Vector3(transform.position.x, surfaceY, transform.position.z);
                break;
        }

        _state = newState;
    }

    private void PushOffTowardsRandomTargetPosition(Vector3 positionOverride, float force)
    {
        SetRandomTargetPosition(positionOverride);

        //Vector3 dir = targetPosition - transform.position;
        //rb.AddForce(dir * force, ForceMode2D.Impulse);
        floaty.MoveTowards(targetPosition, force);
    }

    private void Surfaced()
    {
        print(name + " surfaced");
        Destroy(targetedTreasure);
        Destroy(this.gameObject);
    }

    private void SetRandomTargetPosition(Vector3 positionOverride)
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
    }

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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPosition, targetReachedRadius);

        Gizmos.DrawWireSphere(transform.position, newPositionRadius);
    }
}

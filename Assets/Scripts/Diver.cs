using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diver : MonoBehaviour
{
    public float newPositionRadius = 4f;
    public float speed = 1f;
    public float enterMapSpeed = 3f;
    public float enterMapDepth = 3f;
    public float targetReachedRadius = 1f;
    public float waterFricLerp = 0.5f;

    private Rigidbody2D rb;
    private enum DiverState { EnterMap, Searching, TreasureFound, Surface, RunAway}
    private DiverState _state = DiverState.EnterMap;

    private Vector3 targetPosition = Vector3.zero;
    private float nullFloat = 1000.1f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SwitchState(DiverState.EnterMap);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(_state)
        {
            case DiverState.EnterMap:
                //Exit state
                if(Vector3.Distance(transform.position, targetPosition) <= targetReachedRadius)
                {
                    SwitchState(DiverState.Searching);
                }
                break;

            case DiverState.Searching:
                

                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, waterFricLerp);
                break;
        }
    }

    private void SwitchState(DiverState newState)
    {
        //Previous state
        switch(_state)
        {
            case DiverState.EnterMap:
                //rb.velocity *= 0.1f;
                break;
        }

        //New state
        switch (newState)
        {
            case DiverState.EnterMap:
                /*float roll = Random.Range(-newPositionRadius, newPositionRadius);
                targetPosition.x = transform.position.x + roll;
                targetPosition.y = enterMapDepth;

                Vector3 dir = targetPosition - transform.position;
                rb.AddForce(dir * enterMapSpeed, ForceMode2D.Impulse);*/
                PushOffTowardsRandomTargetPosition(new Vector3(nullFloat, enterMapDepth, nullFloat));
                break;

            case DiverState.Searching:
                /*float rollX = Random.Range(-newPositionRadius, newPositionRadius);
                targetPosition.x = transform.position.x + rollX;
                float rollY = Random.Range(-newPositionRadius, newPositionRadius);
                targetPosition.y = transform.position.x + rollY;

                rb.AddForce((targetPosition - transform.position) * enterMapSpeed, ForceMode2D.Impulse);*/
                break;
        }

        _state = newState;
    }

    private void PushOffTowardsRandomTargetPosition(Vector3 positionOverride)
    {
        SetRandomTargetPosition(positionOverride);

        Vector3 dir = targetPosition - transform.position;
        rb.AddForce(dir * enterMapSpeed, ForceMode2D.Impulse);
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
            targetPosition.y = transform.position.x + rollY;
        }
    }
}

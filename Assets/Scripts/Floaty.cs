using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Floaty : MonoBehaviour
{
    public float waterFricLerp = 0.5f;

    public float minX = 0f;
    public float maxX = 100f;
    public float minY = 0f;
    public float maxY = 100f;

    public float newPositionRadius = 4f;
    public float targetReachedRadius = 1f;

    public Vector3 targetPosition = Vector3.zero;
    [HideInInspector]
    public float nullFloat = 1000.1f;
    [HideInInspector]
    public Vector3 nullVector3 = Vector3.zero;

    private Rigidbody2D rb;

    private void Start()
    {
        nullVector3 = new Vector3(nullFloat, nullFloat, nullFloat);
        rb = GetComponent<Rigidbody2D>();
    }
    public void ApplyFriction(float frictionLerpOverride = 1000)
    {
        if(frictionLerpOverride != 1000)
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, frictionLerpOverride);
        else
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, waterFricLerp);
    }

    public void MoveTowardsTargetPosition(float force)
    {
        Vector3 dir = targetPosition - transform.position;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
    }

    public void PushOffTowardsRandomTargetPosition(Vector3 positionOverride, float force)
    {
        SetRandomTargetPosition(positionOverride);

        //Vector3 dir = targetPosition - transform.position;
        //rb.AddForce(dir * force, ForceMode2D.Impulse);
        MoveTowardsTargetPosition(force);
    }

    public bool TargetPositionReached()
    {
        if (Vector3.Distance(transform.position, targetPosition) <= targetReachedRadius)
            return true;
        return false;
    }


    public void SetRandomTargetPosition(Vector3 positionOverride)
    {
        if (positionOverride != null && positionOverride.x != nullFloat)
            targetPosition.x = positionOverride.x;
        else
        {
            float rollX = Random.Range(-newPositionRadius, newPositionRadius);
            targetPosition.x = transform.position.x + rollX;
        }

        if (positionOverride != null && positionOverride.y != nullFloat)
            targetPosition.y = positionOverride.y;
        else
        {
            float rollY = Random.Range(-newPositionRadius, newPositionRadius);
            targetPosition.y = transform.position.y + rollY;
        }

        //Clamp values inside window
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        print("TargetX " + (targetPosition.x - transform.position.x) + " radius " + newPositionRadius);
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    private void OnDrawGizmosSelected()
    {
        //Draw position bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 1f),
            new Vector3(maxX - minX, maxY - minY, 1f));

        Gizmos.DrawWireSphere(targetPosition, targetReachedRadius);

        Gizmos.DrawWireSphere(transform.position, newPositionRadius);
    }
}

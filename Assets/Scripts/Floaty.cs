using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floaty : MonoBehaviour
{
    public float waterFricLerp = 0.5f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void ApplyFriction(float frictionLerpOverride = 1000)
    {
        if(frictionLerpOverride != 1000)
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, frictionLerpOverride);
        else
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, waterFricLerp);
    }

    public void MoveTowards(Vector3 target, float force)
    {
        Vector3 dir = target - transform.position;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
    }
}

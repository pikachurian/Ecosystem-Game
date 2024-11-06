using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishFood : MonoBehaviour
{
    public float speed = 1f;
    public float despawnHeight = 8f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        transform.position = transform.position + new Vector3(0f, speed * Time.deltaTime, 0f);

        //Despawn
        if(transform.position.y >= despawnHeight)
        {
            Despawn();
        }
    }

    private void Despawn()
    {
        EntityManager.reference.DeleteInstanceFromList(EntityManager.EntityListType.FishFood, this.gameObject);
        Destroy(this.gameObject);
    }
}

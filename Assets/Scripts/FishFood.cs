using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishFood : MonoBehaviour
{
    public float speed = 1f;
    public float despawnHeight = 8f;
    public float eatenDespawnTime = 0.5f;
    public float readyTimeMax = 4f;
    public float readyTimeMin = 1f;

    private Rigidbody2D rb;
    [HideInInspector]
    public bool isEaten = false;

    private SpriteRenderer spriteRenderer;
    private float readyTime = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        readyTime = Random.Range(readyTimeMin, readyTimeMax);
    }

    private void FixedUpdate()
    {
        transform.position = transform.position + new Vector3(0f, speed * Time.deltaTime, 0f);

        readyTime -= Time.deltaTime;

        //Despawn
        if(transform.position.y >= despawnHeight)
        {
            Eaten();
        }

        if (isEaten)
        {
            if (eatenDespawnTime <= 0f)
            {
                Despawn();
            }
            else eatenDespawnTime -= Time.deltaTime;
        }
        
    }

    public void Eaten()
    {
        isEaten = true;
        spriteRenderer.enabled = false;
    }

    public bool IsReady()
    {
        if (readyTime <= 0f)
            return true;
        return false;
    }

    public void Despawn()
    {
        EntityManager.reference.DeleteInstanceFromList(EntityManager.EntityListType.FishFood, this.gameObject);
        Destroy(this.gameObject);
    }

}

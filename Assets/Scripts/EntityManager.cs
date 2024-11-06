using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public enum EntityListType { Diver, Goldfish};


    [HideInInspector]
    public static EntityManager reference;
    [HideInInspector]
    public List<GameObject> divers = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> goldfish = new List<GameObject>();

    public GameObject diverPrefab;

    public float diverSpawnX1 = -5f;
    public float diverSpawnY1 = -5f;
    public float diverSpawnX2 = 5f;
    public float diverSpawnY2 = 5f;

    public int startingDiversMax = 8;
    public int startingDiversMin = 3;


    private void OnEnable()
    {
        reference = this;
    }

    private void Start()
    {
        SpawnDivers();
    }

    private void SpawnDivers()
    {
        int startingDivers = (int)Random.Range((float)startingDiversMin, (float)startingDiversMax);

        for (int i = 0; i < startingDivers; i ++)
        {
            GameObject diver = Instantiate(diverPrefab);
            Vector3 startingPosition = Vector3.zero;
            startingPosition.x = Random.Range(diverSpawnX1, diverSpawnX2);
            startingPosition.y = Random.Range(diverSpawnY1, diverSpawnY2);
            diver.transform.position = startingPosition;
            diver.GetComponent<Diver>().StartUp();
            divers.Add(diver);
        }
    }

    public void DeleteInstanceFromList(EntityListType listType, GameObject instance)
    {
        switch(listType)
        {
            case EntityListType.Diver:
                divers.Remove(instance);
                break;

            case EntityListType.Goldfish:
                goldfish.Remove(instance);
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3((diverSpawnX1 + diverSpawnX2) / 2, (diverSpawnY1 + diverSpawnY2) / 2, 1f),
            new Vector3(diverSpawnX2 - diverSpawnX1, diverSpawnY2 - diverSpawnY1, 1f));
    }
}

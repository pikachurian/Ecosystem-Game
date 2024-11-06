using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public enum EntityListType { Diver, Goldfish, FishFood};


    [HideInInspector]
    public static EntityManager reference;
    [HideInInspector]
    public List<GameObject> divers = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> goldfish = new List<GameObject>();
    //[HideInInspector]
    public List<GameObject> fishFoods = new List<GameObject>();

    public GameObject diverPrefab;
    public GameObject fishFoodPrefab;

    public float diverSpawnX1 = -5f;
    public float diverSpawnY1 = -5f;
    public float diverSpawnX2 = 5f;
    public float diverSpawnY2 = 5f;

    public float fishFoodSpawnX1 = -5f;
    public float fishFoodSpawnY1 = -5f;
    public float fishFoodSpawnX2 = 5f;
    public float fishFoodSpawnY2 = 5f;

    public int startingDiversMax = 8;
    public int startingDiversMin = 3;

    public float fishFoodSpawnTimeMax = 3f;
    public float fishFoodSpawnTimeMin = 0.5f;

    private float fishFoodSpawnTime = 1f;
    private float fishFoodSpawnTick = 0f;


    private void OnEnable()
    {
        reference = this;
    }

    private void Start()
    {
        SpawnDivers();
    }

    private void Update()
    {
        //Spawn fish food
        if (fishFoodSpawnTick >= fishFoodSpawnTime)
        {
            InstantiateWithinArea(fishFoodPrefab, fishFoodSpawnX1, fishFoodSpawnY1, fishFoodSpawnX2, fishFoodSpawnY2, EntityListType.FishFood);
            fishFoodSpawnTime = Random.Range(fishFoodSpawnTimeMin, fishFoodSpawnTimeMax);
            fishFoodSpawnTick = 0;
        }
        else fishFoodSpawnTick += Time.deltaTime;
    }

    private void SpawnDivers()
    {
        int startingDivers = (int)Random.Range((float)startingDiversMin, (float)startingDiversMax);

        for (int i = 0; i < startingDivers; i ++)
        {
            /*GameObject diver = Instantiate(diverPrefab);
            Vector3 startingPosition = Vector3.zero;
            startingPosition.x = Random.Range(diverSpawnX1, diverSpawnX2);
            startingPosition.y = Random.Range(diverSpawnY1, diverSpawnY2);
            diver.transform.position = startingPosition;
            diver.GetComponent<Diver>().StartUp();
            divers.Add(diver);*/
            InstantiateWithinArea(diverPrefab, diverSpawnX1, diverSpawnY1, diverSpawnX2, diverSpawnY2, EntityListType.Diver);
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

            case EntityListType.FishFood:
                fishFoods.Remove(instance);
                break;
        }
    }

    private void InstantiateWithinArea(GameObject prefab, float x1, float y1, float x2, float y2, EntityListType listType)
    {
        GameObject instance = Instantiate(prefab);
        Vector3 startingPosition = Vector3.zero;
        startingPosition.x = Random.Range(x1, x2);
        startingPosition.y = Random.Range(y1, y2);
        instance.transform.position = startingPosition;

        Diver diver = instance.GetComponent<Diver>();
        if(diver != null)
            diver.StartUp();

        switch(listType)
        {
            case EntityListType.Diver: divers.Add(instance); break;
            case EntityListType.Goldfish: goldfish.Add(instance); break;
            case EntityListType.FishFood: fishFoods.Add(instance); break;

        }
    }

    private void OnDrawGizmosSelected()
    {
        //Diver spawn
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3((diverSpawnX1 + diverSpawnX2) / 2, (diverSpawnY1 + diverSpawnY2) / 2, 1f),
            new Vector3(diverSpawnX2 - diverSpawnX1, diverSpawnY2 - diverSpawnY1, 1f));
        
        //Fish food spawn
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3((fishFoodSpawnX1 + fishFoodSpawnX2) / 2, (fishFoodSpawnY1 + fishFoodSpawnY2) / 2, 1f),
            new Vector3(fishFoodSpawnX2 - fishFoodSpawnX1, fishFoodSpawnY2 - fishFoodSpawnY1, 1f));
    }
}

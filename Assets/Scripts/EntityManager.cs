using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public enum EntityListType { Diver, Goldfish, FishFood, Shark};


    [HideInInspector]
    public static EntityManager reference;
    [HideInInspector]
    public List<GameObject> divers = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> goldfish = new List<GameObject>();
    //[HideInInspector]
    public List<GameObject> fishFoods = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> sharks = new List<GameObject>();

    public GameObject diverPrefab;
    public GameObject goldfishPrefab;
    public GameObject fishFoodPrefab;
    public GameObject sharkPrefab;

    public AudioSource audio;
    public AudioClip kaching;

    public float diverSpawnX1 = -5f;
    public float diverSpawnY1 = -5f;
    public float diverSpawnX2 = 5f;
    public float diverSpawnY2 = 5f;

    public float fishFoodSpawnX1 = -5f;
    public float fishFoodSpawnY1 = -5f;
    public float fishFoodSpawnX2 = 5f;
    public float fishFoodSpawnY2 = 5f;

    public float goldfishSpawnX1 = -5f;
    public float goldfishSpawnY1 = -5f;
    public float goldfishSpawnX2 = 5f;
    public float goldfishSpawnY2 = 5f;

    public float sharkSpawnY = 0f;
    public float sharkSpawnLeftX = -1f;
    public float sharkSpawnRightX = 1f;

    public int startingDiversMax = 8;
    public int startingDiversMin = 3;

    public int startingGoldfishMax = 8;
    public int startingGoldfishMin = 5;

    public int startingSharksMax = 3;
    public int startingSharksMin = 1;


    public float sharkSpawnTimeMax = 12f;
    public float sharkSpawnTimeMin = 8f;

    public float fishFoodSpawnTimeMax = 3f;
    public float fishFoodSpawnTimeMin = 0.5f;

    public float diverSpawnTime = 10f;

    private float fishFoodSpawnTime = 1f;
    private float fishFoodSpawnTick = 0f;

    private float diverSpawnTick = 0f;

    private float sharkSpawnTime = 1f;
    private float sharkSpawnTick = 0f;

    private void OnEnable()
    {
        reference = this;
    }

    private void Start()
    {
        SpawnDivers();
        SpawnGoldfish();
        SpawnSharks();
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

        //Spawn divers
        if (diverSpawnTick >= diverSpawnTime)
        {
            SpawnDiversAmount(1);
            diverSpawnTick = 0;
        }
        else diverSpawnTick += Time.deltaTime;

        //Spawn sharks
        if (sharkSpawnTick >= sharkSpawnTime)
        {
            SpawnSharksAmount(1);
            sharkSpawnTick = 0f;
            sharkSpawnTime = Random.Range(sharkSpawnTimeMin, sharkSpawnTimeMax);
        }
        else sharkSpawnTick += Time.deltaTime;
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

    private void SpawnDiversAmount(int amount)
    {
        for (int i = 0; i < amount; i ++)
        {
            InstantiateWithinArea(diverPrefab, diverSpawnX1, diverSpawnY1, diverSpawnX2, diverSpawnY2, EntityListType.Diver);
        }
    }

    public void DiverReturned()
    {
        audio.PlayOneShot(kaching);

        diverSpawnTick = diverSpawnTick * 0.8f;
    }

    private void SpawnGoldfish()
    {
        int startingGoldfish = (int)Random.Range((float)startingGoldfishMin, (float)startingGoldfishMax);

        for (int i = 0; i < startingGoldfish; i ++)
        {
            InstantiateWithinArea(goldfishPrefab, goldfishSpawnX1, goldfishSpawnY1, goldfishSpawnX2, goldfishSpawnY2, EntityListType.Goldfish);
        }
    }

    private void SpawnSharks()
    {
        int startingSharks = (int)Random.Range((float)startingSharksMin, (float)startingSharksMax);

        for (int i = 0; i < startingSharks; i++)
        {
            if(Random.value < 0.5)
            {
                //spawn right
                Shark sharkInst = Instantiate(sharkPrefab).GetComponent<Shark>();
                sharkInst.enteredRight = true;
                sharkInst.transform.position = new Vector3(sharkSpawnRightX, sharkSpawnY, 0f);
            }else
            {
                //spawn left
                Shark sharkInst = Instantiate(sharkPrefab).GetComponent<Shark>();
                sharkInst.enteredRight = false;
                sharkInst.transform.position = new Vector3(sharkSpawnLeftX, sharkSpawnY, 0f);
            }
        }
    }

    private void SpawnSharksAmount(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (Random.value < 0.5)
            {
                //spawn right
                Shark sharkInst = Instantiate(sharkPrefab).GetComponent<Shark>();
                sharkInst.enteredRight = true;
                sharkInst.transform.position = new Vector3(sharkSpawnRightX, sharkSpawnY, 0f);
            }
            else
            {
                //spawn left
                Shark sharkInst = Instantiate(sharkPrefab).GetComponent<Shark>();
                sharkInst.enteredRight = false;
                sharkInst.transform.position = new Vector3(sharkSpawnLeftX, sharkSpawnY, 0f);
            }
        }
    }

    public void DeleteInstanceFromList(EntityListType listType, GameObject instance)
    {/*
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
        }*/
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

        /*switch(listType)
        {
            case EntityListType.Diver: divers.Add(instance); break;
            case EntityListType.Goldfish: goldfish.Add(instance); break;
            case EntityListType.FishFood: fishFoods.Add(instance); break;
            //case EntityListType.Shark: sharks.Add(instance); break;
        }*/
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

        //Goldfish spawn
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3((goldfishSpawnX1 + goldfishSpawnX2) / 2, (goldfishSpawnY1 + goldfishSpawnY2) / 2, 1f),
            new Vector3(goldfishSpawnX2 - goldfishSpawnX1, goldfishSpawnY2 - goldfishSpawnY1, 1f));

        //Shark spawn
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(sharkSpawnLeftX, sharkSpawnY, 0f), new Vector3(sharkSpawnRightX, sharkSpawnY, 0f));
    }
}

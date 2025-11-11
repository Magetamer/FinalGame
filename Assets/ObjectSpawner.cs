using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { SmallGem, BigGem }

    [Header("References")]
    public Tilemap tilemap;
    public GameObject[] objectPrefabs; //0 = SmallGem, 1 = BigGem

    [Header("Spawn Settings")]
    public float bigGemProbability = 0.2f;
    public float smallGemProbability = 0.1f;
    public int maxObjects = 5;
    public float gemLifeTime = 10f;
    public float spawnInterval = 0.5f;

    private List<Vector3> validSpawnPos = new List<Vector3>();
    private List<GameObject> spawnObjects = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        //Safety checks
        if (tilemap == null)
        {
            Debug.LogError($"[ObjectSpawner] Tilemap is not assigned on {gameObject.name}.");
            return;
        }
        if (objectPrefabs == null || objectPrefabs.Length == 0)
        {
            Debug.LogError($"[ObjectSpawner] ObjectPrefabs array is empty on {gameObject.name}.");
            return;
        }

        GatherValidPositions();
        StartCoroutine(SpawnObjectsIfNeeded());
    }

    void Update()
    {
        if (tilemap == null || !tilemap.gameObject.activeInHierarchy)
        {
            LevelChange();
        }

        if (!isSpawning && ActiveObjectCount() < maxObjects)
        {
            StartCoroutine(SpawnObjectsIfNeeded());
        }
    }

    private void LevelChange()
    {
        GameObject ground = GameObject.Find("Ground");
        if (ground == null)
        {
            Debug.LogWarning("[ObjectSpawner] Ground object not found during LevelChange.");
            return;
        }

        Tilemap newTilemap = ground.GetComponent<Tilemap>();
        if (newTilemap == null)
        {
            Debug.LogWarning("[ObjectSpawner] Ground object has no Tilemap.");
            return;
        }

        tilemap = newTilemap;
        GatherValidPositions();
        DestroyAllSpawnedObjects();
    }

    private int ActiveObjectCount()
    {
        
        spawnObjects.RemoveAll(item => item == null);
        return spawnObjects.Count;
    }

    private IEnumerator SpawnObjectsIfNeeded()
    {
        isSpawning = true;
        while (ActiveObjectCount() < maxObjects)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
        isSpawning = false;
    }

    private void DestroyAllSpawnedObjects()
    {
        foreach (GameObject obj in spawnObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnObjects.Clear();
    }

    private bool PositionHasObject(Vector3 positionToCheck)
    {
        return spawnObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, positionToCheck) < 1.0f);
    }

    private ObjectType RandomObjectType()
    {
        float randomChoice = Random.value;
        if (randomChoice <= bigGemProbability)
        {
            return ObjectType.BigGem;
        }
        else
        {
            return ObjectType.SmallGem;
        }
    }

    private void SpawnObject()
    {
        if (validSpawnPos.Count == 0) return;

        Vector3 spawnPos = Vector3.zero;
        bool validPositionFound = false;

        while (!validPositionFound && validSpawnPos.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPos.Count);
            Vector3 potentialPosition = validSpawnPos[randomIndex];
            Vector3 leftPosition = potentialPosition + Vector3.left;
            Vector3 rightPosition = potentialPosition + Vector3.right;

            if (!PositionHasObject(leftPosition) && !PositionHasObject(rightPosition))
            {
                spawnPos = potentialPosition;
                validPositionFound = true;
            }
            validSpawnPos.RemoveAt(randomIndex);
        }

        if (validPositionFound)
        {
            ObjectType objectType = RandomObjectType();
            GameObject spawnedObj = Instantiate(objectPrefabs[(int)objectType], spawnPos, Quaternion.identity);

            
            spawnObjects.Add(spawnedObj);

            if (objectType == ObjectType.SmallGem || objectType == ObjectType.BigGem)
            {
                StartCoroutine(DestroyObjectAfterTime(spawnedObj, gemLifeTime));
            }
        }
    }

    private IEnumerator DestroyObjectAfterTime(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);

        if (gameObject)
        {
            spawnObjects.Remove(gameObject);
            validSpawnPos.Add(gameObject.transform.position);
            Destroy(gameObject);
        }
    }

    private void GatherValidPositions()
    {
        if (tilemap == null) return;

        validSpawnPos.Clear();
        BoundsInt boundsInt = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(boundsInt);
        Vector3 start = tilemap.CellToWorld(new Vector3Int(boundsInt.xMin, boundsInt.yMin, 0));

        for (int x = 0; x < boundsInt.size.x; x++)
        {
            for (int y = 0; y < boundsInt.size.y; y++)
            {
                TileBase tile = allTiles[x + y * boundsInt.size.x];
                if (tile != null)
                {
                    Vector3 place = start + new Vector3(x + 0.5f, y + 2f, 0);
                    validSpawnPos.Add(place);
                }
            }
        }
    }
}

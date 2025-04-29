using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { SmallGem, BigGem, Enemy }

    public Tilemap tilemap;
    public GameObject[] objectPrefabs; // 0=SmallGem, 1=BigGem, 2=Enemy
    public float bigGemProbability = 0.2f;
    public float smallGemProbability = 0.5f;
    public float enemyProbability = 0.1f;
    public int maxObjects = 5;
    public float gemLifeTime = 10f;
    public float spawnInterval = 0.5f;

    private List<Vector3> validSpawnPositions = new List<Vector3>();
    private List<GameObject> spawnObjects = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        GatherValidPositions();
        StartCoroutine(SpawnObjectsIfNeeded());
        GameController.OnReset += LevelChange; // Listen for resets
    }

    void Update()
    {
        if (!tilemap.gameObject.activeInHierarchy)
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
        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        DestroyAllSpawnedObjects();
        GatherValidPositions();
        StartCoroutine(SpawnObjectsIfNeeded());
    }

    private int ActiveObjectCount()
    {
        spawnObjects.RemoveAll(item => item == null); // Clean up destroyed objects
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

    private bool PositionHasObject(Vector3 positionToCheck)
    {
        return spawnObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, positionToCheck) < 1.0f);
    }

    private ObjectType RandomObjectType()
    {
        float total = smallGemProbability + bigGemProbability + enemyProbability;

        float smallGemChance = smallGemProbability / total;
        float bigGemChance = bigGemProbability / total;
        float enemyChance = enemyProbability / total;

        float randomChoice = Random.value; // Random between 0 and 1

        if (randomChoice < smallGemChance)
        {
            return ObjectType.SmallGem;
        }
        else if (randomChoice < smallGemChance + bigGemChance)
        {
            return ObjectType.BigGem;
        }
        else
        {
            return ObjectType.Enemy;
        }
    }

    private void SpawnObject()
    {
        if (validSpawnPositions.Count == 0) return;

        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;

        List<Vector3> tempPositions = new List<Vector3>(validSpawnPositions); // Copy to avoid modifying during checks

        while (!validPositionFound && tempPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, tempPositions.Count);
            Vector3 potentialPosition = tempPositions[randomIndex];
            Vector3 leftPosition = potentialPosition + Vector3.left;
            Vector3 rightPosition = potentialPosition + Vector3.right;

            if (!PositionHasObject(leftPosition) && !PositionHasObject(rightPosition))
            {
                spawnPosition = potentialPosition;
                validPositionFound = true;
            }

            tempPositions.RemoveAt(randomIndex); // Only temp list gets modified
        }

        if (validPositionFound)
        {
            ObjectType objectType = RandomObjectType();
            GameObject obj = Instantiate(objectPrefabs[(int)objectType], spawnPosition, Quaternion.identity);
            spawnObjects.Add(obj);

            validSpawnPositions.Remove(spawnPosition); // Only remove after successful spawn

            if (objectType != ObjectType.Enemy)
            {
                StartCoroutine(DestroyObjectAfterTime(obj, gemLifeTime));
            }
        }
    }

    private IEnumerator DestroyObjectAfterTime(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);

        if (gameObject)
        {
            spawnObjects.Remove(gameObject);
            validSpawnPositions.Add(gameObject.transform.position); // Free the position again
            Destroy(gameObject);
        }
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

    private void GatherValidPositions()
    {
        validSpawnPositions.Clear();
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
                    Vector3 place = start + new Vector3(x + 0.5f, y + 1.5f, 0);
                    validSpawnPositions.Add(place);
                }
            }
        }
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.Tilemaps;
//using UnityEngine.UI;

//public class ObjectSpawner : MonoBehaviour
//{
//    public enum ObjectType { SmallGem, BigGem, Enemy }

//    public Tilemap tilemap;
//    public GameObject[] objectPrefabs; //0=SmallGem, 1=BigGem, 2=Enemy
//    public float bigGemProbability = 0.2f; //chance to spawn big gem 20%
//    public float smallGemProbability = 0.5f; //chance to spawn small gem 50%
//    public float enemyProbability = 0.1f; //chance to spawn enemy 10%
//    public int maxObjects = 5;
//    public float gemLifeTime = 10f; //only for gems
//    public float spawnInterval = 0.5f;

//    private List<Vector3> validSpawnPositions = new List<Vector3>();
//    private List<GameObject> spawnObjects = new List<GameObject>();
//    private bool isSpawning = false;



//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        GatherValidPositions();
//        StartCoroutine(SpawnObjectsIfNeeded());
//        GameController.OnReset += LevelChange;  //this is called whenever we change levels but we can use it to respawn items after a restart as well
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (!tilemap.gameObject.activeInHierarchy)
//        {
//            LevelChange();
//        }
//        if(!isSpawning && ActiveObjectCount() < maxObjects)
//        {
//            StartCoroutine(SpawnObjectsIfNeeded());
//        }
//    }

//    private void LevelChange()
//    {
//        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
//        GatherValidPositions();
//        //destory all spawned objects
//        DestroyAllSpawnedObjects();
//        StartCoroutine (SpawnObjectsIfNeeded());    //immediately start spawning objects
//    }

//    private int ActiveObjectCount()
//    {
//        spawnObjects.RemoveAll(item => item == null); //remove any null objects
//        return spawnObjects.Count;
//    }

//    private IEnumerator SpawnObjectsIfNeeded()
//    {
//        isSpawning = true;
//        while (ActiveObjectCount() < maxObjects)
//        {
//            SpawnObject();
//            yield return new WaitForSeconds(spawnInterval);
//        }
//        isSpawning = false;
//    }

//    private bool PositionHasObject(Vector3 positionToCheck)
//    {
//        return spawnObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, positionToCheck) < 1.0f); //checks in direction needed if there is an object at cushion distance
//    }

//    private ObjectType RandomObjectType()
//    {
//        float randomChoice = Random.value;

//        if (randomChoice <= enemyProbability)
//        {
//            return ObjectType.Enemy;
//        }
//        else if (randomChoice <= (enemyProbability + bigGemProbability))
//        {
//            return ObjectType.BigGem;
//        }
//        else
//        {
//            return ObjectType.SmallGem;
//        }
//    }

//    private void SpawnObject()
//    {
//        if(validSpawnPositions.Count == 0) return;

//        Vector3 spawnPosition = Vector3.zero;
//        bool validPositionFound = false;

//        while(!validPositionFound && validSpawnPositions.Count > 0)
//        {
//            int randomIndex = Random.Range(0, validSpawnPositions.Count);   //get random position from valid spawn positions
//            Vector3 potentialPosition = validSpawnPositions[randomIndex];   //check to see if there are items to left or right
//            Vector3 leftPosition = potentialPosition + Vector3.left;
//            Vector3 rightPosition = potentialPosition + Vector3.right;

//            if (!PositionHasObject(leftPosition) && !PositionHasObject(rightPosition))
//            {
//                spawnPosition = potentialPosition;
//                validPositionFound = true;  //breaks the while loop
//            }

//            validSpawnPositions.RemoveAt(randomIndex); //removes it from list of spawn positions
//        }

//        if (validPositionFound)
//        {
//            ObjectType objectType = RandomObjectType();
//            GameObject gameObject = Instantiate(objectPrefabs[(int)objectType], spawnPosition, Quaternion.identity);    //gets object from array of types
//            spawnObjects.Add(gameObject);

//            //Destroy gems after time interval
//            if(objectType != ObjectType.Enemy)
//            {
//                StartCoroutine(DestroyObjectAfterTime(gameObject, gemLifeTime));
//            }
//        }
//    }

//    private IEnumerator DestroyObjectAfterTime(GameObject gameObject, float time)
//    {
//        yield return new WaitForSeconds(time);

//        if (gameObject)
//        {
//            spawnObjects.Remove(gameObject);
//            validSpawnPositions.Add(gameObject.transform.position);
//            Destroy(gameObject);
//        }
//    }

//    private void DestroyAllSpawnedObjects()
//    {
//        foreach(GameObject obj in spawnObjects)
//        {
//            if(obj != null)
//            {
//                Destroy(obj);
//            }
//        }
//        spawnObjects.Clear();
//    }

//    private void GatherValidPositions()
//    {
//        validSpawnPositions.Clear();
//        BoundsInt boundsInt = tilemap.cellBounds;   //getting the shape of the tile map
//        TileBase[] allTiles = tilemap.GetTilesBlock(boundsInt);
//        Vector3 start = tilemap.CellToWorld(new Vector3Int(boundsInt.xMin, boundsInt.yMin, 0));

//        //loop around tiles in the map and track valid spawn locations
//        for(int x = 0; x < boundsInt.size.x; x++)
//        {
//            for(int y=0; y<boundsInt.size.y; y++)
//            {
//                TileBase tile = allTiles[x + y * boundsInt.size.x];
//                if(tile != null)
//                {
//                    Vector3 place = start + new Vector3(x + 0.5f, y + 1.5f, 0);
//                    validSpawnPositions.Add(place);
//                }
//            }
//        }
//    }
//}

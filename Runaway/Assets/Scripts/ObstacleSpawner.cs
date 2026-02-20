using System.Collections;
using UnityEngine;

[System.Serializable]
public class PrefabInfo
{
    public GameObject prefab;
    public float fixPosY; // Y position relative to spawner
    public float rotationSpeed; // Speed of rotation for the prefab
    public Vector2 initialVelocity; // Initial velocity for the prefab
}

public class ObstacleSpawner : MonoBehaviour
{
    public PrefabInfo[] prefabs; // Array to hold prefab information
    public float spawnIntervalMin = 4f; // Minimum time between spawns
    public float spawnIntervalMax = 10f; // Maximum time between spawns

    private float nextSpawnTime; // Time when the next prefab will be spawned
    private int[] spawnIndices; // Array to hold indices of prefabs to spawn

    void Start()
    {
        // Calculate the time for the first spawn
        nextSpawnTime = Time.time + Random.Range(spawnIntervalMin, spawnIntervalMax);

        // Initialize the spawn indices array
        ResetSpawnIndices();
    }

    void Update()
    {
        // Check if it's time to spawn a new prefab
        if (Time.time >= nextSpawnTime)
        {
            // Spawn the selected prefab
            SpawnPrefab();

            // Calculate the time for the next spawn
            nextSpawnTime = Time.time + Random.Range(spawnIntervalMin, spawnIntervalMax);
        }
    }

    void SpawnPrefab()
    {
        if (spawnIndices.Length == 0)
        {
            // If all prefabs have been spawned once, reset the spawn indices
            ResetSpawnIndices();
        }

        // Randomly select an index from the spawn indices
        int randomIndex = Random.Range(0, spawnIndices.Length);
        int indexToSpawn = spawnIndices[randomIndex];

        // Spawn the selected prefab
        PrefabInfo prefabInfo = prefabs[indexToSpawn];
        Vector2 fixpos = new Vector2(transform.position.x, transform.position.y + prefabInfo.fixPosY);
        GameObject newPrefab = Instantiate(prefabInfo.prefab, fixpos, Quaternion.identity);

        // Set the parent to null to avoid being affected by the spawner's movement
        newPrefab.transform.parent = null;

        // Get the rigidbody2D component if exists
        Rigidbody2D rb = newPrefab.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Set the initial velocity
            rb.velocity = prefabInfo.initialVelocity;

            // Set the angular velocity to make it rotate
            rb.angularVelocity = prefabInfo.rotationSpeed;
        }

        // Remove the spawned prefab index from the spawn indices array
        RemoveSpawnIndex(randomIndex);
        
        // Start checking if the object is out of view every frame
        StartCoroutine(CheckOutOfView(newPrefab));
    }

    void ResetSpawnIndices()
    {
        // Create an array of indices representing each prefab
        spawnIndices = new int[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            spawnIndices[i] = i;
        }
    }

    void RemoveSpawnIndex(int indexToRemove)
    {
        // Remove the specified index from the spawn indices array
        if (indexToRemove >= 0 && indexToRemove < spawnIndices.Length)
        {
            int[] newSpawnIndices = new int[spawnIndices.Length - 1];
            for (int i = 0, j = 0; i < spawnIndices.Length; i++)
            {
                if (i != indexToRemove)
                {
                    newSpawnIndices[j] = spawnIndices[i];
                    j++;
                }
            }
            spawnIndices = newSpawnIndices;
        }
    }

    IEnumerator CheckOutOfView(GameObject obj)
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        bool outOfView = false;

        while (true)
        {
            // Check if the object is out of view
            Vector3 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
            if (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
            {
                // If the object is out of view, set the flag
                outOfView = true;
            }
            else
            {
                // If the object is back in view, reset the flag
                outOfView = false;
            }

            // If the object is out of view for 2 consecutive frames, despawn it
            if (outOfView)
            {
                yield return new WaitForSeconds(2f);
                Destroy(obj);
                yield break;
            }

            yield return null;
        }
    }
}

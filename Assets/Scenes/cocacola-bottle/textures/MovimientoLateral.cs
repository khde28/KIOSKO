using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs y posiciones")]
    public GameObject personaPrefab;       // Prefab de la persona (con animaciones)
    public Transform[] spawnPoints;        // Lista de puntos donde puede aparecer

    [Header("Control de tiempo")]
    public float spawnInterval = 5f;       // Cada cuánto tiempo se genera una persona
    public float lifeTime = 10f;           // Tiempo de vida del prefab antes de destruirse

    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (!isSpawning)
            {
                isSpawning = true;
                SpawnPerson();
                yield return new WaitForSeconds(spawnInterval);
                isSpawning = false;
            }

            yield return null;
        }
    }

    private void SpawnPerson()
    {
        if (personaPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No se ha asignado un prefab o spawn points en el inspector.");
            return;
        }

        // Elegir un punto aleatorio de spawn
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instanciar la persona
        GameObject newPerson = Instantiate(personaPrefab, spawnPoint.position, spawnPoint.rotation);

        // Destruir después de cierto tiempo
        Destroy(newPerson, lifeTime);

        Debug.Log($"Persona generada en {spawnPoint.name}, se destruirá en {lifeTime} segundos.");
    }
}
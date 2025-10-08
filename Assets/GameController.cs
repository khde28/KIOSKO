using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Cliente")]
    public GameObject clientePrefab;
    public Transform[] spawnPoints;
    public float respawnDelay = 2f;

    [Header("Productos disponibles")]
    public string[] availableProducts = { "manzana", "coca" };

    [Header("Economía")]
    private int money = 0;

    [Header("Gestión de clientes")]
    private int angryClients = 0;
    public int maxAngryClients = 3;
    private int satisfiedClients = 0; // CONTADOR DE CLIENTES SATISFECHOS
    public int clientsToWin = 5; // META PARA GANAR

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip campanaClip;
    public AudioClip clienteFelizClip;
    public AudioClip clienteEnojadoClip;

    private bool isSpawning = false;
    private bool IsWin = false;
    private bool IsDead = false;
    private int vidas = 3;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        PlaySound(campanaClip);
        // Generar primer cliente inmediatamente
        StartCoroutine(SpawnNewClient());
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Client") == null && !isSpawning)
        {
            StartCoroutine(SpawnNewClient());
        }

        // Verificar victoria por clientes atendidos
        if (!IsWin && satisfiedClients >= clientsToWin)
        {
            WinGame();
        }

        // Tu condición original de victoria (por si la quieres mantener)
        if (!IsWin && transform.position.y < -10f)
        {
            UIManager.inst.ShowWinScreen();
            IsWin = true;
        }
    }

    private IEnumerator SpawnNewClient()
    {
        isSpawning = true;
        yield return new WaitForSeconds(respawnDelay);

        Transform chosenSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newClient = Instantiate(clientePrefab, chosenSpawn.position, chosenSpawn.rotation);
        newClient.tag = "Client";

        // Configurar productos aleatorios para el cliente
        ConfigureRandomProducts(newClient);

        Debug.Log("Nuevo cliente generado con productos aleatorios.");
        isSpawning = false;
    }

    private void ConfigureRandomProducts(GameObject clientObject)
    {
        Cliente cliente = clientObject.GetComponent<Cliente>();
        if (cliente != null)
        {
            // Limpiar lista existente
            cliente.requestedProductsList.Clear();

            // Elegir 1 producto aleatorio
            string randomProduct = availableProducts[Random.Range(0, availableProducts.Length)];

            // Agregar el producto con cantidad 1
            ProductRequest newRequest = new ProductRequest
            {
                productName = randomProduct,
                quantity = 1
            };

            cliente.requestedProductsList.Add(newRequest);

            Debug.Log($"Cliente nuevo pide: {randomProduct} (cantidad: 1)");
        }
    }

    // ==========================
    //  MÉTODO NUEVO: Contar cliente satisfecho
    // ==========================
    public void AddSatisfiedClient()
    {
        satisfiedClients++;
        Debug.Log($"Clientes satisfechos: {satisfiedClients}/{clientsToWin}");

        // Verificar si ganó inmediatamente después de agregar
        if (!IsWin && satisfiedClients >= clientsToWin)
        {
            WinGame();
        }
    }

    // ==========================
    //  MÉTODO NUEVO: Ganar el juego
    // ==========================
    private void WinGame()
    {
        if (IsWin) return;

        IsWin = true;
        Debug.Log("¡FELICIDADES! Has atendido a 5 clientes satisfactoriamente. ¡GANASTE!");

        PlaySound(campanaClip); // Reproducir sonido de victoria

        UIManager.inst.ShowWinScreen();
        Time.timeScale = 0f; // Pausar el juego
    }

    // ==========================
    //  Economía
    // ==========================
    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log($"Dinero actual: {money}");
    }

    public int GetMoney() => money;

    // ==========================
    //  Clientes molestos
    // ==========================
    public void AddAngryClient()
    {
        angryClients++;
        Debug.Log($"Clientes molestos: {angryClients}/{maxAngryClients}");

        if (vidas > 0 && !IsDead)
        {
            vidas -= 1;
            Debug.Log($"Vidas restantes: {vidas}");
            UIManager.inst.DesactivarVida(vidas);
        }

        if (angryClients >= maxAngryClients)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        PlaySound(campanaClip);
        Debug.Log("¡Demasiados clientes molestos! Juego terminado.");

        if (IsDead) return;

        IsDead = true;

        UIManager.inst.ShowLoseScreen();
        Time.timeScale = 0f;
    }

    // ==========================
    //  Audio
    // ==========================
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayHappyClientSound()
    {
        PlaySound(clienteFelizClip);
    }

    public void PlayAngryClientSound()
    {
        PlaySound(clienteEnojadoClip);
    }
}
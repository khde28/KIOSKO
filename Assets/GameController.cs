using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Cliente")]
    public GameObject clientePrefab;
    public Transform[] spawnPoints; // tres posibles posiciones
    public float respawnDelay = 2f;

    [Header("Econom a")]
    private int money = 0;

    [Header("Gesti n de clientes")]
    private int angryClients = 0;
    public int maxAngryClients = 3;

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
        // reproducir sonido inicial del juego
        PlaySound(campanaClip);
    }

    void Update()
    {
        // si no hay cliente, iniciar respawn
        if (GameObject.FindGameObjectWithTag("Client") == null && !isSpawning)
        {
            StartCoroutine(SpawnNewClient());
        }

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

        // elegir punto aleatorio
        Transform chosenSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject newClient = Instantiate(clientePrefab, chosenSpawn.position, chosenSpawn.rotation);
        newClient.tag = "Client";

        Debug.Log("Nuevo cliente generado en posici n aleatoria.");
        isSpawning = false;
    }

    // ==========================
    //  Econom a
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
        PlaySound(campanaClip); // mismo sonido del inicio
        Debug.Log(" Demasiados clientes molestos! Juego terminado.");

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
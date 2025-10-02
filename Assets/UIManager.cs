using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager inst;

    [Header("UI Gameplay")]
    public TextMeshProUGUI TimeCounterGameplay;

    private float TimeSeconds;
    private int TimeMinutes;

    [Header("Win Screen")]
    public GameObject WinScreen; // Asigna en el inspector
    private bool hasShownWin = false; // Para que solo aparezca una vez

    void Awake()
    {
        inst = this;
    }

    void Update()
    {
        // Actualización del cronómetro
        TimeSeconds += Time.deltaTime;
        if (TimeSeconds >= 60f)
        {
            TimeMinutes++;
            TimeSeconds = 0f;
        }

        // Formato mm:ss con padding
        string minutos = TimeMinutes.ToString("00");
        string segundos = Mathf.FloorToInt(TimeSeconds).ToString("00");
        TimeCounterGameplay.text = $"Tiempo: {minutos}:{segundos}";
    }

    public void ShowWinScreen()
    {
        if (hasShownWin) return; // evitar múltiples llamadas
        WinScreen.SetActive(true);
        hasShownWin = true;
        Debug.Log("Pantalla de victoria mostrada!");
    }
}
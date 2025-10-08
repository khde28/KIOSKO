using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager inst;

    [Header("UI Gameplay")]
    public TextMeshProUGUI TimeCounterGameplay;

    private float TimeSeconds;
    private int TimeMinutes;
    private bool isGamePaused = false; // Nuevo: controlar si el juego está pausado

    [Header("Win Screen")]
    public GameObject WinScreen;
    private bool hasShownWin = false;

    [Header("Lose Screen")]
    public GameObject LoseScreen;
    private bool hasShownLose = false;

    [Header("Vidas")]
    public GameObject[] vidas;

    void Awake()
    {
        inst = this;
    }

    void Update()
    {
        // Si el juego está pausado, no actualizar el tiempo
        if (isGamePaused) return;

        TimeSeconds += Time.deltaTime;
        if (TimeSeconds >= 60f)
        {
            TimeMinutes++;
            TimeSeconds = 0f;
        }

        string minutos = TimeMinutes.ToString("00");
        string segundos = Mathf.FloorToInt(TimeSeconds).ToString("00");
        TimeCounterGameplay.text = $"Tiempo: {minutos}:{segundos}";
    }

    public void ShowWinScreen()
    {
        if (hasShownWin) return;

        WinScreen.SetActive(true);
        hasShownWin = true;
        PausarJuego(); // Pausar el juego al ganar
        Debug.Log("Pantalla de victoria mostrada! Juego pausado.");
    }

    public void ShowLoseScreen()
    {
        if (hasShownLose) return;

        LoseScreen.SetActive(true);
        hasShownLose = true;
        PausarJuego(); // Pausar el juego al perder
        Debug.Log("Pantalla de derrota mostrada! Juego pausado.");
    }

    // Nuevo método para pausar el juego
    private void PausarJuego()
    {
        isGamePaused = true;
        Time.timeScale = 0f; // Esto detiene toda la física y updates
    }

    // Nuevo método para reanudar el juego
    public void ReanudarJuego()
    {
        isGamePaused = false;
        Time.timeScale = 1f; // Esto reanuda el juego a velocidad normal
    }

    // Método para reiniciar el juego (para botones)
    public void ReiniciarJuego()
    {
        ReanudarJuego(); // Asegurar que el tiempo vuelva a la normalidad
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    // Método para volver al menú principal
    public void VolverAlMenu()
    {
        ReanudarJuego(); // Asegurar que el tiempo vuelva a la normalidad
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal");
    }

    public void DesactivarVida(int indice)
    {
        if (indice >= 0 && indice < vidas.Length)
        {
            vidas[indice].SetActive(false);
        }
    }

    public void ActivarVida(int indice)
    {
        if (indice >= 0 && indice < vidas.Length)
        {
            vidas[indice].SetActive(true);
        }
    }
}
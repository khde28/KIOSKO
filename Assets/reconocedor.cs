using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;

public class MoveCubeAndPlayAudio : MonoBehaviour
{
    public GameObject cube;               // Cubo a mover
    public float moveDistance = 1f;       // Distancia a mover
    public AudioClip audioArriba;         // Audio para "arriba"
    public AudioClip audioAbajo;          // Audio para "abajo"
    public AudioClip audioDesea;          // Audio para "que desea"
    public AudioClip audioLlevar;         // Audio para "que va a llevar"
    public AudioClip audioDoy;            // Audio para "que le doy"

    private DictationRecognizer dictationRecognizer;
    private AudioSource audioSource;

    private Dictionary<string, Vector3> commands = new Dictionary<string, Vector3>();

    void Start()
    {
        if (cube == null)
        {
            Debug.LogWarning("No se asign� ning�n cubo!");
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();

        // Frases y direcciones
        commands.Add("que desea", Vector3.up);
        commands.Add("qué desea", Vector3.up); // Esta línea está repetida, puedes eliminarla si no es intencional
        commands.Add("que va a llevar", Vector3.right);
        commands.Add("qué va a llevar", Vector3.right); // También repetida
        commands.Add("que le doy", Vector3.down);
        commands.Add("qué le doy", Vector3.down); // Repetida
        commands.Add("arriba", Vector3.up);
        commands.Add("abajo", Vector3.down);

        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += OnDictationResult;
        dictationRecognizer.DictationComplete += (cause) => dictationRecognizer.Start();
        dictationRecognizer.DictationError += (error, hresult) => Debug.LogError("Dictation error: " + error);

        dictationRecognizer.Start();
        Debug.Log("DictationRecognizer iniciado. Di tus frases para mover cubo y reproducir audio.");
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        string t = text.ToLower();
        Debug.Log("Texto detectado: '" + t + "'");

        foreach (var cmd in commands)
        {
            if (t.Contains(cmd.Key))
            {
                // Mover cubo
                cube.transform.Translate(cmd.Value * moveDistance);
                Debug.Log("Moviendo cubo por frase: '" + cmd.Key + "'");

                // Reproducir audio seg�n frase
                switch (cmd.Key)
                {
                    case "arriba":
                        audioSource.PlayOneShot(audioArriba);
                        break;
                    case "abajo":
                        audioSource.PlayOneShot(audioAbajo);
                        break;
                    case "qu� desea":
                        audioSource.PlayOneShot(audioDesea);
                        break;
                    case "qu� va a llevar":
                        audioSource.PlayOneShot(audioLlevar);
                        break;
                    case "qu� le doy":
                        audioSource.PlayOneShot(audioDoy);
                        break;
                }
            }
        }

        // Reiniciar para seguir escuchando
        if (dictationRecognizer.Status != SpeechSystemStatus.Running)
            dictationRecognizer.Start();
    }

    private void OnDestroy()
    {
        if (dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running)
            dictationRecognizer.Stop();
    }
}
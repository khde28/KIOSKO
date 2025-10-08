using UnityEngine;

public class TestMic : MonoBehaviour
{
    void Start()
    {
        foreach (var device in Microphone.devices)
        {
            Debug.Log("🎤 Micrófono detectado: " + device);
        }
    }
}

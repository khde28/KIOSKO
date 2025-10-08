using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    [Header("Configuración Oculus")]
    public Transform playerHead;
    public float distance = 2f;
    public float heightOffset = 0f;

    [Header("Opciones de seguimiento")]
    public bool followPlayer = true;
    public bool alwaysLookAtPlayer = true;

    void Start()
    {
        // Buscar automáticamente la cabeza del jugador si no está asignada
        if (playerHead == null)
        {
            GameObject cameraRig = GameObject.Find("OVRCameraRig");
            if (cameraRig != null)
            {
                playerHead = cameraRig.transform.Find("TrackingSpace/CenterEyeAnchor");
            }

            // Si no encuentra ese path, busca alternativas
            if (playerHead == null)
            {
                playerHead = Camera.main.transform;
            }
        }
    }

    void Update()
    {
        if (followPlayer && playerHead != null)
        {
            // Calcular posición frente al jugador
            Vector3 targetPosition = playerHead.position +
                                   playerHead.forward * distance +
                                   Vector3.up * heightOffset;

            // Suavizar el movimiento (opcional)
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);

            // Hacer que el UI mire hacia el jugador
            if (alwaysLookAtPlayer)
            {
                transform.LookAt(playerHead);
                transform.Rotate(0, 180, 0); // Rotar para que el texto no esté al revés
            }
        }
    }

    // Método público para cambiar la distancia
    public void SetDistance(float newDistance)
    {
        distance = newDistance;
    }
}
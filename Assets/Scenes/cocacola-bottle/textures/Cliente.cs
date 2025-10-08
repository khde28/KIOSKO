using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class ProductRequest
{
    public string productName;
    public int quantity;
}

public class Cliente : MonoBehaviour
{
    public List<ProductRequest> requestedProductsList = new List<ProductRequest>();
    public int wrongLimit = 2;
    private Dictionary<string, int> requestedProducts = new Dictionary<string, int>();

    private int wrongProductsCount = 0;
    private int totalEarned = 0;
    private bool isLeaving = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] productRequestClips; // Audios para los productos pedidos

    void Start()
    {
        // Inicializar productos solicitados
        foreach (var item in requestedProductsList)
        {
            if (!requestedProducts.ContainsKey(item.productName))
                requestedProducts[item.productName] = item.quantity;
            else
                requestedProducts[item.productName] += item.quantity;
        }

        // Mostrar en consola lo que pidió
        Debug.Log("Cliente pidió:");
        foreach (var kvp in requestedProducts)
            Debug.Log($"- {kvp.Key}: {kvp.Value}");

        // Reproducir audios después de 1 segundo
        StartCoroutine(PlayRequestAudio());
    }

    private IEnumerator PlayRequestAudio()
    {
        // Esperar 1 segundo después de spawnear
        yield return new WaitForSeconds(1f);

        // Reproducir audio para cada producto solicitado
        foreach (var kvp in requestedProducts)
        {
            PlayProductAudio(kvp.Key);
            // Esperar un poco entre audios si hay múltiples productos
            yield return new WaitForSeconds(1.5f);
        }
    }

    private void PlayProductAudio(string productName)
    {
        // Buscar el clip de audio correspondiente al producto
        AudioClip clipToPlay = GetAudioClipForProduct(productName);
        if (clipToPlay != null && audioSource != null)
        {
            audioSource.PlayOneShot(clipToPlay);
            Debug.Log($"Reproduciendo audio para: {productName}");
        }
    }

    private AudioClip GetAudioClipForProduct(string productName)
    {
        // Aquí debes asignar los audios en el inspector
        // Puedes hacer un switch o buscar por nombre
        foreach (AudioClip clip in productRequestClips)
        {
            if (clip.name.ToLower().Contains(productName.ToLower()))
            {
                return clip;
            }
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLeaving) return;

        Product product = other.GetComponent<Product>();
        if (product == null) return;

        string name = product.productName;
        int price = product.price;

        // Producto correcto
        if (requestedProducts.ContainsKey(name))
        {
            totalEarned += price;

            Debug.Log($"Cliente recibió {name}. +{price} soles");

            requestedProducts[name]--;
            if (requestedProducts[name] <= 0)
                requestedProducts.Remove(name);
        }
        else
        {
            wrongProductsCount++;
            Debug.Log($"Producto equivocado: {name}. Error #{wrongProductsCount})");

            if (wrongProductsCount > wrongLimit)
            {
                Destroy(other.gameObject, 0.1f);
                LeaveAngry();
                return;
            }
        }

        Destroy(other.gameObject, 0.1f);

        // Si ya completó todos los pedidos
        if (requestedProducts.Count == 0)
        {
            Debug.Log("¡Cliente satisfecho y se retira.");
            LeaveHappy();
        }
    }

    private void LeaveHappy()
    {
        if (isLeaving) return;
        isLeaving = true;

        GameManager.Instance.AddMoney(totalEarned);
        GameManager.Instance.PlayHappyClientSound();

        // NOTIFICAR AL GAME MANAGER QUE UN CLIENTE FUE SATISFECHO
        GameManager.Instance.AddSatisfiedClient();

        Destroy(gameObject, 2f);
    }

    private void LeaveAngry()
    {
        if (isLeaving) return;
        isLeaving = true;

        Debug.Log("Cliente molesto: se va sin pagar, pierdes el dinero ganado con él.");

        GameManager.Instance.AddMoney(-totalEarned);
        GameManager.Instance.AddAngryClient();
        GameManager.Instance.PlayAngryClientSound();

        Destroy(gameObject, 2f);
    }
}
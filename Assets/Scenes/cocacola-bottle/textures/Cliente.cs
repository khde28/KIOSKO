using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductRequest
{
    public string productName;
    public int quantity;
}

public class Cliente : MonoBehaviour
{
    public List<ProductRequest> requestedProductsList = new List<ProductRequest>();
    public int wrongLimit = 2; // L mite de productos incorrectos antes de que el cliente se vaya molesto
    private Dictionary<string, int> requestedProducts = new Dictionary<string, int>();

    private int wrongProductsCount = 0;  // Productos incorrectos dados
    private int totalEarned = 0;         // Dinero ganado con este cliente
    private bool isLeaving = false;      // Evita l gica doble

    void Start()
    {
        foreach (var item in requestedProductsList)
        {
            if (!requestedProducts.ContainsKey(item.productName))
                requestedProducts[item.productName] = item.quantity;
            else
                requestedProducts[item.productName] += item.quantity;
        }

        Debug.Log("Cliente pidi :");
        foreach (var kvp in requestedProducts)
            Debug.Log($"- {kvp.Key}: {kvp.Value}");
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

            Debug.Log($"Cliente recibi  {name}. +{price} soles");

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
                LeaveAngry();
                return;
            }
        }

        Destroy(other.gameObject, 0.1f);

        // Si ya complet  todos los pedidos
        if (requestedProducts.Count == 0)
        {
            Debug.Log(" Cliente satisfecho y se retira.");
            LeaveHappy();
        }
    }

    private void LeaveHappy()
    {
        if (isLeaving) return;
        isLeaving = true;

        GameManager.Instance.AddMoney(totalEarned);
        GameManager.Instance.PlayHappyClientSound();
        Destroy(gameObject, 2f);
    }

    private void LeaveAngry()
    {
        if (isLeaving) return;
        isLeaving = true;

        Debug.Log("Cliente molesto: se va sin pagar, pierdes el dinero ganado con  l.");

        GameManager.Instance.AddMoney(-totalEarned);
        GameManager.Instance.AddAngryClient();
        GameManager.Instance.PlayAngryClientSound();

        Destroy(gameObject, 2f);
    }
}
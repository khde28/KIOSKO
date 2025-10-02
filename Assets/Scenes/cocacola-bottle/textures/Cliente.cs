using UnityEngine;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    public List<string> requestedProducts = new List<string>();
    private List<string> remainingProducts = new List<string>();
    private bool IsWin = false;


    void Start()
    {
        remainingProducts = new List<string>(requestedProducts);
    }

    private void OnTriggerEnter(Collider other)
    {
        Product product = other.GetComponent<Product>();

        if (product != null)
        {
            if (remainingProducts.Contains(product.productName))
            {
                GameManager.Instance.AddMoney(product.price);
                Debug.Log("Cliente recibió " + product.productName + ". +" + product.price + " soles");

                remainingProducts.Remove(product.productName);
            }
            else
            {
                GameManager.Instance.AddMoney(-product.price);
                Debug.Log("Producto equivocado: " + product.productName + ". -" + product.price + " soles");
            }

            Destroy(other.gameObject);
        }

        if (remainingProducts.Count == 0)
        {
            Debug.Log("Cliente satisfecho y se retira");
            Destroy(gameObject, 2f);
            if (!IsWin)
            {
                UIManager.inst.ShowWinScreen();
                IsWin = true;
            }
        }
    }
}
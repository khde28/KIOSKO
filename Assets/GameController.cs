using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton para acceder fácil
    private int money = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log("Dinero añadido: " + amount);
        Debug.Log("Dinero actual: " + money);
    }
    public int GetMoney()
    {
        return money;
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            Debug.Log("Dinero gastado: " + amount);
            Debug.Log("Dinero restante: " + money);
            return true;
        }
        else
        {
            Debug.Log("No hay suficiente dinero.");
            return false;
        }
    }
}
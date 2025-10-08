using UnityEngine;

public class ClientAnimationController : MonoBehaviour
{
    [Header("Componentes")]
    public Animator animator;

    [Header("Animaciones")]
    public AnimationClip clipIdle;
    public AnimationClip clipCaminar;
    public AnimationClip clipMolesto;
    public AnimationClip clipSatisfecho;

    [Header("Tiempos")]
    public float tiempoAntesDeMolesto = 30f;        // 🔹 Espera antes de enojarse si no lo atienden
    public float tiempoMolestoAntesDeSalir = 3f;    // 🔹 Tiempo que permanece molesto antes de irse

    [Header("Movimiento")]
    public float velocidadMovimiento = 2f;
    public float velocidadRotacion = 90f;
    public Transform puntoEntrada; // Donde aparece
    public Transform puntoSalida;  // Hacia donde se irá

    // Estados
    private bool estaEsperando = false;
    private bool estaCaminando = false;
    private bool estaMolesto = false;
    private bool seEstaDandoVuelta = false;
    private bool seEstaYendo = false;
    private bool clienteSatisfecho = false;

    private float temporizador = 0f;
    private float anguloRotado = 0f;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        IniciarCaminar(); // Comienza caminando al mostrador
    }

    void Update()
    {
        if (estaCaminando)
        {
            transform.Translate(Vector3.forward * velocidadMovimiento * Time.deltaTime);
        }

        if (estaEsperando && !clienteSatisfecho && !estaMolesto)
        {
            temporizador += Time.deltaTime;

            // 🔹 Si pasa demasiado tiempo sin ser atendido, se enoja
            if (temporizador >= tiempoAntesDeMolesto)
            {
                IniciarMolesto();
            }
        }

        if (seEstaDandoVuelta)
        {
            float rotacion = velocidadRotacion * Time.deltaTime;
            transform.Rotate(Vector3.up, rotacion);
            anguloRotado += rotacion;

            if (anguloRotado >= 180f)
            {
                seEstaDandoVuelta = false;
                IniciarSalida();
            }
        }

        if (seEstaYendo)
        {
            transform.Translate(Vector3.forward * velocidadMovimiento * Time.deltaTime);
        }
    }

    // 🔹 Camina hasta el mostrador
    public void IniciarCaminar()
    {
        animator.Play(clipCaminar.name);
        estaCaminando = true;
        estaEsperando = false;
        estaMolesto = false;
        seEstaYendo = false;
        clienteSatisfecho = false;

        // Simula que llega al mostrador en unos segundos
        Invoke(nameof(IniciarEsperar), 2f);
    }

    // 🔹 Espera su pedido
    public void IniciarEsperar()
    {
        estaCaminando = false;
        estaEsperando = true;
        animator.Play(clipIdle.name);
        temporizador = 0f;
        Debug.Log("Cliente esperando su pedido...");
    }

    // 🔹 Se enoja (por demora o producto incorrecto)
    public void IniciarMolesto()
    {
        if (clienteSatisfecho || estaMolesto) return;

        estaEsperando = false;
        estaMolesto = true;
        animator.Play(clipMolesto.name);
        Debug.Log("Cliente molesto. Se irá pronto.");

        Invoke(nameof(IniciarDarseVuelta), tiempoMolestoAntesDeSalir);


        GameManager.Instance.AddAngryClient();

    }

    // 🔹 Se gira para salir
    public void IniciarDarseVuelta()
    {
        seEstaDandoVuelta = true;
        anguloRotado = 0f;
    }

    // 🔹 Se va caminando del lugar
    public void IniciarSalida()
    {
        seEstaYendo = true;
        estaMolesto = false;
        animator.Play(clipCaminar.name);
        Debug.Log("Cliente se va del local.");
        Destroy(gameObject, 5f);
    }

    // 🔹 Llamado por Client.cs si le dieron un producto equivocado
    public void ForzarEnojoYSalida()
    {
        if (clienteSatisfecho) return;

        estaCaminando = false;
        estaEsperando = false;
        estaMolesto = true;
        seEstaDandoVuelta = false;
        seEstaYendo = false;
        temporizador = 0f;
        anguloRotado = 0f;

        animator.Play(clipMolesto.name);
        Debug.Log("Cliente recibió un producto equivocado. Se enoja y se irá.");

        Invoke(nameof(IniciarDarseVuelta), tiempoMolestoAntesDeSalir);
    }

    // 🔹 Llamado por Client.cs cuando recibe todos los productos correctos
    public void ClienteSatisfecho()
    {
        if (estaMolesto) return;

        clienteSatisfecho = true;
        estaCaminando = false;
        estaEsperando = false;
        animator.Play(clipSatisfecho.name);
        Debug.Log("Cliente satisfecho. Se despide feliz.");
    }

    // 🔹 Saber si el cliente está enojado o yéndose
    public bool EstaMolesto()
    {
        return estaMolesto || seEstaYendo;
    }
}

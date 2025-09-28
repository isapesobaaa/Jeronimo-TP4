using UnityEngine;
using UnityEngine.AI;

public class AgentScript : MonoBehaviour
{
    [SerializeField] private Animator animador;
    [SerializeField] private Transform origenVista;

    [SerializeField] private Transform[] puntosPatrulla;

    [SerializeField] private float distanciaVista = 6f;
    [SerializeField] private float anguloVista = 40f;
    [SerializeField] private int cantidadRayos = 4;
    [SerializeField] private float tiempoPerderVista = 2f;

    private NavMeshAgent agente;
    private int indicePatrulla;
    private Transform destinoActual;
    private Transform jugadorDetectado;
    private bool persiguiendo;
    private float temporizadorPerdido;

    private void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
        animador = GetComponent<Animator>();
        animador.applyRootMotion = false;

        if (puntosPatrulla != null && puntosPatrulla.Length > 0)
        {
            indicePatrulla = 0;
            destinoActual = puntosPatrulla[indicePatrulla];
        }
    }

    private void Update()
    {
        if (!persiguiendo)
            Patrullar();

        BuscarJugador();
        ActualizarPersecucion();
        
        animador.SetFloat("Velocidad", agente.velocity.magnitude);

    }

    private void Patrullar()
    {
        if (destinoActual == null) return;

        agente.SetDestination(destinoActual.position);
        //llego al punto
        if (Vector3.Distance(transform.position, destinoActual.position) <= 0.8f)
        {
            indicePatrulla += 1;
            destinoActual = puntosPatrulla[indicePatrulla];
        }
    }

    private void ActualizarPersecucion()
    {
        if (!persiguiendo || jugadorDetectado == null) return;

        agente.SetDestination(jugadorDetectado.position);
        temporizadorPerdido += Time.deltaTime;

        if (temporizadorPerdido >= tiempoPerderVista)
        {
            //reiniciamos patrulla
            persiguiendo = false;
            indicePatrulla = Random.Range(0, 4);
            destinoActual = puntosPatrulla[indicePatrulla];
            agente.SetDestination(destinoActual.position);
        }
    }

    private void BuscarJugador()
    {
        if (origenVista == null) return;

        float medioAngulo = anguloVista * 0.5f;

        // por cada rayo, calcular angulo y chequaer si golpio al jugador (chagtp)
        for (int i = 0; i < cantidadRayos; i++)
        {
            float progreso = cantidadRayos > 1 ? (float)i / (cantidadRayos - 1) : 0.5f;
            float anguloActual = Mathf.Lerp(-medioAngulo, medioAngulo, progreso);

            Vector3 direccion = Quaternion.Euler(0, anguloActual, 0) * origenVista.forward;

            if (Physics.Raycast(origenVista.position, direccion, out RaycastHit golpe, distanciaVista))
            {
                if (golpe.collider.CompareTag("Player"))
                {
                    jugadorDetectado = golpe.collider.transform;
                    temporizadorPerdido = 0f;
                    persiguiendo = true;
                }
            }
        }
    }
    
    

    private void OnDrawGizmosSelected()
    {
        //chatgpt
        if (origenVista == null) return;

        Gizmos.color = Color.cyan;
        float medioAngulo = anguloVista * 0.5f;

        for (int i = 0; i < cantidadRayos; i++)
        {
            float progreso = cantidadRayos > 1 ? (float)i / (cantidadRayos - 1) : 0.5f;
            float anguloActual = Mathf.Lerp(-medioAngulo, medioAngulo, progreso);

            Vector3 direccion = Quaternion.Euler(0, anguloActual, 0) * origenVista.forward;
            Gizmos.DrawRay(origenVista.position, direccion * distanciaVista);
        }
    }
}

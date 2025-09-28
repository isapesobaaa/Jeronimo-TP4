using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Perder : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Text mensajeDerrota;
    private Collider detector;

    private void Awake()
    {
        detector = GetComponent<Collider>();
        if (mensajeDerrota != null)
            mensajeDerrota.enabled = false;

    }

    private void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("Agent"))
        {
            mensajeDerrota.enabled = true;
            StartCoroutine(ReiniciarEscenaTrasEspera(2f));
        }
    }

    private IEnumerator ReiniciarEscenaTrasEspera(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }
}
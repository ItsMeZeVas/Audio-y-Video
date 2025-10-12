using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Decisiones : MonoBehaviour
{
    [Header("Configuración del video")]
    public VideoPlayer videoPlayer;      // Reproductor de video asignado al Raw Image
    public float decisionTime;           // Segundo exacto en el que se mostrará la decisión

    [Header("Interfaz de decisión")]
    public GameObject decisionPanel;     // Panel que contiene los botones de elección
    public Button opcion1Button;         // Botón para la primera opción
    public Button opcion2Button;         // Botón para la segunda opción
    public Text opcion1Text;             // Texto visible en el botón 1
    public Text opcion2Text;             // Texto visible en el botón 2

    [Header("Configuración de opciones")]
    public string opcion1Texto;          // Texto descriptivo de la primera opción
    public string opcion2Texto;          // Texto descriptivo de la segunda opción
    public string siguienteEscena1;      // Escena a la que se dirigirá al elegir la primera opción
    public string siguienteEscena2;      // Escena a la que se dirigirá al elegir la segunda opción

    private bool decisionMostrada = false;

    // Start se ejecuta al inicio de la escena
    void Start()
    {
        // Oculta el panel de decisión hasta que llegue el momento indicado
        if (decisionPanel != null)
            decisionPanel.SetActive(false);

        // Inicia la reproducción del video asignado
        if (videoPlayer != null)
            videoPlayer.Play();

        // Configura los textos y funciones de los botones
        if (opcion1Text != null) opcion1Text.text = opcion1Texto;
        if (opcion2Text != null) opcion2Text.text = opcion2Texto;

        if (opcion1Button != null) opcion1Button.onClick.AddListener(() => CargarEscena(siguienteEscena1));
        if (opcion2Button != null) opcion2Button.onClick.AddListener(() => CargarEscena(siguienteEscena2));
    }

    // Update se ejecuta una vez por frame
    void Update()
    {
        // Controla el tiempo actual del video y muestra la decisión en el momento indicado
        if (videoPlayer != null && videoPlayer.isPlaying && !decisionMostrada)
        {
            if (videoPlayer.time >= decisionTime)
            {
                MostrarOpciones();
            }
        }
    }

    // Pausa el video y muestra las opciones en pantalla
    void MostrarOpciones()
    {
        decisionMostrada = true;
        videoPlayer.Pause();
        decisionPanel.SetActive(true);
    }

    // Carga la escena correspondiente según la elección del usuario
    void CargarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}


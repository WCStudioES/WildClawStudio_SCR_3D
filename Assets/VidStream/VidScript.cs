using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VidScript : MonoBehaviour
{
    //URL DEL VIDEO EN UN PAGINA WEB HOSTEADA POR GITHUB
    [SerializeField] private string videoURL = "https://tasiatas.github.io/creditosSCR3D/CreditosSCR3D_FINAL.mp4";
    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer)
        {
            videoPlayer.url = videoURL;
            videoPlayer.playOnAwake = false; //NOS ASEGURAMOS DE NO PONER EL VIDEO AL EMPEZAR EL JUEGO
        }
    }

    void OnEnable()
    {
        //LLAMADA AL METODO CUANDO ENTRAMOS EN LA ESCENA
        if (videoPlayer)
        {
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += onVideoPrepared;
        }
    }

    void OnDisable()
    {
        //DESUSCRIBIMOS EL OBJETO AL SALIR DE LA ESCENA
        if (videoPlayer)
        {
            videoPlayer.prepareCompleted -= onVideoPrepared;
            videoPlayer.Stop(); //PARAMOS EL VIDEO PARA RESETEARLO
        }
    }

    private void onVideoPrepared(VideoPlayer source)
    {
        if (videoPlayer)
        {
            videoPlayer.Play(); //SE PONE EL VIDEO UNA VEZ ESTA LISTO
        }
    }
}

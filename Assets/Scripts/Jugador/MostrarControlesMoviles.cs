using UnityEngine;

public class MostrarControlesMoviles : MonoBehaviour
{
    private bool esMovil = false;
    [SerializeField] private GameObject ControlesTactiles;

    void Start()
    {
        esMovil = Application.isMobilePlatform && Application.platform == RuntimePlatform.WebGLPlayer;
        if(esMovil)
            ControlesTactiles.SetActive(true);
    }
    
}

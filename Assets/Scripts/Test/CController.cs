using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CController : MonoBehaviour
{
    
    private CharacterController controller;
    private Vector3 direccion = new Vector3(0f,0f, 0f);
    [SerializeField] private Transform CameraPosition;

    public float speed = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        controller.Move( speed * Time.deltaTime * direccion);
    }

    public void Move(Vector2 input)
    {
        direccion = new Vector3(input.x, 0f, input.y);
    }

    public void SetToSpawn()
    {
        this.transform.position = GameObject.Find("PuntoDeSpawn").transform.position;
    }
    
    public void AssignMainCamera()
    {
        Debug.Log("Busca la camara");
        CinemachineVirtualCamera VC = FindObjectOfType<CinemachineVirtualCamera>();
        VC.Follow = CameraPosition;
        VC.LookAt = this.transform;
    }
}

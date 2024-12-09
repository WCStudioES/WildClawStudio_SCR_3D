using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationCamPos : MonoBehaviour
{
    private List<Vector3> cameraPositions = new List<Vector3>();
    private Vector3 basePos = new Vector3(3,3,3);

    [SerializeField] private Transform cameraLookAt;
    private List<Vector3> cameraTargets = new List<Vector3>();
    private Vector3 baseTargetPos = new Vector3(0, -1, 0);

    private void Start()
    {
        cameraPositions.Add(basePos);               //Ravager
        cameraTargets.Add(baseTargetPos);

        cameraPositions.Add(basePos);               //Pandora
        cameraTargets.Add(baseTargetPos);

        cameraPositions.Add(basePos);               //Albatross
        cameraTargets.Add(baseTargetPos);

        cameraPositions.Add(new Vector3(3, 2, 3));  //CargoQueen
        cameraTargets.Add(new Vector3(0, 0, 0));

        cameraPositions.Add(new Vector3(3, 4.5f, 0));  //IronSmith
        cameraTargets.Add(new Vector3(0, 0, -1));

    }
    public void SetCustomizationCamPos(int index)
    {
        if(cameraPositions.Count > index) 
        { 
            transform.localPosition = cameraPositions[index];
            cameraLookAt.localPosition = cameraTargets[index];
        }
        else
        {
            transform.localPosition = basePos;
            cameraLookAt.localPosition = baseTargetPos;
        }
    }
}

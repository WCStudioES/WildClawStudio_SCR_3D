using UnityEngine;
using UnityEngine.UI;

public class AudioToggleButton : MonoBehaviour
{
    public Image buttonImage;

    private void Start()
    {
        UpdateButtonIcon();
    }

    public void OnButtonPressed()
    {
        AudioManager.Instance.ToggleAudio();
        UpdateButtonIcon();
    }

    private void UpdateButtonIcon()
    {
        Debug.Log("AUDIO PULSADO");
        //gameObject.GetComponent<RectTransform>().transform.Rotate(new Vector3(0, 180, 0));
    }
}
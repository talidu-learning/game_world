using Interactables;
using TouchScript.Examples.CameraControl;
using UnityEngine;
using UnityEngine.UI;

public class ZoomSlider : MonoBehaviour
{
    private Slider slider;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private Image handleImage;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(ChangeZoom);
        SelectionManager.SELECT_OBJECT_EVENT.AddListener(OnSelectedObject);
        SelectionManager.DESELECT_OBJECT_EVENT.AddListener(OnDeselectedObject);
        SelectionManager.DELETE_OBJECT_EVENT.AddListener(OnDeselectedObject);
            
    }

    private void OnDeselectedObject()
    {
        slider.interactable = true;
        handleImage.color = ChangeAlpha(1f);
    }

    private Color ChangeAlpha(float amount)
    {
        var tempColor = handleImage.color;
        tempColor.a = amount;
        return tempColor;
    }

    private void OnSelectedObject(Interactable interactable)
    {
        slider.interactable = false;
        handleImage.color = ChangeAlpha(0.4f);
    }

    private void ChangeZoom(float amount)
    {
        cameraController.SetZoom(amount);
    }

    private void LateUpdate()
    {
        slider.value = cameraController.currentZoom;
    }
}

using Interactables;
using UnityEngine;
using UnityEngine.UI;

public class WithdrawButton : MonoBehaviour
{
    [SerializeField] private GameObject button;
    
    void Awake()
    {
        button.GetComponent<Button>().onClick.AddListener(OnClick);
        button.SetActive(false);
        SelectionManager.SELECT_OBJECT_EVENT.AddListener(OnSelectedObject);
        SelectionManager.DESELECT_OBJECT_EVENT.AddListener(OnDeselectObject);
    }

    private void OnDeselectObject()
    {
        button.SetActive(false);
    }

    private void OnSelectedObject(Interactable arg0)
    {
        button.SetActive(true);
    }

    private void OnClick()
    {
        button.SetActive(false);
        SelectionManager.WITHDRAW_OBJECT_EVENT.Invoke();
    }
}

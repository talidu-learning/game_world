using Interactables;
using UnityEngine;
using UnityEngine.UI;

public class WithdrawButton : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        SelectionManager.WITHDRAW_OBJECT_EVENT.Invoke();
    }
}

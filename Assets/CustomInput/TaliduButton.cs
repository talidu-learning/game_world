using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Prevents automatic double tapping on mobile devices
/// </summary>
public abstract class TaliduButton : MonoBehaviour
{
    protected bool isBlocked;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(DoAction);
    }

    private void DoAction()
    {
        if (isBlocked) return;
        isBlocked = true;
        OnClickedButton();
        StartCoroutine(BlockButton());
    }

    private IEnumerator BlockButton()
    {
        yield return new WaitForSeconds(3);
        isBlocked = false;
    }

    protected abstract void OnClickedButton();
}
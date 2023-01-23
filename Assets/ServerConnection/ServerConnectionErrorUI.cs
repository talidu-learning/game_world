using System.Collections;
using TouchScript;
using UnityEngine;
using UnityEngine.Events;

public class ServerConnectionErrorUI : MonoBehaviour
{
    [SerializeField] private GameObject ServerConnectionErrorUIWrapper;
    [SerializeField] private TouchManager TouchManager;
    [SerializeField] private Animation Animation;

    public static UnityEvent ServerErrorOccuredEvent = new UnityEvent();

    private void Awake()
    {
        ServerErrorOccuredEvent.AddListener(OnErrorOccured);
    }

    private void OnErrorOccured()
    {
        ServerConnectionErrorUIWrapper.SetActive(true);
        TouchManager.gameObject.SetActive(false);
        Animation.Play("Flicker");
        // StartCoroutine(WaitForAnimationEnd());
    }

    private IEnumerator WaitForAnimationEnd()
    {
        yield return new WaitWhile(() => Animation.isPlaying);
        ServerConnectionErrorUIWrapper.SetActive(false);
        TouchManager.gameObject.SetActive(true);
    }
}
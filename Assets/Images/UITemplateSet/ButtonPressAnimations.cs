using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonPressAnimations : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnClick = new UnityEvent();
    
    [SerializeField]private RectTransform ButtonGraphic;
    [SerializeField]private RectTransform TargetPosition;
    [SerializeField]private float DownPressTime = 0.04f;
    
    private IEnumerator buttonDown;
    private IEnumerator buttonUp;
    private Vector2 oldButtonWorldPos;

    private void Start()
    {
        oldButtonWorldPos = new Vector2(ButtonGraphic.anchoredPosition.x, ButtonGraphic.anchoredPosition.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonDown();
        OnClick.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ButtonUp();
    }

    public void ButtonDownPosition()
    {
        ButtonGraphic.anchoredPosition = Vector2.zero;
    }

    private void ButtonDown()
    {
        buttonDown = PressDownAnimation();
        StartCoroutine(buttonDown);
    }

    private void ButtonUp()
    {
        buttonUp = PressUpAnimation();
        StartCoroutine(buttonUp);
    }

    private IEnumerator PressDownAnimation()
    {
        float time = 0;


        while (time < DownPressTime)
        {
            ButtonGraphic.anchoredPosition = Vector2.Lerp(oldButtonWorldPos,
                ButtonGraphic.parent.InverseTransformPoint(new Vector3(TargetPosition.localToWorldMatrix.m03,
                    TargetPosition.localToWorldMatrix.m13, TargetPosition.localToWorldMatrix.m23)),
                time / DownPressTime);
            time += Time.deltaTime;
            yield return null;
        }

        ButtonGraphic.anchoredPosition = ButtonGraphic.parent.InverseTransformPoint(
            new Vector3(TargetPosition.localToWorldMatrix.m03, TargetPosition.localToWorldMatrix.m13,
                TargetPosition.localToWorldMatrix.m23));
    }

    private IEnumerator PressUpAnimation()
    {
        float time = 0;


        while (time < DownPressTime)
        {
            ButtonGraphic.anchoredPosition = Vector2.Lerp(
                ButtonGraphic.parent.InverseTransformPoint(new Vector3(TargetPosition.localToWorldMatrix.m03,
                    TargetPosition.localToWorldMatrix.m13, TargetPosition.localToWorldMatrix.m23)), oldButtonWorldPos,
                time / DownPressTime);
            time += Time.deltaTime;
            yield return null;
        }

        ButtonGraphic.anchoredPosition = oldButtonWorldPos;
    }
}
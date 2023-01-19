using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonPressAnimations : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform ButtonGraphic;
    public RectTransform TargetPosition;
    public float downPressTime = 0.04f;
    public float buttonUpTime = 0.1f;
    private IEnumerator buttonDown;
    private IEnumerator buttonUp;
    private Vector2 oldButtonWorldPos;

    private void Start()
    {
        oldButtonWorldPos = new Vector2(ButtonGraphic.anchoredPosition.x , ButtonGraphic.anchoredPosition.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ButtonUp();
    }

    public void ButtonDownPosition()
    {
        ButtonGraphic.anchoredPosition = Vector2.zero;
    }

    public void ButtonDown()
    {
        buttonDown = PressDownAnimation();
        StartCoroutine(buttonDown);
    }

    public void ButtonUp()
    {
        buttonUp = PressUpAnimation();
        StartCoroutine(buttonUp);
    }

    IEnumerator PressDownAnimation()
    {
        float time = 0;


        while (time < downPressTime)
        {
            ButtonGraphic.anchoredPosition = Vector2.Lerp(oldButtonWorldPos, ButtonGraphic.parent.InverseTransformPoint(new Vector3(TargetPosition.localToWorldMatrix.m03, TargetPosition.localToWorldMatrix.m13, TargetPosition.localToWorldMatrix.m23)), time / downPressTime);
            time += Time.deltaTime;
            yield return null;
        }
        ButtonGraphic.anchoredPosition = ButtonGraphic.parent.InverseTransformPoint(new Vector3( TargetPosition.localToWorldMatrix.m03, TargetPosition.localToWorldMatrix.m13, TargetPosition.localToWorldMatrix.m23));
    }

    IEnumerator PressUpAnimation()
    {
        float time = 0;


        while (time < downPressTime)
        {
            ButtonGraphic.anchoredPosition = Vector2.Lerp(ButtonGraphic.parent.InverseTransformPoint(new Vector3(TargetPosition.localToWorldMatrix.m03, TargetPosition.localToWorldMatrix.m13, TargetPosition.localToWorldMatrix.m23)), oldButtonWorldPos, time / downPressTime);
            time += Time.deltaTime;
            yield return null;
        }
        ButtonGraphic.anchoredPosition = oldButtonWorldPos;
    }
}

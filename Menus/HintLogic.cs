using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HintLogic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string hint;

    public Image hintBg;
    public Text hintText;

    [ShowNonSerializedField]
    private float timer = 1f;
    [ShowNonSerializedField]
    private bool mouseEntered = false;

    private void Update()
    {
        if (mouseEntered)
            timer -= Time.deltaTime;

        if (timer < 0)
        {
            hintBg.gameObject.SetActive(true);
            hintText.gameObject.SetActive(true);
            hintText.text = hint;

        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        mouseEntered = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        mouseEntered = false;
        hintBg.gameObject.SetActive(false);
        hintText.gameObject.SetActive(false);
        timer = 1f;
    }
}

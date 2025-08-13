using TMPro;
using UnityEngine;

public class TabletLogic : MonoBehaviour
{
    public GameObject tablet;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    public bool tabletIsOpen;

    public void OpenTablet(string title, string text)
    {
        tablet.SetActive(true);
        titleText.text = title;
        descriptionText.text = text;
        tabletIsOpen = true;
    }

    public void CloseTablet()
    {
        tablet.SetActive(false);
        tabletIsOpen = false;
    }
}

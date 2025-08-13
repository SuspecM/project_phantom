using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    public Image fadeImage;
    [SerializeField] private float currentAlpha;
    [SerializeField] private float desiredAlpha;

    public float fadeMult = .5f;

    void Start()
    {
        fadeMult = .5f;
    }

    private void Update()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, Time.deltaTime * fadeMult);
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currentAlpha);
    }

    public void Fade(bool fadeout)
    {
        if (fadeout)
        {
            desiredAlpha = 1f;
        }
        else
        {
            desiredAlpha = 0f;
        }
    }

    public void QuickFade()
    {
        desiredAlpha = 1f;
        fadeMult = 10f;
    }

}

using TMPro;
using UnityEngine;

public class KeycardTextManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    private float _timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (text.color.a != 0)
        {
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime);
            }
        }
    }

    public void ShowText()
    {
        text.color = new Color(1, 1, 1, 1);
        _timer = 2;
    }
}

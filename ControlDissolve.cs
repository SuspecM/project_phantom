using UnityEngine;

public class ControlDissolve : MonoBehaviour
{
    public bool _enableDissolve;
    public Material _material;
    private float _currentFloat;

    private void Start()
    {
        _currentFloat = 0f;
        _material.SetFloat("_Progress", _currentFloat);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_enableDissolve)
        {
            _currentFloat = Mathf.MoveTowards(_currentFloat, 1f, Time.deltaTime / 2f);

            _material.SetFloat("_Progress", _currentFloat);

            if (_currentFloat == 1f)
            {
                gameObject.SetActive(false);
                _material.SetFloat("_Progress", 0f);
            }
        }
    }

    public void EnableDissolusion()
    {
        _enableDissolve = true;
    }
}

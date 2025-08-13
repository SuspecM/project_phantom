using System.Collections.Generic;
using UnityEngine;

public class CheckForEnemyTrigrger : MonoBehaviour
{
    public string checkForTag;
    public bool tagPresent;
    public List<GameObject> objectsToCheck;
    private float _checkTimer;

    public BankDoor[] doorsToOpenClose;
    public GameObject[] objectsToDisable;
    public GameObject[] objectsToEnable;

    private void Start()
    {
        tagPresent = true;
        _checkTimer = 2f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(checkForTag))
        {
            objectsToCheck.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(checkForTag))
        {
            objectsToCheck.Remove(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void Update()
    {
        _checkTimer -= Time.deltaTime;
        if (_checkTimer <= 0)
        {
            tagPresent = false;
            foreach (var item in objectsToCheck)
            {
                if (item != null && item.gameObject.CompareTag(checkForTag))
                {
                    tagPresent = true;
                }
            }
            _checkTimer = 2f;
            if (!tagPresent) OpenShit();
        }

    }

    public void OpenShit()
    {
        foreach (var item in doorsToOpenClose)
        {
            item.OpenClose();
        }

        foreach (var item in objectsToEnable)
        {
            item.SetActive(true);
        }

        foreach (var item in objectsToDisable)
        {
            item.SetActive(false);
        }

        Destroy(gameObject);
    }
}

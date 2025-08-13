using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutosaveTrigger : MonoBehaviour
{
    public string levelName;
    public int checkPoint;
    public int menuBg;

    private float _startDelay = .75f;

    public bool destroy = true;
    private void OnTriggerEnter(Collider collision)
    {
        if ((collision.transform.CompareTag("Player")) && _startDelay < 0)
        {
            AutoSave(collision);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        
    }

    private void OnTriggerStay(Collider collision)
    {
        if ((collision.transform.CompareTag("Player")) && _startDelay < 0)
        {
            AutoSave(collision);
        }
    }

    private void AutoSave(Collider collision)
    {
        try
        {
            FindFirstObjectByType<StartGame>().elevator = false;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        int canSrynge = 0;
        if (collision.GetComponent<NewCharacterController>().canSrynge)
        {
            canSrynge = 1;
        }

        int hasThrowable = 0;
        if (collision.GetComponent<NewCharacterController>().hasObjectInHand) hasThrowable = 1;

        int hasRedKeycard = 0;
        if (collision.GetComponent<KeycardHolder>().heldKeycards[0]) hasRedKeycard = 1;

        int hasBlueKeycard = 0;
        if (collision.GetComponent<KeycardHolder>().heldKeycards[1]) hasBlueKeycard = 1;

        FindAnyObjectByType<PersistentGameManager>().CopyPlayerVariables();

        FindFirstObjectByType<PersistentGameManager>().saveManager.SaveNonDefaultProfile("Auto save trigger", levelName, checkPoint, menuBg, collision.GetComponent<Gun>().loadedAmmo, collision.GetComponent<Gun>().reserveAmmo, canSrynge, collision.GetComponent<Gun>().srynges, hasThrowable, collision.GetComponent<NewCharacterController>().indexOfActiveObject, hasRedKeycard, hasBlueKeycard, FindAnyObjectByType<PersistentGameManager>().physicsTutorialMessageShown, FindAnyObjectByType<PersistentGameManager>().handObjectTutorialMessageShown);

        Debug.Log($"Stuff saved: {levelName}, {checkPoint}, {menuBg}, {collision.GetComponent<Gun>().loadedAmmo}, {collision.GetComponent<Gun>().reserveAmmo}, {canSrynge}, {collision.GetComponent<Gun>().srynges}," +
            $"{hasThrowable}, {collision.GetComponent<NewCharacterController>().indexOfActiveObject}, {hasRedKeycard}, {hasBlueKeycard}, {FindAnyObjectByType<PersistentGameManager>().physicsTutorialMessageShown}, {FindAnyObjectByType<PersistentGameManager>().handObjectTutorialMessageShown}");
        Debug.Log($"Current active scene at the time of autosaving: {SceneManager.GetActiveScene().name}");
        Debug.Log("Loaded scenes at the time of triggering the autosave: ");

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            Debug.Log($"Scene {i}: {scene.name}");
        }

        collision.GetComponent<SaveIconManager>().PlayAnim();

        if (destroy)
            Destroy(gameObject);
    }

    private void Update()
    {
        _startDelay -= Time.deltaTime;
    }
}

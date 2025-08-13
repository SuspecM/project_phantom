using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI text;
    public bool messageIsAlreadyUp;
    public Queue<string> messageQueue = new Queue<string>();
    public Queue<float> timerQueue = new Queue<float>();

    public int physicsTutorialMessageShown;
    public int handObjectTutorialMessageShown;

    public void QueueMessage(string whatText, float timer)
    {
        messageQueue.Enqueue(whatText);
        timerQueue.Enqueue(timer);

        if (!messageIsAlreadyUp)
        {
            PopUp();
        }
        
    }

    private void PopUp()
    {
        messageIsAlreadyUp = true;

        text.text = messageQueue.Dequeue();
        animator.Play("pop up");

        Invoke("PopBack", timerQueue.Dequeue());
    }

    private void PopBack()
    {
        animator.Play("pop back");

        if (messageQueue.Count > 0)
        {
            Invoke("ProcessQueue", 1);
        }
        else
        {
            messageIsAlreadyUp = false;
        }
    }

    private void ProcessQueue()
    {
        PopUp();
    }
}

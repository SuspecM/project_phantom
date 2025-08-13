using UnityEngine;

public class EnableVentSpiderHearing : MonoBehaviour
{
    public VentSpiderLogic ventSpiderLogic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ventSpiderLogic.canHear = true;
            Destroy(gameObject);
        }
        
    }
}

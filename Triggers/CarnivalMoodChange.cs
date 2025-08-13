using NaughtyAttributes;
using UnityEngine;

public class CarnivalMoodChange : MonoBehaviour
{
    public bool woodwinds;
    public bool excitement;

    private MusicPlayer music;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<MusicPlayer>( out MusicPlayer music))
        {
            music.ChangeCarnivalMood(woodwinds, excitement);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<MusicPlayer>(out MusicPlayer music))
        {
            music.ChangeCarnivalMood(false, false);
        }
    }
}

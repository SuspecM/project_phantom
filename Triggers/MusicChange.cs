using FMODUnity;
using UnityEngine;

public class MusicChange : MonoBehaviour
{
    public EventReference newMusic;
    public bool fadeout;

    public bool destroy;

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<MusicPlayer>(out MusicPlayer musicPlayer);
        musicPlayer.SwapMusic(newMusic, fadeout);

        if (destroy)
            Destroy(gameObject);
    }
}

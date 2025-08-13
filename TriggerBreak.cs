using UnityEngine;

public class TriggerBreak : MonoBehaviour
{
    private FeelEffectsManager _player;
    private void Start()
    {
        _player = FindFirstObjectByType<FeelEffectsManager>();
    }

    public void TriggerFeelsEffect()
    {
        if (_player != null)
        {
            _player.PlayEvent(FeelEffectToPlay.BreakEvent);
        }
        else
        {
            _player = FindFirstObjectByType<FeelEffectsManager>();
            _player.PlayEvent(FeelEffectToPlay.BreakEvent);
        }
    }
}

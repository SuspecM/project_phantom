using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains;
using MoreMountains.Feedbacks;

public enum FeelEffectToPlay
{
    none = 0,
    ShortShakeEvent = 1,
    MediumWeakEvent = 2,
    BreakEvent = 3,
    BigElevatorStartEvent = 4,
    DeconEvent = 5
}

public class FeelEffectsManager : MonoBehaviour
{
    public MMF_Player shortShakeEvent;
    public MMF_Player mediumWeakEvent;
    public MMF_Player breakEvent;
    public MMF_Player bigElevatorStartEvent;
    public MMF_Player deconEvent;

    public void PlayEvent(FeelEffectToPlay effect)
    {
        switch (effect)
        {
            case FeelEffectToPlay.none: break;
            case FeelEffectToPlay.ShortShakeEvent: shortShakeEvent.PlayFeedbacks(); break;
            case FeelEffectToPlay.MediumWeakEvent: mediumWeakEvent.PlayFeedbacks(); break;
            case FeelEffectToPlay.BreakEvent: breakEvent.PlayFeedbacks(); break;
            case FeelEffectToPlay.BigElevatorStartEvent: bigElevatorStartEvent.PlayFeedbacks(); break;
            case FeelEffectToPlay.DeconEvent: deconEvent.PlayFeedbacks(); break;
        }
    }
}

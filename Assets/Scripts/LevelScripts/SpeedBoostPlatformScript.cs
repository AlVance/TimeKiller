using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class SpeedBoostPlatformScript : MonoBehaviour
{
    [SerializeField] private float speed_boost_multiplier;
    [SerializeField] private float speed_boost_timer;
    
    public float speedBoostMultiplier
    {
        get { return speedBoostMultiplier; } private set { speedBoostMultiplier = speed_boost_multiplier;  }
    }

    public float speedBoostTimer
    {
        get { return speedBoostTimer; }
        private set { speedBoostTimer = speed_boost_timer; }
    }

}

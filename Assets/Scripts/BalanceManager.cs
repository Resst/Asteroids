using UnityEngine;

public class BalanceManager : MonoBehaviour
{
    [Header("Score")]
    public int scorePerLargeAsteroid = 20;
    public int scorePerMediumAsteroid = 50;
    public int scorePerSmallAsteroid = 100;
    public int scorePerUfo = 200;

    [Header("Player")] 
    public int playerMaxBullets = 3;
    public float playerReloadingTime = 1f;
    public float playerImmuneTime = 3f;
    
    public float
        playerMaxSpeed = 10f,
        playerAcceleration = 3f,
        playerRotationSpeed = 180f;
    
    [Header("Asteroids")]
    public int asteroidBreakAngle = 45;

    
    [Header("Ufo")]
    public float ufoMinShootingTime = 2f;
    public float ufoMaxShootingTime = 5f;
    public float ufoFlightTime = 10f;
    public float ufoMinSpawningTime = 20f;
    public float ufoMaxSpawningTime = 40f;

    [Header("Other")]
    public float newLevelSpawningTime = 2f;

    #region SingletonRealization

    public static BalanceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion
}
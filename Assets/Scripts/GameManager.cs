using System;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public static bool GameStarted;
    public static int Score;
    public static int Lives;

    public static int AsteroidsLeft;
    public static bool MouseControlled = false;

    private bool _restarting = false;


    [SerializeField] private Ufo pfUfo;
    [SerializeField] private Player pfPlayer;

    private Player _player;

    private PoolingManager _poolingManager;
    private UI _ui;

    private Coroutine _ufoCoroutine;
    private Coroutine _asteroidsCoroutine;

    private int _asteroidsToInit;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        _poolingManager = GetComponent<PoolingManager>();
        _ui = GetComponent<UI>();
    }

    private void Start()
    {
        if (!GameStarted) return;

        _player = Instantiate(pfPlayer);
        _player.SetBulletPool(_poolingManager.PlayerBulletPool);
        _player.GetComponent<PlayerController>().MouseControlled = MouseControlled;

        Lives = 4;
        Score = 0;
        RestartUfo();
        _asteroidsToInit = 2;
        InitAsteroids();
    }

    private void Update()
    {
        if (!GameStarted || _restarting) return;

        if (AsteroidsLeft <= 0 && _asteroidsCoroutine == null)
        {
            NextLevel();
        }

        if (Lives <= 0)
        {
            StartCoroutine(GameOver());
        }
    }

    private void NextLevel()
    {
        RestartUfo();
        _asteroidsToInit++;
        _asteroidsCoroutine = StartCoroutine(InitAsteroidsCoroutine());
    }

    private IEnumerator GameOver()
    {
        GameStarted = false;
        StopCoroutine(_ufoCoroutine);
        Destroy(_player.gameObject);
        yield return new WaitForSeconds(1f);
        _ui.SwitchPaused();
    }

    public void SetControls(bool mouseControlled)
    {
        MouseControlled = mouseControlled;
        if (_player != null)
        {
            _player.GetComponent<PlayerController>().MouseControlled = MouseControlled;
        }
    }

    public void Restart()
    {
        GameStarted = true;
        _restarting = true;
        AsteroidsLeft = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region Asteroids
    private IEnumerator InitAsteroidsCoroutine()
    {
        yield return new WaitForSeconds(BalanceManager.Instance.newLevelSpawningTime);
        InitAsteroids();
        _asteroidsCoroutine = null;
    }

    private void InitAsteroids()
    {
        var asteroidPool = _poolingManager.AsteroidPool;
        for (int i = 0; i < _asteroidsToInit; i++)
        {
            Asteroid a = asteroidPool.Get();
            a.Randomize();
            a.Size = Asteroid.Sizes.Large;
        }
    }

    #endregion

    #region Ufo

    private void RestartUfo()
    {
        if (_ufoCoroutine != null) StopCoroutine(_ufoCoroutine);
        _ufoCoroutine = StartCoroutine(UfoCoroutine());
    }

    private IEnumerator UfoCoroutine()
    {
        yield return new WaitForSeconds(
            Random.Range(
                BalanceManager.Instance.ufoMinSpawningTime, BalanceManager.Instance.ufoMaxSpawningTime));
        var u = Instantiate(pfUfo);
        u.SetBulletPool(_poolingManager.UfoBulletPool);
        u.SetPlayer(_player.transform);
        yield return new WaitUntil(u.IsDestroyed);
        yield return StartCoroutine(UfoCoroutine());
    }

    #endregion
}
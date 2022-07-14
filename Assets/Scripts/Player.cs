using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public Transform gun;

    public float ImmuneTimer { get; private set; }
    
    private int _remainingBullets;
    private float _timeFromFirstShoot;

    private ObjectPool<Bullet> _bulletPool;

    private AudioSource _accelSource;
    private AudioSource _fireSource;

    private PlayerController _controller;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _accelSource = GetComponents<AudioSource>()[0];
        _fireSource = GetComponents<AudioSource>()[1];
        _remainingBullets = BalanceManager.Instance.playerMaxBullets;
        _timeFromFirstShoot = BalanceManager.Instance.playerReloadingTime;
    }

    void Update()
    {
        //Updating Magazine
        if (_timeFromFirstShoot < 1)
        {
            _timeFromFirstShoot += Time.deltaTime;
        }
        else
        {
            _remainingBullets = BalanceManager.Instance.playerMaxBullets;
        }

        //Updating immunity timer
        if (ImmuneTimer > 0)
        {
            ImmuneTimer -= Time.deltaTime;
        }

        HandleInput();
        
        Move();
    }

    private void HandleInput()
    {
        //We don't want to process input if game is paused
        if (UI.Paused) return;
        
        Rotate(_controller.Control.Rotate());

        if (_controller.Control.Accelerate())
        {
            Accelerate();
            if (!_accelSource.isPlaying)
            {
                _accelSource.Play();
            }
        }

        if (_controller.Control.Fire() && _remainingBullets > 0)
        {
            Shoot();
            _fireSource.Play();
        }
    }

    #region Movement

    private Vector3 _speed;

    private void Accelerate()
    {
        _speed += transform.rotation * Vector3.right * (BalanceManager.Instance.playerAcceleration * Time.deltaTime);

        var m = _speed.magnitude;

        if (m <= BalanceManager.Instance.playerMaxSpeed) return;

        _speed /= m;
        _speed *= BalanceManager.Instance.playerMaxSpeed;
    }

    private void Rotate(float direction)
    {
        transform.Rotate(Vector3.back * (direction * BalanceManager.Instance.playerRotationSpeed * Time.deltaTime));
    }

    private void Move()
    {
        transform.position += _speed * Time.deltaTime;
    }

    #endregion


    private void Shoot()
    {
        var bulletDistance = GameCamera.HalfWidth * 2;

        _bulletPool.Get()
            .Init(gun.position, transform.rotation, bulletDistance);

        //reducing bullets in magazine
        if (_remainingBullets == BalanceManager.Instance.playerMaxBullets)
        {
            _timeFromFirstShoot = 0;
        }

        _remainingBullets--;
    }

    public void TakeDamage()
    {
        if (ImmuneTimer > 0)
        {
            Teleport();
            return;
        }

        GameManager.Lives--;
        ImmuneTimer = BalanceManager.Instance.playerImmuneTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var a = col.gameObject.GetComponent<Asteroid>();
        var u = col.gameObject.GetComponent<Ufo>();
        var b = col.gameObject.GetComponent<UfoBullet>();
        
        if (a != null || u != null || b != null)
        {
            TakeDamage();
        }
    }

    private Coroutine _tpCoroutine;
    private void Teleport()
    {
        if (_tpCoroutine == null)
        {
            _tpCoroutine = StartCoroutine(TpCoroutine());
        }
    }

    private IEnumerator TpCoroutine()
    {
        var hh = GameCamera.HalfHeight;
        var hw = GameCamera.HalfWidth;
        transform.SetPositionAndRotation(
            new Vector3(Random.Range(-hw, hw), Random.Range(-hh, hh), 0),
            transform.rotation
        );
        yield return new WaitForSeconds(.5f);
        _tpCoroutine = null;
    }

    public void SetBulletPool(ObjectPool<Bullet> bulletPool) => _bulletPool = bulletPool;
}
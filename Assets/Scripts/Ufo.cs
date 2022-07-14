using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Ufo : MonoBehaviour
{
    private float _speed;
    private float _direction;
    private AudioSource _fireSource;
    [SerializeField] private AudioClip deathSound;

    private Coroutine _shootingCoroutine;
    

    private void Awake()
    {
        _fireSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Spawning UFO with random direction
        _direction = Random.value > 0.5f ? 1 : -1;
        
        var x = -_direction * (GameCamera.HalfWidth + 1);   // + 1 for UFO size
        
        _speed = GameCamera.HalfWidth * 2 / BalanceManager.Instance.ufoFlightTime;
        _speed *= _direction;
        
        // Making sure that distance between UFO and Screen border is more than 20%
        var y = Random.Range(-0.6f, 0.6f) * GameCamera.HalfHeight;
        
        transform.position = new Vector3(x, y, 0);
        
        _shootingCoroutine = StartCoroutine(ShootingCoroutine());
    }

    private void Update()
    {
        Move();
        
        // if out of bounds => destroy
        if (_direction * transform.position.x >= GameCamera.HalfWidth + 1)
        {
            StopCoroutine(_shootingCoroutine);
            Destroy(gameObject, _fireSource.clip.length);
        }
    }


    private void Move()
    {
        transform.Translate(_speed * Time.deltaTime, 0, 0);
    }

    public void TakeDamage()
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var a = col.gameObject.GetComponent<Asteroid>();
        var p = col.gameObject.GetComponent<Player>();
        
        if (a != null || (p != null && p.ImmuneTimer <= 0))
        {
            TakeDamage();
        }
    }

    #region Shooting
    
    private ObjectPool<Bullet> _bulletPool;
    private Transform _playerTransform;

    public void SetBulletPool(ObjectPool<Bullet> bulletPool) => _bulletPool = bulletPool;
    public void SetPlayer(Transform playerTransform) => _playerTransform = playerTransform;
    
    
    private void Shoot()
    {
        var bulletDistance = GameCamera.HalfWidth * 2;
        Vector3 dir = _playerTransform.position - transform.position;
        var angle = -Vector2.SignedAngle(dir, Vector2.right);
        
        _bulletPool.Get().Init(transform.position, Quaternion.Euler(0, 0, angle), bulletDistance);
    }
    private IEnumerator ShootingCoroutine()
    {
        yield return new WaitForSeconds(
            Random.Range(
                BalanceManager.Instance.ufoMinShootingTime, BalanceManager.Instance.ufoMaxShootingTime));
        Shoot();
        _fireSource.Play();
        yield return StartCoroutine(ShootingCoroutine());
    }
    
    #endregion
}
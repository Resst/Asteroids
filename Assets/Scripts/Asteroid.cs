using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    private const float MinSpeed = .5f;
    private const float MaxSpeed = 2f;
    private float _speed;

    [SerializeField] private AudioClip largeExplosion, mediumExplosion, smallExplosion;

    public AudioClip DeathSound
    {
        get
        {
            return _size switch
            {
                Sizes.Large => largeExplosion,
                Sizes.Medium => mediumExplosion,
                _ => smallExplosion
            };
        }
    }

    private ObjectPool<Asteroid> _pool;


    private Sizes _size;

    public Sizes Size
    {
        get => _size;
        set
        {
            float scaling = 0;
            switch (value)
            {
                case Sizes.Large:
                    scaling = 3;
                    break;
                case Sizes.Medium:
                    scaling = 2;
                    break;
                case Sizes.Small:
                    scaling = 1;
                    break;
            }

            transform.localScale = new Vector3(scaling, scaling, 1);
            _size = value;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.right * (_speed * Time.deltaTime));
    }

    public void Randomize()
    {
        Vector3 newPos = new Vector3(
            Random.Range(-GameCamera.HalfWidth, GameCamera.HalfWidth),
            Random.Range(-GameCamera.HalfHeight, GameCamera.HalfHeight),
            0
        );

        Quaternion newRot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        transform.SetPositionAndRotation(newPos, newRot);
        
        _speed = Random.Range(MinSpeed, MaxSpeed);
    }

    public void TakeDamage()
    {
        if (Size != Sizes.Small)
        {
            Split();
        }

        _pool.Release(this);
    }

    private void Split()
    {
        var t = transform;

        var angle = BalanceManager.Instance.asteroidBreakAngle;

        var a1 = _pool.Get();
        a1.transform.SetPositionAndRotation(t.position, t.rotation * Quaternion.Euler(0, 0, angle));

        var a2 = _pool.Get();
        a2.transform.SetPositionAndRotation(t.position, t.rotation * Quaternion.Euler(0, 0, -angle));
        
        a1.Size = Size - 1;
        a2.Size = Size - 1;
        
        var spd = Random.Range(MinSpeed, MaxSpeed);
        a1._speed = spd;
        a2._speed = spd;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var u = col.gameObject.GetComponent<Ufo>();
        var p = col.gameObject.GetComponent<Player>();
        if (u != null || (p != null && p.ImmuneTimer <= 0))
        {
            _pool.Release(this);
        }
    }

    public void SetPool(ObjectPool<Asteroid> pool) => _pool = pool;

    public enum Sizes
    {
        Small,
        Medium,
        Large
    }
}
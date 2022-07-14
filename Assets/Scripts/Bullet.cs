using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private Vector3 _direction;

    private const float Speed = 10f;

    private float _distance;

    protected ObjectPool<Bullet> Pool;

    protected bool Released;


    public void Init(Vector3 position, Quaternion rotation, float distance)
    {
        transform.SetPositionAndRotation(position, rotation);
        _distance = distance;
    }

    void Update()
    {
        float translation = Speed * Time.deltaTime;
        transform.Translate(Vector3.right * translation);
        _distance -= translation;
        if (_distance <= 0)
        {
            Pool.Release(this);
        }
    }

    public void OnRelease()
    {
        Released = true;
        gameObject.SetActive(false);
    }

    public void OnGet()
    {
        Released = false;
        gameObject.SetActive(true);
    }

    public Bullet SetPool(ObjectPool<Bullet> pool)
    {
        Pool = pool;
        return this;
    }
}
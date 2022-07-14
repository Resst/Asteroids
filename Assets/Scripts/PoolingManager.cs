using UnityEngine;
using UnityEngine.Pool;

class PoolingManager : MonoBehaviour
{
    public ObjectPool<Asteroid> AsteroidPool;
    public ObjectPool<Bullet> UfoBulletPool;
    public ObjectPool<Bullet> PlayerBulletPool;

    [SerializeField] private Asteroid pfAsteroid;
    [SerializeField] private PlayerBullet pfPlayerBullet;
    [SerializeField] private UfoBullet pfUfoBullet;

    private void Awake()
    {
        CreateAll();
    }

    private void CreateAll()
    {
        AsteroidPool = new ObjectPool<Asteroid>(
            CreateAsteroid, 
            GetAsteroid, 
            ReleaseAsteroid, 
            asteroid => Destroy(asteroid.gameObject),
            true, 10, 100);

        PlayerBulletPool = new ObjectPool<Bullet>(
            () => Instantiate(pfPlayerBullet).SetPool(PlayerBulletPool),
            bullet => bullet.OnGet(),
            bullet => bullet.OnRelease(),
            bullet => Destroy(bullet.gameObject), 
            true, 20, 50
        );
        
        UfoBulletPool = new ObjectPool<Bullet>(
            () => Instantiate(pfUfoBullet).SetPool(UfoBulletPool),
            bullet => bullet.OnGet(),
            bullet => bullet.OnRelease(),
            bullet => Destroy(bullet.gameObject), 
            true, 20, 50
        );
    }

    public void ReleaseAll()
    {
        UfoBulletPool.Clear();
        PlayerBulletPool.Clear();
        AsteroidPool.Clear();
    }

    private Asteroid CreateAsteroid()
    {
        var asteroid = Instantiate(pfAsteroid);
        return asteroid;
    }

    private void GetAsteroid(Asteroid asteroid)
    {
        GameManager.AsteroidsLeft++;
        asteroid.SetPool(AsteroidPool);
        asteroid.gameObject.SetActive(true);
    }

    private void ReleaseAsteroid(Asteroid asteroid)
    {
        GameManager.AsteroidsLeft--;
        AudioSource.PlayClipAtPoint(asteroid.DeathSound, asteroid.transform.position);
        asteroid.gameObject.SetActive(false);
    }
}
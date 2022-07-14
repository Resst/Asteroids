using UnityEngine;

public class PlayerBullet : Bullet
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (Released) return;
        
        Asteroid asteroid = col.gameObject.GetComponent<Asteroid>();
        if (asteroid != null)
        {
            asteroid.TakeDamage();
            
            //Updating score
            switch (asteroid.Size)
            {
                case Asteroid.Sizes.Large:
                    GameManager.Score += BalanceManager.Instance.scorePerLargeAsteroid;
                    break;
                case Asteroid.Sizes.Medium:
                    GameManager.Score += BalanceManager.Instance.scorePerMediumAsteroid;
                    break;
                case Asteroid.Sizes.Small:
                    GameManager.Score += BalanceManager.Instance.scorePerSmallAsteroid;
                    break;
            }

            Pool.Release(this);
            return;
            
        }

        Ufo ufo = col.gameObject.GetComponent<Ufo>();
        if (ufo != null)
        {
            ufo.TakeDamage();

            GameManager.Score += BalanceManager.Instance.scorePerUfo;
            
            Pool.Release(this);
        }
    }
}
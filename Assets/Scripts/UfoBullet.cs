using UnityEngine;

public class UfoBullet : Bullet
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (Released) return;

        if (col.gameObject.GetComponent<Player>() != null)
        {
            Pool.Release(this);
        }
    }
}

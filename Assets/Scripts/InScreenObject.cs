using UnityEngine;

public class InScreenObject : MonoBehaviour
{
    void Update()
    {
        var hh = GameCamera.HalfHeight;
        var hw = GameCamera.HalfWidth;

        var x = transform.position.x;
        var y = transform.position.y;
        
        // Checking if object is out of screen
        // We shouldn't check x and y coordinates of camera because they are 0, 0 and don't change
        if (x > hw)
        {
            transform.Translate(-2 * hw, 0, 0, Space.World);
        }
        else if (x < -hw)
        {
            transform.Translate(2 * hw, 0, 0, Space.World);
        }

        if (y > hh)
        {
            transform.Translate(0, -2 * hh, 0, Space.World);
        }
        else if (y < -hh)
        {
            transform.Translate(0, 2 * hh, 0, Space.World);
        }
    }
}
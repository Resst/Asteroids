using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Controller Control;
    private Controller _keyboard, _mouse;

    public bool MouseControlled
    {
        set => Control = value ? _mouse : _keyboard;
    }

    private void Awake()
    {
        var t = transform;
        _keyboard = new KeyboardController(t);
        _mouse = new MouseAndKeyboardController(t);
        MouseControlled = false;
    }

    #region ControllerStrategyRealization
    public abstract class Controller
    {
        protected readonly Transform ControlledEntity;
        protected Controller(Transform controlledEntity) => ControlledEntity = controlledEntity;
        public abstract bool Accelerate();
        public abstract float Rotate();
        public abstract bool Fire();
    }
    private class KeyboardController : Controller
    {
        public override bool Accelerate() => Input.GetAxisRaw("Vertical") > 0;

        public override float Rotate() => Input.GetAxisRaw("Horizontal");

        public override bool Fire() => Input.GetKeyDown(KeyCode.Space);

        public KeyboardController(Transform controlledEntity) : base(controlledEntity){}
    }

    private class MouseAndKeyboardController : Controller
    {
        public override bool Accelerate() =>
            Input.GetAxisRaw("Vertical") > 0 || Input.GetKey(KeyCode.Mouse1);

        public override float Rotate()
        {
            var dir = ControlledEntity.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var rot = ControlledEntity.rotation * Vector3.right;

            var angle = Vector2.SignedAngle(rot, dir);
            
            return Math.Sign(angle);
        }

        public override bool Fire() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0);

        public MouseAndKeyboardController(Transform controlledEntity) : base(controlledEntity){}
    }
    #endregion
}
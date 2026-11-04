using UnityEngine;
using UnityEngine.EventSystems;

namespace AetherBlade
{
    public static class TouchInput
    {
        public static float Move;
        static bool jump, attack, ult;
        public static bool ConsumeJump()   { var v = jump;   jump = false;   return v; }
        public static bool ConsumeAttack() { var v = attack; attack = false; return v; }
        public static bool ConsumeUlt()    { var v = ult;    ult = false;    return v; }
        public static void Press(TouchButton.Kind k, bool down)
        {
            switch (k)
            {
                case TouchButton.Kind.Left:   Move = down ? -1 : (Move < 0 ? 0 : Move); break;
                case TouchButton.Kind.Right:  Move = down ? 1 : (Move > 0 ? 0 : Move); break;
                case TouchButton.Kind.Jump:   if (down) jump = true; break;
                case TouchButton.Kind.Attack: if (down) attack = true; break;
                case TouchButton.Kind.Aether: if (down) ult = true; break;
            }
        }
    }

    public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public enum Kind { Left, Right, Jump, Attack, Aether }
        public Kind kind;
        public void OnPointerDown(PointerEventData e) => TouchInput.Press(kind, true);
        public void OnPointerUp(PointerEventData e) => TouchInput.Press(kind, false);
    }

    public class KeyboardInput : MonoBehaviour
    {
        void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            float h = Input.GetAxisRaw("Horizontal");
            if (h != 0 || Input.GetButtonUp("Horizontal")) TouchInput.Move = h;
            if (Input.GetKeyDown(KeyCode.W)) TouchInput.Press(TouchButton.Kind.Jump, true);
            if (Input.GetKeyDown(KeyCode.J)) TouchInput.Press(TouchButton.Kind.Attack, true);
            if (Input.GetKeyDown(KeyCode.K)) TouchInput.Press(TouchButton.Kind.Aether, true);
#endif
        }
    }
}

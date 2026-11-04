using UnityEngine;

namespace AetherBlade
{
    public class CameraShake : MonoBehaviour
    {
        static CameraShake inst; float t, mag; Vector3 basePos;
        void Awake() { inst = this; basePos = transform.localPosition; }
        public static void Shake(float magnitude, float duration) { if (inst) { inst.mag = magnitude; inst.t = duration; } }
        void LateUpdate()
        {
            if (t > 0) { t -= Time.deltaTime; transform.localPosition = basePos + Random.insideUnitSphere * mag; }
            else transform.localPosition = basePos;
        }
    }
}

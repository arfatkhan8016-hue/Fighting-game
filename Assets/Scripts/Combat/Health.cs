using System;
using UnityEngine;

namespace AetherBlade
{
    public class Health : MonoBehaviour
    {
        public float max = 100f;
        public float Current { get; private set; }
        public bool IsDead => Current <= 0f;
        public event Action<float, Vector3> Damaged;
        public event Action Died;

        void Awake() => Current = max;
        public void Heal(float v) => Current = Mathf.Min(max, Current + v);

        public void TakeDamage(float dmg, Vector3 knockback)
        {
            if (IsDead) return;
            Current = Mathf.Max(0, Current - dmg);
            Damaged?.Invoke(dmg, knockback);
            if (IsDead) Died?.Invoke();
        }
    }
}

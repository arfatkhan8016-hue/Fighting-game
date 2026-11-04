using UnityEngine;

namespace AetherBlade
{
    [RequireComponent(typeof(Fighter))]
    public class EnemyAI : MonoBehaviour
    {
        public Transform target;
        public float stopDistance = 1.6f, attackChancePerSec = 1.0f;
        public bool canUseUltimate;
        Fighter f;

        void Awake() => f = GetComponent<Fighter>();

        void Update()
        {
            if (!target) return;
            float dx = target.position.x - transform.position.x, dist = Mathf.Abs(dx);
            f.moveInput = dist > stopDistance ? Mathf.Sign(dx) : 0f;
            if (dist <= stopDistance + 0.3f)
            {
                if (Random.value < attackChancePerSec * Time.deltaTime) f.attackQueued = true;
                if (Random.value < 0.01f) f.moveInput = -Mathf.Sign(dx);
            }
            if (canUseUltimate && f.Aether >= f.aetherMax && dist < 6f) f.ultQueued = true;
        }
    }
}

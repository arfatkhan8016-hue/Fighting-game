using System.Collections;
using UnityEngine;

namespace AetherBlade
{
    [RequireComponent(typeof(CharacterController), typeof(Health))]
    public class Fighter : MonoBehaviour
    {
        [Header("Setup")]
        public bool isPlayer;
        public Animator anim;
        public LayerMask targetMask;
        [Header("Movement")]
        public float moveSpeed = 5f, jumpHeight = 1.6f, gravity = -25f;
        [Header("Combat")]
        public float[] comboDamage = { 8, 8, 14 };
        public float attackRange = 1.8f, comboWindow = 0.6f, hitDelay = 0.15f, attackLock = 0.3f;
        public float ultDamage = 45f, ultRange = 7f, aetherMax = 100f, aetherPerHit = 9f;

        [HideInInspector] public float moveInput;
        [HideInInspector] public bool jumpQueued, attackQueued, ultQueued;

        public float Aether { get; private set; }
        public Health Health { get; private set; }
        public int Combo { get; private set; }

        CharacterController cc; Vector3 vel; int step; float lastAtk, stunUntil, busyUntil; float face = 1;

        void Awake()
        {
            cc = GetComponent<CharacterController>();
            Health = GetComponent<Health>();
            Health.Damaged += OnHurt;
            Health.Died += () => { if (anim) anim.SetTrigger("Die"); enabled = false; };
        }

        void Update()
        {
            if (isPlayer) { moveInput = TouchInput.Move; jumpQueued |= TouchInput.ConsumeJump(); attackQueued |= TouchInput.ConsumeAttack(); ultQueued |= TouchInput.ConsumeUlt(); }
            bool locked = Time.time < stunUntil || Time.time < busyUntil;

            float h = locked ? 0 : moveInput;
            if (Mathf.Abs(h) > 0.1f) { face = Mathf.Sign(h); transform.rotation = Quaternion.Euler(0, face > 0 ? 90 : -90, 0); }

            if (cc.isGrounded && vel.y < 0) vel.y = -1f;
            if (!locked && jumpQueued && cc.isGrounded) vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            vel.y += gravity * Time.deltaTime;
            cc.Move(new Vector3(h * moveSpeed, vel.y, 0) * Time.deltaTime);
            var p = transform.position; p.z = 0; transform.position = p;

            if (!locked && ultQueued && Aether >= aetherMax) StartCoroutine(AetherSlash());
            else if (!locked && attackQueued) StartCoroutine(Attack());
            jumpQueued = attackQueued = ultQueued = false;

            if (anim) { anim.SetFloat("Speed", Mathf.Abs(h)); anim.SetBool("Grounded", cc.isGrounded); }
        }

        IEnumerator Attack()
        {
            step = (Time.time - lastAtk < comboWindow) ? (step % comboDamage.Length) + 1 : 1;
            lastAtk = Time.time; busyUntil = Time.time + attackLock;
            if (anim) anim.SetTrigger("Attack" + step);
            yield return new WaitForSeconds(hitDelay);
            var center = transform.position + Vector3.up + transform.forward * (attackRange * 0.5f);
            foreach (var c in Physics.OverlapSphere(center, attackRange, targetMask))
                if (c.TryGetComponent(out Health hp) && hp != Health) Hit(hp, comboDamage[step - 1], 6f);
        }

        IEnumerator AetherSlash()
        {
            Aether = 0; busyUntil = Time.time + 0.7f;
            if (anim) anim.SetTrigger("Ultimate");
            CameraShake.Shake(0.4f, 0.35f);
            yield return new WaitForSeconds(0.3f);
            var center = transform.position + Vector3.up + transform.forward * (ultRange * 0.5f);
            foreach (var c in Physics.OverlapBox(center, new Vector3(ultRange * 0.5f, 1.5f, 2f), Quaternion.identity, targetMask))
                if (c.TryGetComponent(out Health hp) && hp != Health) Hit(hp, ultDamage, 12f);
        }

        void Hit(Health target, float dmg, float kb)
        {
            target.TakeDamage(dmg, new Vector3(face * kb, 3f, 0));
            Combo++; CameraShake.Shake(0.08f, 0.15f);
            Aether = Mathf.Min(aetherMax, Aether + aetherPerHit);
            CancelInvoke(nameof(ResetCombo)); Invoke(nameof(ResetCombo), comboWindow + 0.8f);
        }
        void ResetCombo() => Combo = 0;

        void OnHurt(float dmg, Vector3 kb)
        {
            stunUntil = Time.time + 0.28f; vel = kb;
            if (anim) anim.SetTrigger("Hurt");
        }
    }
}

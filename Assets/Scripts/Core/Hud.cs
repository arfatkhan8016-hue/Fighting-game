using UnityEngine;
using UnityEngine.UI;

namespace AetherBlade
{
    public class Hud : MonoBehaviour
    {
        public Fighter player, enemy;
        public RectTransform playerHp, enemyHp, aether;
        public Text combo;

        void Awake() { Application.targetFrameRate = 60; }

        void Update()
        {
            if (!player || !enemy) return;
            playerHp.localScale = new Vector3(Mathf.Clamp01(player.Health.Current / player.Health.max), 1, 1);
            enemyHp.localScale  = new Vector3(Mathf.Clamp01(enemy.Health.Current / enemy.Health.max), 1, 1);
            aether.localScale   = new Vector3(Mathf.Clamp01(player.Aether / player.aetherMax), 1, 1);
            combo.text = player.Combo > 1 ? player.Combo + " HIT COMBO" : "";
        }
    }
}

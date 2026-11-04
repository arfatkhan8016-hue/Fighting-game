using UnityEngine;
using UnityEngine.SceneManagement;

namespace AetherBlade
{
    [System.Serializable]
    public class Chapter
    {
        public string title;
        [TextArea] public string intro;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }
        public Chapter[] chapters =
        {
            new Chapter { title = "Chapter 1: Awakening in Neon Ruins", intro = "Kaito ko yaadein nahi hain. Abandoned city mein pehla Shadow Grunt aa gaya." },
            new Chapter { title = "Chapter 2: The Syndicate's Trap", intro = "Lyra ke saath underground fortress par raid." },
            new Chapter { title = "Chapter 3: Clash of Fates", intro = "Valen se final showdown. Core bachane ke liye power sacrifice karni hogi." },
        };
        public int Current { get; private set; }
        public Fighter player, enemy;

        void Awake() { I = this; Current = PlayerPrefs.GetInt("chapter", 0); }

        void Start()
        {
            player.Health.Died += () => Invoke(nameof(Retry), 1.5f);
            enemy.Health.Died += () => Invoke(nameof(Next), 1.5f);
        }

        void Retry() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        void Next()
        {
            Current++;
            PlayerPrefs.SetInt("chapter", Current % chapters.Length);
            KotlinBridge.SaveProgress(Current);
            Retry();
        }
    }
}

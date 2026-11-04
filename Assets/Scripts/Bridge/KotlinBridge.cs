using UnityEngine;

namespace AetherBlade
{
    public class KotlinBridge : MonoBehaviour
    {
        public static string UserToken { get; private set; } = "";

        public void OnLogin(string token) { UserToken = token; Debug.Log("Login ok"); }

        public static void SaveProgress(int chapter)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try { using var c = new AndroidJavaClass("com.aetherstudio.aetherblade.NativeBridge"); c.CallStatic("saveProgress", chapter); }
            catch (System.Exception e) { Debug.LogWarning("Kotlin bridge nahi mila: " + e.Message); }
#endif
        }
    }
}

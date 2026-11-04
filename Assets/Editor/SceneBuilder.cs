using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AetherBlade.Editor
{
    public static class SceneBuilder
    {
        const string ScenePath = "Assets/Scenes/Main.unity";

        [MenuItem("Aether Blade/Build Demo Scene")]
        public static void Build()
        {
            Directory.CreateDirectory("Assets/Scenes"); Directory.CreateDirectory("Assets/Materials");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var lg = new GameObject("Sun"); var li = lg.AddComponent<Light>(); li.type = LightType.Directional; li.intensity = 1.1f;
            lg.transform.rotation = Quaternion.Euler(50, -30, 0);
            RenderSettings.ambientLight = new Color(.25f, .22f, .4f);
            var cg = new GameObject("Main Camera") { tag = "MainCamera" };
            var cam = cg.AddComponent<Camera>(); cg.AddComponent<AudioListener>(); cg.AddComponent<CameraShake>();
            cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(.04f, .03f, .12f);
            cg.transform.position = new Vector3(0, 2.6f, -9.5f); cg.transform.rotation = Quaternion.Euler(8, 0, 0);

            Prim("Ground", PrimitiveType.Cube, new Vector3(0, -.5f, 0), new Vector3(40, 1, 6), Mat("Ground", new Color(.08f, .07f, .16f), 0));
            for (int i = -8; i <= 8; i++)
            {
                var c = i % 2 == 0 ? new Color(0.2f, .9f, 1f) : new Color(1f, .25f, .55f);
                float h = 4 + Mathf.Abs((i * 37) % 7);
                Prim("Building" + i, PrimitiveType.Cube, new Vector3(i * 3f, h / 2, 9), new Vector3(2.4f, h, 2), Mat(i % 2 == 0 ? "NeonC" : "NeonM", c * .5f, 1.2f));
            }

            var kaito = MakeFighter("Kaito", -3f, new Color(.9f, .95f, 1f), new Color(.2f, .9f, 1f), true);
            var valen = MakeFighter("Valen", 3f, new Color(.15f, .1f, .2f), new Color(1f, .15f, .3f), false);
            valen.GetComponent<Health>().max = 140;
            var ai = valen.gameObject.AddComponent<EnemyAI>(); ai.target = kaito.transform; ai.canUseUltimate = true;
            valen.transform.rotation = Quaternion.Euler(0, -90, 0);

            var canvasGo = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var sc = canvasGo.GetComponent<CanvasScaler>(); sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; sc.referenceResolution = new Vector2(1920, 1080); sc.matchWidthOrHeight = .5f;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            var hud = canvasGo.AddComponent<Hud>(); hud.player = kaito; hud.enemy = valen;
            hud.playerHp = Bar(canvasGo.transform, "KaitoHP", new Vector2(0, 1), new Vector2(60, -60), new Vector2(700, 40), new Color(.2f, .9f, 1f), false);
            hud.aether   = Bar(canvasGo.transform, "Aether",  new Vector2(0, 1), new Vector2(60, -110), new Vector2(700, 22), new Color(1f, .82f, .3f), false);
            hud.enemyHp  = Bar(canvasGo.transform, "EnemyHP", new Vector2(1, 1), new Vector2(-60, -60), new Vector2(700, 40), new Color(1f, .24f, .55f), true);
            var combo = Label(canvasGo.transform, "Combo", "", 64, new Vector2(.5f, 1), new Vector2(0, -160), new Vector2(800, 100)); combo.color = new Color(1f, .82f, .3f); hud.combo = combo;

            Btn(canvasGo.transform, "Left",  "◀", TouchButton.Kind.Left,   new Vector2(0, 0), new Vector2(60, 60),   170);
            Btn(canvasGo.transform, "Right", "▶", TouchButton.Kind.Right,  new Vector2(0, 0), new Vector2(260, 60),  170);
            Btn(canvasGo.transform, "Jump",  "↑", TouchButton.Kind.Jump,   new Vector2(1, 0), new Vector2(-60, 260), 170);
            Btn(canvasGo.transform, "Attack","⚔", TouchButton.Kind.Attack, new Vector2(1, 0), new Vector2(-60, 60),  190);
            Btn(canvasGo.transform, "Aether","AETHER", TouchButton.Kind.Aether, new Vector2(1, 0), new Vector2(-270, 100), 170);

            new GameObject("KeyboardInput").AddComponent<KeyboardInput>();
            var gm = new GameObject("GameManager").AddComponent<GameManager>(); gm.player = kaito; gm.enemy = valen;

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("Aether Blade demo scene ready: " + ScenePath);
        }

        static Material Mat(string name, Color c, float emission)
        {
            var path = "Assets/Materials/" + name + ".mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m) return m;
            m = new Material(Shader.Find("Standard")) { color = c };
            if (emission > 0) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", c * emission); }
            AssetDatabase.CreateAsset(m, path); return m;
        }

        static GameObject Prim(string n, PrimitiveType t, Vector3 pos, Vector3 scale, Material m, Transform parent = null)
        {
            var g = GameObject.CreatePrimitive(t); g.name = n;
            if (parent) g.transform.SetParent(parent, false);
            g.transform.position = parent ? parent.position + pos : pos; g.transform.localScale = scale;
            g.GetComponent<Renderer>().sharedMaterial = m; return g;
        }

        static Fighter MakeFighter(string n, float x, Color body, Color glow, bool player)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Capsule); g.name = n;
            Object.DestroyImmediate(g.GetComponent<Collider>());
            g.GetComponent<Renderer>().sharedMaterial = Mat(n + "Body", body, 0);
            g.transform.position = new Vector3(x, 1.1f, 0); g.transform.rotation = Quaternion.Euler(0, 90, 0);
            var f = g.AddComponent<Fighter>();
            f.isPlayer = player; f.targetMask = ~0;
            var cc = g.GetComponent<CharacterController>(); cc.height = 2; cc.radius = .5f; cc.center = Vector3.zero;
            var sword = GameObject.CreatePrimitive(PrimitiveType.Cube); sword.name = "Blade"; Object.DestroyImmediate(sword.GetComponent<Collider>());
            sword.transform.SetParent(g.transform, false); sword.transform.localPosition = new Vector3(0.15f, 0.1f, 1.0f); sword.transform.localScale = new Vector3(.12f, .12f, 1.4f);
            sword.GetComponent<Renderer>().sharedMaterial = Mat(n + "Blade", glow, 3f);
            var hair = GameObject.CreatePrimitive(PrimitiveType.Sphere); hair.name = "Hair"; Object.DestroyImmediate(hair.GetComponent<Collider>());
            hair.transform.SetParent(g.transform, false); hair.transform.localPosition = new Vector3(0, .85f, 0); hair.transform.localScale = new Vector3(.8f, .6f, .8f);
            hair.GetComponent<Renderer>().sharedMaterial = Mat(n + "Hair", player ? Color.white : new Color(.1f, .1f, .12f), 0);
            return f;
        }

        static RectTransform Rect(string n, Transform parent, Vector2 anchor, Vector2 pos, Vector2 size, Vector2? pivot = null)
        {
            var go = new GameObject(n, typeof(RectTransform)); go.transform.SetParent(parent, false);
            var r = go.GetComponent<RectTransform>(); r.anchorMin = r.anchorMax = anchor; r.pivot = pivot ?? anchor; r.anchoredPosition = pos; r.sizeDelta = size; return r;
        }

        static Text Label(Transform p, string n, string t, int size, Vector2 anchor, Vector2 pos, Vector2 sz)
        {
            var r = Rect(n, p, anchor, pos, sz); var tx = r.gameObject.AddComponent<Text>();
            tx.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); tx.text = t; tx.fontSize = size; tx.fontStyle = FontStyle.Bold;
            tx.alignment = TextAnchor.MiddleCenter; tx.color = Color.white; tx.raycastTarget = false; return tx;
        }

        static RectTransform Bar(Transform p, string n, Vector2 anchor, Vector2 pos, Vector2 size, Color c, bool right)
        {
            var bg = Rect(n + "_BG", p, anchor, pos, size); bg.gameObject.AddComponent<Image>().color = new Color(0, 0, 0, .55f);
            var fill = Rect(n, bg, new Vector2(right ? 1 : 0, .5f), Vector2.zero, size); fill.anchorMin = new Vector2(0, 0); fill.anchorMax = new Vector2(1, 1);
            fill.offsetMin = fill.offsetMax = Vector2.zero; fill.pivot = new Vector2(right ? 1 : 0, .5f);
            fill.gameObject.AddComponent<Image>().color = c; return fill;
        }

        static void Btn(Transform p, string n, string label, TouchButton.Kind k, Vector2 anchor, Vector2 pos, float size)
        {
            var r = Rect(n, p, anchor, pos, new Vector2(size, size));
            var img = r.gameObject.AddComponent<Image>(); img.color = new Color(.2f, .9f, 1f, .35f);
            r.gameObject.AddComponent<TouchButton>().kind = k;
            Label(r, "Text", label, label.Length > 2 ? 32 : 80, new Vector2(.5f, .5f), Vector2.zero, new Vector2(size, size));
        }
    }
}

# Aether Blade — Unity + GitHub Actions APK

## 1. Scene / models: kuch banane ki zaroorat nahi!
Scene **code se khud ban jata hai** (ground, neon city, Kaito vs Valen capsule characters, health/Aether bars, touch buttons).

**Option A - Unity bina kholay (sabse aasan):** seedha step 2 par jao. GitHub build ke time scene khud banega aur APK ban jayega.

**Option B - Unity mein dekhna ho:** Unity Hub se project open karo, upar menu **Aether Blade > Build Demo Scene** click karo, Play dabao. (PC par A/D, W, J, K keys.)

Abhi characters simple capsules hain (placeholder). Gameplay poora kaam karta hai. Baad mein real 3D model lagana ho:
1. mixamo.com (free Adobe login) -> koi character + animations (Idle, Run, Punch/Slash, Hit, Death) FBX download.
2. Unity mein FBX drag karo -> Rig > **Humanoid**.
3. Model ko Kaito ka child banao aur capsule ka mesh band karo, Animator mein parameters: Speed, Grounded, Attack1-3, Ultimate, Hurt, Die.
(Ye step mushkil lage to mujhse bolo, main Animator setup script bhi bana dunga.)

## 2. GitHub par push
```
git init && git add . && git commit -m "init"
git branch -M main
git remote add origin https://github.com/USERNAME/REPO.git
git push -u origin main
```

## 3. Unity license secrets (zaroori)
Repo > Settings > Secrets and variables > Actions mein 3 secrets:
- `UNITY_EMAIL`, `UNITY_PASSWORD` — Unity account.
- `UNITY_LICENSE` — Personal license ki `.ulf` file ka content. Guide: https://game.ci/docs/github/activation

## 4. APK build karo
- Actions tab > **Build Android APK** > Run workflow, ya
- `git tag v1.0.0 && git push --tags` -> APK **Releases** mein khud aa jayega.

## 5. Host karna
- `docs/index.html` mein `USERNAME/REPO` badlo.
- Repo > Settings > Pages > Source: `main` branch, folder `/docs`.
- Aapka download page: `https://USERNAME.github.io/REPO/` (button latest APK release par le jata hai).

## 6. Kotlin bridge
`android-bridge/NativeBridge.kt` ko Android Studio library module mein compile karke `.aar` banao, `Assets/Plugins/Android/` mein rakho.

## Notes
- APK debug-signed hai (sideload ke liye theek). Play Store ke liye keystore secrets add karne honge.
- Low-end devices: `AssetStreamer` + Addressables, ASTC texture compression, IL2CPP ARM64, 60 FPS target.

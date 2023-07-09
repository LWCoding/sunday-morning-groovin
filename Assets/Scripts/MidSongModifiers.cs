using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MidSongModifiers : MonoBehaviour
{

    private Color priorTint;
    private Color priorColor;
    private Sprite initialArrowSprite;
    public GameObject heartPrefab;
    public Sprite darkHeart;
    public GameObject particle;
    int heartCount = 0;
    private int quality = 0;
    public bool flicker = false;
    public bool isSwapped = false;
    private LevelHandler levelHandler;
    private PublicVariables p;
    public AudioClip electricShockSFX;
    private WaitForSeconds wfs;

    public void Start() {
        quality = PlayerPrefs.GetInt("Quality");
        levelHandler = GameObject.Find("GameController").GetComponent<LevelHandler>();
        p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
    }

    public void LoadModifier(string code) {
        switch (code) {
            case "00":
                StartCoroutine(FlickerLightsOff());
                break;
            // A case 01 should always be followed after a case 00 to ensure proper color formats
            case "01":
                StartCoroutine(FlickerLightsOn());
                break;
            case "02":
                if (quality == 1) { return; }
                StartCoroutine(Earthquake());
                break;
            case "03":
                AddHeart();
                break;
            case "04":
                RemoveHeart();
                break;
            case "05":
                DarkenHearts();
                break;
            case "06":
                if (quality == 1) { return; }
                flicker = true;
                StartCoroutine(ContinuousFlicker());
                break;
            case "07":
                if (quality == 1) { return; }
                flicker = false;
                break;
            case "08":
                StartCoroutine(CameraZoom());
                break;
            case "09":
                StartCoroutine(CameraUnzoom());
                break;
            case "10":
                StartCoroutine(SwapArrows());
                break;
            case "11":
                for (int i = 0; i < 11; i++) {
                    PlayerPrefs.SetInt("Week" + i + "Complete", 1);
                }
                break;
            case "12":
                StartCoroutine(DepleteHealth(35));
                break;
            case "13":
                StartCoroutine(EnemyFly(true));
                break;
            case "14":
                StartCoroutine(AdrianFry());
                break;
            case "15":
                StartCoroutine(EnemyFly(false));
                break;
            case "16":
                StartCoroutine(Intensity());
                break;
            case "17":
                StartCoroutine(StopIntensity());
                break;
            case "18":
                StartCoroutine(MidIntensity());
                break;
            case "19":
                StartCoroutine(StopMidIntensity());
                break;
        }
    }

    IEnumerator Intensity() {
        levelHandler.bumpFreq = 1;
        levelHandler.bumpStrength *= 2;
        levelHandler.tempo /= 2;
        CameraScale c = GameObject.Find("Main Camera").GetComponent<CameraScale>();
        float delay = 0.012f;
        for (int i = 0; i < 20; i++) {
            c.sceneWidth -= 0.09f;
            wfs = new WaitForSeconds(delay);
            yield return wfs;
            delay += 0.001f;
        }
    }

    IEnumerator StopIntensity() {
        levelHandler.bumpFreq = 4;
        levelHandler.bumpStrength /= 2;
        levelHandler.tempo *= 2;
        CameraScale c = GameObject.Find("Main Camera").GetComponent<CameraScale>();
        float delay = 0.012f;
        for (int i = 0; i < 20; i++) {
            c.sceneWidth += 0.09f;
            wfs = new WaitForSeconds(delay);
            yield return wfs;
            delay += 0.001f;
        }
    }

    IEnumerator MidIntensity() {
        levelHandler.bumpFreq = 1;
        levelHandler.bumpStrength *= 2;
        CameraScale c = GameObject.Find("Main Camera").GetComponent<CameraScale>();
        float delay = 0.012f;
        for (int i = 0; i < 20; i++) {
            c.sceneWidth -= 0.05f;
            wfs = new WaitForSeconds(delay);
            yield return wfs;
            delay += 0.001f;
        }
    }

    IEnumerator StopMidIntensity() {
        levelHandler.bumpFreq = 4;
        levelHandler.bumpStrength /= 2;
        CameraScale c = GameObject.Find("Main Camera").GetComponent<CameraScale>();
        float delay = 0.012f;
        for (int i = 0; i < 20; i++) {
            c.sceneWidth += 0.05f;
            wfs = new WaitForSeconds(delay);
            yield return wfs;
            delay += 0.001f;
        }
    }


    IEnumerator AdrianFry() {
        GameObject enemy = GameObject.Find("Enemy");
        levelHandler.tempo = 999;
        levelHandler.enemyNotes += 99;
        enemy.GetComponent<Animator>().enabled = true;
        enemy.GetComponent<Animator>().Play("FriedChicken");
        wfs = new WaitForSeconds(4.6f);
        yield return wfs;
        p.musicPlayer.PlayOneShot(electricShockSFX);
        wfs = new WaitForSeconds(1.4f);
        yield return wfs;
        enemy.GetComponent<Animator>().enabled = false;
    }

    IEnumerator EnemyFly(bool missBeforeFlying) {
        GameObject enemy = GameObject.Find("Enemy");
        levelHandler.enemyNotes += 99;
        if (missBeforeFlying) {
            enemy.GetComponent<SpriteHandler>().resetDelay = 1.5f;
            enemy.GetComponent<SpriteHandler>().Miss();
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 3; j++) {
                    enemy.transform.position += new Vector3(0.03f, 0, 0);
                    wfs = new WaitForSeconds(0.015f);
                    yield return wfs;
                }
                for (int j = 0; j < 3; j++) {
                    enemy.transform.position -= new Vector3(0.03f, 0, 0);
                    wfs = new WaitForSeconds(0.015f);
                    yield return wfs;
                }
            }
        }
        wfs = new WaitForSeconds(0.7f);
        yield return wfs;
        p.transitionSelection = -3;
        float delay = 0.02f;
        for (int i = 0; i < 30; i++) {
            enemy.transform.position += new Vector3(-0.22f, 0.4f, 0);
            if (quality != 1 && i % 2 == 0) {
                enemy.GetComponent<SpriteHandler>().MakeShadow();
            }
            wfs = new WaitForSeconds(delay);
            yield return wfs;
            delay += 0.001f;
        }
        levelHandler.enemyNotes -= 99;
    }

    IEnumerator DepleteHealth(int amount) {
        for (int i = 0; i < amount; i++) {
            p.Deplete(1, false);
            CreateParticle(new Color(0.9f, 0.46f, 0.9f), GameObject.Find("PlayerIcon").transform.position);
            wfs = new WaitForSeconds(0.2f);
            yield return wfs;
        }
    }

    void CreateParticle(Color c, Vector3 pos, bool isCanvas = true) {
        if (quality == 1) { return; }
        GameObject p = Instantiate(particle);
        if (isCanvas) {
            p.transform.SetParent(GameObject.Find("Canvas").transform);
        } 
        p.GetComponent<Image>().color = c;
        p.transform.position = pos;
    }

    IEnumerator SwapArrows() {
        GameObject pArrows = GameObject.Find("PlayerArrows");
        int mult = (isSwapped) ? -1 : 1;
        float scale = (GameObject.Find("Canvas").GetComponent<RectTransform>().rect.width * GameObject.Find("Canvas").transform.localScale.x) / 1280;
        float delay = 0.01f;
        for (int i = 0; i < 20; i++) {
            pArrows.transform.GetChild(2).position -= new Vector3(0, 5 * scale, 0);
            pArrows.transform.GetChild(3).position -= new Vector3(0, 5 * scale, 0);
            if (i % 5 == 0) {
                CreateParticle(new Color(0.9f, 0.46f, 0.9f), pArrows.transform.GetChild(2).position);
                CreateParticle(new Color(0.9f, 0.46f, 0.9f), pArrows.transform.GetChild(3).position);
            }
            wfs = new WaitForSeconds(delay);
            yield return wfs;
            delay += 0.001f;
        }
        wfs = new WaitForSeconds(0.3f);
        yield return wfs;
        delay = 0.01f;
        for (int i = 0; i < 20; i++) {
            pArrows.transform.GetChild(0).position += new Vector3(11 * scale * mult, 0, 0);
            pArrows.transform.GetChild(1).position += new Vector3(11 * scale * mult, 0, 0);
            pArrows.transform.GetChild(2).position -= new Vector3(11 * scale * mult, 0, 0);
            pArrows.transform.GetChild(3).position -= new Vector3(11 * scale * mult, 0, 0);
            if (i % 5 == 0) {
                CreateParticle(new Color(0.9f, 0.46f, 0.9f), pArrows.transform.GetChild(0).position);
                CreateParticle(new Color(0.9f, 0.46f, 0.9f), pArrows.transform.GetChild(1).position);
                CreateParticle(new Color(0.9f, 0.46f, 0.9f), pArrows.transform.GetChild(2).position);
                CreateParticle(new Color(0.9f, 0.46f, 0.9f), pArrows.transform.GetChild(3).position);
            }
            wfs = new WaitForSeconds(delay);
            yield return wfs;
            delay += 0.001f;
        }
        wfs = new WaitForSeconds(0.3f);
        yield return wfs;
        delay = 0.01f;
        for (int i = 0; i < 20; i++) {
            pArrows.transform.GetChild(2).position += new Vector3(0, 5 * scale, 0);
            pArrows.transform.GetChild(3).position += new Vector3(0, 5 * scale, 0);
            if (i % 5 == 0) {
                CreateParticle(new Color(0.9f, 0.46f, 0.9f), pArrows.transform.GetChild(2).position);
                CreateParticle(new Color(0.9f, 0.46f, 0.9f), pArrows.transform.GetChild(3).position);
            }
            wfs = new WaitForSeconds(delay);
            yield return wfs;
            delay += 0.001f;
        }
        isSwapped = !isSwapped;
    }

    IEnumerator CameraZoom() {
        p.isZoomedIn = true;
        p.CameraToEnemy();
        CameraScale c = GameObject.Find("Main Camera").GetComponent<CameraScale>();
        float delay = 0.008f;
        GameObject spotlight = GameObject.Find("Spotlight");
        spotlight.GetComponent<SpriteRenderer>().enabled = true;
        spotlight.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
        for (int i = 0; i < 20; i++) {
            GameObject.Find("Main Camera").transform.position -= new Vector3(0.1f, 0, 0);
            c.sceneWidth -= 0.3f;
            spotlight.GetComponent<SpriteRenderer>().color += new Color32(0, 0, 0, 5);
            wfs = new WaitForSeconds(delay);
            yield return wfs;
            delay += 0.0008f;
        }
    }

    IEnumerator CameraUnzoom() {
        CameraScale c = GameObject.Find("Main Camera").GetComponent<CameraScale>();
        float delay = 0.012f;
        GameObject spotlight = GameObject.Find("Spotlight");
        for (int i = 0; i < 20; i++) {
            GameObject.Find("Main Camera").transform.position += new Vector3(0.1f, 0, 0);
            c.sceneWidth += 0.3f;
            spotlight.GetComponent<SpriteRenderer>().color -= new Color32(0, 0, 0, 5);
            wfs = new WaitForSeconds(delay);
            yield return wfs;
            delay += 0.001f;
        }
        p.isZoomedIn = false;
        spotlight.GetComponent<SpriteRenderer>().enabled = false;
        spotlight.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
    }

    IEnumerator ContinuousFlicker() {
        GameObject darkness = GameObject.Find("Darkness");
        darkness.GetComponent<SpriteRenderer>().enabled = true;
        darkness.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
        float delay = 0.1f;
        for (int i = 0; i < 16; i++) {
            delay -= 0.005f;
            darkness.GetComponent<SpriteRenderer>().color += new Color32(0, 0, 0, 4);
            wfs = new WaitForSeconds(delay);
            yield return wfs;
        }
        while (true) {
            Color c = darkness.GetComponent<SpriteRenderer>().color;
            delay = 0.1f;
            for (int i = 0; i < 15; i++) {
                delay -= 0.005f;
                darkness.GetComponent<SpriteRenderer>().color += new Color32(0, 0, 0, 4);
                wfs = new WaitForSeconds(delay);
                yield return wfs;
            }
            wfs = new WaitForSeconds(0.25f);
            yield return wfs;
            delay = 0.1f;
            for (int i = 0; i < 15; i++) {
                delay -= 0.005f;
                darkness.GetComponent<SpriteRenderer>().color -= new Color32(0, 0, 0, 4);
                wfs = new WaitForSeconds(delay);
                yield return wfs;
            }
            wfs = new WaitForSeconds(0.25f);
            yield return wfs;
            if (!flicker) {
                break;
            }
            yield return null;
        }
        delay = 0.1f;
        for (int i = 0; i < 16; i++) {
            delay -= 0.005f;
            darkness.GetComponent<SpriteRenderer>().color -= new Color32(0, 0, 0, 4);
            wfs = new WaitForSeconds(delay);
            yield return wfs;
        }
        darkness.GetComponent<SpriteRenderer>().enabled = false;
    }

    void AddHeart() {
        heartCount++;
        GameObject h = Instantiate(heartPrefab);
        h.transform.name = "Heart" + heartCount;
        h.transform.position = new Vector3(-4 + (heartCount - 1) * 0.9f, 1, 0);
    }

    void RemoveHeart() {
        Destroy(GameObject.Find("Heart" + heartCount));
        heartCount--;
    }

    void DarkenHearts() {
        for (int i = 1; i <= heartCount; i++) {
            GameObject.Find("Heart" + i.ToString()).GetComponent<SpriteRenderer>().sprite = darkHeart;
        }
    }

    IEnumerator FlickerLightsOff() {
        GameObject darkness = GameObject.Find("Darkness");
        darkness.GetComponent<SpriteRenderer>().enabled = true;
        GameObject enemy = GameObject.Find("Enemy");
        Color tint = enemy.GetComponent<SpriteRenderer>().color;
        Color c = darkness.GetComponent<SpriteRenderer>().color;
        priorTint = tint;
        priorColor = c;
        float firstDelay = 0.2f;
        float secondDelay = 0.14f;
        for (int i = 0; i < 3; i++) {
            darkness.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.8f);
            enemy.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
            wfs = new WaitForSeconds(firstDelay);
            yield return wfs;
            darkness.GetComponent<SpriteRenderer>().color = c;
            enemy.GetComponent<SpriteRenderer>().color = tint;
            wfs = new WaitForSeconds(secondDelay);
            yield return wfs;
            firstDelay -= 0.03f;
            secondDelay -= 0.03f;
        }
        darkness.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.8f);
        enemy.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
    }

    IEnumerator FlickerLightsOn() {
        GameObject darkness = GameObject.Find("Darkness");
        GameObject enemy = GameObject.Find("Enemy");
        Color tint = priorTint;
        Color c = priorColor;
        float firstDelay = 0.2f;
        float secondDelay = 0.14f;
        for (int i = 0; i < 3; i++) {
            darkness.GetComponent<SpriteRenderer>().color = c;
            enemy.GetComponent<SpriteRenderer>().color = tint;
            wfs = new WaitForSeconds(firstDelay);
            yield return wfs;
            darkness.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.8f);
            enemy.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
            wfs = new WaitForSeconds(secondDelay);
            yield return wfs;
            firstDelay -= 0.03f;
            secondDelay -= 0.03f;
        }
        darkness.GetComponent<SpriteRenderer>().color = c;
        enemy.GetComponent<SpriteRenderer>().color = tint;
        darkness.GetComponent<SpriteRenderer>().enabled = false;
    }

    IEnumerator Earthquake() {
        for (int i = 0; i < 10; i++) {
            p.ShakeScreen();
            wfs = new WaitForSeconds(0.1f);
            yield return wfs;
        }
    }

}

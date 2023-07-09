using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleNavigate : MonoBehaviour
{
    public int currentButton = 1;
    public float lastFlashUpdate = 0.0f;
    public float flashCooldown = 0.1f;
    private float thumbnailOffset = -0.8f;
    private float thumbnailOriginalY = 0;
    public int predeterminedX;
    private bool flash = false;
    private bool interactionsAllowed = false;
    private int buttonCount;
    private Vector2 resolution;
    public AudioClip menuLoop;
    private GameObject thumbnail;
    private GameObject smgIcon;
    private GameObject introCover;
    private GameObject introText;
    private GameObject introLogo;
    public GameObject introScene;
    private PublicVariables p;
    public float bumpDelay = 0.555f;

    void Awake() {
        p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        smgIcon = GameObject.Find("Logo");
        thumbnail = GameObject.Find("Thumbnail");
        if (PlayerPrefs.GetInt("IntroSkip") == 1) {
            p.alreadyLoadedGame = true;
        }
        Vector2 d = p.GetDimensions();
        resolution = new Vector2(d.x, d.y);
    }

    public void Start() {
        predeterminedX = 75;
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Option");
        buttonCount = buttons.Length;
        thumbnailOriginalY = GameObject.Find("Thumbnail").transform.position.y;
        GameObject.Find("Thumbnail").transform.position += new Vector3(0, thumbnailOffset, 0);
        AudioSource musicPlayer = GameObject.Find("Main Audio Source").GetComponent<AudioSource>();
        musicPlayer.loop = true;
        musicPlayer.clip = menuLoop;
        musicPlayer.Play();
        if (!p.alreadyLoadedGame) {
            introScene.SetActive(true);
            introCover = GameObject.Find("IntroCover");
            introLogo = GameObject.Find("IntroLogo");
            introText = GameObject.Find("CoverText");
            StartCoroutine(BeginningAnimation());
        }
        if (p.alreadyLoadedGame) {
            musicPlayer.time = 8.3f;
            interactionsAllowed = true;
            StartCoroutine(IconBump());
            StartCoroutine(ThumbnailBump());
        } else {
            p.alreadyLoadedGame = true;
        }
        UpdateButton(false);
        StartCoroutine(ButtonUpdate());
    }

    IEnumerator BeginningAnimation() {
        AudioSource musicPlayer = GameObject.Find("Main Audio Source").GetComponent<AudioSource>();
        IEnumerator logoBump = IntroLogoBump();
        StartCoroutine(logoBump);
        StartCoroutine(IconBump());
        StartCoroutine(ThumbnailBump());
        introText.GetComponent<TextMeshProUGUI>().text = "";
        yield return new WaitForSeconds(1.63f);
        introText.GetComponent<TextMeshProUGUI>().text = "You are playing\n\n\n\n\n";
        yield return new WaitForSeconds(2);
        introText.GetComponent<TextMeshProUGUI>().text = "You are playing\n\nSunday Morning Groovin'\n\n\n";
        yield return new WaitForSeconds(2);
        introText.GetComponent<TextMeshProUGUI>().text = "You are playing\n\nSunday Morning Groovin'\n\n(It's a fun game)";
        yield return new WaitForSeconds(7.62f - musicPlayer.time);
        float incAmount = 0.18f;
        for (int i = 0; i < 10; i++) {
            introText.transform.localScale += new Vector3(incAmount, incAmount, 0);
            incAmount -= 0.01f;
            yield return new WaitForSeconds(0.03f);
        }
        incAmount = 0.1f;
        for (int i = 0; i < 20; i++) {
            if (introText.transform.localScale.x - incAmount <= 0) { introText.SetActive(false); break; }
            introText.transform.localScale -= new Vector3(incAmount, incAmount, 0);
            incAmount += 0.02f;
            yield return new WaitForSeconds(0.025f);
        }
        StopCoroutine(logoBump);
        for (int i = 0; i < 20; i++) {
            introLogo.GetComponent<Image>().color -= new Color(0, 0, 0, 0.01f);
            introCover.GetComponent<Image>().color -= new Color(0, 0, 0, 0.05f);
            yield return new WaitForSeconds(0.01f);
        }
        introScene.SetActive(false);
        interactionsAllowed = true;
        yield return new WaitForSeconds(0.2f);
        p.RenderLoginAchievement();
    }

    IEnumerator IntroLogoBump() {
        introLogo.transform.localScale += new Vector3(0.02f, 0.02f, 0);
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 10; i++) {
            introLogo.transform.localScale -= new Vector3(0.002f, 0.002f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(bumpDelay);
        StartCoroutine(IntroLogoBump());
    }

    IEnumerator IconBump() {
        smgIcon.transform.localScale += new Vector3(0.05f, 0.05f, 0);
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < 10; i++) {
            smgIcon.transform.localScale -= new Vector3(0.005f, 0.005f, 0);
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(bumpDelay / 2);
        StartCoroutine(IconBump());
    }

    IEnumerator ThumbnailBump() {
        thumbnail.transform.localScale += new Vector3(0.015f, 0.015f, 0);
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 10; i++) {
            thumbnail.transform.localScale -= new Vector3(0.0015f, 0.0015f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(bumpDelay);
        StartCoroutine(ThumbnailBump());
    }

    // This is for WebGL so it loads correctly
    IEnumerator ButtonUpdate() {
        yield return new WaitForSeconds(0.01f);
        UpdateButton(false);
    }

    public void FixedUpdate() {
        if (flash) {
            lastFlashUpdate += Time.deltaTime;
            GameObject button = GameObject.Find("Button" + currentButton);
            if (lastFlashUpdate > flashCooldown) {
                if (button.GetComponent<Text>().color.a > 0.2f) {
                    button.GetComponent<Text>().color = new Color(255, 255, 255, 0.2f);
                } else {
                    button.GetComponent<Text>().color = new Color(255, 255, 255, 0.75f);
                }
                lastFlashUpdate = 0;
            }
        }
        // Make thumbnail move with selections
        Vector3 t = GameObject.Find("Thumbnail").transform.position;
        if (Mathf.Abs(t.y - (thumbnailOriginalY + thumbnailOffset)) > 0.1f) {
            if (t.y < thumbnailOriginalY + thumbnailOffset) {
                GameObject.Find("Thumbnail").transform.position += new Vector3(0, 0.1f, 0);
            } else {
                GameObject.Find("Thumbnail").transform.position -= new Vector3(0, 0.1f, 0);
            }
        }
    }

    private void ScrollUp() {
        currentButton -= 1;
        if (currentButton <= 0) {
            currentButton = buttonCount;
            thumbnailOffset = 0.6f;
            GameObject.Find("Thumbnail").transform.position = new Vector3(GameObject.Find("Thumbnail").transform.position.x, thumbnailOriginalY + thumbnailOffset, 0);
        } else {
            thumbnailOffset -= 0.3f;
        }
        UpdateButton(true);
    }

    private void ScrollDown() {
        currentButton += 1;
        if (currentButton > buttonCount) {
            currentButton = 1;
            thumbnailOffset = -0.6f;
            GameObject.Find("Thumbnail").transform.position = new Vector3(GameObject.Find("Thumbnail").transform.position.x, thumbnailOriginalY + thumbnailOffset, 0);
        } else {
            thumbnailOffset += 0.3f;
        }
        UpdateButton(true);
    }

    public void Update() {
        Vector2 dim = p.GetDimensions();
        if (resolution.x != dim.x || resolution.y != dim.y) {
            UpdateButton(false);
            resolution.x = dim.x;
            resolution.y = dim.y;
        }
        if (flash || !interactionsAllowed) { return; }
        if (Input.GetKeyDown("w") || Input.GetKeyDown("up")) {
            ScrollUp();
        } else if (Input.GetKeyDown("s") || Input.GetKeyDown("down")) {
            ScrollDown();
        } else if (Input.GetKeyDown("space") || Input.GetKeyDown("return")) {
            StartCoroutine(pickChoice());
        }
    }

    IEnumerator pickChoice() {
        p.ConfirmSound();
        flash = true;
        yield return new WaitForSeconds(1);
        switch (currentButton) {
                case 1:
                    SceneManager.LoadScene("StorySelect");
                    break;
                case 2:
                    SceneManager.LoadScene("Gallery");
                    break;
                case 3:
                    SceneManager.LoadScene("Cosmetics");
                    break;
                case 4:
                    SceneManager.LoadScene("Options");
                    break;
                case 5:
                    SceneManager.LoadScene("Achieve");
                    break;
                case 6:
                    SceneManager.LoadScene("Credits");
                    break;
            }
    }

    void UpdateButton(bool makeSound) {
        int staticX = (int)(predeterminedX * GameObject.Find("Canvas").transform.localScale.x);
        GameObject main = GameObject.Find("Button" + currentButton);
        main.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        main.GetComponent<Text>().color = new Color(255, 255, 255, 0.9f);
        main.transform.position = new Vector3(staticX, (GameObject.Find("Canvas").GetComponent<RectTransform>().rect.height * GameObject.Find("Canvas").transform.localScale.y) / 2, main.transform.position.z);
        for (int i = 1; i < buttonCount + 1; i++) {
            GameObject button = GameObject.Find("Button" + i);
            Vector3 pos = GameObject.Find("Button" + i).transform.position;
            if (i != currentButton) {
                float diff = Mathf.Abs(i - currentButton) * 0.25f;
                button.transform.localScale = new Vector3(1 - diff, 1 - diff, 1);
                button.transform.position = new Vector3(staticX, main.transform.position.y - ((i - currentButton) * 150 * GameObject.Find("Canvas").transform.localScale.y), pos.z);;
                button.GetComponent<Text>().color = new Color(255, 255, 255, 0.4f);
            }
        }
        if (makeSound) { p.ScrollSound(); }
    }

}

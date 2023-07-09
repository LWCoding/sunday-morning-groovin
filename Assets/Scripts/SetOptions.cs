using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetOptions : MonoBehaviour
{

    public int optionSelected = 1;
    public float predeterminedX;
    private float staticX;
    public int xSelectedOffset;
    private float lastUpdate = 0;
    public float updateCooldown;
    private int buttonCount = 0;
    private Vector2 resolution;

    void Awake() {
        Vector2 d = GameObject.Find("PublicVariables").GetComponent<PublicVariables>().GetDimensions();
        resolution = new Vector2(d.x, d.y);
    }

    void LoadButtons() {
        for (int i = 1; i < buttonCount + 1; i++) {
            optionSelected = i;
            UpdateText();
        }
        optionSelected = 1;
    }
    
    void Start() {
        buttonCount = GameObject.FindGameObjectsWithTag("Option").Length;
        LoadButtons();
        staticX = predeterminedX * GameObject.Find("Canvas").transform.localScale.x;
        UpdateButton();
    }
    
    IEnumerator WaitAndUpdate() {
        yield return new WaitForSeconds(0.01f);
        UpdateButton();
    }

    private void ScrollUp() {
        optionSelected -= 1;
        if (optionSelected < 1) {
            optionSelected = buttonCount;
        }
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().ScrollSound();
        UpdateButton();
    }

    private void ScrollDown() {
        optionSelected += 1;
        if (optionSelected > buttonCount) {
            optionSelected = 1;
        }
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().ScrollSound();
        UpdateButton();
    }

    private void UpdateOption(bool backwards = false) {
        string sel = GameObject.Find("Option" + optionSelected).GetComponent<Data>().data;
        GameObject text = GameObject.Find("Description");
        switch (sel) {
            case "lenient":
                PlayerPrefs.SetInt("LenientPress", PlayerPrefs.GetInt("LenientPress") ^ 1);
                break;
            case "accuracy":
                PlayerPrefs.SetInt("Accuracy", PlayerPrefs.GetInt("Accuracy") ^ 1);
                break;
            case "cutscenes":
                PlayerPrefs.SetInt("Cutscenes", PlayerPrefs.GetInt("Cutscenes") ^ 1);
                break;
            case "downscroll":
                PlayerPrefs.SetInt("Downscroll", PlayerPrefs.GetInt("Downscroll") ^ 1);
                break;
            case "duetmode":
                PlayerPrefs.SetInt("DuetMode", PlayerPrefs.GetInt("DuetMode") ^ 1);
                break;
            case "keybinds":
                PlayerPrefs.SetInt("Keybinds", PlayerPrefs.GetInt("Keybinds") ^ 1);
                GameObject.Find("PublicVariables").GetComponent<PublicVariables>().UpdateKeys();
                break;
            case "offset":
                float newValue;
                if (backwards) {
                    newValue = Mathf.Round((PlayerPrefs.GetFloat("ArrowOffset") + 0.02f) * 100) / 100;
                } else {
                    newValue = Mathf.Round((PlayerPrefs.GetFloat("ArrowOffset") - 0.02f) * 100) / 100;
                }
                if (newValue > 0.2f) {
                    newValue = -0.2f;
                }
                if (newValue < -0.2f) {
                    newValue = 0.2f;
                }
                PlayerPrefs.SetFloat("ArrowOffset", newValue);
                break;
            case "quality":
                int calcValue;
                if (backwards) {
                    calcValue = PlayerPrefs.GetInt("Quality") - 1;
                } else {
                    calcValue = PlayerPrefs.GetInt("Quality") + 1; // 0 1 2
                }
                if (calcValue > 2) { calcValue = 0; }
                if (calcValue < 0) { calcValue = 2; }
                PlayerPrefs.SetInt("Quality", calcValue);
                break;
            case "introskip":
                PlayerPrefs.SetInt("IntroSkip", PlayerPrefs.GetInt("IntroSkip") ^ 1);
                break;
            case "globalspeed":
            int newValueTwo;
                if (backwards) {
                    newValueTwo = PlayerPrefs.GetInt("GlobalSpeed") - 1;
                } else { 
                    newValueTwo = PlayerPrefs.GetInt("GlobalSpeed") + 1;
                }
                if (newValueTwo == 11) { newValueTwo = 0; }
                else if (newValueTwo == 1) { newValueTwo = 12; }
                else if (newValueTwo < 12) { newValueTwo = 26; }
                else if (newValueTwo > 26) { newValueTwo = 0; }
                PlayerPrefs.SetInt("GlobalSpeed", newValueTwo);
                break;
            case "audibleclick":
                PlayerPrefs.SetInt("AudibleClick", PlayerPrefs.GetInt("AudibleClick") ^ 1);
                break;
        }
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().ScrollSound();
        UpdateText();
    }

    void Update() {
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        Vector2 dim = p.GetDimensions();
        if (resolution.x != dim.x || resolution.y != dim.y) {
            resolution.x = dim.x;
            resolution.y = dim.y;
            StartCoroutine(WaitAndUpdate());
        }
        if (Input.GetKeyDown("backspace") || Input.GetKeyDown("escape")) {
            SceneManager.LoadScene("Start");
        } else if (Input.GetKeyDown("up") || Input.GetKeyDown("w")) {
            ScrollUp();
        } else if (Input.GetKeyDown("down") || Input.GetKeyDown("s")) {
            ScrollDown();
        } else if (Input.GetKeyDown("space") || Input.GetKeyDown("return") || Input.GetKeyDown("right") || Input.GetKeyDown("d")) {
            UpdateOption(false);
        } else if (Input.GetKeyDown("left") || Input.GetKeyDown("a")) {
            UpdateOption(true);
        }
    }

    void UpdateButton() {
        staticX = predeterminedX * GameObject.Find("Canvas").transform.localScale.x;
        GameObject main = GameObject.Find("Option" + optionSelected);
        main.GetComponent<Text>().color = new Color(255, 255, 255, 0.9f);
        main.transform.position = new Vector3(main.transform.position.x, (GameObject.Find("Canvas").GetComponent<RectTransform>().rect.height * GameObject.Find("Canvas").transform.localScale.y) / 2, main.transform.position.z);
        for (int i = 1; i < buttonCount + 1; i++) {
            GameObject optionButton = GameObject.Find("Option" + i);
            Vector3 pos = GameObject.Find("Option" + i).transform.position;
            if (i != optionSelected) {
                optionButton.transform.position = new Vector3(staticX, main.transform.position.y - ((i - optionSelected) * 150 * GameObject.Find("Canvas").transform.localScale.y), pos.z);;
                optionButton.GetComponent<Text>().color = new Color(255, 255, 255, 0.4f);
            }
        }
        UpdateText();
    }

    void FixedUpdate() {
        lastUpdate += Time.deltaTime;
        if (lastUpdate > updateCooldown) {
            lastUpdate = 0;
            Vector3 pos = GameObject.Find("Option" + optionSelected).transform.position;
            if (pos.x < staticX + xSelectedOffset) {
                GameObject.Find("Option" + optionSelected).transform.position = new Vector3(pos.x + 4f, pos.y, pos.z);
            }
        }
    }

    void UpdateText() {
        string sel = GameObject.Find("Option" + optionSelected).GetComponent<Data>().data;
        GameObject text = GameObject.Find("Description");
        switch (sel) {
            case "duetmode":
                if (PlayerPrefs.GetInt("DuetMode") == 0) {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Unique Mode";
                    text.GetComponent<Text>().text = "Unique Mode (Default): The player and enemy both have unique charting for certain sections in a song.";
                } else {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Duet Mode";
                    text.GetComponent<Text>().text = "Duet Mode: You share the enemy's notes in a song. High scores and currency will not be affected.";
                }
                break;
            case "lenient":
                if (PlayerPrefs.GetInt("LenientPress") == 1) {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Lenient Tap";
                    text.GetComponent<Text>().text = "Lenient Tap: Clicking when there is no arrow to be clicked will not count as a miss, but points will be deducted.";
                } else {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Normal Tap";
                    text.GetComponent<Text>().text = "Normal Tap (Default): Clicking when there is no arrow to be clicked will count as a miss.";
                }
                break;
            case "accuracy":
                if (PlayerPrefs.GetInt("Accuracy") == 0) {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Perfect Acc";
                    text.GetComponent<Text>().text = "Perfect Accuracy (Default): Note hit accuracy is calculated by how perfectly each note was hit (how far away an arrow was before press).";
                } else {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Normal Acc";
                    text.GetComponent<Text>().text = "Normal Accuracy: Note hit accuracy is calculated by dividing hit notes by total notes.";
                }
                break;
            case "offset":
                text.GetComponent<Text>().text = "Note Offset (" + Mathf.Abs(PlayerPrefs.GetFloat("ArrowOffset")).ToString() + ((PlayerPrefs.GetFloat("ArrowOffset") > 0) ? " seconds later" : " seconds sooner") + "): Arrows will show up at a different offset of time.";
                break;
            case "globalspeed":
                text.GetComponent<Text>().text = "Global Note Speed (" + ((PlayerPrefs.GetInt("GlobalSpeed") == 0) ? "Off" : PlayerPrefs.GetInt("GlobalSpeed").ToString()) + "): Arrows in all songs will be at a set speed. 12 is the slowest, 26 is the fastest. Most hard songs are at 18.";
                break;
            case "cutscenes":
                if (PlayerPrefs.GetInt("Cutscenes") == 0) {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Show CS";
                    text.GetComponent<Text>().text = "Show Cutscenes (Default): Cutscenes will automatically play at the beginning of songs.";
                } else {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Hide CS";
                    text.GetComponent<Text>().text = "Hide Cutscenes: Cutscenes such as dialogue will never be displayed.";
                }
                break;
            case "downscroll":
                if (PlayerPrefs.GetInt("Downscroll") == 0) {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Upscroll";
                    text.GetComponent<Text>().text = "Upscroll (Default): Arrows will move in an upwards motion from the bottom of the screen.";
                } else {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Downscroll";
                    text.GetComponent<Text>().text = "Downscroll: Arrows will move in a downwards motion from the top of the screen.";
                }
                break;
            case "keybinds":
                if (PlayerPrefs.GetInt("Keybinds") == 0) {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "WASD Keys";
                    text.GetComponent<Text>().text = "WASD Keybinds (Default): To hit notes, use W A S D or the arrow keys.";
                } else {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "DFJK Keys";
                    text.GetComponent<Text>().text = "DFJK Keybinds: To hit notes, use D F J K or the arrow keys.";
                }
                break;
            case "quality":
                if (PlayerPrefs.GetInt("Quality") == 0) {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Best Quality";
                    text.GetComponent<Text>().text = "High Quality (Default): All particles are on to provide the best visual gameplay.";
                } else if (PlayerPrefs.GetInt("Quality") == 1) {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Low Quality";
                    text.GetComponent<Text>().text = "Low Quality: All special effects and particles are off.";
                } else {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "OK Quality";
                    text.GetComponent<Text>().text = "OK Quality: All particles are on, but reduced significantly.";
                }
                break;
            case "introskip":
                if (PlayerPrefs.GetInt("IntroSkip") == 0) {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Intro Scene";
                    text.GetComponent<Text>().text = "Show Intro Scene (Default): An animation will be played on starting up the game.";
                } else {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Skip Scene";
                    text.GetComponent<Text>().text = "Skip Intro Scene: Skip the animation when starting up the game.";
                }
                break;
            case "audibleclick":
                if (PlayerPrefs.GetInt("AudibleClick") == 0) {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Silent Click";
                    text.GetComponent<Text>().text = "Silent Clicks (Default): No additional sounds will be played when you successfully hit a note.";
                } else {
                    GameObject.Find("Option" + optionSelected).GetComponent<Text>().text = "Audible Click";
                    text.GetComponent<Text>().text = "Audible Clicks: Whenever you hit an arrow, you will hear a clicking noise to reinforce that you have hit a note.";
                }
                break;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{

    public int levelSelected = 1;
    public float predeterminedX;
    private float staticX;
    public int xSelectedOffset;
    private float lastUpdate = 0;
    public float updateCooldown;
    private float lastFlashUpdate = 0;
    public float flashCooldown;
    public string[] weekEnemies;
    public string[] weekPlayers;
    private bool flash = false;
    private bool canPlay = true;
    private int buttonCount = 0;
    private Vector2 resolution;
    private Vector2 initialPlayerPos;
    private Vector2 initialEnemyPos;
    public GameObject lockCover;

    void Awake() {
        Vector2 d = GameObject.Find("PublicVariables").GetComponent<PublicVariables>().GetDimensions();
        resolution = new Vector2(d.x, d.y);
    }

    void Start() {
        initialPlayerPos = GameObject.Find("PlayerPreview").transform.position;
        initialEnemyPos = GameObject.Find("EnemyPreview").transform.position;
        staticX = predeterminedX * GameObject.Find("Canvas").transform.localScale.x;
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().cameFromGallery = false;
        buttonCount = GameObject.FindGameObjectsWithTag("Song").Length;
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().songDifficulty = "normal";
        UpdateText();
        UpdateButton();
    }

    private void ScrollUp() {
        levelSelected -= 1;
        if (levelSelected < 1) {
            levelSelected = buttonCount;
        }
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().ScrollSound();
        UpdateText();
        UpdateButton();
    }

    private void ScrollDown() {
        levelSelected += 1;
        if (levelSelected > buttonCount) {
            levelSelected = 1;
        }
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().ScrollSound();
        UpdateText();
        UpdateButton();
    }

    private void DifficultyLeft() {
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        p.ScrollSound();
        string d = p.songDifficulty;
        switch(d) {
            case "normal":
                p.songDifficulty = "easy";
                break;
            case "hard":
                p.songDifficulty = "normal";
                break;
        }
        GameObject.Find("Difficulty").GetComponent<Text>().text = "<- " + p.songDifficulty.ToUpper() + " MODE ->";
        UpdateText();
    }

    private void DifficultyRight() {
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        p.ScrollSound();
        string d = p.songDifficulty;
        switch(d) {
            case "normal":
                p.songDifficulty = "hard";
                break;
            case "easy":
                p.songDifficulty = "normal";
                break;
            case "hard":
                #if UNITY_IPHONE
                    p.songDifficulty = "easy";
                #endif
                break;
        }
        GameObject.Find("Difficulty").GetComponent<Text>().text = "<- " + p.songDifficulty.ToUpper() + " MODE ->";
        UpdateText();
    }

    void Update() {
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        Vector2 dim = p.GetDimensions();
        if (resolution.x != dim.x || resolution.y != dim.y) {
            UpdateButton();
            resolution.x = dim.x;
            resolution.y = dim.y;
        }
        if (flash) { return; }
        #if UNITY_IPHONE
            MobileCheck();
        #endif
        #if UNITY_STANDALONE || UNITY_WEBGL
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                ScrollUp();
            } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                ScrollDown();
                UpdateButton();
            } else if (canPlay && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))) {
                StartCoroutine(startLevel());
            } else if (canPlay && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))) {
                DifficultyLeft();
            } else if (canPlay && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))) {
                DifficultyRight();
            } else if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)) {
                SceneManager.LoadScene("Start");
            }
        #endif
    }

    IEnumerator startLevel() {
        flash = true;
        string[] songs = GameObject.Find("PublicVariables").GetComponent<PublicVariables>().splitWeekSongs[levelSelected];
        for (int i = 1; i < songs.Length; i++) {
            GameObject.Find("PublicVariables").GetComponent<PublicVariables>().songQueue.Add(songs[i]);
        }
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().ConfirmSound();
        yield return new WaitForSeconds(1);
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().levelSelected = levelSelected;
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().song = songs[0];
        SceneManager.LoadScene("Game");
    }

    void FixedUpdate() {
        lastUpdate += Time.deltaTime;
        lastFlashUpdate += Time.deltaTime;
        if (flash) {
            GameObject stageButton = GameObject.Find("Stage" + levelSelected);
            if (lastFlashUpdate > flashCooldown) {
                if (stageButton.GetComponent<Text>().color.a > 0.2f) {
                    stageButton.GetComponent<Text>().color = new Color(255, 255, 255, 0.2f);
                } else {
                    stageButton.GetComponent<Text>().color = new Color(255, 255, 255, 0.75f);
                }
                lastFlashUpdate = 0;
            }
        }
        if (lastUpdate > updateCooldown) {
            lastUpdate = 0;
            Vector3 pos = GameObject.Find("Stage" + levelSelected).transform.position;
            if (pos.x < staticX + xSelectedOffset) {
                GameObject.Find("Stage" + levelSelected).transform.position = new Vector3(pos.x + 4f, pos.y, pos.z);
            }
        }
    }

    void UpdateButton() {
        staticX = predeterminedX * GameObject.Find("Canvas").transform.localScale.x;
        GameObject main = GameObject.Find("Stage" + levelSelected);
        main.GetComponent<Text>().color = new Color(255, 255, 255, 0.9f);
        main.transform.position = new Vector3(main.transform.position.x, (GameObject.Find("Canvas").GetComponent<RectTransform>().rect.height * GameObject.Find("Canvas").transform.localScale.y) / 2, main.transform.position.z);
        for (int i = 1; i < buttonCount + 1; i++) {
            GameObject stageButton = GameObject.Find("Stage" + i);
            Vector3 pos = GameObject.Find("Stage" + i).transform.position;
            stageButton.transform.position = new Vector3(staticX, pos.y, pos.z);
            if (i != levelSelected) {
                stageButton.transform.position = new Vector3(staticX, main.transform.position.y - ((i - levelSelected) * 150 * GameObject.Find("Canvas").transform.localScale.y), pos.z);;
                stageButton.GetComponent<Text>().color = new Color(255, 255, 255, 0.4f);
            }
        }

        GameObject player = GameObject.Find("PlayerPreview");
        GameObject enemy = GameObject.Find("EnemyPreview");
        List<Character> chars = GameObject.Find("PublicVariables").GetComponent<SongData>().characters;
        string p = weekPlayers[levelSelected - 1];
        string e = weekEnemies[levelSelected - 1];
        enemy.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + e + "Sprite/Idle");
        foreach (Character c in chars) {
            if (c.name == p) {
                if (c.skin != "Normal" && c.skin != "Chromatic") {
                    player.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + p + "Sprite/" + c.skin + "/Idle");
                } else {
                    player.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + p + "Sprite/Idle");
                }
                player.transform.position = initialPlayerPos + new Vector2(0, c.yOffset);
                player.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(c.spriteScale.x * -1, c.spriteScale.y, 1);
                if (PlayerPrefs.GetInt("Unlocked" + c.name) == 0) {
                    player.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
                } else {
                    player.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                }
            }
            if (c.name == e) {
                if (c.skin != "Normal" && c.skin != "Chromatic") {
                    enemy.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + e + "Sprite/" + c.skin + "/Idle");
                } else {
                    enemy.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + e + "Sprite/Idle");
                }
                enemy.transform.position = initialEnemyPos + new Vector2(0, c.yOffset);
                enemy.transform.position += new Vector3(0, 0, 1);
                enemy.GetComponent<SpriteRenderer>().transform.localScale = c.spriteScale;
                if (PlayerPrefs.GetInt("Unlocked" + c.name) == 0) {
                    enemy.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
                } else {
                    enemy.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                }
            }
        }

        string[] songs = GameObject.Find("PublicVariables").GetComponent<PublicVariables>().splitWeekSongs[levelSelected];
        string songText = "";
        for (int i = 0; i < songs.Length; i++) {
            if (songs[i] != "cryptic") {
                songText += songs[i];
            }
            if (i != songs.Length - 1 && songs[i] != "cryptic") {
                songText += "      ";
            }
        }
        GameObject.Find("SongNames").GetComponent<Text>().text = songText;
    }

    void UpdateText() {
        Song firstSongInWeek = GameObject.Find("PublicVariables").GetComponent<SongData>().GetSong(GameObject.Find("PublicVariables").GetComponent<PublicVariables>().splitWeekSongs[levelSelected][0]);
        GameObject[] bgs = GameObject.FindGameObjectsWithTag("Background");
        foreach (GameObject bg in bgs) {
            if (bg.name != firstSongInWeek.bgName + "BG") {
                bg.GetComponent<SpriteRenderer>().enabled = false;
            } else {
                bg.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        // if (levelSelected == 1 || PlayerPrefs.GetInt("Week" + (levelSelected - 1) + "Complete") == 1) {
        //     lockCover.SetActive(false);
        //     canPlay = true;
        // } else {
        //     lockCover.SetActive(true);
        //     canPlay = false;
        // }
        GameObject.Find("HighScore").GetComponent<Text>().text = "High Score: " + PlayerPrefs.GetInt("Week" + levelSelected + "HS" + GameObject.Find("PublicVariables").GetComponent<PublicVariables>().GetDifficulty());
    }

}

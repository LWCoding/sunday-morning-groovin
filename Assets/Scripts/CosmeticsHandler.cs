using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CosmeticsHandler : MonoBehaviour
{

    private SongData s;
    private int currChar = 0;
    private Vector3 originalMainPlacement;
    private List<string> blacklistCharacters = new List<string>();
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    private int currentSkin = 0;
    private int currentSkinCost = 0;
    private bool canSwitchSkins = false;
    private PublicVariables p;

    void Awake() {
        p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        s = GameObject.Find("PublicVariables").GetComponent<SongData>();
        originalMainPlacement = GameObject.Find("MainSprite").transform.position;
        blacklistCharacters.Add("NULL");
        blacklistCharacters.Add("Floof");
        blacklistCharacters.Add("Henry");
        blacklistCharacters.Add("Camo&Lone");
        blacklistCharacters.Add("LoneAngry");
        blacklistCharacters.Add("Ariel");
        blacklistCharacters.Add("AriongUs");
        blacklistCharacters.Add("JakobHuman");
        blacklistCharacters.Add("Jackob");
        blacklistCharacters.Add("JackobAngry");
        blacklistCharacters.Add("JackobShadow");
        blacklistCharacters.Add("PatchesCrazy");
    }

    void Start() {
        GameObject.Find("Currency").GetComponent<Text>().text = PlayerPrefs.GetInt("money").ToString();
        LoadSprites(true, true);
        // PlayerPrefs.SetInt("UnlockedJackobAngryBSides", 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace)) {
            SceneManager.LoadScene("Start");
        }   
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            currentSkin = 0;
            currChar--;
            if (currChar < 0) {
                currChar = s.characters.Count - 1;
            }
            LoadSprites(false, true);
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            currentSkin = 0;
            currChar++;
            if (currChar > s.characters.Count - 1) {
                currChar = 0;
            }
            LoadSprites(true, true);
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) && canSwitchSkins) {
            Character c = s.characters[currChar];
            if (c.skins.Count > 1) {
                p.ScrollSound();
                currentSkin--;
                if (currentSkin < 0) {
                    currentSkin = c.skins.Count - 1;
                }
                LoadSprites(true, false);
            }
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) && canSwitchSkins) {
            Character c = s.characters[currChar];
            if (c.skins.Count > 1) {
                p.ScrollSound();
                currentSkin++;
                if (currentSkin > c.skins.Count - 1) {
                    currentSkin = 0;
                }
                LoadSprites(true, false);
            }
        }
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && currentSkinCost != 0) {
            if (PlayerPrefs.GetInt("money") > currentSkinCost) {
                Character c = s.characters[currChar];
                PlayerPrefs.SetInt("Unlocked" + c.name + c.skins[currentSkin].ToString(), 1);
                PlayerPrefs.SetInt("money", PlayerPrefs.GetInt("money") - currentSkinCost);
                p.achievements[2].Increment(currentSkinCost);
                while (p.achievements[2].NextLevel()) {
                    p.RenderAchievement(p.achievements[2].name, p.achievements[2].description);
                }
                currentSkinCost = 0;
                GameObject.Find("Currency").GetComponent<Text>().text = PlayerPrefs.GetInt("money").ToString();
                p.ConfirmSound();
                LoadSprites(true, false, false);
            } else {
                p.ScrollSound();
            }
        }
    }

    IEnumerator ChangeColor(string currSelection = "") {
        currSelection = currChar.ToString() + currentSkin.ToString();
        float h, s, v;
        Color.RGBToHSV(GameObject.Find("MainSprite").GetComponent<SpriteRenderer>().color, out h, out s, out v);
        if (h >= 1) {
            h = 0;
        }
        GameObject.Find("MainSprite").GetComponent<SpriteRenderer>().color = Color.HSVToRGB(h + 0.01f, 0.45f, v);
        yield return new WaitForSeconds(0.06f);
        if (currSelection == currChar.ToString() + currentSkin.ToString() && currentSkin != 0) {
            StartCoroutine(ChangeColor(currSelection));
        }   
    }

    void SetCharacter() {
        Character c = s.characters[currChar];
        c.skin = c.skins[currentSkin];
        PlayerPrefs.SetInt(c.name + "Skin", currentSkin);
    }
    
    void LoadSprites(bool scrollRight, bool firstLoad, bool playSound = true) {
        GameObject.Find("MainSprite").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        GameObject main = GameObject.Find("MainSprite");
        Character c = s.characters[currChar];
        if (playSound) {
            p.ScrollSound();
        }
        while (blacklistCharacters.Contains(c.name)) {
            if (scrollRight) {
                currChar++;
                if (currChar > s.characters.Count - 1) {
                    currChar = 0;
                }
            } else {
                currChar--;
                if (currChar < 0) {
                currChar = s.characters.Count - 1;
                }
            }
            c = s.characters[currChar];
        }
        // Sets the correct skin on first load when you scroll to a new character
        if (firstLoad) {
            if (c.skin != "Normal") {
                currentSkin = c.skins.IndexOf(c.skin);
            }
        }
        GameObject.Find("SpriteName").GetComponent<TextMeshPro>().text = c.name;
        GameObject.Find("SpriteSkin").GetComponent<TextMeshPro>().text = "(" + c.skins[currentSkin] + " Skin)";
        string skin = "";
        if (PlayerPrefs.GetInt("Unlocked" + c.name + c.skins[currentSkin]) == 1) {
            SetCharacter();
            Unlocked();
        } else {
            currentSkinCost = c.skinCosts[currentSkin];
            Locked();
        }
        if (c.skins[currentSkin] != "Normal" && c.skins[currentSkin] != "Chromatic") {
            skin = "/" + c.skins[currentSkin];
        }
        if (c.skin == "Chromatic") {
            StartCoroutine(ChangeColor());
        }
        if (c.skins[currentSkin] == "Normal" || PlayerPrefs.GetInt("Unlocked" + c.name) == 0) {
            if (PlayerPrefs.GetInt("Unlocked" + c.name) == 1) {
                SetCharacter();
                canSwitchSkins = true;
                Unlocked();
            } else {
                GameObject.Find("SpriteName").GetComponent<TextMeshPro>().text = "???";
                GameObject.Find("SpriteSkin").GetComponent<TextMeshPro>().text = "Not Seen Yet";
                canSwitchSkins = false;
                Locked();
            }
        } else {
            canSwitchSkins = true;
        }
        main.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + c.name + "Sprite" + skin + "/idle");
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Placeholder")) {
            g.transform.localScale = new Vector3(c.spriteScale.x * 0.6f, c.spriteScale.y * 0.6f, 1);
        }
        GameObject.Find("LeftSprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + c.name + "Sprite" + skin + "/left");
        GameObject.Find("DownSprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + c.name + "Sprite" + skin + "/down");
        GameObject.Find("UpSprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + c.name + "Sprite" + skin + "/up");
        GameObject.Find("RightSprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + c.name + "Sprite" + skin + "/right");
        main.transform.localScale = c.spriteScale;
        main.transform.position = originalMainPlacement;
        main.transform.position += new Vector3(0, c.yOffset, 0);
        if (c.name == "Adrian") { 
            GameObject.Find("LeftSprite").transform.localScale -= new Vector3(0.27f, 0.27f, 0);
            GameObject.Find("DownSprite").transform.localScale -= new Vector3(0.27f, 0.27f, 0);
            GameObject.Find("UpSprite").transform.localScale -= new Vector3(0.27f, 0.27f, 0);
            GameObject.Find("RightSprite").transform.localScale -= new Vector3(0.27f, 0.27f, 0);
        }
    }

    void Unlocked() {
        GameObject.Find("LockSymbol").GetComponent<SpriteRenderer>().sprite = unlockedSprite;
        GameObject.Find("LockText").GetComponent<TextMeshPro>().text = "UNLOCKED!";
        GameObject.Find("SpaceToBuy").GetComponent<TextMeshPro>().enabled = false;
        GameObject.Find("MainSprite").GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Placeholder")) {
            g.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        }
        currentSkinCost = 0;
    }

    void Locked() {
        GameObject.Find("LockSymbol").GetComponent<SpriteRenderer>().sprite = lockedSprite;
        if (currentSkinCost != 0) {
            GameObject.Find("LockText").GetComponent<TextMeshPro>().text = "COST: " + currentSkinCost.ToString();
            GameObject.Find("SpaceToBuy").GetComponent<TextMeshPro>().enabled = true;
        } else {
            GameObject.Find("LockText").GetComponent<TextMeshPro>().text = "LOCKED!";
        }
        GameObject.Find("MainSprite").GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 255);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Placeholder")) {
            g.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 255);
        }
    }
}

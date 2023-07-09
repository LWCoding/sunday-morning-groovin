using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character
{
    public string name;
    public string skin;
    public Vector2 spriteScale;
    public Vector2 iconScale;
    public float yOffset;
    public List<string> skins;
    public List<int> skinCosts;
    public Character(string name, Vector2 spriteScale, Vector2 iconScale, float yOffset, string[] skins = null, int[] skinCosts = null)
    {
        this.name = name;
        this.skin = "Normal";
        this.spriteScale = spriteScale;
        this.iconScale = iconScale;
        this.yOffset = yOffset;
        this.skins = new List<string>();
        this.skinCosts = new List<int>();
        this.skins.Add("Normal");
        this.skinCosts.Add(0);
        if (skins != null)
        {
            foreach (string s in skins)
            {
                this.skins.Add(s);
            }
        }
        if (skinCosts != null)
        {
            foreach (int s in skinCosts)
            {
                this.skinCosts.Add(s);
            }
        }
    }
}


public class SongData : MonoBehaviour
{

    public string currentMod;
    public float enemyOrigY;
    [HideInInspector] public List<Character> characters = new List<Character>();

    // The skins are set in the SetCharacter() function in CosmeticsHandler
    void LoadSkins()
    {
        foreach (Character c in characters)
        {
            if (PlayerPrefs.GetInt(c.name + "Skin") != 0)
            {
                if (PlayerPrefs.GetInt(c.name + "Skin") < c.skins.Count)
                {
                    c.skin = c.skins[PlayerPrefs.GetInt(c.name + "Skin")];
                }
            }
        }
    }

    public void Start()
    {
        characters.Add(new Character("Skylar", new Vector2(1.1f, 1.1f), new Vector2(0.8f, 0.8f), 0.1f, new string[] { "Alt", "Chromatic" }, new int[] { 500, 1000 }));
        characters.Add(new Character("Oliver", new Vector2(1.25f, 1.25f), new Vector2(0.85f, 0.85f), 0.6f, new string[] { "BSides", "Chromatic" }, new int[] { 500, 1000 }));
        characters.Add(new Character("Camo", new Vector2(1.5f, 1.5f), new Vector2(0.8f, 0.8f), 0.1f, new string[] { "BSides", "Chromatic" }, new int[] { 500, 1000 }));
        characters.Add(new Character("Lone", new Vector2(1.6f, 1.55f), new Vector2(0.8f, 0.8f), 0.2f, new string[] { "BSides", "Gangster", "Chromatic" }, new int[] { 500, 750, 1000 }));
        characters.Add(new Character("LoneAngry", new Vector2(1.61f, 1.56f), new Vector2(0.8f, 0.8f), 0.2f));
        characters.Add(new Character("Camo&Lone", new Vector2(1.7f, 1.7f), new Vector2(0.85f, 0.85f), 0.35f));
        characters.Add(new Character("Adrian", new Vector2(1, 1), new Vector2(0.8f, 0.8f), 0.8f, new string[] { "Crypto", "Chromatic" }, new int[] { 750, 1000 }));
        characters.Add(new Character("Henry", new Vector2(1.25f, 1.25f), new Vector2(0.8f, 0.75f), 0.4f));
        characters.Add(new Character("Man", new Vector2(1.1f, 1.1f), new Vector2(0.8f, 0.8f), 0.3f, new string[] { "OG", "Chromatic" }, new int[] { 500, 1000 }));
        characters.Add(new Character("Domino", new Vector2(1.3f, 1.3f), new Vector2(0.88f, 0.88f), 0.7f, new string[] { "Social Studies", "Alt", "Chromatic" }, new int[] { 750, 750, 1000 }));
        characters.Add(new Character("Jackob", new Vector2(1.5f, 1.5f), new Vector2(0.8f, 0.8f), 0.1f, new string[] { "BSides", "Alt", "Chromatic" }, new int[] { 500, 750, 1000 }));
        characters.Add(new Character("JackobAngry", new Vector2(1.5f, 1.5f), new Vector2(0.7f, 0.7f), 0.1f, new string[] { "BSides", "Pride", "Chromatic" }, new int[] { 500, 500, 1000 }));
        characters.Add(new Character("JackobShadow", new Vector2(1.6f, 1.6f), new Vector2(0.73f, 0.73f), 0.6f, new string[] { "BSides", "Chromatic" }, new int[] { 500, 250 }));
        characters.Add(new Character("Mew", new Vector2(1.15f, 1.15f), new Vector2(0.85f, 0.85f), 1, new string[] { "Shiny", "Legacy", "Chromatic" }, new int[] { 500, 750, 1000 }));
        characters.Add(new Character("MewAngry", new Vector2(1.37f, 1.37f), new Vector2(0.78f, 0.78f), 1, new string[] { "Alt", "Chromatic" }, new int[] { 750, 1000 }));
        characters.Add(new Character("JakobHuman", new Vector2(1.1f, 1.1f), new Vector2(0.85f, 0.85f), 0.3f));
        characters.Add(new Character("Ariel", new Vector2(1.38f, 1.38f), new Vector2(0.9f, 0.9f), 0.8f));
        characters.Add(new Character("AriongUs", new Vector2(1.28f, 1.28f), new Vector2(0.75f, 0.75f), 0.4f));
        characters.Add(new Character("Floof", new Vector2(1.4f, 1.4f), new Vector2(0.8f, 0.9f), -0.4f));
        characters.Add(new Character("NULL", new Vector2(2, 2), new Vector2(1.1f, 1.1f), 1.3f));
        characters.Add(new Character("Patches", new Vector2(1.3f, 1.3f), new Vector2(0.73f, 0.73f), 0.7f, new string[] { "Chromatic" }, new int[] { 1000 }));
        characters.Add(new Character("PatchesCrazy", new Vector2(1.3f, 1.3f), new Vector2(0.73f, 0.73f), 0.7f));
        LoadSkins();
    }

    // Returns a list of Characters. The first is the player, and the second is the enemy.
    public Character[] GetCharacterInfo(Song s)
    {
        Character[] result = { null, null };
        foreach (Character c in characters)
        {
            if (s.playerName == c.name)
            {
                result[0] = c;
            }
            if (s.enemyName == c.name)
            {
                result[1] = c;
            }
            if (result[0] != null && result[1] != null)
            {
                break;
            }
        }
        return result;
    }

    public Song GetSong(string song)
    {
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        Song[] songs = p.songs;
        song = song.Replace(" ", "");
        foreach (Song s in songs)
        {
            if (s.songName == song)
            {
                return s;
            }
        }
        Debug.Log("ERROR! COULDN'T FIND SPECIFIED SONG IN SONGDATA! => " + song);
        return null;
    }

    public void SetSong(string song)
    {
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        Song s = GetSong(song);
        p.dialogue = s.dialogue;
        p.songList = s.movements;
        currentMod = s.modifier;
        Show((p.cameFromDebug) ? p.playerName : s.playerName, (p.cameFromDebug) ? p.enemyName : s.enemyName, s.bgName, s.songName);
    }

    public void TintCharacters(byte r, byte g, byte b, string target = "")
    {
        if (target != "enemy")
        {
            GameObject.Find("Player").GetComponent<SpriteRenderer>().color = new Color32(r, g, b, 255);
        }
        if (target != "player")
        {
            GameObject.Find("Enemy").GetComponent<SpriteRenderer>().color = new Color32(r, g, b, 255);
        }
    }

    public void SetScene(string background)
    {
        // Set dialogue box color.
        switch (background)
        {
            case "Desert":
                GameObject.Find("DBox").GetComponent<Image>().color = new Color32(255, 250, 83, 0);
                break;
            case "City":
                GameObject.Find("DBox").GetComponent<Image>().color = new Color32(45, 202, 97, 0);
                break;
            case "Paper":
                GameObject.Find("DName").GetComponent<Text>().color = new Color32(22, 22, 22, 0);
                break;
            case "Alley":
                GameObject.Find("DBox").GetComponent<Image>().color = new Color32(0, 0, 183, 0);
                break;
            case "Dataroom":
                GameObject.Find("DBox").GetComponent<Image>().color = new Color32(0, 12, 255, 0);
                break;
            case "Spaceship":
                GameObject.Find("DBox").GetComponent<Image>().color = new Color32(0, 255, 25, 0);
                break;
        }
        // Disable initial foregrounds.
        GameObject.Find("OceanFG").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("LibraryFG").GetComponent<SpriteRenderer>().enabled = false;
        // Set initial player tints.
        GameObject.Find("Player").GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        GameObject.Find("Enemy").GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        // Set background & apply proper tints.
        GameObject[] bgs = GameObject.FindGameObjectsWithTag("Background");
        foreach (GameObject bg in bgs)
        {
            if (bg.name != background + "BG")
            {
                bg.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                bg.GetComponent<SpriteRenderer>().enabled = true;
                if (bg.name == "OceanBG")
                {
                    GameObject.Find("OceanFG").GetComponent<SpriteRenderer>().enabled = true;
                    TintCharacters(192, 255, 255);
                }
                if (bg.name == "LibraryBG")
                {
                    GameObject.Find("LibraryFG").GetComponent<SpriteRenderer>().enabled = true;
                    Vector2 mult = GameObject.Find("Player").transform.localScale;
                    GameObject.Find("Player").transform.localScale += new Vector3(mult.x * 0.1f, mult.y * 0.1f, 0);
                    mult = GameObject.Find("Enemy").transform.localScale;
                    GameObject.Find("Enemy").transform.localScale += new Vector3(mult.x * 0.1f, mult.y * 0.1f, 0);
                    TintCharacters(192, 181, 181);
                }
                if (bg.name == "Library2BG")
                {
                    GameObject.Find("Library2FG").GetComponent<SpriteRenderer>().enabled = true;
                    Vector2 mult = GameObject.Find("Player").transform.localScale;
                    GameObject.Find("Player").transform.localScale += new Vector3(mult.x * 0.1f, mult.y * 0.1f, 0);
                    mult = GameObject.Find("Enemy").transform.localScale;
                    GameObject.Find("Enemy").transform.localScale += new Vector3(mult.x * 0.1f, mult.y * 0.1f, 0);
                    TintCharacters(192, 181, 181);
                }
                if (bg.name == "CityBG")
                {
                    TintCharacters(227, 227, 227);
                }
                if (bg.name == "DesertBG")
                {
                    TintCharacters(255, 210, 223);
                }
                if (bg.name == "AlleyBG")
                {
                    TintCharacters(186, 212, 255);
                }
                if (bg.name == "SpaceshipBG")
                {
                    TintCharacters(193, 231, 227);
                }
                if (bg.name == "DataroomBG")
                {
                    TintCharacters(190, 199, 204);
                }
            }
        }
    }

    public void SetCharacter(string name, bool isEnemy)
    {
        // Get character object; if it doesn't exist then Debug Log it.
        Character character = null;
        foreach (Character c in characters)
        {
            if (c.name == name)
            {
                character = c;
                break;
            }
        }
        if (character == null)
        {
            Debug.Log("ERROR! COULD NOT FIND CHARACTER BY NAME IN SONGDATA (" + name + ")!");
            return;
        }
        GameObject characterObj = (isEnemy) ? GameObject.Find("Enemy") : GameObject.Find("Player");
        characterObj.transform.position = new Vector3(characterObj.transform.position.x, enemyOrigY, characterObj.transform.position.z);
        // Place sprite in correct position.
        characterObj.transform.position += new Vector3(0, character.yOffset, 0);
        if (isEnemy)
        {
            GameObject.Find("Spotlight").transform.position += new Vector3(0, character.yOffset, 0);
        }
        // Set sprite dimensions correctly.
        characterObj.transform.localScale = new Vector2(((isEnemy) ? 1 : -1) * character.spriteScale.x, character.spriteScale.y);
        // Set isPlayer to true/false, so it inverts movements if necessary.
        characterObj.GetComponent<SpriteHandler>().isPlayer = !isEnemy;
        // Set all of the sprite images.
        if (character.name == "JackobShadow" || character.name == "NULL")
        {
            characterObj.GetComponent<SpriteHandler>().shadowEffect = true;
        }
        if (character.name == "NULL")
        {
            characterObj.GetComponent<SpriteHandler>().shadowOpacity = 0.8f;
        }
        if (character.skin == "Chromatic")
        {
            StartCoroutine(characterObj.GetComponent<SpriteHandler>().ChangeColor());
        }
        string skinFolder = "";
        if (character.skin != "Normal" && character.skin != "Chromatic")
        {
            skinFolder = "/" + character.skin;
        }
        Sprite enemyIdle = Resources.Load<Sprite>("Images/" + name + "Sprite" + skinFolder + "/Idle");
        characterObj.GetComponent<SpriteRenderer>().sprite = enemyIdle;
        characterObj.GetComponent<SpriteHandler>().idle = enemyIdle;
        characterObj.GetComponent<SpriteHandler>().up = Resources.Load<Sprite>("Images/" + name + "Sprite" + skinFolder + "/Up");
        characterObj.GetComponent<SpriteHandler>().down = Resources.Load<Sprite>("Images/" + name + "Sprite" + skinFolder + "/Down");
        characterObj.GetComponent<SpriteHandler>().left = Resources.Load<Sprite>("Images/" + name + "Sprite" + skinFolder + "/Left");
        characterObj.GetComponent<SpriteHandler>().right = Resources.Load<Sprite>("Images/" + name + "Sprite" + skinFolder + "/Right");
        characterObj.GetComponent<SpriteHandler>().miss = Resources.Load<Sprite>("Images/" + name + "Sprite" + skinFolder + "/Miss");
        // Set the global variable for other scripts to use the correct sprite.
        if (isEnemy)
        {
            GameObject.Find("PublicVariables").GetComponent<PublicVariables>().enemyName = name;
        }
        else
        {
            GameObject.Find("PublicVariables").GetComponent<PublicVariables>().playerName = name;
        }
        // Set icons and flip if necessary. 
        Sprite normalIcon = Resources.Load<Sprite>("Images/" + name + "Sprite/Icon");
        Sprite losingIcon = Resources.Load<Sprite>("Images/" + name + "Sprite/IconLose");
        GameObject.Find((isEnemy) ? "EnemyIcon" : "PlayerIcon").GetComponent<IconSprites>().current = null;
        GameObject.Find((isEnemy) ? "EnemyIcon" : "PlayerIcon").GetComponent<IconSprites>().normalIcon = normalIcon;
        if (losingIcon != null)
        {
            GameObject.Find((isEnemy) ? "EnemyIcon" : "PlayerIcon").GetComponent<IconSprites>().losingIcon = losingIcon;
        }
        else
        {
            GameObject.Find((isEnemy) ? "EnemyIcon" : "PlayerIcon").GetComponent<IconSprites>().losingIcon = normalIcon;
        }
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().IconAdjust();
        GameObject.Find((isEnemy) ? "EnemyIcon" : "PlayerIcon").transform.localScale = new Vector3(((isEnemy) ? 1 : -1) * character.iconScale.x, character.iconScale.y, 1);
    }

    private void SetExtras(string songName)
    {
        switch (songName)
        {
            case "belligerent":
                GameObject.Find("Darkness").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.25f);
                GameObject.Find("Darkness").GetComponent<SpriteRenderer>().enabled = true;
                break;
            case "overkill":
                GameObject.Find("Darkness").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.4f);
                GameObject.Find("Darkness").GetComponent<SpriteRenderer>().enabled = true;
                break;
        }
    }

    public void Show(string playerName, string enemyName, string background, string songName)
    {
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        p.background = background + "BG";
        SetCharacter(playerName, false);
        SetCharacter(enemyName, true);
        SetScene(background);
        SetExtras(songName);
    }

}

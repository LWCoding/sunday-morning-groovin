using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSprites : MonoBehaviour
{
    public Sprite normalIcon;
    public Sprite losingIcon;
    public string current = null;
    public void ToNormal() {
        if (current == "n") { return; }
        GetComponent<Image>().sprite = normalIcon;
        current = "n";
    }  
    public void ToLosing() {
        if (current == "l") { return; }
        GetComponent<Image>().sprite = losingIcon;
        current = "l";
    }
}

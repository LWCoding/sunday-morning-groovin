using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StreakTextFade : MonoBehaviour
{
    
    [HideInInspector] public float timeAlive = 0f;
    [HideInInspector] public float lastUpdate = 0f;
    private float xDiff;
    [HideInInspector] public bool xMove = true;

    void Start() {
        xDiff = Random.Range(-0.008f, 0.008f);
    }

    void Update() {
        timeAlive += Time.deltaTime;
        lastUpdate += Time.deltaTime;
        if (timeAlive < 0.4f) {
            if (lastUpdate > 0.01f) {
                transform.Translate(((xMove) ? xDiff : 0), 0.025f * (timeAlive * 2 / 1.3f), 0);
                Color c = GetComponent<TextMeshPro>().color;
                GetComponent<TextMeshPro>().color = new Color(c.r, c.g, c.b, GetComponent<TextMeshPro>().color.a - 0.01f); 
                lastUpdate = 0;
            }
        } else if (timeAlive < 1.4f) {
            if (lastUpdate > 0.01f) {
                transform.Translate(((xMove) ? xDiff : 0) * 0.5f, -0.02f, 0);
                Color c = GetComponent<TextMeshPro>().color;
                GetComponent<TextMeshPro>().color = new Color(c.r, c.g, c.b, GetComponent<TextMeshPro>().color.a - 0.01f);  
                lastUpdate = 0;
            }
        } else if (timeAlive > 1.7f) {
            gameObject.SetActive(false);
        }
    }

}

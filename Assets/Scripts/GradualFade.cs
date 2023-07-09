using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradualFade : MonoBehaviour
{

    private float localScale;

    void Start()
    {
        StartCoroutine(Fade());
        transform.localScale = new Vector2(1, 1);
    }

    IEnumerator Fade() {
        float fadeTime = 0.014f;
        if (PlayerPrefs.GetInt("Quality") == 0) {
            for (int i = 0; i < 20; i++) {
                transform.localScale -= new Vector3(0.03f, 0.03f, 0);
                GetComponent<Image>().color -= new Color(0, 0, 0, 0.05f);
                yield return new WaitForSeconds(fadeTime);
                fadeTime += 0.001f;
            }
        } else {
            for (int i = 0; i < 10; i++) {
                transform.localScale -= new Vector3(0.04f, 0.04f, 0);
                GetComponent<Image>().color -= new Color(0, 0, 0, 0.05f);
                yield return new WaitForSeconds(fadeTime);
                fadeTime += 0.002f;
            }
        }
        Destroy(gameObject);
    }
}

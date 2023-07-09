using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadowFade : MonoBehaviour
{

    private WaitForSeconds wfs;

    void Start()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade() {
        if (PlayerPrefs.GetInt("Quality") == 0) {
            for (int i = 0; i < 20; i++) {
                transform.localScale += new Vector3(0.02f, 0.02f, 0);
                GetComponent<Image>().color -= new Color(0, 0, 0, 0.05f);
                wfs = new WaitForSeconds(0.05f);
                yield return wfs;
            }
        } else {
            for (int i = 0; i < 10; i++) {
                transform.localScale += new Vector3(0.03f, 0.03f, 0);
                GetComponent<Image>().color -= new Color(0, 0, 0, 0.05f);
                wfs = new WaitForSeconds(0.1f); 
                yield return wfs;
            }
        }
        Destroy(gameObject);
    }
}

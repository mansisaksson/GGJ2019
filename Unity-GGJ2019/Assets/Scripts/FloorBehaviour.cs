using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBehaviour : MonoBehaviour
{

    public bool Visible = false;
    Material mat;
    Renderer rend;
    Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = Visible;
        mat = rend.material;
        originalColor = mat.color;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rend.enabled) return;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.name == "PlayerModel")
            {
                GetComponent<Renderer>().enabled = true;
                FadeColor(0.5f, originalColor, originalColor * new Color(1, 1, 1, 0), mat, () => GetComponent<Renderer>().enabled = false);
            }
        }
    }

    public void FadeColor(float fadeTime, Color startColor, Color endColor, Material mat, Action onComplete = null)
    {
        StartCoroutine(inner_FadeColor(fadeTime, startColor, endColor, mat, onComplete));
    }

    private IEnumerator inner_FadeColor(float fadeTime, Color startColor, Color endColor, Material mat, Action onComplete = null)
    {
        float timerElapsed = 0;
        float lerpVal = 0;
        while (timerElapsed < fadeTime)
        {
            lerpVal = Mathf.InverseLerp(0, fadeTime, timerElapsed);
            if (lerpVal >= 1) break;

            timerElapsed += Time.deltaTime;
            mat.color = Color.Lerp(startColor, endColor, lerpVal);
            yield return null;
        }

        mat.color = endColor;
        onComplete?.Invoke();
    }
}

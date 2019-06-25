using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveEffect : MonoBehaviour
{

    private Material mat;
    private Texture2D tex;
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.SetVector("_RippleData", new Vector4(0,0,0, 0));
        tex = mat.mainTexture as Texture2D;
    }

    void Update()
    {

    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.name != "PlayerModel") return;

        var contact = collisionInfo.contacts.First();
        RaycastHit hit;

        //if (!Physics.Raycast(contact.point-contact.normal, contact.normal, out hit, 5)) return;
        if (!Physics.Raycast(contact.point + contact.normal, -contact.normal, out hit, 5)) return;
        //if (!Physics.Raycast(contact.thisCollider.transform.position, contact.thisCollider.transform.position-contact.otherCollider.transform.position, out hit, 5, LayerMask.NameToLayer("Wave"))) return;
        mat.SetVector("_RippleData", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 2f, 0));

        InvokeDelayed(2f, () => mat.SetVector("_RippleData", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0)));
    }

    public void InvokeDelayed(float delayTime, Action func)
    {
        StartCoroutine(inner_InvokeDelayed(delayTime, func));
    }

    private IEnumerator inner_InvokeDelayed(float delayTime, Action func)
    {
        float timerElapsed = 0;
        while (timerElapsed < delayTime)
        {
            timerElapsed += Time.deltaTime;
            yield return null;
        }

        func();
    }
    //void OnCollisionStay(Collision collisionInfo)
    //{
    //    foreach (ContactPoint contact in collisionInfo.contacts)
    //    {
    //        Debug.DrawRay(contact.point, contact.normal, Color.white);
    //    }
    //}
}

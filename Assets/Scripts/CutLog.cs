using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutLog : MonoBehaviour
{
    bool cutting = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!cutting && other.CompareTag("Log"))
        {
            Transform logPos = transform.GetChild(0);
            other.gameObject.tag = "Untagged";
            other.gameObject.layer = 0;
            other.transform.parent = logPos;
            other.transform.position = logPos.position;
            other.transform.rotation = logPos.rotation;
            GetComponent<Animator>().SetTrigger("Cut");
            StartCoroutine(cut());
        }
    }

    IEnumerator cut()
    {
        Transform logPos = transform.GetChild(0);
        yield return new WaitForSeconds(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length+.5f);
        Destroy(logPos.GetChild(0).gameObject);
        Instantiate(GameManager.instance.pickupTypes[5], logPos.position + new Vector3(0, 0, -.1f), logPos.rotation, worldGen.instances[gameObject.scene.buildIndex].pickupParent);
        Instantiate(GameManager.instance.pickupTypes[5], logPos.position + new Vector3(0, 0, .1f), logPos.rotation, worldGen.instances[gameObject.scene.buildIndex].pickupParent);
        cutting = false;
        yield return null;
    }
}

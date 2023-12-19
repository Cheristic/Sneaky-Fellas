using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPrefabBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DespawnSound());
    }

    private IEnumerator DespawnSound()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

}

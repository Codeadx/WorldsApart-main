using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iterator : MonoBehaviour
{
    private IEnumerator coroutine;
    public int i;
    public int a;
    public int j;
    public int b;
    public int k;
    public int c;
    public int l;
    public int d;

    // Start is called before the first frame update
    void Start()
    {
        coroutine = StartIterator();
        StartCoroutine(coroutine);
    }

    IEnumerator StartIterator() {
        for(i = 0; i < 8; i++) {
            a = i + 8;
            yield return new WaitForSeconds(1f);
        }
        for(j = 0; j < 4; j++) {
            b = j + 4;
            yield return new WaitForSeconds(1f);
        }
        for(k = 8; k < 12; k++) {
            c = k + 2;
            yield return new WaitForSeconds(1f);
        }
        for(l = 0; l < 8; l += 2) {
            d = l + 1;
            yield return new WaitForSeconds(1f);
        }

    }
}

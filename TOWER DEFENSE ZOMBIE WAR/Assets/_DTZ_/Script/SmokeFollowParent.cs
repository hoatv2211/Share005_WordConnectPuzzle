using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeFollowParent : MonoBehaviour
{
    Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = parent.position;
    }
}

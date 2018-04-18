using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {
    public GameObject target;
    public string searchTag;

	// Update is called once per frame
	void LateUpdate () {

        if (tag != "" && target == null)
            target = GameObject.FindWithTag(searchTag);

        if (target == null && searchTag == "")
            return;

        transform.LookAt(target.transform);
	}
}

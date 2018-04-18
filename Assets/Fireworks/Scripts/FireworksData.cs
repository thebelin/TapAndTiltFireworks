using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FireworksData : MonoBehaviour {
    // The name to show
    public string title;

    // The color to paint the identifiers and cursors
    public Color color;

    // The time between users allowed to launch
    public float minDelay = 5f;

    // The prefab to use for this item
    public GameObject prefab;
}

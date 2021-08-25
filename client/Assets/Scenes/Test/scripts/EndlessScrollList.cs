using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessScrollList : MonoBehaviour
{

    public List<GameObject> Scrollpanels  = new List<GameObject>();
    public int countPanels;

    public void Awake()
    {
        countPanels = Scrollpanels.Count;
    }
}

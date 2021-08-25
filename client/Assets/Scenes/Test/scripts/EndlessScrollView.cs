using System;
using System.Collections;
using System.Collections.Generic;
using IoC.Attribute;
using UnityEngine;

public class EndlessScrollView : MonoBehaviour
{
    [SerializeField]
    private EndlessScrollList _endlessScrollList;
    // Update is called once per frame
    public void Start()
    {
        foreach (GameObject scrollpanel in _endlessScrollList.Scrollpanels) {
            
        }
    }
}

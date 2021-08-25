
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EndlessScroll : MonoBehaviour
{
    [Range(1, 50)]
    [Header("Controllers")]
    [FormerlySerializedAs("elementCount")]
    public int _elementCount;

    [Range(0, 500)]
    public int offset;
    
    [Header("Clone Object")]
    public GameObject Prefab;


    private GameObject[] instPans;

    public List<GameObject> PanelList;
    
    
    public void Start()
    {
        
        instPans = new GameObject[_elementCount];
        for (int i = 0; i < _elementCount; i++) {
            
            instPans[i] =  Instantiate(Prefab, transform, false);
            if (i == 0 ) continue;
            
            instPans[i].transform.localPosition = new Vector2(instPans[i-1].transform.localPosition.x + Prefab.GetComponent<RectTransform>().sizeDelta.x + offset, 
                                                              instPans[i].transform.localPosition.y); 
        }
        foreach (GameObject panel in PanelList) {
            
        }
        
    }

}

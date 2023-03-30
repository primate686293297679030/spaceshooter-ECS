using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class OnHoover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Color origin = new Color(0.306f, 0.153f, 0.490f, 1.000f);
    public TextMeshProUGUI tmPro;
    
    
    private void Start()
    {
        tmPro = GetComponentInChildren<TextMeshProUGUI>();
    }
      
      //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        tmPro.color =Color.green;
    }
      //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        tmPro.color = origin;
    }
}

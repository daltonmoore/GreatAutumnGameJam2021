using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Slider m_healthBar;

    private void Start()
    {
        
    }

    public void Refresh()
    {
        m_healthBar.value = PlayerController.instance.Health;
    }
}

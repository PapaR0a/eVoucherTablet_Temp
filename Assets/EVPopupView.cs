using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EVPopupView : MonoBehaviour
{
    [SerializeField] private Text m_errorMessage;
        

    public void ShowMessage(string message) 
    {
        m_errorMessage.text = message;
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EVVoucherItemView : MonoBehaviour
{
    [SerializeField] private InputField m_IfQuantity;
    [SerializeField] private Text m_TxtItemName;

    public void Setup(int quantity, string itemName)
    {
        m_IfQuantity.text = quantity.ToString();
        m_TxtItemName.text = itemName;
    }
}

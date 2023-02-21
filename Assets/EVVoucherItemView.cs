using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EVVoucherItemView : MonoBehaviour
{
    [SerializeField] private InputField m_IfQuantity;
    [SerializeField] private Text m_TxtItemName;

    public string ItemId;

    public void Setup(int quantity, string itemName, bool editable = false, string itemId = "")
    {
        m_IfQuantity.interactable = editable;
        m_IfQuantity.text = quantity.ToString();
        m_TxtItemName.text = itemName;

        ItemId = itemId;
    }

    public int GetRedeemingCount()
    {
        int remaining = int.Parse(m_IfQuantity.text);
        remaining = remaining < 0 ? 0 : remaining;
        return remaining;
    }

    public string GetItemName()
    {
        return m_TxtItemName.text;
    }
}

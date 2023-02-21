using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EVVoucherProductItemView : MonoBehaviour
{
    private InputField m_TxtQuantity;
    private Text m_TxtProductName;
    private InputField m_IfRedeemCount;

    private VoucherProduct m_Data;
    private int m_Remaining;
    private int m_RedeemCount;

    private void OnDestroy()
    {
        if (m_IfRedeemCount != null)
        {
            m_IfRedeemCount.onEndEdit.RemoveAllListeners();
        }
    }

    public void Setup(VoucherProduct data, bool readOnly = false)
    {
        m_Data = data;

        m_TxtQuantity = transform.Find("quantity").GetComponent<InputField>();
        m_TxtProductName = transform.Find("name").GetComponent<Text>();
        m_IfRedeemCount = transform.Find("redeem").GetComponent<InputField>();
        m_IfRedeemCount.gameObject.SetActive(!readOnly);
        m_IfRedeemCount.onEndEdit.AddListener(delegate { OnEditRedeem(); });

        m_TxtQuantity.text = readOnly ? data.remaining.ToString() : data.remaining.ToString();
        m_TxtProductName.text = data.name;
    }

    private void OnEditRedeem()
    {
        if (float.TryParse(m_IfRedeemCount.text, out float output1))
        {
            m_IfRedeemCount.text = Mathf.Clamp(output1, 0, m_Data.remaining).ToString();
            m_Remaining = m_Data.remaining - (int)output1;
            m_TxtQuantity.text = m_Remaining < 0 ? 0.ToString() : m_Remaining.ToString();

            m_RedeemCount = int.Parse(m_IfRedeemCount.text);
            m_Remaining = int.Parse(m_TxtQuantity.text);

            m_RedeemCount = m_RedeemCount < 0 ? 0 : m_RedeemCount;
            m_Remaining = m_Remaining < 0 ? 0 : m_Remaining;
        }
    }

    public int GetItemRemaining()
    {
        m_Remaining = int.Parse(m_TxtQuantity.text);
        m_Remaining = m_Remaining < 0 ? 0 : m_Remaining;
        return m_Remaining;
    }

    public string GetItemId()
    {
        return m_Data.id;
    }

    public int GetRedeemCount()
    {
        return m_RedeemCount;
    }

    public string GetItemName()
    {
        return m_Data.name;
    }

    public int GetItemDefaultQuantity()
    {
        return m_Data.remaining;
    }
}

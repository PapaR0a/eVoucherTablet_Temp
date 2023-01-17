using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EVVoucherLineView : MonoBehaviour
{
    [SerializeField] private Text m_TxtId;
    [SerializeField] private Text m_TxtOrg;
    [SerializeField] private Text m_TxtDept;
    [SerializeField] private Text m_TxtExpiry;
    [SerializeField] private Text m_TxtIssuer;

    private Voucher m_Data;
    private Action<Voucher> onClickEvent;

    public void Setup(Voucher data, Action<Voucher> onClick = null)
    {
        m_Data = data;
        m_TxtId.text = data.id;
        m_TxtOrg.text = data.org;
        m_TxtDept.text = data.department;
        m_TxtExpiry.text = data.expiry_date;
        m_TxtIssuer.text = data.issuer;

        onClickEvent = onClick;
    }

    public void OnClick()
    {
        onClickEvent?.Invoke(m_Data);
    }
}

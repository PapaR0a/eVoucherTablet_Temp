//using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EVHistoryPageItemView : MonoBehaviour
{
    private Text m_TxtStatus;
    private Text m_TxtOrganization;
    private Text m_TxtDepartment;
    private Text m_TxtFundingType;
    private Text m_TxtExpiryDate;

    private Voucher m_Data;

    public void SetupCard(Voucher data)
    {
        m_Data = data;

        m_TxtStatus = transform.Find("status").GetComponent<Text>();
        m_TxtOrganization = transform.Find("organization").GetComponent<Text>();
        m_TxtDepartment = transform.Find("department").GetComponent<Text>();
        m_TxtFundingType = transform.Find("fundingtype").GetComponent<Text>();
        m_TxtExpiryDate = transform.Find("expirationdate").GetComponent<Text>();

        m_TxtStatus.text = $"{data.status}";
        m_TxtOrganization.text = $"Organization: {data.org}";
        m_TxtDepartment.text = $"Department: {data.department}";
        m_TxtFundingType.text = $"Funding Type: {data.fundingType}";
        m_TxtExpiryDate.text = $"Expiration Date: {data.expiry_date}";
    }

    public void OnClickCard()
    {
        EVModel.Api.CachedCurrentVoucher = m_Data;
        //DOVirtual.DelayedCall(0.01f, () => EVControl.Api.ShowVoucherDetails(m_Data, true));
    }
}

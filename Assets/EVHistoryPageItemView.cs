//using DG.Tweening;
using DG.Tweening;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class EVHistoryPageItemView : MonoBehaviour
{
    public Text m_TxtStatus;
    public Text m_TxtOrganization;
    public Text m_TxtDepartment;
    public Text m_TxtFundingType;
    public Text m_TxtExpiryDate;

    private Voucher m_Data;

    public void SetupCard(Voucher data)
    {
        m_Data = data;

        string status = data.status;
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        status = textInfo.ToTitleCase(status);

        m_TxtStatus.text = $"{status}";
        m_TxtOrganization.text = $"Organization: {data.org}";
        m_TxtDepartment.text = $"Department: {data.department}";
        m_TxtFundingType.text = $"Funding Type: {data.fundingType}";
        m_TxtExpiryDate.text = $"Expiration Date: {data.expiry_date}";
    }

    public void OnClickCard()
    {
        EVModel.Api.CachedCurrentVoucher = m_Data;
        DOVirtual.DelayedCall(0.01f, () => EVControl.Api.ShowVoucherDetails(m_Data, true));
    }
}

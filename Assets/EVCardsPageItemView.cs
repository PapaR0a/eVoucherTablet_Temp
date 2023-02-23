using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EVCardsPageItemView : MonoBehaviour
{
    private Text m_TxtTitle;
    private Text m_TxtFundingType;
    private Text m_TxtOrganization;
    private Text m_TxtDepartment;
    private Text m_TxtExpiryDate;
    private Button m_BtnRedeem;

    private Text m_TxtId;

    private Voucher m_Data;

    [SerializeField] private List<Sprite> m_OrgLogos;
    [SerializeField] private Image m_ImageOrgLogo;

    [SerializeField] private List<Sprite> m_FrontCardImages;
    [SerializeField] private List<Sprite> m_BackCardImages;

    [SerializeField] private Image m_ImageCardFront;
    [SerializeField] private Image m_ImageCardBack;

    public void OnDestroy()
    {
        m_BtnRedeem.onClick.RemoveAllListeners();
    }

    public void OnEnable()
    {
        m_TxtTitle = transform.Find("front/Text").GetComponent<Text>();
        m_TxtFundingType = transform.Find("front/FundingType").GetComponent<Text>();
        m_TxtOrganization = transform.Find("front/Organization").GetComponent<Text>();
        m_TxtDepartment = transform.Find("front/Department").GetComponent<Text>();
        m_TxtExpiryDate = transform.Find("front/ExpiryDate").GetComponent<Text>();

        m_TxtId = transform.Find("back/Text").GetComponent<Text>();
        m_BtnRedeem = transform.Find("back/Redeem").GetComponent<Button>();

        m_BtnRedeem.onClick.AddListener(OnClickRedeem);
    }

    public void OnDisable()
    {
        m_BtnRedeem.onClick.RemoveAllListeners();
    }

    public void SetupCard(Voucher data)
    {
        m_Data = data;

        if (m_TxtFundingType)
            m_TxtFundingType.text = $"Funding Type: {data.fundingType ?? string.Empty}";

        if (m_TxtOrganization)
            m_TxtOrganization.text = $"Organization: {data.org ?? string.Empty}";

        if (m_TxtDepartment)
            m_TxtDepartment.text = $"Department: {data.department ?? string.Empty}";

        if (m_TxtExpiryDate)
            m_TxtExpiryDate.text = $"Expiry Date: {data.expiry_date ?? string.Empty}";

        if (m_TxtId)
            m_TxtId.text = data.id ?? string.Empty;

        m_ImageOrgLogo.sprite = GetOrgSprite(m_Data.org);
        m_ImageCardFront.sprite = GetFrontCardSprite(m_Data.org);
        m_ImageCardBack.sprite = GetBackCardSprite(m_Data.org);
    }

    public void OnClickRedeem()
    {
        EVModel.Api.CachedCurrentVoucher = m_Data;
        EVControl.Api.ShowVoucherDetails(m_Data, false);
    }

    private Sprite GetOrgSprite(string org)
    {
        switch (org)
        {
            case "TTSH":
                return m_OrgLogos[(int)Organizations.TTSH];

            case "WDL":
                return m_OrgLogos[(int)Organizations.WDL];

            case "NHGP":
                return m_OrgLogos[(int)Organizations.NHGP];

            default:
                return m_OrgLogos[(int)Organizations.TTSH];
        }
    }

    private Sprite GetFrontCardSprite(string org)
    {
        switch (org)
        {
            case "TTSH":
                return m_FrontCardImages[(int)Organizations.TTSH];

            case "WDL":
                return m_FrontCardImages[(int)Organizations.WDL];

            case "NHGP":
                return m_FrontCardImages[(int)Organizations.NHGP];

            default:
                return m_FrontCardImages[(int)Organizations.TTSH];
        }
    }

    private Sprite GetBackCardSprite(string org)
    {
        switch (org)
        {
            case "TTSH":
                return m_BackCardImages[(int)Organizations.TTSH];

            case "WDL":
                return m_BackCardImages[(int)Organizations.WDL];

            case "NHGP":
                return m_BackCardImages[(int)Organizations.NHGP];

            default:
                return m_BackCardImages[(int)Organizations.TTSH];
        }
    }
}

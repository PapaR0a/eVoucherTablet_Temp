using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class EVRedeemPageView : MonoBehaviour
{
    private Text m_TxtFundingType;
    private Text m_TxtOrganization;
    private Text m_TxtDepartment;
    private Text m_TxtExpiryDate;
    private Text m_TxtId;
    private Button m_BtnRedeem;

    private Voucher m_Data;

    [SerializeField] private GameObject m_PrefVoucherProduct;
    [SerializeField] private Transform m_ProductsContainer;

    [SerializeField] private List<Sprite> m_OrgLogos;
    [SerializeField] private Image m_ImageOrgLogo;

    [SerializeField] private List<Sprite> m_FrontCardImages;
    [SerializeField] private Image m_ImageCardFront;

    [SerializeField] private RawImage m_QRCode;
    [SerializeField] private Text m_QRCodeIdDisplay;
    [SerializeField] private GameObject m_ScanToRedeem;
    [SerializeField] private Text m_TxtToRedeem;
    [SerializeField] private Text m_TxtRedeemButton;
    private Texture2D m_storeEncodedTexture;
    private string m_newVoucherId;

    // Start is called before the first frame update
    void Start()
    {
        m_storeEncodedTexture = new Texture2D(256, 256);
    }

    private Color32[] Encode(string textForEncoding, int width, int height)
    {
        BarcodeWriter writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };

        return writer.Write(textForEncoding);
    }

    private void CreateQR(string id)
    {
        m_storeEncodedTexture = new Texture2D(256, 256);

        string newVoucherID = id;
        Color32[] convertPixelToTexture = Encode(newVoucherID, m_storeEncodedTexture.width, m_storeEncodedTexture.height);
        m_storeEncodedTexture.SetPixels32(convertPixelToTexture);
        m_storeEncodedTexture.Apply();

        m_QRCode.texture = m_storeEncodedTexture;
        m_QRCodeIdDisplay.text = id;
    }

    void OnDestroy()
    {
        EVControl.Api.OnShowVoucherDetails -= UpdateDetailsView;
        m_BtnRedeem.onClick.RemoveAllListeners();
    }

    private void OnEnable()
    {
        m_TxtFundingType = transform.Find("ScrollView/Viewport/Content/card/front/FundingType").GetComponent<Text>();
        m_TxtOrganization = transform.Find("ScrollView/Viewport/Content/card/front/Organization").GetComponent<Text>();
        m_TxtDepartment = transform.Find("ScrollView/Viewport/Content/card/front/Department").GetComponent<Text>();
        m_TxtExpiryDate = transform.Find("ScrollView/Viewport/Content/card/front/ExpiryDate").GetComponent<Text>();
        m_TxtId = transform.Find("ScrollView/Viewport/Content/ID").GetComponent<Text>();
        m_BtnRedeem = transform.Find("ScrollView/Viewport/Content/CreateQRCode").GetComponent<Button>();

        m_BtnRedeem.onClick.AddListener(OnGenerateQR);
        StartCoroutine(ClearItems());

        EVControl.Api.OnShowVoucherDetails += UpdateDetailsView;
    }

    private void OnDisable()
    {
        m_BtnRedeem.onClick.RemoveAllListeners();
        EVControl.Api.OnShowVoucherDetails -= UpdateDetailsView;

        APIHelper.GetAllVouchers();
    }

    public void UpdateDetailsView(Voucher voucherData, bool readOnly = false)
    {
        m_Data = voucherData;

        m_TxtFundingType.text = $"Funding Type: {voucherData.fundingType}";
        m_TxtOrganization.text = $"Organization: {voucherData.org}";
        m_TxtDepartment.text = $"Department: {voucherData.department}";
        m_TxtExpiryDate.text = $"Expiry Date: {voucherData.expiry_date}";
        m_TxtId.text = voucherData.id;

        m_ImageOrgLogo.sprite = GetOrgSprite(m_Data.org);
        m_ImageCardFront.sprite = GetFrontCardSprite(m_Data.org);

        StartCoroutine( CreateItems(voucherData.items, readOnly) );
        m_QRCode.gameObject.SetActive(readOnly);

        bool isRedeemable = voucherData.status.ToLower() == "pending";

        m_BtnRedeem.gameObject.SetActive(!readOnly || isRedeemable);
        //m_TxtRedeemButton.text = isRedeemable ? "Redeem Voucher" : "Redeem Items";

        m_ScanToRedeem.SetActive(!isRedeemable && readOnly);
        m_TxtToRedeem.text = $"Voucher is {voucherData.status}";

        if (readOnly)
        {
            CreateQR(m_Data.id);
        }
        else
        {
            m_QRCodeIdDisplay.text = "";
        }
    }

    private IEnumerator CreateItems(VoucherProduct[] products, bool readOnly = false)
    {
        yield return ClearItems();

        var wait = new WaitForEndOfFrame();

        if (products.Length > 0)
        {
            foreach (var product in products)
            {
                var productView = Instantiate(m_PrefVoucherProduct, m_ProductsContainer).GetComponent<EVVoucherProductItemView>();
                yield return wait;

                if (productView != null && product != null)
                    productView.Setup(product, readOnly);
            }
        }
    }

    private IEnumerator ClearItems()
    {
        foreach (Transform item in m_ProductsContainer)
        {
            Destroy(item.gameObject);
        }

        yield return new WaitForSeconds(0.1f);
    }

    private string GenerateRandomId(int length = 0)
    {
        return System.Guid.NewGuid().ToString();
    }

    private void OnGenerateQR()
    {
        if (m_Data.status.ToLower() != "pending")
        {
            m_newVoucherId = GenerateRandomId();
            CreateQR(m_newVoucherId);
            m_QRCodeIdDisplay.text = m_newVoucherId;

            var newVoucher = new PostVoucherData();
            newVoucher.patientId = EVModel.Api.UserDetail.id;

            newVoucher.voucher = new Voucher();
            newVoucher.voucher.id = m_newVoucherId;
            newVoucher.voucher.status = "Redeemed";
            newVoucher.voucher.department = m_Data.department;
            newVoucher.voucher.org = m_Data.org;
            newVoucher.voucher.expiry_date = m_Data.expiry_date;
            newVoucher.voucher.fundingType = EVModel.Api.UserDetail.fundingType;

            var redeemingItems = new List<VoucherProduct>();
            var remainingItems = new PatchVoucherData();
            remainingItems.patiendId = EVModel.Api.UserDetail.id;
            remainingItems.voucherId = m_Data.id;
            remainingItems.items = new List<VoucherProduct>();
            foreach (Transform item in m_ProductsContainer)
            {
                EVVoucherProductItemView itemView = item.GetComponent<EVVoucherProductItemView>();
                if (itemView != null)
                {
                    var remainingItem = new VoucherProduct();
                    remainingItem.id = itemView.GetItemId();
                    remainingItem.name = itemView.GetItemName();
                    remainingItem.quantity = itemView.GetItemDefaultQuantity();
                    remainingItem.remaining = itemView.GetItemRemaining();
                    remainingItems.items.Add(remainingItem);

                    var redeemingItem = new VoucherProduct();
                    redeemingItem.id = itemView.GetItemId();
                    redeemingItem.name = itemView.GetItemName();
                    redeemingItem.quantity = itemView.GetRedeemCount();
                    redeemingItems.Add(redeemingItem);
                }
            }

            newVoucher.voucher.items = redeemingItems.ToArray();

            EVControl.Api.UpdateVoucherData(remainingItems);
            EVControl.Api.DirectRedeemVoucher(newVoucher);
        }
        else 
        {
            var updateVoucher = new PostVoucherData();
            updateVoucher.patientId = EVModel.Api.UserDetail.id;

            updateVoucher.voucher = new Voucher();
            updateVoucher.voucher.id = m_Data.id;
            updateVoucher.voucher.status = "Redeemed";
            updateVoucher.voucher.department = m_Data.department;
            updateVoucher.voucher.org = m_Data.org;
            updateVoucher.voucher.expiry_date = m_Data.expiry_date;
            updateVoucher.voucher.fundingType = EVModel.Api.UserDetail.fundingType;
            updateVoucher.voucher.items = m_Data.items;

            EVControl.Api.DirectRedeemVoucher(updateVoucher);
        }

        //EVControl.Api.FetchUserData(EVModel.Api.UserDetail.id);
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
}

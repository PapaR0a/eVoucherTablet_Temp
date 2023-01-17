using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class EVVouchersScannedView : MonoBehaviour
{
    [SerializeField] private Transform m_VouchersList;
    [SerializeField] private GameObject m_VoucherItem;

    [SerializeField] private RawImage m_imageBackground;
    [SerializeField] private AspectRatioFitter m_aspectRatioFitter;
    [SerializeField] private RectTransform m_scanZone;
    [SerializeField] private GameObject m_scannerUI;
    [SerializeField] private Text m_QRResultText;
    [SerializeField] private GameObject m_DetailsView;

    private bool m_isCamAvailable;
    private WebCamTexture m_cameraTexture;

    private IEnumerator CreateVouchers()
    {
        var wait = new WaitForEndOfFrame();

        foreach (Transform item in m_VouchersList)
        {
            Destroy(item.gameObject);
        }

        foreach (var voucher in EVModel.Api.ScannedVouchers)
        {
            EVVoucherLineView voucherLine = Instantiate(m_VoucherItem, m_VouchersList).GetComponent<EVVoucherLineView>();
            yield return wait;
            voucherLine.Setup(voucher, ShowVoucherDetails);
        }
    }

    private void Start()
    {
        SetupCamera();
        StartCoroutine(GetVouchers());
    }

    private IEnumerator GetVouchers()
    {
        yield return APIHelper.GetAllVouchers();
        StartCoroutine(CreateVouchers());
    }

    private void SetupCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            m_isCamAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                m_cameraTexture = new WebCamTexture(devices[i].name, (int)m_scanZone.rect.width / 4, (int)m_scanZone.rect.height / 4);
            }
        }

        if (m_cameraTexture != null)
        {
            m_cameraTexture.Play();
            m_imageBackground.texture = m_cameraTexture;
            m_isCamAvailable = true;
        }
    }

    private void Update()
    {
        UpdateCameraRender();
    }

    private void UpdateCameraRender()
    {
        if (!m_isCamAvailable)
            return;

        float ratio = (float)m_cameraTexture.width / (float)m_cameraTexture.height;
        m_aspectRatioFitter.aspectRatio = ratio;

        int orientation = -m_cameraTexture.videoRotationAngle;
        m_imageBackground.rectTransform.localEulerAngles = new Vector3(0,0,orientation);

        if (m_scannerUI.activeSelf)
            Scan();
    }

    public void OnClickScan()
    {
        m_scannerUI.SetActive(true);
    }

    private void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(m_cameraTexture.GetPixels32(), m_cameraTexture.width, m_cameraTexture.height);
            if (result != null && EVModel.Api.AllVouchers.Count > 0)
            {
                foreach (var voucher in EVModel.Api.AllVouchers)
                {
                    if (voucher.id == result.Text)
                    {
                        ShowVoucherDetails(voucher);
                        break;
                    }
                }

                m_QRResultText.text = $"Unrecognized ID: {result.Text}";
            }
            else
            {
                if (EVModel.Api.AllVouchers.Count <= 0)
                {
                    m_QRResultText.text = "No voucher records found to fetch details from";
                }
                else
                {
                    m_QRResultText.text = "Scanning Voucher ID..";
                }
            }
        }
        catch
        {
            m_QRResultText.text = "Error";
        }
    }

    private void ShowVoucherDetails(Voucher data)
    {
        m_scannerUI.SetActive(false);
        m_DetailsView.SetActive(true);
        m_DetailsView.GetComponent<EVVoucherDetailsView>().UpdateDetailsView(data);
    }

    public void OnCloseScanner()
    {
        m_scannerUI.SetActive(false);
    }
}

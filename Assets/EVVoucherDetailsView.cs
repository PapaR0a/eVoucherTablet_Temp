using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EVVoucherDetailsView : MonoBehaviour
{
    [SerializeField] private Text m_PatientName;
    [SerializeField] private Text m_Organization;
    [SerializeField] private Text m_FundingType;
    [SerializeField] private Text m_Department;
    [SerializeField] private Text m_Expiration;
    [SerializeField] private Text m_QR;

    [SerializeField] private Transform m_itemsList;
    [SerializeField] private GameObject m_itemPrefab;
    [SerializeField] private GameObject m_issueButton;

    private Voucher m_Data;

    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    public void UpdateDetailsView(Voucher data)
    {
        m_Data = data;

        m_PatientName.text = data.patientName;
        m_Organization.text = $"Organization: {data.org}";
        m_FundingType.text = $"Funding Type: {data.status}";
        m_Department.text = $"Department: {data.department}";
        m_Expiration.text = $"Exp: {data.expiry_date}";
        m_QR.text = $"QR ID: {data.id}";

        m_issueButton.SetActive(data.status.ToLower() == "pending");

        StartCoroutine(CreateItems(data.items));
    }

    private IEnumerator CreateItems(VoucherProduct[] items)
    {
        var wait = new WaitForEndOfFrame();
        ClearItems();
        foreach (var product in items)
        {
            var view = Instantiate(m_itemPrefab, m_itemsList).GetComponent<EVVoucherItemView>();
            yield return wait;
            view.Setup(product.quantity, product.name);
        }
    }

    private void ClearItems()
    {
        foreach (Transform item in m_itemsList)
        {
            Destroy(item.gameObject);
        }
    }

    public void OnIssue()
    {
        APIHelper.IssueVoucher(m_Data);
        APIHelper.GetAllVouchers();
        gameObject.SetActive(false);
    }
}

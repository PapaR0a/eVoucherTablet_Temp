using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Text m_Address;
    [SerializeField] private Text m_ContactNo;
    [SerializeField] private Text m_Email;
    [SerializeField] private Text m_DeliveryDate;
    [SerializeField] private Text m_DeliveryTime;

    [SerializeField] private Transform m_itemsList;
    [SerializeField] private GameObject m_itemPrefab;
    [SerializeField] private GameObject m_issueButton;
    [SerializeField] private GameObject m_deliveryButton;

    private Voucher m_Data;

    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    public void UpdateDetailsView(Voucher data)
    {
        m_Data = data;

        m_PatientName.text = data.patientName ?? string.Empty;
        m_Organization.text = $"Organization: {data.org ?? string.Empty}";
        m_FundingType.text = $"Funding Type: {data.fundingType ?? string.Empty}";
        m_Department.text = $"Department: {data.department ?? string.Empty}";
        m_Expiration.text = $"Exp: {data.expiry_date ?? string.Empty}";
        m_QR.text = $"QR ID: {data.id ?? string.Empty}";

        m_Address.text = $"Address: {data.address ?? "--"}";
        m_ContactNo.text = $"ContactNo: {data.contactNo ?? "--"}";
        m_Email.text = $"Email: {data.email ?? "--"}";

        m_DeliveryDate.text = $"Delivery Date: {data.deliveryDate ?? "--"}";
        m_DeliveryTime.text = $"Delivery Time: {data.deliveryTime ?? "--"}";

        bool isPending = data.status.ToLower() == "pending";

        m_issueButton.SetActive(isPending);
        m_deliveryButton.SetActive(false);

        StartCoroutine(CreateItems(data.items, isPending));
    }

    private IEnumerator CreateItems(VoucherProduct[] items, bool editable = false)
    {
        var wait = new WaitForEndOfFrame();
        ClearItems();
        foreach (var product in items)
        {
            var view = Instantiate(m_itemPrefab, m_itemsList).GetComponent<EVVoucherItemView>();
            yield return wait;
            view.Setup(product.remaining, product.name, editable, product.id);
        }
    }

    private void ClearItems()
    {
        foreach (Transform item in m_itemsList)
        {
            Destroy(item.gameObject);
        }
    }

    private void UpdateActiveVoucher()
    {
        var activeVoucher = new PatchVoucherData();

        activeVoucher.patiendId = m_Data.patientId;

        foreach (var voucher in EVModel.Api.AllVouchers)
        {
            if (voucher.status.ToLower() == "active" && voucher.patientId == m_Data.patientId)
            {
                activeVoucher.voucherId = voucher.id;
                activeVoucher.items = voucher.items.ToList();
                break;
            }
        }

        List<VoucherProduct> redeemingItems = new List<VoucherProduct>();
        foreach (Transform item in m_itemsList)
        {
            EVVoucherItemView itemView = item.GetComponent<EVVoucherItemView>();
            if (itemView != null)
            {
                var redeemingItem = new VoucherProduct();
                redeemingItem.id = itemView.ItemId;
                redeemingItem.name = itemView.GetItemName();
                redeemingItem.remaining = itemView.GetRedeemingCount();
                redeemingItems.Add(redeemingItem);
            }
        }

        m_Data.items = redeemingItems.ToArray();

        foreach (var activeRemaining in activeVoucher.items)
        {
            foreach (var redeemingItem in redeemingItems)
            {
                if (activeRemaining.id == redeemingItem.id)
                {
                    activeRemaining.remaining = activeRemaining.remaining - redeemingItem.remaining;
                    break;
                }
            }
        }

        EVControl.Api.UpdateVoucherData(activeVoucher);
    }

    public void OnIssue()
    {
        UpdateActiveVoucher();
        APIHelper.IssueVoucher(m_Data);
        APIHelper.GetAllVouchers();
        gameObject.SetActive(false);
    }
}

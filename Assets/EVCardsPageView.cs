using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EVCardsPageView : MonoBehaviour
{
    [SerializeField] private Dropdown m_Dropdown;
    [SerializeField] private GameObject m_PrefCard;
    [SerializeField] private Transform m_CardsContainer;
    [SerializeField] private GameObject m_PrefHistory;
    [SerializeField] private Transform m_HistoryContainer;

    private bool m_isDataAcquired = false;

    public void OnSelectUser(int val)
    {
        if (APIHelper.GetAllVouchers())
        {
            EVModel.Api.UpdateUserVouchers(m_Dropdown.options[m_Dropdown.value].text);
            DisplayUserDetails();
        }
    }

    private void DisplayUserDetails()
    {
        ClearItems();

        List<Voucher> activeVouchers = EVModel.Api.UserActiveVouchers;
        //Debug.Log($"<color=yellow>activeVouchers {JsonConvert.SerializeObject(activeVouchers)}</color>");
        if (activeVouchers != null && activeVouchers.Count > 0)
        {
            foreach (var voucherData in activeVouchers)
            {
                StartCoroutine(CreateActiveCards(voucherData));
            }
        }

        List<Voucher> historyVouchers = EVModel.Api.UserHistoryVouchers;
        //Debug.Log($"<color=yellow>historyVouchers {JsonConvert.SerializeObject(historyVouchers)}</color>");
        if (historyVouchers != null && historyVouchers.Count > 0)
        {
            historyVouchers.Reverse();
            foreach (var voucherData in historyVouchers)
            {
                StartCoroutine(CreateHistoryCards(voucherData));
            }
        }
    }

    private void OnEnable()
    {
        if (APIHelper.GetAllVouchers())
        {
            UpdateDropdown();
            EVModel.Api.UpdateUserVouchers(m_Dropdown.options[m_Dropdown.value].text);
            DisplayUserDetails();
        }
    }

    private void ClearItems()
    {
        foreach (Transform card in m_CardsContainer)
        {
            Destroy(card.gameObject);
        }

        foreach (Transform card in m_HistoryContainer)
        {
            Destroy(card.gameObject);
        }
    }

    private void Start()
    {
        EVControl.Api.OnFinishUpdatingUserVouchers += UpdateDropdown;
        m_Dropdown.onValueChanged.AddListener(OnSelectUser);
    }

    private void UpdateDropdown()
    {
        m_Dropdown.options.Clear();
        m_Dropdown.AddOptions(EVModel.Api.UsersList);
    }

    private void OnDestroy()
    {
        EVControl.Api.OnFinishUpdatingUserVouchers -= UpdateDropdown;
        m_Dropdown.onValueChanged.RemoveAllListeners();
    }

    private void Update()
    {
        //if (!m_isDataAcquired && EVModel.Api.CachedUserData != null)
        //{
        //    m_isDataAcquired = true;
        //}
    }

    private IEnumerator CreateActiveCards(Voucher voucherData)
    {
        var wait = new WaitForEndOfFrame();
        var card = Instantiate(m_PrefCard, m_CardsContainer).GetComponent<EVCardsPageItemView>();
        yield return wait;
        card.SetupCard(voucherData);
    }

    private IEnumerator CreateHistoryCards(Voucher voucherData)
    {
        var wait = new WaitForEndOfFrame();
        var card = Instantiate(m_PrefHistory, m_HistoryContainer).GetComponent<EVHistoryPageItemView>();
        yield return wait;
        card.SetupCard(voucherData);
    }
}

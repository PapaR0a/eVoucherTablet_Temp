using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EVDeliveryView : MonoBehaviour
{
    public InputField InputAddress;
    public InputField InputNumber;
    public InputField InputEmail;

    public InputField InputDay;
    public InputField InputMonth;
    public InputField InputYear;

    public InputField InputHour;
    public InputField InputMinute;

    public Button RequestButton;
    public GameObject Message;

    public PostVoucherData VoucherData;
    private Action<PostVoucherData> OnRequestDelivery;

    public void Setup(PostVoucherData voucher, Action<PostVoucherData> requestCallback)
    {
        if (voucher == null)
            return;

        OnRequestDelivery = requestCallback;

        bool isReadOnly = voucher.voucher.status.ToLower().Contains("deliver");
        VoucherData = voucher;

        InputAddress.text = voucher.voucher.address ?? string.Empty;
        InputNumber.text = voucher.voucher.contactNo ?? string.Empty;
        InputEmail.text = voucher.voucher.email ?? string.Empty;

        if (voucher.voucher.deliveryDate != null && voucher.voucher.deliveryDate.Length == 10)
        {
            InputDay.text = voucher.voucher.deliveryDate.Substring(0,2);
            InputMonth.text = voucher.voucher.deliveryDate.Substring(3, 2);
            InputYear.text = voucher.voucher.deliveryDate.Substring(6, 4);
        }

        if (voucher.voucher.deliveryTime != null && voucher.voucher.deliveryTime.Length == 8)
        {
            InputHour.text = voucher.voucher.deliveryTime.Substring(0, 2);
            InputMinute.text = voucher.voucher.deliveryTime.Substring(3, 2);
        }

        InputAddress.interactable = !isReadOnly;
        InputNumber.interactable = !isReadOnly;
        InputEmail.interactable = !isReadOnly;

        InputDay.interactable = !isReadOnly;
        InputMonth.interactable = !isReadOnly;
        InputYear.interactable = !isReadOnly;

        InputHour.interactable = !isReadOnly;
        InputMinute.interactable = !isReadOnly;

        //RequestButton.gameObject.SetActive(!isReadOnly);
    }

    public string GetDate()
    {
        int day = int.Parse(InputDay.text);
        string strDay = day < 10 ? $"0{day}" : $"{day}";

        int month = int.Parse(InputMonth.text);
        string strMonth = month < 10 ? $"0{month}" : $"{month}";

        return $"{strDay}/{strMonth}/{InputYear.text}";
    }

    public string GetTime()
    {
        int hr = int.Parse(InputHour.text);
        string strHr = hr < 10 ? $"0{hr}" : $"{hr}";

        int min = int.Parse(InputMinute.text);
        string strmin = min < 10 ? $"0{min}" : $"{min}";

        return $"{strHr}:{strmin}:00";
    }

    public void RequestDelivery()
    {
        if (string.IsNullOrEmpty(InputAddress.text) ||
            string.IsNullOrEmpty(InputNumber.text) ||
            string.IsNullOrEmpty(InputEmail.text) ||
            string.IsNullOrEmpty(InputDay.text) ||
            string.IsNullOrEmpty(InputMonth.text) ||
            string.IsNullOrEmpty(InputYear.text) ||
            string.IsNullOrEmpty(InputHour.text) ||
            string.IsNullOrEmpty(InputMinute.text))
        {
            Message.SetActive(true);
            return;
        }   

        VoucherData.voucher.address = InputAddress.text;
        VoucherData.voucher.contactNo = InputNumber.text;
        VoucherData.voucher.email = InputEmail.text;
        VoucherData.voucher.deliveryDate = GetDate();
        VoucherData.voucher.deliveryTime = GetTime();

        OnRequestDelivery?.Invoke(VoucherData);
    }

    public void ClearFields()
    {
        InputAddress.text = string.Empty;
        InputNumber.text = string.Empty;
        InputEmail.text = string.Empty;

        InputDay.text = string.Empty;
        InputMonth.text = string.Empty;
        InputYear.text = string.Empty;

        InputHour.text = string.Empty;
        InputMinute.text = string.Empty;

        Message.SetActive(false);
    }

    public void CloseDelivery()
    {
        gameObject.SetActive(false);
    }
}

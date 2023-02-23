using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using UnityEngine;

public class EVControl
{
    #region API
    private static EVControl api;

    public static EVControl Api
    {
        get
        {
            if (api == null)
            {
                api = new EVControl();
            }
            return api;
        }
    }
    #endregion
    public Action OnFetchUsers { get; set; }
    public Action<Voucher, bool> OnShowVoucherDetails { get; set; }
    public Action<string> OnUpdateUserIdDisplay { get; set; }
    public Action OnFinishUpdatingUserVouchers { get; set; }
    public Action<bool, string> ShowPopupMessage { get; set; }

    public void Init()
    {
        // Instantiate other stuff here if needed
    }

    public void FinishUpdatingUserVouchers()
    {
        OnFinishUpdatingUserVouchers?.Invoke();
    }

    public void UpdateVoucherData(PatchVoucherData updateVoucherData)
    {
        APIHelper.UpdateVoucher(updateVoucherData);
    }

    public void DirectRedeemVoucher(RedeemVoucherDTO newVoucherData)
    {
        APIHelper.DirectRedeemVoucher(newVoucherData, (result, message) => 
        {
            Debug.LogError($"DirectRedeemVoucher Result: Successful: {result} Message: {message}");
            ShowPopupMessage?.Invoke(result, message);
        });
    }

    public void UpdatePendingVoucher(RedeemVoucherDTO newVoucherData)
    {
        APIHelper.UpdatePendingVoucher(newVoucherData, (result, message) =>
        {
            Debug.Log($"<color=yellow>UpdatePendingVoucher Result: Successful: {result} Message: {message}</color>");
            ShowPopupMessage?.Invoke(result, message);
        });
    }

    public void ShowVoucherDetails(Voucher voucher, bool readOnly = false)
    {
        OnShowVoucherDetails?.Invoke(voucher, readOnly);
    }
}

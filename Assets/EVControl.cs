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

    public void DirectRedeemVoucher(PostVoucherData newVoucherData)
    {
        APIHelper.DirectRedeemVoucher(newVoucherData, (result, message) => 
        {
            Debug.LogError($"DirectRedeemVoucher Result: Successful: {result} Message: {message}");
            ShowPopupMessage?.Invoke(result, message);
        });
    }

    public void UpdatePendingVoucher(PostVoucherData newVoucherData)
    {
        APIHelper.UpdatePendingVoucher(newVoucherData, (result, message) =>
        {
            Debug.Log($"<color=yellow>UpdatePendingVoucher Result: Successful: {result} Message: {message}</color>");
            ShowPopupMessage?.Invoke(result, message);
        });
    }

    public void CreateNewRequestDelivery(PostVoucherData newRequestData, Action callback = null)
    {
        APIHelper.CreateDeliveryRequest(newRequestData, callback);
    }

    public void UpdateRequestDelivery(PostVoucherData updateRequestData, Action callback = null)
    {
        APIHelper.UpdateDeliveryRequest(updateRequestData, callback);
    }

    public void ShowVoucherDetails(Voucher voucher, bool readOnly = false)
    {
        OnShowVoucherDetails?.Invoke(voucher, readOnly);
    }
}

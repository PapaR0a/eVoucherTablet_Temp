using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVModel
{
    public static EVModel Api { get; set; } = new EVModel();

    public List<Voucher> AllVouchers { get; set; } = new List<Voucher>();

    public List<Voucher> ScannedVouchers { get; set; } = new List<Voucher>();

    public List<Voucher> DeliveryVouchers { get; set; } = new List<Voucher>();

    public List<Voucher> ActiveVouchers { get; set; } = new List<Voucher>();

    public List<UserDetail> UsersData { get; set; } = null;

    public UserDetail UserDetail { get; set; } = null;

    public Voucher CachedCurrentVoucher { get; set; } = null;

    public List<string> UsersList { get; set; } = null;

    public List<Voucher> UserActiveVouchers = null;

    public List<Voucher> UserHistoryVouchers = null;

    public void UpdateUserVouchers(string userName)
    {
        UserActiveVouchers = new List<Voucher>();
        UserHistoryVouchers = new List<Voucher>();

        foreach (var data in UsersData)
        {
            if (data.name == userName)
            {
                UserDetail = data;
                break;
            }
        }

        foreach (var voucher in AllVouchers)
        {
            if (userName == voucher.patientName)
            {
                if (voucher.status == "active")
                {
                    UserActiveVouchers.Add(voucher);
                }
                else
                {
                    UserHistoryVouchers.Add(voucher);
                }
            }
        }
    }
}

public enum Organizations
{
    TTSH,
    WDL,
    NHGP
}

[System.Serializable]
public class UserDetail
{
    public string name;
    public string id;
    public string fundingType;
}

[System.Serializable]
public class PostVoucherData
{
    public string patientId;
    public Voucher voucher;
}

[System.Serializable]
public class PatchVoucherData
{
    public string patiendId;
    public string voucherId;
    public List<VoucherProduct> items;
}

[System.Serializable]
public class Voucher
{
    public string id;
    public string org;
    public string department;
    public string status;
    public string expiry_date;
    public string fundingType;
    public string issuer;
    public string patientName;
    public string patientId;
    public VoucherProduct[] items;
    public string email;
    public string address;
    public string contactNo;
    public string deliveryDate;
    public string deliveryTime;
}


[System.Serializable]
public class VoucherProduct
{
    public string id;
    public string name;
    public int remaining;
}

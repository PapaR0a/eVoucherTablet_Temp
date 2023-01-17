using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVModel
{
    public static EVModel Api { get; set; } = new EVModel();

    public List<Voucher> AllVouchers { get; set; } = new List<Voucher>();

    public List<Voucher> ScannedVouchers { get; set; } = new List<Voucher>();
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
}


[System.Serializable]
public class VoucherProduct
{
    public string id;
    public string name;
    public int quantity;
    public int remaining;
}

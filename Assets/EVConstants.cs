using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EVConstants
{
    public const float REFRESH_INTERVAL = 10.0f;

    public const string URL_USERDATA_TEST = "https://worker-typescript-template.cabzarmi.workers.dev/api/clinician/client/42918365";
    public const string URL_USERLIST = "https://worker-typescript-template.cabzarmi.workers.dev/api/clinician/client";
    public const string URL_USERDATA = "https://worker-typescript-template.cabzarmi.workers.dev/api/clinician/client/{0}";
    public const string URL_VOUCHER_UPDATE = "https://worker-typescript-template.cabzarmi.workers.dev/api/client/voucher/item";
    public const string URL_VOUCHER_CREATE = "https://worker-typescript-template.cabzarmi.workers.dev/api/client/voucher";
    public const string URL_VOUCHER_SCANNED = "https://worker-typescript-template.cabzarmi.workers.dev/api/scanner/vouchers";
    public const string URL_VOUCHER_SCANNED_DETAILS = "http://worker-typescript-template.cabzarmi.workers.dev/api/client/voucher/{0}?patientId={1}";
}

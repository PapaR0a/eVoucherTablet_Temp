using UnityEngine;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System;

public static class APIHelper
{
    public static bool GetScannedVouchers()
    {
        JArray voucherIds;

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(EVConstants.URL_VOUCHER_SCANNED));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();

            reader.Close();
            response.Close();

            voucherIds = JsonConvert.DeserializeObject<JArray>(json);
        }
        catch
        {
            Debug.LogError($"GetScannedVouchers failed");
            return false;
        }

        EVModel.Api.ScannedVouchers = new List<Voucher>();

        foreach (var id in voucherIds)
        {
            string voucherId = id.Value<string>("voucherId");
            foreach (var voucher in EVModel.Api.AllVouchers)
            {
                if (voucher.id == voucherId && !EVModel.Api.ScannedVouchers.Contains(voucher))
                {
                    EVModel.Api.ScannedVouchers.Add(voucher);
                    break;
                }
            }
        }

        return true;
    }

    public static bool GetScannedVoucherDetails(string voucherId, string patientId)
    {
        JArray vouchers;

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(EVConstants.URL_VOUCHER_SCANNED_DETAILS, voucherId, patientId));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();

            reader.Close();
            response.Close();

            Debug.LogError($"GetScannedVoucherDetails {json}");

            return false;

            vouchers = JsonConvert.DeserializeObject<JArray>(json);
        }
        catch
        {
            return false;
        }

        EVModel.Api.ScannedVouchers = new List<Voucher>();

        foreach (var voucher in vouchers)
        {
            try
            {
                var receivedVoucher = JsonUtility.FromJson<Voucher>(JsonConvert.SerializeObject(voucher));
                EVModel.Api.ScannedVouchers.Add(receivedVoucher);
            }
            catch
            {
                Debug.LogError($"<color=yellow>Fetch Scanned Data Failed</color>");
                return false;
            }
        }

        return true;
    }

    public static void AddScannedVoucher(Voucher data)
    {
        try
        {
            HttpWebRequest createRequest = (HttpWebRequest)WebRequest.Create(EVConstants.URL_VOUCHER_SCANNED);
            createRequest.Method = "POST";

            JArray vouchers = new JArray();

            var postData = new JObject()
            {
                ["voucherId"] = data.id,
                ["patientId"] = data.patientId
            };

            vouchers.Add(postData);

            Debug.Log($"<color=yellow>POST Json: {JsonConvert.SerializeObject(vouchers)}</color>");
            var encoded = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(vouchers));

            createRequest.ContentType = "application/json";
            Stream dataStream = createRequest.GetRequestStream();
            dataStream.Write(encoded, 0, encoded.Length);
            dataStream.Close();

            HttpWebResponse response = (HttpWebResponse)createRequest.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();

            reader.Close();
            response.Close();

            Debug.Log($"<color=yellow>Add Scanned Voucher Success {json}</color>");
        }
        catch
        {
            Debug.LogError($"<color=yellow>Add Scanned Voucher Failed</color>");
        }
    }

    public static bool GetAllVouchers()
    {
        JArray userList;

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(EVConstants.URL_USERLIST));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();

            reader.Close();
            response.Close();

            userList = JsonConvert.DeserializeObject<JArray>(json);

        }
        catch
        {
            Debug.LogError($"<color=yellow>Fetch Data Failed</color>");
            return false;
        }

        EVModel.Api.UsersData = new List<UserDetail>();
        EVModel.Api.UsersList = new List<string>();
        EVModel.Api.AllVouchers = new List<Voucher>();
        EVModel.Api.DeliveryVouchers = new List<Voucher>();
        EVModel.Api.ActiveVouchers = new List<Voucher>();

        foreach (JObject userData in userList)
        {
            try
            {
                JArray vouchersList = userData.Value<JArray>("vouchers");
                string userName = userData.Value<string>("name");

                EVModel.Api.UsersList.Add(userName);

                UserDetail userDetails = new UserDetail();
                userDetails.id = userData.Value<string>("id");
                userDetails.name = userName;
                userDetails.fundingType = userData.Value<string>("fundingType");

                EVModel.Api.UsersData.Add(userDetails);

                foreach (var voucher in vouchersList)
                {
                    try
                    {
                        var receivedVoucher = JsonUtility.FromJson<Voucher>(JsonConvert.SerializeObject(voucher));
                        receivedVoucher.patientId = userData.Value<string>("id");
                        receivedVoucher.patientName = userData.Value<string>("name");
                        EVModel.Api.AllVouchers.Add(receivedVoucher);

                        if (receivedVoucher.status.ToLower().Contains("deliver"))
                        {
                            EVModel.Api.DeliveryVouchers.Add(receivedVoucher);
                        }

                        if (receivedVoucher.status.ToLower() == "active")
                        {
                            EVModel.Api.ActiveVouchers.Add(receivedVoucher);
                        }
                    }
                    catch
                    {
                        Debug.LogError($"<color=yellow>Fetch Data Failed</color>");
                        return false;
                    }
                }
            }
            catch
            {
                Debug.LogError($"<color=yellow>Fetch Data Failed</color>");
                return false;
            }
        }

        try
        {
            GetScannedVouchers();
        }
        catch
        {
            return false;
        }

        EVControl.Api.FinishUpdatingUserVouchers();
        return true;
    }

    public static void IssueVoucher(Voucher data)
    {
        HttpWebRequest createRequest = (HttpWebRequest)WebRequest.Create(EVConstants.URL_VOUCHER_CREATE);
        createRequest.Method = "POST";

        var postData = new JObject()
        {
            ["patientId"] = data.patientId,
            ["voucher"] = new JObject()
            {
                ["id"] = data.id,
                ["org"] = data.org,
                ["department"] = data.department,
                ["status"] = "Redeemed",
                ["expiry_date"] = data.expiry_date,
                ["fundingType"] = data.fundingType,
                ["items"] = JArray.FromObject(data.items),
                ["address"] = data.address,
                ["contactNo"] = data.contactNo,
                ["email"] = data.email,
            }
        };

        //Debug.Log($"<color=yellow>POST Json: {JsonConvert.SerializeObject(postData)}</color>");
        var encoded = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(postData));

        createRequest.ContentType = "application/json";
        Stream dataStream = createRequest.GetRequestStream();
        dataStream.Write(encoded, 0, encoded.Length);
        dataStream.Close();

        HttpWebResponse response = (HttpWebResponse)createRequest.GetResponse();

        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();

        reader.Close();
        response.Close();

        Debug.Log($"<color=yellow>Issue Voucher Success</color>");
    }

    public static void UpdateVoucher(PatchVoucherData updateVoucherData)
    {
        HttpWebRequest updateRequest = (HttpWebRequest)WebRequest.Create(EVConstants.URL_VOUCHER_UPDATE);
        updateRequest.Method = "POST";

        var putData = new JObject()
        {
            ["patientId"] = updateVoucherData.patiendId,
            ["voucherId"] = updateVoucherData.voucherId,
            ["items"] = JArray.FromObject(updateVoucherData.items)
        };

        Debug.Log($"<color=yellow>POST Json: {JsonConvert.SerializeObject(putData)}</color>");
        var encoded = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(putData));

        updateRequest.ContentType = "application/json";
        Stream dataStream = updateRequest.GetRequestStream();
        dataStream.Write(encoded, 0, encoded.Length);
        dataStream.Close();
        HttpWebResponse updateResponse = (HttpWebResponse)updateRequest.GetResponse();

        StreamReader updateReader = new StreamReader(updateResponse.GetResponseStream());
        string json = updateReader.ReadToEnd();

        updateReader.Close();
        updateResponse.Close();

        Debug.Log($"<color=yellow>UpdateVoucher Success</color>");
    }

    public static void DirectRedeemVoucher(PostVoucherData data, Action<bool, string> callback = null)
    {
        HttpWebRequest createRequest = (HttpWebRequest)WebRequest.Create(EVConstants.URL_REDEEM_VOUCHER);
        createRequest.Method = "POST";

        var postData = new JObject()
        {
            ["patientId"] = data.patientId,
            ["voucher"] = new JObject()
            {
                ["redeemId"] = data.voucher.id,
                ["items"] = JArray.FromObject(data.voucher.items),
                ["address"] = data.voucher.address,
                ["contactNo"] = data.voucher.contactNo,
                ["email"] = data.voucher.email,
                ["deliveryDate"] = data.voucher.deliveryDate,
                ["deliveryTime"] = data.voucher.deliveryTime
            }
        };

        Debug.Log($"<color=yellow>DirectRedeemVoucher Json: {JsonConvert.SerializeObject(postData)}</color>");
        var encoded = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(postData));

        createRequest.ContentType = "application/json";
        Stream dataStream = createRequest.GetRequestStream();
        dataStream.Write(encoded, 0, encoded.Length);
        dataStream.Close();

        HttpWebResponse response = (HttpWebResponse)createRequest.GetResponse();

        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();

        JObject result  = JsonConvert.DeserializeObject<JObject>(json);
        bool isSuccess = result.Value<int>("code") == 200;
        string message = result.Value<string>("message") ?? string.Empty;
        callback?.Invoke(isSuccess, message);

        reader.Close();
        response.Close();
    }

    public static void UpdatePendingVoucher(PostVoucherData data, Action<bool, string> callback = null)
    {
        HttpWebRequest createRequest = (HttpWebRequest)WebRequest.Create(EVConstants.URL_UPDATE_PENDING_VOUCHER);
        createRequest.Method = "POST";

        var postData = new JObject()
        {
            ["patientId"] = data.patientId,
            ["voucher"] = new JObject()
            {
                ["id"] = data.voucher.id,
                ["items"] = JArray.FromObject(data.voucher.items),
            }
        };

        Debug.Log($"<color=yellow>UpdatePendingVoucher Json: {JsonConvert.SerializeObject(postData)}</color>");
        var encoded = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(postData));

        createRequest.ContentType = "application/json";
        Stream dataStream = createRequest.GetRequestStream();
        dataStream.Write(encoded, 0, encoded.Length);
        dataStream.Close();

        HttpWebResponse response = (HttpWebResponse)createRequest.GetResponse();

        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();

        JObject result = JsonConvert.DeserializeObject<JObject>(json);
        bool isSuccess = result.Value<int>("code") == 200;
        string message = result.Value<string>("message") ?? string.Empty;
        callback?.Invoke(isSuccess, message);

        reader.Close();
        response.Close();
    }

    public static void CreateDeliveryRequest(PostVoucherData data, Action callback = null)
    {
        HttpWebRequest createRequest = (HttpWebRequest)WebRequest.Create(EVConstants.URL_CREATE_REQUEST_DELIVERY);
        createRequest.Method = "POST";

        var postData = new JObject()
        {
            ["patientId"] = data.patientId,
            ["voucher"] = new JObject()
            {
                ["redeemId"] = data.voucher.id,
                ["items"] = JArray.FromObject(data.voucher.items),
                ["address"] = data.voucher.address,
                ["contactNo"] = data.voucher.contactNo,
                ["email"] = data.voucher.email,
                ["deliveryDate"] = data.voucher.deliveryDate,
                ["deliveryTime"] = data.voucher.deliveryTime
            }
        };

        Debug.Log($"<color=yellow>POST Json: {JsonConvert.SerializeObject(postData)}</color>");
        var encoded = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(postData));

        createRequest.ContentType = "application/json";
        Stream dataStream = createRequest.GetRequestStream();
        dataStream.Write(encoded, 0, encoded.Length);
        dataStream.Close();

        HttpWebResponse response = (HttpWebResponse)createRequest.GetResponse();

        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();

        reader.Close();
        response.Close();

        callback?.Invoke();

        Debug.Log($"<color=yellow>Request Delivery Success: {json}</color>");
    }

    public static void UpdateDeliveryRequest(PostVoucherData data, Action callback = null)
    {
        HttpWebRequest createRequest = (HttpWebRequest)WebRequest.Create(EVConstants.URL_UPDATE_REQUEST_DELIVERY);
        createRequest.Method = "POST";

        var postData = new JObject()
        {
            ["patientId"] = data.patientId,
            ["voucher"] = new JObject()
            {
                ["id"] = data.voucher.id,
                ["items"] = JArray.FromObject(data.voucher.items),
                ["address"] = data.voucher.address,
                ["contactNo"] = data.voucher.contactNo,
                ["email"] = data.voucher.email,
                ["deliveryDate"] = data.voucher.deliveryDate,
                ["deliveryTime"] = data.voucher.deliveryTime
            }
        };

        Debug.Log($"<color=yellow>POST Json: {JsonConvert.SerializeObject(postData)}</color>");
        var encoded = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(postData));

        createRequest.ContentType = "application/json";
        Stream dataStream = createRequest.GetRequestStream();
        dataStream.Write(encoded, 0, encoded.Length);
        dataStream.Close();

        HttpWebResponse response = (HttpWebResponse)createRequest.GetResponse();

        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();

        reader.Close();
        response.Close();

        callback?.Invoke();

        Debug.Log($"<color=yellow>Request Delivery Success: {json}</color>");
    }
}

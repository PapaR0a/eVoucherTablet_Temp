using UnityEngine;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

public static class APIHelper
{
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
            Debug.Log($"<color=yellow>Fetch Data Failed</color>");
            return false;
        }

        EVModel.Api.UsersData = new List<UserDetail>();
        EVModel.Api.UsersList = new List<string>();
        EVModel.Api.AllVouchers = new List<Voucher>();
        EVModel.Api.ScannedVouchers = new List<Voucher>();

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

                        if (receivedVoucher.status == "Pending" || receivedVoucher.status == "Redeemed")
                        {
                            EVModel.Api.ScannedVouchers.Add(receivedVoucher);
                        }
                    }
                    catch
                    {
                        Debug.Log($"<color=yellow>Fetch Data Failed</color>");
                        return false;
                    }
                }
            }
            catch
            {
                Debug.Log($"<color=yellow>Fetch Data Failed</color>");
                return false;
            }
        }

        Debug.Log($"<color=yellow>Fetch Data Success</color>");
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
                ["items"] = JArray.FromObject(data.items)
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

    public static void DirectRedeemVoucher(PostVoucherData data)
    {
        HttpWebRequest createRequest = (HttpWebRequest)WebRequest.Create(EVConstants.URL_VOUCHER_CREATE);
        createRequest.Method = "POST";

        var postData = new JObject()
        {
            ["patientId"] = data.patientId,
            ["voucher"] = new JObject()
            {
                ["id"] = data.voucher.id,
                ["org"] = data.voucher.org,
                ["department"] = data.voucher.department,
                ["status"] = "Redeemed",
                ["expiry_date"] = data.voucher.expiry_date,
                ["fundingType"] = EVModel.Api.UserDetail.fundingType,
                ["items"] = JArray.FromObject(data.voucher.items)
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

        Debug.Log($"<color=yellow>Issue Voucher Success</color>");
    }
}

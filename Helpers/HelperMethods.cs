using QRCoder;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Globalization;
using System.Data;

namespace MMS.Helpers
{
    public class ParamAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var data = filterContext.HttpContext.Request.QueryString.Get("q");

            if (data != null)
            {
                string decryptedData = data.DecryptVar();
                string[] paramArr = decryptedData.Split('&');
                foreach (var item in paramArr)
                {
                    var d = item.Split('=');
                    filterContext.ActionParameters[d[0]] = d[1];
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
    public static class HelperMethods
    {
        static string EncryptionKey = "abcdefghijklmnopqrstuvwxyz0123456789";
        private static string GetQRCode(string RackNumber,string ItemCategory,string ItemDescription,string ItemName,string ItemCode,string SKUCode,string Unit)
        {
            string imgUrl = "";
            string information = "Item Location/Rack Number : " + RackNumber + " , Item Category : " +ItemCategory+ " , Description : "+ItemDescription+" , Item Name: "+ItemName+" , Item Code : "+ItemCode+" , SKU Code : "+SKUCode+" , Unit : "+Unit;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(information, QRCodeGenerator.ECCLevel.Q);
            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    imgUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }
            }
            return imgUrl;

        }

        public static string GetPCs(string ProjectId,string PCID)
        {
            string Baseurl = "http://208.109.9.215/SITACENTRAL/";
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseTask = client.GetAsync("api/PMCDropdown/BindPCByProjectId?ProjectId=" + ProjectId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    string stringData = result.Content.ReadAsStringAsync().Result;

                    var data= new JavaScriptSerializer().Deserialize<List<VMPC>>(stringData);
                    var pcName = data.SingleOrDefault(x => x.Value == PCID);
                    return pcName.Text;
                }
                else
                {
                    return "";
                }
            }
        }

        public static List<VMPC> GetPCList(string ProjectId)
        {
            string Baseurl = "http://208.109.9.215/SITACENTRAL/";
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseTask = client.GetAsync("api/PMCDropdown/BindPCByProjectId?ProjectId=" + ProjectId);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    string stringData = result.Content.ReadAsStringAsync().Result;

                    var data = new JavaScriptSerializer().Deserialize<List<VMPC>>(stringData);
                    return data.OrderBy(x=>x.Text).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public static string GetFinancialYear(DateTime date)
        {
            int year = date.Year;

            int month = date.Month;

            int day = date.Day;

            if (month < 4)
            {
                year = year - 1;
            }

            var yy = (year + 1).ToString().Substring(2, 2);

            return year + "-" + yy;
        }

        public static DateTime GetDateTimeConverted(this string date)
        {

            DateTime d = DateTime.ParseExact(date, "dd-MM-yyyy",
                                CultureInfo.InvariantCulture);

            return d;

        }

        public static string ToServerDateTimeFormatString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static List<VMPC> GetItemGroups(MMS.DAL.MMSEntities context)
        {
            return context.tblItemGroupMasters.Select(x => new VMPC{ Text = x.GroupName, Value = x.ItemGroupID }).OrderBy(x => x.Text).ToList();
        }

        public static List<VMPC> GetGroupItems(string GroupId,MMS.DAL.MMSEntities context)
        {
            return context.tblItemMasters.Where(x=>x.ItemGroupID==GroupId && x.ItemName!=null).Select(x => new VMPC { Text = x.ItemName.TrimStart(), Value = x.ItemID }).OrderBy(x => x.Text).ToList();
        }

        public static string EncryptVar(this string data)
        {
            return Encrypt(data);
        }

        public static string DecryptVar(this string data)
        {
            return Decrypt(data);
        }
        public static string Encrypt(string clearText)
        {

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string DataTableToJSON(this DataTable table)
        {
            if(table !=null && table.Rows.Count > 0)
            {
                var JSONString = new StringBuilder();
                if (table.Rows.Count > 0)
                {
                    JSONString.Append("[");
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        JSONString.Append("{");
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            if (j < table.Columns.Count - 1)
                            {
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                            }
                            else if (j == table.Columns.Count - 1)
                            {
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                            }
                        }
                        if (i == table.Rows.Count - 1)
                        {
                            JSONString.Append("}");
                        }
                        else
                        {
                            JSONString.Append("},");
                        }
                    }
                    JSONString.Append("]");
                }
                return JSONString.Replace("\n","").ToString();
            }
            return "";
            
        }
    }

    public static class Repos
    {
        public static int GetUserDivision()
        {
            return Convert.ToInt32(HttpContext.Current.Session["Division"].ToString());
        }
    }

    public class VMPC
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
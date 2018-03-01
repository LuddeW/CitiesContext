using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CitiesConext
{
    public class GoogleApiHandler
    {
        HttpWebRequest myHttpWebRequest;
        string postData;
        string accessToken = "";

        long now;
        long yesterday;

        public GoogleApiHandler()
        {
            now = (long)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            yesterday = (long)DateTime.Now.AddDays(-1).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

        }

        public int GetSteps()
        {
            InitRefreshRequest();
            SendRefreshTokenRequest();
            InitStepRequest();
            return SendStepRequest();
        }

        void InitRefreshRequest()
        {
            myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/oauth2/v4/token");
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            postData = "client_id=201588886496-3a2ot27qinou8es6ttdj5e7b9ol66d3g.apps.googleusercontent.com&client_secret=c0GoDGqJFbIf1hlx_h-RoCAm&refresh_token=1/5HYlkTysflKXeLC7pMMU9Xn0AtwPpcnB7eJZizhj2Ts&grant_type=refresh_token";
        }

        void InitStepRequest()
        {
            myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/fitness/v1/users/me/dataset:aggregate");
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            myHttpWebRequest.ContentType = "application/json";
            postData =  "{ 'aggregateBy': [{'dataTypeName':'com.google.step_count.delta','dataSourceId':'derived:com.google.step_count.delta:com.google.android.gms:estimated_steps'}],'bucketByTime':{'durationMillis':86400000},'startTimeMillis':" + yesterday + ",'endTimeMillis':" + now + "}";
        }

        void SendRefreshTokenRequest()
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(postData);
            myHttpWebRequest.ContentLength = byte1.Length;

            Stream newStream;

            try
            {
                newStream = myHttpWebRequest.GetRequestStream();
            }
            catch (Exception e)
            {
                throw;
            }

            newStream.Write(byte1, 0, byte1.Length);
            newStream.Close();

            WebResponse response = myHttpWebRequest.GetResponse();

            newStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(newStream);

            string responseFromServer = reader.ReadToEnd();

            AcessKeyModel model = UnityEngine.JsonUtility.FromJson<AcessKeyModel>(responseFromServer);
            accessToken = model.access_token;

        }

        public bool MyRemoteCertificateValidationCallback(System.Object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        continue;
                    }
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                        break;
                    }
                }
            }
            return isOk;
        }
        int SendStepRequest()
        {
            InitStepRequest();
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] stepdata = encoder.GetBytes(postData);            
            myHttpWebRequest.ContentLength = stepdata.Length;
            myHttpWebRequest.GetRequestStream().Write(stepdata, 0, stepdata.Length);
           
            using (HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine("Not OK");
                }
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    string str = sr.ReadToEnd();

                    string step = getBetween(str, "intVal\": ", ",");

                    return int.Parse(step);
                }
            }                
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
    }
}

using ColossalFramework.Plugins;
using SimpleJSON;
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
        static string uri = "https://www.googleapis.com/oauth2/v4/token";
        HttpWebRequest myHttpWebRequest;
        string postData;

        long now;
        long yesterday;

        public GoogleApiHandler()
        {
            now = (long)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            yesterday = (long)DateTime.Now.AddDays(-1).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            InitRefreshRequest();
        }

        void InitRefreshRequest()
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "ApiHandler construtor has been called.");
            myHttpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            postData = "client_id=201588886496-3a2ot27qinou8es6ttdj5e7b9ol66d3g.apps.googleusercontent.com&client_secret=c0GoDGqJFbIf1hlx_h-RoCAm&refresh_token=1/5HYlkTysflKXeLC7pMMU9Xn0AtwPpcnB7eJZizhj2Ts&grant_type=refresh_token";
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "ApiHandler construtor has finished.");
        }

        void InitAcessRequest(string token)
        {
            myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/fitness/v1/users/me/dataset:aggregate");
            myHttpWebRequest.Method = WebRequestMethods.Http.Post;
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.Headers.Add("Authorization", "Bearer " + token);
            myHttpWebRequest.ContentType = "application/json";
            postData =  "{ 'aggregateBy': [{'dataTypeName':'com.google.step_count.delta','dataSourceId':'derived:com.google.step_count.delta:com.google.android.gms:estimated_steps'}],'bucketByTime':{'durationMillis':86400000},'startTimeMillis':" + yesterday + ",'endTimeMillis':" + now + "}";
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Now and yesterday" + now + yesterday);
        }

        public void SendRefreshTokenRequestRequest()
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
            
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, model.access_token);

            SendAccessTokenRequest(model.access_token);

        }

        public bool MyRemoteCertificateValidationCallback(System.Object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain,
            // look at each error to determine thes cause.
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
        public void SendAccessTokenRequest(string token)
        {
            InitAcessRequest(token);
            //ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
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

                    Bucket bucket = UnityEngine.JsonUtility.FromJson<Bucket>(str);

                    //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, str);
                    ////JSONObject json = new JSONObject();
                    ////json.Add("root", JSON.Parse(str));
                    //JSONObject json = new JSONObject();
                    //JSONNode node = JSONNode.Parse(str);

                    DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, step);
                }
            }

            //StreamReader reader = new StreamReader(newStream);

            //string responseFromServer = reader.ReadToEnd();

            

            
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

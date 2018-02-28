using CitiesConext.Source;
using ColossalFramework.Plugins;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

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

        }

        public bool MyRemoteCertificateValidationCallback(System.Object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain,
            // look at each error to determine the cause.
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
    }
}

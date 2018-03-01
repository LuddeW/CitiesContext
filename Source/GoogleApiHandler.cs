
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

        void InitAcessRequest(string token)
        {
            myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/fitness/v1/users/me/dataset:aggregate");
            myHttpWebRequest.Method = WebRequestMethods.Http.Post;
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.Headers.Add("Authorization", "Bearer " + token);
            myHttpWebRequest.ContentType = "application/json";
            postData =  "{ 'aggregateBy': [{'dataTypeName':'com.google.step_count.delta','dataSourceId':'derived:com.google.step_count.delta:com.google.android.gms:estimated_steps'}],'bucketByTime':{'durationMillis':86400000},'startTimeMillis':" + yesterday + ",'endTimeMillis':" + now + "}";
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

            SendStepRequest(model.access_token);

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
        public void SendStepRequest(string token)
        {
            InitAcessRequest(token);
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] stepdata = encoder.GetBytes(postData);            
            myHttpWebRequest.ContentLength = stepdata.Length;

            Stream newStream;

            try
            {
                newStream = myHttpWebRequest.GetRequestStream();
            }
            catch (Exception e)
            {
                throw;
            }

            newStream.Write(stepdata, 0, stepdata.Length);
            newStream.Close();

            WebResponse response = myHttpWebRequest.GetResponse();


            

            newStream = response.GetResponseStream();

            

            StreamReader reader = new StreamReader(newStream);

            //ReadFully(newStream, 4);
            byte[] test = ReadFully(newStream, 0);
            string s = System.Text.Encoding.UTF8.GetString(test, 0, test.Length);
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, s);

            string responseFromServer = reader.ReadToEnd();

            
            

            Bucket bucket = UnityEngine.JsonUtility.FromJson<Bucket>(responseFromServer);

            //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, responseFromServer);
        }

        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="initialLength">The initial buffer length</param>
        public static byte[] ReadFully(Stream stream, int initialLength)
        {
            // If we've been passed an unhelpful initial length, just
            // use 32K.
            if (initialLength < 1)
            {
                initialLength = 32768;
            }

            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Entered read fully!");
            byte[] buffer = new byte[initialLength];
            int read = 0;


            int chunk;
            while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
            {
                read += chunk;


                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();


                    // End of stream? If so, we're done
                    if (nextByte == -1)
                    {
                        return buffer;
                    }


                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, read);

            return ret;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ColossalFramework.Plugins;

namespace CitiesConext
{
    public class GoogleApiHandler
    {
        HttpWebRequest myHttpWebRequest;
        string postData;
        string accessToken = "";
        List<SpeedModel> speedModels = new List<SpeedModel>();
        private static string refresh_token = "1/hGTNfC-LKRsYk8ms0xrZeJsj1TsXfr11m9UqDx285gw";
        private static string client_id = "201588886496-h7an999j4h8sdir55q697m3svsta00ei.apps.googleusercontent.com";
        private static string client_secret = "PuaqSlu3cyS6KAm7E9iaMBYN";

        long now;
        long then;

        public GoogleApiHandler()
        {
            now = (long)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            then = (long)DateTime.Now.AddDays(-1).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public int GetSteps()
        {
            then = (long)DateTime.Now.AddDays(-1).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            //HÄMTA DEN FÖRSTA
            InitRefreshRequest();
            SendRefreshTokenRequest();
            InitStepRequest(then);
            return SendStepRequest();
        }

        public int GetAvgSteps()
        {
            then = (long)DateTime.Now.AddDays(-3).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            InitStepRequest(then);
            int avgSteps = SendStepRequest();
            return avgSteps / 3;
        }

        public List<SpeedModel> GetSpeedModels()
        {
            InitSpeedRequest();
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Entering Get speed models");
            UTF8Encoding encoder = new UTF8Encoding();
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "encoder created");
            byte[] speedData = encoder.GetBytes(postData);
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "speeddata created");
            myHttpWebRequest.ContentLength = speedData.Length;
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Speedlength collected");
            myHttpWebRequest.GetRequestStream().Write(speedData, 0, speedData.Length);
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "GetRequest called");
            string str;

            using (HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse())
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Inside the using statement");
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "NOT OK response from GetSpeeds");
                }
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "trying to read from sr from inside second using statement");
                    str = sr.ReadToEnd();
                }
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Calling getAllSpeedFromJson()");
            return getAllSpeedsFromJson(str, "fpVal\": ", "apVal");
        }

        void InitRefreshRequest()
        {
            myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/oauth2/v4/token");
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            postData = "client_id=" + client_id + "&client_secret=" + client_secret + "&refresh_token=" + refresh_token + "&grant_type=refresh_token";
        }

        void InitStepRequest(long then)
        {
            myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/fitness/v1/users/me/dataset:aggregate");
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            myHttpWebRequest.ContentType = "application/json";
            postData =  "{ 'aggregateBy': [{'dataTypeName':'com.google.step_count.delta','dataSourceId':'derived:com.google.step_count.delta:com.google.android.gms:estimated_steps'}],'bucketByTime':{'durationMillis':86400000},'startTimeMillis':" + then + ",'endTimeMillis':" + now + "}";
        }

        public void InitSpeedRequest()
        {
            double yMorning = (long)DateTime.Today.AddHours(-15).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; //09 yesterday morning
            double yEvening = (long)DateTime.Today.AddHours(-6).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; //18 yesterday evning

            myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/fitness/v1/users/me/dataset:aggregate");
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            myHttpWebRequest.ContentType = "application/json";
            postData = "{ 'aggregateBy':[{'dataTypeName':'com.google.speed','':''}],'bucketByTime':{'durationMillis':'3600000'},'startTimeMillis':'" + yMorning + "','endTimeMillis':'" + yEvening + "'}";
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
            //InitStepRequest();
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] stepdata = encoder.GetBytes(postData);            
            myHttpWebRequest.ContentLength = stepdata.Length;
            myHttpWebRequest.GetRequestStream().Write(stepdata, 0, stepdata.Length);
           
            using (HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "NOT OK response from SendStepReq");
                }
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    string str = sr.ReadToEnd();

                    string step = getBetween(str, "intVal\": ", ",");

                    int steps = 0;
                    Int32.TryParse(step, out steps);

                    return steps;
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

        public List<SpeedModel> getAllSpeedsFromJson(string strSource, string strStart, string strEnd)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Entering getAllSpeedsFromJson");
            List<int> Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.AllIndexesOf(strStart);
                End = strSource.AllIndexesOf(strEnd);

                //string retStr = strSource.Substring(Start, End - Start);

                List<float> retList = new List<float>();
                string valueString = "";
                for (int i = 0; i < Start.Count; i++)
                {
                    valueString = "";
                    for (int j = Start[i] + strStart.Length; j < End[i] - (strEnd.Length + 8); j++)
                    {
                        valueString += strSource[j];
                    }
                    float value;
                    float.TryParse(valueString, out value);
                    retList.Add(value);

                    if (retList.Count == 3) //Now we have all ´three speed values read. We now need to find end and start time.
                    {
                        SpeedModel sm = new SpeedModel();
                        sm.speeds.Add(retList[0]);
                        sm.speeds.Add(retList[1]);
                        sm.speeds.Add(retList[2]);
                        speedModels.Add(sm);
                        retList.Clear();

                    }
                }
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Calling SetAllStartEnd()");
                SetAllStartEndTimesForSpeedModels(strSource, speedModels);
            }

            return speedModels;
        }

        public void SetAllStartEndTimesForSpeedModels(string strSource, List<SpeedModel> speedModels)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Entering SetAllStartEnds");
            //StartTimes
            string strStart, strEnd;
            strStart = "startTimeNanos";
            strEnd = "endTimeNanos";

            List<int> Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.AllIndexesOf(strStart); //Fill list with all start indexes of the stat word
                End = strSource.AllIndexesOf(strEnd); //-""-  for the end word
                List<long> startTimes = new List<long>(); //List that will hold all start times
                string startTimeString;
                for (int i = 0; i < Start.Count; i++)
                {
                    startTimeString = "";
                    for (int j = Start[i] + strStart.Length + 4; j < End[i] - (strEnd.Length - 1); j++) //Start at the start of start word + length of the word plus some spaces. End at end minus a space
                    {
                        startTimeString += strSource[j];
                    }
                    long parsedStartTime;
                    long.TryParse(startTimeString, out parsedStartTime); //Parse tge start time
                    speedModels[i].startTime = parsedStartTime; //Set the start time of the correct speed model
                }
                Start.Clear();
                End.Clear(); //Clear them for next session
            }

            //EndTimes
            strEnd = "dataTypeName";
            strStart = "endTimeNanos";

            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.AllIndexesOf(strStart); //Fill list with all start indexes of the stat word
                End = strSource.AllIndexesOf(strEnd); //-""-  for the end word
                List<long> startTimes = new List<long>(); //List that will hold all start times
                string startTimeString;
                for (int i = 0; i < Start.Count; i++)
                {
                    startTimeString = "";
                    for (int j = Start[i] + strStart.Length + 4; j < End[i] - (strEnd.Length - 1); j++) //Start at the start of start word + length of the word plus some spaces. End at end minus a space
                    {
                        startTimeString += strSource[j];
                    }
                    long parsedStartTime;
                    long.TryParse(startTimeString, out parsedStartTime); //Parse tge start time
                    speedModels[i].endtime = parsedStartTime; //Set the start time of the correct speed model
                }
            }

        }

    }
}

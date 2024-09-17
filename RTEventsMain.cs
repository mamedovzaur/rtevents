using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Net;
using System.Collections.Specialized;
using System.Reflection;
using Ini;

namespace RTEvents
{
    public partial class RTEventsMain : Form
    {
        private string ini_fayl;
        public string tam_unvan;
        public string timestamp;
        public string ips;
        public string d_ip;
        public string startTime = "";
        public string endTime = "";
        public int closems = 60000;
        //public bool discon = false;
        public string sToken = "";

        public string[] device_ip;
        public int ErrorCode = 0;
        public string[] F18_IP1 = { "10.131.2.40" };
        //10.131.1.40
        // "192.168.87.3", "10.90.139.201", "10.101.6.183", "10.30.139.50",  "192.168.80.3", "10.90.139.200", "192.168.4.3", "10.20.140.143", "10.101.1.102", "10.127.139.53", "192.168.23.3", "10.100.139.14", "10.52.74.132", "10.90.139.203", "10.90.139.202", "10.110.19.151", "192.168.151.3", "10.110.19.152", "192.168.160.3", "192.168.49.3", "192.168.31.3", "192.168.76.3", "10.110.19.153", "192.168.163.201", "192.168.3.3", "192.168.22.3", "10.30.139.51", "192.168.46.3", "10.101.8.149", "10.30.139.52", "10.30.139.53", "10.30.139.54", "10.101.4.141", "10.101.4.142", "10.90.139.204", "10.127.139.4", "192.168.86.90", "192.168.158.3", "195.195.195.195" };
        // ,F18 { "192.168.87.3", "10.90.139.201", "10.101.6.183", "10.30.139.50", "192.168.80.3", "10.90.139.200", "192.168.4.3", "10.20.140.143", "10.101.1.102", "10.127.139.53", "192.168.23.3", "10.100.139.14", "10.52.74.132", "10.90.139.203", "10.90.139.202", "10.110.19.151", "192.168.151.3", "10.110.19.152", "192.168.160.3", "192.168.49.3", "192.168.31.3", "192.168.76.3", "10.110.19.153", "192.168.163.201", "192.168.3.3", "192.168.22.3", "10.30.139.51", "192.168.46.3", "10.101.8.149", "10.30.139.52", "10.30.139.53", "10.30.139.54", "10.101.4.141", "10.101.4.142", "10.90.139.204", "10.127.139.4", "192.168.86.90", "192.168.158.3", "195.195.195.195" };
        //TFT { "10.50.139.201", "10.100.139.4", "10.100.139.6", "10.100.139.7", "10.100.139.13", "10.100.139.2", "10.100.139.11", "10.60.139.5", "10.100.139.10", "10.70.139.11", "10.100.139.5", "10.10.139.3", "10.10.139.2", "10.10.139.4", "10.10.139.8", "10.10.139.9", "10.10.139.6", "10.10.139.7", "10.100.139.3", "10.100.139.8", "10.50.139.10", "10.50.139.105", "10.60.139.5", "10.80.0.215", "10.101.6.183","195.195.195.195" };





        public RTEventsMain()
        {
            InitializeComponent();
            this.ini_fayl = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config.ini";
            IniFile iniFile = new IniFile(this.ini_fayl);
            this.tam_unvan = iniFile.IniReadValue("umumi", "URL");
            this.timestamp = iniFile.IniReadValue("umumi", "checksum");
            Int32.TryParse(iniFile.IniReadValue("umumi", "closems"), out closems);
            this.startTime = iniFile.IniReadValue("logclear", "start");
            this.endTime = iniFile.IniReadValue("logclear", "end");
            this.ips = iniFile.IniReadValue("network", "ips");
            this.d_ip = iniFile.IniReadValue("network", "devip");

            timerRestart.Interval = closems;
            timerRestart.Enabled = false;
            Logger("Server", "Start applicaton");
            //All_Devices();
        }

        public static Dictionary<string, zkemkeeper.CZKEMClass> device2FingersList = new Dictionary<string, zkemkeeper.CZKEMClass>();
        public static Dictionary<string, string> device2type = new Dictionary<string, string>();
        public static Dictionary<string, int> LastFingTime = new Dictionary<string, int>();
        public static Dictionary<string, bool> OnlineDev = new Dictionary<string, bool>();
        public static Dictionary<string, bool> DevConnected = new Dictionary<string, bool>();
        public static Dictionary<string, int> MashineNumber = new Dictionary<string, int>();
        public static Dictionary<string, string> DevPlatform = new Dictionary<string, string>();
        public static Dictionary<string, string> DevSerial = new Dictionary<string, string>();
        //public static Dictionary<string, string> SerialNumber = new Dictionary<string, string>();
        public Dictionary<string, DateTime> SendedMail = new Dictionary<string, DateTime>();
        Dictionary<string, Thread> DeviceThread = new Dictionary<string, Thread>();
        Dictionary<string, List<string>> Actions = new Dictionary<string, List<string>>();
        List<Thread> ThrArr = new List<Thread>();


        private int iMachineNumber = 1;

        public void Logger(string ip,string logtxt)
        {
            string FilePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\log\\" + DateTime.Now.ToString("MM.dd.yyyy");
            System.IO.Directory.CreateDirectory(FilePath);
            string log_fayl = FilePath + "\\" + ip + ".log";
            FileWrite(log_fayl, DateTime.Now.ToString() + " | " + logtxt + " \r\n");
            string log_fayl1 = FilePath + "\\" + "All.log";
            FileWrite(log_fayl1, DateTime.Now.ToString() + " | " + logtxt + " \r\n");
        }
        private void rtTimer_Tick(object sender, EventArgs e)
        {
            rtTimer.Enabled = false;
            All_Devices();
            // timerReconnect.Enabled = true;
        }





        private bool PingHost(string nameOrAddress)
        {
            Logger(nameOrAddress,"Ping: " + nameOrAddress);
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(nameOrAddress, 2000);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {

            }

            pinger.Dispose();
            return pingable;
        }

        public class WebClientWithTimeout : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest wr = base.GetWebRequest(address);
                wr.Timeout = 30000; // timeout in milliseconds (ms)
                return wr;
            }
        }

        public string DownloadFromURL(string address)
        {
            // GetProPassToken("dservice", "123456", "0");
            if (this.sToken != "")
            {
                string reply = "";
                try
                {
                    WebClient client = new WebClientWithTimeout();
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)");
                    client.Headers.Set("Authorization", "Bearer " + this.sToken);

                    reply = client.DownloadString(address);
                }
                catch (WebException E)
                {
                    Logger("Server","Download Exception: " + E.Message);
                    reply = "";
                }


                return reply;
            }
            else
            {
                return "";
            }

        }

        public long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }
       

        private void ConnectDevice1(string deviceIp)
        {
            int key = 0;
            string reply_key = DownloadFromURL(this.tam_unvan + "/api/ex/fp-server/auth-key/" + deviceIp);
            //Logger(deviceIp,reply_key);
            var apikeyreply = JsonConvert.DeserializeObject<ConnectKey>(reply_key);
            if (apikeyreply.status)
            { key = Convert.ToInt32(apikeyreply.auth_key); }
            Logger(deviceIp, "Connecting... " + deviceIp);
           
            if (!PingHost(deviceIp))
            {

                ErrorCode = -111;
                Logger("ping_error", "ErrorCode: " + ErrorCode + " ( " + deviceIp + " (2)) cannot ping!");
                device2FingersList.Remove("device_" + deviceIp);
                device2type.Remove(deviceIp);
                LastFingTime.Remove(deviceIp);
                DevConnected.Remove(deviceIp);
                return;
            }
            if (!RTEventsMain.device2FingersList.ContainsKey("device_" + deviceIp))
            {
                device2FingersList["device_" + deviceIp] = new zkemkeeper.CZKEMClass();
            }


            string ser_num = "";


            // if (!DevConnected.ContainsKey(deviceIp))
            //  {
            RTEventsMain.device2FingersList["device_" + deviceIp].SetCommPassword(key);
            var first= DateTime.Now;
            DevConnected[deviceIp] = RTEventsMain.device2FingersList["device_" + deviceIp].Connect_Net(deviceIp, 4370);

            if (DevConnected[deviceIp])
            {
                var second = DateTime.Now;
                var diff = second.TimeOfDay-first.TimeOfDay;
                MashineNumber[deviceIp] = 1;
                Logger(deviceIp, deviceIp + " connected! "+diff.ToString());
                string platform = "";
                device2FingersList["device_" + deviceIp].GetPlatform(1, ref platform);
                device2FingersList["device_" + deviceIp].GetSerialNumber(1, out ser_num);
                Logger(deviceIp, deviceIp + " platform: " + platform + " serial: " + ser_num);

                string nowTime = DateTime.Now.ToString("HH:mm:ss");
                if (DateTime.Parse(nowTime) > DateTime.Parse(this.startTime) && DateTime.Parse(nowTime) < DateTime.Parse(this.endTime))
                {
                    device2FingersList["device_" + deviceIp].DisableDeviceWithTimeOut(1, 10);
                    Logger(deviceIp, deviceIp + " time to disable!");
                }
                else
                {
                    device2FingersList["device_" + deviceIp].EnableDevice(1, true);
                    Logger(deviceIp, deviceIp + " not time to disable!");
                }

                device2FingersList["device_" + deviceIp].CancelOperation();
                Logger(deviceIp, deviceIp + " platform: " + platform + " serial: " + ser_num);
                if (device2FingersList["device_" + deviceIp].IsTFTMachine(MashineNumber[deviceIp]))
                {
                    device2FingersList["device_" + deviceIp].SetDeviceTime(MashineNumber[deviceIp]);
                    device2FingersList["device_" + deviceIp].RefreshData(MashineNumber[deviceIp]);
                    string devip = deviceIp;
                    Thread thread = new Thread(() => GetInsertOldData_TFT(devip, ser_num));
                    thread.Start();
                }
                else
                {
                    string devip = deviceIp;
                    DateTime dt = DateTime.Now;
                    int tt = ((dt.Year - 2000) * 12 * 31 + (dt.Month - 1) * 31 + (dt.Day - 1)) * (24 * 60 * 60) + dt.Hour * 60 * 60 + dt.Minute * 60 + dt.Second;
                    int dwYear = 0;
                    int dwMonth = 0;
                    int dwDay = 0;
                    int dwHour = 0;
                    int dwMinute = 0;
                    int dwSecond = 0;
                    if (device2FingersList["device_" + deviceIp].SetDeviceTime2(MashineNumber[deviceIp], dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second))
                    {
                        if (device2FingersList["device_" + deviceIp].RefreshData(MashineNumber[deviceIp]))
                        {
                            Logger(deviceIp, deviceIp + " Time synced and refreshed");
                            device2FingersList["device_" + deviceIp].GetDeviceTime(1, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond);
                            Logger(deviceIp, "Device time is: "+dwYear.ToString() + "-" + dwMonth.ToString() + "-" + dwDay.ToString() + " " + dwHour.ToString() + ":" + dwMinute.ToString() + ":" + dwSecond.ToString()); ;
                        }
                        else
                        { Logger(deviceIp, deviceIp + " Time synced but not refreshed"); }
                    }
                    else
                    {
                        Logger(deviceIp, deviceIp + "Cannot settime");
                    }

                    Thread thread = new Thread(() =>
                    GetInsertOldData_BAW(devip, ser_num));
                    thread.Start();
                }
                device2FingersList["device_" + deviceIp].SetDeviceTime(1);
                LastFingTime[deviceIp] = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                if (lbConnected.InvokeRequired)
                {
                    lbConnected.Invoke(new MethodInvoker(delegate { lbConnected.Items.Add(deviceIp); }));
                }
            }
            else
            {
                Logger(deviceIp, "ErrorCode: " + ErrorCode + " ( " + deviceIp + " (2)) unable to connect!");
                Logger("connect_error", "ErrorCode: " + ErrorCode + " ( " + deviceIp + " (2)) unable to connect!");
            }

        }

        private bool clearLogData2(string deviceIp)
        {
            {
                if (device2FingersList["device_" + deviceIp].ClearGLog(iMachineNumber))
                {
                    device2FingersList["device_" + deviceIp].RefreshData(iMachineNumber);
                    Logger(deviceIp, deviceIp + ": Cleared old data!");
                }

            }
            return true;

        }

        private bool GetInsertOldData_TFT(string deviceIp, string ser_num)
        {
            string serialnumbertft = ser_num;

            Logger(deviceIp, deviceIp + ":start insert old data TFT!");
            try
            {
                bool clear = false;
                if (device2FingersList["device_" + deviceIp].ReadGeneralLogData(1))//read all the attendance records to the memory ReadNewGLogData
                {
                    string sdwEnrollNumber = "";
                    int idwVerifyMode = 0;
                    int idwInOutMode = 0;
                    int idwYear = 0;
                    int idwMonth = 0;
                    int idwDay = 0;
                    int idwHour = 0;
                    int idwMinute = 0;
                    int idwSecond = 0;
                    int idwWorkcode = 0;
                    int logcnt = 0;
                    NameValueCollection data = new NameValueCollection();
                    data.Clear();
                    string datastr = "";
                    int idwEnrollNumber;
                    while (device2FingersList["device_" + deviceIp].SSR_GetGeneralLogData(1, out sdwEnrollNumber, out idwVerifyMode,
                               out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                    {

                        string dt = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();
                        if (int.TryParse(sdwEnrollNumber, out idwEnrollNumber))
                        {

                            data.Add("finger_id", idwEnrollNumber.ToString());
                            data.Add("serial_number", ser_num);
                            data.Add("date", dt);
                            datastr += "[ ID: " + sdwEnrollNumber + " DT: " + dt + " ]";
                            Logger(deviceIp, deviceIp + " ID: " + sdwEnrollNumber + " DT: " + dt);
                            logcnt++;
                        }

                    }
                    // Logger("SN: "+ ser_num+" : "+ datastr);
                    Logger(deviceIp, deviceIp + ":" + ser_num + ": " + logcnt.ToString() + " find logs!");
                    if (!InoutSaveAllToAPI(data))
                    {
                        Logger(deviceIp, deviceIp + ":" + ser_num + ":" + sdwEnrollNumber + " insert fail!");
                        clear = false;
                    }
                    else
                    {
                        Logger(deviceIp, deviceIp + ":" + ser_num + " insert ok!");
                        clear = true;
                    }




                    if (clear)
                    {

                        string nowTime = DateTime.Now.ToString("HH:mm:ss");
                        //DateTime.Parse(nowTime) > DateTime.Parse(startTime) && DateTime.Parse(nowTime) < DateTime.Parse(endTime)
                        if (DateTime.Parse(nowTime) > DateTime.Parse(this.startTime) && DateTime.Parse(nowTime) < DateTime.Parse(this.endTime))
                        {
                            Logger(deviceIp, deviceIp + " time to clear!");
                            device2FingersList["device_" + deviceIp].DisableDeviceWithTimeOut(1, 10);
                            if (device2FingersList["device_" + deviceIp].ClearGLog(iMachineNumber))
                            {
                                device2FingersList["device_" + deviceIp].ClearSLog(iMachineNumber);
                                device2FingersList["device_" + deviceIp].RefreshData(iMachineNumber);

                                device2FingersList["device_" + deviceIp].EnableDevice(1, true);

                                Logger(deviceIp, deviceIp + ": Cleared old data!");
                            }
                        }
                        else
                        {
                            Logger(deviceIp, deviceIp + " not time to clear!");
                            device2FingersList["device_" + deviceIp].EnableDevice(1, true);
                        }



                    }
                    else
                    {
                        Logger(deviceIp, deviceIp + " error during put InOut api!");
                        return false;
                    }

                }
                else
                {
                    Logger(deviceIp, deviceIp + ":none old data TFT!");
                    // device2FingersList["device_" + deviceIp].CancelOperation();
                    device2FingersList["device_" + deviceIp].EnableDevice(1, true);
                    device2FingersList["device_" + deviceIp].Disconnect();
                    return false;
                }
                device2FingersList["device_" + deviceIp].EnableDevice(1, true);
                Logger(deviceIp, "log read finish " + deviceIp);
                return true;
            }
            catch (Exception e)
            {
                Logger(deviceIp, e.ToString() + " | ( " + deviceIp + " (2)) error during read old data TFT!");
                device2FingersList["device_" + deviceIp].EnableDevice(1, true);
                return false;
            }


        }

        public static DateTime UnixTimeStampToDateTime(double seconds)
        {
            //System.DateTime dtDateTime = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            //dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);//.ToLocalTime();
            double iYear = 0;
            double Differ = 0;
            double iMonth = 0;
            double iDay = 0;
            double iHour = 0;
            double iMinute = 0;
            double iSecond = 0;

            Differ = seconds + (32 * 24 * 60 * 60);
            iYear = (int)Differ / (3600 * 24 * 31 * 12);
            Differ = Differ % (3600 * 24 * 31 * 12);
            iMonth = (int)Differ / (3600 * 24 * 31);
            Differ = Differ % (3600 * 24 * 31);
            iDay = (int)Differ / (3600 * 24);
            Differ = Differ % (3600 * 24);
            iHour = (int)Differ / 3600;
            Differ = Differ % 3600;
            iMinute = (int)Differ / 60;
            Differ = Differ % 60;
            iSecond = (int)Differ;
            iYear += 2000;
            string dt = iYear.ToString() + "-" + iMonth.ToString() + "-" + iDay.ToString() + " " + iHour.ToString() + ":" + iMinute.ToString() + ":" + iSecond.ToString();
            return Convert.ToDateTime(dt);
        }

        private bool GetInsertOldData_BAW(string deviceIp, string ser_num)
        {
            string serialnumberbaw = ser_num;
            Logger(deviceIp, deviceIp + ":start insert old data B&W device!");
            string str = "Cardno\tPin\tVerified\tDoorID\tEventType\tInOutState\tTime_second";
            try
            {
                bool clear = false;
                int BUFFERSIZE = 10 * 1024 * 1024;
                string buffer = "";
                int logcnt = 0;
                if (device2FingersList["device_" + deviceIp].SSR_GetDeviceData(1, out buffer, BUFFERSIZE, "transaction", str, "", ""))//read all the attendance records to the memory
                {
                    char[] delimiterChars = { ',' };
                    //Logger(buffer);
                    string[] logs = buffer.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    int i = 1;
                    NameValueCollection data = new NameValueCollection();
                    data.Clear();
                    foreach (var log in logs)
                    {
                        if (i == 1)
                        { i += 1; }
                        else
                        {
                            if ((log != "") || (log.IndexOf("CardNo") != -1))
                            {
                                string[] userlog = log.Split(delimiterChars);
                                {
                                    // if (userlog[1].Substring(0) != "0")
                                    {

                                        DateTime ddt;
                                        ddt = UnixTimeStampToDateTime(Convert.ToDouble(userlog[6].Substring(0)));
                                        string dt = ddt.ToString("yyyy-MM-dd HH:mm:ss");
                                        //Logger(userlog[1].Substring(0) + ":" + dt);
                                        data.Add("finger_id", userlog[1].Substring(0));
                                        data.Add("serial_number", ser_num);
                                        data.Add("date", dt);
                                        logcnt++;
                                    }



                                }
                                i += 1;

                            }
                        }
                    }
                    Logger(deviceIp, deviceIp + ": " + logcnt.ToString() + " find logs!");
                    if (!InoutSaveAllToAPI(data))
                    {
                        Logger(deviceIp, deviceIp + " insert fail!");
                        clear = false;
                    }
                    else
                    {
                        Logger(deviceIp, deviceIp + " insert ok!");
                        clear = true;
                    }

                    string nowTime = DateTime.Now.ToString("HH:mm:ss");

                    if (DateTime.Parse(nowTime) > DateTime.Parse(this.startTime) && DateTime.Parse(nowTime) < DateTime.Parse(this.endTime))
                    {
                        if (clear)
                        {
                            device2FingersList["device_" + deviceIp].ClearGLog(1);
                            device2FingersList["device_" + deviceIp].ClearSLog(1);
                            //SSR_DeleteDeviceData(1, "transaction", "Pin=" + userlog[1].Substring(0) + "\tTime_second" + userlog[2].Substring(0), "");
                            Logger(deviceIp, deviceIp + " : clearlog");
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                else
                {
                    Logger(deviceIp, deviceIp + ":none old data BAW!");
                    return false;
                }
                device2FingersList["device_" + deviceIp].EnableDevice(1, true);
                // device2FingersList["device_" + deviceIp].Disconnect();
                // device2FingersList.Remove("device_" + deviceIp);
                // device2type.Remove(deviceIp);
                //  LastFingTime.Remove(deviceIp);
                //  DevConnected.Remove(deviceIp);
                return true;
            }
            catch (Exception e)
            {
                Logger(deviceIp, e.ToString() + " | ( " + deviceIp + " (2)) error during read old data BAW!");
                device2FingersList["device_" + deviceIp].EnableDevice(1, true);
                //device2FingersList["device_" + deviceIp].Disconnect();
                //device2FingersList.Remove("device_" + deviceIp);
                //device2type.Remove(deviceIp);
                //LastFingTime.Remove(deviceIp);
                //DevConnected.Remove(deviceIp);
                return false;
            }
        }

        private bool device2_log(string deviceIp, string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode, string deviceType, string serial_num)
        {
            string str = ".\n" + sEnrollNumber + "," + iMonth.ToString() + "/" + iDay.ToString() + "/" + iYear.ToString() + " " + iHour.ToString() + ":" + iMinute.ToString() + ":" + iSecond.ToString();
            string dt = iYear.ToString() + "-" + iMonth.ToString() + "-" + iDay.ToString() + " " + iHour.ToString() + ":" + iMinute.ToString() + ":" + iSecond.ToString();
            int intsEnrollNumber = 0;
            string devece_serial = serial_num;
            if (!Int32.TryParse(sEnrollNumber, out intsEnrollNumber) || intsEnrollNumber <= 0)
            {
                Logger(deviceIp, sEnrollNumber + " - (" + deviceIp + " (2 " + deviceType + ")-)");
                return false;
            }
            else Logger(deviceIp, sEnrollNumber + " - (" + deviceIp + " (2 " + deviceType + " State: " + iAttState + "))");

            InoutSaveToAPI(sEnrollNumber, devece_serial, dt);
            Logger(deviceIp, sEnrollNumber + " : " + serial_num + " : " + dt + " : " + deviceIp + ": verify mode: " + iVerifyMethod.ToString() + ": att state: " + iAttState.ToString());
            return true;
        }
        private bool InoutSaveToAPI(string sEnrollNumber, string serialnumber, string date)
        {
            string json = "";
            string sn = serialnumber;
            //GetProPassToken("dservice", "123456", "0");
            if (this.sToken != "")
            {
                using (WebClient webClient = new WebClient())
                {
                    NameValueCollection data = new NameValueCollection();
                    data.Add("finger_id", sEnrollNumber);
                    data.Add("serial_number", sn);
                    data.Add("date", date);

                    webClient.Headers.Set("Authorization", "Bearer " + this.sToken);

                    try
                    {
                        json = Encoding.UTF8.GetString(webClient.UploadValues(this.tam_unvan + "/api/fingerprint/in-out-log/save", "POST", data));
                        //Logger(this.tam_unvan + "/api/fingerprint/in-out-log/save?" + "finger_id=" + sEnrollNumber + "&serial_number=" + sn);
                        if (json.Contains("Log successfully was saved") || (json.Contains("Log is already saved")) || (json.Contains("Device not found with given serial number")))
                        {
                            //if (json.Contains("Log successfully was saved"))
                            //{ Logger("Log successfully was saved: " + sEnrollNumber); }
                            //if (json.Contains("Log is already saved"))
                            //{ Logger("Log is already saved: " + sEnrollNumber); }
                            //if (json.Contains("Device not found with given serial number"))
                            //{ Logger("Device not found with given serial number: " + sEnrollNumber); }
                            return true;
                        }
                        else
                        {
                            var writereply = JsonConvert.DeserializeObject<InOutSave>(json);
                            if (writereply.status.ToString() == "ok")
                            {
                                return true;
                            }
                            else
                            {
                                Logger("Server", "writestatus log error");
                                return false;
                            }
                        }
                    }
                    catch (Exception e)
                    {

                        Logger("Server", e.Message + " InoutSaveToAPI " + this.tam_unvan + "/api/fingerprint/in-out-log/save?" + "finger_id=" + sEnrollNumber + "&serial_number=" + sn);
                        return false;
                    }

                }
            }
            else
            {
                Logger("Server","InoutSaveToAPI notoken");
                return false;
            }
        }

        private bool InoutSaveAllToAPI(NameValueCollection data)
        {
            string json = "";

            if (this.sToken != "")
            {
                using (WebClient webClient = new WebClient())
                {

                    webClient.Headers.Set("Authorization", "Bearer " + this.sToken);

                    try
                    {

                        json = Encoding.UTF8.GetString(webClient.UploadValues(this.tam_unvan + "/api/fingerprint/in-out-log-all/save", "POST", data));
                        //Logger(json);
                        var writereply = JsonConvert.DeserializeObject<InOutAllError>(json);
                        if (writereply.message.ToString() != "Device not found with given serial number or User already deleted")
                        {
                            Logger("Server","writestatus log OK");
                            Logger("Server", writereply.message.ToString());
                            return true;
                        }
                        else
                        {
                            Logger("Server","writestatus log ERROR: "+ writereply.message.ToString());
                            
                            return false;
                        }

                    }
                    catch (Exception e)
                    {

                        Logger("Server",e.Message + " InoutSaveAllToAPI " + this.tam_unvan + "/api/fingerprint/in-out-log-all/save");
                        return false;
                    }

                }
            }
            else
            {
                Logger("Server", "InoutSaveAllToAPI notoken");
                return false;
            }
        }

        public void GetProPassToken(string username, string password, string eu)
        {

            string json = "";

            using (WebClient webClient = new WebClient())
            {
                NameValueCollection data = new NameValueCollection();
                data.Add("username", username);
                data.Add("password", password);
                data.Add("eu", eu);
                try
                {
                    json = Encoding.UTF8.GetString(webClient.UploadValues(this.tam_unvan + "/pps/oauth/login", "POST", data));
                    //Logger(this.tam_unvan + "/pps/oauth/login?" + "username=" + username + "&password=" + password);

                    try
                    {
                        var gettoken = JsonConvert.DeserializeObject<GetTokenfromPropass>(json);
                        this.sToken = gettoken.user.auth_token;
                    }
                    catch (Exception e)
                    {
                        Logger("Server","JsonConvert.Deserialize:" + e.Message);
                        this.sToken = "";

                    }
                }

                catch (Exception e)
                {
                    this.sToken = "";
                    Logger("Server","GetToken: " + e.Message);
                }




            }

        }

        public void All_Devices()
        {
            Logger("Server","All devices list");
            string json = "";
            GetProPassToken("dservice", "123456", "0");
            if (this.sToken != "")
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Set("Authorization", "Bearer " + this.sToken);
                    try
                    {
                        json = webClient.DownloadString(this.tam_unvan + "/api/ex/fp-server/device/all");
                        // Logger(this.tam_unvan + "/api/ex/fp-server/device/all");

                        var devicereply = JsonConvert.DeserializeObject<DeviceObject>(json);
                        if (devicereply.status)
                        {
                            lbRTShow.Items.Clear();
                            lbConnected.Items.Clear();
                            //Logger("Server","get " + devicereply.data.Length + " devices from api");
                            //groupBox3.Text = "Device list: " + devicereply.data.Length.ToString() + " devices found";
                            //from api
                            /*
                            for (int i = 1; i <= devicereply.data.Length; i++)
                            {
                                //Logger("i: " + i.ToString() + " ip: " + devicereply.data[i - 1].ip_address);
                                lbRTShow.Items.Add(devicereply.data[i - 1].ip_address);
                                int k = i;
                                if (devicereply.data[i - 1].ip_address.Contains(this.ips))
                                {
                                    try
                                    {
                                        Thread devthread = new Thread(() => ConnectDevice1(devicereply.data[k - 1].ip_address));
                                        devthread.Name = "thr" + devicereply.data[k - 1].ip_address;
                                        //Logger("thr__" + devthread.Name);
                                        ThrArr.Add(devthread);
                                        devthread.Start();
                                    }
                                    catch (Exception e)
                                    {
                                        Logger("Server","error: " + e.Message + " :" + devicereply.data[k - 1].ip_address);
                                    }
                                }

                            }
                            */
                            //from array
                            //Logger("Server", "get " + d_ip + " devices from array");
                            //groupBox3.Text = "Device list: " + d_ip.Length.ToString();
                                //and "+ ADMS_IP.Length.ToString() + " ADMS devices found";
                          //  for (int i = 0; i < 1; i++)
                            {

                                lbRTShow.Items.Add(d_ip);
                                 {
                                    try
                                    {
                                        Thread devthread = new Thread(() => ConnectDevice1(d_ip));
                                        devthread.Name = "thr" + d_ip;
                                        //Logger("Server", "thr__" + devthread.Name);
                                        ThrArr.Add(devthread);
                                        devthread.Start();
                                    }
                                    catch (Exception e)
                                    {
                                        Logger("Service", "error: " + e.Message + " :" + d_ip);
                                    }
                                }

                            }
                            

                            // groupBox3.Text = "Device list: " + (F18_IP1.Length + F18_IP2.Length).ToString() + " F18 and " + ADMS_IP.Length.ToString() + " ADMS devices found";
                            /*   for (int k = 0; k < ADMS_IP.Length; k++)
                               {

                                   lbRTShow.Items.Add(ADMS_IP[k]);
                                    {
                                       try
                                       {
                                           Thread devthread = new Thread(() => ConnectDevice1(ADMS_IP[k]));
                                           devthread.Name = "thr" + ADMS_IP[k];
                                           //Logger("Server", "thr__" + devthread.Name);
                                           ThrArr.Add(devthread);
                                           devthread.Start();
                                       }
                                       catch (Exception e)
                                       {
                                           Logger("Service", "error: " + e.Message + " :" + ADMS_IP[k]);
                                       }
                                   }

                               }
                            */
                        }
                        else
                        {
                        }
                    }
                    catch (Exception e)
                    {

                        Logger("Server","error: " + e.Message);

                    }
                }
            }
            //Thread.Sleep(15000);
            //Environment.Exit(0);
        }


        public bool FileWrite(string path, string content)
        {
            try
            {
                File.AppendAllText(path, content);
                return true;
            }
            catch (Win32Exception)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }
        private string tarix()
        {
            return DateTime.Now.ToString();
        }

        private void DeviceList_Tick(object sender, EventArgs e)
        {


        }

        private void timerRestart_Tick(object sender, EventArgs e)
        {
            Logger("Server",this.closems.ToString() + "ms interval shutdown");
            // Application.Restart();
            // Application.Exit();
            Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //int idwErrorCode=0;

            //    if (device2FingersList["device_" + "10.234.20.219"].ClearData(1, 5))
            //    {
            //        device2FingersList["device_" + "10.234.20.219"].RefreshData(1);//the data in the device should be refreshed

            //    }
            //    else
            //    {
            //        device2FingersList["device_" + "10.234.20.219"].GetLastError(ref idwErrorCode);

            //    }
            // ConnectDevice1("10.234.20.211");
            All_Devices();

        }
    }
}
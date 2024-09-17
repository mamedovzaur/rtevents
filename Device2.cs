using System.Windows.Forms;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.ComponentModel;

namespace RTEvents
{
    class Device2 : ApplicationContext
    {
        System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        string deviceIp;
        public bool discon = false;
        public int ErrorCode = 0;

        public Device2(string deviceIp)
        {
            this.deviceIp = deviceIp;

            ConnectDevice2(deviceIp);
        }

        #region DEVICE2

        public void Logger(string logtxt)
        {


            string FilePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\log";
            System.IO.Directory.CreateDirectory(FilePath);
            string log_fayl2 = FilePath + "\\" + DateTime.Now.ToString("MM.dd.yyyy") + ".log";
            FileWrite(log_fayl2, DateTime.Now.ToString() + " | " + logtxt + " \r\n");


        }


        private void ConnectDevice2(string deviceIp)
        {
            if (!PingHost(deviceIp))
            {
                ErrorCode = -111;
                Logger(" | ErrorCode: " + ErrorCode + " ( " + deviceIp + " (2)) cannot ping device!");

                RTEventsMain.device2FingersList.Remove("device_" + deviceIp);
                RTEventsMain.device2type.Remove(deviceIp);
                RTEventsMain.LastFingTime.Remove(deviceIp);
                discon = true;

                return;
            }

            string dvTYPE = "";
            string isdevice = "";
            try { isdevice = RTEventsMain.device2FingersList["device_" + deviceIp].ToString(); }
            catch { isdevice = "none"; }
            bool state = false;
            if (isdevice == "none")
            {
                RTEventsMain.device2FingersList["device_" + deviceIp] = new zkemkeeper.CZKEMClass();
                state = false;
            }


            state = RTEventsMain.device2FingersList["device_" + deviceIp].Connect_Net(deviceIp, 4370);



            if (state == true)
            {
                Logger(state.ToString() + " | ( " + deviceIp + " (2)) connected to device!");

                //
                RTEventsMain.device2FingersList["device_" + deviceIp].EnableDevice(1, false);//disable the device
                string platform = "";
                RTEventsMain.device2FingersList["device_" + deviceIp].GetPlatform(1, ref platform);
                if (platform.IndexOf("_TFT") != -1)
                {
                    GetInsertOldData_TFT(deviceIp);
                    dvTYPE = "TFT";
                }
                else
                {
                    GetInsertOldData_BAW(deviceIp);
                    dvTYPE = "BAW";
                }
                clearLogData2(deviceIp);
                RTEventsMain.device2FingersList["device_" + deviceIp].SetDeviceTime(1);
                RTEventsMain.LastFingTime[deviceIp] = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                //

                if (RTEventsMain.device2FingersList["device_" + deviceIp].RegEvent(1, 65535))
                {
                    RTEventsMain.device2FingersList["device_" + deviceIp].OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(delegate (string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
                    {
                        RTEventsMain.LastFingTime[deviceIp] = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                        devece2_log(deviceIp, sEnrollNumber, iIsInValid, iAttState, iVerifyMethod, iYear, iMonth, iDay, iHour, iMinute, iSecond, iWorkCode, "TRIGGER");
                    });
                    //Form1.device2FingersList["device_" + deviceIp].OnDisConnected += new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(delegate () { devece2_disconnected(deviceIp); });
                    Logger(deviceIp + " (2)) waiting logs from device!!! \r\n");

                    RTEventsMain.device2FingersList["device_" + deviceIp].EnableDevice(1, true);//disable the device
                    RTEventsMain.device2type[deviceIp] = dvTYPE;
                }
                else
                {
                    RTEventsMain.device2FingersList["device_" + deviceIp].GetLastError(ref ErrorCode);
                    RTEventsMain.device2FingersList["device_" + deviceIp].EnableDevice(1, true);//disable the device

                    RTEventsMain.device2FingersList["device_" + deviceIp].Disconnect();
                    RTEventsMain.device2FingersList.Remove("device_" + deviceIp);
                    RTEventsMain.device2type.Remove(deviceIp);
                    RTEventsMain.LastFingTime.Remove(deviceIp);
                    discon = true;

                    Logger(" | ErrorCode: " + ErrorCode + " ( " + deviceIp + " (2)) unable to read logs from device!");
                }
            }
            else
            {
                RTEventsMain.device2FingersList["device_" + deviceIp].GetLastError(ref ErrorCode);
                RTEventsMain.device2FingersList.Remove("device_" + deviceIp);
                RTEventsMain.device2type.Remove(deviceIp);
                RTEventsMain.LastFingTime.Remove(deviceIp);
                discon = true;

                Logger(state.ToString() + " | ErrorCode: " + ErrorCode + " ( " + deviceIp + " (2)) cannot connect to device!");
            }
        }

        private bool GetInsertOldData_TFT(string deviceIp)
        {
            try
            {
                if (RTEventsMain.device2FingersList["device_" + deviceIp].ReadGeneralLogData(1))//read all the attendance records to the memory
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
                    while (RTEventsMain.device2FingersList["device_" + deviceIp].SSR_GetGeneralLogData(1, out sdwEnrollNumber, out idwVerifyMode,
                               out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                    {
                        devece2_log(deviceIp, sdwEnrollNumber, 0, idwInOutMode, idwVerifyMode, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond, idwWorkcode, "TFT");
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Logger(e.ToString() + " | ( " + deviceIp + " (2)) get error during reading data!");
                return false;
            }
        }

        private bool GetInsertOldData_BAW(string deviceIp)
        {
            try
            {
                int idwEnrollNumber = 0;
                int idwVerifyMode = 0;
                int idwInOutMode = 0;

                int idwYear = 0;
                int idwMonth = 0;
                int idwDay = 0;
                int idwHour = 0;
                int idwMinute = 0;
                int idwSecond = 0;
                int idwWorkCode = 0;
                int idwReserved = 0;

                if (RTEventsMain.device2FingersList["device_" + deviceIp].ReadGeneralLogData(1))//read all the attendance records to the memory
                {
                    while (RTEventsMain.device2FingersList["device_" + deviceIp].GetGeneralExtLogData(1, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
                             ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode, ref idwReserved))//get records from the memory
                    {
                        devece2_log(deviceIp, idwEnrollNumber.ToString(), 0, idwInOutMode, idwVerifyMode, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond, idwWorkCode, "BAW");
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Logger(e.ToString() + " | ( " + deviceIp + " (2)) error during reading data frm device!");
                return false;
            }
        }

        private bool devece2_log(string deviceIp, string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode, string deviceType)
        {
            string str = ".\n" + sEnrollNumber + "," + iMonth.ToString() + "/" + iDay.ToString() + "/" + iYear.ToString() + " " + iHour.ToString() + ":" + iMinute.ToString() + ":" + iSecond.ToString();

            int intsEnrollNumber = 0;
            if (!Int32.TryParse(sEnrollNumber, out intsEnrollNumber) || intsEnrollNumber <= 0)
            {
                Logger(sEnrollNumber + " - (" + deviceIp + " (2 " + deviceType + ")-)");
                return false;
            }
            else Logger(sEnrollNumber + " - (" + deviceIp + " (2 " + deviceType + " State: " + iAttState + "))");

            Logger(deviceIp + ":" + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str)) + ":2:" + iAttState);

            return true;
        }

        private bool clearLogData2(string deviceIp)
        {

            return true;
        }

        #endregion

        #region FileWrite
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

        public bool FileWrite2(string path, string content)
        {
            try
            {
                File.WriteAllText(path, content);
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
        #endregion

        #region SomeFunc
        private bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(nameOrAddress, 2000);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }

            pinger.Dispose();
            return pingable;
        }

        private string tarix()
        {
            return DateTime.Now.ToString();
        }
        #endregion
    }
}

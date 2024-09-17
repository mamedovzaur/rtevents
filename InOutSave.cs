namespace RTEvents
{
    public class InOutSave
    {
        public bool status { get; set; }
        public InOutSaveMessage message { get; set; }

    }

    public class InOutSaveMessage
    {
        public string[] finger_id { get; set; }
        public string[] serial_number { get; set; }
        public string[] date { get; set; }


    }





    public class DeviceObject
    {
        public bool status { get; set; }
        public DeviceDatum[] data { get; set; }
        public object message { get; set; }
    }

    public class DeviceDatum
    {
        public int id { get; set; }
        public string device_name { get; set; }
        public string device_manufacture_id { get; set; }
        public string device_model_id { get; set; }
        public string device_destination_id { get; set; }
        public string ip_address { get; set; }
        public string serial_number { get; set; }
        public string google_map { get; set; }
        public string note { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public object deleted_at { get; set; }
        public string device_finger_code_id { get; set; }
        public Device_Types[] device_types { get; set; }
    }

    public class Device_Types
    {
        public int id { get; set; }
        public string name { get; set; }
    }



    public class GetTokenfromPropass
    {
        public bool status { get; set; }
        public GetTokenData user { get; set; }
    }

    public class GetTokenData
    {
        public int id { get; set; }
        public object userId { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public object email { get; set; }
        public string username { get; set; }
        public object remember_token { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string is_system_user { get; set; }
        public string auth_token { get; set; }
    }



    public class SyncDevice
    {
        public bool status { get; set; }
        public SyncData data { get; set; }
    }

    public class SyncData
    {
        public Deleted_Fingers[] deleted_fingers { get; set; }
        public Added_fingers[] added_fingers { get; set; }
    }

    public class Deleted_Fingers
    {
        public string userid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string cardnumber { get; set; }
    }

    public class Added_fingers
    {
        public string finger_id { get; set; }
        public string finger_index { get; set; }
        public string fing_template { get; set; }
        public string card_id { get; set; }
        public string user_id { get; set; }
        public string username { get; set; }
        public string structure_id { get; set; }
        public string card_number { get; set; }
        public string card_name { get; set; }
    }

    public class ConnectKey
    {
        public bool status { get; set; }
        public object auth_key { get; set; }
    }



    public class InOutAll
    {
        public bool status { get; set; }
        public string message { get; set; }
        public InOutAllError[] errors { get; set; }
    }

    public class InOutAllError
    {
        public InOutAllData data { get; set; }
        public string message { get; set; }
    }

    public class InOutAllData
    {
        public string finger_id { get; set; }
        public string serial_number { get; set; }
        public string date { get; set; }
    }

}

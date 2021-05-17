using System;
using System.Collections.Generic;
using System.Text;

namespace NetMQMedia.Models
{
    public class ResponseModel
    {
        public string IpAddress { get; set; }
        public RequestType RequestType { get; set; }
        public int DataType { get; set; }

        public List<DataType> GetDataType()
        {

        }
    }
    public enum RequestType
    {
        Call=0,
        Message=1,
        JoinRoom=2,
        RoomCall=3
    }
    public enum DataType
    {
        Empty=0,
        Message=1,
        Audio=2,
        Video=4,
    }
}

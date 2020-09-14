using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocketTest
{
    [DataContract]
    class MessagePacket : JsonBase
    {
        public static MessagePacket Parse(string buf)
        {
            return Deseialize<MessagePacket>(buf);
        }

        public override string ToString()
        {
            return Serialize(this);
        }

        [DataMember]
        public string NickName { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}

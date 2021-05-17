using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMQMedia.Models
{
    public class Audio:Media
    {
        public int DevNumber { get; set; }
        public int Channels { get; set; }
    }
}

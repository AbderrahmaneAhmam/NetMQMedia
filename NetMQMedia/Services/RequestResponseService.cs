using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace NetMQMedia.Services
{
    public class RequestResponseService
    {
        private readonly int _portCall;
        private readonly int _portRoom;
        private RouterSocket router;
        private DealerSocket dealer;
        private NetMQPoller poller;

        public RequestResponseService(int portCall,int portRoom)
        {
            _portCall = portCall;
            _portRoom = portRoom;
            router = new RouterSocket();
            poller = new NetMQPoller { router };
            poller.RunAsync();
            router.ReceiveReady += Router_ReceiveReady;
        }

        private void Router_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            
        }

        private void StartConnection()
        {
            router.Bind($"tcp://*:{_portCall}");
        }
    }
}

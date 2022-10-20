using Java.Net;
using Java.Util;
using System;
using System.Collections.Generic;

namespace NET
{
    public class Network
    {
        public List<string> GetIP()
        {
            List<string> ip = new List<string>();
            IEnumeration ie = NetworkInterface.NetworkInterfaces;
            while (ie.HasMoreElements)
            {
                NetworkInterface intf = ie.NextElement() as NetworkInterface;
                IEnumeration enumIPAddr = intf.InetAddresses;
                while (enumIPAddr.HasMoreElements)
                {
                    InetAddress inetAddress = enumIPAddr.NextElement() as InetAddress;
                    //return inetAddress.ToString();
                    ip.Add(inetAddress.ToString());
                }
            }
            return ip;
        }
    }
}

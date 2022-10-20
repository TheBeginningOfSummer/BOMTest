using BOMTest.Droid;
using Java.Net;
using Java.Util;
using System;
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidNetTool))]
namespace BOMTest.Droid
{
    public class AndroidNetTool : IAndroidNetTool
    {
        public List<string> GetIP()
        {
            string ipString = "";
            List<string> ipList = new List<string>();
            IEnumeration ie = NetworkInterface.NetworkInterfaces;
            while (ie.HasMoreElements)
            {
                NetworkInterface intf = ie.NextElement() as NetworkInterface;
                IEnumeration enumIPAddr = intf.InetAddresses;
                while (enumIPAddr.HasMoreElements)
                {
                    InetAddress inetAddress = enumIPAddr.NextElement() as InetAddress;
                    if (!inetAddress.IsLoopbackAddress)
                        ipString += inetAddress.ToString();
                }
            }
            foreach (var ip in ipString.Split('/'))
                if (ip != "") ipList.Add(ip);
            return ipList;
        }
    }
}

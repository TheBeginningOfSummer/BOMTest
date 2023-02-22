using System.Net.Sockets;
using static MyToolkit.ConnectionToolkit;
using static MyToolkit.DataConverter;
using static MyToolkit.ByteArrayToolkit;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace CommunicationsToolkit
{
    public class FinsTCPServer
    {
        public SocketConnection Connection { get; set; }
        public int RemoteNode { get; set; }
        public int ServerNode { get; set; }
        public byte[] WArea;
        public byte[] HArea;
        public byte[] DArea;
        private readonly ConcurrentQueue<ClientData> cache = new ConcurrentQueue<ClientData>();

        public FinsTCPServer(int serverNode = 0, int wAreaLength = 100, int hAreaLength = 2048, int dAreaLength = 2048)
        {
            Connection = new SocketConnection();
            ServerNode = serverNode;
            WArea = new byte[wAreaLength * 2];
            HArea = new byte[hAreaLength * 2];
            DArea = new byte[dAreaLength * 2];
            Connection.ReceiveFromClient += MessageHandling;
            Task.Run(MessageRespond);
            InitializeMemoryArea();
        }

        private void InitializeMemoryArea()
        {
            byte[] bom = WordByteReverse(Encoding.ASCII.GetBytes("01151:2;"));
            DArea[1] = 0x01; DArea[3] = 0x04; DArea[5] = 0x02;
            bom.CopyTo(DArea, 6);
            HArea[0] = 0xFF; HArea[1] = 0xFF;
        }

        public void MessageHandling(Socket client, byte[] data)
        {
            cache.Enqueue(new ClientData(client, data));
        }

        private void MessageRespond()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(200);
                    while (!cache.IsEmpty)
                    {
                        if (!cache.TryDequeue(out ClientData clientData)) continue;
                        if (clientData.Data != null)
                        {
                            byte[] response = ParseMessage(clientData.Data);
                            if (response != null) clientData.Client.Send(response);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        public byte[] ParseMessage(byte[] data)
        {
            if (data.Length == 20)
                return HandshakeResponse(data);
            else if (data[26] == 0x01 && data[27] == 0x01)
                return ReadResponse(data);
            else if (data[26] == 0x01 && data[27] == 0x02)
                return WriteResponse(data);
            return default;
        }

        public byte[] HandshakeResponse(byte[] data)
        {
            byte[] response = new byte[24];
            response[0] = 0x46;
            response[1] = 0x49;
            response[2] = 0x4E;
            response[3] = 0x53;
            response[7] = 0x10;
            response[11] = 0x01;
            response[19] = data[19];
            RemoteNode = data[19];
            response[23] = (byte)ServerNode;
            return response;
        }

        public byte[] ReadResponse(byte[] data)
        {
            if (data.Length < 34) return null;
            byte[] errorCode = new byte[2] { 0x00, 0x00 };
            if (data[26] == 0x01 && data[27] == 0x01)
            {
                byte[] response = data.Take(28).ToArray();
                response[16] = 0xC0;
                int address = TwoBytesToUInt(data.Skip(29).Take(2).ToArray());
                int arrayLength = TwoBytesToUInt(data.Skip(32).Take(2).ToArray()) * 2;
                switch (data[28])
                {
                    case 0xB1:
                        byte[] wAreaData = WArea.Skip(address * 2).Take(arrayLength).ToArray();
                        byte[] returnWAreaData = SpliceBytes(errorCode, wAreaData);
                        return SpliceBytes(response, returnWAreaData);
                    case 0xB2:
                        byte[] hAreaData = HArea.Skip(address * 2).Take(arrayLength).ToArray();
                        byte[] returnHAreaData = SpliceBytes(errorCode, hAreaData);
                        return SpliceBytes(response, returnHAreaData); ;
                    case 0x82:
                        byte[] dAreaData = DArea.Skip(address * 2).Take(arrayLength).ToArray();
                        byte[] returnDAreaData = SpliceBytes(errorCode, dAreaData);
                        return SpliceBytes(response, returnDAreaData); ;
                }
            }
            return null;
        }

        public byte[] WriteResponse(byte[] data)
        {
            if (data.Length < 34) return null;
            byte[] errorCode = new byte[2] { 0x00, 0x00 };
            if (data[26] == 0x01 && data[27] == 0x02)
            {
                byte[] response = data.Take(28).ToArray();
                response[16] = 0xC0;
                int address = TwoBytesToUInt(data.Skip(29).Take(2).ToArray());
                int arrayLength = TwoBytesToUInt(data.Skip(32).Take(2).ToArray()) * 2;
                byte[] receivedData = data.Skip(34).ToArray();
                switch (data[28])
                {
                    case 0xB1:
                        receivedData.CopyTo(WArea, address * 2);
                        return SpliceBytes(response, errorCode);
                    case 0xB2:
                        receivedData.CopyTo(HArea, address * 2);
                        return SpliceBytes(response, errorCode); ;
                    case 0x82:
                        receivedData.CopyTo(DArea, address * 2);
                        return SpliceBytes(response, errorCode); ;
                }
            }
            return null;
        }

        public static byte[] WordByteReverse(byte[] bytes)
        {
            if (bytes == null) return Encoding.ASCII.GetBytes("null:0");
            if (bytes.Length == 0) return Encoding.ASCII.GetBytes("null:0");
            List<byte> list = new List<byte>();
            if ((bytes.Length % 2) == 0)
            {
                for (int i = 0; i < bytes.Length; i += 2)
                {
                    list.Add(bytes[i + 1]);
                    list.Add(bytes[i]);
                }
                return list.ToArray();
            }
            else
            {
                for (int i = 0; i < bytes.Length - 1; i += 2)
                {
                    list.Add(bytes[i + 1]);
                    list.Add(bytes[i]);
                }
                list.Add(bytes.Last());
                return list.ToArray();
            }
        }

        public static byte[] ReadData(byte[] area, int address, int length)
        {
            return area.Skip(address * 2).Take(length * 2).ToArray();
        }

    }

    public class ClientData
    {
        public Socket Client { get; set; }
        public byte[] Data { get; set; }

        public ClientData(Socket client, byte[] data)
        {
            Client = client;
            Data = data;
        }
    }
}

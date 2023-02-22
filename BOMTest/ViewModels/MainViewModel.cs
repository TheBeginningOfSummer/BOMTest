using BOMTest.Models;
using CommunicationsToolkit;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using MyToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Xamarin.Forms;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;


namespace BOMTest.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        #region 绑定的变量
        private string serverIP;
        public string ServerIP
        {
            get => serverIP;
            set => SetProperty(ref serverIP, value);
        }

        private string serverPort;
        public string ServerPort
        {
            get => serverPort;
            set => SetProperty(ref serverPort, value);
        }

        private string partID;
        public string PartID
        {
            get => partID;
            set => SetProperty(ref partID, value);
        }

        private string partAmount;
        public string PartAmount
        {
            get => partAmount;
            set => SetProperty(ref partAmount, value);
        }

        private string receivedData;
        public string ReceivedData
        {
            get => receivedData;
            set => SetProperty(ref receivedData, value);
        }

        private StringWrapper selectedItem;
        public StringWrapper SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
        #endregion
        
        #region 绑定的指令
        private RelayCommand testCommand;
        public RelayCommand TestCommand => testCommand ?? (testCommand = new RelayCommand(() =>
        {
            //ReceivedData = Environment.GetFolderPath(Environment.SpecialFolder.) + Environment.NewLine;
            //ReceivedData = getAddress.GetAddress();
            foreach (var item in getAddress.GetIP())
            {
                if (item.Split(':')[0] != "fe80")
                    ReceivedData += item + Environment.NewLine;
            }
        }));

        private RelayCommand addCommand;
        public RelayCommand AddCommand => addCommand ?? (addCommand = new RelayCommand(() =>
        {
            try
            {
                if (PartID == "") return;
                if (PartAmount == "") return;
                Items.Add(new ItemData(PartID, int.Parse(PartAmount)));
            }
            catch (Exception e)
            {
                Shell.Current.DisplayAlert("数据添加", e.Message, "确定");
            }
        }));

        private RelayCommand clearCommand;
        public RelayCommand ClearCommand => clearCommand ?? (clearCommand = new RelayCommand(() =>
        {
            try
            {
                Items.Clear();
            }
            catch (Exception e)
            {
                Shell.Current.DisplayAlert("数据清除", e.Message, "确定");
            }
        }));

        private RelayCommand listeningCommand;
        public RelayCommand ListeningCommand => listeningCommand ?? (listeningCommand = new RelayCommand(() =>
        {
            try
            {
                if (Server != null) Server.StopListening();
                finsTCPServer = new FinsTCPServer(int.Parse(ServerIP.Split('.')[3]));
                Server = finsTCPServer.Connection;
                Server.StartListening(IPAddress.Parse(ServerIP), int.Parse(ServerPort));
                //Server = new ConnectionToolkit.SocketConnection(ServerIP, int.Parse(ServerPort));
                Server.ClientListUpdate += UpdateClientList;
                Server.ReceiveFromClient += UpdateClientInfo;
                //Server.StartListening();
                Shell.Current.DisplayAlert("服务端监听", "监听成功", "确定");
            }
            catch (Exception e)
            {
                Shell.Current.DisplayAlert("服务端监听", e.Message, "确定");
            }
        }));

        private RelayCommand stoplisteningCommand;
        public RelayCommand StopListeningCommand => stoplisteningCommand ?? (stoplisteningCommand = new RelayCommand(() =>
        {
            try
            {
                Server.StopListening();
                Shell.Current.DisplayAlert("服务端监听", "停止监听", "确定");
            }
            catch (Exception e)
            {
                Shell.Current.DisplayAlert("服务端监听", e.Message, "确定");
            }
        }));

        private RelayCommand sendCommand;
        public RelayCommand SendCommand => sendCommand ?? (sendCommand = new RelayCommand(() =>
        {
            try
            {
                if (Server.ClientDic.Count == 0) return;
                if (Server.ClientDic.ContainsKey(SelectedItem.Value))
                    Server.ClientDic[SelectedItem.Value].Send(ItemConvertor(Items));
            }
            catch (Exception e)
            {
                Shell.Current.DisplayAlert("发送数据", e.Message, "确定");
            }
        }));

        private RelayCommand seletionCommand;
        public RelayCommand SeletionCommand => seletionCommand ?? (seletionCommand = new RelayCommand(() =>
        {
            try
            {
                
            }
            catch (Exception e)
            {
                Shell.Current.DisplayAlert("选中客户端", e.Message, "确定");
            }
        }));

        private RelayCommand saveCommand;
        public RelayCommand SaveCommand => saveCommand ?? (saveCommand = new RelayCommand(() =>
        {
            try
            {
                Config.Change("ServerIP", ServerIP);
                Config.Change("ServerPort", ServerPort);
                Shell.Current.DisplayAlert("保存地址", "已保存", "确定");
            }
            catch (Exception e)
            {
                Shell.Current.DisplayAlert("保存地址", e.Message, "确定");
            }
        }));
        #endregion

        //发送数据
        public ObservableRangeCollection<ItemData> Items { get; } = new ObservableRangeCollection<ItemData>();
        //客户端列表
        public ObservableRangeCollection<StringWrapper> ClientList { get; } = new ObservableRangeCollection<StringWrapper>();
        //服务端连接实例
        public ConnectionToolkit.SocketConnection Server;
        //配置管理实例
        public KeyValueLoader Config;

        FinsTCPServer finsTCPServer;

        IAndroidNetTool getAddress;

        public MainViewModel()
        {
            getAddress = DependencyService.Get<IAndroidNetTool>();
            Config = new KeyValueLoader("Config.json", "/storage/emulated/0/Documents/BOMTestConfig");
            ServerIP = Config.Load("ServerIP");
            ServerPort = Config.Load("ServerPort");
        }

        public byte[] WordByteReverse(byte[] bytes)
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

        private void UpdateClientList()
        {
            ClientList.Clear();
            foreach (var key in Server.ClientDic.Keys)
                ClientList.Add(new StringWrapper() { Value = key });
        }

        private void UpdateClientInfo(System.Net.Sockets.Socket client, byte[] data)
        {
            //ReceivedData = Encoding.UTF8.GetString(data);
            ReceivedData += DataConverter.BytesToHexString(data.Skip(28).ToArray()) + Environment.NewLine;
        }

        private byte[] ItemConvertor(ObservableRangeCollection<ItemData> Items, bool isReverse = true)
        {
            string data = "";
            foreach (var item in Items)
                data += item.Name + ":" + item.Amount + "; ";
            if (isReverse)
                return WordByteReverse(Encoding.ASCII.GetBytes(data));
            return Encoding.ASCII.GetBytes(data);
        }
    }
}

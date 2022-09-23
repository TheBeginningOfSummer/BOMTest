using BOMTest.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using MyToolkit;
using System;
using System.Text;
using Xamarin.Forms;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace BOMTest.ViewModels
{
    public class MainViewModel : ObservableObject
    {
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

        private string selectedItem;
        public string SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        private RelayCommand testCommand;
        public RelayCommand TestCommand => testCommand ?? (testCommand = new RelayCommand(() =>
        {
            try
            {
                if (Server != null) Server.StopListening();
                Server = new ConnectionToolkit.SocketConnection(ServerIP, int.Parse(ServerPort));
                Server.ClientListUpdate += UpdateClientList;
                Server.StartListening();
                Shell.Current.DisplayAlert("服务端监听", "监听成功", "确定");
            }
            catch (Exception e)
            {
                Shell.Current.DisplayAlert("服务端监听", e.Message, "确定");
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

        private RelayCommand sendCommand;
        public RelayCommand SendCommand => sendCommand ?? (sendCommand = new RelayCommand(() =>
        {
            try
            {
                if (Server.ClientDic.Count == 0) return;
                if (Server.ClientDic.ContainsKey(SelectedItem))
                    Server.ClientDic[SelectedItem].Send(ItemConvertor(Items));
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

        public ObservableRangeCollection<ItemData> Items { get; } = new ObservableRangeCollection<ItemData>();
        public ObservableRangeCollection<StringWrapper> ClientList { get; } = new ObservableRangeCollection<StringWrapper>();

        public ConnectionToolkit.SocketConnection Server;

        public MainViewModel()
        {
            
        }

        private void UpdateClientList()
        {
            ClientList.Clear();
            foreach (var key in Server.ClientDic.Keys)
            {
                ClientList.Add(new StringWrapper() { Value = key });
            }
        }

        private byte[] ItemConvertor(ObservableRangeCollection<ItemData> Items)
        {
            string data = "";
            foreach (var item in Items)
            {
                data += item.Name + ":" + item.Amount + ";";
            }
            return Encoding.ASCII.GetBytes(data);
        }
    }
}

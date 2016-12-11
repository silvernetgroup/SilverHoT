using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Diagnostics;

namespace IoT160.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new IoT160.App());

            GetTableData();
        }

        public async void GetTableData()
        {
            string partitionKey = "SilverRPI";
            string rowKey = "1";

            try
            {
                var accountName = "silverhot";
                var accountKey = "+aR0rcYc4pF9WfhTKmayejycGGRx4tqDp7/LN95BYg5wsIP+8XCL6r0SPFSE6TNEWAi9WEHdK/2O02JYDQiCRg==";
                var credentials = new StorageCredentials(accountName, accountKey);
                var account = new CloudStorageAccount(credentials, true);
                
                CloudTableClient client = account.CreateCloudTableClient();
                CloudTable table = client.GetTableReference("RPIData");

                TableOperation op = TableOperation.Retrieve<Entity>(partitionKey, rowKey);

                TableQuery<Entity> query = new TableQuery<Entity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "SilverRPI"));

                var token = new TableContinuationToken();
                var temp = await table.ExecuteQuerySegmentedAsync(query,token);

                foreach (Entity item in temp)
                {
                    Debug.WriteLine("{0},{1},{2},{3},{4}!!!", item.PartitionKey, item.RowKey, item.deviceid, item.message, item.time);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public class Entity : TableEntity
        {
            public Entity(string pk, string rk,string msg)
            {
                this.PartitionKey = pk;
                this.RowKey = rk;
                this.message = msg;
            }
            public Entity() { }
            public string deviceid { get; set; }
            public string message { get; set; }
            public Int64 time { get; set; }

            public override string ToString()
            {
                return String.Format("{0},{1},{2},{3}",PartitionKey,RowKey,message,time);
            }
        }
    }
}

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

            //StorageUri uri = new StorageUri(https://silverhot.table.core.windows.net/RPIData);
            try
            {
                var accountName = "silverhot";
                var accountKey = "+aR0rcYc4pF9WfhTKmayejycGGRx4tqDp7/LN95BYg5wsIP+8XCL6r0SPFSE6TNEWAi9WEHdK/2O02JYDQiCRg==";
                var credentials = new StorageCredentials(accountName, accountKey);
                var account = new CloudStorageAccount(credentials, true);

                CloudTableClient client = account.CreateCloudTableClient();
                CloudTable table = client.GetTableReference("RPIData");

                //TableOperation retrieveOperation = TableOperation.Retrieve<Entity>(partitionKey, rowKey);

                //TableOperation op = TableOperation.Retrieve<Entity>(partitionKey, rowKey);
                //var res = await table.ExecuteAsync(op);
                //Entity e = (Entity)res.Result;

                TableQuery<Entity> query = new TableQuery<Entity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "SilverRPI"));

                //TableQuery<Entity> query = new TableQuery<Entity>();//.Where(TableQuery.GenerateFilterCondition(partitionKey,QueryComparisons.Equal,"SilverRPI"));
                //Debug.WriteLine(((Entity)query.Result).PartitionKey);
                //Debug.WriteLine(((Entity)query.Result).RowKey);
                /*Debug.WriteLine(e.PartitionKey);
                Debug.WriteLine(e.RowKey);
                Debug.WriteLine(e.deviceid);
                Debug.WriteLine(e.message);
                Debug.WriteLine(e.ToString());*/

                //var temp = await table.ExecuteAsync(query);

                /*foreach (Entity item in temp)
                {
                    Debug.WriteLine("{0},{1},{2},{3},{4},{5}", item.PartitionKey, item.RowKey, item.deviceid, item.message, item.time);
                }*/

                /*if (query.Result != null)
                {
                    outline = outline + ((ServiceAlertsEntity) query.Result).alertMessage + " * ";
                }
                else
                {
                    Console.WriteLine("No Alerts");
                }*/
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
            public string time { get; set; }
        }


        /*public class Entity:ITableEntity
        {
            public string ETag
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public string PartitionKey
            {
                get; set;
            }

            public string RowKey
            {
                get; set;
            }

            public DateTimeOffset Timestamp
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                throw new NotImplementedException();
            }

            public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                throw new NotImplementedException();
            }
        }*/
    }
}

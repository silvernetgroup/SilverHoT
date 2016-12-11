using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace IoT160
{
    public partial class MainPage : ContentPage
    {
        public Int64 rKtemp;
        public Int64 lVtemp;
        public Int64 tetemp;
        public Int64 hutemp;
        public MainPage()
        {
            InitializeComponent();

            ActionButton.Clicked += ActionButton_Clicked;

        }

        private void ActionButton_Clicked(object sender, EventArgs e)
        {
            TempGetTableData();
            lVtemp = 0;
            rKtemp = 0;
            tetemp = 0;
            hutemp = 0;
        }


        public async void TempGetTableData()
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
                var temp = await table.ExecuteQuerySegmentedAsync(query, token);

                foreach (Entity item in temp)
                {
                    Debug.WriteLine("{0},{1},{2},{3},{4},{5},{6}!!!", item.PartitionKey, item.RowKey, item.deviceid, item.lightValue, item.time, item.temperatureValue, item.humidityValue);
                    if (Convert.ToInt64(item.RowKey) > rKtemp)
                    {
                        rKtemp = Convert.ToInt64(item.RowKey);
                        lVtemp = Convert.ToInt64(item.lightValue);
                        hutemp = Convert.ToInt64(item.humidityValue);
                        tetemp = Convert.ToInt64(item.temperatureValue);
                        FState.Text = "Lumens: " + Convert.ToString(lVtemp);
                        HState.Text = "g/m^3: " + Convert.ToString(hutemp);
                        State.Text = "Celcius: " + Convert.ToString(tetemp);
                        Debug.WriteLine("Light Value: {0},Temperature: {1}, Humidity: {2}", lVtemp, tetemp, hutemp);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

    }   
    public class Entity : TableEntity
    {
        public Entity(string pk, string rk)
        {
            this.PartitionKey = pk;
            this.RowKey = rk;
        }
        public Entity() { }
        public string deviceid { get; set; }
        public Int64 lightValue { get; set; }

        public Int64 time { get; set; }
        public Int64 temperatureValue { get; set; }
        public Int64 humidityValue { get; set; }

        public override string ToString()
        {
            return String.Format("{0},{1},{2},{3},{4},{5}", PartitionKey, RowKey, lightValue, time, temperatureValue, humidityValue);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AzureMLTutorial
{

    public sealed partial class MainPage : Page
    {
        private string[] answers = new string[6];
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task InvokeRequestResponseService()
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"Class", "handicapped-infants", "water-project-cost-sharing", "adoption-of-the-budget-resolution", "physician-fee-freeze", "el-salvador-aid", "religious-groups-in-schools"},
                                Values = new string[,] {  { "null", answers[0], answers[1], answers[2], answers[3], answers[4], answers[5] }, }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "9+ws340Btm5hKbJt0PXGEXlQAEuwvNIVARFQYXCieG34BhmrWQwpa1z+312ixqHCGaoHNP+HOxd5LkY3SkKSIQ=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://europewest.services.azureml.net/workspaces/a802484db6f04b8eb830fdec73ea63be/services/89ce4ebde3f54faeaa78744b0cac789e/execute?api-version=2.0&details=true");

                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject<dynamic>(json);
                    var whatAreYou = result.Results.output1.value.Values[0][7];
                    var thatMuch = result.Results.output1.value.Values[0][8];
                    float percent = (float)thatMuch;
                    percent = (percent < 0.5) ? (1 - percent) * 100 : percent * 100;
                    percent = (int)percent;
                    var dialog = new MessageDialog(String.Format("You are a {0} and I'm {1}% sure about it!", whatAreYou, percent));
                    await dialog.ShowAsync();

                }
                else
                {
                    // handle failure of request
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (FrameworkElement item in myGrid.Children)
            {
                if (item is ToggleSwitch) answers[Grid.GetRow(item) - 1] = (((ToggleSwitch)item).IsOn == true) ? "y" : "n";
            }
            await InvokeRequestResponseService();
        }
    }
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }
}

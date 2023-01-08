using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using DBm;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace App2
{
    public partial class MainPage : ContentPage
    {
        public List<Employees> mylist;
        private Uri uri = new Uri("https://192.168.43.232:7117/Employee");
        public MainPage()
        {
            Title = "Employee Page";
            InitializeComponent();
        }
        
        private async void Button_Clicked(object sender, EventArgs e)
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback = 
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            var client = new HttpClient(handler);

            string rescontent = await client.GetStringAsync(uri);

            mylist = JsonConvert.DeserializeObject<List<Employees>>(rescontent);

            StringBuilder str = new StringBuilder();
            foreach (var emp in mylist)
            {
                str.Append(emp.Id);
                str.Append(' ');
                str.Append(emp.Name);
                str.Append(": ");
                str.Append(emp.Job);
                str.Append(", salary: ");
                str.Append(emp.Salary);
                str.Append(", schedule: ");
                str.Append(emp.Schedule);
                str.Append(", vac: ");
                str.Append(emp.InVacation);
                str.Append('\n');
            }

            text.Text = str.ToString();
        }
        
        private async void Button_Add_Clicked(object sender, EventArgs e)
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true;

            var client = new HttpClient(handler);
            
            var v = editor.Text.Split('+');
            var json = JsonConvert.SerializeObject(new Employees(v[0], v[1], Int32.Parse(v[2]), v[3], Int32.Parse(v[4])));
            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post,
                RequestUri = uri
            };
            await client.SendAsync(request);
            text.Text = "sent";
        }

        private async void Button_Delete_Clicked(object sender, EventArgs e)
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true;
            
            var client = new HttpClient(handler);
            var v = editor.Text.Split('\n', ' ');
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(uri.ToString() + "/" + v[0])
            };
            await client.SendAsync(request);
            text.Text = "deleted";
        }
        
        private async void Button_Update_Clicked(object sender, EventArgs e)
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true;
            
            var client = new HttpClient(handler);
            var v = editor.Text.Split('+');
            var json = JsonConvert.SerializeObject(new Employees(v[1], v[2], Int32.Parse(v[3]), v[4], Int32.Parse(v[5])) {Id = Int32.Parse(v[0])});
            HttpRequestMessage request = new HttpRequestMessage
            {
                
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Put,
                RequestUri = new Uri(uri.ToString() + "/" + v[0])
            };
            await client.SendAsync(request);
            text.Text = "updated";
        }
        
        private async void Button_Supplier_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SupplierPage());
        }
    }
}
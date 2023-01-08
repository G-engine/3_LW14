using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using DBm;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SupplierPage : ContentPage
    {
        public List<Suppliers> mylist;
        private Uri uri = new Uri("https://192.168.43.232:7117/Supplier");
        public SupplierPage()
        {
            Title = "Supplier Page";
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

            mylist = JsonConvert.DeserializeObject<List<Suppliers>>(rescontent, new JsonSerializerSettings()
            { 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            StringBuilder str = new StringBuilder();
            foreach (var s in mylist)
            {
                str.Append(s.Id);
                str.Append(' ');
                str.Append(s.Name);
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
            var supplier = new Suppliers() { Name = v[0] };
            var product = new Products(Int32.Parse(v[2]), v[1], Int32.Parse(v[3]), Int32.Parse(v[4]), supplier);
            supplier.Products.Add(product);
            var json = JsonConvert.SerializeObject(supplier, new JsonSerializerSettings()
            { 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            
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
            var supplier = new Suppliers() { Id = Int32.Parse(v[0]), Name = v[1] };
            var json = JsonConvert.SerializeObject(supplier, new JsonSerializerSettings()
            { 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            HttpRequestMessage request = new HttpRequestMessage
            {
                
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Put,
                RequestUri = new Uri(uri.ToString() + "/" + v[0])
            };
            await client.SendAsync(request);
            text.Text = "updated";
        }
        
        private async void Button_Product_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductPage());
        }
    }
}
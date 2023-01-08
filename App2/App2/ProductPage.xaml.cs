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
    public partial class ProductPage : ContentPage
    {
        public List<Products> mylist;
        private Uri uri = new Uri("https://192.168.43.232:7117/Product");
        public ProductPage()
        {
            Title = "Product Page";
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

            mylist = JsonConvert.DeserializeObject<List<Products>>(rescontent, new JsonSerializerSettings()
            { 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            StringBuilder str = new StringBuilder();
            foreach (var p in mylist)
            {
                str.Append(p.Id);
                str.Append(' ');
                str.Append(p.Name);
                str.Append(", SN: ");
                str.Append(p.SerialNumber);
                str.Append(", price: ");
                str.Append(p.Price);
                str.Append(", number: ");
                str.Append(p.Number);
                str.Append(", supplier: ");
                str.Append(p.SupplierId);
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
            var product = new Products(Int32.Parse(v[1]), v[0], Int32.Parse(v[2]), Int32.Parse(v[3]), Int32.Parse(v[4]));
            var json = JsonConvert.SerializeObject(product, new JsonSerializerSettings()
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
            var product = new Products(Int32.Parse(v[2]), v[1], Int32.Parse(v[3]), Int32.Parse(v[4]), Int32.Parse(v[5]))
                { Id = Int32.Parse(v[0]) };
            var json = JsonConvert.SerializeObject(product, new JsonSerializerSettings()
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
    }
}
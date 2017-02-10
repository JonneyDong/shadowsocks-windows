//==============================================================
//  Copyright (C) 2017 JonneyDong Inc. All rights reserved.
//
//==============================================================
//  Create by JonneyDong at 2017/2/9 17:31:01.
//  Version 1.0
//  JonneyDong [mailto:jonneydong@gmail.com]
//==============================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace shadowsocks.extends
{
    public class AutoNetProxy
    {
        private static List<string> ss = new List<string>();

        public AutoNetProxy()
        {
        }

        public async Task<string> GetProxiesByArukasCloud()
        {
            ss.Clear();
            var baseUrl_result = await Http.Get("https://superss.arukascloud.io/");
            var proxyUrls = new List<string>();

            foreach (Match regex in Regex.Matches(baseUrl_result, "href=.(.*).>点击"))
            {
                if (!proxyUrls.Contains(regex.Groups[1].Value)) proxyUrls.Add(regex.Groups[1].Value);
            }

            var tasks = new List<Task>();
            int index = 1;
            foreach (var item in proxyUrls)
            {
                //tasks.Add(ReadArukasCloude($"https://superss.arukascloud.io/{item}"));
                var jsonString = await Http.Get($"https://superss.arukascloud.io/{item}");
                var jsonList = JArray.Parse(jsonString);
                foreach (JObject jo in jsonList)
                {
                    var tmp = $"{jo.GetValue("method")}:{jo.GetValue("password")}@{jo.GetValue("server")}:{jo.GetValue("server_port")}";
                    ss.Add($"ss://{tmp.ToBase64()}#{DateTime.Today.ToString("MMdd") + "-" + index++}");
                }
            }
            //Task.WaitAll(tasks.ToArray());
            return await Task.FromResult(string.Join("|", ss));
        }
        private static async Task<int> ReadArukasCloude(string url)
        {
            try
            {

                var jsonString = await Http.Get(url);
                var jsonList = JArray.Parse(jsonString);
                foreach (JObject item in jsonList)
                {
                    System.Diagnostics.Trace.WriteLine(item.GetValue("server_port"));
                }
                //var jsonObject = JsonObject.Create(json);

                //foreach (var jsonItem in jsonObject.GetCollection())
                //{
                //    SSList.Add(
                //        new SSModel
                //        {
                //            id = jsonItem.GetValue("appid"),
                //            server = jsonItem.GetValue("server"),
                //            port = int.Parse(jsonItem.GetValue("server_port")),
                //            password = jsonItem.GetValue("password"),
                //            method = jsonItem.GetValue("method")
                //        });
                //}
            }
            catch (Exception)
            {

            }
            return await Task.FromResult(0);
        }
    }

    public static class Http
    {
        private static WebClient _http = new WebClient() { Encoding = System.Text.Encoding.UTF8 };
        private static object asynHttp = new object();

        public static async Task<string> Get(string url)
        {

            return await _http.DownloadStringTaskAsync(url);
        }

        public static string ToBase64(this string source)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
        }
    }
}

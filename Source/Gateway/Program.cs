// Copyright 2018 Louis S.Berman.
//
// This file is part of MailGateway.
//
// MailGateway is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation, either version 3 of the License, 
// or (at your option) any later version.
//
// MailGateway is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU 
// General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MailGateway.  If not, see <http://www.gnu.org/licenses/>.


using Common;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Toolbelt.Net.Smtp;

namespace Gateway
{
    class Program
    {
        static void Main()
        {
            ServicePointManager.DefaultConnectionLimit = 50;

            var config = new JobHostConfiguration();

            if (Environment.UserInteractive)
                config.UseDevelopmentSettings();

            GetContainer().CreateIfNotExists();

            StartEmailServer();

            var host = new JobHost(config);

            host.RunAndBlock();
        }

        private static void StartEmailServer()
        {
            var host = Dns.GetHostEntry(
                ConfigurationManager.AppSettings["SmtpServerHost"]);

            var server = new SmtpServerCore(host.AddressList.First(x =>
                x.AddressFamily == AddressFamily.InterNetwork),
                int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]));

            server.ReceiveMessage += (s, e) =>
            {
                var json = JsonConvert.SerializeObject(e.Message);

                var container = GetContainer();

                var blob = container.GetBlockBlobReference(
                    Guid.NewGuid().ToString("N") + ".json");

                blob.UploadText(json);

                Console.WriteLine(MiscHelpers.GetSavedToQueueMessage(e.Message));
            };

            server.Start();
        }

        private static CloudBlobContainer GetContainer()
        {
            var account = CloudStorageAccount.Parse(ConfigurationManager
                .ConnectionStrings["AzureWebJobsStorage"].ConnectionString);

            var client = account.CreateCloudBlobClient();

            return client.GetContainerReference("smtpmessagestorelay");
        }
    }
}

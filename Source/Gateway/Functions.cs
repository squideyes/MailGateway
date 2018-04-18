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
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Toolbelt.Net.Smtp;

namespace Gateway
{
    public class Functions
    {
        // TODO: Copy ALL properties from SmtpMessage to EmailMessage
        // TODO: Add logging, analytics and alerts
        public static async Task ProcessQueueMessage(
            [BlobTrigger(WellKnown.QueueName)] string json, TextWriter log)
        {
            var source = JsonConvert.DeserializeObject<SmtpMessage>(json);;

            var apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(source.From.Address);

            var tos = source.To.Select(t => new EmailAddress(t.Address)).ToList();

            var target = MailHelper.CreateSingleEmailToMultipleRecipients(
                from, tos, source.Subject, source.Body, null);

            target.AddHeader("X-OriginalMessageId", source.Id.ToString("N"));

            target.AddHeader("SourceId", source.Id.ToString("N"));

            var response = await client.SendEmailAsync(target);

            log.WriteLine($"Dispatched message \"{source.Id:N}\" via SendGrid");
        }
    }
}

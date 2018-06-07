MailGateway is a simple demo that shows how to receive emails via SMTP then dispatch those emails via SendGrid.

To run the demo, do the following:

- Create a SendGrid Account (https://sendgrid.com), if needed
- On the SendGridPortal create an API Key with  "Mail Send" priviledges 
- Add a "SendGridApiKey" Environment Variable to your local system, using the key you created in the previous step
- Compile and run the Gatwway application
- Update the SmtpSendDemo with your own email address
- Run SmtpSendDemo to dispatch an email that will be delivered through the Gateway 
 
A potential improvement would be to replace the Toolbelt.Net.Smtp library with a more advanced library like https://github.com/cosullivan/SmtpServer, but altered to allow the creation of serializable SmtpMessages.  Once that was done an API endpoint (packaged as a Functions HttpTrigger) could be developed to provide a more-secure-and-modern methodology for posting emails.
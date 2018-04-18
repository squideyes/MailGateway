MailGateway is a simple demo that shows how to receive emails via SMTP then dispatch those emails via SendGrid.

A potential improvement would be to replace the Toolbelt.Net.Smtp library with a more advanced library like https://github.com/cosullivan/SmtpServer, but altered to allow the creation of serializable SmtpMessages.  Once that was done an API endpoint (packaged as a Functions HttpTrigger) could be developed to provide a more-secure-and-modern methodology for posting emails.






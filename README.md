# SQL Data Sync Failure Monitor

![SQL Data Sync Failure Monitor sample](sample.png)

A SendGrid API Key is necessary, please follow the instructions below in case you don't have one already.
After getting the SendGrid API Key you can use this button to deploy SQL Data Sync Failure Monitor in Azure:

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fvitomaz-msft%2FSQLDataSyncFailureMonitor%2Fmaster%2Ftemplate.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>


### Prerequisite - SendGrid account

1. **We need to create a new SendGrid Email Delivery resource in Azure:**

![We need to create a new SendGrid Email Delivery resource in Azure:](pic1.png)

2. **You can pick the free pricing tier:**

![You can pick the free pricing tier:](pic2.png)

3. **We need to configure an API key, please copy the user name from the SendGrid resource you just created:**

![We need to configure an API key, please copy the user name from the SendGrid resource you just created:](pic3.png)

4. **Then navigate into https://sendgrid.com/**
**and Login with the copied username and password you chose when created the SendGrid resource.**
**Then navigate into API Keys**

![At SendGrid.com navigate into API Keys:](pic4.png)

6. **Create an API Key and save the key for later usage:**

![Create an API Key](pic5.png)


![and save the key for later usage:](pic6.png)
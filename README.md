# SQL Data Sync Failure Monitor


A SendGrid API Key is necessary, please follow the instructions below in case you don't have one already.
After getting the SendGrid API Key you can use this button to deploy SQL Data Sync Failure Monitor in Azure:

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fvitomaz-msft%2FSQLDataSyncFailureMonitor%2Fmaster%2Ftemplate.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>


### Prerequisite - SendGrid account

1. **We need to create a new SendGrid Email Delivery resource in Azure:**
<br/>
![Pic1](pic1.png)
<br/>

2. **You can pick the free pricing tier:**
<br/>
![Pic2](pic2.png)
<br/>

3. **We need to configure an API key, please copy the user name from the SendGrid resource you just created:**
<br/>
![Pic3](pic3.png)
<br/>

4. **Then navigate into https://sendgrid.com/**
**and Login with the copied username and password you chose when created the SendGrid resource.**
**Then navigate into API Keys**
<br/>
![Pic4](pic4.png)
<br/>

6. **Create an API Key and save the key for later usage:**
![Pic5](pic5.png)
<br/>
![Pic6](pic6.png)
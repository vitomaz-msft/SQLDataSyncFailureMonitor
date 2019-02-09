#r "SendGrid"
#r "System.Data"

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Azure.WebJobs.Host;
using SendGrid.Helpers.Mail;

public static SendGridMessage Run(TimerInfo myTimer, string myInputBlob, out string myOutputBlob, ILogger log)
{
    string SyncDbServer = Environment.GetEnvironmentVariable("SQLDataSyncFailureMonitor_SyncDbServer");
    string SyncDbDatabase = Environment.GetEnvironmentVariable("SQLDataSyncFailureMonitor_SyncDbDatabase");
    string SyncDbUser = Environment.GetEnvironmentVariable("SQLDataSyncFailureMonitor_SyncDbUser");
    string SyncDbPassword = Environment.GetEnvironmentVariable("SQLDataSyncFailureMonitor_SyncDbPassword");
    string ToAddress = Environment.GetEnvironmentVariable("SQLDataSyncFailureMonitor_NotifyAddress");
    string dateTimeFormat = "yyyy-MM-dd HH:mm";
    SendGridMessage message = null;

    //Get last check datetime
    DateTime lastCheck;
    if (!DateTime.TryParse(myInputBlob, out lastCheck))
    {
        log.LogInformation($"Was not possible to parse myInputBlob");
        lastCheck = DateTime.UtcNow.AddHours(-1);
    };
    log.LogInformation($"lastCheck: {lastCheck}");
    myOutputBlob = lastCheck.ToString();

    SqlConnection conn = new SqlConnection(string.Format("Server=tcp:{0}.database.windows.net,1433;Initial Catalog={1};Persist Security Info=False;User ID={2};Password={3};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", SyncDbServer, SyncDbDatabase, SyncDbUser, SyncDbPassword));
    SqlCommand cmd = new SqlCommand("SELECT [ui].[completionTime] AS [CompletionTime] ,[ui].[detailEnumId] AS [State],[ud].[server] AS [Server],[ud].[database] AS [Database],[sg].[name] AS [SyncGroup],[ui].[detailStringParameters] AS [Details] FROM [dss].[UIHistory] ui LEFT OUTER JOIN dss.syncgroup sg ON ui.syncgroupId = sg.id LEFT OUTER JOIN dss.userdatabase ud ON ui.databaseid = ud.id WHERE [ui].[recordType] = 0 AND [ui].[completionTime] >= @lastCheck", conn);
    SqlParameter lastCheckParameter = cmd.Parameters.Add("@lastCheck", System.Data.SqlDbType.DateTime);
    lastCheckParameter.Value = lastCheck;
    SqlDataReader rdr = null;
    StringBuilder sb = new StringBuilder();

    try
    {
        conn.Open();
        rdr = cmd.ExecuteReader();

        while (rdr.Read())
        {
            DateTime CompletionTime = rdr.GetDateTime(0);
            string State = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
            string Server = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
            string Database = rdr.IsDBNull(3) ? "" : rdr.GetString(3);
            string SyncGroup = rdr.IsDBNull(4) ? "" : rdr.GetString(4);
            string Details = rdr.IsDBNull(5) ? "" : rdr.GetString(5);

            var startIndex = Details.IndexOf("<string>") + 8;
            Details = Details.Substring(startIndex);
            var endIndex = Details.IndexOf("</string>");
            Details = Details.Substring(0, endIndex);

            if(State == "SyncFailure"){
                sb.AppendLine($"<tr><th style='background-color: #E74C3C;'>{State}</th></tr>");
            }
            else{
                if(State.Contains("Failure") || State.Contains("Warning")){
                    sb.AppendLine($"<tr><th style='background-color: #F39C12;'>{State}</th></tr>");
                }
            }

            sb.AppendLine($"<tr><td><b>Sync Group:</b> {SyncGroup}</td></tr>");
            sb.AppendLine($"<tr><td><b>Completion Time:</b> {CompletionTime.ToString(dateTimeFormat)} UTC</td></tr>");
            sb.AppendLine($"<tr><td><b>Server:</b> {Server}</td></tr>");
            sb.AppendLine($"<tr><td><b>Database:</b> {Database}</td></tr>");
            sb.AppendLine($"<tr><td><b>Details:</b> {Details}</td></tr>");
        }

        log.LogInformation($"sb.Length: {sb.Length}");
        log.LogInformation($"sb.ToString: {sb.ToString()}");

        if (sb.Length > 0)
        {
            StringBuilder emailContentSB = new StringBuilder();
            emailContentSB.Append(@"<style>
table { border-collapse: collapse; width: 100%;}
th, td { text-align: left; padding: 8px;}
tr:nth-child(even){background-color: #f2f2f2}
th { color: white;}
</style>
<div style='overflow-x:auto;'>");
            emailContentSB.Append($"<h3>Data Sync alerts at {DateTime.UtcNow.ToString(dateTimeFormat)} UTC</h3>");
            emailContentSB.Append($"Last check at {lastCheck.ToString("s")}</h3><br/>");
            emailContentSB.Append(@"<table >");
            emailContentSB.Append(sb.ToString());
            emailContentSB.Append(@"</table></div>");

            message = new SendGridMessage() { Subject = "Data Sync failure alert" };
            message.AddTo(ToAddress);
            message.AddContent("text/html", emailContentSB.ToString());
            myOutputBlob = DateTime.UtcNow.ToString();
        }
    }
    catch (Exception ex)
    {
        log.LogError($"Exception: {ex.Message}");
    }
    finally
    {
        if (rdr != null) { rdr.Close(); }
        if (conn != null) { conn.Close(); }
    }
    return message;
}
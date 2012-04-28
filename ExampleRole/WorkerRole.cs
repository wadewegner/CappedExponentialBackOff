using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace ExampleRole
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("$projectname$ entry point called", "Information");

            string queueName = "queuetest";

            int minInterval = 1;
            int interval = minInterval;

            int exponent = 2;
            int maxInterval = 60;

            CloudStorageAccount account = CloudStorageAccount.DevelopmentStorageAccount;
            CloudQueueClient queueClient = account.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExist();

            while (true)
            {
                var msg = queue.GetMessage();
                if (msg != null)
                {
                    // do something
                    queue.DeleteMessage(msg);
                    interval = minInterval;

                    Trace.WriteLine(string.Format("Duration reset to {0} seconds", interval));
                }
                else
                {
                    Trace.WriteLine(string.Format("Sleep for {0} seconds", interval));
                    Thread.Sleep(TimeSpan.FromSeconds(interval));
                    interval = Math.Min(maxInterval, interval * exponent);
                }
            }

        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}

using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp.Google;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace ElYamanyDashboard.Utils
{
    
        public class PushManager
        {

       public static void pushToAndroidDevice2(String token, string title, string message,long NotificationTypeId,long NotificationToId)
            {
            var provider = "GCM";

            string AppId = "27568461234";
            string ServerToken = "AAAABms1VbI:APA91bH3GKY6inj5TcX39omZ-iA6XHcq3Fl2HdRuclx-drtsXHuuqR1izKi7PIRXrD73Q7RKnML4v93NsHf2giEc6-2BQZadZfxqbwU_Bva-aIJ_K8-Z1tij6bAKiqZ8WSmLdzVCb-PC";

                // Configuration
                var config = new GcmConfiguration(AppId, ServerToken, null);
                config.OverrideUrl("https://fcm.googleapis.com/fcm/send");

            // Create a new broker
            var gcmBroker = new GcmServiceBroker(config);

            // Wire up events
            gcmBroker.OnNotificationFailed += (notification, aggregateEx) => {

                aggregateEx.Handle(ex => {

                    // See what kind of exception it was to further diagnose
                    if (ex is GcmNotificationException notificationException)
                    {

                        // Deal with the failed notification
                        var gcmNotification = notificationException.Notification;
                        var description = notificationException.Description;

                        Console.WriteLine($"{provider} Notification Failed: ID={gcmNotification.MessageId}, Desc={description}");
                    }
                    else if (ex is GcmMulticastResultException multicastException)
                    {

                        foreach (var succeededNotification in multicastException.Succeeded)
                        {
                            Console.WriteLine($"{provider} Notification Succeeded: ID={succeededNotification.MessageId}");
                        }

                        foreach (var failedKvp in multicastException.Failed)
                        {
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;

                            Console.WriteLine($"{provider} Notification Failed: ID={n.MessageId}, Desc={e.Message}");
                        }

                    }
                    else if (ex is DeviceSubscriptionExpiredException expiredException)
                    {

                        var oldId = expiredException.OldSubscriptionId;
                        var newId = expiredException.NewSubscriptionId;

                        Console.WriteLine($"Device RegistrationId Expired: {oldId}");

                        if (!string.IsNullOrWhiteSpace(newId))
                        {
                            // If this value isn't null, our subscription changed and we should update our database
                            Console.WriteLine($"Device RegistrationId Changed To: {newId}");
                        }
                    }
                    else if (ex is RetryAfterException retryException)
                    {

                        // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                        Console.WriteLine($"{provider} Rate Limited, don't send more until after {retryException.RetryAfterUtc}");
                    }
                    else
                    {
                        Console.WriteLine("{provider} Notification Failed for some unknown reason");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            // Wire up events

            gcmBroker.OnNotificationSucceeded += (notification) => {
                    Console.WriteLine("GCM Notification Sent!");
                };

                // Start the broker
                gcmBroker.Start();
                // Queue a notification to send
                gcmBroker.QueueNotification(new GcmNotification
                {
                    RegistrationIds = new List<string> {
                                        token
                                    },

                    Data = JObject.Parse("{ \"title\" : \"" + title + "\",\"body\" : \"" + message + "\",\"NotificationTypeId\" : \"" + NotificationTypeId + "\",\"NotificationToId\" : \"" + NotificationToId + "\"}")
                });


                // Stop the broker, wait for it to finish   
                // This isn't done after every message, but after you're
                // done with the broker
                gcmBroker.Stop();

            }

        public static void pushToAndroidDevice(String token, string title, string messageToPush, long NotificationTypeId, long NotificationToId)
        {
            string AppId = "27568461234";
            string ServerToken = "AAAABms1VbI:APA91bH3GKY6inj5TcX39omZ-iA6XHcq3Fl2HdRuclx-drtsXHuuqR1izKi7PIRXrD73Q7RKnML4v93NsHf2giEc6-2BQZadZfxqbwU_Bva-aIJ_K8-Z1tij6bAKiqZ8WSmLdzVCb-PC";
            try
            {

                var message = new
                {
                    to = token,
                    priority = "high",
                    content_available = true,
                    notification = new
                    {
                        body = messageToPush,
                        title = title,
                        sound = "default"
                    },
                    data = new
                    {
                        id = NotificationToId,
                        notificationtype = ((int)NotificationTypeId).ToString(),
                    }
                };

                var Serializer = new JavaScriptSerializer();
                var Json = Serializer.Serialize(message);
                var byteArray = Encoding.UTF8.GetBytes(Json);
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ServerToken));
                tRequest.Headers.Add(string.Format("Sender: id={0}", AppId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                string sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }


        }

        public static void PushToIphoneDevice(String token, string message)
        {
            try
            {
                string cerPath = "/Certificates/Certificates.p12";
                var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string cerPassword = "Big4Big4";
                string fullPath = basePath + cerPath;

                var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Sandbox, fullPath, cerPassword);


                // Create a new broker
                var apnsBroker = new ApnsServiceBroker(config);

                apnsBroker.OnNotificationFailed += (notification, aggregateEx) => {

                    aggregateEx.Handle(ex => {

                        // See what kind of exception it was to further diagnose
                        if (ex is GcmNotificationException)
                        {
                            var notificationException = (GcmNotificationException)ex;

                            // Deal with the failed notification
                            var gcmNotification = notificationException.Notification;
                            var description = notificationException.Description;

                            Console.WriteLine($"GCM Notification Failed: ID={gcmNotification.MessageId}, Desc={description}");
                        }
                        else if (ex is GcmMulticastResultException)
                        {
                            var multicastException = (GcmMulticastResultException)ex;

                            foreach (var succeededNotification in multicastException.Succeeded)
                            {
                                Console.WriteLine($"GCM Notification Succeeded: ID={succeededNotification.MessageId}");
                            }

                            foreach (var failedKvp in multicastException.Failed)
                            {
                                var n = failedKvp.Key;
                                var e = failedKvp.Value;

                                Console.WriteLine($"GCM Notification Failed: ID={n.MessageId}, Desc={e.Message}");
                            }

                        }
                        else if (ex is DeviceSubscriptionExpiredException)
                        {
                            var expiredException = (DeviceSubscriptionExpiredException)ex;

                            var oldId = expiredException.OldSubscriptionId;
                            var newId = expiredException.NewSubscriptionId;

                            Console.WriteLine($"Device RegistrationId Expired: {oldId}");

                            if (!string.IsNullOrWhiteSpace(newId))
                            {
                                // If this value isn't null, our subscription changed and we should update our database
                                Console.WriteLine($"Device RegistrationId Changed To: {newId}");
                            }
                        }
                        else if (ex is RetryAfterException)
                        {
                            var retryException = (RetryAfterException)ex;
                            // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                            Console.WriteLine($"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}");
                        }
                        else
                        {
                            Console.WriteLine("GCM Notification Failed for some unknown reason");
                        }

                        // Mark it as handled
                        return true;
                    });
                };

                apnsBroker.OnNotificationSucceeded += (notification) =>
                {
                    Console.WriteLine("Apple Notification Sent!");
                };

                // Start the broker
                apnsBroker.Start();

                // var modelToJson = JsonConvert.SerializeObject(technicianNotification);

                string title = "Big4app";
                string payloadobject = "{\"aps\" :"
                + " {\"alert\" : {\"title\" : \"" + title + "\",\"body\" :\"" + message + "\"},"
                + "\"badge\" : 1" + ", \"sound\":\"default\"}}";
                var Payloadd = JObject.Parse(payloadobject);
                // Queue a notification to send
                apnsBroker.QueueNotification(new ApnsNotification
                {
                    DeviceToken = token,
                    Payload = Payloadd
                });

                // Stop the broker, wait for it to finish   
                // This isn't done after every message, but after you're
                // done with the broker
                apnsBroker.Stop();

            }
            catch (Exception e)
            {

            }

        }

    }

}
using AHI.Infrastructure.AzureCoapTriggerExtension;
using AHI.Infrastructure.AzureCoapTriggerExtension.Bindings;
using AHI.Infrastructure.AzureCoapTriggerExtension.Messaging;
using CoAP;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureCoapTriggerExtensions.Listeners
{
    public class CoapTriggerListener : IListener
    {
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly CoapTriggerAttribute _attribute;

        private bool _started;

        public CoapTriggerListener(
            ITriggeredFunctionExecutor executor,
            CoapTriggerAttribute attribute)
        {
            _executor = executor;
            _attribute = attribute;
        }

        public void Cancel()
        {
            StopAsync(CancellationToken.None).Wait();
        }

        public void Dispose()
        {
            return;
        }

        private void RecurringTask(System.Action action, int seconds, CancellationToken token)
        {
            if (action == null)
                return;
            Task.Run(async () => {
                while (!token.IsCancellationRequested)
                {
                    action();
                    await Task.Delay(TimeSpan.FromSeconds(seconds), token);
                }
            }, token);
        }

        private void SendHeartBeat(
            string server,
            string clientId,
            string token)
        {
            var heartbeat = Request.NewPut();
            heartbeat.URI = new Uri($"coap://{server}/mqtt/connection?clientid={clientId}&token={token}");
            heartbeat.Send();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_started)
            {
                throw new InvalidOperationException("The listener has already been started.");
            }

            var coapConnectionString = new CoapConnectionString(_attribute.ConnectionString);
            string clientId = Guid.NewGuid().ToString();
            var client = new CoapClient(new Uri($"coap://{coapConnectionString.Server}/mqtt/connection?clientid={clientId}&username={coapConnectionString.Username}&password={coapConnectionString.Password}"));
            var responseMessage = client.Post("");
            var token = responseMessage.ResponseText;

            try
            {
                RecurringTask(() => SendHeartBeat(coapConnectionString.Server, clientId, token), 10, cancellationToken);

                Request request = Request.NewGet();
                request.URI = new Uri($"coap://{coapConnectionString.Server}/ps/coap/{_attribute.TopicName}?clientid={clientId}&token={token}");
                request.MarkObserve();
                request.Send();

                request.Respond += async (o, e) =>
                {
                    Response response = e.Response;
                    if (response != null)
                    {
                        var message = string.IsNullOrWhiteSpace(response.PayloadString) ? string.Empty : response.PayloadString;
                        await _executor.TryExecuteAsync(new TriggeredFunctionData() { TriggerValue = new CoapMessage(_attribute.TopicName, Encoding.UTF8.GetBytes(message)) }, cancellationToken);
                    }
                    //Console.WriteLine(Utils.ToString(response));
                };

                request.TimedOut += (o, e) =>
                {
                    SendHeartBeat(coapConnectionString.Server, clientId, token);
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed executing request: " + ex.Message);
                Console.WriteLine(ex);
            }

            _started = true;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

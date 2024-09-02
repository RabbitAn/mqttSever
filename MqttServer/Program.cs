using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Adapter;
using MQTTnet.Server;
using System.Diagnostics.Tracing;
using System.Net.Security;
using MQTTnet.Protocol;


namespace MqttServer
{
    class Program
    {
        private IMqttServerAdapter mqttServer = null;
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            var mqttServerOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()

                .WithDefaultEndpointPort(1883)

                .Build();
            var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

            mqttServer.ValidatingConnectionAsync += e =>
            {
                if (e.UserName != "admin" || e.Password != "Wgp19930605.")
                {
                    e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                }
                else
                {
                    e.ReasonCode = MqttConnectReasonCode.Success;
                }
                return Task.CompletedTask;
            };

            mqttServer.ClientDisconnectedAsync += e =>
            {
                Console.WriteLine($"客户端已断开连接: {e.ClientId}");
                return Task.CompletedTask;
            };

            mqttServer.ApplicationMessageEnqueuedOrDroppedAsync += e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                Console.WriteLine($"收到消息 - 主题: {e.ApplicationMessage.Topic}, 内容: {payload}");
                return Task.CompletedTask;
            };


            await mqttServer.StartAsync();

            Console.WriteLine("MQTT服务器已启动，监听所有网络接口");

            Console.WriteLine("按下任意键退出...");
            Console.ReadKey();

            await mqttServer.StopAsync();

            Console.WriteLine("MQTT服务器已停止");
        }


        //private async  void ReplyMessage(string topic,string message )
        //{
        //    //回复消息
        //    var replyMessage = new MqttApplicationMessageBuilder()
        //        .WithTopic(topic)
        //        .WithPayload($"已收到话题:{topic},消息:{message}")
        //        .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
        //        .Build();
        //    await mqttServer.PublishAsync(replyMessage);
        //}
    }
}
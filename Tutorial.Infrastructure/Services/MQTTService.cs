using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutorial.Infrastructure.Facades.Common.HttpClients;
using Tutorial.Infrastructure.Services.Interfaces;

namespace Tutorial.Infrastructure.Services
{
    public class MQTTService : IMQTTService
    {
        private static MqttFactory _mqttFactory = new MqttFactory();
        private static IMqttClient _mqttClient = _mqttFactory.CreateMqttClient();
        private static MqttClientOptions _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("9c8e149b9c6249ddade9e41bfb1a25ac.s1.eu.hivemq.cloud", 8883)
            .WithCredentials("tuandao8826", "Tuandao22820038826@")
            .WithTls()
            .Build();
        private MqttClientSubscribeOptions _mqttClientSubscribeOptions;
        private TaskCompletionSource<string> _messageCompletionSource;

        public MQTTService()
        {
            // Kết nối đến MQTT Broker
        }

        public async Task ConnectAsync()
        {
            // Kết nối đến MQTT broker
            if (!_mqttClient.IsConnected)
                await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);

            // Đăng ký event nhận tin nhắn chỉ một lần khi kết nối
            _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
        }

        public async Task CreateSubscribeOptionsBuilder(string topic)
        {
            _mqttClientSubscribeOptions = _mqttFactory
                .CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic(topic); })
                .Build();

            await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build());
        }

        public async Task SendMessageAsync(string topic, string payload)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
        }

        // Hàm xử lý tin nhắn nhận được
        private async Task<string> OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            // Trả kết quả cho TaskCompletionSource khi nhận được tin nhắn
            _messageCompletionSource?.TrySetResult(payload);

            return payload;
        }

        // Hàm chờ tin nhắn và trả về kết quả
        public async Task<string> WaitForMessageAsync()
        {
            _messageCompletionSource = new TaskCompletionSource<string>();
            return await _messageCompletionSource.Task;
        }
    }
}

﻿using Helpers;
using Models;
using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MySocket
{
    public class MyClient
    {
     //   readonly string terminator = "|||";
        EasyClient client;
        public delegate void ReceiveHandle(ReceieveMessage message);
        public event ReceiveHandle OnReveieveData;
        readonly string serverIP = System.Configuration.ConfigurationManager.AppSettings["serverIP"];
        readonly int serverPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["serverPort"]);
        bool connected;

        public bool Connected
        {
            get
            {
                return connected;
            }

            set
            {
                connected = value;
            }
        }

        public MyClient()
        {
            client = new EasyClient();
            client.Initialize(new MyReceiveFilter(), (response) =>
            {
                OnReveieveData(response);
            });

            Connected = client.ConnectAsync(new IPEndPoint(IPAddress.Parse(serverIP), serverPort)).Result;
        }


        public void SendMessage<T>(SendMessage<T> message) where T : class, new()
        {
            if (client.IsConnected)
            {
                var messageByte = CreateSendMessageByte(message);
                client.Send(messageByte);
            }
            else
            {
                throw new Exception("客户端未连接到服务器");
            }

        }

        private byte[] CreateSendMessageByte<T>(SendMessage<T> message) where T : class, new()
        {
            string jsonString = JsonHelper.SerializeObj(message.Data);
            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonString);
            string time = DateTime.Now.ToString("yyyyMMddHHmmss");
            byte[] timeBytes = Encoding.UTF8.GetBytes(time);
            var actionBytes = BitConverter.GetBytes(message.Action);
            var lengthByte = BitConverter.GetBytes(dataBytes.Length + timeBytes.Length + actionBytes.Length);// StringHelper.ConvertIntToByteArray4(dataBytes.Length + 18, ref buf);

            List<byte> byteSource = new List<byte>();
            byteSource.AddRange(lengthByte);
            byteSource.AddRange(timeBytes);
            byteSource.AddRange(actionBytes);
            byteSource.AddRange(dataBytes);
            return byteSource.ToArray();
        }
    }

    class MyReceiveFilter : FixedHeaderReceiveFilter<ReceieveMessage>
    {
        public MyReceiveFilter()
        : base(4) // two vertical bars as package terminator
        {
        }

        public override ReceieveMessage ResolvePackage(IBufferStream bs)
        {
            ReceieveMessage message = new ReceieveMessage();
            var lenBytes = new byte[4];
            bs.Read(lenBytes, 0, 4);
            message.Length = BitConverter.ToInt32(lenBytes, 0);
            var timeBytes = new byte[14];
            bs.Read(timeBytes, 0, 14);
            message.TimeStamp = Encoding.UTF8.GetString(timeBytes);
            var actionBytes = new byte[4];
            bs.Read(actionBytes, 0, 4);
            message.Action = BitConverter.ToInt32(actionBytes, 0);
            var dataLength = message.Length - 18;
            var dataBytes = new byte[dataLength];
            bs.Read(dataBytes, 0, dataLength);
            message.DataStr = Encoding.UTF8.GetString(dataBytes);
            return message;
        }


        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            var lenBytes = new byte[length];
            bufferStream.Read(lenBytes, 0, length);
            return BitConverter.ToInt32(lenBytes, 0);
        }
    }


    public class ReceieveMessage : IPackageInfo
    {
        public int Length { get; set; }
        public string TimeStamp { get; set; }

        public string DataStr { get; set; }

        public int Action { get; set; }


    }

}

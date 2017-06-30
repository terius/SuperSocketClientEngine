using Helpers;
using Models;
using MySocket;
using System;


namespace SuperSocketClientEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            MyClient client = new MyClient();
            if(!client.Connected)
            {
                Console.WriteLine("未连接到服务器");
            }
            client.OnReveieveData += Client_OnReveieveData;
            Console.WriteLine("请输入username");
            while (true)
            {
                string str = Console.ReadLine();
                var msg = CreateLoginMessage(str, "老师登录", DateTime.Now.ToLongTimeString(), ClientRole.Teacher);
                client.SendMessage(msg);
            }
           
        }

        private static void Client_OnReveieveData(ReceieveMessage message)
        {
            Console.WriteLine("收到信息：" + JsonHelper.SerializeObj(message) + "\r\n");
        }

        static SendMessage<LoginInfo> CreateLoginMessage(string userName, string nickName, string password, ClientRole clientRole)
        {
            // GetAudioName();
            var loginInfo = new LoginInfo();

            loginInfo.username = userName;
            loginInfo.nickname = nickName;
            loginInfo.password = password;  

            loginInfo.clientRole = clientRole;
            loginInfo.clientStyle = ClientStyle.PC;
            SendMessage<LoginInfo> message = new SendMessage<LoginInfo>();
            message.Action = (int)CommandType.UserLogin;
            message.Data = loginInfo;
            return message;
        }



    }

    public class LoginInfo
    {
        public string username { get; set; }
        public string password { get; set; }

        public string nickname { get; set; }

        public string no { get; set; }

        public ClientStyle clientStyle { get; set; }
        public ClientRole clientRole { get; set; }
    }



}

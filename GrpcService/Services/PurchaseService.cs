using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using XsollaSchoolBackend.Proto;
using System.Net.Mail;

namespace XsollaSchoolBackend.Services
{
    public class PurchaseService : Purchase.PurchaseBase
    {
        public override Task<ReplyMsg> SendEmail(RequestMsg input, ServerCallContext context)
        {
            string mailText = $"<h1>Спасибо за покупку в MerchShop.</h1> " +
                $"Ваш заказ {input.OrderNumber} будет отправлен в ближайшее время. <br>Список приобритенных товаров:<br>";

            string goodsInfo = "";
            foreach (var item in input.Items)
                goodsInfo += $"{item.ItemName} {item.Price} руб. Кол-во: {item.Count} шт. <br>";
            string goodsResult = $"Общая стоимость товаров: {input.Items.Sum(i => i.Price * i.Count)} руб.";
            mailText += goodsInfo + goodsResult;


            string to = input.Email;
            string from = "gitBleidd@gmail.com";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Sending email using Asp.Net and gRPC";
            message.Body = mailText;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            using (SmtpClient client = new SmtpClient())
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(from, Environment.GetEnvironmentVariable("EmailPassword"));
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;


                var replyMsg = Task.FromResult(new ReplyMsg());
                try
                {
                    client.Send(message);
                    replyMsg.Result.Message = 0;
                }

                catch (Exception e)
                {
                    //TODO логгирование ошибки
                    Console.WriteLine(e.Message);
                    replyMsg.Result.Message = -1;
                }
                return replyMsg;
            }

            
        }
    }
}

using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XsollaSchoolBackend.Data;
using XsollaSchoolBackend.Models;
using XsollaSchoolBackend.Proto;

namespace XsollaSchoolBackend.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IItemRepository _repository;

        public OrderController(IItemRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<int> CreateNewComment([FromBody] Order order)
        {
            // Формируем сообщение для gRpc
            var requestMsg = new RequestMsg
            {
                Email = order.Email,
                OrderNumber = Math.Abs((order.Email + DateTime.Now.ToString()).GetHashCode()).ToString()
            };

            // Ищем информацию о товарах по id и добавляем в gRpc сообщение
            foreach (var item in order.Items)
            {
                var res = _repository.GetItemById(item.Key);
                if (res is null)
                    return BadRequest("The request contains a non-existing item.");

                requestMsg.Items.Add(new Proto.Item { ItemName = res.Name, Price = res.Price, Count = item.Value });
            }
            
            var channel = GrpcChannel.ForAddress("https://localhost:5011");
            var client = new Purchase.PurchaseClient(channel);
            var reply = client.SendEmail(requestMsg);

            return Created("", requestMsg.OrderNumber);
        }
    }
}

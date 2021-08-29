using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XsollaSchoolBackend.Data;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.Controllers
{
    [Route("api/merch")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class MerchController : ControllerBase
    {
        private readonly IItemRepository _repository;
        private readonly IDistributedCache _cache;

        public MerchController(IItemRepository repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        /// <summary>
        /// Получение информации о товаре по его id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Item> GetItemById(int id)
        {
            using var redis = ConnectionMultiplexer.Connect("localhost");
            var db = redis.GetDatabase();

            // Формат ключа: "page:[page url]:[date]"
            string key = $"page:{HttpContext.Request.Path.Value}:{DateTime.Now.ToString("dd-MM-yyyy")}";
            if (db.StringGet(key).IsNull)
            {
                db.StringSet(key, "0");
            }
            db.StringIncrement(key);

            var res = _repository.GetItemById(id);
            if (res == null)
                return NotFound();
            return Ok(res);
        }

        /// <summary>
        /// Получение информации о товаре по его sku
        /// </summary>
        [HttpGet("sku/{sku}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Item> GetItemBySku(string sku)
        {
            var res = _repository.GetItemBySku(sku);
            if (res == null)
                return NotFound();
            return Ok(res);
        }

        /// <summary>
        /// Получение каталога товаров по частям, с возможностью сортировки по стоимости и типам
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Item>> GetCatalog([FromQuery] string type, [FromQuery] string sortBy = "-price", [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var res = _repository.GetAllItems(type, sortBy, page, pageSize);

            foreach (var header in res.Headers)
            {
                Response.Headers.Add(header.Key, header.Value);
            }
            return Ok(res.Items);
        }

        /// <summary>
        /// Создает товар на основе входящего JSON и возвращает его id
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<int> CreateNewItem([FromBody] Item newItem)
        {
            var res = _repository.CreateNewItem(newItem);
            return Created("", res.Id);
        }

        /// <summary>
        /// Редактирование всех данных о товаре по его id
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateItem(int id, [FromBody] Item updatedItem)
        {
            var isUpdated = _repository.UpdateItem(id, updatedItem);
            if (!isUpdated)
                return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Редактирование всех данных о товаре по его sku
        /// </summary>
        [HttpPut("sku/{sku}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateItemBySku(string sku, [FromBody] Item updatedItem)
        {
            var isUpdated = _repository.UpdateItemBySku(sku, updatedItem);
            if (!isUpdated)
                return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Удаление товара по его id
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeleteItem(int id)
        {
            var isDeleted = _repository.DeleteItem(id);
            if (!isDeleted)
                return BadRequest();
            return NoContent();
        }

        /// <summary>
        /// Удаление товара по его sku
        /// </summary>
        [HttpDelete("sku/{sku}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeleteItemBySku(string sku)
        {
            var isDeleted = _repository.DeleteItemBySku(sku);
            if (!isDeleted)
                return BadRequest();
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
    [Authorize]
    public class MerchController : ControllerBase
    {
        private readonly IItemRepository _itemRepo;
        private readonly IAccountRepository _accountRepo;

        public MerchController(IItemRepository repository, IAccountRepository accountRepo)
        {
            _itemRepo = repository;
            _accountRepo = accountRepo;
        }

        /// <summary>
        /// Получение информации о товаре по его id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public ActionResult<Item> GetItemById(int id)
        {
            // Проверка авторизации
            var userInfo = Utils.ClaimUtil.ParseClaims(User.Claims);
            var userDbInfo = _accountRepo.GetUserByEmail(userInfo.Email);
            if (userDbInfo == null || (userDbInfo.RoleName != "vendor" && userDbInfo.RoleName != "consumer"))
                return Forbid();

            var res = _itemRepo.GetItemById(id);

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
            // Проверка авторизации
            var userInfo = Utils.ClaimUtil.ParseClaims(User.Claims);
            var userDbInfo = _accountRepo.GetUserByEmail(userInfo.Email);
            if (userDbInfo == null || (userDbInfo.RoleName != "vendor" && userDbInfo.RoleName != "consumer"))
                return Forbid();

            var res = _itemRepo.GetItemBySku(sku);
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
            // Проверка авторизации
            var userInfo = Utils.ClaimUtil.ParseClaims(User.Claims);
            var userDbInfo = _accountRepo.GetUserByEmail(userInfo.Email);
            if (userDbInfo == null || (userDbInfo.RoleName != "vendor" && userDbInfo.RoleName != "consumer"))
                return Forbid();

            var res = _itemRepo.GetAllItems(type, sortBy, page, pageSize);

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
            // Проверка авторизации
            var userInfo = Utils.ClaimUtil.ParseClaims(User.Claims);
            var userDbInfo = _accountRepo.GetUserByEmail(userInfo.Email);
            if (userDbInfo == null || userDbInfo.RoleName != "vendor")
                return Forbid();

            var res = _itemRepo.CreateNewItem(newItem);
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
            // Проверка авторизации
            var userInfo = Utils.ClaimUtil.ParseClaims(User.Claims);
            var userDbInfo = _accountRepo.GetUserByEmail(userInfo.Email);
            if (userDbInfo == null || userDbInfo.RoleName != "vendor")
                return Forbid();

            var isUpdated = _itemRepo.UpdateItem(id, updatedItem);
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
            // Проверка авторизации
            var userInfo = Utils.ClaimUtil.ParseClaims(User.Claims);
            var userDbInfo = _accountRepo.GetUserByEmail(userInfo.Email);
            if (userDbInfo == null || userDbInfo.RoleName != "vendor")
                return Forbid();

            var isUpdated = _itemRepo.UpdateItemBySku(sku, updatedItem);
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
            // Проверка авторизации
            var userInfo = Utils.ClaimUtil.ParseClaims(User.Claims);
            var userDbInfo = _accountRepo.GetUserByEmail(userInfo.Email);
            if (userDbInfo == null || userDbInfo.RoleName != "vendor")
                return Forbid();

            var isDeleted = _itemRepo.DeleteItem(id);
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
            // Проверка авторизации
            var userInfo = Utils.ClaimUtil.ParseClaims(User.Claims);
            var userDbInfo = _accountRepo.GetUserByEmail(userInfo.Email);
            if (userDbInfo == null || userDbInfo.RoleName != "vendor")
                return Forbid();

            var isDeleted = _itemRepo.DeleteItemBySku(sku);
            if (!isDeleted)
                return BadRequest();
            return NoContent();
        }
    }
}

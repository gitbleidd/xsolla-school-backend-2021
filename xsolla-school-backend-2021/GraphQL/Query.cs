using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XsollaSchoolBackend.Data;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.GraphQL
{
    public class Query
    {
        public Item GetItemById([Service]IItemRepository itemRepository, int id) => itemRepository.GetItemById(id);

        public List<Item> GetAllItems([Service] IItemRepository itemRepository, string type, string sortBy, int page, int pageSize) 
            => itemRepository.GetAllItems(type, sortBy, page, pageSize).Items;
    }
}

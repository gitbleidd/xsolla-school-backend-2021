using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XsollaSchoolBackend.Data;
using XsollaSchoolBackend.Models;


namespace XsollaSchoolBackend.GraphQL
{
    public class Mutation
    {
        private readonly IItemRepository _itemRepository;
        public Mutation(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public Item AddItem(ItemInput input)
        {
            var item = new Item { Name = input.Name, Count = input.Count, Type = input.Type, Price = input.Price };
            return _itemRepository.CreateNewItem(item);
        }
        public bool UpdateItem(int id, ItemInput input)
        {
            var item = new Item { Name = input.Name, Count = input.Count, Type = input.Type, Price = input.Price };
            return _itemRepository.UpdateItem(id, item);
        }
        public bool DeleteItem(int id)
        {
            return _itemRepository.DeleteItem(id);
        }
    }
}

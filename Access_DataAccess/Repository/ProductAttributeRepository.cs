﻿using Access_DataAccess.Data;
using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_DataAccess.Repository
{
    public class ProductAttributeRepository : Repository<ProductAttribute>, IProductAttributeRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductAttributeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == nameof(AttributeType))
            {
                List<SelectListItem> listItem = new List<SelectListItem>();
                listItem.AddRange(_db.AttributeType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }));
                return listItem;
            }
            if (obj == nameof(AttributeValue))
            {
                List<SelectListItem> listItem = new List<SelectListItem>();
                listItem.AddRange(_db.AttributeValue.Select(i => new SelectListItem
                {
                    Text = i.Value,
                    Value = i.Id.ToString()
                }));
                return listItem;
            }
            return null;
        }

        public void Update(ProductAttribute obj)
        {
            _db.ProductAttribute.Update(obj);
        }
    }
}
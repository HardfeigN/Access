﻿using Access_Models;
using Access_Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);
        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
        Task<IEnumerable<SelectListItem>> GetAllDropdownListAsync(string obj);
        IndividualProductVM GetIndividualProductVM(int id);
        Task<IndividualProductVM> GetIndividualProductVMAsync(int id);
    }
}

using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Access.Controllers
{
    public class ProductAttributeController : Controller
    {
        private readonly IProductAttributeRepository _prodAttrRepos;
        private readonly IProductImageRepository _prodImgRepos;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductAttributeController(IProductAttributeRepository prodAttrRepos, IProductImageRepository prodImgRepos, IWebHostEnvironment webHostEnvironment)
        {
            _prodAttrRepos = prodAttrRepos;
            _prodImgRepos = prodImgRepos;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            ICollection<ProductAttribute> objList = _prodAttrRepos.GetAll(includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)},{nameof(Product)}");
            return View(objList);
        }


        //GET - Upsert
        public IActionResult Upsert(int? id)
        {
            ProductAttributeVM productAttributeVM = new ProductAttributeVM()
            {
                ProductAttribute = new ProductAttribute(),
                AttributeTypeSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeType)),
                AttributeValueSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeValue)),
                ProductImages = new List<ProductImage>()
            };

            if (id == null)
            {
                //for create
                return View(productAttributeVM);
            }
            else
            {
                productAttributeVM.ProductAttribute = _prodAttrRepos.Find(id.GetValueOrDefault());
                if (productAttributeVM.ProductAttribute == null)
                {
                    return NotFound();
                }
                productAttributeVM.ProductImages = _prodImgRepos.GetProductAttributeImages(productAttributeVM.ProductAttribute.Id);
                return View(productAttributeVM);
            }
        }

        //POST - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductAttributeVM productAttributeVM)
        {
            if (ModelState.IsValid)
            {
                if (productAttributeVM.ProductAttribute.Id == 0)
                {
                    //create
                    _prodAttrRepos.Add(productAttributeVM.ProductAttribute);
                    _prodAttrRepos.Save();

                    var files = HttpContext.Request.Form.Files;
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string upload = webRootPath + WebConstants.ProductImagePath + $"{productAttributeVM.ProductAttribute.ProductId}\\";
                    List<ProductImage> images = new List<ProductImage>() ;
                    if (!Directory.Exists(upload))
                    {
                        Directory.CreateDirectory(upload);
                    }


                    for (int i = 0; i < files.Count(); i++)
                    {

                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[i].FileName);
                        using (var fileStrem = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[i].CopyTo(fileStrem);
                        };
                        ProductImage productImage = new ProductImage()
                        {
                            Name = fileName + extension,
                            AttributeId = productAttributeVM.ProductAttribute.Id,
                            ProductId = productAttributeVM.ProductAttribute.ProductId,
                            ImageNumber = i
                        };
                        images.Add(productImage);
                        _prodImgRepos.Add(productImage);
                    }
                    productAttributeVM.ProductImages = images;
                    _prodImgRepos.Save();
                    TempData[WebConstants.Success] = "Pruduct Attribute created successfully";
                }
                else
                {
                    //update

                    /*
                    if (files.Count > 0)
                    {
                        string extension = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStrem = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStrem);
                        };

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    */

                    _prodAttrRepos.Update(productAttributeVM.ProductAttribute);
                    _prodAttrRepos.Save();
                    TempData[WebConstants.Success] = "Product Attribute updated successfully";
                }
                return RedirectToAction("Index");
            }
            TempData[WebConstants.Error] = "Error while creating or updating Product Attribute";
            productAttributeVM.AttributeTypeSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeType));
            productAttributeVM.AttributeValueSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeValue));
            productAttributeVM.ProductImages = new List<ProductImage>();
            if (_prodAttrRepos.Find(productAttributeVM.ProductAttribute.Id) != null)
            {
                productAttributeVM.AttributeTypeSelectList = productAttributeVM.AttributeTypeSelectList.Where(u => u.Value == productAttributeVM.ProductAttribute.Id.ToString());
                productAttributeVM.AttributeValueSelectList = productAttributeVM.AttributeValueSelectList.Where(u => u.Value == productAttributeVM.ProductAttribute.Id.ToString());
                productAttributeVM.ProductImages = _prodImgRepos.GetProductImages(productAttributeVM.ProductAttribute.ProductId);
            }
            return View(productAttributeVM);
        }

        //GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _prodAttrRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            ProductAttributeVM productAttributeVM = new ProductAttributeVM()
            {
                ProductAttribute = obj,
                AttributeTypeSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeType)),
                AttributeValueSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeValue))
            };

            return View(productAttributeVM);
        }

        //POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var obj = _prodAttrRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                TempData[WebConstants.Error] = "Error deleting Product Attribute";
                return NotFound();
            }

            IEnumerable<ProductImage> images = _prodImgRepos.GetProductAttributeImages(obj.Id);
            string upload = _webHostEnvironment.WebRootPath + WebConstants.ProductImagePath + $"{obj.ProductId}\\";
            foreach (var image in images)
            {
                if (System.IO.File.Exists(upload + image.Name))
                {
                    System.IO.File.Delete(upload + image.Name);
                }
            }
            if (Directory.GetFiles(upload).Length < 1)
            {
                Directory.Delete(upload);
            }

            _prodImgRepos.RemoveRange(images);
            _prodAttrRepos.Remove(obj);
            _prodAttrRepos.Save();
            TempData[WebConstants.Success] = "Product Attribute deleted successfully";
            return RedirectToAction("Index");
        }
    }
}

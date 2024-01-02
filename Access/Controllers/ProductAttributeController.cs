using Access_DataAccess.Repository.IRepository;
using Access_Models;
using Access_Models.ViewModels;
using Access_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Access.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class ProductAttributeController : Controller
    {
        private readonly IProductAttributeRepository _prodAttrRepos;
        private readonly IProductImageRepository _prodImgRepos;
        private readonly IProductRepository _prodRepos;
        private readonly IWebHostEnvironment _webHostEnvironment;
        [BindProperty]
        public ProductAttributeVM ProductAttributeVM { get; set; }

        public ProductAttributeController(IProductAttributeRepository prodAttrRepos, IProductImageRepository prodImgRepos, IWebHostEnvironment webHostEnvironment, IProductRepository prodRepos)
        {
            _prodAttrRepos = prodAttrRepos;
            _prodImgRepos = prodImgRepos;
            _webHostEnvironment = webHostEnvironment;
            _prodRepos = prodRepos;
        }

        public IActionResult Index()
        {
            ProductAttributeListVM productAttributeListVM = new ProductAttributeListVM()
            {
                ProductAttribute = _prodAttrRepos.GetAll(includeProperties: $"{nameof(AttributeType)},{nameof(AttributeValue)},{nameof(Product)}"),
            };
            List<ProductImage> images = new List<ProductImage>();
            foreach(ProductAttribute prodAttr in productAttributeListVM.ProductAttribute)
            {
                images.AddRange(_prodImgRepos.GetProductAttributeImages(prodAttr.Id));
            }
            productAttributeListVM.ProductImages = images;

            return View(productAttributeListVM);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductAttributeVM = new ProductAttributeVM()
            {
                ProductAttribute = new ProductAttribute(),
                AttributeTypeSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeType)),
                AttributeValueSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeValue)),
                ProductSelectList = _prodRepos.GetAllDropdownList(nameof(Product)),
                ProductImages = new List<ProductImage>()
            };

            if (id == null)
            {
                //for create
                return View(ProductAttributeVM);
            }
            else
            {
                ProductAttributeVM.ProductAttribute = _prodAttrRepos.Find(id.GetValueOrDefault());
                if (ProductAttributeVM.ProductAttribute == null)
                {
                    return NotFound();
                }
                ProductAttributeVM.ProductImages = _prodImgRepos.GetProductAttributeImages(ProductAttributeVM.ProductAttribute.Id);
                return View(ProductAttributeVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (ProductAttributeVM.ProductAttribute.Id == 0)
                {
                    //create
                    _prodAttrRepos.Add(ProductAttributeVM.ProductAttribute);
                    _prodAttrRepos.Save();

                    var files = HttpContext.Request.Form.Files;
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string upload = webRootPath + WebConstants.ProductImagePath + $"{ProductAttributeVM.ProductAttribute.ProductId}\\";
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
                            AttributeId = ProductAttributeVM.ProductAttribute.Id,
                            ProductId = ProductAttributeVM.ProductAttribute.ProductId,
                            ImageNumber = i
                        };
                        images.Add(productImage);
                        _prodImgRepos.Add(productImage);
                    }
                    ProductAttributeVM.ProductImages = images;
                    _prodImgRepos.Save();
                    TempData[WebConstants.Success] = "Pruduct Attribute created successfully";
                }
                else
                {
                    //update

                    var files = HttpContext.Request.Form.Files;
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string upload = webRootPath + WebConstants.ProductImagePath + $"{_prodAttrRepos.FirstOrDefault(u => u.Id == ProductAttributeVM.ProductAttribute.Id, isTracking: false).ProductId}\\";
                    string newUpload = webRootPath + WebConstants.ProductImagePath + $"{ProductAttributeVM.ProductAttribute.ProductId}\\";
                    List<ProductImage> images = new List<ProductImage>();
                    if (!Directory.Exists(upload))
                    {
                        Directory.CreateDirectory(upload);
                    }
                    if (Convert.ToBoolean(ProductAttributeVM.DeleteOldImages))
                    {
                        List<ProductImage> oldImages = _prodImgRepos.GetAll(u => u.AttributeId == ProductAttributeVM.ProductAttribute.Id).ToList();
                        foreach (var image in oldImages)
                        {
                            if (System.IO.File.Exists(upload + image.Name))
                            {
                                System.IO.File.Delete(upload + image.Name);
                            }
                        }
                        _prodImgRepos.RemoveRange(oldImages);
                    }else if(ProductAttributeVM.ProductAttribute.ProductId != _prodAttrRepos.FirstOrDefault(u => u.Id == ProductAttributeVM.ProductAttribute.Id, isTracking: false).ProductId)
                    {
                        List<ProductImage> oldImages = _prodImgRepos.GetAll(u => u.AttributeId == ProductAttributeVM.ProductAttribute.Id).ToList();
                        foreach (var image in oldImages)
                        {
                            if (System.IO.File.Exists(upload + image.Name))
                            {
                                System.IO.File.Move(upload + image.Name, newUpload + image.Name, true);
                                image.ProductId = ProductAttributeVM.ProductAttribute.ProductId;
                            }
                        }
                        _prodImgRepos.Save();
                    }

                    for (int i = 0; i < files.Count(); i++)
                    {

                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[i].FileName);
                        using (var fileStrem = new FileStream(Path.Combine(newUpload, fileName + extension), FileMode.Create))
                        {
                            files[i].CopyTo(fileStrem);
                        };
                        ProductImage productImage = new ProductImage()
                        {
                            Name = fileName + extension,
                            AttributeId = ProductAttributeVM.ProductAttribute.Id,
                            ProductId = ProductAttributeVM.ProductAttribute.ProductId,
                            ImageNumber = i
                        };
                        images.Add(productImage);
                        _prodImgRepos.Add(productImage);
                    }
                    _prodAttrRepos.Update(ProductAttributeVM.ProductAttribute);
                    _prodAttrRepos.Save();
                    ProductAttributeVM.ProductImages = images;
                    _prodImgRepos.Save();

                    TempData[WebConstants.Success] = "Product Attribute updated successfully";
                }
                return RedirectToAction("Index");
            }
            TempData[WebConstants.Error] = "Error while creating or updating Product Attribute";
            ProductAttributeVM.AttributeTypeSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeType));
            ProductAttributeVM.AttributeValueSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeValue));
            ProductAttributeVM.ProductImages = new List<ProductImage>();
            if (_prodAttrRepos.Find(ProductAttributeVM.ProductAttribute.Id) != null)
            {
                ProductAttributeVM.AttributeTypeSelectList = ProductAttributeVM.AttributeTypeSelectList.Where(u => u.Value == ProductAttributeVM.ProductAttribute.Id.ToString());
                ProductAttributeVM.AttributeValueSelectList = ProductAttributeVM.AttributeValueSelectList.Where(u => u.Value == ProductAttributeVM.ProductAttribute.Id.ToString());
                ProductAttributeVM.ProductImages = _prodImgRepos.GetProductImages(ProductAttributeVM.ProductAttribute.ProductId);
            }
            return View(ProductAttributeVM);
        }

        [HttpGet]
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

            ProductAttributeVM = new ProductAttributeVM()
            {
                ProductAttribute = obj,
                AttributeTypeSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeType)),
                AttributeValueSelectList = _prodAttrRepos.GetAllDropdownList(nameof(AttributeValue))
            };

            return View(ProductAttributeVM);
        }

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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.ViewModels;
using Repository.IRepositories;

namespace BasicCRUD.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // GET: ProductController
        // List all products
        public async Task<IActionResult> Index()
        {
                var products = await _productRepository.GetAllAsync();
                return View(products);
        }

        //GET: ProductController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return PartialView("_ProductDetails", product);
        }

        // GET: ProductController/Create
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            ProductViewModel productView = new ProductViewModel();

            if (id == 0)
            {
                productView = new ProductViewModel
                {
                    ManufactureDate = DateTime.Today,
                    ExpireDate = DateTime.Today.AddDays(180)
                };
            }
            else
            {
                var model = await _productRepository.GetByIdAsync(id);
                productView = new ProductViewModel
                {
                    ProductId = model.Id,
                    Name = model.Name,
                    SKU = model.SKU,
                    CategoryId = model.CategoryId,
                    BasePrice = model.BasePrice,
                    MRP = model.MRP,
                    Description = model.Description,
                    Currency = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), model.Currency, true),
                    ManufactureDate = model.ManufactureDate,
                    ExpireDate = model.ExpireDate,
                };
            }

            ViewData["Id"] = id;

            await SetViewBag();

            return View(productView);
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool skuExists = await _productRepository.CheckUniqueSKU(model.SKU);
                if (skuExists)
                {
                    ModelState.AddModelError("SKU", "The SKU must be unique.");
                }
                else
                {
                    var productModel = new Product
                    {
                        Name = model.Name,
                        SKU = model.SKU,
                        CategoryId = model.CategoryId,
                        BasePrice = model.BasePrice,
                        MRP = model.MRP,
                        Description = model.Description,
                        Currency = model.Currency.ToString(),
                        ManufactureDate = model.ManufactureDate,
                        ExpireDate = model.ExpireDate,
                    };

                    await _productRepository.AddAsync(productModel);
                }
                return RedirectToAction("Index");
            }

            await SetViewBag();

            return View("CreateOrEdit", model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = await _productRepository.GetByIdAsync(model.ProductId);
                if (product == null) return NotFound();

                product.Name = model.Name;
                product.SKU = model.SKU;
                product.BasePrice = model.BasePrice;
                product.MRP = model.MRP;
                product.Description = model.Description;
                product.Currency = model.Currency.ToString();
                product.CategoryId = model.CategoryId;
                product.ManufactureDate = model.ManufactureDate;
                product.ExpireDate = model.ExpireDate;

                await _productRepository.UpdateAsync(product);
                return RedirectToAction("Index");
            }

            await SetViewBag();

            return View("CreateOrEdit", model);
        }

        // DELETE: ProductController/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            await _productRepository.DeleteAsync(id);

            return Json(new { success = true, message = "Product deleted successfully." });
        }

        private async Task SetViewBag()
        {
            ViewBag.CurrencyList = new SelectList(
                Enum.GetValues(typeof(CurrencyEnum))
                    .Cast<CurrencyEnum>()
                    .Select(x => new { Value = (int)x, Text = x.ToString() }),
                "Value",
                "Text"
            );

            var categories = await _categoryRepository.GetAllAsync();

            ViewBag.CategoryList = new SelectList(
                categories.Select(x => new { Value = x.Id, Text = x.Name.ToString() }),
                "Value",
                "Text"
            );
        }
    }
}

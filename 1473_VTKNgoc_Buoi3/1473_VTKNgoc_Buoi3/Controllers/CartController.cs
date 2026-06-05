using _1473_VTKNgoc_Buoi3.Extensions;
using _1473_VTKNgoc_Buoi3.Models;
using _1473_VTKNgoc_Buoi3.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _1473_VTKNgoc_Buoi3.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private const string CartKey = "Cart";
        private readonly IProductRepository _productRepository;

        public CartController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity++;
            }

            HttpContext.Session.SetObject(CartKey, cart);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Increase(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
            {
                item.Quantity++;
            }

            HttpContext.Session.SetObject(CartKey, cart);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrease(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
            }

            HttpContext.Session.SetObject(CartKey, cart);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
            {
                cart.Remove(item);
            }

            HttpContext.Session.SetObject(CartKey, cart);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartKey);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Count()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var count = cart.Sum(x => x.Quantity);

            return Json(count);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BookSwap.Models;
using BookSwap.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System;
using System.Threading.Tasks;

public class RecentOrdersViewComponent : ViewComponent
{
    private readonly IOrderRepository _orderRepo;
    private readonly UserManager<ApplicationUser> _userManager;

    public RecentOrdersViewComponent(IOrderRepository orderRepo, UserManager<ApplicationUser> userManager)
    {
        _orderRepo = orderRepo;
        _userManager = userManager;
    }

    public IViewComponentResult Invoke(int count = 5)
    {
        var sellerId = _userManager.GetUserId(HttpContext.User);
        var orders = _orderRepo.getAllOrders()
                               .Where(o => o.Book.SellerId == sellerId)
                               .OrderByDescending(o => o.OrderId)
                               .Take(count)
                               .ToList();
        return View(orders);
    }
}

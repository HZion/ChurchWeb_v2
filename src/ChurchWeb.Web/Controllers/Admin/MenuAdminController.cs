using ChurchWeb.Core.Entities.Common;
using ChurchWeb.Infrastructure.Services;
using ChurchWeb.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchWeb.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Menu")]
public class MenuAdminController : Controller
{
    private readonly IMenuAdminService _menuService;
    private readonly ILogger<MenuAdminController> _logger;

    public MenuAdminController(
        IMenuAdminService menuService,
        ILogger<MenuAdminController> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var menuItems = await _menuService.GetAllMenuItemsAsync();

        var model = new MenuListViewModel
        {
            MenuItems = menuItems.Select(m => new MenuItemViewModel
            {
                Id = m.Id,
                Title = m.Title,
                Url = m.Url,
                ParentId = m.ParentId,
                SortOrder = m.SortOrder,
                IsVisible = m.IsVisible,
                IconClass = m.IconClass,
                OpenInNewTab = m.OpenInNewTab,
                ParentTitle = m.Parent?.Title,
                ChildCount = m.Children.Count
            }).ToList()
        };

        return View(model);
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var allMenuItems = await _menuService.GetAllMenuItemsAsync();
        ViewBag.ParentMenuItems = allMenuItems;

        return View("Form", new MenuItemViewModel());
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var menuItem = await _menuService.GetMenuItemByIdAsync(id);
        if (menuItem == null)
            return NotFound();

        var allMenuItems = await _menuService.GetAllMenuItemsAsync();
        ViewBag.ParentMenuItems = allMenuItems.Where(m => m.Id != id).ToList();

        var model = new MenuItemViewModel
        {
            Id = menuItem.Id,
            Title = menuItem.Title,
            Url = menuItem.Url,
            ParentId = menuItem.ParentId,
            SortOrder = menuItem.SortOrder,
            IsVisible = menuItem.IsVisible,
            IconClass = menuItem.IconClass,
            OpenInNewTab = menuItem.OpenInNewTab
        };

        return View("Form", model);
    }

    [HttpPost("Save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(MenuItemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var allMenuItems = await _menuService.GetAllMenuItemsAsync();
            ViewBag.ParentMenuItems = allMenuItems.Where(m => m.Id != model.Id).ToList();
            return View("Form", model);
        }

        try
        {
            if (model.Id == 0)
            {
                // 생성
                var menuItem = new NavMenuItem
                {
                    Title = model.Title,
                    Url = model.Url,
                    ParentId = model.ParentId,
                    SortOrder = model.SortOrder,
                    IsVisible = model.IsVisible,
                    IconClass = model.IconClass,
                    OpenInNewTab = model.OpenInNewTab
                };

                await _menuService.CreateMenuItemAsync(menuItem);
                TempData["SuccessMessage"] = "메뉴가 성공적으로 생성되었습니다.";
            }
            else
            {
                // 수정
                var menuItem = await _menuService.GetMenuItemByIdAsync(model.Id);
                if (menuItem == null)
                    return NotFound();

                menuItem.Title = model.Title;
                menuItem.Url = model.Url;
                menuItem.ParentId = model.ParentId;
                menuItem.SortOrder = model.SortOrder;
                menuItem.IsVisible = model.IsVisible;
                menuItem.IconClass = model.IconClass;
                menuItem.OpenInNewTab = model.OpenInNewTab;

                await _menuService.UpdateMenuItemAsync(menuItem);
                TempData["SuccessMessage"] = "메뉴가 성공적으로 수정되었습니다.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving menu item");
            ModelState.AddModelError(string.Empty, "저장 중 오류가 발생했습니다.");

            var allMenuItems = await _menuService.GetAllMenuItemsAsync();
            ViewBag.ParentMenuItems = allMenuItems.Where(m => m.Id != model.Id).ToList();
            return View("Form", model);
        }
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _menuService.DeleteMenuItemAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "메뉴가 삭제되었습니다." });
        }

        return Json(new { success = false, message = "하위 메뉴가 있거나 메뉴를 찾을 수 없습니다." });
    }

    [HttpPost("ToggleVisibility/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        var result = await _menuService.ToggleVisibilityAsync(id);
        if (result)
        {
            return Json(new { success = true, message = "표시 여부가 변경되었습니다." });
        }

        return Json(new { success = false, message = "메뉴를 찾을 수 없습니다." });
    }

    [HttpPost("UpdateSortOrders")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSortOrders([FromBody] Dictionary<int, int> sortOrders)
    {
        var result = await _menuService.UpdateSortOrdersBulkAsync(sortOrders);
        if (result)
        {
            return Json(new { success = true, message = "정렬 순서가 저장되었습니다." });
        }

        return Json(new { success = false, message = "정렬 순서 저장에 실패했습니다." });
    }
}

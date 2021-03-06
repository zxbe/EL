﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EL.Application;
using EL.Entity;

namespace EL.Admin.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        public AccountController(IAccountService accountService, IRoleService roleService)
        {
            _accountService = accountService;
            _roleService = roleService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAccount(int id)
        {
            var model = await _accountService.GetAccount(id);
            var obj = new
            {
                model.Id,
                model.Name,
                model.Account,
                RoleId = model.Role != null ? model.Role.Id : 0,
                model.Enabled,
                model.Sort
            };
            return Json(obj);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleList(AccountEntity entity)
        {
            var list = await _roleService.GetRoleList();
            var obj = list != null ? list.OrderBy(p => p.Name).Select(p => new
            {
                text = p.Name,
                value = p.Id,
            }) : null;
            return Json(obj);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(Account_DTO entity)
        {
            await _accountService.Submit(entity);
            return Json(new { code = 0 });
        }

        [HttpPost]
        public IActionResult GetAccountPageList(int pageIndex, int pageSize, string searchKey = null)
        {
            int total = 0;
            var list = _accountService.GetAccountPageList(pageIndex, pageSize, out total, searchKey);
            var obj = new
            {
                pageIndex,
                pageSize,
                total,
                data = list.Select(p => new
                {
                    p.Id,
                    p.Name,
                    RoleName = p.Role != null ? p.Role.Name : null,
                    p.Enabled,
                    p.Sort,
                    CreateTime = p.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList()
            };
            return Json(obj);
        }

        [HttpPost]
        public async Task<IActionResult> Deletes(int[] ids)
        {
            var status = await _accountService.Deletes(ids);
            return Json(new { code = status ? 0 : -2 });
        }

        [HttpPost]
        public async Task<IActionResult> Enableds(int[] ids)
        {
            await _accountService.Enableds(ids);
            return Json(new { code = 0 });
        }
    }
}
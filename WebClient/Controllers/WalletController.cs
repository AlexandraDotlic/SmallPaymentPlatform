using ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class WalletController : Controller
    {
        private readonly WalletService WalletService;

        public WalletController(WalletService walletService)
        {
            WalletService = walletService;
        }
        [HttpGet]
        public IActionResult CreateWallet()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateWallet(WalletVM walletVM)
        {
            string pass;
            try
            {
                pass = await WalletService.CreateWallet(walletVM.JMBG, walletVM.FirstName, walletVM.LastName, walletVM.BankType, walletVM.BankAccountNumber, walletVM.BankPIN);
                ModelState.Clear();
                return View(new WalletVM() { PASS = pass });
            }
            catch (Exception ex)
            {

                ViewData["ErrorMessage"] = ex.Message;

                return View("ErrorMessage");
            }
        }
    }
}

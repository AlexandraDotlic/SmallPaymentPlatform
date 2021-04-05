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
                ViewData["Success"] = "True";
                ViewData["WalletPASS"] = pass;
                return View();
            }
            catch (Exception ex)
            {

                ViewData["ErrorMessage"] = ex.Message;
                ViewData["Success"] = "False";
                return View();
            }
        }
        [HttpGet]
        public IActionResult Deposit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(WalletDepositVM walletDepositVM)
        {
            try
            {
                await WalletService.Deposit(walletDepositVM.JMBG, walletDepositVM.PASS, walletDepositVM.Amount);
                ModelState.Clear();
                ViewData["Success"] = "True";
                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                ViewData["Success"] = "False";
                return View();
            }
        }
    }
}

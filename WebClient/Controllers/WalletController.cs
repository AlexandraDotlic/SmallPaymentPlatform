using ApplicationServices;
using Core.ApplicationServices.DTOs;
using Core.Domain.Entities;
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
        [HttpGet]
        public IActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(WalletWithdrawVM walletWithdrawVM)
        {
            try
            {
                await WalletService.Withdraw(walletWithdrawVM.JMBG, walletWithdrawVM.PASS, walletWithdrawVM.Amount);
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

        [HttpGet]
        public IActionResult Transfer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(WalletTransferVM walletTransferVM)
        {
            try
            {
                await WalletService.Transfer(walletTransferVM.SourceJMBG, walletTransferVM.SourcePASS, walletTransferVM.Amount, walletTransferVM.DestinationJMBG);
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

        [HttpPost]
        public async Task<IActionResult> CalculateFee([FromBody] CalculateFeeVM calculateFeeVM)
        {
            try
            {
                decimal fee = await WalletService.CalculateTransferFee(calculateFeeVM.JMBG, calculateFeeVM.PASS, calculateFeeVM.Amount);
                return Ok(fee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ManageWallet()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ManageWallet(WalletManagementVM walletManagementVM)
        {
            try
            {
                if(walletManagementVM.Action == "Block")
                {
                    await WalletService.BlockWallet(walletManagementVM.WalletJMBG, walletManagementVM.AdminPASS);
                    ModelState.Clear();
                    ViewData["SuccessMessage"] = "Wallet successfully blocked.";
                    ViewData["Success"] = "True";
                    return View();

                }
                else
                {
                    await WalletService.UnblockWallet(walletManagementVM.WalletJMBG, walletManagementVM.AdminPASS);
                    ModelState.Clear();
                    ViewData["SuccessMessage"] = "Wallet successfully unblocked.";
                    ViewData["Success"] = "True";
                    return View();
                }
             
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                ViewData["Success"] = "False";
                return View();
            }
        }

        [HttpGet]
        public IActionResult ChangeWalletPass()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeWalletPass(ChangeWalletPassVM changeWalletPassVM)
        {
            try
            {
                await WalletService.ChangePass(changeWalletPassVM.JMBG, changeWalletPassVM.OldPASS, changeWalletPassVM.NewPASS);
                ModelState.Clear();
                ViewData["SuccessMessage"] = "Wallet password successfully changed.";
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

        [HttpGet]
        public IActionResult WalletInfo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> WalletInfo(WalletInfoRequestVM walletInfoRequestVM)
        {
            try
            {
                WalletInfoDTO walletInfoDTO = await WalletService.GetWalletInfo(walletInfoRequestVM.JMBG, walletInfoRequestVM.PASS);

                var walletInfoResponseVM = new WalletInfoResponseVM(
                    walletInfoDTO.JMBG,
                    walletInfoDTO.FirstName,
                    walletInfoDTO.LastName,
                    (short)walletInfoDTO.Bank,
                    walletInfoDTO.BankAccountNumber,
                    walletInfoDTO.Balance,
                    walletInfoDTO.IsBlocked,
                    walletInfoDTO.WalletCreationTime,
                    walletInfoDTO.MaxDeposit,
                    walletInfoDTO.UsedDeposit,
                    walletInfoDTO.MaxWithdraw,
                    walletInfoDTO.UsedWithdraw);
                ModelState.Clear();
                var walletInfoVM = new WalletInfoVM(walletInfoRequestVM, walletInfoResponseVM);
                ViewData["Success"] = "True";
                return View(walletInfoVM);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                ViewData["Success"] = "False";
                return View();
            }
        }

        [HttpGet]
        public IActionResult WalletTransactions()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> WalletTransactions(WalletTransactionsRequestVM walletTransactionsRequestVM)
        {
            try
            {
                WalletTransactionsDTO walletTransactionsDTO = await WalletService.GetWalletTransactionsByDate(walletTransactionsRequestVM.JMBG, walletTransactionsRequestVM.PASS, walletTransactionsRequestVM.Date);
                var transactionsVM = walletTransactionsDTO.Transactions.Select(t => new TransactionVM(t.Id, t.Amount, Enum.GetName(typeof(TransactionType), t.Type))).ToList();
                var walletTransactionsResponseVM = new WalletTransactionsResponseVM(
                    transactionsVM
                    );
                ModelState.Clear();
                var walletTransactionsVM = new WalletTransactionsVM(walletTransactionsRequestVM, walletTransactionsResponseVM);
                ViewData["Success"] = "True";
                return View(walletTransactionsVM);
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Services.External.BankService
{
    public class BankResponseDTO
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }

        public BankResponseDTO(bool isSuccess, string errorMsg = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMsg;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Shared
{
    public enum Financial_transactionsEnum : byte
    {
        In = 1,
         
        Out=2

    }
    public static class Financial_transactionsEnumExtensions
    {
        public static string GetTitle(this Financial_transactionsEnum inputData)
        {
            switch (inputData)
            {
                case Financial_transactionsEnum.In:
                    return "ورودی";
                case Financial_transactionsEnum.Out:
                    return "خروجی";


                default:
                    return "خطا";
            }
        }
    }
    public enum PaymentStatusEnum
    {
        Cash=1,
        Cheque=2,
        Request= 3,
        Returning=4
    }
    public static class PaymentStatusEnumExtensions
    {
        public static string GetTitle(this PaymentStatusEnum inputData)
        {
            switch (inputData)
            {
                case PaymentStatusEnum.Cash:
                    return "نقد";
                case PaymentStatusEnum.Cheque:
                    return "چک";
                case PaymentStatusEnum.Request:
                    return "واخواست";
                case PaymentStatusEnum.Returning:
                    return "عودت";
  
                default:
                    return "خطا";
            }
        }
    }
    public enum OperationCompletedEnum
    {
        yes=1,
        No=2
    }

}

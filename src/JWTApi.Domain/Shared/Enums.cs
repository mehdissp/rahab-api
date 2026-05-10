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
        CashAndCheque= 3,
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
                case PaymentStatusEnum.CashAndCheque:
                    return "نقدوچک";
         
  
                default:
                    return "خطا";
            }
        }
    }
    public enum OperationCompletedEnum
    {
        Waiting=0,
        yes=1,
        No=2
    }
    public static class OperationCompletedEnumExtensions
    {
        public static string GetTitle(this OperationCompletedEnum inputData)
        {
            switch (inputData)
            {
                case OperationCompletedEnum.yes:
                    return "بلی";
                case OperationCompletedEnum.No:
                    return "خیر";
                case OperationCompletedEnum.Waiting:
                    return "درانتظار";


                default:
                    return "خطا";
            }
        }
    }


    public enum PaymentChequeStatusEnum
    {
        WATING=0,
        YES = 1,
        NO = 2,
        Returning = 3
    }
    public static class PaymentChequeStatusEnumExtensions
    {
        public static string GetTitle(this PaymentChequeStatusEnum inputData)
        {
            switch (inputData)
            {
                case PaymentChequeStatusEnum.WATING:
                    return "در انتظار";
                case PaymentChequeStatusEnum.YES:
                    return "بلی";
                case PaymentChequeStatusEnum.NO:
                    return "خیر";
                case PaymentChequeStatusEnum.Returning:
                    return "برگشت خورد";


                default:
                    return "خطا";
            }
        }
    }

}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Payments
{
    public interface IPaymentService
    {
        PaymentPostData GeneratePostDataFor(OrderPaymentRequest orderRequest);
        Task<TransactionResult> HandleCallBack(OrderPaymentRequest orderRequest, IFormCollection collection);
        int GetOrderIdFor(IFormCollection collection);
    }
}
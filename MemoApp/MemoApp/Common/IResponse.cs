using System.Collections.Generic;
using System.Net;

namespace MemoApp.Common
{
    public interface IResponse<T>
    {
        HttpStatusCode StatusCode { get; set; }
        T Content { get; set; }
        List<ErrorResult> ErrorMessage { get; set; }
    }
}

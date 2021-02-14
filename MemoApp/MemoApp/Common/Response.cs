using System.Collections.Generic;
using System.Net;

namespace MemoApp.Common
{
    public class Response<T> : IResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T Content { get; set; }
        public List<ErrorResult> ErrorMessage { get; set; }
    }
}

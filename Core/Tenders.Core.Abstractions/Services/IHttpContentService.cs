using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Tenders.Core.Abstractions.Services
{
    public interface IHttpContentService
    {
        FormUrlEncodedContent GetFormUrlEncoded(params string[] keyValues);
        StringContent GetJsonContent(object data);
    }
}

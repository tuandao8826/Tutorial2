using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Facades.Common.HttpClients
{
    public class HttpResultProfile : Profile
    {
        public HttpResultProfile()
        {
            CreateMap<HttpResponseMessage, HttpResult>();
        }
    }
}

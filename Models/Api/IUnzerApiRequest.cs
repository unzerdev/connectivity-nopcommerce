using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public interface IUnzerApiRequest
    {
        public string BaseUrl { get; }

        public string Path { get; }

        public string Method { get; }
    }
}

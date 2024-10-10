using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutorial.Infrastructure.Services.Interfaces;

namespace Tutorial.Infrastructure.Services
{
    public class CaptiveDependencyService : ISingletonService
    {
        Guid _id;

        private readonly ITransientService _transientService;

        public CaptiveDependencyService(ITransientService transientService)
        {
            this._transientService = transientService;
        }
              
        public Guid GetID()
        {
            return _transientService.GetID();
        }
    }
}

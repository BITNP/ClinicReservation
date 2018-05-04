using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ClinicReservation.Services.Database;

namespace ClinicReservation.Services
{
    public interface IServiceState
    {
        bool AllowCreate { get; }

        bool RefreshState();
    }

    internal sealed class ServiceState : IServiceState
    {
        private readonly object locker;
        private readonly IHttpContextAccessor accessor;
        private bool load;
        private bool state;
        public bool AllowCreate
        {
            get
            {
                lock (locker)
                {
                    if (load)
                        return RefreshState();
                    return state;
                }
            }
        }

        public ServiceState(IHttpContextAccessor accessor)
        {
            load = true;
            locker = new object();
            this.accessor = accessor;
        }
        public bool RefreshState()
        {
            lock (locker)
            {
                IDbQuery dbquery = accessor.HttpContext.RequestServices.GetService<IDbQuery>();
                state = dbquery.RetriveLastServerStateChangedRecord().IsServiceEnabled;
                load = false;
                return state;
            }
        }
    }
}

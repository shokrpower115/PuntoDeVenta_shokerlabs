using POS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Services
{
    public interface IMetodoPagoService
    {
        Task<List<MetodoPago>> ObtenerActivosAsync();
    }
}

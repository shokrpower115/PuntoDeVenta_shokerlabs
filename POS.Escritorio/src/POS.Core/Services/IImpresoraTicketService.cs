using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.Services
{
    public interface IImpresoraTicketService
    {
        Task ImprimirAsync(TicketVenta ticket, DatosNegocio negocio);
        Task ImprimirCorteAsync(TicketCorte ticket, DatosNegocio negocio); // nuevo
    }
}
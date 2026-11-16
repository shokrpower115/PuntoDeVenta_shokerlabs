using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using POS.Core.Models;
using POS.Core.Services;
using POS.UI.Apariencia;

namespace POS.UI.ViewModels
{
    // Datos del negocio (nombre, ticket, color y logo). Se usa en dos lugares:
    // el asistente de primer arranque y la sección Configuración del Administrador.
    public class ConfiguracionNegocioViewModel : ViewModelBase
    {
        private const int TamanoMaximoLogoBytes = 2 * 1024 * 1024;

        private readonly IConfiguracionNegocioService _configuracionService;
        private readonly DatosNegocio _datosCompartidos;

        public bool EsPrimerArranque { get; }
        public string[] ColoresSugeridos => Marca.ColoresSugeridos;

        public event Action? Guardado;
        public event Action? VolverSolicitado;

        private string _nombre;
        public string Nombre { get => _nombre; set => SetProperty(ref _nombre, value); }

        private string _direccion;
        public string Direccion { get => _direccion; set => SetProperty(ref _direccion, value); }

        private string _telefono;
        public string Telefono { get => _telefono; set => SetProperty(ref _telefono, value); }

        private string _rfc;
        public string Rfc { get => _rfc; set => SetProperty(ref _rfc, value); }

        private string _sitioWeb;
        public string SitioWeb { get => _sitioWeb; set => SetProperty(ref _sitioWeb, value); }

        private string _mensajeTicket;
        public string MensajeTicket { get => _mensajeTicket; set => SetProperty(ref _mensajeTicket, value); }

        private string _colorPrimario;
        public string ColorPrimario { get => _colorPrimario; set => SetProperty(ref _colorPrimario, value); }

        private byte[]? _logo;
        public byte[]? Logo
        {
            get => _logo;
            set
            {
                if (SetProperty(ref _logo, value))
                    OnPropertyChanged(nameof(TieneLogo));
            }
        }

        public bool TieneLogo => Logo is { Length: > 0 };

        private string _mensajeError = string.Empty;
        public string MensajeError { get => _mensajeError; set => SetProperty(ref _mensajeError, value); }

        private string _mensajeExito = string.Empty;
        public string MensajeExito { get => _mensajeExito; set => SetProperty(ref _mensajeExito, value); }

        public RelayCommand GuardarCommand { get; }
        public RelayCommand SeleccionarLogoCommand { get; }
        public RelayCommand QuitarLogoCommand { get; }
        public RelayCommand<string> ElegirColorCommand { get; }
        public RelayCommand VolverCommand { get; }

        // datosCompartidos es la instancia que usa toda la aplicación (tickets, pantalla
        // principal): al guardar se actualiza para que los cambios se vean de inmediato.
        public ConfiguracionNegocioViewModel(IConfiguracionNegocioService configuracionService,
            DatosNegocio datosCompartidos, bool esPrimerArranque)
        {
            _configuracionService = configuracionService;
            _datosCompartidos = datosCompartidos;
            EsPrimerArranque = esPrimerArranque;

            _nombre = datosCompartidos.Nombre;
            _direccion = datosCompartidos.Direccion;
            _telefono = datosCompartidos.Telefono;
            _rfc = datosCompartidos.Rfc;
            _sitioWeb = datosCompartidos.SitioWeb;
            _mensajeTicket = datosCompartidos.MensajeTicket;
            _colorPrimario = datosCompartidos.ColorPrimario;
            _logo = datosCompartidos.Logo;

            GuardarCommand = new RelayCommand(async () => await GuardarAsync());
            SeleccionarLogoCommand = new RelayCommand(SeleccionarLogo);
            QuitarLogoCommand = new RelayCommand(() => Logo = null);
            ElegirColorCommand = new RelayCommand<string>(color => ColorPrimario = color);
            VolverCommand = new RelayCommand(() => VolverSolicitado?.Invoke());
        }

        private void SeleccionarLogo()
        {
            var dialogo = new OpenFileDialog
            {
                Title = "Selecciona el logo del negocio",
                Filter = "Imágenes (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };
            if (dialogo.ShowDialog() != true) return;

            var info = new FileInfo(dialogo.FileName);
            if (info.Length > TamanoMaximoLogoBytes)
            {
                MensajeError = "El logo no debe pesar más de 2 MB.";
                return;
            }

            var bytes = File.ReadAllBytes(dialogo.FileName);
            if (Marca.CrearImagen(bytes) == null)
            {
                MensajeError = "No se pudo leer la imagen. Usa un archivo PNG o JPG.";
                return;
            }

            MensajeError = string.Empty;
            Logo = bytes;
        }

        private async Task GuardarAsync()
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                MensajeError = "Escribe el nombre del negocio; aparece en la pantalla y en los tickets.";
                return;
            }
            if (Marca.ParsearColor(ColorPrimario) == null)
            {
                MensajeError = "El color debe tener el formato #RRGGBB (por ejemplo #2563EB).";
                return;
            }

            var datos = new DatosNegocio
            {
                Nombre = Nombre.Trim(),
                Direccion = Direccion.Trim(),
                Telefono = Telefono.Trim(),
                Rfc = Rfc.Trim().ToUpperInvariant(),
                SitioWeb = SitioWeb.Trim(),
                MensajeTicket = string.IsNullOrWhiteSpace(MensajeTicket) ? DatosNegocio.MensajeTicketPorDefecto : MensajeTicket.Trim(),
                ColorPrimario = ColorPrimario.Trim().ToUpperInvariant(),
                Logo = Logo
            };

            await _configuracionService.GuardarAsync(datos);

            _datosCompartidos.CopiarDe(datos);
            Marca.Aplicar(_datosCompartidos);

            MensajeExito = "Configuración guardada.";
            Guardado?.Invoke();
        }
    }
}

using System;

namespace HeroServer
{
    public class Negocio
    {
        public int Id { get; set; }
        public int PerfilEconomicoId { get; set; }
        public String NombreComercial { get; set; }
        public String PrincipalActividadEconomica { get; set; }
        public DateTime? FechaInscripcionNegocio { get; set; }
        public int? NumeroRegistro { get; set; }
        public int? Folio { get; set; }
        public int? Libro { get; set; }
        public String DireccionNegocio { get; set; }
        public int PaisNegocioId { get; set; }
        public int? DepartamentoNegocioId { get; set; }
        public int? MunicipioNegocioId { get; set; }
        public int TipoMonedaId { get; set; }
        public double MontoAproximado { get; set; }


        public Negocio(int id, int perfilEconomicoId, String nombreComercial, String principalActividadEconomica, DateTime? fechaInscripcionNegocio,
                       int? numeroRegistro, int? folio, int? libro, String direccionNegocio, int paisNegocioId, int? departamentoNegocioId,
                       int? municipioNegocioId, int tipoMonedaId, double montoAproximado)
        {
            Id = id;
            PerfilEconomicoId = perfilEconomicoId;
            NombreComercial = nombreComercial;
            PrincipalActividadEconomica = principalActividadEconomica;
            FechaInscripcionNegocio = fechaInscripcionNegocio;
            NumeroRegistro = numeroRegistro;
            Folio = folio;
            Libro = libro;
            DireccionNegocio = direccionNegocio;
            PaisNegocioId = paisNegocioId;
            DepartamentoNegocioId = departamentoNegocioId;
            MunicipioNegocioId = municipioNegocioId;
            TipoMonedaId = tipoMonedaId;
            MontoAproximado = montoAproximado;
        }
    }
}
using System;

namespace HeroServer
{
    public class Dependencia
    {
        public int Id { get; set; }
        public int PerfilEconomicoId { get; set; }
        public int SectorId { get; set; }
        public String NombreEmpleador { get; set; }
        public String PrincipalActividadEconomicaEmpleador { get; set; }
        public String PuestoDesempenia { get; set; }
        public String DireccionEmpleador { get; set; }
        public int PaisEmpleadorId { get; set; }
        public int? DepartamentoEmpleadorId { get; set; }
        public int? MunicipioEmpleadorId { get; set; }
        public int TipoMonedaId { get; set; }
        public double MontoAproximado { get; set; }

        public Dependencia(int id, int perfilEconomicoId, int sectorId, String nombreEmpleador,String principalActividadEconomicaEmpleador,
                           String puestoDesempenia, String direccionEmpleador, int paisEmpleadorId, int? departamentoEmpleadorId,
                           int? municipioEmpleadorId, int tipoMonedaId, double montoAproximado)
        {
            Id = id;
            PerfilEconomicoId = perfilEconomicoId;
            SectorId = sectorId;
            NombreEmpleador = nombreEmpleador;
            PrincipalActividadEconomicaEmpleador = principalActividadEconomicaEmpleador;
            PuestoDesempenia = puestoDesempenia;
            DireccionEmpleador = direccionEmpleador;
            PaisEmpleadorId = paisEmpleadorId;
            DepartamentoEmpleadorId = departamentoEmpleadorId;
            MunicipioEmpleadorId = municipioEmpleadorId;
            TipoMonedaId = tipoMonedaId;
            MontoAproximado = montoAproximado;
        }
    }
}
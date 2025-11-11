using System;

namespace HeroServer
{
    public class OtroIngreso
    {
        public int Id { get; set; }
        public int PerfilEconomicoId { get; set; }
        public int TipoIngresoOtroId { get; set; }
        public String DetalleOtrosIngresos { get; set; }
        public int TipoMonedaId { get; set; }
        public double MontoAproximado { get; set; }


        public OtroIngreso(int id, int perfilEconomicoId, int tipoIngresoOtroId, String detalleOtrosIngresos, int tipoMonedaId, double montoAproximado)
        {
            Id = id;
            PerfilEconomicoId = perfilEconomicoId;
            TipoIngresoOtroId = tipoIngresoOtroId;
            DetalleOtrosIngresos = detalleOtrosIngresos;
            TipoMonedaId = tipoMonedaId;
            MontoAproximado = montoAproximado;
        }
    }
}
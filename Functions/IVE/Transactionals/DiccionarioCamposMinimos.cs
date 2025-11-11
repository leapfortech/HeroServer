using System;
using System.Text.Json.Serialization;

namespace HeroServer
{
	public class DiccionarioCamposMinimos
	{
        public String TipoActuacion { get; set; }
        public String CalidadActua { get; set; }
        public DiccionarioLugar Lugar { get; set; }
        public String Fecha { get; set; }
        public DiccionarioDatosPersonales Cliente { get; set; }
        public DiccionarioDatosPersonales Representante { get; set; }
        public DiccionarioInformacionEconomicaInicial InfoEconomica { get; set; }

		public DiccionarioCamposMinimos()
		{
		}


        public DiccionarioCamposMinimos(String tipoActuacion, String calidadActua, DiccionarioLugar lugar, String fecha, DiccionarioDatosPersonales cliente,
										DiccionarioDatosPersonales representante, DiccionarioInformacionEconomicaInicial infoEconomica)
		{
			TipoActuacion = tipoActuacion;
			CalidadActua = calidadActua;
			Lugar = lugar;
			Fecha = fecha;
			Cliente = cliente;
			Representante = representante;
			InfoEconomica = infoEconomica;
		}
	}
}

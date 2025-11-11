using System;
using System.Text.Json.Serialization;

namespace HeroServer
{
	public class DiccionarioCamposMinimosFirmante
	{
        public String TipoActuacion { get; set; }
        public String CalidadActua { get; set; }
        public DiccionarioLugar Lugar { get; set; }
        public String Fecha { get; set; }
        public DiccionarioDatosPersonales Firmante { get; set; }
        public DiccionarioDatosPersonales Representante { get; set; }


		public DiccionarioCamposMinimosFirmante(String tipoActuacion, String calidadActua, DiccionarioLugar lugar, String fecha, DiccionarioDatosPersonales firmante,
												DiccionarioDatosPersonales representante)
		{
			TipoActuacion = tipoActuacion;
			CalidadActua = calidadActua;
			Lugar = lugar;
			Fecha = fecha;
			Firmante = firmante;
			Representante = representante;
		}
	}
}

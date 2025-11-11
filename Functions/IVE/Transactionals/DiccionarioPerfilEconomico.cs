using System;

namespace HeroServer
{
	public class DiccionarioPerfilEconomico
	{
		public String Actualizacion { get; set; }
		public String Fecha { get; set; }
		public DiccionarioPerfilEconomicoNegocioPropio[] NegocioPropio { get; set; }
		public DiccionarioPerfilEconomicoRelacionDependencia[] RelacionDependencia { get; set; }
		public DiccionarioPerfilEconomicoOtrosIngresos[] OtrosIngresos { get; set; }



		public DiccionarioPerfilEconomico(String actualizacion, String fecha,
										  DiccionarioPerfilEconomicoNegocioPropio[] negocioPropio,
										  DiccionarioPerfilEconomicoRelacionDependencia[] relacionDependencia,
										  DiccionarioPerfilEconomicoOtrosIngresos[] otrosIngresos)
		{
			Actualizacion = actualizacion;
			Fecha = fecha;
			NegocioPropio = negocioPropio;
			RelacionDependencia = relacionDependencia;
			OtrosIngresos = otrosIngresos;
		}
	}
}

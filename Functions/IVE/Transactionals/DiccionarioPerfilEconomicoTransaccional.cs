using System;

namespace HeroServer
{
	public class DiccionarioPerfilEconomicoTransaccional
	{
		public String Actualizacion { get; set; }
		public String Fecha { get; set; }
		public DiccionarioPerfilEconomicoNegocioPropio[] NegocioPropio { get; set; }
		public DiccionarioPerfilEconomicoRelacionDependencia[] RelacionDependencia { get; set; }
		public DiccionarioPerfilEconomicoOtrosIngresos[] OtrosIngresos { get; set; }
		public DiccionarioPerfilTransaccional[] PerfilTransaccional { get; set; }


		public DiccionarioPerfilEconomicoTransaccional(String actualizacion, String fecha, DiccionarioPerfilEconomicoNegocioPropio[] negocioPropio,
													   DiccionarioPerfilEconomicoRelacionDependencia[] relacionDependencia,
													   DiccionarioPerfilEconomicoOtrosIngresos[] otrosIngresos,
													   DiccionarioPerfilTransaccional[] perfilTransaccional)
		{
			Actualizacion = actualizacion;
			Fecha = fecha;
			NegocioPropio = negocioPropio;
			RelacionDependencia = relacionDependencia;
			OtrosIngresos = otrosIngresos;
			PerfilTransaccional = perfilTransaccional;
		}
	}
}

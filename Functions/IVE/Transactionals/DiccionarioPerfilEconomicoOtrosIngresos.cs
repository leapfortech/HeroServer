using System;

namespace HeroServer
{
	public class DiccionarioPerfilEconomicoOtrosIngresos
	{
		public int TipoOtrosIngresos { get; set; }
		public String DetalleOtrosIngresos { get; set; }
		public DiccionarioIngresos Ingresos { get; set; }



		public DiccionarioPerfilEconomicoOtrosIngresos(int tipoOtrosIngresos, String detalleOtrosIngresos, DiccionarioIngresos ingresos)
		{
			TipoOtrosIngresos = tipoOtrosIngresos;
			DetalleOtrosIngresos = detalleOtrosIngresos;
			Ingresos = ingresos;
		}

		public DiccionarioPerfilEconomicoOtrosIngresos(OtroIngreso otroIngreso, DiccionarioIngresos ingresos)
		{
			TipoOtrosIngresos = otroIngreso.TipoIngresoOtroId;
			DetalleOtrosIngresos = otroIngreso.DetalleOtrosIngresos;
			Ingresos = ingresos;
		}
	}
}

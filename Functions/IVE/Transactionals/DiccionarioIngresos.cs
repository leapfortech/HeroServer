using System;

namespace HeroServer
{
	public class DiccionarioIngresos
	{
		public String TipoMoneda { get; set; }
		public double MontoAproximado { get; set; }


		public DiccionarioIngresos(String tipoMoneda, double montoAproximado)
		{
			TipoMoneda = tipoMoneda;
			MontoAproximado = montoAproximado;
		}
	}
}

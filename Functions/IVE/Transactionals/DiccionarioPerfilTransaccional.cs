using System;

namespace HeroServer
{
	public class DiccionarioPerfilTransaccional
	{
		public String Fecha { get; set; }
		public String ProductoServicio { get; set; }
		public String TipoMoneda { get; set; }
		public double MontoPromedioMensual { get; set; }
		public DiccionarioLugar[] PrincipalesUbicacionesGeograficas { get; set; }


		public DiccionarioPerfilTransaccional(String fecha, String productoServicio, String tipoMoneda, double montoPromedioMensual,
			                                  DiccionarioLugar[] principalesUbicacionesGeograficas)
		{
			Fecha = fecha;
			ProductoServicio = productoServicio;
			TipoMoneda = tipoMoneda;
			MontoPromedioMensual = montoPromedioMensual;
			PrincipalesUbicacionesGeograficas = principalesUbicacionesGeograficas;
		}
	}
}

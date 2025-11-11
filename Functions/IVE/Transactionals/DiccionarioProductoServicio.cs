using System;

namespace HeroServer
{
	public class DiccionarioProductoServicio
	{
		public DiccionarioLugar Lugar { get; set; }
		public String Fecha { get; set; }
		public String Tipo { get; set; }
		public String Nombre { get; set; }
		public String Descripcion { get; set; }
		public String Identificador { get; set; }
		public String NombreContrata { get; set; }
		public String Moneda { get; set; }
		public double Valor { get; set; }
		public DiccionarioCamposMinimosFirmante[] OtrosFirmantes { get; set; }
		public DiccionarioCamposMinimos[] Beneficiarios { get; set; }


		public DiccionarioProductoServicio(DiccionarioLugar lugar, String fecha, String tipo, String nombre, String descripcion, String identificador,
										   String nombreContrata, String moneda, double valor,
										   DiccionarioCamposMinimosFirmante[] otrosFirmantes,
										   DiccionarioCamposMinimos[] beneficiarios)
		{
			Lugar = lugar;
			Fecha = fecha;
			Tipo = tipo;
			Nombre = nombre;
			Descripcion = descripcion;
			Identificador = identificador;
			NombreContrata = nombreContrata;
			Moneda = moneda;
			Valor = valor;
			OtrosFirmantes = otrosFirmantes;
			Beneficiarios = beneficiarios;
		}
	}
}

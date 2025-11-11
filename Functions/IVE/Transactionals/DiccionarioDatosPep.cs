using System;
using System.Text.Json.Serialization;

namespace HeroServer
{
	public class DiccionarioDatosPep
	{
        public String Entidad { get; set; }
        public String PuestoDesempenia { get; set; }
        public String PaisEntidad { get; set; }
        public int OrigenRiqueza { get; set; }
        public String OtroOrigenRiqueza { get; set; }

		public DiccionarioDatosPep(String entidad, String puestoDesempenia, String paisEntidad, int origenRiqueza, String otroOrigenRiqueza)
		{
			Entidad = entidad;
			PuestoDesempenia = puestoDesempenia;
			PaisEntidad = paisEntidad;
			OrigenRiqueza = origenRiqueza;
			OtroOrigenRiqueza = otroOrigenRiqueza;
		}
	}
}

using System;

namespace HeroServer
{
	public class DiccionarioParienteAsociadoPep
	{
		public String Parentesco { get; set; }
		public String OtroParentesco { get; set; }
		public String MotivoAsociacion { get; set; }
		public String OtroMotivoAsociacion { get; set; }
		public String PrimerApellido { get; set; }
		public String SegundoApellido { get; set; }
		public String ApellidoCasada { get; set; }
		public String PrimerNombre { get; set; }
		public String SegundoNombre { get; set; }
		public String OtrosNombres { get; set; }
		public String Sexo { get; set; }
		public String Condicion { get; set; }
		public String Entidad { get; set; }
		public String PuestoDesempenia { get; set; }
		public String PaisEntidadId { get; set; }


		public DiccionarioParienteAsociadoPep(String parentesco, String otroParentesco, String motivoAsociacion, String otroMotivoAsociacion, String primerApellido,
											  String segundoApellido, String apellidoCasada, String primerNombre, String segundoNombre, String otrosNombres,
											  String sexo, String condicion, String entidad, String puestoDesempenia, String paisEntidadId)
		{
			Parentesco = parentesco;
			OtroParentesco = otroParentesco;
			MotivoAsociacion = motivoAsociacion;
			OtroMotivoAsociacion = otroMotivoAsociacion;
			PrimerApellido = primerApellido;
			SegundoApellido = segundoApellido;
			ApellidoCasada = apellidoCasada;
			PrimerNombre = primerNombre;
			SegundoNombre = segundoNombre;
			OtrosNombres = otrosNombres;
			Sexo = sexo;
			Condicion = condicion;
			Entidad = entidad;
			PuestoDesempenia = puestoDesempenia;
			PaisEntidadId = paisEntidadId;
		}
	}
}
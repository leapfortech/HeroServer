using System;

namespace HeroServer
{
	public class DiccionarioDatosPersonales
	{
		public String PrimerApellido { get; set; }
		public String SegundoApellido { get; set; }
		public String ApellidoCasada { get; set; }
		public String PrimerNombre { get; set; }
		public String SegundoNombre { get; set; }
		public String OtrosNombres { get; set; }
		public String FechaNacimiento { get; set; }
		public String[] Nacionalidades { get; set; }
		public DiccionarioLugar Nacimiento { get; set; }
		public int? CondicionMigratoria { get; set; }
		public String OtraCondicionMigratoria { get; set; }
		public String Sexo { get; set; }
		public String EstadoCivil { get; set; }
		public String ProfesionOficio { get; set; }
		public String TipoDocumentoIdentificacion { get; set; }
		public String NumeroDocumentoIdentificacion { get; set; }
		public String EmisionPasaporte { get; set; }
		public String Nit { get; set; }
		public long[] Telefonos { get; set; }
		public String Email { get; set; }
		public String DireccionResidencia { get; set; }
		public DiccionarioLugar Residencia { get; set; }
		public String Pep { get; set; }
		public DiccionarioDatosPep DatosPep { get; set; }
		public String ParienteAsociadoPep { get; set; }
		public DiccionarioParienteAsociadoPep[] DatosParienteAsociadoPep { get; set; }
		public String Cpe { get; set; }


        public DiccionarioDatosPersonales()
		{
		}

        public DiccionarioDatosPersonales(String primerApellido, String segundoApellido, String apellidoCasada, String primerNombre, String segundoNombre,
                                          String otrosNombres, String fechaNacimiento, String[] nacionalidades,	DiccionarioLugar nacimiento,
										  int? condicionMigratoria, String otraCondicionMigratoria, String sexo, String estadoCivil, String profesionOficio,
                                          String tipoDocumentoIdentificacion, String numeroDocumentoIdentificacion, String emisionPasaporte, String nit,
										  long[] telefonos, String email, String direccionResidencia, DiccionarioLugar residencia, String pep,
										  DiccionarioDatosPep datosPep, String parienteAsociadoPep,DiccionarioParienteAsociadoPep[] datosParienteAsociadoPep,
                                          String cpe)
		{

			PrimerApellido = primerApellido;
			SegundoApellido = segundoApellido;
			ApellidoCasada = apellidoCasada;
			PrimerNombre = primerNombre;
			SegundoNombre = segundoNombre;
			OtrosNombres = otrosNombres;
			FechaNacimiento = fechaNacimiento;
			Nacionalidades = nacionalidades;
			Nacimiento = nacimiento;
			CondicionMigratoria = condicionMigratoria;
			OtraCondicionMigratoria = otraCondicionMigratoria;
			Sexo = sexo;
			EstadoCivil = estadoCivil;
			ProfesionOficio = profesionOficio;
			TipoDocumentoIdentificacion = tipoDocumentoIdentificacion;
			NumeroDocumentoIdentificacion = numeroDocumentoIdentificacion;
			if (emisionPasaporte!=null)
				EmisionPasaporte = emisionPasaporte;
			Nit = nit;
			Telefonos = telefonos;
			Email = email;
			DireccionResidencia = direccionResidencia;
			Residencia = residencia;
			Pep = pep;
			if (datosPep != null)
				DatosPep = datosPep;
			ParienteAsociadoPep = parienteAsociadoPep;
			if (datosParienteAsociadoPep != null)
				DatosParienteAsociadoPep = datosParienteAsociadoPep;
			Cpe = cpe;
		}
	}
}
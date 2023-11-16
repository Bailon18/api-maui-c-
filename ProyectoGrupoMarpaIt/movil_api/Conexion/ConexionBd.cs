namespace movil_api.Conexion
{
    public class ConexionBd
    {
        private string conecctionString = string.Empty;

        public ConexionBd() {

            var constructor = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            conecctionString = constructor.GetSection("ConnectionStrings:conexionmaestro").Value;
        }

        public string cadenaSQL()
        {
            return conecctionString;
        }
    }
}

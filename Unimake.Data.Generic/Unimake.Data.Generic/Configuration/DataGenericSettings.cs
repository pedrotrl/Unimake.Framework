using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Unimake.Data.Generic.Configuration
{
    /// <summary>
    /// Classe de configuração do app.config
    /// </summary>
    public class DataGenericSettings : ConfigurationSection
    {
        private static DataGenericSettings settings = ConfigurationManager.GetSection("DataGenericSettings") as DataGenericSettings;

        /// <summary>
        /// Se true, as dlls do SQLite já foram carregadas.
        /// </summary>
        internal static bool Initialized = false;

        /// <summary>
        /// Ponto de entrada do componente Unimake.Data.Generic
        /// </summary>
        static DataGenericSettings()
        {
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Initialize();
        }

        internal static void Initialize()
        {
            if (!Initialized)
            {
                string basePath = "Unimake.Data.Generic.Embedded.";

                #region SQLite
                //aqui temos que extrair as dlls do SQLite
                EmbeddedAssembly.Load(basePath + "sqlite.x64.SQLite.Interop.dll", @"\x64\SQLite.Interop.dll");
                EmbeddedAssembly.Load(basePath + "sqlite.x86.SQLite.Interop.dll", @"\x86\SQLite.Interop.dll");

                EmbeddedAssembly.Load(basePath + "sqlite.SQLite.Designer.dll", @"\SQLite.Designer.dll");
                EmbeddedAssembly.Load(basePath + "sqlite.System.Data.SQLite.dll", @"\System.Data.SQLite.dll");
                EmbeddedAssembly.Load(basePath + "sqlite.System.Data.SQLite.Linq.dll", @"\System.Data.SQLite.Linq.dll");
                #endregion

                #region Npgsql
                EmbeddedAssembly.Load(basePath + "Npgsql.Npgsql.dll", @"\Npgsql.dll");
                EmbeddedAssembly.Load(basePath + "Npgsql.policy.2.0.Npgsql.config", @"\policy.2.0.Npgsql.config");
                EmbeddedAssembly.Load(basePath + "Npgsql.policy.2.0.Npgsql.dll", @"\policy.2.0.Npgsql.dll");
                EmbeddedAssembly.Load(basePath + "Npgsql.Mono.Security.dll", @"\Mono.Security.dll");
                #endregion

                Initialized = true;
            }
        }

        /// <summary> wu
        /// Cria o objeto principal
        /// </summary>
        public static DataGenericSettings Settings
        {
            get { return settings; }
        }

        DatabaseType _databaseType = (DatabaseType)(-1);
        /// <summary>
        /// Tipo de base de dados
        /// </summary>
        [ConfigurationProperty(
            name: "DatabaseType",
            DefaultValue = DatabaseType.SQLite,
            IsRequired = false)]
        public DatabaseType DatabaseType
        {
            get
            {
                if ((int)_databaseType == -1)
                    _databaseType = (DatabaseType)this["DatabaseType"];
                return _databaseType;
            }
            set { _databaseType = value; }
        }

        string _connectionString = "";
        /// <summary>
        /// String de conexão
        /// </summary>
        [ConfigurationProperty(
            name: "ConnectionString",
            IsRequired = true)]
        public string ConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(_connectionString))
                    _connectionString = this["ConnectionString"].ToString();
                return _connectionString;
            }
            set { _connectionString = value; }
        }

        bool _useBoolAsInt = true;
        /// <summary>
        /// String de conexão
        /// </summary>
        [ConfigurationProperty(
            name: "UseBoolAsInt",
            IsRequired = false)]
        public bool UseBoolAsInt
        {
            get
            {
                if (Contains("BoolAsInt"))
                    _useBoolAsInt = Convert.ToBoolean(this["BoolAsInt"]);

                return _useBoolAsInt;
            }
            set { _useBoolAsInt = value; }
        }

        /// <summary>
        /// Retorna true se contém a propriedade
        /// </summary>
        /// <param name="index">nome da propriedade</param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThrough()]
        private bool Contains(string index)
        {
            bool result = false;

            try
            {
                if (this["BoolAsInt"] != null)
                    result = true;
            }
            catch (NullReferenceException)
            {
                result = false;
            }

            return result;
        }
    }
}

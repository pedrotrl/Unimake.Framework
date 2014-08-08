using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic;

namespace Unimake
{
    public static class ConnectionExtensions
    {
        /// <summary>
        /// Executa uma instrução sql (SELECT) e retorna
        /// </summary>
        /// <param name="connection">conexão ativa</param>
        /// <param name="commandText">instrução a ser executada</param>
        /// <param name="whereParameters">filtros do registro, se houver</param>
        /// <param name="order">ordenação dos registros, se houver</param>
        /// <returns></returns>
        public static DataReader ExecuteReader(this Connection connection, string commandText,
            IEnumerable<Parameter> parameters = null)
        {
            using(Command command = connection.CreateCommand())
            {
                command.CommandText = commandText;

                if(parameters != null)
                {
                    foreach(Parameter item in parameters)
                    {
                        command.Parameters.Add(item);
                    }
                }

                DataReader result = command.ExecuteReader();
                return result;
            }
        }

        /// <summary>
        /// Executa um comando do tipo INSERT, UPDATE, DELETE.
        /// </summary>
        /// <param name="commandText">Query sql que será executada contra a base de dados</param>
        /// <param name="parameters">Parâmetros que serão adicionados ao comando para execução</param>
        ///<param name="connection">Conexão atual válida</param>
        public static void ExecuteNonQuery(this Connection connection, string commandText,
            IEnumerable<Parameter> parameters = null)
        {
            Command command = connection.CreateCommand();
            command.CommandText = commandText;

            if(parameters != null)
            {
                foreach(Parameter item in parameters)
                {
                    command.Parameters.Add(item);
                }
            }

            command.ExecuteNonQuery();
        }
    }
}

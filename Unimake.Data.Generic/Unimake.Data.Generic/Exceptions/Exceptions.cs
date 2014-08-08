using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic.Exceptions
{
    public class ValuesNotDefined: Exception
    {
        public override string Message
        {
            get
            {
                return "Values not defined for IN comparison";
            }
        }
    }

    public class ComparisonNotDefined: Exception
    {
        public override string Message
        {
            get
            {
                return "Comparison not defined";
            }
        }
    }

    /// <summary>
    /// ocorre quando um campo não existe na recordset
    /// </summary>
    public class FieldDoesntExist: Exception
    {
        private string fieldName = "";
        private string listOfFields = "";

        public FieldDoesntExist(string fieldName, string listOfFields = "")
        {
            this.fieldName = fieldName;
            this.listOfFields = listOfFields;
        }

        public override string Message
        {
            get
            {

                string result = "The field name \"" + fieldName + "\"cannot be found.";

                if(!String.IsNullOrEmpty(listOfFields))
                    result += " List of fields:" + listOfFields;

                return result;
            }
        }

    }

    public class InvalidPageNumber: Exception
    {
        public override string Message
        {
            get
            {
                return "Invalid page number";
            }
        }
    }

    /// <summary>
    /// tipo de base de dados não definida
    /// </summary>
    public class DatabaseTypeNotDefined: Exception
    {
        public override string Message
        {
            get
            {
                return "the database type is undefined";
            }
        }

    }

    /// <summary>
    /// tipo de base de dados não implementado
    /// </summary>
    public class DatabaseTypeNotImplemented: Exception
    {
        public override string Message
        {
            get
            {
                return "database type is not implemented yet";
            }
        }
    }

    public class DMLTypeNotDefined: Exception
    {
        public override string Message
        {
            get
            {
                return "the DML type is undefined";
            }
        }

    }

    /// <summary>
    /// para usar os métodos Execute da classe Connection. Os parâmetros tem que estar definidos
    /// </summary>
    public class ParametersNotDefined: Exception
    {
        public override string Message
        {
            get
            {
                return "parameters not defined.";
            }
        }

    }

    /// <summary>
    /// para usar os métodos Execute da classe Connection. O nome da tabela deve estar definido
    /// </summary>
    public class FromTableNotDefined: Exception
    {
        public override string Message
        {
            get
            {
                return "Property BaseTable not defined.\n\tEx. Command.BaseTable = \"tab_Users\"";
            }
        }

    }

    /// <summary>
    /// Para usar a manipulação de dados a transação tem que estar definida
    /// </summary>
    public class TransactionNotDefined: Exception
    {
        public override string Message
        {
            get
            {
                return "Transaction is not defined";
            }
        }

    }

    /// <summary>
    /// este erro acontece quando a conexão não está no buffer e é usado o método ChangeCurrentConnection
    /// </summary>
    public class ConnectionNotInBuffer: Exception
    {
        public override string Message
        {
            get
            {
                return "Connection doesn't exist in Buffer.";
            }
        }

    }

    /// <summary>
    /// Lançada quando uma conexão não foi passada para o objeto
    /// </summary>
    public class NoConnectionSet: Exception
    {
        public override string Message
        {
            get
            {
                return "No connection set.";
            }
        }
    }

    /// <summary>
    /// Exceção lançada quando o DynamicPaging não está definido
    /// </summary>
    public class DynamicPaging: Exception
    {
        public override string Message
        {
            get
            {
                return "The DynamicPaging was not defined for this object";
            }
        }
    }
}
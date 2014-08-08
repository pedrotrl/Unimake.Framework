using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Unimake.Data.Generic.Model
{

    /// <summary>
    ///public interface base para todos os tipos de objeto dentro da aplicação
    /// </summary>
    public interface IBaseModel: ICloneable, IDisposable
    {
        /// <summary>
        /// DataReader associada à este objeto
        /// </summary>
        [XmlIgnore]
        DataReader CurrentDataReader { get; set; }

        /// <summary>
        /// Conexão associada à esta instância
        /// </summary>
        [XmlIgnore]
        Connection Connection { get; set; }

        /// <summary>
        /// Preenche este objeto com os dados do recordset
        /// </summary>
        /// <param name="rs">recordset com os dados que deverão ser usados para preencher este objeto</param>
        void Populate(DataReader dataReader);

        /// <summary>
        /// Valida o objeto antes de salvar na base de dados.
        /// <para>retorna o erro se o objeto não puder ser salvo.</para>
        /// </summary>
        /// <param name="updating">se true está atualizando</param>
        /// <returns>Erro se o objeto não puder ser salvo</returns>
        void Validate(bool updating);

        /// <summary>
        /// Valida o objeto antes de excluir
        /// <para>retorna o erro se o objeto não puder ser excluído.</para>
        /// </summary>
        /// <param name="model">modelo base para ser validado</param>
        /// <returns>Erro, se não puder ser feito um delete</returns>
        void ValidateDelete();

        /// <summary>
        /// Acontece sempre antes de salvar o registro, e depois de iniciar a transação
        /// </summary>
        /// <param name="updating">se true está sendo atualizado na base de dados</param>
        void BeforeSave(bool updating);

        /// <summary>
        /// Acontece sempre depois de salvar o registro e antes da confirmar a transação
        /// </summary>
        /// <param name="updating">se true está sendo atualizado na base de dados</param>
        void AfterSave(bool updating);
        /// <summary>
        /// Retorna o tablehash da tabela
        /// </summary>
        /// <returns>string com o tablehash do objeto</returns>
        string GetTableHash();
        /// <summary>
        /// Salva o objeto e retorna o GUID criado para este objeto
        /// </summary>
        /// <returns></returns>
        PK Save<PK>();
        /// <summary>
        /// Detecta se o registro é novo ou não. Se true é um novo registro
        /// </summary>
        bool New { get; set; }

        /// <summary>
        /// Avalida se este modelo é um IChidlModel e retorna true em caso afirmativo.
        /// </summary>
        /// <param name="model">Modelo que deverá ser comparado</param>
        /// <returns>Retorna true se este modelo for um IChidlModel</returns>
        bool IsChildModel();

        #region Find
        /// <summary>
        /// Encontra todos os registros
        /// </summary>
        /// <param name="model">Modelo base para pesquisa</param>
        /// <typeparam name="T">Tipo que o find deverá retornar </typeparam>
        /// <returns>Lista de modelos base</returns>
        List<T> Find<T>()
            where T: IBaseModel;

        /// <summary>
        /// Encontra os registros com base na condição informada
        /// </summary>
        /// <param name="w">condição para buscar os registros. Não é obrigatório e pode ser nulo</param>
        /// <param name="model">Modelo base para pesquisa</param>
        /// <typeparam name="T">Tipo que o find deverá retornar </typeparam>
        /// <returns>Lista de modelos base</returns>
        List<T> Find<T>(Where w)
            where T: IBaseModel;

        /// <summary>
        /// Encontra os registros com base na condição informada
        /// </summary>
        /// <param name="where">Parâmetros que serão usados na condição para buscar os registros. Não é obrigatório e pode ser nulo</param>
        /// <param name="model">Modelo base para pesquisa</param>
        /// <typeparam name="T">Tipo que o find deverá retornar </typeparam>
        /// <returns>Lista de modelos base</returns>
        List<T> Find<T>(IEnumerable<Parameter> where = null)
            where T: IBaseModel;

        /// <summary>
        /// Encontra os registros com base na condição informada
        /// </summary>
        /// <param name="whereParameters">Parâmetros que serão usados na condição para buscar os registros. Não é obrigatório e pode ser nulo</param>
        /// <param name="model">Modelo base para pesquisa</param>
        /// <typeparam name="T">Tipo que o find deverá retornar </typeparam>
        /// <param name="w">Condição where do filtro</param>
        /// <returns>Lista de modelos base</returns>
        List<T> Find<T>(Where w = null, IEnumerable<Parameter> whereParameters = null)
            where T: IBaseModel;


        /// <summary>
        /// Retorna um regitro pelo GUID. Null se não encontrar nada
        /// </summary>
        /// <param name="id">identificador do registro</param>
        /// <returns></returns>
        T Get<T>(object id)
            where T: IBaseModel, new();

        /// <summary>
        /// Encontra os registros com base na condição informada
        /// </summary>
        /// <param name="w">condfição para buscar os registros</param>
        /// <param name="model">Modelo base para pesquisa</param>
        /// <typeparam name="T">Tipo que o find deverá retornar </typeparam>
        /// <param name="parent">Objeto pai. Quando pesquisado classes filhas</param>
        /// <typeparam name="P">Tipo do objeto pai</typeparam>
        /// <returns>Lista de modelos base</returns>
        List<T> Find<T, P>(Where w, P parent)
            where P: IParentModel
            where T: IBaseModel, IChildModel<P>;
        #endregion

        /// <summary>
        /// Prepara o comando antes de executar um insert ou update
        /// </summary>
        /// <param name="command">comando que será preparado antes da execução</param>
        /// <param name="updating">se true está sendo atualizado na base de dados</param>
        void PrepareCommand(Command command, bool updating);

        /// <summary>
        /// Prepara o comando antes de executar um select
        /// </summary>
        /// <param name="command">comando que será preparado antes da execução</param>
        /// <param name="where">Filtro where adicional</param>
        void PrepareReader(Command command, Where where);

        #region IDispose members
        /// <summary>
        /// Retorna true se o objeto está descarregado
        /// </summary>
        bool Disposed { get; }
        #endregion
    }
}

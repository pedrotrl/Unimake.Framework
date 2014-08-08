using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.ComponentModel;

namespace Unimake.Data.Generic
{
    /// <summary>
    /// Esta classe é responsável por gerar um log de toda ação que é executada pelo data.generic
    /// </summary>
    public static class Debug
    {
        #region Construtor
        public static void Activate()
        {
            log4net.Config.XmlConfigurator.Configure();
            DebugModeOn = true;
        }

        public static void Deactivate()
        {
            DebugModeOn = false;
        }
        #endregion

        #region propriedades

        private static readonly ILog log = LogManager.GetLogger("Unimake.Data.Generic");

        /// <summary>
        /// Se true, o modo debug está ligado
        /// </summary>
        public static bool DebugModeOn { get; private set; }

        /// <summary>
        /// Categoria onde é gravado os logs de debug no visual studio
        /// </summary>
        public static string Category { get { return "Unimake.Data.Generic Debugger"; } }
        #endregion

        #region ExecuteNonQuery
        internal static void ExecuteNonQueryStart(Command command)
        {
            Log(command.ToString());
        }

        internal static void ExecuteNonQueryError(Command command, Exception ex)
        {
            Log(command.ToString());
            LogError(ex);
        }

        internal static void ExecuteNonQueryFinish(Command command)
        {
            Log(command.ToString());
        }
        #endregion

        #region ExecuteReader
        internal static void ExecuteReaderStart(Command command)
        {
            Log(command.ToString());
        }

        internal static void ExecuteReaderError(Command command, Exception ex)
        {
            Log(command.ToString());
            LogError(ex);
        }

        internal static void ExecuteReaderFinish(Command command)
        {
            Log(command.ToString());
        }
        #endregion

        #region LOG
        /// <summary>
        /// Escreve uma informação de debug no arquivo
        /// </summary>
        /// <param name="message">mensagem a ser escrita</param>
        public static void Log(string message)
        {
            if(!DebugModeOn) return;

            using(BackgroundWorker worker = new BackgroundWorker())
            {
                worker.DoWork += new DoWorkEventHandler((sender, e) =>
                {
                    try
                    {
                        log.Debug(message);
                        System.Diagnostics.Debug.WriteLine(message, Category);
                    }
                    catch
                    {
                        //nada, pois se não conseguir criar o Log, não tem problema.
                    }
                });

                worker.RunWorkerAsync();
            }
        }



        /// <summary>
        /// Escreve uma informação de erro no arquivo
        /// </summary>
        /// <param name="ex">erro lançado</param>
        public static void LogError(Exception ex)
        {
            if(!DebugModeOn) return;
            log.Error(ex.Message, ex);
            System.Diagnostics.Debug.WriteLine(ex.ToString(), Category);
        }

        /// <summary>
        /// Escreve uma informação de erro no arquivo
        /// </summary>
        /// <param name="message">Mensagem a ser exibida</param>
        public static void LogError(string message)
        {
            if(!DebugModeOn) return;
            log.Error(message);
            System.Diagnostics.Debug.WriteLine(message, Category);
        }
        #endregion
    }
}
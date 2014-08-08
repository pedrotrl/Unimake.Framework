using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace Unimake.Data.Generic.Factory
{

#if DEBUG
    public struct ConnectionThreadPool
#else
        struct ConnectionThreadPool
#endif
    {
        public Connection CurrentConnection { get; set; }
        public Thread CurrentThread { get; set; }
    }


    /// <summary>
    /// Cria uma nova conexão com a base de dados
    /// </summary>
    public static class ConnectionFactory
    {
        #region locais
        static BackgroundWorker wrk = null;
        static bool wrkBusy = false;
#if DEBUG
        public static IDictionary<int, ConnectionThreadPool> instances = new Dictionary<int, ConnectionThreadPool>();
        public static IDictionary<int, ConnectionThreadPool> newInstances = new Dictionary<int, ConnectionThreadPool>();
#else
        static IDictionary<int, ConnectionThreadPool> instances = new Dictionary<int, ConnectionThreadPool>();
        static IDictionary<int, ConnectionThreadPool> newInstances = new Dictionary<int, ConnectionThreadPool>();
#endif
        #endregion

        #region Construtores
        static ConnectionFactory()
        {

        }
        #endregion

        #region Factory
        /// <summary>
        /// Cria uma nova conexão e retorna
        /// </summary>
        /// <param name="open">se true, cria e abre a conexão</param>
        /// <param name="_new">Se true, não usa a instancia atual, retorna uma nova</param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static Connection CreateConnection(bool open, bool _new)
        {
            #region IsAlive
            if(wrk == null)
            {
                //-------------------------------------------------------------------------
                // Controla se as thread estão em execução
                //-------------------------------------------------------------------------
                wrk = new BackgroundWorker();
                wrk.DoWork += (s, e) =>
                {
                    wrkBusy = true;

                    while(wrkBusy)
                    {
                        Thread.Sleep(1000);

                        try
                        {
                            IEnumerator<KeyValuePair<int, ConnectionThreadPool>> enumerator = instances.GetEnumerator();

                            while(enumerator.MoveNext())
                            {
                                Thread t = enumerator.Current.Value.CurrentThread;
                                if(t != null && !t.IsAlive)
                                {
                                    int index = t.ManagedThreadId;
                                    Connection conn = enumerator.Current.Value.CurrentConnection;
                                    if(conn != null)
                                    {
                                        conn.Dispose();
                                        conn = null;
                                    }

                                    instances.Remove(t.ManagedThreadId);
                                }
                            }

                            enumerator = newInstances.GetEnumerator();

                            while(enumerator.MoveNext())
                            {
                                Thread t = enumerator.Current.Value.CurrentThread;
                                if(t != null && !t.IsAlive)
                                {
                                    int index = t.ManagedThreadId;
                                    Connection conn = enumerator.Current.Value.CurrentConnection;
                                    if(conn != null)
                                    {
                                        conn.Dispose();
                                        conn = null;
                                    }

                                    newInstances.Remove(t.ManagedThreadId);
                                }
                            }
                        }
                        catch(InvalidOperationException)
                        {
                            //nada
                        }
                    }
                };

                wrk.WorkerSupportsCancellation = true;
                wrk.Disposed += (s, e) =>
                {
                    wrkBusy = false;
                };

                wrk.RunWorkerAsync();
            }
            #endregion


            Connection result = _new ? GetNewInstance() : GetFromInstance();
            if(open) result.Open();
            return result;
        }

        /// <summary>
        /// Retorna a instancia da conexão
        /// </summary>
        /// <returns></returns>
        private static Connection GetFromInstance()
        {
            int index = Thread.CurrentThread.ManagedThreadId;
            ConnectionThreadPool pool = instances.ContainsKey(index) ? instances[index] : new ConnectionThreadPool();

            Connection result = pool.CurrentConnection;

            if(result == null || result.IsDisposed)
                result = GetNewInstance();

            pool.CurrentConnection = result;
            pool.CurrentThread = Thread.CurrentThread;

            instances[index] = pool;

            return result;
        }

        /// <summary>
        /// Retorna a instancia da conexão
        /// </summary>
        /// <returns></returns>
        private static Connection GetNewInstance()
        {
            int index = Thread.CurrentThread.ManagedThreadId;
            ConnectionThreadPool pool = newInstances.ContainsKey(index) ? newInstances[index] : new ConnectionThreadPool();

            Connection result = pool.CurrentConnection;

            if(result == null || result.IsDisposed)
                result = new Connection();

            pool.CurrentConnection = result;
            pool.CurrentThread = Thread.CurrentThread;

            newInstances[index] = pool;

            return result;
        }
        #endregion
    }
}
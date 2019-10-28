using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Threading;

namespace ShutDown
{
    static class Program
    {
        static bool IsSingleInstance()
        {
            try
            {
                //Проверяем на наличие мутекса в системе
                Mutex.OpenExisting("SHUT_DOWN_TIMER_MUTEX_NAME");
            }
            catch
            {
                //Если получили исключение значит такого мутекса нет, и его нужно создать
                Mutex mutex = new Mutex(true, "SHUT_DOWN_TIMER_MUTEX_NAME");
                return true;
            }
            //Если исключения не было, то процесс с таким мутексом уже запущен
            return false;
        }
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!IsSingleInstance()) return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AFKForm());
        }
    }
}

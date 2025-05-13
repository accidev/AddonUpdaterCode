using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AddonUpdater
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        /// 
        [STAThread]
        static void Main()
        {      
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) => 
            {
                MessageBox.Show($"Произошла ошибка: {e.Exception.Message}\n\nПожалуйста, перезапустите приложение.", 
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                MessageBox.Show($"Произошла критическая ошибка: {(e.ExceptionObject as Exception)?.Message}\n\nПожалуйста, перезапустите приложение.", 
                                "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            
            Application.Run(new FormMainMenu());
        }
    }
}

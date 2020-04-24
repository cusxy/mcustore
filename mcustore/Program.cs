using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mcustore
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*DataBaseClass.CreateNewMicrocontroller("mc 1", 1000, 552.5);
            DataBaseClass.CreateNewMicrocontroller("mc 2", 1000, 642.6);
            DataBaseClass.CreateNewMicrocontroller("mc 3", 1000, 705.4);

            List<string> mc = new List<string>();
            List<int> q = new List<int>();
            mc.Add("mc 1"); q.Add(1100);
            mc.Add("mc 2"); q.Add(500);
            mc.Add("mc 3"); q.Add(357);
            DataBaseClass.CreateNewOrder("ООО ПРАКТИКА", mc, q);*/

            Application.Run(new Work_Window());
        }
    }
}

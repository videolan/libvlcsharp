
namespace LibVLCSharp.Eto.Sample
{
    using System;
    using global::Eto.Forms;

    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                new Application(global::Eto.Platform.Detect).Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception !", MessageBoxType.Error);
            }
        }
    }
}

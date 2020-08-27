namespace ServiceExample
{
    partial class Service
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.BW_NPipe = new System.ComponentModel.BackgroundWorker();
            this.t_last_check = new System.Windows.Forms.Timer(this.components);
            this.BW_last_check = new System.ComponentModel.BackgroundWorker();
            // 
            // BW_NPipe
            // 
            this.BW_NPipe.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BW_NPipe_DoWork);
            // 
            // t_last_check
            // 
            this.t_last_check.Enabled = true;
            this.t_last_check.Interval = 1000;
            this.t_last_check.Tick += new System.EventHandler(this.t_last_check_Tick);
            // 
            // BW_last_check
            // 
            this.BW_last_check.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BW_last_check_DoWork);
            // 
            // Service
            // 
            this.ServiceName = "Service1";

        }

        #endregion

        private System.ComponentModel.BackgroundWorker BW_NPipe;
        private System.Windows.Forms.Timer t_last_check;
        private System.ComponentModel.BackgroundWorker BW_last_check;
    }
}

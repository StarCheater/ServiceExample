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
            this.BW_NPipe = new System.ComponentModel.BackgroundWorker();
            // 
            // BW_NPipe
            // 
            this.BW_NPipe.WorkerSupportsCancellation = true;
            this.BW_NPipe.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BW_NPipe_DoWork);
            this.BW_NPipe.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BW_NPipe_RunWorkerCompleted);
            // 
            // Service
            // 
            this.ServiceName = "Service1";

        }

        #endregion

        private System.ComponentModel.BackgroundWorker BW_NPipe;
    }
}

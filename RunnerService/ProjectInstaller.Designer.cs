namespace RunnerService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.runnerSvcProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.runnerSvcInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // runnerSvcProcessInstaller
            // 
            this.runnerSvcProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.runnerSvcProcessInstaller.Password = null;
            this.runnerSvcProcessInstaller.Username = null;
            // 
            // runnerSvcInstaller
            // 
            this.runnerSvcInstaller.Description = "Automatic School Bell Service.";
            this.runnerSvcInstaller.DisplayName = "Runner Service For Automatic School Bell";
            this.runnerSvcInstaller.ServiceName = "RunnerSvc";
            this.runnerSvcInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.runnerSvcProcessInstaller,
            this.runnerSvcInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller runnerSvcProcessInstaller;
        private System.ServiceProcess.ServiceInstaller runnerSvcInstaller;
    }
}
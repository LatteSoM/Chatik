using Chat.ViewModel.HelpTools;
using System;
using Chat;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Chat.ViewModel///sdfghjk
{
    internal class StartViewModel : BindingTools
    {
        private static string userName;

        private static string ipAdres;

        public string _ipAdres
        {
            get { return ipAdres; }
            set
            {
                ipAdres = value;
                OnPropertyChanged();
            }
        }

        public string _userName
        {
            get { return userName; }
            set
            {
                userName = value;
                OnPropertyChanged();
            }
        }

        public void JoinServer()
        {
            try
            {
                string regexpettern = @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)(.(?!$)|$)){4}$";

                if (Regex.IsMatch(_ipAdres, regexpettern) || (_ipAdres == "localhost"))
                {
                    MainViewModel.nickname = _userName;
                    MainViewModel.ipAddress = _ipAdres;
                    MainViewModel.needToHide = true;
                    MainWindow s = new MainWindow();
                    /*CloseWIn();*/
                    s.Show();
                }
                else { throw new Exception("Неверный формат IP адреса"); }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public BindableCommand JoinCom { get; set; }

        public StartViewModel()
        {
            JoinCom = new BindableCommand(_ => JoinServer());
            CreateCom = new BindableCommand(_ => CreateServer());
        }

        public void CreateServer()
        {
            /*CloseWIn();*/
            try
            {
                if (_userName != null || !_userName.Contains(""))
                {
                    MainViewModel.needToHide = false;
                    MainWindow s = new MainWindow();
                    s.Show();
                }
            }
            catch
            {
                MessageBox.Show("Имя пользоввателя не должно быть пустым");
            }




        }

        public BindableCommand CreateCom { get; set; }
    }

}

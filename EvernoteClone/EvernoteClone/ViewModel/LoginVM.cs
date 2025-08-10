using EvernoteClone.Model;
using EvernoteClone.ViewModel.Commends;
using System.ComponentModel;
using System.Windows;

namespace EvernoteClone.ViewModel
{
    public class LoginVM : INotifyPropertyChanged
    {
        private bool isShowingRegister = false;

        private User user;
        public User User
        {
            get { return user; }
            set 
            { 
                user = value; 
                OnPropertyChanged(nameof(User));
            }
        }

        private string userName;
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                User = new User
                {
                    Username = userName,
                    Password = this.Password,
                    Name = this.User.Name,
                    Lastname = this.User.Lastname,
                    ConfirmPassword = this.User.ConfirmPassword
                };
                OnPropertyChanged(nameof(UserName));
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                User = new User
                {
                    Username = this.UserName,
                    Password = this.Password,
                    Name = name,
                    Lastname = this.Lastame,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged(nameof(Name));
            }
        }

        private string lastname;
        public string Lastame
        {
            get { return lastname; }
            set
            {
                lastname = value;
                User = new User
                {
                    Username = this.UserName,
                    Password = this.Password,
                    Name = this.Name,
                    Lastname = lastname,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged(nameof(Lastame));
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                User = new User
                {
                    Username = this.UserName,
                    Password = password,
                    Name = this.User.Name,
                    Lastname = this.User.Lastname,
                    ConfirmPassword = this.User.ConfirmPassword
                };
                OnPropertyChanged(nameof(Password));
            }
        }

        private string confirmPassword;
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                confirmPassword = value;
                User = new User
                {
                    Username = this.UserName,
                    Password = this.Password,
                    Name = this.Name,
                    Lastname = this.Lastame,
                    ConfirmPassword = confirmPassword
                };
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        private Visibility loginVis;
        public Visibility LoginVis
        {
            get { return loginVis; }
            set 
            { 
                loginVis = value;
                OnPropertyChanged(nameof(LoginVis));
            }
        }

        private Visibility registerVis;
        public Visibility RegisterVis
        {
            get { return registerVis; }
            set
            {
                registerVis = value;
                OnPropertyChanged(nameof(RegisterVis));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public RegisterCommand RegisterCommand { get; set; }
        public LoginCommand LoginCommand { get; set; }
        public ShowRegisterCommand ShowRegisterCommand { get; set; }

        public LoginVM()
        {
            LoginVis = Visibility.Visible;
            RegisterVis = Visibility.Collapsed;

            RegisterCommand = new RegisterCommand(this);
            LoginCommand = new LoginCommand(this);
            ShowRegisterCommand = new ShowRegisterCommand(this);

            User = new User();
        }

        public void SwitchViews()
        {
            isShowingRegister = !isShowingRegister;

            if (isShowingRegister)
            {
                RegisterVis = Visibility.Visible;
                LoginVis = Visibility.Collapsed;
            }
            else
            {
                RegisterVis = Visibility.Collapsed;
                LoginVis = Visibility.Visible;
            }
        }

        public void Login()
        {
            // TODO: Logic for login
            MessageBox.Show("Login logic not implemented yet.");
        }

        public void Register()
        {
            // TODO : Logic for registration
            MessageBox.Show("Register logic not implemented yet.");
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using StandUpTimer.Annotations;
using StandUpTimer.Common;
using StandUpTimer.Models;
using StandUpTimer.Services;

namespace StandUpTimer.ViewModels
{
    internal class StandUpViewModel : INotifyPropertyChanged, ICanHandleDeskStateStarts
    {
        private readonly DispatcherTimer updateTimer;
        private string currentImage;
        private Visibility exitButtonVisibility;
        private Visibility okButtonVisibility;
        private Visibility creativeCommonsVisibility;
        private bool shake;
        private readonly StandUpModel model;
        private readonly AuthenticationService authenticationService;
        private readonly IBringToForeground bringToForeground;
        private ICommand okCommand;
        private ICommand skipCommand;
        private ICommand loginCommand;

        public StandUpViewModel(StandUpModel model, AuthenticationService authenticationService, IBringToForeground bringToForeground)
        {
            Contract.Requires(model != null);
            Contract.Requires(authenticationService != null);
            Contract.Requires(bringToForeground != null);

            this.model = model;
            this.authenticationService = authenticationService;
            this.bringToForeground = bringToForeground;

            model.DeskStateChanged += (sender, args) => DeskStateEnded();

            SetImageAccordingToDeskState();
            ExitButtonVisibility = Visibility.Hidden;
            OkButtonVisibility = Visibility.Collapsed;
            CreativeCommonsVisibility = Visibility.Hidden;

            updateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            updateTimer.Tick += OnUpdateTimerTicked;
            updateTimer.Start();
        }

        public void DeskStateEnded()
        {
            Contract.Requires<InvalidOperationException>(Shake == false);

            SetImageAccordingToDeskState();

            OkButtonVisibility = Visibility.Visible;
            Shake = true;

            bringToForeground.Now();
        }

        private void SetImageAccordingToDeskState()
        {
            switch (model.DeskState)
            {
                case DeskState.Sitting:
                    CurrentImage = @"..\Images\sitting.png";
                    break;
                case DeskState.Standing:
                    CurrentImage = @"..\Images\standing.png";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnUpdateTimerTicked(object sender, EventArgs e)
        {
            OnPropertyChanged(() => RemainingTimeToChangeAsString);
            OnPropertyChanged(() => RemainingTimeToChangeInPercent);
            OnPropertyChanged(() => TimeOfLegInFraction);
        }

        public void DeskStateStarted()
        {
            Contract.Requires<InvalidOperationException>(Shake == true);

            Shake = false;
            OkButtonVisibility = Visibility.Collapsed;

            model.NewDeskStateStarted();
        }

        public string RemainingTimeToChangeAsString
        {
            get { return model.ChangeTime.Subtract(TestableDateTime.Now).FormatRemainingTime(); }
        }

        public double RemainingTimeToChangeInPercent
        {
            get { return model.ChangeTime.Subtract(TestableDateTime.Now).PercentageTo(model.CurrentLeg); }
        }

        public double TimeOfLegInFraction
        {
            get { return model.ChangeTime.Subtract(TestableDateTime.Now).FractionTo(model.CurrentLeg); }
        }

        public string CurrentImage
        {
            get { return currentImage; }
            set
            {
                currentImage = value;
                OnPropertyChanged();
            }
        }

        public string AuthenticationStatus
        {
            get { return authenticationService.IsLoggedIn ? @"..\Images\loggedInButton.png" : @"..\Images\loginButton.png"; }
        }

        public Visibility ExitButtonVisibility
        {
            get { return exitButtonVisibility; }
            set
            {
                exitButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility OkButtonVisibility
        {
            get { return okButtonVisibility; }
            set
            {
                okButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility CreativeCommonsVisibility
        {
            get { return creativeCommonsVisibility; }
            set
            {
                creativeCommonsVisibility = value;
                OnPropertyChanged();
            }
        }

        public bool Shake
        {
            get { return shake; }
            set
            {
                shake = value;
                OnPropertyChanged();
            }
        }

        public string VersionNumber { get { return "V" + Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        public ICommand OkCommand
        {
            get
            {
                return okCommand ?? (okCommand = new RelayCommand(_ => DeskStateStarted()));
            }
        }

        public ICommand SkipCommand
        {
            get
            {
                return skipCommand ?? (skipCommand = new RelayCommand(_ =>
                {
                    model.Skip();
                    DeskStateStarted();
                }));
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                return loginCommand ?? (loginCommand = new RelayCommand(_ =>
                {
                    authenticationService.ChangeState();

                    OnPropertyChanged(() => AuthenticationStatus);
                }, _ => !authenticationService.IsLoggedIn));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPropertyChanged<T>(Expression<Func<T>> exp)
        {
            var memberExpression = (MemberExpression)exp.Body;
            var propertyName = memberExpression.Member.Name;

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
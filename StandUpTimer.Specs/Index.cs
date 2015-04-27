﻿using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using Concordion.Runners.NUnit;
using NUnit.Framework;
using StandUpTimer.Specs.Properties;

namespace StandUpTimer.Specs
{
    [TestFixture]
    public class Index : ExecutableSpecification
    {
        public string ItBeginsWithTheSittingPhase(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                var currentImage = standUpTimer.CurrentImage;

                if (currentImage == null)
                    return "No image loaded.";

                var currentImageFileName = standUpTimer.CurrentImageFileName;

                if (currentImageFileName == null)
                    return "Cannot determine current image.";

                return currentImageFileName.Equals("..\\Images\\sitting.png")
                           ? Resources.ItBeginsWithTheSittingPhase
                           : "It does not begin with the sitting phase.";
            }
        }

        public string YouCanSeeTheCloseButton(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                var closeButton = standUpTimer.CloseButton;

                if (closeButton == null)
                    return "There is no close button.";

                return closeButton.Visible
                           ? Resources.TheCloseButton
                           : "The close button is not visible.";
            }
        }

        public string YouCanSeeTheSkipButton(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                var skipButton = standUpTimer.SkipButton;

                if (skipButton == null)
                    return "There is no skip button.";

                return skipButton.Visible
                           ? Resources.TheSkipButton
                           : "The skip button is not visible.";
            }
        }

        public string YouCanSeeTheAttributionButton(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                var attributionButton = standUpTimer.AttributionButton;

                if (attributionButton == null)
                    return "There is no attribution button.";

                return attributionButton.Visible
                           ? Resources.TheAttributionButton
                           : "The attribution button is not visible.";
            }
        }

        public string YouCanSeeTheLoginButton(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                var loginButton = standUpTimer.LoginButton;

                if (loginButton == null)
                    return "There is no login button.";

                return loginButton.Visible
                           ? Resources.TheLoginButton
                           : "The login button is not visible.";
            }
        }

        public string TheLoginDialogHasTheRegisterLink(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                string errorMessage;
                var loginDialog = standUpTimer.OpenLoginDialog(out errorMessage);

                if (loginDialog == null)
                    return errorMessage;

                var registerLink = loginDialog.RegisterLink;

                if (registerLink == null)
                    return "There is no register link";

                return registerLink.Visible
                    ? Resources.TheLoginDialogHasTheRegisterLink
                    : "The register link is not visible";
            }
        }

        public void TakeLoginScreenshot()
        {
            using (var standUpTimer = StandUpTimer.Launch())
            {
                string errorMessage;
                var loginDialog = standUpTimer.OpenLoginDialog(out errorMessage);

                if (loginDialog == null)
                    return;

                loginDialog.TakeScreenshot("login.png");
            }
        }

        public string CreateNewUser(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                string errorMessage;
                var loginDialog = standUpTimer.OpenLoginDialog(out errorMessage);

                if (loginDialog == null)
                    return errorMessage;

                loginDialog.RegisterLink.Click();
            }

            var homePage = Host.Instance.NavigateToInitialPage<HomePage>()
                .GoToRegisterPage()
                .RegisterUser(new RegisterModel("test@example.com", "password", "password"));

            return homePage != null
                ? Resources.CreateNewUser
                : "Could not create a new user account via the website";
        }

        public void TakeRegisterScreenshot()
        {
            Host.Instance.NavigateToInitialPage<HomePage>()
                .GoToRegisterPage();

            Host.Camera.TakeScreenshot("register.png");
        }

        public string YouCanLogin(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                string errorMessage;
                var loginDialog = standUpTimer.OpenLoginDialog(out errorMessage);

                if (loginDialog == null)
                    return errorMessage;

                loginDialog.LogIn("test@example.com", "password");

                standUpTimer.WaitUntilLoggedIn();

                var currentAuthenticationStatusFileName = standUpTimer.CurrentAuthenticationStatusFileName;

                return currentAuthenticationStatusFileName.Equals("..\\Images\\loggedInButton.png")
                           ? Resources.YouCanLogin
                           : "It was not possible to log you in.";
            }
        }

        public string RetryLoggingIn(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                string errorMessage;
                var loginDialog = standUpTimer.OpenLoginDialog(out errorMessage);

                if (loginDialog == null)
                    return errorMessage;

                loginDialog.LogIn("test@example.com", "wrongPassword");

                var secondLoginDialog = standUpTimer.FindLoginDialog();

                return secondLoginDialog != null
                    ? Resources.RetryLoggingIn
                    : "Didn't get the second login dialog with the error message.";
            }
        }

        public void TakeRetryScreenshot(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                string errorMessage;
                var loginDialog = standUpTimer.OpenLoginDialog(out errorMessage);

                if (loginDialog == null)
                    return;

                loginDialog.LogIn("test@example.com", "wrongPassword");

                var secondLoginDialog = standUpTimer.FindLoginDialog();
                secondLoginDialog.TakeScreenshot("retry.png");
            }
        }

        public void TakeStatisticsScreenshot()
        {
            Host.Instance.NavigateToInitialPage<HomePage>()
                .GoToLoginPage()
                .Login(new LoginModel("test@example.com", "password"))
                .GoToStatisticsPage();

            Host.Camera.TakeScreenshot("statistics.png");
        }

        public string YouCanSeeTheRemainingTimeInProgressBar(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                var progressBar = standUpTimer.ProgressBar;

                if (progressBar == null)
                    return "There is no progress bar.";

                if (!progressBar.Visible)
                    return "The progress bar is not visible.";

                var progressText = standUpTimer.ProgressBarText;

                return string.IsNullOrEmpty(progressText)
                           ? "No progress information available in progress bar"
                           : Resources.YouCanSeeTheRemainingTimeInProgressBar;
            }
        }

        public string YouCanSeeTheRemainingTimeInTaskbar(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                var title = standUpTimer.Title;

                return title.Contains("60\nmin")
                           ? "No progress information available in title"
                           : Resources.YouCanSeeTheRemainingTimeInTaskbar;
            }
        }

        public void TakeStartScreenshot()
        {
            using (var standUpTimer = StandUpTimer.Launch())
            {
                standUpTimer.TakeScreenshot("start.png");
            }
        }

        public string WaitForTheTimeToElapseToGoToTheNextPosition(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            const int sittingWaitTime = 1000;

            using (var standUpTimer = StandUpTimer.Launch(sittingWaitTime))
            {
                Thread.Sleep(sittingWaitTime);

                var currentImageFileName = standUpTimer.CurrentImageFileName;

                if (currentImageFileName == null)
                    return "Cannot determine current image.";

                return currentImageFileName.Equals("..\\Images\\standing.png")
                           ? Resources.WaitForTheTimeToElapse
                           : "There was a wrong image after waiting the sitting time.";
            }
        }

        public string UseTheSkipButtonToGoToTheNextPosition(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var standUpTimer = StandUpTimer.Launch())
            {
                string errorMessage;

                if (!standUpTimer.TryGoToNextPosition(out errorMessage))
                    return errorMessage;

                var currentImageFileName = standUpTimer.CurrentImageFileName;

                if (currentImageFileName == null)
                    return "Cannot determine current image.";

                return currentImageFileName.Equals("..\\Images\\standing.png")
                           ? Resources.UseTheSkipButton
                           : "There was a wrong image after clicking the skip button.";
            }
        }

        public string AfterTheTimeElapsedTheAppGetsIntoView(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            const int sittingWaitTime = 1000;

            using (var standUpTimer = StandUpTimer.Launch(sittingWaitTime))
            {
                var psi = new ProcessStartInfo("notepad.exe") { WindowStyle = ProcessWindowStyle.Maximized };
                var process = Process.Start(psi);

                if (process == null)
                    return "could not start notepad";

                Thread.Sleep(sittingWaitTime);

                var result = standUpTimer.IsFocussed
                                 ? Resources.TheAppWillGetIntoView
                                 : "the app didn't get into view";

                process.Kill();

                return result;
            }
        }

        public string AfterTheTimeElapsedTheOkButtonIsVisible(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            const int sittingWaitTime = 1000;

            using (var standUpTimer = StandUpTimer.Launch(sittingWaitTime))
            {
                Thread.Sleep(sittingWaitTime);

                var okButton = standUpTimer.OkButton;

                if (okButton == null)
                    return "Cannot find the OK button.";

                return okButton.Visible
                           ? Resources.TheOkButtonIsVisible
                           : "the OK button is not visible";
            }
        }

        public string AfterClickingOkTheTimeTicksAgain(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            const int sittingWaitTime = 1000;

            using (var standUpTimer = StandUpTimer.Launch(sittingWaitTime))
            {
                Thread.Sleep(sittingWaitTime);

                string errorMessage;
                if (!standUpTimer.TryStopShaking(out errorMessage))
                    return errorMessage;

                standUpTimer.WaitUntilProgressBarTextIs("60\nmin");
                var progressBarText = standUpTimer.ProgressBarText;

                return progressBarText.Equals("60\nmin")
                           ? Resources.TheTimeIsTickingAgain
                           : "the time is not ticking correctly, " + progressBarText + " was shown.";
            }
        }

        public void TakeNextPhaseScreenshot()
        {
            using (var standUpTimer = StandUpTimer.Launch())
            {
                string errorMessage;

                if (!standUpTimer.TryGoToNextPosition(out errorMessage))
                    return;

                standUpTimer.WaitUntilProgressBarTextIs("20\nmin");
                standUpTimer.TakeScreenshot("nextPhase.png");
            }
        }

        public string TheAppStartsOnTheSamePosition(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            Point savedLocation;

            using (var standUpTimer = StandUpTimer.Launch())
            {
                savedLocation = standUpTimer.Location;
            }

            using (var standUpTimer = StandUpTimer.Launch())
            {
                return savedLocation == standUpTimer.Location
                           ? Resources.OnTheNextStartupTheAppStartOnThatPositionAgain
                           : "the app is not on the previous position.";
            }
        }

        public void TakeAttributionScreenshot()
        {
            using (var standUpTimer = StandUpTimer.Launch())
            {
                standUpTimer.OpenAttributionBox();
                standUpTimer.TakeScreenshot("attribution.png");
            }
        }
    }
}
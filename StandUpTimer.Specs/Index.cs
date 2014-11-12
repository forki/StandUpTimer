﻿using System.Diagnostics;
using System.IO;
using System.Threading;
using Concordion.Integration;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.WindowsAPI;
using Image = TestStack.White.UIItems.Image;

namespace StandUpTimer.Specs
{
    [ConcordionTest]
    public class Index
    {
        public string ItBeginsWithTheSittingPhase()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var currentImage = window.Get<Image>("CurrentImage");

                if (currentImage == null)
                    return "No image loaded.";

                var currentImageFileName = window.Get<Label>("CurrentImageFileName");

                if (currentImageFileName == null)
                    return "Cannot determine current image.";

                return currentImageFileName.Text.Equals("..\\Images\\sitting.png")
                           ? "It begins with the sitting phase."
                           : "It does not begin with the sitting phase.";
            }
        }

        public string YouCanSeeTheCloseButton()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var closeButton = window.Get<Button>("CloseButton");

                if (closeButton == null)
                    return "There is no close button.";

                return closeButton.Visible
                           ? "The close button"
                           : "The close button is not visible.";
            }
        }

        public string YouCanSeeTheSkipButton()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var skipButton = window.Get<Button>("SkipButton");

                if (skipButton == null)
                    return "There is no skip button.";

                return skipButton.Visible
                           ? "The skip button"
                           : "The skip button is not visible.";
            }
        }

        public string YouCanSeeTheAttributionButton()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var attributionButton = window.Get<Button>("AttributionButton");

                if (attributionButton == null)
                    return "There is no attribution button.";

                return attributionButton.Visible
                           ? "The attribution button"
                           : "The attribution button is not visible.";
            }
        }

        public string YouCanSeeTheRemainingTime()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var progressBar = window.Get<ProgressBar>("ProgressBar");

                if (progressBar == null)
                    return "There is no progress bar.";

                if (!progressBar.Visible)
                    return "The progress bar is not visible.";

                var progressText = window.Get<Label>("ProgressText");

                return string.IsNullOrEmpty(progressText.Text)
                           ? "No progress information available"
                           : "You can see how much time is left before changing to another position.";
            }
        }

        public void TakeStartScreenshot()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.PRINTSCREEN);

                Directory.CreateDirectory(@"results\StandUpTimer\Specs");
                File.Move("screenshot.png", @"results\StandUpTimer\Specs\start.png");
            }
        }

        public string WaitForTheTimeToElapseToGoToTheNextPosition()
        {
            const int sittingWaitTime = 1000;
            var processStartInfo = new ProcessStartInfo("StandUpTimer.exe", string.Format("--sit {0} --stand 3600000", sittingWaitTime));

            using (var application = Application.Launch(processStartInfo))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                Thread.Sleep(sittingWaitTime);

                var currentImageFileName = window.Get<Label>("CurrentImageFileName");

                if (currentImageFileName == null)
                    return "Cannot determine current image.";

                return currentImageFileName.Text.Equals("..\\Images\\standing.png")
                           ? "Wait for the time to elapse"
                           : "There was a wrong image after waiting the sitting time.";
            }
        }

        public string UseTheSkipButtonToGoToTheNextPosition()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var skipButton = window.Get<Button>("SkipButton");
                skipButton.Click();

                var currentImageFileName = window.Get<Label>("CurrentImageFileName");

                if (currentImageFileName == null)
                    return "Cannot determine current image.";

                return currentImageFileName.Text.Equals("..\\Images\\standing.png")
                           ? "Use the skip button"
                           : "There was a wrong image after clicking the skip button.";
            }
        }

        public string AfterTheTimeElapsedTheAppGetsIntoView()
        {
            const int sittingWaitTime = 1000;
            var processStartInfo = new ProcessStartInfo("StandUpTimer.exe", string.Format("--sit {0} --stand 3600000", sittingWaitTime));

            using (var application = Application.Launch(processStartInfo))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var psi = new ProcessStartInfo("notepad.exe") { WindowStyle = ProcessWindowStyle.Maximized };
                var process = Process.Start(psi);

                Thread.Sleep(sittingWaitTime);

                var result = window.IsFocussed
                                 ? "the app will get into view"
                                 : "the app didn't get into view";

                process.Kill();

                return result;
            }
        }

        public string AfterTheTimeElapsedTheOkButtonIsVisible()
        {
            const int sittingWaitTime = 1000;
            var processStartInfo = new ProcessStartInfo("StandUpTimer.exe", string.Format("--sit {0} --stand 3600000", sittingWaitTime));

            using (var application = Application.Launch(processStartInfo))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                Thread.Sleep(sittingWaitTime);

                var okButton = window.Get<Button>("OkButton");

                if (okButton == null)
                    return "Cannot find the OK button.";

                return okButton.Visible
                           ? "the OK button is visible"
                           : "the OK button is not visible";
            }
        }

        public string AfterClickingOkTheTimeTicksAgain()
        {
            return string.Empty;
        }

        public void TakeNextPhaseScreenshot()
        {
        }

        public string TheAppStartsOnTheSamePosition()
        {
            return string.Empty;
        }

        public void TakeAttributionScreenshot()
        {
        }
    }
}
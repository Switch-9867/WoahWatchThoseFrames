using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace WoahWatchThoseFrames
{
    public class WoahWTFMod : ModBehaviour
    {

        private readonly string PauseMenuManagers_String = "/PauseMenu/PauseMenuManagers";

        private Dictionary<string, Key> KeyBinds = new Dictionary<string, Key>()
        {
            { "activate", Key.Comma }
        };

        private int PauseFrameRate = 30;
        private int BackgroundFrameRate = 1;

        private int PlayerDefined_targetFrameRate;
        private int PlayerDefined_vSyncCount;

        private PauseMenuManager pauseMenuManager = null;
        private bool[] isPaused = new bool[2];

        private float[] frameTimes = new float[30];

        private void Start()
        {
            GetCurrentQualitySettings();

            ModHelper.Console.WriteLine($"OnStart: {nameof(WoahWTFMod)}");
        }


        private void Update()
		{
            int i = Time.frameCount % frameTimes.Length;
            frameTimes[i] = Time.deltaTime;

            if (GetKeyDown(KeyBinds["activate"]))
            {
                string fps = GetFps().ToString();
                ModHelper.Console.WriteLine("FPS: " + fps);
            }

            if (pauseMenuManager == null) pauseMenuManager = GetPauseMenuManager();
            if (pauseMenuManager == null) return;

            isPaused[1] = isPaused[0];
			isPaused[0] = pauseMenuManager.IsOpen();

            if(isPaused[0] && isPaused[0] != isPaused[1])
			{
                OnPause();
            }

            if (!isPaused[0] && isPaused[0] != isPaused[1])
            {
                OnResume();
            }
        }

        private double GetFps()
		{

            return Math.Round(1d / frameTimes.Average());
		}

		private void OnApplicationFocus(bool isFocused)
		{
            if (isPaused[0]) return;

            if (isFocused)
            {
                RestoreQulaitySettings();
            }
			else
			{
                GetCurrentQualitySettings();
                ApplyBackgroundQulaitySettings();
            }
        }

		private void ApplyBackgroundQulaitySettings()
		{
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = BackgroundFrameRate;
        }

		private PauseMenuManager GetPauseMenuManager()
		{
            GameObject go = null;
			try
			{
                go = GameObject.Find(PauseMenuManagers_String);
            }
            catch (Exception ex)
			{
                ModHelper.Console.WriteLine(ex.Message);
			}
            if (go == null) return null;

            return go.GetComponent<PauseMenuManager>();
        }

		private void GetCurrentQualitySettings()
		{
            PlayerDefined_targetFrameRate = Application.targetFrameRate;
            PlayerDefined_vSyncCount = QualitySettings.vSyncCount;
        }

        private void OnPause()
		{
            GetCurrentQualitySettings();
            ApplyPauseQulaitySettings();
		}

		private void ApplyPauseQulaitySettings()
		{
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = PauseFrameRate;
        }

		private void OnResume()
		{
            RestoreQulaitySettings();
        }

		private void RestoreQulaitySettings()
		{
            QualitySettings.vSyncCount = PlayerDefined_vSyncCount;
            Application.targetFrameRate = PlayerDefined_targetFrameRate;
        }
        private bool GetKeyDown(Key keyCode)
        {
            return Keyboard.current[keyCode].wasPressedThisFrame;
        }
    }
}

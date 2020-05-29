#if UNITY_ANDROID
using UnityEngine;
using System;

public static class AndroidNativePicker
{
    public delegate void OnDatePicked(int year,int month,int day);
    public delegate void OnTimePicked(int hourOfDay, int minute);

    public static void ShowDatePicker(OnDatePicked callback)
    {
        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;
        int day = DateTime.Now.Day;
        AndroidJavaObject dialog = new AndroidJavaObject("android.app.DatePickerDialog", GetActivity(), 
            new AndroidNativePicker.OnDateSetListenerPoxy(callback), year, month, day);
        dialog.Call("show");
    }

    public static void ShowTimePicker(OnTimePicked callback)
    {
        int hourOfDay = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        AndroidJavaObject dialog = new AndroidJavaObject("android.app.TimePickerDialog", GetActivity(), 
            new AndroidNativePicker.OnTimeSetListenerPoxy(callback), hourOfDay, minute, true);
        dialog.Call("show");
    }

    public static void SaveEvent(long beginTime, long endTime, string location)
    {
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.EDIT");
        intent.Call<AndroidJavaObject>("setType", "vnd.android.cursor.item/event");
        intent.Call<AndroidJavaObject>("putExtra", "beginTime", beginTime);
        intent.Call<AndroidJavaObject>("putExtra", "endTime", endTime);
        intent.Call<AndroidJavaObject>("putExtra", "allDay", false);
        intent.Call<AndroidJavaObject>("putExtra", "rrule", "FREQ=YEARLY");
        intent.Call<AndroidJavaObject>("putExtra", "title", "Meet Rebeka Virtually");
        intent.Call<AndroidJavaObject>("putExtra", "eventLocation", location);
        intent.Call<AndroidJavaObject>("putExtra", "description", "Meet Rebeka Virtually for the first time");
        GetActivity().Call("startActivity", intent);
    }

    private static AndroidJavaObject GetActivity()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }

    private static void RunOnUiThread(Action action)
    {
        GetActivity().Call("runOnUiThread", new AndroidJavaRunnable(action));
    }

    public class OnDateSetListenerPoxy : AndroidJavaProxy
    {
        private readonly OnDatePicked _onDatePicked;

        public OnDateSetListenerPoxy(OnDatePicked onDatePicked)
            : base("android.app.DatePickerDialog$OnDateSetListener")
        {
            _onDatePicked = onDatePicked;
        }

        void onDateSet(AndroidJavaObject view, int year, int month, int dayOfMonth)
        {
            _onDatePicked(year, month, dayOfMonth);
        }
    }

    public class OnTimeSetListenerPoxy : AndroidJavaProxy
    {
        private readonly OnTimePicked _onTimePicked;

        public OnTimeSetListenerPoxy(OnTimePicked onTimePicked)
            : base("android.app.TimePickerDialog$OnTimeSetListener")
        {
            _onTimePicked = onTimePicked;
        }

        void onTimeSet(AndroidJavaObject view, int hourOfDay, int minute)
        {
            _onTimePicked(hourOfDay, minute);
        }
    }


}
#endif
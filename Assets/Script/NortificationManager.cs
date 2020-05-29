using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;

public class NortificationManager : MonoBehaviour
{
    
    public void CreateNortificationChannel()
    {
        var c = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.High, 
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);
    }

    public void SendNortification(string title,string text,double n)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.FireTime = System.DateTime.Now.AddSeconds(n);

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
        print("New Nortification");
    }
}

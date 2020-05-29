using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;


public class MainSaveFiledetails : MonoBehaviour
{
    public static bool isLogIn = false;

    public static string password = "";
    public static string email = "";
    public static bool remember_me;
    public static string pu_filenames = " ";
    public static string pu_fileCallname = " ";
    public static string pr_filenames = " ";
    public static string pr_fileCallname = " ";

    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/files.dat");
        Files_data data = new Files_data();

        data.RememberMe = remember_me;
        data.Password = Encrypt(password);
        data.Email = Encrypt(email);
        data.Pu_FilesNames =Encrypt(pu_filenames);
        data.Pu_FilesCallNames = Encrypt(pu_fileCallname);
        data.Pr_FilesNames = Encrypt(pr_filenames);
        data.Pr_FilesCallNames = Encrypt(pr_fileCallname);

        bf.Serialize(file, data);
        file.Close();
    }

    public static void Load()
    {

        if (File.Exists(Application.persistentDataPath + "/files.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/files.dat", FileMode.Open);
            Files_data data = (Files_data)bf.Deserialize(file);

            remember_me = data.RememberMe;
            password = Decrypt(data.Password);
            email = Decrypt(data.Email);
            pu_filenames = Decrypt(data.Pu_FilesNames);
            pu_fileCallname = Decrypt(data.Pu_FilesCallNames);
            pr_filenames = Decrypt(data.Pr_FilesNames);
            pr_fileCallname = Decrypt(data.Pr_FilesCallNames);

            file.Close();

            if (remember_me == true)
                isLogIn = true;

        }
        else
        {
            //saving first time
            pu_filenames = "Pu_Note:info.dat;";
            pu_fileCallname = "Pu_Note:Quick Public Note;";
            pr_filenames = "Pr_Note:pr_info1.dat;";
            pr_fileCallname = "Pr_Note:Quick Private Note;";
            saveload.Save();
        }
    }

    #region Encrypt and Decrypt

    private static string hash = "9452@abc";

    public static string Encrypt(string input)
    {
        byte[] data = UTF8Encoding.UTF8.GetBytes(input);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                ICryptoTransform tr = trip.CreateEncryptor();
                byte[] results = tr.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(results, 0, results.Length);
            }
        }
    }

    public static string Decrypt(string input)
    {
        byte[] data = Convert.FromBase64String(input);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                ICryptoTransform tr = trip.CreateDecryptor();
                byte[] results = tr.TransformFinalBlock(data, 0, data.Length);
                return UTF8Encoding.UTF8.GetString(results);
            }
        }
    }

    #endregion
}


[Serializable]
class Files_data
{
    public string Password;
    public string Email;
    public bool RememberMe;
    public string Pr_FilesNames;
    public string Pr_FilesCallNames;
    public string Pu_FilesNames;
    public string Pu_FilesCallNames;
}
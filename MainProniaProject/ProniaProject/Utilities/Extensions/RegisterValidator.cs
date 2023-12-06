using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProniaProject.Utilities.Extensions;
using ProniaProject.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ProniaProject.Utilities.Extensions
{
    public static class RegisterValidator
    {
        public static bool IsEmailValid(this string userVM)
        {

            Regex regex = new Regex("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
            bool result = regex.Match(userVM).Success;
            return result; 
        }
        public static string Capitalize(this string name)
        {
            string[] strings = name.Split(' ');
            for (int i = 0; i < strings.Length; i++)
            {
                if (!string.IsNullOrEmpty(strings[i]))
                {
                    strings[i] = char.ToUpper(strings[i][0]) + strings[i].Substring(1);
                }
            }
            return string.Join(" ", strings);
        }
        public static bool IsDigit(this string name)
        {
            bool result = false;
            foreach (var item in name)
            {
                result = Char.IsDigit(item);
            }
            return result;
        }
        public static bool IsSymbol(this string name)
        {
            bool result = false;
            foreach (var item in name)
            {
                result = Char.IsSymbol(item);
            }
            return result;
        }
    }
}



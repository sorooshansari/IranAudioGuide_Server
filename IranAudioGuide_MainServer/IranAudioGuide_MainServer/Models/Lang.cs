using System;
namespace IranAudioGuide_MainServer.Models
{

    public static class LangService
    {

        public static EnumLang FindByName( string str)
        {
            foreach (EnumLang val in Enum.GetValues(typeof(EnumLang)))
                if (val.ToString() == str)
                    return val;
            return EnumLang.en;
        }
        public static int GetId(EnumLang en)
        {
            return (int)en;
        }
        public static int GetId(string Name)
        {
            foreach (EnumLang val in Enum.GetValues(typeof(EnumLang)))
                if (val.ToString() == Name)
                    return  (int) val;
            return (int) EnumLang.en;
        }
        public static string GetNameById(int id)
        {
            return Enum.GetName(typeof(EnumLang), id);
        }
    }

}
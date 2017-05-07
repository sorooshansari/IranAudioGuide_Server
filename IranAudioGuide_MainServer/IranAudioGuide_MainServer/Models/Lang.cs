using System;
namespace IranAudioGuide_MainServer.Models
{

    [Flags]
    public enum LangEnum
    {
        en = 1,
        fa = 2,

    }
    public static class LangService
    {

        public static LangEnum FindByName( string str)
        {
            foreach (LangEnum val in Enum.GetValues(typeof(LangEnum)))
                if (val.ToString() == str)
                    return val;
            return LangEnum.en;
        }
        public static int GetId(LangEnum en)
        {
            return (int)en;
        }
        public static int GetId(string Name)
        {
            foreach (LangEnum val in Enum.GetValues(typeof(LangEnum)))
                if (val.ToString() == Name)
                    return  (int) val;
            return (int) LangEnum.en;
        }
        public static string GetNameById(int id)
        {
            return Enum.GetName(typeof(LangEnum), id);
        }
    }

}
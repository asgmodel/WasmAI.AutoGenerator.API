using AutoGenerator.Config;
using AutoMapper;
using Newtonsoft.Json;

namespace AutoGenerator.Helper.Translation
{




    public interface IRoleCase
    {
        void Add(string key, Func<bool, object> func);
        void Update(string key, Func<bool, object> func);
        bool Remove(string key);
        bool CheckRole(string key);
        bool AllRolesPass();
        Dictionary<string, bool> PreviewRoles();
        List<string> ListRoles();
    }



    public class RoleCase : IRoleCase
    {
        private readonly Dictionary<string, Func<bool, object>> _roles;

        public RoleCase()
        {
            _roles = new Dictionary<string, Func<bool, object>>();
        }

        public void Add(string key, Func<bool, object> func)
        {
            if (_roles.ContainsKey(key))
                throw new ArgumentException($"Role '{key}' already exists.");

            _roles[key] = func;
        }

        public void Update(string key, Func<bool, object> func)
        {
            if (!_roles.ContainsKey(key))
                throw new KeyNotFoundException($"Role '{key}' does not exist.");

            _roles[key] = func;
        }

        public bool Remove(string key)
        {
            return _roles.Remove(key);
        }

        public bool CheckRole(string key)
        {
            return _roles.ContainsKey(key) && _roles[key](true) != null;
        }

        public bool AllRolesPass()
        {
            foreach (var role in _roles.Values)
            {
                if (role(true) == null)
                    return false;
            }

            return true;
        }

        public Dictionary<string, bool> PreviewRoles()
        {
            var result = new Dictionary<string, bool>();
            foreach (var role in _roles)
            {
                result[role.Key] = role.Value(false) != null;
            }

            return result;
        }

        public List<string> ListRoles()
        {
            return new List<string>(_roles.Keys);
        }
    }

    public class TranslationView<T>
    {
        public T Value { get; set; }
        public string LG { get; set; }
    }

    public class TranslationData : ITranslationData
    {

        public Dictionary<string, string>? Value { get; set; }

        public string? ToFilter(string? lg)
        {

            if (Value == null || !Value.ContainsKey(lg))
                return null;

            return Value[lg];



        }


        public static implicit operator TranslationData(string text)
        {
            return HelperTranslation.ConvertToTranslationData(text);
        }
    }



    public class HelperTranslation
    {



        public static readonly string KEYLG = "lg";
        public static Dictionary<string, string>? ConvertTextToTranslationData(string? textTranslation)
        {

            try
            {
                return  JsonConvert.DeserializeObject<Dictionary<string, string>>(textTranslation);

            }
            catch
            {
                if (!string.IsNullOrEmpty(textTranslation))
                    return new() { { "en", textTranslation } };
            }

            return new();
                
        }


        public static TranslationData ConvertToTranslationData(string textTranslation)
        {
            return new TranslationData
            {
                Value = ConvertTextToTranslationData(textTranslation)
            };
        }

        public static string CoverTranslationDataToText(TranslationData translationData)
        {
            if (translationData == null || translationData.Value == null)
                return string.Empty;
            return JsonConvert.SerializeObject(translationData.Value);
        }


        public static string getTranslationValueByLG(string textTranslation, string lg)
        {
            if (string.IsNullOrEmpty(textTranslation))
                return string.Empty;
            return ConvertTextToTranslationData(textTranslation)[lg];
        }

        public static string ConvertTranslationDataToText(Dictionary<string, string> translationData)
        {
            return JsonConvert.SerializeObject(translationData);
        }





        public static object MapToTranslationData<S, D>(S src, D dest, object destMember, ResolutionContext context)
        {



            // Ensure that neither src nor dest are null
            if (src == null || dest == null)
            {
                return destMember;
            }

            // Try to get the property name in model based on the value of destMember
            var name = dest.GetType().GetProperties()
                            .FirstOrDefault(p => p.GetValue(dest) == destMember)?.Name;




            if (string.IsNullOrEmpty(name))
            {
                return destMember; // If property name is not found or it's null, return destMember as is
            }

            // Try to get the property value from src using the identified name
            var item = src.GetType().GetProperty(name)?.GetValue(src);






            if (item is ITranslationData && destMember is ITranslationData)
            {
                return item;
            }
            // Check if item is of type ITranslationData
            if (item is ITranslationData translationData)
            {



                Dictionary<string, object>? items = new Dictionary<string, object>();
                context.TryGetItems(out items);

                if (items != null && items.ContainsKey(KEYLG))
                    return translationData.ToFilter((string)items[KEYLG]);

                else

                    return ConvertTranslationDataToText(translationData.Value); // Convert ITranslationData to text


            }            // Check if destMember is of type ITranslationData
            else if (destMember is ITranslationData)
            {
                // Convert the string to ITranslationData
                return ConvertToTranslationData((string)item);
            }

            var filterlg = dest.GetType().GetProperties().Where(t => GlobalAttribute.CheckFilterLGEnabled(t.PropertyType)).FirstOrDefault();
            if (filterlg != null)
            {
                var lg = filterlg.GetValue(dest);


                return getTranslationValueByLG(item.ToString(), lg.ToString());


            }
            // If item is not null, return its value
            if (item != null)
            {
                return item;
            }

            // If destMember is not null and types match, return item
            if (destMember != null && item?.GetType() == destMember?.GetType())
            {
                return item;
            }

            // In case none of the above conditions are met, return destMember as is
            return destMember;
        }

        public static void MapToProcessAfter<S, D>(S src, D dest, ResolutionContext context)
        {



            var srcitems = src.GetType().GetProperties().Where(t => typeof(ITranslationData).IsAssignableFrom(t.PropertyType) && t.PropertyType.IsClass);
            foreach (var srcitem in srcitems)
            {

                var kname = srcitem.Name;

                if (string.IsNullOrEmpty(kname))
                {
                    continue;
                }

                var item = src.GetType().GetProperty(kname)?.GetValue(src) as ITranslationData;
                if(item==null)
                    continue;
                var destitem = dest.GetType().GetProperty(kname);


                if (!typeof(ITranslationData).IsAssignableFrom(destitem.PropertyType))
                {





                    Dictionary<string, object>? items = new Dictionary<string, object>();
                    context.TryGetItems(out items);
                    if (items != null && items.ContainsKey(KEYLG))
                        destitem.SetValue(dest, item.ToFilter((string)items[KEYLG]));

                    else

                        destitem.SetValue(dest, ConvertTranslationDataToText(item.Value)); // Convert ITranslationData to text


                }





            }

        }
        public static void MapToProcessAfterT<S, D>(S src, D dest, ResolutionContext context)
        {



            var destitems = dest.GetType().GetProperties().Where(t => t is ITranslationData || GlobalAttribute.CheckFilterLGEnabled(t.PropertyType));
            foreach (var destitem in destitems)
            {

                var kname = destitem.Name;

                if (string.IsNullOrEmpty(kname))
                {
                    continue;
                }


                var item = src.GetType().GetProperty(kname)?.GetValue(src);
                var filterlg = dest.GetType().GetProperties().Where(t => GlobalAttribute.CheckFilterLGEnabled(t.PropertyType)).FirstOrDefault();

                var lg = filterlg.GetValue(dest);

                if (item is ITranslationData translationData && !(destitem is ITranslationData))
                {





                    Dictionary<string, object>? items = new Dictionary<string, object>();
                    context.TryGetItems(out items);
                    if (items != null && items.ContainsKey(KEYLG))
                        destitem.SetValue(dest, translationData.ToFilter((string)items[KEYLG]));

                    else

                        destitem.SetValue(dest, ConvertTranslationDataToText(translationData.Value)); // Convert ITranslationData to text


                }
                else if (lg != null)
                {


                    destitem.SetValue(dest, getTranslationValueByLG(item.ToString(), lg.ToString()));
                }
                else if (destitem is ITranslationData && item is string)
                {
                    // Convert the string to ITranslationData
                    destitem.SetValue(dest, ConvertToTranslationData((string)item));
                }






            }

        }
    }
}

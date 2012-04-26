using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Sparkle_Configuration
{
    sealed class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) })); }
        }

        #region Nested type: DynamicJsonObject

        public sealed class DynamicJsonObject : DynamicObject
        {
            private readonly IDictionary<string, object> _dictionary;

            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                _dictionary = dictionary;
            }

            public override string ToString()
            {
                var sb = new StringBuilder("{");
                ToString(sb);
                return sb.ToString();
            }

            public void ToString(StringBuilder sb)
            {
                var firstInDictionary = true;
                foreach (var pair in _dictionary)
                {
                    if (!firstInDictionary)
                        sb.Append(",\n");
                    firstInDictionary = false;
                    var value = pair.Value;
                    var name = pair.Key;
                    if (value is string)
                    {
                        sb.AppendFormat("\"{0}\": \"{1}\"", name, value);
                    }
                    else if (value is IDictionary<string, object>)
                    {
                        sb.AppendFormat("\"{0}\": ", name);
                        sb.Append("{\n    ");
                        StringBuilder a = new StringBuilder();
                        new DynamicJsonObject((IDictionary<string, object>)value).ToString(a);
                        a.Replace(",\n", ",\n    ");
                        a.Replace("\",\n\"", "\",\n    \"");

                        sb.Append(a);
                    }
                    else if (value is ArrayList)
                    {
                        Console.WriteLine("test: " + value + " " + name);
                        sb.Append(name + ": [");
                        var firstInArray = true;
                        foreach (var arrayValue in (ArrayList)value)
                        {
                            if (!firstInArray)
                            {
                                sb.Append(",");
                            }
                            else
                            {
                                sb.Append("");
                            }
                            firstInArray = false;
                            if (arrayValue is IDictionary<string, object>)
                            {
                                sb.AppendFormat("\"{0}\": ", name);
                                sb.Append("{\n");
                                StringBuilder a = new StringBuilder();
                                new DynamicJsonObject((IDictionary<string, object>)value).ToString(a);
                                a.Replace("\",\n\"", "\",\n\t\"");
                                sb.Append(a);
                            }
                            else if (arrayValue is string)
                            {
                                sb.AppendFormat("\"{0}\"", arrayValue);
                            }
                            else
                            {
                                sb.AppendFormat("{0}", arrayValue);
                            }
                        }
                        sb.Append("]");
                    }
                    else if (value is Boolean)
                    {
                        // needed otherwise it will output True/False, not valid JSON
                        if (value.Equals(true))
                        {
                            sb.AppendFormat("\"{0}\": {1}", name, "true");
                        }
                        else
                        {
                            sb.AppendFormat("\"{0}\": {1}", name, "false");
                        }
                    }
                    else
                    {
                        sb.AppendFormat("\"{0}\": {1}", name, value);
                    }
                }
                sb.Append("}");
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (binder == null)
                {
                    result = null;
                    return true;
                }

                if (!_dictionary.TryGetValue(binder.Name, out result))
                {
                    // return null to avoid exception.  caller can check for null this way...
                    result = null;
                    return true;
                }

                var dictionary = result as IDictionary<string, object>;
                if (dictionary != null)
                {
                    result = new DynamicJsonObject(dictionary);
                    return true;
                }

                var arrayList = result as ArrayList;
                if (arrayList != null && arrayList.Count > 0)
                {
                    if (arrayList[0] is IDictionary<string, object>)
                        result = new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)));
                    else
                        result = new List<object>(arrayList.Cast<object>());
                }

                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object result)
            {
                if (_dictionary.ContainsKey(binder.Name))
                {
                    _dictionary.Remove(binder.Name);
                }
                _dictionary.Add(binder.Name, result);
                return true;
            }
        }

        #endregion
    }
}
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace PlannerNameSpace
{
    public class SerializationUtils
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reads a string that is assumed to be serialized text from the specified property 
        /// field of the given StoreItem, and attempts to unserialized that text to an object
        /// of the specified type.
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static T UnserializeFromItemProperty<T>(StoreItem item, string propName, [CallerMemberName] string publicPropName = "") where T : new()
        {
            T unserializedObject;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            string objectText = item.GetStringValue(propName, publicPropName);
            if (!string.IsNullOrWhiteSpace(objectText))
            {
                StringReader stringReader = new StringReader(objectText);

                try
                {
                    unserializedObject = (T)serializer.Deserialize(stringReader);
                }

                catch
                {
                    unserializedObject = new T();
                }
            }
            else
            {
                unserializedObject = new T();
            }

            return unserializedObject;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Serializes the object of the given type, stores the serialized text in the
        /// specified property field of the given StoreItem, and commits the change to the
        /// store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void SerializeToItemProperty<T>(StoreItem item, string propName, T objectToSerialize, [CallerMemberName] string publicPropName = "")
        {
            item.BeginSaveImmediate();

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, objectToSerialize);
            string serializedText = stringWriter.ToString();
            item.SetStringValue(propName, serializedText, publicPropName);

            item.SaveImmediate();
        }

    }
}
